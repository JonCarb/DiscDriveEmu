using DiscUtils.Raw;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.Storage;
using WpfAnimatedGif;

namespace EmuDiscReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static class AppService
        {
            public static StorageFolder DocumentsFolder { get; } = KnownFolders.DocumentsLibrary;
            public static StorageFile? PathsFile { get; set; }
            public static EmulationPaths? PathEmu { get; set; }
            public static bool CacheGame { get; set; }
            public static bool InSettings { get; set; }
            public static TaskCompletionSource<bool> EmuLoaded { get; } = new();
            //add more if needed
        }
        private const uint WM_DEVICECHANGE = 0x0219;
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        private ProcessDisc pd;
        public MainWindow()
        {
            InitializeComponent();
            SourceInitialized += MainWindow_SourceInitialized;
            Console.WriteLine("To you 5000 years from now");
            LoadEmulatorJson();
            AppService.CacheGame = false;
            pd = new ProcessDisc();
            pd.CheckDisc(this);
        }
        private void MainWindow_SourceInitialized(object? sender, EventArgs e)
        {
            HwndSource source = (HwndSource)PresentationSource.FromVisual(this)!;
            source.AddHook(WndProc);
        }
        private IntPtr WndProc( IntPtr hwnd,int msg,IntPtr wParam,IntPtr lParam, ref bool handled)
        {
            if (msg == WM_DEVICECHANGE)
            {
                switch (wParam.ToInt32())
                {
                    case DBT_DEVICEARRIVAL:
                        pd.CheckDisc(this);
                        break;

                    case DBT_DEVICEREMOVECOMPLETE:
                        DisplayError("Disc Tray Was Opened");
                        break;
                }
            }

            return IntPtr.Zero;
        }


        private static async void LoadEmulatorJson()
        {
            Console.WriteLine("Init Emulator Paths");

            AppService.PathsFile = await AppService.DocumentsFolder.CreateFileAsync
                ("EmulatorPaths.json", CreationCollisionOption.OpenIfExists);

            string readJson = await FileIO.ReadTextAsync(AppService.PathsFile);
            Console.WriteLine(readJson);

            //Fill file if its empty
            if (string.IsNullOrWhiteSpace(readJson))
            {
                AppService.PathEmu = new EmulationPaths();

                string newJson = JsonSerializer.Serialize(AppService.PathEmu, AppJsonContext.Default.EmulationPaths);

                File.WriteAllText(AppService.PathsFile.Path, newJson);
            }
            //Read the content and load it into the class
            else
            {
                AppService.PathEmu = JsonSerializer.Deserialize(readJson, AppJsonContext.Default.EmulationPaths);
            }
            AppService.EmuLoaded.SetResult(true);
        }
        /*
         HANDLE DISPLAY
        */
        public void ChangeDesc(string message)
        {
            Dispatcher.Invoke(() =>
            {
                Description.Text = message;
            });
        }
        public void MakeDiscImgRun()
        {
            Dispatcher.Invoke(() =>
            {
                ImageBehavior.SetAnimatedSource(Disc,new BitmapImage(new Uri(
                "pack://application:,,,/EmuDiscReader;component/Assets/readingDisc.gif")));
            });
        }
        public void buttonVisble(bool swtich)
        {
            Dispatcher.Invoke(() =>
            {
                if (swtich)
            {
                SettingsBTN.IsEnabled = true;
            }
            else
            {
                SettingsBTN.IsEnabled = false;
            }
            });
        }
        public void InstallBarValue(double value)
        {
            InstallBar.Value = value;
        }

        public void CacheBarVis(bool swtich)
        {
            Dispatcher.Invoke(() =>
            {
                if (swtich)
            {
                InstallBar.Visibility = Visibility.Visible;
            }
            else
            {
                InstallBar.Visibility = Visibility.Collapsed;
            }
            });
        }

        public async void DisplayError(string message = "Disc Error")
        {
            buttonVisble(true);
            ImageBehavior.SetAnimatedSource(Disc, new BitmapImage(new Uri(
                "pack://application:,,,/EmuDiscReader;component/Assets/diskError.gif")));
            Description.Text = message;

            await Task.Delay(3220);
            ImageBehavior.SetAnimatedSource(Disc, new BitmapImage(new Uri(
                "pack://application:,,,/EmuDiscReader;component/Assets/spinningDisc.gif")));
            Description.Text = "Please insert a Disc";
        }

        private void SettingsBTN_Click(object sender, RoutedEventArgs e)
        {
            AppService.InSettings = true;
            Settings setWin = new Settings();
            setWin.Owner = this;
            setWin.ShowDialog();
            AppService.InSettings = false;
            pd.CheckDisc(this);
        }
    }

    [JsonSourceGenerationOptions(WriteIndented = false)]
    [JsonSerializable(typeof(EmulationPaths))]
    internal partial class AppJsonContext : JsonSerializerContext { }
}