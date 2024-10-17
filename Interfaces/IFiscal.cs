using System;
using RetailFixer.Configurations;

namespace RetailFixer.Interfaces;

public interface IFiscal : IDisposable
{
    public bool HasDriver { get; }
    public bool IsElectronically { set; }
    public bool OpenSession();
    public bool CloseSession();
    public bool Connect();
    /// <param name="opcode">
    /// Значение тега 1054:<br/>
    /// 1 - приход<br/>
    /// 2 - возврат прихода<br/>
    /// 3 - расход<br/>
    /// 4 - возврат расхода
    /// </param>
    /// <param name="dt">Дата и время закрытия чека</param>
    /// <returns></returns>
    public bool OpenReceipt(uint opcode, DateTime dt);
    public bool Registration();
    public bool CloseReceipt(uint sumCash = 0, uint sumEcash = 0, uint sumCredit = 0, uint sumPrepaid = 0);
}

