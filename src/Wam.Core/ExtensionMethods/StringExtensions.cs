namespace Wam.Core.ExtensionMethods;

public static class StringExtensions
{
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private static readonly Random Random;

    public static string GenerateGameCode(int length = 6)
    {
        var result = new string(
                       Enumerable.Repeat(Chars, length)
                                      .Select(s => s[Random.Next(s.Length)])
                                      .ToArray());
        return result;
    }

    static StringExtensions()
    {
        Random = new Random();
    }
}