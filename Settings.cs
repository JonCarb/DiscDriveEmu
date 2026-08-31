using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Storage.Pickers;
using static EmuDiscDriveGUI.MainWindow;

namespace EmuDiscDriveGUI
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
            configInstall();
            LoadPaths();
        }

        private async void LoadPaths()
        {
            if (AppService.PathEmu == null)
            {
                label1.Text = "An Error has occured. Please Restart the App";
                return;
            }

            Dolphin.Text = "Dolphin Path: " + AppService.PathEmu.Dolphin;
            Xemu.Text = "Xemu Path: " + AppService.PathEmu.Xemu;
            PCSX2.Text = "PCSX2 Path: " + AppService.PathEmu.PCSX2;
            RPCS3.Text = "RPCS3 Path: " + AppService.PathEmu.RPCS3;
            Cemu.Text = "Cemu Path: " + AppService.PathEmu.Cemu;
            //Change other emu text here
        }

        private async void AddPath_Click(object sender, EventArgs e)
        {
            await AppService.EmuLoaded.Task;
            if (AppService.PathEmu is null || AppService.PathsFile is null)
            {
                label1.Text = "PathEmu or PathsFile is null. Please try again";
                return;
            }
            try
            {
                string? fullPath = null;

                using (OpenFileDialog dialog = new OpenFileDialog())
                {
                    dialog.Filter = "Executable files (*.exe)|*.exe";

                    if (dialog.ShowDialog(this) == DialogResult.OK)
                    {
                        fullPath = dialog.FileName;
                    }
                }

                if (fullPath != null)
                {
                    string emuName = Path.GetFileName(fullPath).ToUpper();
                    if (emuName.Contains("DOLPHIN"))
                    {
                        AppService.PathEmu.Dolphin = fullPath;
                        Dolphin.Text = "Dolphin Path: " + fullPath;
                    }
                    else if (emuName.Contains("PCSX2"))
                    {
                        AppService.PathEmu.PCSX2 = fullPath;
                        PCSX2.Text = "PCSX2 Path: " + fullPath;
                    }
                    else if (emuName.Contains("XEMU"))
                    {
                        AppService.PathEmu.Xemu = fullPath;
                        Xemu.Text = "Xemu Path: " + fullPath;
                    }
                    else if (emuName.Contains("CEMU"))
                    {
                        AppService.PathEmu.Cemu = fullPath;
                        Cemu.Text = "Cemu Path: " + fullPath;
                    }
                    else if (emuName.Contains("RPCS3"))
                    {
                        AppService.PathEmu.RPCS3 = fullPath;
                        RPCS3.Text = "RPCS3 Path: " + fullPath;
                    }
                    //Add other emulators
                }
                //Dont forget to write it back
                string json = JsonSerializer.Serialize(AppService.PathEmu, AppJsonContext.Default.EmuPath);
                File.WriteAllText(AppService.PathsFile.Path, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private void configInstall()
        {
            if (AppService.CacheGame == true)
            {
                InstallBox.CheckState = CheckState.Checked;
            }
            else
            {
                InstallBox.CheckState = CheckState.Unchecked;
            }
        }

        private void InstallBox_CheckedChanged(object sender, EventArgs e)
        {
            if(InstallBox.CheckState == CheckState.Checked)
            {
                AppService.CacheGame = true;
            } 
            else
            {
                AppService.CacheGame = false;
            }
        }
    }
}
