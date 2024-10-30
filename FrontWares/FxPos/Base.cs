using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using RetailFixer.Data;
using RetailFixer.Enums;
using RetailFixer.Exceptions;
using RetailFixer.FrontWares.FxPos.Data;
using RetailFixer.Interfaces;

namespace RetailFixer.FrontWares.FxPos;

public sealed partial class FxPos: IFrontWare
{
    #region Переменные
    protected readonly SQLiteConnection _connection = 
        (SQLiteConnection)SQLiteFactory.Instance.CreateConnection()!;
    protected uint lastTrId;
    protected uint lastTrCount;
    protected uint lastDocId;
    protected uint lastDocNumb;
    protected uint lastDocSession;
    protected string lastDocContext = "";
    protected string lastDocCorrect = "";
    protected readonly Dictionary<ReceiptOperation, string> docKinds = [];
    protected readonly Dictionary<string, string> operators = [];

    public static string Name => "FxPOS 3";
    private const string ServiceName = "FrontolxPOSExchangeService";
    #endregion
    #region Свойства
    public static bool IsInstalled
    {
        get
        {
            #if WIN
            eturn ServiceController.GetServices().Any(sc => sc.ServiceName == ServiceName);
            #else
            return false;
            #endif
        }
    }

    #endregion
    #region Инициализация и деинициализации
    static FxPos()
    {
        DbProviderFactories.RegisterFactory("System.Data.SQLite", SQLiteFactory.Instance);
    }

    public void OpenConnection()
    {
        _connection.ConnectionString = $"Data Source = {Settings.Soft.PathMainBase}";
        _connection.Open();

        #region Информация о последнем документе
        var sql =
            "select id, documentnumber, ecrsession, round_context, correction_type from documents " +
            "where ecrdocumenttype in (0,1,25,26) order by id desc limit 1;";
        using (SQLiteCommand cmd = new(sql, _connection))
        {
            using (var reader = cmd.ExecuteReader())
            {
                if (!reader.Read()) throw new NotFoundDataException();
                lastDocId = (uint)reader.GetInt64(0);
                lastDocNumb = (uint)reader.GetInt64(1);
                lastDocSession = (uint)reader.GetInt64(2);
                lastDocContext = reader.GetString(3);
                lastDocCorrect = reader.GetString(4);
            }
        }
        #endregion
        #region Информация о последней транкзации
        sql = "select id, trcount from TRANSACTIONS order by id desc limit 1;";
        using (SQLiteCommand cmd = new(sql, _connection))
        {
            using (var reader = cmd.ExecuteReader())
            {
                if (!reader.Read()) throw new NotFoundDataException();
                lastTrId = (uint)reader.GetInt64(0);
                lastTrCount = (uint)reader.GetInt64(1);
            }
        }
        #endregion
        #region Информация о видах документа
        sql = "select ecrreceipttype, code from dockind where ecrreceipttype in (0,1,25,26)";
        using (SQLiteCommand cmd = new(sql, _connection))
        {
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var opcode = new Converters.Operation().Convert(reader.GetInt32(0));
                    docKinds.Add(opcode, reader.GetString(1));
                }
                if(docKinds.Count == 0) throw new NotFoundDataException();
            }
        }
        #endregion
        #region Информация о кассирах
        using (SQLiteCommand cmd = new("select name, code from user;", _connection))
        {
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                    operators.Add(reader.GetString(0), reader.GetString(1));
                if(operators.Count == 0) throw new NotFoundDataException();
            }
        }
        #endregion
    }
    public void Dispose()
    {
        _connection.Dispose();
    }
    #endregion
    
    private Ware GetWareByName(string name)
    {
        var sql =
            $"select w.code, mark, (select barcode from barcode where WAREID = w.ID), r.SECTION, r.CODE, (select code "+
            $"from taxgroup where id = w.taxgroupid), w.IS_GIFT_CARD, w.product_TYPE from ware as w, TAXGRPRT as t, "+
            $"taxrate as r where w.text = \"{name}\" and t.TAXGROUPID == w.TAXGROUPID and t.TAXRATEID == r.id order "+
            $"by id desc;";
        using SQLiteCommand cmd = new(sql, _connection);
        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) throw new NotFoundDataException();
        return new Ware(reader);
    }

    public List<Receipt> PullReceipts()
    {
        var resp = ReadReceipts(Settings.SearchPeriod.Start.Date, Settings.SearchPeriod.End.Date);
        ReadPayment(ref resp);
        ReadItems(ref resp);
        ReadDeletedItems(ref resp);
        return resp;
        //App.Receipts.AddRange(resp);
    }

    public void PushReceipt(Receipt fd)
    {
        using var tr = _connection.BeginTransaction();
        try
        {
            AddDocument(tr, fd);
            OpenDocument(tr, fd);
            foreach(var pos in fd.Items)
                AddPosition(tr, pos, fd.DateTime, fd.Operator);
            Payment(tr, fd.Payment, fd.DateTime, fd.Operator);
            CloseDocumentInSequence(tr, fd.TotalSum, fd.DateTime, fd.Operator);
            CloseDocumentInDevice(tr, fd);
            CloseDocument(tr, fd);
        }
        catch
        {
            tr.Rollback();
        }
    }
}