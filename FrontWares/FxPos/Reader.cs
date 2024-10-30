using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using RetailFixer.Data;
using RetailFixer.Enums;
using Receipt = RetailFixer.Data.Receipt;

namespace RetailFixer.FrontWares.FxPos;

public sealed partial class FxPos
{
    private List<Receipt> ReadReceipts(DateTime from, DateTime to)
    {
        using SQLiteCommand cmd = 
            new(Sql.Receipts(DateOnly.FromDateTime(from), DateOnly.FromDateTime(to)), _connection);
        using var reader = cmd.ExecuteReader();
        var receipts = new List<Receipt>(10);
        while (reader.Read())
        {            
            var opcode = reader.GetInt16(2);
            var receipt = new Receipt
            {
                Number = (uint)reader.GetInt64(4),
                OpCode = new Converters.Operation().Convert(opcode),
                DateTime = reader.GetDateTime(3),
                InnerProperties = new Dictionary<string, string>
                {
                    { "Number", $"{reader.GetInt64(1)}" },
                    { "Id", $"{reader.GetInt64(0)}" }
                },
                Subject = PullSubjects.FrontSystem,
                Items = [],
                Payment = new Payment()
            };
            receipts.Add(receipt);
        }
        return receipts;
    }
    private void ReadPayment(ref List<Receipt> receipts)
    {
        using SQLiteCommand cmd = new(Sql.Payments(receipts.Select(k=>k["Id"])), _connection);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var index = receipts.FindIndex(k => int.Parse(k["Id"]) == reader.GetInt32(0));
            var sum = (uint)(reader.GetInt32(2) / 100);
            switch (reader.GetInt32(1))
            {
                case 0:
                    receipts[index].Payment += new Payment(cash: sum);
                    break;
                case 1:
                    receipts[index].Payment += new Payment(ecash: sum);
                    break;
            }
        }
    }
    private void ReadItems(ref List<Receipt> receipts)
    {
        using SQLiteCommand cmd = new(Sql.Items(receipts.Select(k => k["Id"])), _connection);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var index = receipts.FindIndex(k => int.Parse(k["Id"]) == reader.GetInt64(0));
            var delCode = reader.GetString(1);
            var quantity = (uint)(reader.GetInt64(2) / 1000);
            var index2 = receipts[index].Items.FindIndex(k => k["DelCode"] == delCode);
            if(index2 == -1)
                receipts[index].Items.Add(new Position
                {
                    InnerProperties = new Dictionary<string, string>{{"DelCode",delCode}},
                    Name = reader.GetString(5),
                    Price = (uint)(reader.GetInt64(3)/100),
                    Quantity = quantity,
                    Total = (uint)(reader.GetInt64(4)/100),
                    TaxRate = (byte)(reader.GetInt64(7)/1000000),
                    PaymentType = (PaymentMethod)reader.GetInt64(8),
                    Type = new Converters.PositionType().Convert((int)reader.GetInt64(6)),
                    Measure = new Converters.Measure().Convert(reader.GetInt32(9))
                });
            else
            {
                receipts[index].Items[index2].Quantity += quantity;
                receipts[index].Items[index2].Total = 
                    (uint)Math.Round(receipts[index].Items[index2].Price *
                                     (receipts[index].Items[index2].Quantity / 1000.0));
            }
        }
    }
    private void ReadDeletedItems(ref List<Receipt> receipts)
    {
        using SQLiteCommand cmd = new(Sql.DeletedItems(receipts.Select(k => k["Id"])), _connection);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var index = receipts.FindIndex(k => int.Parse(k["Id"]) == reader.GetInt64(0));
            var index2 = receipts[index].Items.FindIndex(k => k["DelCode"] == reader.GetString(1));
            if(index2 == -1) return;
            receipts[index].Items[index2].Quantity -= (uint)(reader.GetInt64(2)/ 1000);
            if (receipts[index].Items[index2].Quantity <= 0) receipts[index].Items.RemoveAt(index2);
            else
                receipts[index].Items[index2].Total = 
                    (uint)Math.Round(receipts[index].Items[index2].Price *
                                     (receipts[index].Items[index2].Quantity / 1000.0));
        }
    }
}

file static class Sql
{
    public static string Items(IEnumerable<string> ids) =>
        "select tr.documentId as docId, tr.wareCode as code, tr.quantity as quantity, tr.price as price, " +
        "tr.totalWithDiscount as total, w.text as name, w.item_type as type, (select value from taxRate where " +
        "code = tr.taxRate_code) as tax, w.payment_type as payment, w.measure as measure from transactions tr, " +
        $"ware w where tr.trType = 11 and tr.wareCode = w.code and tr.documentId in ({string.Join(", ", ids)});";

    public static string Receipts(DateOnly begin, DateOnly end) =>
        $"select d.id as id, d.documentNumber as innerNumber, d.ecrDocumentType as type, d.closeDateTime as date, "+
        $"tr.quantity as number from documents d, transactions tr where d.closeDateTime between "+
        $"\"{begin:O}T00:00:00\" and \"{end:O}T23:59:59\" and d.ecrDocumentType in (0, 1, 25, 26) "+
        $"and d.documentState = 1 and tr.documentId = d.id and tr.trType = 45;";

    public static string Payments(IEnumerable<string> ids) =>
        $"select documentId, (select operation from payment where code = tr.wareCode) as type, total from"+
        $" transactions tr where documentId in ({string.Join(", ", ids)}) and trType = 43;";

    public static string DeletedItems(IEnumerable<string> ids) =>
        $"select documentId, wareCode, quantity from transactions where documentId in "+
        $"({string.Join(", ", ids)}) and trType = 12;";
    
}