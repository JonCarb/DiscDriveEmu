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
        private const uint WM_DEVICECHANGE = 0x0219;
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;

        public static class AppService
        {
            public static StorageFolder DocumentsFolder { get; } = KnownFolders.DocumentsLibrary;
            public static StorageFile? PathsFile { get; set; }
            public static EmuPath? PathEmu { get; set; }
            public static bool CacheGame { get; set; }
            public static bool InSettings { get; set; }
            public static TaskCompletionSource<bool> EmuLoaded { get; } = new();
            //add more if needed
        }

        private ProcessDisc pd;
        public MainWindow()
        {
            InitializeComponent();
            LoadEmulatorJson();
            AppService.InSettings = false;

            AppService.CacheGame = false; //set true to cache

            pd = new ProcessDisc();
            pd.InitForm(this);
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
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_DEVICECHANGE)
            {
                switch (m.WParam.ToInt32())
                {
                    case DBT_DEVICEARRIVAL:
                        pd.InitForm(this);
                        break;

                    case DBT_DEVICEREMOVECOMPLETE:
                        DisplayError("Disc Tray Was Opened");
                        break;
                }
            }
            base.WndProc(ref m);
        }

        /*
         HANDLE DISPLAY
        */
        public void ChangeDesc(string message)
        {
            Description.Text = message;
        }
        public void MakeDiscImgRun()
        {
            Disc.Image = Properties.Resources.readingDisc;
        }
        public void turnOffOnSetting(bool swtich)
        {
            if (swtich)
            {
                SettingButton.Enabled = true;
                SettingButton.Visible = true;
            }
            else
            {
                SettingButton.Enabled = false;
                SettingButton.Visible = false;
            }
        }

        public async void DisplayError(string message = "Disc Error")
        {
            turnOffOnSetting(true);
            Disc.Image = Properties.Resources.diskError;
            Description.Text = message;

            await Task.Delay(3220);
            Disc.Image = Properties.Resources.spinningDisc;
            Description.Text = "Please insert a Disc";
        }
        /*
         * 
         */

        private void SettingButton_Click(object sender, EventArgs e)
        {
            AppService.InSettings = true;
            Settings settingsForm = new Settings();
            settingsForm.Location = this.Location;
            settingsForm.StartPosition = FormStartPosition.CenterParent;

            settingsForm.FormClosing += (s, args) => {
                this.Show();
                AppService.InSettings = false;
                pd.InitForm(this);
            };
            settingsForm.Show();
            this.Hide();
        }
    }
    [JsonSourceGenerationOptions(WriteIndented = false)]
    [JsonSerializable(typeof(EmuPath))]
    internal partial class AppJsonContext : JsonSerializerContext { }
}
