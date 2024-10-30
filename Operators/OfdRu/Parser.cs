using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Nodes;
using Newtonsoft.Json;
using System.Threading.Tasks;
using RetailFixer.Attributes;
using RetailFixer.Data;
using RetailFixer.Enums;
using RetailFixer.Interfaces;
using static RetailFixer.Utils.OperatorAddon;

namespace RetailFixer.Operators.OfdRu;

[DevelopStatus(DevelopStatus.Testing)]
public sealed class Parser : IOperator
{
    public static string Name => "OFDru";
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
            var d = Settings.SearchPeriod.Start; 
            d <= Settings.SearchPeriod.End; 
            d += TimeSpan.FromDays(1))
            await PullReceipts(d.DateTime);
    }

    private static async Task PullReceipts(DateTime day)
    {
        var uri = new Uri(
            $"https://ofd.ru/api/integration/v2" +
            $"/inn/{Settings.Info.Vatin}/" +
            $"kkt/{Settings.Info.RegId}/receipts-info?" +
            $"dateFrom={day:yyyy-MM-dd}T00:00:00&" +
            $"dateTo={day:yyyy-MM-dd}T23:59:59&" +
            $"AuthToken={Settings.Ofd.Auth}");
        var msg = await App.Http.GetAsync(uri);
        var content = await msg.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<(string Status, OfdRu.Data.Receipt[]? Data)>(content);
        if (resp.Data is null) return;
        foreach(var info in resp.Data)
        {
            var receipt = new Receipt
            {
                Number = info.DocNumber,
                DateTime = info.DocDateTime,
                Items = new List<Position>((int)info.Depth),
                Payment = new Payment(
                    info.CashSumm, info.ECashSumm,
                    info.CreditSumm, info.PrepaidSumm),
                Subject = PullSubjects.Operator,
                OpCode = new OperationConverter().Convert(info.OperationType.ToLower()),
                TotalSum = info.TotalSumm,
                FiscalSign = info.DecimalFiscalSign,
                Operator = info.Operator,
            };
            foreach(var pos in info.Items)
            {
                receipt.Items.Add(new Position
                {
                    Name = pos.Name,
                    Price = pos.Price,
                    Total = pos.Total,
                    Quantity = (uint)Math.Round(pos.Quantity*1000,0),
                    Measure = (MeasureUnit)pos.ProductUnitOfMeasure,
                    Type = (PositionType)pos.SubjectType,
                    PaymentType = (PaymentMethod)pos.CalculationMethod,
                    TaxRate = new TaxRateConverter().Convert(pos.NDS_Rate)
                });
            }
            App.Receipts.Add(receipt);
        }

        Debug.Print(null);
    }
}

