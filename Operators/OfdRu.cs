using System;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using RetailFixer.Data;
using RetailFixer.Enums;
using RetailFixer.Interfaces;
using static RetailFixer.Utils.OperatorAddon;

namespace RetailFixer.Operators;

public sealed class OfdRu : IOperator
{
    public string Name => "OFDru";
    public async Task<bool> CheckInfo(string token)
    {
        try
        {
            ArgumentException.ThrowIfNullOrEmpty(Settings.Info.Vatin);
            ArgumentException.ThrowIfNullOrEmpty(Settings.Info.RegId);
            ArgumentException.ThrowIfNullOrEmpty(token);
            var uri = new Uri(
                $"https://ofd.ru/api/integration/v2" +
                $"/inn/{Settings.Info.Vatin}/kkts?" +
                $"KKTRegNumber={Settings.Info.RegId}&" +
                $"AuthToken={token}");
            var resp = await App.Http.GetAsync(uri);
            var content = JsonNode.Parse(await resp.Content.ReadAsStringAsync());
            if (content is null)
                return FailCheck("Получен некорректный ответ запроса");
            if (content!["Status"] is null)
                return FailCheck("Используемый токен недействителен");
            if (content["Status"]!.GetValue<string>() == "Failed")
                return FailCheck("Полученный ИНН компании от ККМ не верен!");
            return content["Data"]?.AsArray().Count > 0 ||
                   FailCheck("Данный ККМ отсутствует в ОФД (не зарегистрирован)");
        }
        catch (Exception e)
        {
            return FailCheck($"Возникла проблема: {e.Message}");
        }
    }

    public async Task PullReceipts()
    {
        for (
            var d = Settings.SearchPeriod.from; 
            d <= Settings.SearchPeriod.to; 
            d += TimeSpan.FromDays(1))
            await PullReceipts(d);
    }

    private static async Task PullReceipts(DateTime day)
    {
        var uri = new Uri(
            $"https://ofd.ru/api/integration/v2" +
            $"/inn/{Settings.Info.Vatin}/" +
            $"kkt/{Settings.Info.RegId}/receipts-info?" +
            $"dateFrom={day:yyyy-MM-dd}T00:00:00&" +
            $"dateTo={day:yyyy-MM-dd}T23:59:59&" +
            $"AuthToken={Settings.Ofd.AuthToken}");
        var resp = await App.Http.GetAsync(uri);
        var content = JsonNode.Parse(await resp.Content.ReadAsStringAsync());
        if (content["Data"] is not JsonArray receipts) return;
        foreach(var info in receipts)
        {
            var receipt = new Receipt
            {
                Number = uint.Parse(info["DocNumber"].GetValue<string>()),
                DateTime = info["DocDateTime"].GetValue<DateTime>(),
                Items = [],
                Payment = (info["CashSumm"].GetValue<uint>(),
                    info["ECashSumm"].GetValue<uint>(),
                    info["CreditSumm"].GetValue<uint>(),
                    info["PrepaidSumm"].GetValue<uint>()),
                Subject = PullSubjects.Operator,
                OpCode = ConvertOpcode(info["OperationType"].GetValue<string>())
            };
            foreach (var pos in info["Items"].AsArray())
            {
                receipt.Items.Add(new Position
                {
                    Name = pos["Name"].GetValue<string>(),
                    Price = pos["Price"].GetValue<uint>(),
                    Total = pos["Total"].GetValue<uint>(),
                    Quantity = (uint)Math.Round(pos["Quantity"].GetValue<double>()*1000,0),
                    Measure = (MeasureUnit)pos["ProductUnitOfMeasure"].GetValue<byte>(),
                    Type = (PositionType)pos["SubjectType"].GetValue<byte>(),
                    PaymentType = (PaymentMethod)pos["CalculationMethod"].GetValue<byte>(),
                    TaxRate = pos["NDS_Rate"].GetValue<byte>(),
                });
            }
            App.Receipts.Add(receipt);
        }
    }

    private static byte ConvertOpcode(string op) =>
        (byte)((op.EndsWith("ncome") ? 1 : 3) + (op.StartsWith("Refund") ? 1 : 0));

    private static byte ConvertTaxRate(byte src) =>
        src switch
        {
            1 => 20,
            2 => 10,
            3 => 120,
            4 => 110,
            5 => 0,
            6 => 255,
            _ => throw new ArgumentOutOfRangeException(nameof(src)) // todo При появлении БАГа 
        };
}