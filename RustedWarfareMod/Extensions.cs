namespace RustedWarfareMod;

public static class Extensions
{
    public static string ToUpperFirst(this string self) => Char.ToUpper(self[0]) + self[1..];
}
