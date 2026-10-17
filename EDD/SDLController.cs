using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using SDL3;
using Avalonia.Platform;
namespace EDD;

public class SDLController
{
    private readonly Dictionary<uint, IntPtr> _OGP = new();
    private MainWindow? _mainWindow;
    private string? _currGamepad = null;
    private DispatcherTimer elTick = new();
    
    public void IntiSDL(MainWindow window)
    {
        Task.Run(() =>
        {
            _mainWindow = window;
            var ret = SDL.Init(SDL.InitFlags.Gamepad);
            Console.WriteLine($"SDL init: {ret}");
            elTick.Tick += CheckPoll;
            elTick.Interval = new TimeSpan(0, 0, 0, 0, 16);
            elTick.Start();
        });
    }
    private void CheckPoll(object? sender, EventArgs e)
    {
        SDL.Event sdlEvent;
        while (SDL.PollEvent(out sdlEvent))
        {
            HandleEvent(sdlEvent);
        }
    }
    private bool HandleEvent(SDL.Event sdlEvent)
    {
        if(_mainWindow == null) { return false; }
        switch ((SDL.EventType)sdlEvent.Type)
        {
            case SDL.EventType.GamepadAdded:
                var gpa = SDL.OpenGamepad(sdlEvent.GDevice.Which);
                if (gpa != IntPtr.Zero)
                {
                    Console.WriteLine($"Gamepad added: {SDL.GetGamepadName(gpa)}");
                    if (_OGP.Count < 1) //Only change button displays for the first controller it gets
                    {
                        ChangeDisplayButtons(sdlEvent.GDevice.Which); 
                    }
                    _OGP[sdlEvent.GDevice.Which] = gpa;
                }
                break;
                case SDL.EventType.GamepadRemoved:
                    var gpr = SDL.GetGamepadFromID(sdlEvent.GDevice.Which);
                    if (gpr != IntPtr.Zero)
                    {
                        Console.WriteLine($"Gamepad Removed: {SDL.GetGamepadName(gpr)}");
                        _OGP.Remove(sdlEvent.GDevice.Which);
                        SDL.CloseGamepad(gpr);
                    }
                    break;
                case SDL.EventType.GamepadButtonDown:
                    uint which = sdlEvent.GButton.Which;
                    ChangeDisplayButtons(which);
                    var button = (SDL.GamepadButton)sdlEvent.GButton.Button;
                    switch (button)
                    {
                        case SDL.GamepadButton.North:       //Quit
                            //Console.WriteLine("Y pressed");
                            _mainWindow.ShutDown();
                            break;
                        case SDL.GamepadButton.East:        //Settings Button
                            //Console.WriteLine("B pressed");
                            _mainWindow.SettingClick(null,null);
                            break;
                        case SDL.GamepadButton.West:        //Cache/Install Game
                            //Console.WriteLine("X pressed");
                            _mainWindow.InstallSwitch();
                            break;
                        case SDL.GamepadButton.Start:       //Start Game
                            //Console.WriteLine("Start pressed");
                            _ = App.Services.PD.RunGame();
                            break;
                        case SDL.GamepadButton.LeftShoulder: //Auto start game
                            //Console.WriteLine("Left Shoulder pressed");
                            _mainWindow.AutoPlaySwitch();
                            break;
                    }
                    break;
        }
        return true;
    }
    
    private void ChangeDisplayButtons(uint which)
        {
            if(_mainWindow == null) return;
            var controllerType = SDL.GetGamepadStringForType(SDL.GetGamepadTypeForID(which));
            if (controllerType == null || _currGamepad == controllerType) return;
            _currGamepad = controllerType;
            
            switch (_currGamepad)
            {
                case "ps3":
                case "ps4":
                case "ps5":
                    _mainWindow.InstallBtnIcon.Source = new Bitmap(AssetLoader.Open(new Uri("avares://EDD/Assets/PS4Square.png")));
                    _mainWindow.SettingBtnIcon.Source = new Bitmap(AssetLoader.Open(new Uri("avares://EDD/Assets/PS4Circle.png")));
                    _mainWindow.QuitBtnIcon.Source = new Bitmap(AssetLoader.Open(new Uri("avares://EDD/Assets/PS4Triangle.png")));
                    _mainWindow.AutoPlayBtnIcon.Source = new Bitmap(AssetLoader.Open(new Uri("avares://EDD/Assets/PS4L1.png")));
                    break;
                default:
                    _mainWindow.InstallBtnIcon.Source = new Bitmap(AssetLoader.Open(new Uri("avares://EDD/Assets/Xbox360X.png")));
                    _mainWindow.SettingBtnIcon.Source = new Bitmap(AssetLoader.Open(new Uri("avares://EDD/Assets/Xbox360B.png")));
                    _mainWindow.QuitBtnIcon.Source = new Bitmap(AssetLoader.Open(new Uri("avares://EDD/Assets/Xbox360Y.png")));
                    _mainWindow.AutoPlayBtnIcon.Source = new Bitmap(AssetLoader.Open(new Uri("avares://EDD/Assets/XboxOneLB.png")));
                    break;
                //Maybe add nintendo support
            }
        }
    
    public void CleanUpSDL()
    {
        elTick.Stop();
        foreach (var gp in _OGP.Values) { SDL.CloseGamepad(gp); }
        _OGP.Clear();
        SDL.Quit();
    }
    
}