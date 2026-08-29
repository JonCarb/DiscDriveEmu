using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
namespace EmuDiscDriveGUI
{
    public partial class MainWindow : Form
    {
        public static class AppService
        {
            public static IntPtr HWND { get; set; }
            public static StorageFolder DocumentsFolder { get; } = KnownFolders.DocumentsLibrary;
            public static StorageFile? PathsFile { get; set; }
            public static EmuPath? PathEmu { get; set; }
            public static bool CasheGame { get; set; }
            public static TaskCompletionSource<bool> EmuLoaded { get; } = new();
            //add more if needed
        }

        public MainWindow()
        {
            InitializeComponent();
            AppService.HWND = this.Handle;
            LoadEmulatorJson();

            ProcessDisc pd = new ProcessDisc();
            pd.initForm(this);
            pd.runDisc();
        }

        private static async void LoadEmulatorJson()
        {
            Console.WriteLine("Collecting Emulator Paths");

            AppService.PathsFile = await AppService.DocumentsFolder.CreateFileAsync
                ("EmulatorPaths.json", CreationCollisionOption.OpenIfExists);

            string readJson = await FileIO.ReadTextAsync(AppService.PathsFile);
            Console.WriteLine(readJson);

            //Fill file if its empty
            if (string.IsNullOrWhiteSpace(readJson))
            {
                AppService.PathEmu = new EmuPath();

                string newJson = JsonSerializer.Serialize
                    (AppService.PathEmu, AppJsonContext.Default.EmuPath);

                File.WriteAllText(AppService.PathsFile.Path, newJson);
            }
            //Read the content and load it into the class
            else
            {
                AppService.PathEmu = JsonSerializer.Deserialize
                    (readJson, AppJsonContext.Default.EmuPath);
            }
            AppService.EmuLoaded.SetResult(true);
        }


        /*
         HANDLE DISPLAY
        */
        public void testDescription(string message)
        {
            Description.Text = message;
        }
        public async void DisplayError(string message = "Disc Error")
        {
            //Setting.IsEnabled = true;
            //Disc.Image = Image.FromFile("Assets\\diskError.gif");
            await Task.Delay(1000);
            Description.Text = message;

            await Task.Delay(2225);
            //Disc.Image = Image.FromFile("Assets\\spinningDisc.gif");
            Description.Text = "Please insert a Disc";
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {

        }
    }
    [JsonSourceGenerationOptions(WriteIndented = false)]
    [JsonSerializable(typeof(EmuPath))]
    internal partial class AppJsonContext : JsonSerializerContext { }
}
