using System.Collections.Generic;
using RetailFixer.Enums;

namespace RetailFixer.Data;

public class Position
{
    /// <summary>Наименование</summary>
    public string Name;
    /// <summary>Цена в копейках</summary>
    public uint Price;
    /// <summary>Стоимость в копейках</summary>
    public uint Total;
    /// <summary>Количество (1 шт = 1000)</summary>
    public uint Quantity;
    /// <summary>Единица измерения</summary>
    public MeasureUnit Measure;
    /// <summary>Ставка НДС</summary>
    public byte TaxRate;
    /// <summary>Тип позиции</summary>
    public PositionType Type;
    /// <summary>Способ оплаты</summary>
    public PaymentMethod PaymentType;
    /// <summary>Скрытые параметры</summary>
    public Dictionary<string, string> InnerProperties;
    public string this[string param] => InnerProperties[param];
}