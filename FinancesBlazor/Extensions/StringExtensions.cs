using System.Reflection.Metadata.Ecma335;

namespace FinancesBlazor.Extensions;

public static class StringExtensions
{
    public static string MakeFirstLetterUppercase(this string input) =>
        input switch
        {
            null => null,
            "" => "",
            _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
        };
}
