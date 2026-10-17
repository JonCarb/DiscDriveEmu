using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace EDD;

public partial class SettingsWindow : Window
{
    private ObservableCollection<string> InstallsNames { get; set; } = [];
    public SettingsWindow()
    {
        InitializeComponent();
        InstallList.ItemsSource = InstallsNames;
        LoadSettings();
    }

    private void LoadSettings()
    {
        if (App.Services.Settings == null) return;
        
        Dolphin.Text = $"Dolphin Path: {App.Services.Settings.Dolphin}";
        Xemu.Text = $"Xemu Path: {App.Services.Settings.Xemu}";
        Xenia.Text = $"Xenia Path: {App.Services.Settings.Xenia}";
        PCSX2.Text = $"PCSX2 Path: {App.Services.Settings.PCSX2}";
        RPCS3.Text = $"RPCS3 Path: {App.Services.Settings.RPCS3}";
        Cemu.Text = $"Cemu Path: {App.Services.Settings.Cemu}";
        //Add others future emu here
        string installDirectory = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Installs");
        InstallPath.Text = $"Default Path: {installDirectory}";
        ShowInstalls(installDirectory,InstallsNames);
    }
    private void ShowInstalls(string fullPath, ObservableCollection<string> folders)
    {
        if (!Directory.Exists(fullPath)) return;

        folders.Clear();
        foreach (var dir in Directory.GetDirectories(fullPath))
            folders.Add(System.IO.Path.GetFileName(dir));
    }

    private async void AddPaths(object sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        
        var files = await topLevel?.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select Emulator",
            AllowMultiple = false
        })!;

        if (files.Count == 1)
        {
            var path = files[0].TryGetLocalPath();
            if (path == null) return;
            var name = path.ToUpper();

            if (name.Contains("DOLPHIN"))
            {
                App.Services.Settings?.Dolphin = path;
                Dolphin.Text = $"Dolphin Path: {App.Services.Settings?.Dolphin}";
            }
            else if (name.Contains("XEMU"))
            {
                App.Services.Settings?.Xemu = path;
                Xemu.Text = $"Xemu Path: {App.Services.Settings?.Xemu}";
            }
            else if (name.Contains("Xenia"))
            {
                App.Services.Settings?.Xenia = path;
                Xenia.Text = $"Xenia Path: {App.Services.Settings?.Xenia}";
            }
            else if (name.Contains("PCSX2"))
            {
                App.Services.Settings?.PCSX2 = path;
                PCSX2.Text = $"PCSX2 Path: {App.Services.Settings?.PCSX2}";
            }
            else if (name.Contains("RPCS3"))
            {
                App.Services.Settings?.RPCS3 = path;
                RPCS3.Text = $"RPCS3 Path: {App.Services.Settings?.RPCS3}";
            }
            else if (name.Contains("CEMU"))
            {
                App.Services.Settings?.Cemu = path;
                Cemu.Text = $"Cemu Path: {App.Services.Settings?.Cemu}";
            }
            App.SaveCurrentSettings();
        }
        else
        {
            Console.WriteLine("User canceled Adding path");
        }
    }

    private async void RemoveInstall(object sender, RoutedEventArgs e)
    {
        string installDirectory = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Installs");
        var topLevel = TopLevel.GetTopLevel(this);
        var startLocation = await topLevel?.StorageProvider.TryGetFolderFromPathAsync(installDirectory)!;
        var folders = await topLevel?.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select Games to delete",
            SuggestedStartLocation = startLocation,
            AllowMultiple = true
        })!;

        if (folders.Count < 1) return;

        try
        {
            foreach (var folder in folders)
            {
                var path = folder.TryGetLocalPath();
                if (Directory.Exists(path) && path != installDirectory)
                    Directory.Delete(path, true);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        ShowInstalls(installDirectory,InstallsNames);
    }

}