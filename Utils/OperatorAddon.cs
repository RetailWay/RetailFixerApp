namespace RetailFixer.Utils;

public static class OperatorAddon
{
    public static bool FailCheck(string msg)
    {
        App.Logger.Fatal(msg);
        return false;
    }
}