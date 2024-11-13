using System.Diagnostics;
using System.Text.Json;
using Lab9_Personen.Extensions;

namespace Lab9_Personen
{
    internal class Program
    {
        static void Main(string[] args)
        {
            #region File lesen
            string readJson = File.ReadAllText(@"..\..\..\Personen.json");
            List<Person> personen = JsonSerializer.Deserialize<List<Person>>(readJson);
            #endregion

            //Hier eigenen Code schreiben

            // Finde alle Personen, die mindestens 60 Jahre alt sind.
            personen.Where(e => e.Alter >= 60)
                .PrintTop10();

            // Finde alle Personen, die im Jahr 1977 geboren wurden.
            personen.Where(e => e.Geburtsdatum.Year == 1977)
                .PrintTop10();

            // Finde alle Personen, die mehr als 5000€/Monat verdienen.
            personen.Where(e => e.Job.Gehalt > 5000)
                .PrintTop10();

            // Sortiere alle Personen nach Jobtitel, danach nach Gehalt.
            personen.OrderBy(e => e.Job.Titel)
                .ThenBy(e => e.Job.Gehalt)
                .PrintTop10();

            // Wieviele Personen haben einen Vornamen mit mindestens 10 Buchstaben?
            _ = personen.Count(e => e.Vorname.Length >= 10);

            // Wieviel verdienen Softwareentwickler im Durchschnitt?
            _ = personen.Where(e => e.Job.Titel == "Softwareentwickler")
                .Average(e => e.Job.Gehalt);

            // Wie viele Personen haben genau zwei Hobbies?
            _ = personen.Count(e => e.Hobbies.Count == 2);

            // Finde alle Personen, die Radfahren und Laufen als Hobbies haben.
            personen.Where(e => e.Hobbies.Contains("Radfahren") && e.Hobbies.Contains("Laufen"))
                .PrintTop10();

            // Finde alle Personen, deren Vorname mit M beginnt und deren Nachname mit S beginnt.
            personen.Where(e => e.Vorname[0] == 'M' && e.Nachname[0] == 'S')
                .PrintTop10();

            // Finde alle Personen, bei der der Vorname und der Nachname den selben Anfangsbuchstaben haben.
            personen.Where(e => e.Vorname[0] == e.Nachname[0])
                .PrintTop10();

            // Finde alle Personen, die überdurchschnittlich alt sind in Relation zu allen Personen.
            _ = personen.Where(e => e.Alter > personen.Average(x => x.Alter));

            // Wieviele Personen die über 60 Jahre alt sind betreiben noch Sport (eine Tätigkeit von: Laufen, Radfahren, Ballsport, Fitness)?
            var desiredHobbies = new string[] { "Laufen", "Radfahren", "Ballsport", "Fitness" };
            _ = personen.Where(e => e.Alter > 60 && e.Hobbies.Any(h => desiredHobbies.Contains(h)));
            _ = personen.Where(e => e.Alter > 60 && e.Hobbies.Intersect(desiredHobbies).Any());

            // Wieviele verschiedene Jobs gibt es?
            _ = personen.Select(e => e.Job.Titel)
                .Distinct()
                .Count();

            // Finde das höchste Gehalt aller Tischler.
            _ = personen
                .Where(e => e.Job.Titel == "Tischler")
                .Max(e => e.Job.Gehalt);

            // Verdienen alle Personen die über 50 Jahre alt sind über 2000€?
            _ = personen.All(e => e.Alter > 50 && e.Job.Gehalt > 2000);

            // Gib alle Vor- und Nachnamekombinationen aus, sortiert nach Länge.
            _ = personen
                .Select(e => e.Vorname + " " + e.Nachname)
                .OrderBy(e => e.Length);

            // Finde die Top 5 Höchstverdiener.
            _ = personen
                .OrderByDescending(e => e.Job.Gehalt)
                .Take(5)
                .PrintTop10();

            // Finde alle Personen, die seit mindestens 20 Jahren in ihrem Job angestellt sind.
            _ = personen.Where(e => (int)(DateTime.Today.Subtract(e.Job.Einstellungsdatum).TotalDays / 365.25) >= 20);

            // Welche Vornamen kommen in der Liste am häufigsten vor?
            _ = personen
                .GroupBy(e => e.Job.Titel)
                .ToDictionary(e => e.Key, e => e.Max(x => x.Job.Gehalt))
                .OrderBy(e => e.Key);

            // Finde pro Beruf das höchste Gehalt, sortiert nach Jobtitel.
            _ = personen
                .GroupBy(e => e.Vorname)
                .ToDictionary(e => e.Key, e => e.Count())
                .OrderByDescending(e => e.Value);

            // Finde das am häufigsten vorkommende Hobby.
            _ = personen
                .Select(e => e.Hobbies)
                .SelectMany(e => e)
                .GroupBy(e => e)
                .ToDictionary(e => e.Key, e => e.Count())
                .OrderByDescending(e => e.Value);

            // Finde pro Berufsgruppe die Top 3 Höchstverdiener.
            _ = personen
                .GroupBy(e => e.Job.Titel)
                .ToDictionary(e => e.Key, e => e.OrderByDescending(x => x.Job.Gehalt).Take(3));
        }
    }

    [DebuggerDisplay("Person - ID: {ID}, Vorname: {Vorname}, Nachname: {Nachname}, GebDat: {Geburtsdatum.ToString(\"yyyy.MM.dd\")}, Alter: {Alter}, " +
"Jobtitel: {Job.Titel}, Gehalt: {Job.Gehalt}, Einstellungsdatum: {Job.Einstellungsdatum.ToString(\"yyyy.MM.dd\")}")]
    public record Person(int ID, string Vorname, string Nachname, DateTime Geburtsdatum, int Alter, Beruf Job, List<string> Hobbies);

    public record Beruf(string Titel, int Gehalt, DateTime Einstellungsdatum);
}
