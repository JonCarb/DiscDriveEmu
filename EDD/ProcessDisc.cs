using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace EDD;

public class ProcessDisc
{

    private MainWindow? _mainWindow;
    private enum ProcessState { DiscNotChecked, DiscChecked, GamePrepared}
    private ProcessState _state = ProcessState.DiscNotChecked; //The states of processing the discs
    
    private App.DiscType _discType = App.DiscType.Custom;   //Holds what type of disc it is, custom, real xbox 360, real ps3, etc
    private App.Emulators _emulator = App.Emulators.None;   //Holds the name of which emulator is right for the game
    private string? _gamePath = null;                       //Holds the absolute path of the game file
    
    private bool _startingProcess = false;      //Meant to prevent spamming start, once it's trying to start, other attempts should be prevented in RunGame Function
    
    public void InitMainWindowForProcessDisc(MainWindow window)
    {
        _mainWindow = window;
    }

    public void ResetGlobalValues()
    {
        _gamePath = null;
        _emulator = App.Emulators.None;
        _discType =  App.DiscType.Custom;
        _state = ProcessState.DiscNotChecked;
        _startingProcess = false;
    }

    public async Task StartProcess(DriveInfo? discDrive)
    {
        if(discDrive == null|| _mainWindow == null || App.Services.Settings == null) return;
        Console.WriteLine("Start process called");
        
        ResetGlobalValues();
        
        await _mainWindow.ChangeDescription("Reading Disc");
        await _mainWindow.ChangeDiscGifToRun();

        _gamePath = await GetGamePathAndDiscType(discDrive.Name);
        if (_gamePath == null) { await _mainWindow.DisplayError("No games were found"); return;}
        Console.WriteLine($"Game Path: {_gamePath}");
            
        CheckForGameType cfgt = new();
        _emulator =  await cfgt.ChooseEmulator(_gamePath, _discType);
        if(_emulator == App.Emulators.None){ await _mainWindow.DisplayError("Couldn't find the right Emulator"); return;}
        Console.WriteLine($"Emulator: {_emulator}");
        
         _state = ProcessState.DiscChecked;
        
        await PrepareGame();
        if (App.Services.Settings.WillAutoStart) await RunGame(); 
        
        Console.WriteLine("Process Complete");
    }

    private async Task<string?> GetGamePathAndDiscType(string discDriveRoot)
    {
        //Checks for real ps2/ps3 discs else grabs the first file it sees
        string? fileName = await Task.Run(() =>
        {
            //PS2 Disc Check
            if (File.Exists(Path.Combine(discDriveRoot, "SYSTEM.CNF")))
            {
                _discType = App.DiscType.PS2; //Disc has ps2 game files
                return discDriveRoot;
            }
            
            //PS3 Disc Check
            if (File.Exists(Path.Combine(discDriveRoot, "PS3_DISC.SFB")) 
                && Directory.Exists(Path.Combine(discDriveRoot, "PS3_GAME")))
            {
                _discType = App.DiscType.PS3; //Disc has ps3 unpackaged game files
                return Path.Combine(discDriveRoot, "PS3_GAME");
            }

            //Xbox 360 Check
            if (Directory.Exists(Path.Combine(discDriveRoot, "VIDEO_TS")))
            {
                _discType = App.DiscType.Xbox360;
                return discDriveRoot;
            }
            
            //Else discType is custom, and we return the first file we can find
            return Directory.EnumerateFiles(discDriveRoot).FirstOrDefault();
        });
        
        return fileName;
    }
    

    //Will automatically be called by disc being/inserted. Can also be called when install button is TURNED on
    public async Task PrepareGame()
    {
        if (App.Services.Settings == null || _gamePath == null || _mainWindow == null || _state == ProcessState.DiscNotChecked) return;
        
        //if game is already prepared, move it one state back so user cant start the game
        if(_state == ProcessState.GamePrepared) _state = ProcessState.DiscChecked;
        
        //For specific discs like xbox 360, installation is needed to play
        if (_discType == App.DiscType.Xbox360 && !App.Services.Settings.WillCache)
        {
            await _mainWindow.DisplayError("You need to install this game to play!");
            return;
        }
        
        //Only apply if install button is on, skip if they can and want to play off disc
        if (App.Services.Settings.WillCache)
        {
            string? ret = null;
            Console.WriteLine("About to install");
            
            //Real discs need to be installed through MPF
            if (_discType != App.DiscType.Custom)
            {
                Console.WriteLine("Real disc");
                _mainWindow.InstallBtn.IsEnabled = false;
                ret = await InstallWithMPF();
                _mainWindow.InstallBtn.IsEnabled = true;
            }
            //"Copied" single file discs need are installed through custom installer
            else
            {
                ret = await InstallSingleFile();
            }
            
            //Handle errors that's returned
            if (ret == null)
            {
                await _mainWindow.DisplayError("Installation failed!");
                return;
            }
            
            _gamePath = ret;
        } 
        
        _state =  ProcessState.GamePrepared;
        await _mainWindow.ChangeDescription("Press Start To Play");
    }

    private async Task<string?> InstallSingleFile()
    {
        if(_mainWindow == null || _gamePath == null) return null;
        
        var install = new Install();
        _mainWindow.InstallBar.IsIndeterminate = false;
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

        return installedCopy;
    }

    private async Task<string?> InstallWithMPF()
    {
        if (_mainWindow == null || _gamePath == null) return null;
        if(_discType == App.DiscType.Custom) return null;
        
        _mainWindow.InstallBar.IsIndeterminate = true;
        _mainWindow.InstallBar.IsVisible = true;
        await _mainWindow.ChangeDescription($"Reading {_discType.ToString()} game");
        
        var source = new DriveInfo(_gamePath);
        string? path = await Install.CallMPF(_discType.ToString(), source);
        _mainWindow.InstallBar.IsVisible = false;
        return path;
    }

    //Only Autorun or pressing start can make this function run
    public async Task RunGame()
    {
        if (_mainWindow == null || App.Services.Settings == null  || _gamePath == null) return;
        
        //If the state is anything BUT prepared, don't allow it to continue
        if(_state != ProcessState.GamePrepared){ Console.WriteLine("Disc game is not ready!"); return; }
        
        //Prevents button spams
        if(_startingProcess) return;
        _startingProcess = true;
        
        switch (_emulator)
            {
                case App.Emulators.Dolphin:
                    if (!File.Exists(App.Services.Settings.Dolphin)) { await _mainWindow.DisplayError("NO DOLPHIN (GC/WII) PATH"); return; }
                    System.Diagnostics.Process.Start(App.Services.Settings.Dolphin, "-b " + "-e " + "\"" + _gamePath + "\"");
                    break;
                case App.Emulators.PCSX2:
                    if (!File.Exists(App.Services.Settings.PCSX2)) { await _mainWindow.DisplayError("NO PCSX2 (PS2) PATH"); return; }
                    var discArg = " ";
                    if(_discType == App.DiscType.PS2) discArg = "-disc ";
                    System.Diagnostics.Process.Start(App.Services.Settings.PCSX2, " -fullscreen " + discArg + "\"" + _gamePath + "\"");
                    break;
                case App.Emulators.Xemu:
                    if (!File.Exists(App.Services.Settings.Xemu)) { await _mainWindow.DisplayError("NO XEMU (XBOX) PATH"); return; }
                    System.Diagnostics.Process.Start(App.Services.Settings.Xemu, " -full-screen " + " -dvd_path " + "\"" + _gamePath + "\"");
                    break;
                case App.Emulators.Cemu:
                    if (!File.Exists(App.Services.Settings.Cemu)) { await _mainWindow.DisplayError("NO CEMU (WII U) PATH"); return; }
                    System.Diagnostics.Process.Start(App.Services.Settings.Cemu, " -g " + "\"" + _gamePath + "\"" + " -f");
                    break;
                case App.Emulators.RPCS3:
                    if (!File.Exists(App.Services.Settings.RPCS3)) { await _mainWindow.DisplayError("NO RPCS3 (PS3) PATH"); return; }
                    System.Diagnostics.Process.Start(App.Services.Settings.RPCS3, " " + "\"" + _gamePath + "\"");
                    break;
                case App.Emulators.Xenia:
                    if (!File.Exists(App.Services.Settings.Xenia)) { await _mainWindow.DisplayError("NO Xenia (XBOX 360) PATH"); return; }
                    System.Diagnostics.Process.Start(App.Services.Settings.Xenia, "--fullscreen " + "\"" + _gamePath + "\"");
                    break;
                default:
                    await _mainWindow.DisplayError("Unknown/Unsupported Disc"); 
                    return;
            }
        
        await _mainWindow.ChangeDescription("Starting Game");
        await Task.Delay(7000);
        _mainWindow.ShutDown();
    }
}