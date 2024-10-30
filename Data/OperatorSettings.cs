using RetailFixer.Interfaces;

namespace RetailFixer.Data;

public record struct OperatorSettings(IOperator Operator, string Auth);