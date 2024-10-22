using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.ServiceProcess;
using System.Linq;
using RetailFixer.Data;
using RetailFixer.Enums;
using RetailFixer.Exceptions;

namespace RetailFixer.Soft;

public sealed class FxPOS
{
    #region Переменные
    private readonly SQLiteConnection _connection;
    private uint lastId = 0;
    private uint lastDocNum = 0;
    private SessionCollection sessions = new();
    #endregion

    #region Инициализация и деинициализации
    static FxPOS()
    {
        DbProviderFactories.RegisterFactory("System.Data.SQLite", SQLiteFactory.Instance);
    }
    public FxPOS()
    {
        if (!IsInstalled) throw new Exception();
        if (SQLiteFactory.Instance.CreateConnection() is not SQLiteConnection conn)
            throw new FailCreateConnectionException();
        _connection = conn;
        
    }
    ~FxPOS() => _connection.Dispose();
    #endregion

    #region Генерируемые свойства
#pragma warning disable CA1416
    private static bool IsInstalled =>
        ServiceController.GetServices().Any(i => i.ServiceName == "FrontolxPOSExchangeService");
#pragma warning restore CA1416
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
    #endregion

    #region Выгрузка

    private void PullSessions()
    {
        using SQLiteCommand cmd = new(Sql.GetSessions, _connection);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            sessions.Add((uint)reader.GetInt64(0), reader.GetDateTime(1), reader.GetDateTime(2));
    }
    
    private List<Receipt> GetReceipts(DateOnly from, DateOnly to)
    {
        using SQLiteCommand cmd = new(Sql.GetReceipts(from, to), _connection);
        using var reader = cmd.ExecuteReader();
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
        using SQLiteCommand cmd = new(Sql.GetPayments(receipts.Select(k=>k["Id"])), _connection);
        using var reader = cmd.ExecuteReader();
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
        using SQLiteCommand cmd = new(Sql.GetItems(receipts.Select(k => k["Id"])), _connection);
        using var reader = cmd.ExecuteReader();
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
        using SQLiteCommand cmd = new(Sql.GetDeletedItems(receipts.Select(k => k["Id"])), _connection);
        using var reader = cmd.ExecuteReader();
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

    #endregion

    #region Создание ФД
    private void CreateDocument(Receipt r)
    {
        AddDocumentArgs args = new(++lastId, ++lastDocNum,  r.DateTime, 0, (ulong)r.Items.Sum(i=>i.Total), r.TotalSum, 0);
        using var cmd = new SQLiteCommand(Sql.AddDocument(args), _connection);
        cmd.ExecuteNonQuery();
    } 
    #endregion
    
    public void OpenConnection(string path)
    {
        _connection.ConnectionString = $"Data Source = {path}";
        _connection.Open();
    }

    public void SetLast()
    {
        using SQLiteCommand cmd = new(Sql.GetLast, _connection);
        using var reader = cmd.ExecuteReader();
        if (!reader.Read())
            throw new Exception();
        lastId = (uint)reader.GetInt32(0);
        lastDocNum = (uint)reader.GetInt32(1);
    }
    public void Start(DateOnly @from, DateOnly @to)
    {
        var resp = GetReceipts(@from, @to);
        SetPayment(ref resp);
        AddPositions(ref resp);
        DelPositions(ref resp);
        App.Receipts.AddRange(resp);
    }

}

file static class Sql
{
    #region Запросы на выгрузку
    public static string GetItems(IEnumerable<string> ids) =>
        "select tr.documentId as docId, tr.wareCode as code, tr.quantity as quantity, tr.price as price, " +
        "tr.totalWithDiscount as total, w.text as name, w.item_type as type, (select value from taxRate where " +
        "code = tr.taxRate_code) as tax, w.payment_type as payment, w.measure as measure from transactions tr, " +
        $"ware w where tr.trType = 11 and tr.wareCode = w.code and tr.documentId in ({string.Join(", ", ids)});";

    public static string GetReceipts(DateOnly begin, DateOnly end) =>
        $"select d.id as id, d.documentNumber as innerNumber, d.ecrDocumentType as type, d.closeDateTime as date, "+
        $"tr.quantity as number from documents d, transactions tr where d.closeDateTime between "+
        $"\"{begin:O}T00:00:00\" and \"{end:O}T23:59:59\" and d.ecrDocumentType in (0, 1, 25, 26) "+
        $"and d.documentState = 1 and tr.documentId = d.id and tr.trType = 45;";

    public static string GetPayments(IEnumerable<string> ids) =>
        $"select documentId, (select operation from payment where code = tr.wareCode) as type, total from"+
        $" transactions tr where documentId in ({string.Join(", ", ids)}) and trType = 43;";

    public static string GetDeletedItems(IEnumerable<string> ids) =>
        $"select documentId, wareCode, quantity from transactions where documentId in "+
        $"({string.Join(", ", ids)}) and trType = 12;";

    public static string GetLast => "select id, documentNumber from documents order by id desc limit 1;";
    
    public static string GetSessions => 
        "select distinct ecrSession, min(openDateTime), max(closeDateTime) from documents group by ecrSession;";
    #endregion

    #region Запросы на добавление данных

    public static string AddDocument(AddDocumentArgs args) => 
        $"insert into documents(id, documentNumber, documentState, openDateTime, closeDateTime, ecrDocumentType, "+
        $"ecrSession, ecrDepartment, posId, total, totalWithDiscount, isSaved, educationMode, externalOperation, "+
        $"printGroup, uid, is_loyalty_closed, correction_type, uids_by_printGroup, open_user, close_user, "+
        $"document_type) values ({args.Id}, {args.DocNum}, 1, \"{args.ClosedDate:O}\", \"{args.ClosedDate:O}\", "+
        $"0, {args.SessionId}, -1, 1, {args.Total*100}, {args.TotalWithDiscount*100}, 0, 0, 0, 1, "+
        $"\"{args.Guid:B}\",1, 11242442, \"{args.CryptedGuid}\", {args.CashierId}, {args.CashierId}, 1);";

    #endregion
}


file struct AddDocumentArgs(uint id, uint docId, DateTime dt, uint sid, ulong sum, ulong total, uint userId)
{
    public readonly uint Id = id;
    public readonly uint DocNum = docId;
    public readonly DateTime ClosedDate = dt;
    public readonly uint SessionId = sid;
    public readonly ulong Total = sum;
    public readonly ulong TotalWithDiscount = total;
    public readonly Guid Guid = Guid.NewGuid();
    public readonly string CryptedGuid; // todo
    public readonly uint CashierId = userId;
}