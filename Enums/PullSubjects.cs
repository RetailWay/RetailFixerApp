using System;

namespace RetailFixer.Enums;

[Flags]
public enum PullSubjects: byte
{
    Operator = 1,
    FrontSystem = 2,
}