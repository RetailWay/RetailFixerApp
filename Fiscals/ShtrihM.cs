using System;
using DrvFRLib;
using RetailFixer.Data;
using RetailFixer.Interfaces;

namespace RetailFixer.Fiscals;

public sealed class ShtrihM: IFiscal
{
    public string Name => "DrvFR (Штрих-М)";
    private readonly DrvFR _driver;

    public ShtrihM()
    {
        throw new NotImplementedException();
        _driver = new DrvFR();
    }

    public bool IsConnected { get; }
    public bool? SessionStage { get; }
    public void SetIsElectronically(bool value)
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

    public uint ConvertTaxRate(int raw)
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

    public bool CloseReceipt(uint sumCash = 0, uint sumEcash = 0, uint sumCredit = 0, uint sumPrepaid = 0)
    {
        throw new NotImplementedException();
    }
}