using System.Threading.Tasks;
using RetailFixer.Interfaces;

namespace RetailFixer.Operators;

public sealed class PlatformOfd:IOperator
{
    public string Name => "Платформа ОФД";
    public Task<bool> CheckInfo(string token)
    {
        throw new System.NotImplementedException();
    }

    public Task PullReceipts()
    {
        throw new System.NotImplementedException();
    }

    public PlatformOfd()
    {
        throw new System.NotImplementedException();
    }
}