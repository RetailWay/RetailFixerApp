using System;
using RetailFixer.Data;
using RetailFixer.Enums;

namespace RetailFixer.Interfaces;

public interface IFiscal: ISoftWare
{
    /// <summary>
    /// Присутствует ли подключение между ККТ и драйвером?
    /// </summary>
    public bool IsConnected { get; }
    /// <summary>
    /// null если не удалось получить статус смены, иначе одно из значений <see cref="SessionStage"/>  
    /// </summary>
    public SessionStage? Session { get; }
    /// <summary>
    /// Установка вида фискальных документов
    /// </summary>
    /// <param name="value">true если не нужно печатать физический фискальный документ, иначе false</param>
    public void SetTypeReceipt(bool isElectronically);
    /// <summary>
    /// Открытие смены
    /// </summary>
    /// <returns>true если операция выполнена успешно, иначе false</returns>
    public bool OpenSession();
    /// <summary>
    /// Закрытие смены
    /// </summary>
    /// <returns>true если операция выполнена успешно, иначе false</returns>
    public bool CloseSession();
    /// <summary>
    /// Отмена фискального чека
    /// </summary>
    /// <returns>true если операция выполнена успешно, иначе false</returns>
    public bool CancelReceipt();
    /// <summary>
    /// Подключение к ККМ
    /// </summary>
    /// <returns>true если операция выполнена успешно, иначе false</returns>
    public bool Connect();
    /// <summary>
    /// Выгрузка информации для работы с ОФД из ККМ
    /// </summary>
    /// <returns>true если операция выполнена успешно, иначе false</returns>
    public bool PullInfo();
    /// <summary>
    /// Открытие фискального чека
    /// </summary>
    /// <param name="opcode">Значение тега 1054 (<see cref="ReceiptOperation"/>)</param>
    /// <param name="dt">Дата и время закрытия чека</param>
    /// <returns>true если операция выполнена успешно, иначе false</returns>
    public bool OpenReceipt(ReceiptOperation opcode, DateTime dt);
    /// <summary>
    /// Регистрация позиции в фискальный чек
    /// </summary>
    /// <param name="position">Информация о позиции</param>
    /// <seealso cref="Position"/>
    /// <returns>true если операция выполнена успешно, иначе false</returns>
    public bool Registration(Position position);
    /// <summary>
    /// Закрытие фискального чека
    /// </summary>
    /// <returns>true если операция выполнена успешно, иначе false</returns>
    public bool CloseReceipt(Payment payment);
}
