using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using static EmuDiscReader.MainWindow;

namespace EmuDiscReader
{
    class ProcessDisc
    {
        private string gamePath = "NULL";   //Holds the absolute path to run with (potential) emulator
        private string gameName = "NULL";   //Only holds the game file name
        private string discDrive = "NULL";  //Holds the Root Path of the disc drive since its diff for every computer
        private string discArg = " ";       //Only used for ps2 cmd args
        private bool isRealPS2Game = false;
        private bool nonIsoPS3Game = false;

        private MainWindow? MainForm;
        public async void CheckDisc(MainWindow f)
        {
            if (AppService.InSettings) { return; }
            this.MainForm = f;

            await AppService.EmuLoaded.Task;

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
        private async void RunDisc()
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
                MainForm.DisplayError("Drive not ready");
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
                MainForm.DisplayError("Couldnt get game path");
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
            if (MainForm == null) { Console.WriteLine("Form is null"); return false; }
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
            MainForm.ChangeDesc("Couldnt Find a Game");
            return false;
        }

        private async Task<string> ChooseEmulator()
        {
            Debug.WriteLine("Choose EMU Called");

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
        private async void RunGame(string emulator)
        {
            if (MainForm == null) { Console.WriteLine("Form is null"); return; }
            if (AppService.PathEmu == null) { MainForm.DisplayError("Path Class is NULL!"); return; }

            MainForm.ChangeDesc("Reading Disc");
            MainForm.MakeDiscImgRun();

            if ( AppService.PathEmu.WillCache == true && !isRealPS2Game && !nonIsoPS3Game) //Wont cache real PS2 Discs or certain ps3 discs, only ISO and single file formats (for now)
            {
                Console.WriteLine("About to cache: Game Name: " + gameName + " game Path: " + gamePath);
                Cache ca = new();
                MainForm.ChangeDesc("Installing Game");
                MainForm.CacheBarVis(true);
                MainForm.InstallBarValue(0);
                var progress = new Progress<double>(percentage =>
                {
                    MainForm.InstallBarValue(percentage);
                    MainForm.ChangeDesc($"Installing Game {percentage:F0}%");
                });

                StorageFile? copy = await Task.Run(async () =>
                {
                    return await ca.CacheGame(gameName, gamePath, progress);
                });

                MainForm.CacheBarVis(false);

                if (copy is null)
                {
                    MainForm.DisplayError("Cache Error/Interupted!");
                    await Task.Run(() =>
                    {
                        MainForm.EjectDiscDrive();
                    });
                    return;
                }

                gamePath = copy.Path;
            }

            await Task.Delay(500); //Force people into seeing my cool animation

            switch (emulator)
            {
                case "DOLPHIN":
                    if (File.Exists(AppService.PathEmu.Dolphin) == false) { MainForm.DisplayError("NO DOLPHIN (GC/WII) PATH"); return; }
                    System.Diagnostics.Process.Start(AppService.PathEmu.Dolphin, "-b " + "-e " + "\"" + gamePath + "\"");
                    break;
                case "PCSX2":
                    if (File.Exists(AppService.PathEmu.PCSX2) == false) { MainForm.DisplayError("NO PCSX2 (PS2) PATH"); return; }
                    System.Diagnostics.Process.Start(AppService.PathEmu.PCSX2, " -fullscreen " + discArg + "\"" + gamePath + "\"");
                    break;
                case "XEMU":
                    if (File.Exists(AppService.PathEmu.Xemu) == false) { MainForm.DisplayError("NO XEMU (XBOX) PATH"); return; }
                    System.Diagnostics.Process.Start(AppService.PathEmu.Xemu, " -full-screen " + " -dvd_path " + "\"" + gamePath + "\"");
                    break;
                case "CEMU":
                    if (File.Exists(AppService.PathEmu.Cemu) == false) { MainForm.DisplayError("NO CEMU (WII U) PATH"); return; }
                    System.Diagnostics.Process.Start(AppService.PathEmu.Cemu, " -g " + "\"" + gamePath + "\"" + " -f");
                    break;
                case "RPCS3":
                    if (File.Exists(AppService.PathEmu.RPCS3) == false) { MainForm.DisplayError("NO RPCS3 (PS3) PATH"); return; }
                    System.Diagnostics.Process.Start(AppService.PathEmu.RPCS3, " " + "\"" + gamePath + "\"");
                    break;
                default:
                    MainForm.DisplayError("Unknown/Unsupported Disc"); return;
            }
            MainForm.ChangeDesc("Starting Game");
            await Task.Delay(2000);
            System.Windows.Application.Current.Shutdown();
        }
    }
}
