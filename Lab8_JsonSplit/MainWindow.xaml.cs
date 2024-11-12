using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace JsonSplit
{
    public partial class MainWindow : Window
	{
		public MainWindow() => InitializeComponent();

		private async void SaveSplitJson(object sender, EventArgs e)
		{
			ButtonGrid.IsEnabled = false;
			
			using StreamReader sr = new StreamReader("history.city.list.min.json");
			JsonDocument jd = await JsonDocument.ParseAsync(sr.BaseStream);
			//StreamReader erstellen und BaseStream davon als Stream-Parameter hier benutzen
			//Json File einlesen mit await um UI-Freeze zu verhindern

			if (Directory.Exists("Lab"))
				Directory.Delete("Lab", true); //Übung zurücksetzen
			Directory.CreateDirectory("Lab");

			ConcurrentDictionary<string, List<JsonElement>> cities = new();
			foreach (JsonElement je in jd.RootElement.EnumerateArray()) //Json Datei iterieren
			{
				string countryCode = je.GetProperty("city").GetProperty("country").GetString(); //Auf den CountryCode zugreifen (AT, DE, IT, ...)
				if (!cities.ContainsKey(countryCode))
					cities.TryAdd(countryCode, new());
				cities[countryCode].Add(je);
			}

			Progress.Maximum = cities.Count; //Maximum der ProgressBar setzen

			//await ForEachAsync um UI nicht freezen zu lassen
			//await Parallel.ForEachAsync(cities, (kv, x) => //Json Files schreiben
			//{
			//	File.WriteAllText(Path.Combine("Lab", $"{kv.Key}.json"), JsonListToJson(kv.Value)); //Methode weiter unten
			//	Dispatcher.Invoke(() =>
			//	{
			//		Output.Text += $"{kv.Key} in die Datei geschrieben\n"; //Logging
			//		Scroll.ScrollToEnd(); //Dispatcher.Invoke: Side Threads/Tasks können nicht auf den UI Thread zugreifen, der Dispatcher ermöglicht einen Umweg
			//		Progress.Value++;
			//	});
			//	return ValueTask.CompletedTask; //Hier einfach "leeres" Objekt zurückgeben
			//});

			//Ohne Parallel, langsamer
			List<Task> writeTasks = new();
			foreach (KeyValuePair<string, List<JsonElement>> kv in cities) //Json Files schreiben
				writeTasks.Add(WriteFileAndUpdateUI(kv.Key, JsonListToJson(kv.Value)));
			await Task.WhenAll(writeTasks);

			Progress.Value = 0;
			ButtonGrid.IsEnabled = true;
		}

        /// <summary>
        /// Diese Methode soll die aufgeteilten Json Dateien wieder einlesen und danach in einem Dictionary bereit stellen.
        /// Diese Methode ist mit dem rechten Button (Load Json) in der UI verbunden
        /// </summary>
        private async Task WriteFileAndUpdateUI(string path, string content)
		{
			File.WriteAllTextAsync(Path.Combine("Lab", $"{path}.json"), content); //Methode weiter unten
			Output.Text += $"{path} in die Datei geschrieben\n"; //Logging
			Scroll.ScrollToEnd();
			Progress.Value++;
		}

		private async void LoadSplitJsonFiles(object sender, RoutedEventArgs e)
		{
			ButtonGrid.IsEnabled = false;

			ConcurrentDictionary<string, List<JsonElement>> jsons = new(); //Parallel-sicheres Dictionary
			List<string> countryCodes = Directory.GetFiles("Lab").Select(path => Path.GetFileNameWithoutExtension(path)).ToList();
			//Alle Dateinamen ohne Pfad und Erweiterung

			Progress.Maximum = countryCodes.Count; //Maximum der ProgressBar setzen

			//await ForEachAsync um UI nicht freezen zu lassen
			await Parallel.ForEachAsync(countryCodes, (code, ct) =>
			{
				string path = Path.Combine("Lab", $"{code}.json"); //Einzelnes File angreifen
				string file = File.ReadAllText(path); //File einlesen
				var array = JsonDocument.Parse(file).RootElement;
                jsons.TryAdd(code, array.EnumerateArray().ToList()); //Element hinzufügen (hier mit TryAdd)
				Dispatcher.Invoke(() => //UI aktualisieren
				{
					Output.Text += $"{code} geladen\n";
					Scroll.ScrollToEnd();
					Progress.Value = jsons.Count;
				});
				return ValueTask.CompletedTask; //Hier einfach "leeres" Objekt zurückgeben
			});

            //Ohne Parallel, langsamer
            //foreach (string code in countryCodes)
            //{
            //	string path = Path.Combine("Lab", $"{code}.json"); //Einzelnes File angreifen
            //	string file = await File.ReadAllTextAsync(path); //File einlesen
            //	var array = JsonDocument.Parse(file).RootElement;
            //	jsons.TryAdd(code, array.EnumerateArray().ToList()); //Element hinzufügen (hier mit TryAdd)

            //	Output.Text += $"{code} geladen\n";
            //	Scroll.ScrollToEnd();
            //	Progress.Value++; //ProgressBar aktualisieren
            //}

            Progress.Value = 0;
			ButtonGrid.IsEnabled = true;
		}

		//Es gibt keine Methode um aus einer Liste von JsonElements ein JsonArray zu generieren
		private string JsonListToJson(List<JsonElement> jsons)
		{
			return jsons.Aggregate(new StringBuilder("[\n"), (sb, je) =>
				sb.Append('\t')
				  .Append(je.GetRawText())
				  .Append(",\n"))
				  .ToString()
				  .TrimEnd(',', '\n') + "\n]";
		}
	}
}
