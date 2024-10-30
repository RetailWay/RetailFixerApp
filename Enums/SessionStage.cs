namespace RetailFixer.Enums;

public enum SessionStage
{
    /// <summary>
    /// Смена закрыта
    /// </summary>
    Closed = 0,
    /// <summary>
    /// Смена открыта
    /// </summary>
    Opened = 1,
    /// <summary>
    /// Смена истекла (открыта более 24 часов)
    /// </summary>
    Expired = 2
}