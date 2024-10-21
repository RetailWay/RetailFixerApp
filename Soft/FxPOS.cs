using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.ServiceProcess;
using System.Linq;
using System.Resources;
using System.Text;
using RetailFixer.Data;
using RetailFixer.Enums;
using RetailFixer.Exceptions;

namespace RetailFixer.Soft;

public sealed class FxPOS: IDisposable
{
    private static class Sql
    {
        /// <summary>
        /// Возвращает информацию о позициях:<br/>
        /// - Номер документа<br/>
        /// - Код<br/>
        /// - Кол-во<br/>
        /// - Цена (10000 = 1 р.)<br/>
        /// - Стоимость (10000 = 1 р.)<br/>
        /// - Название<br/>
        /// - Тип позиции<br/>
        /// - Ставка НДС<br/>
        /// - Способ оплаты<br/>
        /// - Единица измерения<br/>
        /// </summary>
        public static string GetItems =>
            "select tr.documentId as docId, tr.wareCode as code, tr.quantity as quantity, tr.price as price, " +
            "tr.totalWithDiscount as total, w.text as name, w.item_type as type, (select value from taxRate where " +
            "code = tr.taxRate_code) as tax, w.payment_type as payment, w.measure as measure from transactions tr, " +
            "ware w where tr.trType = 11 and tr.wareCode = w.code and tr.documentId in ({0});";

        public static string GetReceipts =>
            "select d.id as id, d.documentNumber as innerNumber, d.ecrDocumentType as type, d.closeDateTime as date, "+
            "tr.quantity as number from documents d, transactions tr where d.closeDateTime between \"{0}\" and "+
            "\"{1}\" and d.ecrDocumentType in (0, 1, 25, 26) and d.documentState = 1 and "+
            "tr.documentId = d.id and tr.trType = 45;";

        public static string GetPayments =>
            "select documentId, (select operation from payment where code = tr.wareCode) as type, total from"+
            " transactions tr where documentId in ({0}) and trType = 43;";

        public static string GetDeletedItems =>
            "select documentId, wareCode, quantity from transactions where documentId in ({0}) and trType = 12;";
    }
    
    private readonly SQLiteConnection _connection;
    public Receipt[] Receipts;
    static FxPOS()
    {
        DbProviderFactories.RegisterFactory("System.Data.SQLite", SQLiteFactory.Instance);
    }

    private FxPOS()
    {
        if (!IsInstalled) throw new Exception();
    }
    #pragma warning disable CA1416
    private static bool IsInstalled =>
        ServiceController.GetServices().Any(i => i.ServiceName == "FrontolxPOSExchangeService");
    #pragma warning restore CA1416
    private SQLiteDataReader SendResponse(string sql, params string[] args)
    {
        sql = string.Format(sql, args);
        using SQLiteCommand cmd = new(sql, _connection);
        return cmd.ExecuteReader();
    }
    private List<Receipt> GetReceipts(DateOnly @from, DateOnly @to)
    {
        using var reader = SendResponse(Sql.GetReceipts, $"{@from:yyyy-MM-dd}T00:00:00", $"{@to:yyyy-MM-dd}T23:59:59");
        var receipts = new List<Receipt>(10);
        while (reader.Read())
        {            
            var opcode = reader.GetInt16(2);
            var receipt = new Receipt
            {
                Number = (uint)reader.GetInt64(4),
                OpCode = (byte)((opcode % 25 == 1 ? 1 : 0) + (opcode < 2 ? 1 : 3)),
                DateTime = reader.GetDateTime(3),
                InnerProperties = new Dictionary<string, string>
                {
                    { "Number", $"{reader.GetInt64(1)}" },
                    { "Id", $"{reader.GetInt64(0)}" }
                },
                Subject = PullSubjects.FxPos,
                Items = [],
                Payment = (0, 0, 0, 0)
            };
            receipts.Add(receipt);
        }
        return receipts;
    }
    private void SetPayment(ref List<Receipt> receipts)
    {
        using var reader = SendResponse(Sql.GetPayments, string.Join(", ", receipts.Select(k=>k["Id"])));
        while (reader.Read())
        {
            var index = receipts.FindIndex(k => int.Parse(k["Id"]) == reader.GetInt32(0));
            var sum = (uint)(reader.GetInt32(2) / 100);
            switch (reader.GetInt32(1))
            {
                case 0:
                    receipts[index].Payment.c = sum;
                    break;
                case 1:
                    receipts[index].Payment.e = sum;
                    break;
            }
        }
    }
    private void AddPositions(ref List<Receipt> receipts)
    {
        using var reader = SendResponse(Sql.GetItems, string.Join(", ", receipts.Select(k => k["Id"])));
        while (reader.Read())
        {
            var index = receipts.FindIndex(k => int.Parse(k["Id"]) == reader.GetInt64(0));
            var delCode = reader.GetString(1);
            var quantity = (ulong)(reader.GetInt64(2) / 1000);
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
                    Type = ConvertType(reader.GetInt64(6)),
                    Measure = ConvertMeasure(reader.GetInt64(9))
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
    private void DelPositions(ref List<Receipt> receipts)
    {
        using var reader = SendResponse(Sql.GetDeletedItems, string.Join(", ", receipts.Select(k => k["Id"])));
        while (reader.Read())
        {
            var index = receipts.FindIndex(k => int.Parse(k["Id"]) == reader.GetInt64(0));
            var index2 = receipts[index].Items.FindIndex(k => k["DelCode"] == reader.GetString(1));
            if(index2 == -1) return;
            receipts[index].Items[index2].Quantity -= (ulong)(reader.GetInt64(2)/ 1000);
            if (receipts[index].Items[index2].Quantity <= 0) receipts[index].Items.RemoveAt(index2);
            else
                receipts[index].Items[index2].Total = 
                    (uint)Math.Round(receipts[index].Items[index2].Price *
                                     (receipts[index].Items[index2].Quantity / 1000.0));
        }
    }
    private static PositionType ConvertType(long raw) =>
        raw switch
        {
            1 => PositionType.Product,
            2 => PositionType.Excise,
            3 => PositionType.Work,
            4 => PositionType.Service,
            10 or 16 => PositionType.Payment,
            12 => PositionType.Other,
            13 => PositionType.AgencyRemuneration,
            14 => PositionType.LotteryTicket,
            15 => PositionType.ResultIntellectual,
            17 => PositionType.ResortFee,
            18 => PositionType.IssueMoney,
            _ => throw new ArgumentOutOfRangeException()
        };
    private static MeasureUnit ConvertMeasure(long raw) =>
        raw switch
        {
            0 => MeasureUnit.Piece,
            1 => MeasureUnit.Gram,
            2 => MeasureUnit.Kilogram,
            3 => MeasureUnit.Ton,
            4 => MeasureUnit.Centimeter,
            5 => MeasureUnit.Decimeter,
            6 => MeasureUnit.Meter,
            7 => MeasureUnit.SquareCentimeter,
            8 => MeasureUnit.SquareDecimeter,
            9 => MeasureUnit.SquareMeter,
            10 => MeasureUnit.Milliliter,
            11 => MeasureUnit.Liter,
            12 => MeasureUnit.CubicMeter,
            13 => MeasureUnit.KilowattHour,
            14 => MeasureUnit.GKal,
            15 => MeasureUnit.Day,
            16 => MeasureUnit.Hour,
            17 => MeasureUnit.Minute,
            18 => MeasureUnit.Second,
            19 => MeasureUnit.Kilobyte,
            20 => MeasureUnit.Megabyte,
            21 => MeasureUnit.Gigabyte,
            22 => MeasureUnit.Terabyte,
            23 => MeasureUnit.Other,
            _ => throw new ArgumentOutOfRangeException()
        };
    public FxPOS(string path) : this()
    {
        var factory = (SQLiteFactory)DbProviderFactories.GetFactory("System.Data.SQLite");
        if (factory is null) throw new DbFactoryNotFoundException("System.Data.SQLite");
        _connection = (SQLiteConnection)factory.CreateConnection();
        _connection.ConnectionString = $"Data Source = {path}";
        _connection.Open();
    }
    public void Dispose() => _connection.Dispose();
    public void Start(DateOnly @from, DateOnly @to)
    {
        var resp = GetReceipts(@from, @to);
        SetPayment(ref resp);
        AddPositions(ref resp);
        DelPositions(ref resp);
        App.Receipts.AddRange(resp);
    }
}