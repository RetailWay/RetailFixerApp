namespace RetailFixer.Data;

public struct Position
{
    /// <summary>Наименование</summary>
    public string Name;
    /// <summary>Цена в копейках</summary>
    public uint Price;
    /// <summary>Стоимость в копейках</summary>
    public uint Total;
    /// <summary>Количество (1 шт = 1000)</summary>
    public ulong Quantity;
    /// <summary>Единица измерения</summary>
    public byte Measure;
    /// <summary>Ставка НДС</summary>
    public byte TaxRate;
    /// <summary>Тип позиции</summary>
    public byte Type;
    /// <summary>Способ оплаты</summary>
    public byte PaymentType;
    
}