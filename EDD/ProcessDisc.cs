using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace EDD;

public class ProcessDisc
{
    private MainWindow? _mainWindow;
    
    private string? _gamePath = null;       //Holds the absolute path of the game
    private string? _emulator = "NONE";     //Holds the path of which emulator is right for the gaame
    
    private bool _isDiscChecked = false;    //A checkpoint to prevent installation when disc hasn't been read yet
    private bool _isGameReady = false;      //A checkpoint to prevent starting game during installation
    private bool _runningGame = false;      //Meant to prevent spamming start, once it's trying to start, other attempts should be prevented
    
    private bool _isRealPS2Disc =  false;
    private bool _isRealPS3Disc =  false;

    public void InitMainWindowForProcessDisc(MainWindow window)
    {
        _mainWindow = window;
    }

    public void ResetGlobalValues()
    {
        _gamePath = null;
        _emulator = "NONE";
        
        _isDiscChecked = false;
        _isGameReady = false;
        _runningGame = false;
        
        _isRealPS2Disc =  false;
        _isRealPS3Disc =  false;
    }
    
    public async Task CheckDiscDrive()
    {
        if(_mainWindow == null) return;
        DriveInfo? discDrive = null;
        var driveExists = await Task.Run(() =>
        {
            discDrive = DriveInfo.GetDrives().FirstOrDefault(d => d.DriveType == DriveType.CDRom);
            return discDrive != null && Directory.Exists(discDrive.Name);
        });
        if (driveExists && discDrive is { IsReady: true }) { await StartProcess(discDrive); }
    }

    private async Task StartProcess(DriveInfo discDrive)
    {
        if(_mainWindow == null || App.Services.Settings == null) return;
        Console.WriteLine("Start process called");
        Console.WriteLine("Disc Drive Name: " + discDrive.Name);
        //add a wait here 
        
        await _mainWindow.ChangeDescription("Reading Disc");
        await _mainWindow.ChangeDiscGifToRun();
        
        if (await GetGamePath(discDrive.Name))
        {
            _emulator =  await ChooseEmulator();
            _isDiscChecked = true;
            
            await PrepareGame();
            if (App.Services.Settings.WillAutoStart) await RunGame(); 
        }    
        else
        {
            await _mainWindow.DisplayError("No games files found");
        }
    }

    private async Task<bool> GetGamePath(string discDriveRoot)
    {
        //Checks for real ps2/ps3 discs else grabs the first file it sees
        var fileName = await Task.Run(() =>
        {
            if (File.Exists(Path.Combine(discDriveRoot, "SYSTEM.CNF"))) 
            { 
                _isRealPS2Disc = true; //Disc has ps2 game files
                return discDriveRoot;
            }  
            if (File.Exists(Path.Combine(discDriveRoot, "PS3_DISC.SFB")) 
                && Directory.Exists(Path.Combine(discDriveRoot, "PS3_GAME")))
            {
                _isRealPS3Disc = true; //Disc has ps3 unpackaged game files
                return Path.Combine(discDriveRoot, "PS3_GAME");
            }
            return Directory.EnumerateFiles(discDriveRoot).FirstOrDefault();
        });
        
        if (fileName == null) return false;
        _gamePath = fileName; 
        Console.WriteLine(_gamePath);
        return true;
    }
    
    private async Task<string> ChooseEmulator()
    {
        string emuName = "NONE";
        if (_gamePath == null) return emuName;
        if (_isRealPS2Disc) return "PCSX2";
        if (_isRealPS3Disc) return "RPCS3";
        CheckForGameType cfgt = new();
        await Task.Run(() =>
        {
            string fileEx = Path.GetExtension(_gamePath).ToUpper();
            if (fileEx == ".ISO")
            {
                if (cfgt.ReadGCWII(_gamePath)) { emuName = "DOLPHIN"; }       //reads 1 byte
                else if (cfgt.ReadPS3(_gamePath)) { emuName = "RPCS3"; }      //seeks and reads 12 bytes
                else if (cfgt.ReadXBOX(_gamePath)) { emuName = "XEMU"; }      //seeks and reads 20 bytes
                else if (cfgt.ReadXBOX360(_gamePath)) { emuName = "XENIA"; }  //seeks and reads 20 bytes
                else if (cfgt.ReadPS2(_gamePath)) { emuName = "PCSX2"; }      //converts whole iso (twice maybe) and checks (expensive so last case)
                //Add other emulators
            }
            else
            {
                //Add method for Non-iso files (Wii u, etc.)
                if (cfgt.ReadWiiU(fileEx)) { emuName = "CEMU"; }                 //Check for CEMU supported extensions
                else if (cfgt.ReadGCWIINonIso(fileEx)) { emuName = "DOLPHIN"; }  //Checks for dolphin supported extensions thats not ISO
                else if(cfgt.ReadXbox360NonIso(fileEx)) { emuName = "XENIA"; }   //Checks if file is xex for Xenia
            }
        });
        return emuName;
    }

    //Will automatically be called by disc being/inserted. Can also be called when install button is TURNED on
    public async Task PrepareGame()
    {
        if (App.Services.Settings == null || _gamePath == null || _mainWindow == null) return;
        if(!_isDiscChecked)
        {
            Console.WriteLine("No disc has been checked yet.");
            return;
        }
        _isGameReady = false;
        if (App.Services.Settings.WillCache)
        {
            if (_isRealPS2Disc || _isRealPS3Disc)
            {
                await _mainWindow.ChangeDescription("Sorry these types of discs cannot be installed");
                await Task.Delay(2500);
                await _mainWindow.ChangeDescription("Press Start To Play");
                _isGameReady = true;
                return;
            }
            
            var install = new Install();
            
            _mainWindow.InstallBar.IsVisible = true;
            _mainWindow.InstallBar.Value = 0;
            
            _ = _mainWindow.ChangeDescription($"Installing Game");
            
            var progress = new Progress<double>(async void (percentage) =>
            {
                _mainWindow.InstallBar.Value = percentage;
                await _mainWindow.ChangeDescription($"Installing Game {percentage:F0}%");
            });
            
            var installedCopy = await install.InstallGame(_gamePath, progress);
           
            _mainWindow.InstallBar.IsVisible = false;
            
            if (installedCopy == null)
            {
                await _mainWindow.DisplayError("Cache Error/Interrupted!");
                return;
            }
            _gamePath =  installedCopy;
        }
        _isGameReady = true;
        await _mainWindow.ChangeDescription("Press Start To Play");
    }

    //Only Autorun or pressing start can make this function run
    public async Task RunGame()
    {
        if (_mainWindow == null || App.Services.Settings == null || !_isDiscChecked || _gamePath == null) return;
        
        if(!_isGameReady){ 
            Console.WriteLine("Disc game is not ready!");
            return; 
        }
        
        //Prevents button spams
        if(_runningGame) return;
        _runningGame = true;
        
        switch (_emulator)
            {
                case "DOLPHIN":
                    if (!File.Exists(App.Services.Settings.Dolphin)) { await _mainWindow.DisplayError("NO DOLPHIN (GC/WII) PATH"); return; }
                    System.Diagnostics.Process.Start(App.Services.Settings.Dolphin, "-b " + "-e " + "\"" + _gamePath + "\"");
                    break;
                case "PCSX2":
                    if (!File.Exists(App.Services.Settings.PCSX2)) { await _mainWindow.DisplayError("NO PCSX2 (PS2) PATH"); return; }
                    var discArg = " ";
                    if(_isRealPS2Disc) discArg = "-disc ";
                    System.Diagnostics.Process.Start(App.Services.Settings.PCSX2, " -fullscreen " + discArg + "\"" + _gamePath + "\"");
                    break;
                case "XEMU":
                    if (!File.Exists(App.Services.Settings.Xemu)) { await _mainWindow.DisplayError("NO XEMU (XBOX) PATH"); return; }
                    System.Diagnostics.Process.Start(App.Services.Settings.Xemu, " -full-screen " + " -dvd_path " + "\"" + _gamePath + "\"");
                    break;
                case "CEMU":
                    if (!File.Exists(App.Services.Settings.Cemu)) { await _mainWindow.DisplayError("NO CEMU (WII U) PATH"); return; }
                    System.Diagnostics.Process.Start(App.Services.Settings.Cemu, " -g " + "\"" + _gamePath + "\"" + " -f");
                    break;
                case "RPCS3":
                    if (!File.Exists(App.Services.Settings.RPCS3)) { await _mainWindow.DisplayError("NO RPCS3 (PS3) PATH"); return; }
                    System.Diagnostics.Process.Start(App.Services.Settings.RPCS3, " " + "\"" + _gamePath + "\"");
                    break;
                case "XENIA":
                    if (!File.Exists(App.Services.Settings.Xenia)) { await _mainWindow.DisplayError("NO Xenia (XBOX 360) PATH"); return; }
                    System.Diagnostics.Process.Start(App.Services.Settings.Xenia, "--fullscreen " + "\"" + _gamePath + "\"");
                    break;
                default:
                    await _mainWindow.DisplayError("Unknown/Unsupported Disc"); return;
            }
        await _mainWindow.ChangeDescription("Starting Game");
        await Task.Delay(2000);
        _mainWindow.ShutDown();
    }
}