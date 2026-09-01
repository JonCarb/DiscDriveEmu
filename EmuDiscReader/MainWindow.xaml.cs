using DiscUtils.Raw;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
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
        private ProcessDisc pd;
        public MainWindow()
        {
            InitializeComponent();
            Console.WriteLine("To you 5000 years from now");
            LoadEmulatorJson();
            AppService.CacheGame = true;

            pd = new ProcessDisc();
            pd.CheckDisc(this);
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
    }

    [JsonSourceGenerationOptions(WriteIndented = false)]
    [JsonSerializable(typeof(EmulationPaths))]
    internal partial class AppJsonContext : JsonSerializerContext { }
}