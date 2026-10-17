using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace EDD;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        _ = LoadSettings();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }
        base.OnFrameworkInitializationCompleted();
    }
    
    public static class Services
    {
        public static SettingsValues? Settings {get; set;}                      //Holds the string values for where each emulator is stored
        public static TaskCompletionSource<bool> InitLoaded { get; } = new();   //Needed so Json loading finishes first before continuing
        public static ProcessDisc PD { get; } = new();
        public static SDLController SDL { get; } = new();
    }

    private async Task LoadSettings()
    {
        string destination = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Installs");
        if (!Directory.Exists(destination)) Directory.CreateDirectory(destination);
        
        string jsonPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Settings.json");
        if (File.Exists(jsonPath))
        {
            await using FileStream file = File.OpenRead(jsonPath);
            App.Services.Settings = await JsonSerializer.DeserializeAsync<SettingsValues>(file);
        }
        else
        {
            App.Services.Settings = new SettingsValues();
            await using FileStream create = File.Create(jsonPath);
            await JsonSerializer.SerializeAsync(create, App.Services.Settings);
        }
        App.Services.InitLoaded.SetResult(true);
    }
    
    public static void SaveCurrentSettings()
    {
        if (App.Services.Settings == null) return;
        string jsonPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Settings.json");
        string json = JsonSerializer.Serialize(App.Services.Settings);
        File.WriteAllText(jsonPath, json);
    }
}