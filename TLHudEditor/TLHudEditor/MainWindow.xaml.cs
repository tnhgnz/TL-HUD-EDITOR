using System.Windows.Input;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Windows;
using System.IO;

namespace TLHudEditor
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> configs = new();

            foreach (string filePath in Directory.GetFiles(Utils.ConfigsPath))
            {
                configs.Add(filePath.Replace(Utils.ConfigsPath, "").Replace(Utils.ConfigExt, ""));
            }

            ConfigsList.ItemsSource = configs;
            ConfigsList.SelectedIndex = 0;
        }

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Close(object sender, RoutedEventArgs e) => Environment.Exit(0);

        private void OpenTwitch(object sender, MouseButtonEventArgs e) =>
            Process.Start(new ProcessStartInfo("https://www.twitch.tv/tnhgnz") { UseShellExecute = true });
        private void OpenTelegram(object sender, MouseButtonEventArgs e) =>
            Process.Start(new ProcessStartInfo("https://t.me/tnhgnz") { UseShellExecute = true });

        private void OpenConfigEditWindow(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = Utils.ConfigsPath + ConfigsList.SelectedValue + Utils.ConfigExt;
                var config = JsonConvert.DeserializeObject<AzulejoLayoutDocument>(File.ReadAllText(path));
                if (config == null)
                    return;
                var configWindow = new ConfigEditWindow(path, config);
                Hide();
                configWindow.Show();
            }
            catch { }
        }
    }
}