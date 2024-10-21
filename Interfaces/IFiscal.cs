using System;
using RetailFixer.Data;

namespace RetailFixer.Interfaces;

public interface IFiscal
{
    /// <summary>
    /// Название драйвера
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Присутствует ли подключение между ККТ и драйвером?
    /// </summary>
    public bool IsConnected { get; }
    /// <summary>
    /// true при открытой смене, false при закрытой смене, иначе null
    /// </summary>
    public bool? SessionStage { get; }
    /// <summary>
    /// Печатать фискальные документы
    /// </summary>
    /// <param name="value">true для работы с электронными фискальными документами</param>
    public void SetIsElectronically(bool value);
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool OpenSession();
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool CloseSession();
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool CancelReceipt();
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool Connect();
    /// <summary>
    /// 
    /// </summary>
    public void PullInfo();
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
    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool Registration(Position position);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sumCash"></param>
    /// <param name="sumEcash"></param>
    /// <param name="sumCredit"></param>
    /// <param name="sumPrepaid"></param>
    /// <returns></returns>
    public bool CloseReceipt(uint sumCash = 0, uint sumEcash = 0, uint sumCredit = 0, uint sumPrepaid = 0);
}

