using System.Threading.Tasks;
using RetailFixer.Interfaces;

namespace RetailFixer.Operators;

public sealed class Saby:IOperator
{
    public string Name => "Saby (Сбис ОФД)";

    public Saby()
    {
        throw new System.NotImplementedException();
    }
    public Task<bool> CheckInfo(string token)
    {
        throw new System.NotImplementedException();
    }

    public Task PullReceipts()
    {
        throw new System.NotImplementedException();
    }
}