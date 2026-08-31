namespace EmuDiscDriveGUI
{
    partial class Settings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            SettingTable = new TableLayoutPanel();
            Cemu = new Label();
            RPCS3 = new Label();
            PCSX2 = new Label();
            Xemu = new Label();
            Dolphin = new Label();
            label1 = new Label();
            AddPath = new Button();
            InstallBox = new CheckBox();
            SettingTable.SuspendLayout();
            SuspendLayout();
            // 
            // SettingTable
            // 
            SettingTable.BackColor = SystemColors.WindowText;
            SettingTable.ColumnCount = 3;
            SettingTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            SettingTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75F));
            SettingTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 186F));
            SettingTable.Controls.Add(Cemu, 1, 6);
            SettingTable.Controls.Add(RPCS3, 1, 5);
            SettingTable.Controls.Add(PCSX2, 1, 4);
            SettingTable.Controls.Add(Xemu, 1, 3);
            SettingTable.Controls.Add(Dolphin, 1, 2);
            SettingTable.Controls.Add(label1, 1, 1);
            SettingTable.Controls.Add(AddPath, 1, 7);
            SettingTable.Controls.Add(InstallBox, 1, 9);
            SettingTable.Dock = DockStyle.Fill;
            SettingTable.Location = new Point(0, 0);
            SettingTable.Name = "SettingTable";
            SettingTable.RowCount = 11;
            SettingTable.RowStyles.Add(new RowStyle(SizeType.Percent, 38.42105F));
            SettingTable.RowStyles.Add(new RowStyle(SizeType.Percent, 61.57895F));
            SettingTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            SettingTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 63F));
            SettingTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            SettingTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
            SettingTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 63F));
            SettingTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 65F));
            SettingTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            SettingTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
            SettingTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 353F));
            SettingTable.Size = new Size(1083, 970);
            SettingTable.TabIndex = 0;
            // 
            // Cemu
            // 
            Cemu.AutoSize = true;
            Cemu.BackColor = SystemColors.ActiveCaptionText;
            Cemu.BorderStyle = BorderStyle.FixedSingle;
            Cemu.Dock = DockStyle.Fill;
            Cemu.Font = new Font("Segoe UI", 14F);
            Cemu.ForeColor = SystemColors.Control;
            Cemu.Location = new Point(227, 411);
            Cemu.Name = "Cemu";
            Cemu.Size = new Size(666, 63);
            Cemu.TabIndex = 4;
            Cemu.Text = "Cemu Path:";
            Cemu.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // RPCS3
            // 
            RPCS3.AutoSize = true;
            RPCS3.BackColor = SystemColors.ActiveCaptionText;
            RPCS3.BorderStyle = BorderStyle.FixedSingle;
            RPCS3.Dock = DockStyle.Fill;
            RPCS3.Font = new Font("Segoe UI", 14F);
            RPCS3.ForeColor = SystemColors.Control;
            RPCS3.Location = new Point(227, 353);
            RPCS3.Name = "RPCS3";
            RPCS3.Size = new Size(666, 58);
            RPCS3.TabIndex = 3;
            RPCS3.Text = "RPCS3 Path:";
            RPCS3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // PCSX2
            // 
            PCSX2.AutoSize = true;
            PCSX2.BackColor = SystemColors.ActiveCaptionText;
            PCSX2.BorderStyle = BorderStyle.FixedSingle;
            PCSX2.Dock = DockStyle.Fill;
            PCSX2.Font = new Font("Segoe UI", 14F);
            PCSX2.ForeColor = SystemColors.Control;
            PCSX2.Location = new Point(227, 293);
            PCSX2.Name = "PCSX2";
            PCSX2.Size = new Size(666, 60);
            PCSX2.TabIndex = 2;
            PCSX2.Text = "PCSX2 Path:";
            PCSX2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Xemu
            // 
            Xemu.AutoSize = true;
            Xemu.BackColor = SystemColors.ActiveCaptionText;
            Xemu.BorderStyle = BorderStyle.FixedSingle;
            Xemu.Dock = DockStyle.Fill;
            Xemu.Font = new Font("Segoe UI", 14F);
            Xemu.ForeColor = SystemColors.Control;
            Xemu.Location = new Point(227, 230);
            Xemu.Name = "Xemu";
            Xemu.Size = new Size(666, 63);
            Xemu.TabIndex = 1;
            Xemu.Text = "Xemu Path:";
            Xemu.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Dolphin
            // 
            Dolphin.AutoSize = true;
            Dolphin.BackColor = SystemColors.ActiveCaptionText;
            Dolphin.BorderStyle = BorderStyle.FixedSingle;
            Dolphin.Dock = DockStyle.Fill;
            Dolphin.Font = new Font("Segoe UI", 14F);
            Dolphin.ForeColor = SystemColors.Control;
            Dolphin.Location = new Point(227, 170);
            Dolphin.Name = "Dolphin";
            Dolphin.Size = new Size(666, 60);
            Dolphin.TabIndex = 0;
            Dolphin.Text = "Dolphin Path:";
            Dolphin.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Segoe UI", 30F);
            label1.ForeColor = SystemColors.ControlLight;
            label1.Location = new Point(227, 65);
            label1.Name = "label1";
            label1.Size = new Size(666, 105);
            label1.TabIndex = 9;
            label1.Text = "Supported emulators";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // AddPath
            // 
            AddPath.BackColor = Color.Green;
            AddPath.Cursor = Cursors.Hand;
            AddPath.Dock = DockStyle.Fill;
            AddPath.Font = new Font("Segoe UI", 12F);
            AddPath.ForeColor = SystemColors.ControlLight;
            AddPath.Location = new Point(227, 477);
            AddPath.Name = "AddPath";
            AddPath.Size = new Size(666, 59);
            AddPath.TabIndex = 5;
            AddPath.Text = "Add Emulator Path";
            AddPath.UseVisualStyleBackColor = false;
            AddPath.Click += AddPath_Click;
            // 
            // InstallBox
            // 
            InstallBox.BackColor = Color.Snow;
            InstallBox.Cursor = Cursors.Hand;
            InstallBox.Dock = DockStyle.Fill;
            InstallBox.FlatAppearance.BorderSize = 0;
            InstallBox.FlatStyle = FlatStyle.Popup;
            InstallBox.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            InstallBox.ForeColor = Color.DarkSlateGray;
            InstallBox.Location = new Point(227, 567);
            InstallBox.Name = "InstallBox";
            InstallBox.Size = new Size(666, 46);
            InstallBox.TabIndex = 0;
            InstallBox.Text = "Install Game";
            InstallBox.TextAlign = ContentAlignment.MiddleCenter;
            InstallBox.UseVisualStyleBackColor = false;
            InstallBox.CheckedChanged += InstallBox_CheckedChanged;
            // 
            // Settings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaptionText;
            ClientSize = new Size(1083, 970);
            Controls.Add(SettingTable);
            Name = "Settings";
            Text = "Settings";
            SettingTable.ResumeLayout(false);
            SettingTable.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel SettingTable;
        private Label Dolphin;
        private Label Xemu;
        private Label PCSX2;
        private Label RPCS3;
        private Label Cemu;
        private Button AddPath;
        private Label label1;
        private CheckBox InstallBox;
    }
}