using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.IO;
using static EmuDiscReader.MainWindow;

namespace EmuDiscReader
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
            LoadPaths();
        }

        private void LoadPaths()
        {
            if (AppService.PathEmu == null)
            {
                Label.Text = "An Error has occured. Please Restart the App";
                return;
            }

            Dolphin.Text = "Dolphin Path: " + AppService.PathEmu.Dolphin;
            Xemu.Text = "Xemu Path: " + AppService.PathEmu.Xemu;
            PCSX2.Text = "PCSX2 Path: " + AppService.PathEmu.PCSX2;
            RPCS3.Text = "RPCS3 Path: " + AppService.PathEmu.RPCS3;
            Cemu.Text = "Cemu Path: " + AppService.PathEmu.Cemu;
            //Change other emu text here
        }

        private async void AddPath_Click(object sender, RoutedEventArgs e)
        {
            await AppService.EmuLoaded.Task;
            if (AppService.PathEmu is null || AppService.PathsFile is null)
            {
                Label.Text = "PathEmu or PathsFile is null. Please try again";
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
    }
}
