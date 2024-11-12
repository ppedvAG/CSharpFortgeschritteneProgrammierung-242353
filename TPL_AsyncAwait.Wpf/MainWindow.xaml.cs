using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TPL_AsyncAwait.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string RequestUri = "http://www.gutenberg.org/files/54700/54700-0.txt";

        private readonly CancellationTokenSource _cts = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Clear(object sender, MouseButtonEventArgs e)
        {
            Output.Text = string.Empty;
        }

        private void Start(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(25);
                Output.Text += i + Environment.NewLine;
            }
        }

        private void StartTaskRun(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    Thread.Sleep(25);

                    // UI Updates duerfen nicht von Side Threads/Tasks ausgefuehrt werden
                    // Dispatcher: Property welches auf jedem UI Element enthalten ist
                    // und ermoeglicht uns auf dem Thread der Komponente beliebigen Code auszufuehren
                    Dispatcher.Invoke(() => Output.Text += i + Environment.NewLine);
                }
            });
        }

        private async void StartAsyncAwait(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                btn.IsEnabled = false;

                // Mit Async/Await kann der Code darueber stark vereinfacht werden
                try
                {
                    for (int i = 0; i < 100; i++)
                    {
                        // Wir uebergeben den Cancellation Token hier
                        await Task.Delay(25, _cts.Token);
                        Output.Text += i + Environment.NewLine;

                    }
                }
                catch (TaskCanceledException)
                {
                    Output.Text += "Aktion vom Benutzer abgebrochen";
                }
                finally
                {
                    btn.IsEnabled = true;
                }
            }
        }

        private void CancelTask(object sender, RoutedEventArgs e)
        {
            _cts.Cancel();
        }

        private async void StartHttpClient(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                using HttpClient client = new();

                Task<HttpResponseMessage> request = client.GetAsync(RequestUri);
                Output.Text = "Request gestartet";
                btn.IsEnabled = false;

                //var resonse = await request;

                // Alternative wenn wir das async Keyword nicht benutzen koennen
                // dann koennen wir es hiermit umgehen
                var response = request.ConfigureAwait(false).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    Output.Text = "Response auslesen";
                    var content = await response.Content.ReadAsStringAsync();
                    Output.Text = content;
                } 
                else
                {
                    Output.Text = response.ReasonPhrase;
                }

                btn.IsEnabled = true;
            }
        }

        #region Mit Legacy Code umgehen

        /// <summary>
        /// Simulierte Legacy Methode welche sehr langsam ist und wir nicht anfassen duerfen 
        /// weil keine Tests und koennte beim Refactoring kaputt gehen.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public long CalcValuesVerySlow(int input)
        {
            Thread.Sleep(5000);

            return 42 * input;
        }

        // Schritt 1: Async Variante machen
        public Task<long> CalcValuesVerySlowAsync(int input)
        {
            return Task.Factory.StartNew((o) => CalcValuesVerySlow((int)o), input, _cts.Token);
        }

        private async void StartLegacyCode(object sender, RoutedEventArgs e)
        {
            try
            {
                Output.Text = "Anfrage gestartet fuer 122";
                var result = await CalcValuesVerySlowAsync(122);
                Output.Text = "Ergebnis: " + result; 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hilfe, Fehler!");
            }
        }

        #endregion

    }
}