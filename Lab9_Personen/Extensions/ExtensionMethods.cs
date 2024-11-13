using System.Text;

namespace Lab9_Personen.Extensions;

public static class ExtensionMethods
{
	public static string PrintTop10(this IEnumerable<Person> list)
    {
        return list.Take(10).Aggregate(new StringBuilder(), AppendLine).ToString();

        static StringBuilder AppendLine(StringBuilder agg, Person p)
        {
            return agg.AppendLine($"Die Person {($"{p.Vorname} {p.Nachname}")} ist {p.Alter} Jahre alt und ...");
        }
    }
}
