using RetailFixer.Enums;

namespace RetailFixer.Data;

/// <summary>
/// Информация об подключении ККТ
/// </summary>
/// <param name="Type">Тип подключения</param>
/// <param name="Address">Адрес/путь ККТ</param>
/// <param name="Port">TCP-порт для TCP/IP-подключения</param>
public readonly record struct FiscalConnectionData(FiscalConnectionType Type, string Address = "", ushort? Port = null);