using System;
using System.ComponentModel;
using RetailFixer.Data;
using RetailFixer.Enums;
using RetailFixer.Exceptions;
using RetailFixer.Interfaces;
using RetailFixer.Utils;

namespace RetailFixer;

public static class Settings
{
    public static FiscalConnectionData FiscalConnect { get; private set; }
    public static OperatorSettings Ofd { get; private set; }
    public static FiscalSoftSettings Soft { get; private set; }
    public static Period SearchPeriod { get; private set; } = new();
    public static FiscalInfo Info { get; private set; } = new("", "", "");
    public static string AuthDataOperator { get; private set; } = "";
    
    public static void UpdateFiscal(FiscalConnectionType type, string address, ushort? port) =>
        FiscalConnect = CheckData.Check(new FiscalConnectionData(type, address, port));

    public static void UpdateOfd(IOperator ofd, string data)
    {
        Ofd = new OperatorSettings(ofd, data);
    }
    
    public static void SetAuthDataOperator(string data) => AuthDataOperator = data;

    public static void UpdateSoft(byte softId, string mainPath, string logPath) =>
        Soft = new FiscalSoftSettings(softId, mainPath, logPath);

    public static void UpdateInfo(string vatin, string storageId, string regId) =>
        Info = new FiscalInfo(vatin, storageId, regId);
}