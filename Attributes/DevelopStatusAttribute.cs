using System;
using System.Diagnostics;
using RetailFixer.Enums;

namespace RetailFixer.Attributes;

[AttributeUsage(AttributeTargets.Class), Conditional("DEBUG")]
public class DevelopStatusAttribute(DevelopStatus status) : Attribute
{
    public DevelopStatus Status = status;
}