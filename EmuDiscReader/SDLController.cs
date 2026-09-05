using SDL3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace EmuDiscReader
{
    public class SDLController
    {
        private readonly Dictionary<uint, IntPtr> _OGP = new();
        private DispatcherTimer t = new();
        private MainWindow? MainForm;
        public void InitSDL(MainWindow f)
        {
            this.MainForm = f;
            SDL.Init(SDL.InitFlags.Gamepad);
            t.Tick += new EventHandler(CheckPoll);
            t.Interval = TimeSpan.FromMilliseconds(16);
            t.Start();
        }
        private void CheckPoll(object? sender, EventArgs e)
        {
            SDL.Event sdlEvent;
            while (SDL.PollEvent(out sdlEvent))
            {
                HandleEvent(sdlEvent);
            }
        }

        private string? currentController = "blank";
        private bool HandleEvent(SDL.Event sdlEvent)
        {
            if(MainForm == null) { return false; }
            switch ((SDL.EventType)sdlEvent.Type)
            {
                case SDL.EventType.GamepadAdded:
                    var gpa = SDL.OpenGamepad(sdlEvent.GDevice.Which);
                    if (gpa != IntPtr.Zero)
                    {
                        Console.WriteLine($"Gamepad added: {SDL.GetGamepadName(gpa)}");
                        if (_OGP.Count < 1) 
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
                        case SDL.GamepadButton.North:       //IDK
                            Console.WriteLine("Y pressed");
                            break;
                        case SDL.GamepadButton.East:        //Settings Button
                            Console.WriteLine("B pressed");
                            MainForm.OpenSettingsPage();
                            break;
                        case SDL.GamepadButton.West:        //Cache/Install Game
                            Console.WriteLine("X pressed");
                            MainForm.InstallBTNController();
                            break;
                        case SDL.GamepadButton.South:       //Start Game
                            Console.WriteLine("A pressed");
                            MainForm.StartGameController();
                            break;
                    }
                    break;
                default:
                    break;
            }
            return true;
        }
        private void ChangeDisplayButtons(uint which)
        {
            if(MainForm == null) {  return; }
            string? controllerType = SDL.GetGamepadStringForType(SDL.GetGamepadTypeForID(which));
            if (controllerType != null && currentController != controllerType)
            {
                currentController = controllerType;
                Console.WriteLine(currentController);
                switch (currentController)
                {
                    case "PS3":
                    case "ps4":
                    case "ps5":
                        MainForm.InstallBtnIcon.Source = new BitmapImage(new Uri("/EmuDiscReader;component/Assets/ButtonIcon-PS4-Square.png", UriKind.Relative));
                        MainForm.SettingBtnIcon.Source = new BitmapImage(new Uri("/EmuDiscReader;component/Assets/ButtonIcon-PS4-Circle.png", UriKind.Relative));
                        MainForm.DescIcon.Source = new BitmapImage(new Uri("/EmuDiscReader;component/Assets/ButtonIcon-PS4-Cross.png", UriKind.Relative));
                        break;
                    case "unknown":
                    case "steam":
                    case "xbox360":
                    case "xboxone":
                        MainForm.InstallBtnIcon.Source = new BitmapImage(new Uri("/EmuDiscReader;component/Assets/ButtonIcon-Xbox360-X.png", UriKind.Relative));
                        MainForm.SettingBtnIcon.Source = new BitmapImage(new Uri("/EmuDiscReader;component/Assets/ButtonIcon-Xbox360-B.png", UriKind.Relative));
                        MainForm.DescIcon.Source = new BitmapImage(new Uri("/EmuDiscReader;component/Assets/ButtonIcon-Xbox360-A.png", UriKind.Relative));
                        break;
                    //Maybe add nintendo support
                }
            }
        }
        public void CleanUpSDL()
        {
            t.Stop();
            foreach (var gp in _OGP.Values) { SDL.CloseGamepad(gp); }
            _OGP.Clear();
            SDL.Quit();
        }
    }
}
