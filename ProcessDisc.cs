using DiscUtils.Raw;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using static EmuDiscDriveGUI.MainWindow;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EmuDiscDriveGUI
{
    internal class ProcessDisc
    {
        private string gamePath = "NULL";   //Holds the absolute path to run with (potential) emulator
        private string gameName = "NULL";   //Only holds the game file name
        private string discDrive = "NULL";  //Holds the Root Path of the disc drive since its diff for every computer
        private string discArg = " ";       //Only used for ps2 cmd args
        private bool isRealPS2Game = false;
        private bool nonIsoPS3Game = false;

        private MainWindow? form;

        public async void InitForm(MainWindow f)
        {
            if (AppService.InSettings) { return; }
            this.form = f;

            await AppService.EmuLoaded.Task;

            await Task.Delay(1000); //Give UI thread time to init 

            await Task.Run(() =>
            {
                discDrive = DriveInfo.GetDrives().FirstOrDefault(d => d.DriveType == DriveType.CDRom)?.Name ?? "NULL";
            });
            //Check to see if disc is already in, otherwise wait for event
            if ((discDrive != "NULL") && (Directory.Exists(discDrive) == true))
            {
                RunDisc();
            }
        }

        public async void RunDisc()
        {
            if(form == null) { Console.WriteLine("Form is null"); return; }

            //Disable Settings Button
            form.turnOffOnSetting(false);

            await Task.Run(() =>
            {
                discDrive = DriveInfo.GetDrives().FirstOrDefault(d => d.DriveType == DriveType.CDRom)?.Name ?? discDrive; //Quick check
            });

            bool ready = await Task.Run(() => WaitForDriveReady(discDrive, TimeSpan.FromSeconds(10)));

            if (!ready)
            {
                form.DisplayError("Drive not ready");
                return;
            }

            if (await GetGamePath())
            {
                Console.WriteLine(gamePath);
                string emu = await ChooseEmulator();
                RunGame(emu);
            }
            else
            {
                form.DisplayError("Couldnt get game path");
            }
        }

        private bool WaitForDriveReady(string driveName, TimeSpan timeout)
        {
            var drive = new DriveInfo(driveName);
            var sw = Stopwatch.StartNew();

            while (!drive.IsReady && sw.Elapsed < timeout)
            {
                Thread.Sleep(250);
                drive = new DriveInfo(driveName);
            }

            return drive.IsReady;
        }
        private async Task<bool> GetGamePath()
        {
            if (form == null) { Console.WriteLine("Form is null"); return false; }
            string? fileName = null;

            await Task.Run(() =>
            {
                fileName = Directory.EnumerateFiles(discDrive).FirstOrDefault();
                if (File.Exists(Path.Combine(discDrive, "SYSTEM.CNF"))) { isRealPS2Game = true; }   //Disc has ps2 game files
                if (File.Exists(Path.Combine(discDrive, "PS3_DISC.SFB"))) { nonIsoPS3Game = true; } //Disc has ps3 unpackaged game files
            });

            if (isRealPS2Game)
            {
                gamePath = discDrive;   //For non-iso/single file - Give root of drive, emulators will know what to do
                discArg = "-disc ";     //important for real ps2 disc
                return true;
            }
            if (nonIsoPS3Game)
            {
                gamePath = discDrive + "PS3_GAME";
                Console.WriteLine(gamePath);
                return true;
            }

            if (fileName != null)
            {
                gameName = Path.GetFileName(fileName);
                gamePath = fileName;
                return true;
            }
            form.ChangeDesc("Couldnt Find a Game");
            return false;
        }

        private async Task<string> ChooseEmulator()
        {
            Debug.WriteLine("Choose EMU Called");

            if (isRealPS2Game) { return "PCSX2"; } //IF its a real ps2 disc, just return ps2
            if (nonIsoPS3Game) { return "RPCS3"; } //No need to check ISO, return for ps3

            string emuName = "NONE";
            CheckGameType CGT = new CheckGameType();
            await Task.Run(() =>
            {
                string fileEx = Path.GetExtension(gamePath).ToUpper();
                if (fileEx == ".ISO")
                {
                    if (CGT.ReadGCWII(gamePath)) { emuName = "DOLPHIN"; }   //reads 1 byte
                    else if (CGT.ReadXBOX(gamePath)) { emuName = "XEMU"; }  //seeks and reads 20 bytes
                    else if (CGT.ReadPS3(gamePath)) { emuName = "RPCS3"; }  //seeks and reads 12 bytes
                    else if (CGT.ReadPS2(gamePath)) { emuName = "PCSX2"; }  //converts whole iso (twice maybe) and checks (expensive so last case)
                    //Add other emulators
                }
                else
                {
                    //Add method for Non-iso files (Wii u, etc)
                    if (CGT.ReadWiiU(fileEx)) { emuName = "CEMU"; }             //Check for CEMU supported extensions
                    if (CGT.ReadGCWIINonIso(fileEx)) { emuName = "DOLPHIN"; }   //Checks for dolphin supported extensions thats not ISO
                }
            });

            return emuName;
        }

        private async void RunGame(string emulator)
        {
            if(form == null) { Console.WriteLine("Form is null"); return; }
            if (AppService.PathEmu == null) { form.DisplayError("Path Class is NULL!"); return; }

            form.ChangeDesc("Reading Disc");
            form.MakeDiscImgRun();

            if (AppService.CacheGame == true && !isRealPS2Game && !nonIsoPS3Game) //Wont cache real PS2 Discs or certain ps3 discs, only ISO (for now)
            {
                Console.WriteLine("About to cache: Game Name: " + gameName + " game Path: " + gamePath);
                Cache ca = new Cache();

                //CacheProgressBar.Visibility = Visibility.Visible;
                //CacheProgressBar.Value = 0;

                var progress = new Progress<double>(percentage =>
                {
                    //CacheProgressBar.Value = percentage;
                    form.ChangeDesc($"Installing Disc {percentage:F0}%");
                });

                StorageFile? copy = await Task.Run(async () =>
                {
                    return await ca.CacheGame(gameName, gamePath, progress);
                });

                //CacheProgressBar.Visibility = Visibility.Collapsed;

                if (copy is null)
                {
                    form.DisplayError("Cache Error: Playing off Disc!");
                    return;
                }

                gamePath = copy.Path;
            }

            await Task.Delay(500); //Force people into seeing my cool animation

            switch (emulator)
            {
                case "DOLPHIN":
                    if (File.Exists(AppService.PathEmu.Dolphin) == false) { form.DisplayError("NO DOLPHIN (GC/WII) PATH"); return; }
                    System.Diagnostics.Process.Start(AppService.PathEmu.Dolphin, "-b " + "-e " + "\"" + gamePath + "\"");
                    break;
                case "PCSX2":
                    if (File.Exists(AppService.PathEmu.PCSX2) == false) { form.DisplayError("NO PCSX2 (PS2) PATH"); return; }
                    System.Diagnostics.Process.Start(AppService.PathEmu.PCSX2, " -fullscreen " + discArg + "\"" + gamePath + "\"");
                    break;
                case "XEMU":
                    if (File.Exists(AppService.PathEmu.Xemu) == false) { form.DisplayError("NO XEMU (XBOX) PATH"); return; }
                    System.Diagnostics.Process.Start(AppService.PathEmu.Xemu, " -full-screen " + " -dvd_path " + "\"" + gamePath + "\"");
                    break;
                case "CEMU":
                    if (File.Exists(AppService.PathEmu.Cemu) == false) { form.DisplayError("NO CEMU (WII U) PATH"); return; }
                    System.Diagnostics.Process.Start(AppService.PathEmu.Cemu, " -g " + "\"" + gamePath + "\"" + " -f");
                    break;
                case "RPCS3":
                    if (File.Exists(AppService.PathEmu.RPCS3) == false) { form.DisplayError("NO RPCS3 (PS3) PATH"); return; }
                    System.Diagnostics.Process.Start(AppService.PathEmu.RPCS3, " " + "\"" + gamePath + "\"");
                    break;
                default:
                    form.DisplayError("Unknown/Unsupported Disc Type"); return;
            }
            await Task.Delay(2000);
            Application.Exit();
        }
    }
}
