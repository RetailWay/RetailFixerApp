using System.Threading.Tasks;

namespace RetailFixer.Interfaces;

public interface IOperator
{
    public string BaseUri { get; }
    public Task<bool> CheckInfo();
}