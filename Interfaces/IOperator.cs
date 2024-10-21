using System.Threading.Tasks;

namespace RetailFixer.Interfaces;

public interface IOperator
{
    public string Name { get; }
    public Task<bool> CheckInfo(string token);
    public Task PullReceipts();
}