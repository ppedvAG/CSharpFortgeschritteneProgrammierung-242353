using System.Collections.Concurrent;

namespace Lab_Images;

public class Program
{
	private const int DEFAULT_WORKER_COUNT = 4;

	private static List<Scanner> Scanners = new();
	private static List<Worker> Workers = new();

	private static bool ProcessRunning = false;

	private static string LastOutput = "Keine";

	private static string OutputPath = "Output";

	private static bool SuspendRefresh = false;

    /// <summary>
    /// Dictionary fuer die Eingaben; 
	/// Alternativ wuerde es auch ein Switch-Case tun was zur Compilezeit statt zur Laufzeit ausgewertet wird
    /// </summary>
    private static readonly Dictionary<ConsoleKey, Action> Inputs = new()
	{
		{ ConsoleKey.D1, CreateScanner },
		{ ConsoleKey.D2, AdjustWorkerAmount },
		{ ConsoleKey.D3, AdjustOutputPath },
		{ ConsoleKey.D4, StartProcess },
		{ ConsoleKey.D5, PauseProcess }
	};

	static void Main(string[] args)
	{
		CreateOutputTask();
		while (true)
		{
			ConsoleKey inputKey = Console.ReadKey(true).Key;
			if (Inputs.TryGetValue(inputKey, out Action a))
				ProcessInput(a, inputKey != ConsoleKey.D4 && inputKey != ConsoleKey.D5);

			#region Alternative
			//if (inputKey == ConsoleKey.D4 || inputKey == ConsoleKey.D5)
			//{
			//	value.Invoke();
			//	Console.Clear();
			//	continue;
			//}

			//SuspendRefresh = true;
			//value.Invoke();
			//Console.Clear();
			//SuspendRefresh = false;
			#endregion
		}

		// local function
		void ProcessInput(Action a, bool suspend = false)
		{
			SuspendRefresh = suspend;
			a.Invoke();
			Console.Clear();
			SuspendRefresh = false;
		}
	}

	#region Print Methoden
	private static void PrintUserInputs()
	{
		Console.WriteLine("Eingaben: ");
		Console.WriteLine("1: Neuen Scanner erstellen");
		Console.WriteLine("2: Anzahl Worker Tasks anpassen");
		Console.WriteLine("3: Speicherpfad anpassen");
		Console.WriteLine("4: Prozess starten/fortsetzen");
		Console.WriteLine("5: Prozess pausieren");
	}

	private static void PrintStatus()
	{
		if (Scanners.Count != 0)
		{
			Console.WriteLine("\nScanner Liste: ");
			for (int i = 0; i < Scanners.Count; i++)
				Console.WriteLine($"{i}: {Scanners[i].ScanPath}");
		}

		if (Workers.Count != 0)
		{
			Console.WriteLine("\nWorker Liste: ");
			for (int i = 0; i < Workers.Count; i++)
				Console.WriteLine($"{i}: {Workers[i].CurrentPath}");
		}

		Console.WriteLine($"\nSpeicherpfad: {Path.GetFullPath(OutputPath)}");
		Console.WriteLine($"Letzte Meldung: {LastOutput}");
	}

    /// <summary>
    /// Hier wird die Oberflaeche in einem eigenen Thread gezeichnet
    /// </summary>
    private static void CreateOutputTask()
	{
		Task.Run(() =>
		{
			while (true)
			{
				if (SuspendRefresh)
					continue;

				Console.SetCursorPosition(0, 0);
				PrintUserInputs();
				PrintStatus();
				Thread.Sleep(1000);
			}
		});
	}
	#endregion

	#region Input Methoden
	private static void CreateScanner()
    {
        Console.Write("Gib einen Pfad zum Scannen ein: ");
        string input = Console.ReadLine();

        var path = Path.Combine(Environment.CurrentDirectory, string.IsNullOrWhiteSpace(input) ? "images" : input);

        if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
            LastOutput = $"Pfad {path} wurde erstellt";
        }

        Scanners.Add(new Scanner(path));
    }

    private static void AdjustWorkerAmount()
	{
		Console.Write($"Gib eine neue Anzahl von Worker-Tasks ein (derzeit {Workers.Count}): ");
		string workerEingabe = Console.ReadLine();
		if (int.TryParse(workerEingabe, out int newWorkers))
		{
			if (newWorkers > 0 && newWorkers != Workers.Count)
			{
				PauseList(Workers);

				int newAmount = newWorkers - Workers.Count;
				for (int i = 0; i < newAmount; i++)
					if (Workers.Count < newWorkers)
						Workers.Add(new Worker(OutputPath, ProcessRunning));

				if (newAmount < 0)
					Workers.RemoveRange(0, Workers.Count - newWorkers);
			}
			else
				LastOutput = "Ungültige Eingabe";
		}
	}

	private static void AdjustOutputPath()
	{
		Console.Write("Gib einen neuen Speicherpfad ein: ");
		string input = Console.ReadLine();

        var path = Path.Combine(Environment.CurrentDirectory, string.IsNullOrWhiteSpace(input) ? "output" : input);

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
			LastOutput = $"Pfad {path} wurde erstellt";
        }

		OutputPath = path;

    }

    private static void StartProcess()
	{
		if (ProcessRunning)
		{
			LastOutput = "Prozess läuft bereits";
			return;
		}

		if (Scanners.Count == 0)
		{
			LastOutput = "Keine Scanner erstellt";
			return;
		}

		Scanners.ForEach(e => e.Continue = true);

		if (Workers.Count == 0)
		{
			for (int i = 0; i < DEFAULT_WORKER_COUNT; i++)
				Workers.Add(new Worker(OutputPath, true));
			LastOutput = "Prozess gestartet mit 4 Worker-Tasks";
		}
		else
		{
			Workers.ForEach(e => e.Continue = true);
			LastOutput = "Prozess gestartet";
		}

		if (!Directory.Exists(OutputPath))
			Directory.CreateDirectory(OutputPath);

		ProcessRunning = true;
	}

	private static void PauseProcess()
	{
		if (!ProcessRunning)
		{
			LastOutput = "Prozess läuft nicht";
			return;
		}

		PauseList(Scanners);
		PauseList(Workers);
		ProcessRunning = false;
		LastOutput = "Prozess pausiert";
	}
	#endregion

	private static void PauseList<T>(List<T> runnables) where T : Runnable
	{
		Console.WriteLine($"Warte auf das Beenden aller {typeof(T).Name}");
		foreach (Runnable r in runnables)
			r.Continue = false;

		while (runnables.Any(e => e.CurrentTask.Status == TaskStatus.Running))
			continue;
    }
}