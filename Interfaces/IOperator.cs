using System.Threading.Tasks;

namespace RetailFixer.Interfaces;

public interface IOperator
{
    public static virtual string Name { get; } = "";
    public Task<bool> CheckInfo(string token);
    public Task PullReceipts();
}