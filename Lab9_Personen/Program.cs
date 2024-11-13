using System.Diagnostics;

namespace Lab9_Personen
{
    internal class Program
    {
        static void Main(string[] args)
        {

        }

        [DebuggerDisplay("Person - ID: {ID}, Vorname: {Vorname}, Nachname: {Nachname}, GebDat: {Geburtsdatum.ToString(\"yyyy.MM.dd\")}, Alter: {Alter}, " +
    "Jobtitel: {Job.Titel}, Gehalt: {Job.Gehalt}, Einstellungsdatum: {Job.Einstellungsdatum.ToString(\"yyyy.MM.dd\")}")]
        public record Person(int ID, string Vorname, string Nachname, DateTime Geburtsdatum, int Alter, Beruf Job, List<string> Hobbies);

        public record Beruf(string Titel, int Gehalt, DateTime Einstellungsdatum);
    }
}
