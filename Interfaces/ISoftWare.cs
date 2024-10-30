using System;

namespace RetailFixer.Interfaces;

public interface ISoftWare: IDisposable
{
    public static virtual string Name { get; } = "";
    public static virtual bool IsInstalled { get; } = false;
}