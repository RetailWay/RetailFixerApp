namespace RetailFixer.Operators.OfdRu.Data;

internal struct Item
{
    public string Name { get; set; }
    public uint Price { get; set; }
    public double Quantity { get; set; }
    public uint Total { get; set; }
    public byte SubjectType { get; set; }
    public byte CalculationMethod { get; set; }
    public byte ProductUnitOfMeasure { get; set; }
    public byte NDS_Rate { get; set; }
}