namespace RetailFixer.Interfaces;

/// <typeparam name="TS">Тип в источнике данных</typeparam>
/// <typeparam name="TM">Тип в памяти программы</typeparam>
public interface IConverter<TS, TM>
{
    /// <summary>
    /// Конвертация типов данных при загрузке данных из источника (ДБ или API)
    /// </summary>
    public TM Convert(TS src);
    /// <summary>
    /// Конвертация типов данных при выгрузке данных в источник (ДБ или API)
    /// </summary>
    public TS ConvertBack(TM src);
}