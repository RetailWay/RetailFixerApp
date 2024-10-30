namespace RetailFixer.Data;

public struct Payment(uint cash = 0,uint ecash = 0,uint credit = 0,uint prepaid = 0, params OtherPayment[] other)
{
    public uint Cash = cash;
    public uint ECash = ecash;
    public uint Credit = credit;
    public uint Prepaid = prepaid;
    public OtherPayment[] Other = other;

    public static Payment operator +(Payment src, Payment other) =>
        new()
        {
            Cash = src.Cash + other.Cash,
            Credit = src.Credit + other.Credit,
            Prepaid = src.Prepaid + other.Prepaid,
            ECash = src.ECash + other.ECash,
            Other = [..src.Other, ..other.Other] 
        };
}

public record OtherPayment(byte Type, uint Sum);