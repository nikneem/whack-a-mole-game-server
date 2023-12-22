namespace Wam.Core.ExtensionMethods;

public static class IntegerExtensions
{
    
    public static bool IsHttpSuccessCode(this int code)
        => code >= 200 && code <= 299;

}