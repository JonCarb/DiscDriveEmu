using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AnimatedImage.Avalonia;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace EDD;

public partial class MainWindow : Window
{
    private readonly DispatcherTimer _elTick = new();
    private DriveInfo? _disc = null;
    private bool _inProcess = false;
    private bool _pollingIsBusy = false;
    
    public MainWindow()
    {
        InitializeComponent();
        Console.WriteLine("No need for luck");
        Console.WriteLine(Directory.GetCurrentDirectory());
        _ = InitStartUp();
    }
    
    //Loads up components for drives, checks and visual aspects
    private async Task InitStartUp()
    {
        await App.Services.InitLoaded.Task;
        
        App.Services.PD.InitMainWindowForProcessDisc(this);
        App.Services.SDL.IntiSDL(this);
        
        //Load visual aspects first
        InstallBtn.IsChecked = App.Services.Settings?.WillCache;
        AutoPlayBtn.IsChecked = App.Services.Settings?.WillAutoStart;
        
        await Task.Run(() =>
        {
            _elTick.Tick += CheckPoll;
            _elTick.Interval = new TimeSpan(0, 0, 0, 4);
            _elTick.Start();
        });
    }
    
    private async void CheckPoll(object? sender, EventArgs e)
    {
        if (!_inProcess)
        {
            if (_pollingIsBusy) return;
            _pollingIsBusy = true;
            Console.WriteLine("Inisde polling called");
            _disc = await GetDriveInfo();
            if (_disc != null)
            {
                _inProcess = true;
                await App.Services.PD.StartProcess(_disc);
            }
            _pollingIsBusy = false;
        }
        else
        {
            Console.WriteLine("Outside polling called");
            if (_disc is not { IsReady: false }) return;
            _inProcess = false;
            await DisplayError("Disc Drive was opened!");
        }
    }
    //Fine for now but is there a way to wait for it to finish mounting fully
    private async Task<DriveInfo?> GetDriveInfo()
    {
        DriveInfo? discDrive = null;
        var driveExists = await Task.Run(() =>
        {
            discDrive = DriveInfo.GetDrives().FirstOrDefault(d => d.DriveType == DriveType.CDRom);
            return discDrive != null && Directory.Exists(discDrive.Name);
        });
        if (driveExists && discDrive is { IsReady: true }) { return discDrive; }
        return null;
    }
    
    //-----------------------------------------------------//
    //-------------------DISPLAY---------------------------//
    //-----------------------------------------------------//
    public async Task ChangeDescription(string description)
    {
        Dispatcher.Invoke(() => { Description.Text = description; });
    }

    public async Task ChangeDiscGifToRun()
    {
        Dispatcher.Invoke(() =>
        {
            ImageBehavior.SetAnimatedSource(Disc, new Uri("avares://EDD/Assets/readingDisc.gif"));
        });
    }

    public async Task DisplayError(string error = "Disc Error")
    {
        App.Services.PD.ResetGlobalValues();
        Dispatcher.Invoke(() =>
        {
            //change to error GIF
            ImageBehavior.SetAnimatedSource(Disc, new Uri("avares://EDD/Assets/diskError.gif"));
            Description.Text = error;
        });
        
        await Task.Run(() =>
        {
            int ret = mciSendString("set cdaudio door open", null, 0, IntPtr.Zero);   //Eject Disc
            Console.WriteLine($"Eject Disc return: {ret}");
        });
        
        await Task.Delay(3220); //A bit of timing here to show error animation
        
        Dispatcher.Invoke(() =>
        {
            //change GIF back to normal
            ImageBehavior.SetAnimatedSource(Disc, new Uri("avares://EDD/Assets/spinningDisc.gif"));
            Description.Text = "Please insert a Disc";
        });
    }
    //-----------------------------------------------------//
    //-----------------------------------------------------//
    //-----------------------------------------------------//
    public void ShutDown()
    {
        //Add any clean up that might be needed
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopApp)
        {
            App.SaveCurrentSettings();
            App.Services.SDL.CleanUpSDL();
            desktopApp.Shutdown();
        }
    }
    //-----------------------------------------------------//
    //-----------------Buttons Actions---------------------//
    //-----------------------------------------------------//
    private SettingsWindow? _settingsWindow;
    public void SettingClick(object? sender, RoutedEventArgs? e)
    {
        var parentWindow = TopLevel.GetTopLevel(this) as Window;
        if (_settingsWindow != null)
        {
            _settingsWindow.Close();
            _settingsWindow = null;
        }
        else
        {
            _settingsWindow = new SettingsWindow();
            if (parentWindow != null)
            { 
                _settingsWindow.ShowDialog<string>(parentWindow);
            }
        }
    }
    private void InstallClick(object? sender, RoutedEventArgs e)
    {
        if (InstallBtn.IsChecked == true)
        {
            App.Services.Settings?.WillCache = true;
            _ =  App.Services.PD.PrepareGame();
            return;
        }
        App.Services.Settings?.WillCache = false;
    }
    private void AutoPlayClicked(object? sender, RoutedEventArgs e)
    {
        if (AutoPlayBtn.IsChecked == true)
        {
            App.Services.Settings?.WillAutoStart = true;
            _ =  App.Services.PD.RunGame();
            return;
        }
        App.Services.Settings?.WillAutoStart = false;
    }
    
    private void QuitClick(object sender, RoutedEventArgs e)
    {
        ShutDown();
    }
    
    //-----------------------------------------------------//
    //-----------------Controller Actions------------------//
    //-----------------------------------------------------//
    
    public void AutoPlaySwitch()
    {
        if(AutoPlayBtn.IsChecked == true)
        {
            AutoPlayBtn.IsChecked = false;
            return;
        }
        AutoPlayBtn.IsChecked = true;
    }

    public void InstallSwitch()
    {
        if(!InstallBtn.IsEnabled) return;
        if(InstallBtn.IsChecked == true)
        {
            InstallBtn.IsChecked = false;
            return;
        }
        InstallBtn.IsChecked = true;
    }
    
    //-----------------------------------------------------//
    //-----------------------------------------------------//
    //-----------------------------------------------------//
    
    [DllImport("winmm.dll", EntryPoint = "mciSendStringA", CharSet = CharSet.Ansi)]
    protected static extern int mciSendString(string lpstrCommand, StringBuilder lpstrReturnString, int uReturnLength, IntPtr hwndCallback); 
}