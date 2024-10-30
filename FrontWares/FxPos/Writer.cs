using System;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using RetailFixer.Data;
using RetailFixer.Enums;

namespace RetailFixer.FrontWares.FxPos;

public sealed partial class FxPos
{
    #region Запросы

    /// <summary>
    /// Создание записи в таблице DOCUMENTS
    /// </summary>
    /// <param name="tr">Транкзация SQLite</param>
    /// <param name="data">Информация о чеке</param>
    private void AddDocument(SQLiteTransaction tr, Receipt data)
    {
        var dt = $"{data.DateTime:yyyy-MM-ddTHH:mm:ss}.000";
        var operation = new Converters.Operation().ConvertBack(data.OpCode);
        var sum = data.Items.Sum(i => i.Total);
        var sql = 
            $"insert into documents(id, documentNumber, documentState, openDateTime, closeDateTime, ecrDocumentType, "+
            $"ecrSession, ecrDepartment, posId, totalWithDiscount, isSaved, commentCode, educationMode, total, uid, "+
            $"externalOperation, printGroup, round_context, is_loyalty_closed, correction_type, uid_by_printGroup, "+
            $"open_user, close_user, document_type) values ({++lastDocId}, {++lastDocNumb}, 1, \"{dt}\", \"{dt}\", "+
            $"{operation}, {lastDocSession}, -1, 1, {data.TotalSum}00, 0, \"\", 0, {sum}00, {Guid.NewGuid():B}, 0, "+
            $"(select code from print_groups limit 1), \"{lastDocContext}\", 1, \"{lastDocCorrect}\", "+
            $"\"{GenerateUidByPrintGroup(data.OpCode)}\", \"{operators[data.Operator]}\", "+
            $"\"{operators[data.Operator]}\", \"{docKinds[data.OpCode]}\");";
        using var cmd = new SQLiteCommand(sql, _connection, tr);
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Создание записи транкзации №42 в таблице TRANSACTIONS
    /// </summary>
    /// <param name="tr">Транкзация SQLite</param>
    /// <param name="data">Информация о чеке</param>
    private void OpenDocument(SQLiteTransaction tr, Receipt data)
    {
        var quantity = data.Items.Sum(i => i.Quantity);
        var sum = data.Items.Sum(i => i.Total);
        var sql =
            $"insert into transactions(id, trcount, documentid, trtype, trhour, trdatetime, totalwithdiscount, "+
            $"total, quantity, info, authorizationindex, printgroup, user_code) values ({++lastTrId}, "+
            $"{++lastTrCount}, {lastDocNumb}, 42, {data.DateTime:H}, \"{data.DateTime:yyyy-MM-ddTHH:mm:ss}.000\", "+
            $"{sum}00, {data.TotalSum}00, {quantity}000, \"\", -1, \"{docKinds[data.OpCode]}\", "+
            $"\"{operators[data.Operator]}\");";
        using var cmd = new SQLiteCommand(sql, _connection, tr);
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Создание записей транкзаций №11, №14 и №16 в таблице TRANSACTIONS
    /// </summary>
    /// <param name="tr">Транкзация SQLite</param>
    /// <param name="p">Информация о позиции</param>
    /// <param name="dt">Дата и время проведения действия</param>
    /// <param name="user">ФИО кассира выполнившего операцию</param>
    private void AddPosition(SQLiteTransaction tr, Position p, DateTime dt, string user)
    {
        var ware = GetWareByName(p.Name);
        foreach (var trtype in new[] {11, 14, 16})
        {
            var sql =
                $"insert into transactions(id, trcount, documentid, trtype, trhour, trdatetime, warecode, waremark, " +
                $"price, pricewithdiscount, quantity, total, totalwithdiscount, barcode, factor, roundedtotal, " +
                $"authorizationindex, printgroup, positiondiscount, documentdiscount, isclosed, issended, bonus, " +
                $"is_gift_card, tax_section, taxgroup_code, taxrate_code, nomenclature_type, user_code) values " +
                $"({++lastTrId}, {++lastTrCount}, {lastDocNumb}, {trtype}, {dt:H}, \"{dt:yyyy-MM-ddTHH:mm:ss}.000\", " +
                $"{ware.Code}, \"{ware.Mark}\", {p.Price}00, {p.Total / p.Quantity}00, {p.Quantity}000, "+
                $"{p.Price * p.Quantity}00, {p.Total}00, {ware.Barcode}, {ware.Factor}, {p.Total}00, -1, (select code "+
                $"from print_groups limit 1), 0, 0, 1, 0, 0, {(ware.IsGiftCard ? 1 : 0)}, {ware.TaxSection}, "+
                $"{ware.TaxGroupCode}, {ware.TaxRateCode}, {ware.Type}, \"{operators[user]}\");";
            using var cmd = new SQLiteCommand(sql, _connection, tr);
            cmd.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// Создание записей транкзаций №40 и №43 в таблице TRANSACTIONS
    /// </summary>
    /// <param name="tr">Транкзация SQLite</param>
    /// <param name="data">Информация об оплате</param>
    /// <param name="dt">Дата и время проведения действия</param>
    /// <param name="user">ФИО кассира выполнившего операцию</param>
    private void Payment(SQLiteTransaction tr, Payment data, DateTime dt, string user)
    {
        var sums = new[] { data.Cash, data.ECash };
        for (var i = 0; i < 2; i++)
        {
            if (sums[i] == 0) continue;
            var sql =
                $"insert into transactions(id, trcount, documentid, trtype, trhour, trdatetime, warecode, price, " +
                $"total, pricewithdiscount, info, authorizationindex, printgroup, operation_payment, bonus, " +
                $"user_code) values ({++lastTrId}, {++lastTrCount}, {lastDocNumb}, 40, {dt:H}, " +
                $"\"{dt:yyyy-MM-ddTHH:mm:ss}.000\", {i + 1}, {sums[i]}00, {sums[i]}00, 0, \"1\", 0, \"0\", {i}, " +
                $"0, \"{operators[user]}\");";
            using (var cmd = new SQLiteCommand(sql, _connection, tr))
            {
                cmd.ExecuteNonQuery();
            }
            sql =
                $"insert into transactions(id, trcount, documentid, trtype, trhour, trdatetime, warecode, price, " +
                $"total, pricewithdiscount, info, authorizationindex, printgroup, operation_payment, bonus, " +
                $"user_code) values ({++lastTrId}, {++lastTrCount}, {lastDocNumb}, 43, {dt:H}, " +
                $"\"{dt:yyyy-MM-ddTHH:mm:ss}.000\", {i + 1}, {sums[i]}00, {sums[i]}00, 0, \"0/0/0\", 0, (select "+
                $"code from print_groups limit 1), {i}, 0, \"{operators[user]}\");";
            using (var cmd = new SQLiteCommand(sql, _connection, tr))
            {
                cmd.ExecuteNonQuery();
            }
        }
    }

    /// <summary>
    /// Создание записей транкзаций №55 в таблице TRANSACTIONS
    /// </summary>
    /// <param name="tr">Транкзация SQLite</param>
    /// <param name="data">Информация о чеке</param>
    private void CloseDocument(SQLiteTransaction tr, Receipt data)
    {
        var sql =
            $"insert into transactions(id, trcount, documnetid, trtype, trhour, trdatetime, user_code, quantity, "+
            $"total, totalwithdiscount) values ({++lastTrId}, {++lastTrCount}, {lastDocNumb}, 55, {data.DateTime:H}, "+
            $"\"{data.DateTime:yyyy-MM-ddTHH:mm:ss}.000\", \"{operators[data.Operator]}\", "+
            $"{data.Items.Sum(p=>p.Quantity)}000, {data.Items.Sum(p => p.Total)}00, "+
            $"{data.TotalSum}00);";
        using SQLiteCommand cmd = new(sql, _connection, tr);
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Создание записей транкзаций №45 в таблице TRANSACTIONS
    /// </summary>
    /// <param name="tr">Транкзация SQLite</param>
    /// <param name="data">Информация о чеке</param>
    private void CloseDocumentInDevice(SQLiteTransaction tr, Receipt data)
    {
        var sql =
            $"insert into transactions(id, trcount, documentid, trtype, trhour, trdatetime, user_code, warecode, "+
            $"price, pricewithdiscount, quantity, totalwithdiscount, waremark) values ({++lastTrId}, {++lastTrCount}, "+
            $"{lastDocNumb}, 45, {data.DateTime:H}, \"{data.DateTime:yyyy-MM-ddTHH:mm:ss}.000\", "+
            $"\"{operators[data.Operator]}\", \"{Settings.Info.RegId}\", \"{data.DateTime:yyyy-MM-ddTHH:mm:ss}\", "+
            $"\"{Settings.Info.StorageId}\", {data.Number}, {data.TotalSum}00, \"{data.FiscalSign}\");";
        using SQLiteCommand cmd = new(sql, _connection, tr);
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Создание записей транкзаций №49 в таблице TRANSACTIONS
    /// </summary>
    /// <param name="tr">Транкзация SQLite</param>
    /// <param name="total">Итоговая сумма</param>
    /// <param name="dt">Дата и время проведения действия</param>
    /// <param name="user">ФИО кассира выполнившего операцию</param>
    private void CloseDocumentInSequence(SQLiteTransaction tr, uint total, DateTime dt, string user)
    {
        var sql =
            $"insert into transactions(id, trcount, documentid, trtype, trhour, trdatetime, user_code, totalwithdiscount) "+
            $"values ({++lastTrId}, {++lastTrCount}, {lastDocNumb}, 49, {dt:H}, \"{dt:yyyy-MM-ddTHH:mm:ss}.000\", "+
            $"\"{operators[user]}\", {total}00);";
        using SQLiteCommand cmd = new(sql, _connection, tr);
        cmd.ExecuteNonQuery();
    }

    #endregion

    /// <summary>
    /// Генерация значения uids_by_printgroup для таблицы DOCUMENTS 
    /// </summary>
    private string GenerateUidByPrintGroup(ReceiptOperation operation)
    {
        var raw = new StringBuilder("���$�u�i�d�s�_�b�y�_�p�r�i�n�t�g�r�o�u�p������");
        foreach (var c in docKinds[operation])
            raw.Append($"\ufffd{c}");
        raw.Append("\ufffd\ufffd\ufffdL");
        foreach (var c in $"{Guid.NewGuid():B}")
            raw.Append($"\ufffd{c}");
        var bytes = Encoding.UTF8.GetBytes(raw.ToString());
        return Convert.ToBase64String(bytes);
    }
}