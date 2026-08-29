using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Storage.Pickers;
using static EmuDiscDriveGUI.MainWindow;

namespace EmuDiscDriveGUI
{
    public partial class Settings : Form
    {
        private FileOpenPicker openPicker = new FileOpenPicker() // Allows for file selection, we want specificly Dolphin.exe, xemu.exe ETC!
        {
            CommitButtonText = "Choose Emulator EXE",
            FileTypeFilter = { ".exe" }
        };
        public Settings()
        {
            InitializeComponent();
            displayEmulatorPaths();
        }
        private void displayEmulatorPaths()
        {
            if (AppService.PathEmu == null)
            {
                //Display Error
            }
            else
            {
                //dolPath.Text = "Dolphin Path: " + AppService.emuPath.Dolphin;
                //etc
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
