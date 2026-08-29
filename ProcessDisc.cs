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

        private MainWindow form;

        public void initForm(MainWindow f)
        {
            this.form = f;
        }

        public async void runDisc()
        {
            //Disable Settings Button
            form.DisplayError("NO DIGAS TOLEIT!");

            /*
            bool ready = await Task.Run(() => WaitForDriveReady(discDrive, TimeSpan.FromSeconds(10)));
            if (!ready)
            {
                //DisplayError("Drive not ready");
                return;
            }

            if (await GetGamePath())
            {
                string emu = await chooseEmulator();
                //RunGame(emu);
            }
            else
            {
                //DisplayError("Couldnt get game path");
            }
            */
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
            //Description.Text = "Couldnt Find a Game";
            return false;
        }

        private async Task<string> chooseEmulator()
        {
            Debug.WriteLine("Choose EMU Called");

            if (isRealPS2Game) { return "PCSX2"; } //IF its a real ps2 disc, just return ps2
            if (nonIsoPS3Game) { return "RPCS3"; } //No need to check ISO, return for ps3

            CheckGameType CGT = new CheckGameType();
            string emuName = "NONE";
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
    }

}
