/*using System;
using RetailFixer.Attributes;
using RetailFixer.Data;
using RetailFixer.Enums;
using RetailFixer.Interfaces;

namespace RetailFixer.Fiscals;

// todo Рекомендуется использовать обычный чек, а не чек-коррекции (ТОЛЬКО одну позицию можно добавить)
[DevelopStatus(DevelopStatus.ToDo)]
public sealed class Atol8 : IFiscal
{
    public string Name => "FprnM (Atol v.8)";
    public bool IsConnected { get; }
    public SessionStage? Session { get; }
    public static InstalledServices IsInstalled { get; }

    public void SetTypeReceipt(bool isElectronically)
    {
        throw new NotImplementedException();
    }

    public bool OpenSession()
    {
        throw new NotImplementedException();
    }

    public bool CloseSession()
    {
        throw new NotImplementedException();
    }

    public bool CancelReceipt()
    {
        throw new NotImplementedException();
    }

    public bool Connect()
    {
        throw new NotImplementedException();
    }

    public void PullInfo()
    {
        throw new NotImplementedException();
    }

    public bool OpenReceipt(uint opcode, DateTime dt)
    {
        throw new NotImplementedException();
    }

    public bool Registration(Position position)
    {
        throw new NotImplementedException();
    }

    public bool CloseReceipt(Payment payment)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
*/