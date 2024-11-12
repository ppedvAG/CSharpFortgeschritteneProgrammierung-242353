using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace JsonSplit
{
	public partial class MainWindow : Window
	{
		/// <summary>
		/// Datei herunterladen, an einen beliebigen Ort speichern, entpacken und danach mit SaveSplitJson einlesen und bearbeiten.
		/// http://bulk.openweathermap.org/sample/history.city.list.min.json.gz
		/// </summary>
		public MainWindow() => InitializeComponent();

		/// <summary>
		/// Diese Methode soll die originale Json Datei laden, aufteilen und in die einzelnen Dateien speichern.
		/// Diese Methode ist mit dem linken Button (Split Json) in der UI verbunden
		/// </summary>
		private async void SaveSplitJson(object sender, EventArgs e)
		{

		}

		/// <summary>
		/// Diese Methode soll die aufgeteilten Json Dateien wieder einlesen und danach in einem Dictionary bereit stellen.
		/// Diese Methode ist mit dem rechten Button (Load Json) in der UI verbunden
		/// </summary>
		private async void LoadSplitJsonFiles(object sender, RoutedEventArgs e)
		{

		}

		/// <summary>
		/// Verwende diese Methode, um einen Text in der TextBox anzuzeigen.
		/// </summary>
		/// <param name="text"></param>
		private void WriteText(string text)
		{
			Dispatcher.Invoke(() => 
			{
                Output.Text += text;
                Output.Text += Environment.NewLine;
				Scroll.ScrollToEnd();
			});
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
