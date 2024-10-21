using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using RetailFixer.Data;
using RetailFixer.Enums;
using RetailFixer.Exceptions;
using RetailFixer.Interfaces;
using static RetailFixer.Utils.FiscalAddon;

namespace RetailFixer.Fiscals;

public sealed class Atol10 : IFiscal
{
    #region Конструктор

    private static void TryInitialize()
    {
        if (!TryGetPathDriver(@"SOFTWARE\WOW6432Node\ATOL\Drivers\10.0\KKT", out var path) &&
            !TryGetPathDriver(@"SOFTWARE\ATOL\Drivers\10.0\KKT", out path)) return;
        if ((File.GetAttributes(path) & FileAttributes.Directory) == 0)
            path = Path.GetDirectoryName(path)!;
        Path.Combine(path, "msvcp140.dll");
        LoadLib(Path.Combine(path, "msvcp140.dll"));
        LoadLib(Path.Combine(path, "fptr10.dll"));
    }
    public Atol10()
    {
        TryInitialize();
        if (libfptr_create(out _driver) != 0)
            throw new FailCreateHandleException();
    }
    private nint _driver;
    #endregion
    #region Выгруженные методы
#pragma warning disable SYSLIB1054
    [DllImport("fptr10.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_get_param_str(IntPtr handle, int paramId, byte[] value, int size);
    [DllImport("fptr10.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_fn_query_data(IntPtr handle);
    [DllImport("fptr10.dll", CallingConvention = (CallingConvention) 2)]
    private static extern int libfptr_open_receipt(IntPtr handle);
    [DllImport("fptr10.dll", CallingConvention = (CallingConvention) 2)]
    private static extern int libfptr_close_receipt(IntPtr handle);
    [DllImport("fptr10.dll", CallingConvention = (CallingConvention) 2)]
    private static extern int libfptr_registration(IntPtr handle);
    [DllImport("fptr10.dll", CallingConvention = (CallingConvention) 2)]
    private static extern int libfptr_destroy(out IntPtr handle);
    [DllImport("fptr10.dll", CallingConvention = (CallingConvention) 2)]
    private static extern int libfptr_create(out IntPtr handle);
    [DllImport("fptr10.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_query_data(IntPtr handle);
    [DllImport("fptr10.dll", CallingConvention = (CallingConvention) 2)]
    private static extern int libfptr_error_code(IntPtr handle);
    [DllImport("fptr10.dll", CallingConvention = (CallingConvention) 2)]
    private static extern int libfptr_open(IntPtr handle);
    [DllImport("fptr10.dll", CallingConvention = (CallingConvention) 2)]
    private static extern int libfptr_close(IntPtr handle);
    [DllImport("fptr10.dll", CallingConvention = (CallingConvention) 2)]
    private static extern int libfptr_open_shift(IntPtr handle);
    [DllImport("fptr10.dll", CallingConvention = (CallingConvention) 2)]
    private static extern int libfptr_report(IntPtr handle);
    [DllImport("fptr10.dll", CallingConvention = (CallingConvention) 2)]
    private static extern void libfptr_set_param_int(IntPtr handle, int paramId, uint value);
    [DllImport("fptr10.dll", CallingConvention = (CallingConvention) 2)]
    private static extern int libfptr_util_form_tlv(IntPtr handle);
    [DllImport("fptr10.dll", CallingConvention = (CallingConvention) 2)]
    private static extern void libfptr_set_param_datetime(IntPtr handle, int paramId, int year, int month, int day, int hour, int minute, int second);
    [DllImport("fptr10.dll", CallingConvention = (CallingConvention) 2)]
    private static extern void libfptr_set_param_str(IntPtr handle, int paramId, byte[] value);
    [DllImport("fptr10.dll", CallingConvention = (CallingConvention) 2)]
    private static extern int libfptr_get_param_bytearray(IntPtr handle, int paramId, byte[] value, int size);
    [DllImport("fptr10.dll", CallingConvention = (CallingConvention) 2)]
    private static extern void libfptr_set_param_bytearray(IntPtr handle, int paramId, byte[] value, int size);
    [DllImport("fptr10.dll", CallingConvention = (CallingConvention) 2)]
    private static extern int libfptr_payment(IntPtr handle);
    [DllImport("fptr10.dll", CallingConvention = (CallingConvention) 2)]
    private static extern void libfptr_set_param_double(IntPtr handle, int paramId, double value);
    [DllImport("fptr10.dll", CallingConvention = (CallingConvention) 2)]
    private static extern void libfptr_set_param_bool(IntPtr handle, int paramId, bool value);
    [DllImport("fptr10.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern uint libfptr_get_param_int(IntPtr handle, int paramId);
    [DllImport("fptr10.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int libfptr_cancel_receipt(IntPtr handle);
    [DllImport("fptr10.dll", CallingConvention = (CallingConvention) 2)]
    private static extern void libfptr_set_single_setting(IntPtr handle, byte[] key, byte[] setting);
#pragma warning restore SYSLIB1054
    #endregion
    #region Промежуточные методы
    private static byte[] libfptr_get_param_bytearray(IntPtr handle, int paramId)
    {
        var array = new byte[128];
        var paramBytearray = libfptr_get_param_bytearray(handle, paramId, array, array.Length);
        if (paramBytearray > array.Length)
        {
            Array.Resize<byte>(ref array, paramBytearray);
            _ = libfptr_get_param_bytearray(handle, paramId, array, array.Length);
        }
        else
            Array.Resize<byte>(ref array, paramBytearray);
        return array;
    }
    
    private static string libfptr_get_param_str(IntPtr handle, int paramId)
    {
        var bytes = new byte[256];
        var paramStr = libfptr_get_param_str(handle, paramId, bytes, 128);
        if (paramStr > 128)
        {
            bytes = new byte[2 * paramStr];
            _ = libfptr_get_param_str(handle, paramId, bytes, paramStr);
        }
        return Encoding.UTF8.GetString(bytes, 0, paramStr * 2).TrimEnd(new char[1]);
    }

    private static int libfptr_payment(IntPtr handle, uint type, uint sum)
    {
        var dSum = sum / 100.0;
        App.Logger.Information($"Оплата {{\"type\": {type}, \"sum\": {dSum}}}");
        libfptr_set_param_int(handle,65564,type);
        libfptr_set_param_double(handle,65565,dSum);
        return libfptr_payment(handle);
    }
    
    private static void libfptr_set_single_setting(IntPtr handle, string key, string value) => 
        libfptr_set_single_setting(handle, Encoding.Unicode.GetBytes(key), Encoding.Unicode.GetBytes(value));
    #endregion
    #region Публичные методы
    public string Name => "FPTR (Atol v.10)";
    public bool IsConnected { get; }

    public bool? SessionStage
    {
        get
        {
            libfptr_set_param_int(_driver, 65587, 14);
            if (libfptr_query_data(_driver) != 0)
                throw new Exception($"{libfptr_error_code(_driver)}");
            return libfptr_get_param_int(_driver, 65592) switch
            {
                0 => false,
                1 => true,
                2 => null,
                _ => throw new ArgumentException()
            };
        }
    }

    public void PullInfo()
    {
        libfptr_set_param_int(_driver,65622, 9);
        _ = libfptr_fn_query_data(_driver);
        var vatin = libfptr_get_param_str(_driver, 1018);
        var regId = libfptr_get_param_str(_driver, 1037);
        libfptr_set_param_int(_driver,65622, 2);
        _ = libfptr_fn_query_data(_driver);
        var storageId = libfptr_get_param_str(_driver, 65559);
        Settings.UpdateInfo(vatin, storageId, regId);
    }
    
    public void SetIsElectronically(bool value) => 
        libfptr_set_param_bool(_driver, 65572, value);

    public bool OpenSession() =>
        libfptr_open_shift(_driver) != 0;

    public bool CloseSession()
    {
        libfptr_set_param_int(_driver, 65546, 0);
        return libfptr_report(_driver) != 0;
    }

    public bool CancelReceipt() => libfptr_cancel_receipt(_driver) != 0;

    public bool Connect()
    {
        #region Настройка драйвера
        libfptr_set_single_setting(_driver, "Model", "500");
        libfptr_set_single_setting(_driver, "Port", $"{Settings.FiscalConnect.Type}");
        switch (Settings.FiscalConnect.Type)
        {
            case FiscalConnectionType.SerialPort:
                libfptr_set_single_setting(_driver, "BaudRate", "115200");
                libfptr_set_single_setting(_driver, "ComFile", Settings.FiscalConnect.Address);
                break;
            case FiscalConnectionType.Usb:
                if(!string.IsNullOrWhiteSpace(Settings.FiscalConnect.Address))
                    libfptr_set_single_setting(_driver, "UsbDevicePath", Settings.FiscalConnect.Address);
                break;
            case FiscalConnectionType.TcpIp:
                libfptr_set_single_setting(_driver, "IPAddress", Settings.FiscalConnect.Address);
                libfptr_set_single_setting(_driver, "IPPort", $"{Settings.FiscalConnect.Port!}");
                break;
            case FiscalConnectionType.Bluetooth:
                libfptr_set_single_setting(_driver, "MACAddress", Settings.FiscalConnect.Address);
                break;
        }
        #endregion
        return libfptr_open(_driver) != 0;
    }

    public bool OpenReceipt(uint opcode, DateTime dt)
    {
        #region Создание основания коррекции
        libfptr_set_param_datetime(_driver, 1178, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
        libfptr_set_param_str(_driver, 1179, Encoding.Unicode.GetBytes(" "));
        if (libfptr_util_form_tlv(_driver) != 0) return false;
        var corrInfo = libfptr_get_param_bytearray(_driver, 65624);
        #endregion
        libfptr_set_param_int(_driver, 65545, opcode + 6);
        libfptr_set_param_int(_driver, 1173, 0);
        libfptr_set_param_bytearray(_driver, 1174, corrInfo, corrInfo.Length);
        return libfptr_open_receipt(_driver) == 0;
    }

    private static uint ConvertTaxRate(byte raw) =>
        raw switch
        {
            255 => 6,
            0 => 5,
            10 => 2,
            20 => 7,
            110 => 4,
            120 => 8,
            _ => throw new Exception()
        };
    public bool Registration(Position position)
    {
        libfptr_set_param_str(_driver, 65631, Encoding.UTF8.GetBytes(position.Name));
        libfptr_set_param_double(_driver, 65632, position.Price / 100.0);
        libfptr_set_param_double(_driver, 65633, position.Quantity / 1000.0);
        libfptr_set_param_int(_driver, 65569, ConvertTaxRate(position.TaxRate));
        libfptr_set_param_bool(_driver, 65654, position.Quantity % 1000 == 0);
        libfptr_set_param_int(_driver, 2108, (uint)position.Measure);
        libfptr_set_param_int(_driver, 1212, (uint)position.Type);
        libfptr_set_param_int(_driver, 1214, (uint)position.PaymentType);
        libfptr_set_param_double(_driver, 65634, position.Total / 100.0);
        return libfptr_registration(_driver) == 0;
    }
    
    public bool CloseReceipt(uint sumCash = 0, uint sumEcash = 0, uint sumCredit = 0, uint sumPrepaid = 0)
    {
        if (sumCash > 0 && libfptr_payment(_driver, 0, sumCash) != 0) return false;
        if (sumEcash > 0 && libfptr_payment(_driver, 1, sumEcash) != 0) return false;
        if (sumCredit > 0 && libfptr_payment(_driver, 3, sumCredit) != 0) return false;
        if (sumPrepaid > 0 && libfptr_payment(_driver, 2, sumPrepaid) != 0) return false;
        return libfptr_close_receipt(_driver) == 0;
    }

    ~Atol10()
    {
        _ = libfptr_close(_driver);
        _ = libfptr_destroy(out _driver);
    }

    #endregion
}