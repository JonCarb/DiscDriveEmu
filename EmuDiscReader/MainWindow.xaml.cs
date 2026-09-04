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
using System.Windows.Threading;
using Windows.Storage;
using WpfAnimatedGif;
using System.Runtime.InteropServices;

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
            public static StorageFile? PathsFile { get; set; }      //Path of where the json file is stored
            public static EmulationPaths? PathEmu { get; set; }
            public static bool InSettings { get; set; }
            public static bool GameReady { get; set; }
            public static TaskCompletionSource<bool> EmuLoaded { get; } = new();

            public static void SaveJson()
            {
                if (PathEmu == null || PathsFile == null) { Console.WriteLine("PathEmu or PathsFile were NULL"); return; }
                string json = JsonSerializer.Serialize(PathEmu, AppJsonContext.Default.EmulationPaths);
                File.WriteAllText(PathsFile.Path, json);
            }
            //add more if needed
        }

        [DllImport("winmm.dll", EntryPoint = "mciSendStringA", CharSet = CharSet.Ansi)]
        static extern int MciSendString(string command, StringBuilder? returnString, int returnLength, IntPtr callback);
        private const uint WM_DEVICECHANGE = 0x0219;
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        private ProcessDisc pd;
        private bool initInstall = true;
        public MainWindow()
        {
            Console.WriteLine("To you 5000 years from now");
            InitializeComponent();
            pd = new ProcessDisc();

            SourceInitialized += MainWindow_SourceInitialized;
            _ = LoadEmulatorJson();
            _ = InitInstallBTN();

            AppService.GameReady = false;

            _ = pd.CheckDisc(this);

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
                if(wParam.ToInt32() == DBT_DEVICEARRIVAL)
                {
                    _ = pd.CheckDisc(this);
                }
            }
            return IntPtr.Zero;
        }


        private static async Task LoadEmulatorJson()
        {
            //Console.WriteLine("Init Emulator Paths");

            AppService.PathsFile = await AppService.DocumentsFolder.CreateFileAsync
                ("EmulatorPaths.json", CreationCollisionOption.OpenIfExists);

            string readJson = await FileIO.ReadTextAsync(AppService.PathsFile);
            Console.WriteLine(readJson);

            //Fill file if its empty
            if (string.IsNullOrWhiteSpace(readJson))
            {
                AppService.PathEmu = new();
                AppService.PathEmu.WillCache = false;

                string newJson = JsonSerializer.Serialize(AppService.PathEmu,AppJsonContext.Default.EmulationPaths);

                File.WriteAllText(AppService.PathsFile.Path, newJson);
                Console.WriteLine(newJson);
            }
            //Read the content and load it into the class
            else
            {
                AppService.PathEmu = JsonSerializer.Deserialize(readJson, AppJsonContext.Default.EmulationPaths);
            }
            AppService.EmuLoaded.SetResult(true);
        }
        /*
        /
         HANDLE DISPLAY
        /
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
        public void ButtonVisble(bool swtich)
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

        public async Task DisplayError(string message = "Disc Error")
        {

            ButtonVisble(true);
            AppService.GameReady = false;
            ImageBehavior.SetAnimatedSource(Disc, new BitmapImage(new Uri(
                "pack://application:,,,/EmuDiscReader;component/Assets/diskError.gif")));
            Description.Text = message;

            await Task.Run(() =>
            {
                MciSendString("set cdaudio door open", null, 0, IntPtr.Zero);   //Eject Disc
            });

            await Task.Delay(3220);

            ImageBehavior.SetAnimatedSource(Disc, new BitmapImage(new Uri(
                "pack://application:,,,/EmuDiscReader;component/Assets/spinningDisc.gif")));
            Description.Text = "Please insert a Disc";
        }

        /*
        /
        /
        /
        */

        private void SettingsBTN_Click(object sender, RoutedEventArgs e)
        {
            AppService.InSettings = true;
            Settings setWin = new();
            setWin.Owner = this;
            setWin.ShowDialog();
            AppService.InSettings = false;
            _ = pd.CheckDisc(this);
        }

        private async Task InitInstallBTN()
        {
            await AppService.EmuLoaded.Task;
            if (AppService.PathEmu is null) { return; }
            InstallBTN.IsChecked = AppService.PathEmu.WillCache;
            initInstall = false;
        }

        private async void InstallBTN_Checked(object sender, RoutedEventArgs e)
        {
            if(initInstall) { return; }
            await AppService.EmuLoaded.Task;
            if (AppService.PathEmu is null){ return; }
            AppService.PathEmu.WillCache = true;
            AppService.SaveJson();
            await pd.ReadyGame();
        }
        private async void InstallBTN_Unchecked(object sender, RoutedEventArgs e)
        {
            if (initInstall) { return; }
            await AppService.EmuLoaded.Task;
            if (AppService.PathEmu is null) { return; }
            AppService.PathEmu.WillCache = false;
            AppService.SaveJson();
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            pd.PlayGame();
        }
    }

    [JsonSourceGenerationOptions(WriteIndented = false)]
    [JsonSerializable(typeof(EmulationPaths))]
    internal partial class AppJsonContext : JsonSerializerContext { }
}