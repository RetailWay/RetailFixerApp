using System;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using RetailFixer.Data;
using RetailFixer.Interfaces;

namespace RetailFixer.Operators;

public class OfdRu : IOperator
{
    public string BaseUri => "https://ofd.ru/api/integration/v2";
    public async Task<bool> CheckInfo()
    {
        var uri = new Uri(
            $"{BaseUri}/inn/{App.Operator.VatinCompany}/kkts?" +
            $"KKTRegNumber={App.Operator.FiscalRegNum}&" +
            $"AuthToken={App.Operator.Token}");
        try
        {
            var resp = await App.Http.GetAsync(uri);
            var content = JsonNode.Parse(await resp.Content.ReadAsStringAsync());
            if (content is null)
            {
                // todo Invalid Response
                return false;
            }
            if (content!["Status"] is null)
            {
                // todo Invalid Token
                return false;
            }
            if (content["Status"]!.GetValue<string>() == "Failed")
            {
                // todo Invalid Vatin
                return false;
            }
            if (content["Data"]?.AsArray().Count == 0)
            {
                // todo Invalid RegId
                return false;
            }
            return true;
        }
        catch
        {
            // todo Error
            return false;
        }
    }

    public async Task<Receipt[]> GetReceipts()
    {
        throw new Exception();
    }
}