using System.Collections.Generic;
using RetailFixer.Data;

namespace RetailFixer.Interfaces;

public interface IFrontWare : ISoftWare
{
    public List<Receipt> PullReceipts();
    public void PushReceipt(Receipt fd);
    public void OpenConnection();
}