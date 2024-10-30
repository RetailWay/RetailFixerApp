using System.Data.SQLite;

namespace RetailFixer.FrontWares.FxPos.Data;

public struct Ware
{
    public uint Code;
    public string Mark;
    public string Barcode;
    public uint Factor;
    public uint TaxSection;
    public uint TaxRateCode;
    public uint TaxGroupCode;
    public bool IsGiftCard;
    public uint Type;

    public Ware(SQLiteDataReader reader)
    {
        Code = (uint)reader.GetInt64(0);
        Mark = reader.GetString(1);
        Barcode = reader.GetString(2);
        Factor = (uint)reader.GetInt64(3);
        TaxSection = (uint)reader.GetInt64(4);
        TaxRateCode = (uint)reader.GetInt64(5);
        TaxGroupCode = (uint)reader.GetInt64(6);
        IsGiftCard = reader.GetInt16(7) == 1;
        Type = (uint)reader.GetInt64(8);
        
    }
}