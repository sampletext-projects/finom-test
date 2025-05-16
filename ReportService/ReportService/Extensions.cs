using System.Text;

namespace ReportService;

public static class Extensions
{
    public static string AsReadableString<TKey, TValue>(this IDictionary<TKey, TValue> dict)
    {
        var builder = new StringBuilder();
        builder.AppendLine("{");
        builder.AppendJoin(
            "\n",
            from key in dict.Keys
            select $"  {key} -> {dict[key]}"
        );
        builder.Append("\n}");

        return builder.ToString();
    }

    public static string StringJoin<T>(this IEnumerable<T> enumerable, string separator)
    {
        return string.Join(separator, enumerable);
    }

    public static string StringJoin<T>(this IEnumerable<T> enumerable, char separator)
    {
        return string.Join(separator, enumerable);
    }
}