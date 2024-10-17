namespace RetailFixer.Configurations;

public struct FiscalConfiguration
{
    public byte ConnType;
    public string SerialPort;
    public string USBPath;
    public string IPAddress;
    public ushort TCPPort;
    public string MACAddress;

    public static void Apply(byte type, string com, string path, string ip, ushort port, string mac) =>
        App.Fiscal = new FiscalConfiguration
        {
            ConnType = type,
            SerialPort = com,
            IPAddress = ip,
            MACAddress = mac,
            TCPPort = port,
            USBPath = path,
        };
}