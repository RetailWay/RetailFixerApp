namespace RetailFixer.Enums;

public enum PaymentMethod: byte
{
    FullPrepaid = 1,
    PartPrepaid,
    Advance,
    FullPayment,
    PartPayment,
    TransCredit,
    PayCredit
}