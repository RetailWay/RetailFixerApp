using System;
using System.Runtime.InteropServices;
using System.Text;
using RetailFixer.Interfaces;

namespace RetailFixer.Fiscals;

public class Atol : IFiscal
{
    #region Конструктор

    public Atol()
    {
        // todo
    }
    private nint driver = IntPtr.Zero;
    #endregion
    #region Выгруженные методы
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr LoadLibrary(string lpPathName);
    [DllImport("fptr10", CallingConvention = (CallingConvention) 2)]
    private static extern int libfptr_open_receipt(IntPtr handle);
    [DllImport("fptr10", CallingConvention = (CallingConvention) 2)]
    private static extern int libfptr_close_receipt(IntPtr handle);
    [DllImport("fptr10", CallingConvention = (CallingConvention) 2)]
    private static extern int libfptr_registration(IntPtr handle);
    [DllImport("fptr10", CallingConvention = (CallingConvention) 2)]
    private static extern int libfptr_destroy(out IntPtr handle);
    [DllImport("fptr10", CallingConvention = (CallingConvention) 2)]
    private static extern int libfptr_create(out IntPtr handle);
    [DllImport("fptr10", CallingConvention = (CallingConvention) 2)]
    private static extern int libfptr_error_code(IntPtr handle);
    [DllImport("fptr10", CallingConvention = (CallingConvention) 2)]
    private static extern int libfptr_open(IntPtr handle);
    [DllImport("fptr10", CallingConvention = (CallingConvention) 2)]
    private static extern int libfptr_close(IntPtr handle);
    [DllImport("fptr10", CallingConvention = (CallingConvention) 2)]
    private static extern int libfptr_open_shift(IntPtr handle);
    [DllImport("fptr10", CallingConvention = (CallingConvention) 2)]
    private static extern int libfptr_report(IntPtr handle);
    [DllImport("fptr10", CallingConvention = (CallingConvention) 2)]
    private static extern void libfptr_set_param_int(IntPtr handle, int paramID, uint value);
    [DllImport("fptr10", CallingConvention = (CallingConvention) 2)]
    private static extern int libfptr_util_form_tlv(IntPtr handle);
    [DllImport("fptr10", CallingConvention = (CallingConvention) 2)]
    private static extern void libfptr_set_param_datetime(IntPtr handle, int paramID, int year, int month, int day, int hour, int minute, int second);
    [DllImport("fptr10", CallingConvention = (CallingConvention) 2)]
    private static extern void libfptr_set_param_str(IntPtr handle, int paramID, byte[] value);
    [DllImport("fptr10", CallingConvention = (CallingConvention) 2)]
    private static extern int libfptr_get_param_bytearray(IntPtr handle, int paramID, byte[] value, int size);
    [DllImport("fptr10", CallingConvention = (CallingConvention) 2)]
    private static extern void libfptr_set_param_bytearray(IntPtr handle, int paramID, byte[] value, int size);
    [DllImport("fptr10", CallingConvention = (CallingConvention) 2)]
    private static extern int libfptr_payment(IntPtr handle);
    [DllImport("fptr10", CallingConvention = (CallingConvention) 2)]
    private static extern void libfptr_set_param_double(IntPtr handle, int paramID, double value);
    [DllImport("fptr10", CallingConvention = (CallingConvention) 2)]
    private static extern void libfptr_set_param_bool(IntPtr handle, int paramID, bool value);

    [DllImport("fptr10", CallingConvention = (CallingConvention) 2)]
    private static extern void libfptr_set_single_setting(IntPtr handle, byte[] key, byte[] setting);
    #endregion
    #region Промежуточные методы
    private byte[] libfptr_get_param_bytearray(IntPtr handle, int paramID)
    {
        byte[] array = new byte[128];
        int paramBytearray = libfptr_get_param_bytearray(driver, paramID, array, array.Length);
        if (paramBytearray > array.Length)
        {
            Array.Resize<byte>(ref array, paramBytearray);
            libfptr_get_param_bytearray(driver, paramID, array, array.Length);
        }
        else
            Array.Resize<byte>(ref array, paramBytearray);
        return array;
    }

    private int libfptr_payment(IntPtr handle, uint type, uint sum)
    {
        var dSum = sum / 100.0;
        App.Logger.Information($"Оплата {{\"type\": {type}, \"sum\": {dSum}}}");
        libfptr_set_param_int(driver,65564,type);
        libfptr_set_param_double(driver,65565,dSum);
        return libfptr_payment(driver);
    }

    private void libfptr_set_single_setting(IntPtr handle, string key, string value) => 
        libfptr_set_single_setting(handle, Encoding.Unicode.GetBytes(key), Encoding.Unicode.GetBytes(value));
    #endregion
    #region Публичные методы
    public bool HasDriver => driver != IntPtr.Zero;
    public bool IsElectronically
    {
        set => libfptr_set_param_bool(driver, 65572, value);
    }

    public bool OpenSession() =>
        libfptr_open_shift(driver) != 0;

    public bool CloseSession()
    {
        libfptr_set_param_int(driver, 65546, 0);
        return libfptr_report(driver) != 0;
    }

    public bool Connect()
    {
        #region Настройка драйвера
        libfptr_set_single_setting(driver, "Model", "500");
        libfptr_set_single_setting(driver, "Port", $"{App.Fiscal.ConnType}");
        switch (App.Fiscal.ConnType)
        {
            case 0: // COM
                libfptr_set_single_setting(driver, "BaudRate", "115200");
                libfptr_set_single_setting(driver, "ComFile", App.Fiscal.SerialPort);
                break;
            case 1: // USB
                libfptr_set_single_setting(driver, "UsbDevicePath", App.Fiscal.USBPath);
                break;
            case 2: // IP
                libfptr_set_single_setting(driver, "IPAddress", App.Fiscal.IPAddress);
                libfptr_set_single_setting(driver, "IPPort", $"{App.Fiscal.TCPPort}");
                break;
            case 3: // BT
                libfptr_set_single_setting(driver, "MACAddress", App.Fiscal.MACAddress);
                break;
        }
        #endregion
        return libfptr_open(driver) != 0;
    }

    /*public bool Disconnect() =>
        libfptr_close(driver) != 0;*/
    
    /// <param name="opcode">
    /// Значение тега 1054:<br/>
    /// 1 - приход<br/>
    /// 2 - возврат прихода<br/>
    /// 3 - расход<br/>
    /// 4 - возврат расхода
    /// </param>
    /// <param name="dt">Дата и время закрытия чека</param>
    /// <returns></returns>
    public bool OpenReceipt(uint opcode, DateTime dt)
    {
        #region Создание основания коррекции
        libfptr_set_param_datetime(driver, 1178, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
        libfptr_set_param_str(driver, 1179, Encoding.Unicode.GetBytes(" "));
        if (libfptr_util_form_tlv(driver) != 0) return false;
        var corrInfo = libfptr_get_param_bytearray(driver, 65624);
        #endregion
        libfptr_set_param_int(driver, 65545, opcode + 6);
        libfptr_set_param_int(driver, 1173, 0);
        libfptr_set_param_bytearray(driver, 1174, corrInfo, corrInfo.Length);
        return libfptr_open_receipt(driver) == 0;
    }

    public bool Registration()
    {
        return libfptr_registration(driver) == 0;
    }
    
    public bool CloseReceipt(uint sumCash = 0, uint sumEcash = 0, uint sumCredit = 0, uint sumPrepaid = 0)
    {
        if (sumCash > 0 && libfptr_payment(driver, 0, sumCash) != 0) return false;
        if (sumEcash > 0 && libfptr_payment(driver, 1, sumEcash) != 0) return false;
        if (sumCredit > 0 && libfptr_payment(driver, 3, sumCredit) != 0) return false;
        if (sumPrepaid > 0 && libfptr_payment(driver, 2, sumPrepaid) != 0) return false;
        return libfptr_close_receipt(driver) == 0;
    }

    public void Dispose()
    {
        libfptr_close(driver);
        libfptr_destroy(out driver);
    }

    #endregion
}