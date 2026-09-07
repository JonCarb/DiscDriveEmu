using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static EmuDiscReader.MainWindow;

namespace EmuDiscReader
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private ObservableCollection<string> DefaultFolderNames { get; set; } = new ObservableCollection<string>();
        //private ObservableCollection<string> CustomFolderNames { get; set; } = new ObservableCollection<string>();
        string? installDirectory = null;
        public Settings()
        {
            InitializeComponent();
            DefaultFolderList.ItemsSource = DefaultFolderNames;
            //CustomFolderList.ItemsSource = CustomFolderNames;
            LoadSettings();
        }

        private void LoadSettings()
        {
            if (AppService.PathEmu == null)
            {
                Label.Text = "An Error has occured. Please Restart the App";
                return;
            }
            // -- Emulator Paths --
            Dolphin.Text = "Dolphin Path: " + AppService.PathEmu.Dolphin;
            Xemu.Text = "Xemu Path: " + AppService.PathEmu.Xemu;
            Xenia.Text = "Xenia Path: " + AppService.PathEmu.Xenia;
            PCSX2.Text = "PCSX2 Path: " + AppService.PathEmu.PCSX2;
            RPCS3.Text = "RPCS3 Path: " + AppService.PathEmu.RPCS3;
            Cemu.Text = "Cemu Path: " + AppService.PathEmu.Cemu;
            //Change other emu text here

            // -- Install Destination Paths --
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            installDirectory = System.IO.Path.Combine(documentsPath, "CachedGames");
            DefaultPath.Text = $"Default Path: {installDirectory}";
            ShowInstalls(installDirectory, DefaultFolderNames);
        }

        private void ShowInstalls(string fullPath, ObservableCollection<string> folders)
        {
            if (!Directory.Exists(fullPath)) { return; }

            folders.Clear();
            foreach (string dir in Directory.GetDirectories(fullPath))
            {
                Console.WriteLine(dir);
                folders.Add(System.IO.Path.GetFileName(dir));
            }
        }

        private async void AddPath_Click(object sender, RoutedEventArgs e)
        {
            await AppService.EmuLoaded.Task;
            if (AppService.PathEmu is null)
            {
                Label.Text = "PathEmu is null. Please try again";
                return;
            }
            try
            {
                string? appName = null;
                string? path = null;

                OpenFileDialog dialog = new()
                {
                    Title = "Choose capatible exe Emulator",
                    Filter = "Executable files (*.exe)|*.exe",
                };


                if (dialog.ShowDialog() == true)
                {
                    appName = dialog.SafeFileName;
                    path = dialog.FileName;
                    appName = appName.ToUpper();
                }

                if (appName != null && path != null)
                {
                    if (appName.Contains("DOLPHIN"))
                    {
                        AppService.PathEmu.Dolphin = path;
                        Dolphin.Text = "Dolphin Path: " + AppService.PathEmu.Dolphin;
                    }
                    else if (appName.Contains("PCSX2"))
                    {
                        AppService.PathEmu.PCSX2 = path;
                        PCSX2.Text = "PCSX2 Path: " + AppService.PathEmu.PCSX2;
                    }
                    else if (appName.Contains("XEMU"))
                    {
                        AppService.PathEmu.Xemu = path;
                        Xemu.Text = "Xemu Path: " + AppService.PathEmu.Xemu;
                    }
                    else if (appName.Contains("CEMU"))
                    {
                        AppService.PathEmu.Cemu = path;
                        Cemu.Text = "Cemu Path: " + AppService.PathEmu.Cemu;
                    }
                    else if (appName.Contains("RPCS3"))
                    {
                        AppService.PathEmu.RPCS3 = path;
                        RPCS3.Text = "RPCS3 Path: " + AppService.PathEmu.RPCS3;
                    } 
                    else if (appName.Contains("XENIA"))
                    {
                        AppService.PathEmu.Xenia = path;
                        Xenia.Text = "Xenia Path: " + AppService.PathEmu.Xenia;
                    }
                    //Add other emulators
                }
                //Dont forget to write it back
                AppService.SaveJson();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private void DeleteBTN_Click(object sender, RoutedEventArgs e)
        {
            if (AppService.PathEmu == null || installDirectory == null) { return; }

            string? dirPath = null;
            try
            {
                OpenFolderDialog dialog = new()
                {
                    Title = "Select a folder to delete",
                    InitialDirectory = installDirectory,
                };

                if (dialog.ShowDialog() == true)
                {
                    dirPath = dialog.FolderName;
                }

                if (dirPath != null && Directory.Exists(dirPath))
                {
                    Directory.Delete(dirPath, true);
                    ShowInstalls(installDirectory, DefaultFolderNames);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }
}
