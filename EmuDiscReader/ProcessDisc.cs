using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Storage;
using static EmuDiscReader.MainWindow;

namespace EmuDiscReader
{
    class ProcessDisc
    {
        private string gamePath = "NULL";   //Holds the absolute path to run with (potential) emulator
        private string gameName = "NULL";   //Only holds the game file name
        private string discDrive = "NULL";  //Holds the Root Path of the disc drive since its diff for every computer
        private string emulator = "NULL";
        private string discArg = " ";       //Only used for ps2 cmd args
        private bool isRealPS2Game = false;
        private bool nonIsoPS3Game = false;
        private bool discChecked = false;
        private bool isBusy = false;
        private MainWindow? MainForm;
        public async Task CheckDisc(MainWindow f)
        {
            if (AppService.InSettings) { return; }
            this.MainForm = f;

            await AppService.EmuLoaded.Task;

            bool driveExists = await Task.Run(() =>
            {
                discDrive = DriveInfo.GetDrives().FirstOrDefault(d => d.DriveType == DriveType.CDRom)?.Name ?? "NULL";
                return discDrive != "NULL" && Directory.Exists(discDrive);
            });

            if (driveExists) { await RunDisc(); }
        }
        public void ResetValues()
        {
            isRealPS2Game = false;
            nonIsoPS3Game = false;
            discChecked = false;
            gamePath = "NULL";
            gameName = "NULL";
            discArg = " ";
            emulator = "NULL";
        }
        private async Task RunDisc()
        {
            if (MainForm == null) { Console.WriteLine("Form is null"); return; }

            //Disable Settings Button
            MainForm.ButtonVisble(false);

            await Task.Run(() =>
            {
                discDrive = DriveInfo.GetDrives().FirstOrDefault(d => d.DriveType == DriveType.CDRom)?.Name ?? discDrive; //Quick check
            });

            bool ready = await Task.Run(() => WaitForDriveReady(discDrive, TimeSpan.FromSeconds(10)));

            if (!ready)
            {
                await MainForm.DisplayError("Drive not ready");
                return;
            }

            if (await GetGamePath())
            {
                Console.WriteLine(gamePath);
                emulator = await ChooseEmulator();
                discChecked = true;
                await ReadyGame();
            }
            else
            {
                await MainForm.DisplayError("Couldnt get game path");
                MainForm.ButtonVisble(true);
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
            //Reset values in case disc get swapped
            ResetValues();

            if (MainForm == null) { Console.WriteLine("Form is null"); return false; }
            string? fileName = null;

            await Task.Run(() =>
            {
                fileName = Directory.EnumerateFiles(discDrive).FirstOrDefault();
                if (File.Exists(Path.Combine(discDrive, "SYSTEM.CNF"))) 
                { 
                    isRealPS2Game = true; //Disc has ps2 game files
                }  
                if (File.Exists(Path.Combine(discDrive, "PS3_DISC.SFB")) 
                    && Directory.Exists(Path.Combine(discDrive, "PS3_GAME")))
                {
                    nonIsoPS3Game = true; //Disc has ps3 unpackaged game files
                }
            });

            if (isRealPS2Game)
            {
                gamePath = discDrive;   //For non-iso/single file - Give root of drive, emulators will know what to do
                discArg = "-disc ";     //important for real ps2 disc
                return true;
            }
            if (nonIsoPS3Game)
            {
                gamePath = Path.Combine(discDrive, "PS3_GAME");
                Console.WriteLine(gamePath);
                return true;
            }

            if (fileName != null)
            {
                gameName = Path.GetFileName(fileName);
                gamePath = fileName;
                return true;
            }
            MainForm.ChangeDesc("Couldnt Find a Game");
            return false;
        }

        private async Task<string> ChooseEmulator()
        {
            Console.WriteLine("Choose EMU Called");

            if (isRealPS2Game) { return "PCSX2"; } //IF its a real ps2 disc, just return ps2
            if (nonIsoPS3Game) { return "RPCS3"; } //No need to check ISO, return for ps3

            string emuName = "NONE";
            CheckGameType CGT = new();
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
        public async Task ReadyGame()
        {
            if (MainForm == null) { Console.WriteLine("Form is null"); return; }
            if (AppService.PathEmu == null) { await MainForm.DisplayError("Path Class is NULL!"); return; }

            if (!discChecked)
            {
                Console.WriteLine("No disc has been checked yet.");
                return;
            }
            if (emulator == "NULL" || emulator == "NONE")
            {
                await MainForm.DisplayError("Unknown/Unsupported Disc");
                return;
            }

            MainForm.ChangeDesc("Reading Disc");
            MainForm.MakeDiscImgRun(); 

            //Wont cache real PS2 Discs or certain ps3 formats, only ISO and single file formats
            if ( AppService.PathEmu.WillCache == true && !isRealPS2Game && !nonIsoPS3Game) 
            {
                MainForm.ButtonVisble(false);   //Disable settings for noww
                Console.WriteLine($"About to cache: Game Name: {gameName} game Path: {gamePath}");
                Cache ca = new();
                MainForm.ChangeDesc("Installing Game");
                MainForm.CacheBarVis(true);
                MainForm.InstallBar.Value = 0;
                MainForm.InstallBTN.Content = "Cancel Install";

                var progress = new Progress<double>(percentage =>
                {
                    MainForm.InstallBar.Value = percentage;
                    MainForm.ChangeDesc($"Installing Game {percentage:F0}%");
                });

                StorageFile? copy = await Task.Run(async () =>
                {
                    return await ca.CacheGame(gameName, gamePath, progress);
                });

                MainForm.CacheBarVis(false);
                MainForm.InstallBTN.Content = "⬇️ Install Game";

                if (copy is null)
                {
                    ResetValues(); //Disc tray gets opened so reset values to not allow broken installs
                    await MainForm.DisplayError("Cache Error/Interupted!");
                    MainForm.ButtonVisble(true);
                    return;
                }

                gamePath = copy.Path;
            }

            AppService.GameReady = true;
            MainForm.DescPrefix.Text = " Press";
            MainForm.DescIcon.Visibility = Visibility.Visible;
            MainForm.DescSuffix.Text = "to Start";
            MainForm.ButtonVisble(true);
        }
        public async void PlayGame()
        {
            if (MainForm == null) { Console.WriteLine("Form is null"); return; }
            if (AppService.PathEmu == null) { await MainForm.DisplayError("Path Class is NULL!"); return; }
            if(AppService.GameReady == false) { Console.WriteLine("Disc not ready!"); return; }
            if(isBusy) { return; }

            isBusy = true;
            switch (emulator)
            {
                case "DOLPHIN":
                    if (File.Exists(AppService.PathEmu.Dolphin) == false) { await MainForm.DisplayError("NO DOLPHIN (GC/WII) PATH"); return; }
                    System.Diagnostics.Process.Start(AppService.PathEmu.Dolphin, "-b " + "-e " + "\"" + gamePath + "\"");
                    break;
                case "PCSX2":
                    if (File.Exists(AppService.PathEmu.PCSX2) == false) { await MainForm.DisplayError("NO PCSX2 (PS2) PATH"); return; }
                    System.Diagnostics.Process.Start(AppService.PathEmu.PCSX2, " -fullscreen " + discArg + "\"" + gamePath + "\"");
                    break;
                case "XEMU":
                    if (File.Exists(AppService.PathEmu.Xemu) == false) { await MainForm.DisplayError("NO XEMU (XBOX) PATH"); return; }
                    System.Diagnostics.Process.Start(AppService.PathEmu.Xemu, " -full-screen " + " -dvd_path " + "\"" + gamePath + "\"");
                    break;
                case "CEMU":
                    if (File.Exists(AppService.PathEmu.Cemu) == false) { await MainForm.DisplayError("NO CEMU (WII U) PATH"); return; }
                    System.Diagnostics.Process.Start(AppService.PathEmu.Cemu, " -g " + "\"" + gamePath + "\"" + " -f");
                    break;
                case "RPCS3":
                    if (File.Exists(AppService.PathEmu.RPCS3) == false) { await MainForm.DisplayError("NO RPCS3 (PS3) PATH"); return; }
                    System.Diagnostics.Process.Start(AppService.PathEmu.RPCS3, " " + "\"" + gamePath + "\"");
                    break;
                default:
                    isBusy = false;
                    await MainForm.DisplayError("Unknown/Unsupported Disc"); return;
            }
            MainForm.ChangeDesc("Starting Game");
            await Task.Delay(2000);
            MainForm.Quit();
        }
    }
}
