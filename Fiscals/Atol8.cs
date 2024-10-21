using System;
using FprnM1C;
using RetailFixer.Data;
using RetailFixer.Interfaces;

namespace RetailFixer.Fiscals;

// todo Рекомендуется использовать обычный чек, а не чек-коррекции (ТОЛЬКО одну позицию можно добавить)

public sealed class Atol8 : IFiscal
{
    public string Name => "FprnM45 (Atol v.8)";
    private readonly FprnM45 _driver;

    public Atol8()
    {
        throw new NotImplementedException();
        _driver = new FprnM45();
    }

    public bool IsConnected { get; }
    public bool? SessionStage
    {
        get
        {
            _driver.RegisterNumber = 18;
            _driver.GetRegister();
            return (_driver.SessionOpened, _driver.SessionExceedLimit) switch
            {
                (false, _) => false,
                (true, false) => true,
                (true, true) => null
            };
        }
    }
    public void SetIsElectronically(bool value) => _driver.CheckMode = value ? 0 : 1;

    public bool OpenSession() => _driver.OpenSession() == 0;

    public bool CloseSession()
    {
        _driver.RecordType = 1;
        return _driver.Report() == 0;
    }

    public bool CancelReceipt() => _driver.CancelCheck() == 0;

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
        //todo correction data
        _driver.CheckType = (int)opcode + 6;
        return _driver.OpenCheck() == 0;
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