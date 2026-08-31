namespace EmuDiscDriveGUI
{
    partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            MainTable = new TableLayoutPanel();
            Disc = new PictureBox();
            Description = new Label();
            CacheBar = new ProgressBar();
            SettingButton = new Button();
            MainTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Disc).BeginInit();
            SuspendLayout();
            // 
            // MainTable
            // 
            MainTable.ColumnCount = 2;
            MainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            MainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 8F));
            MainTable.Controls.Add(Disc, 0, 1);
            MainTable.Controls.Add(Description, 0, 2);
            MainTable.Controls.Add(CacheBar, 0, 3);
            MainTable.Controls.Add(SettingButton, 0, 4);
            MainTable.Dock = DockStyle.Fill;
            MainTable.Location = new Point(0, 0);
            MainTable.Name = "MainTable";
            MainTable.RowCount = 5;
            MainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 18.4496117F));
            MainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 81.5503845F));
            MainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 81F));
            MainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 177F));
            MainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 69F));
            MainTable.Size = new Size(1740, 973);
            MainTable.TabIndex = 0;
            // 
            // Disc
            // 
            Disc.BackgroundImageLayout = ImageLayout.Zoom;
            Disc.Dock = DockStyle.Fill;
            Disc.Image = Properties.Resources.spinningDisc;
            Disc.Location = new Point(3, 122);
            Disc.Name = "Disc";
            Disc.Size = new Size(1726, 520);
            Disc.SizeMode = PictureBoxSizeMode.Zoom;
            Disc.TabIndex = 7;
            Disc.TabStop = false;
            // 
            // Description
            // 
            Description.AutoSize = true;
            Description.BackColor = Color.Transparent;
            Description.Dock = DockStyle.Fill;
            Description.FlatStyle = FlatStyle.Flat;
            Description.Font = new Font("Verdana", 35F, FontStyle.Italic, GraphicsUnit.Point, 0);
            Description.ForeColor = SystemColors.Control;
            Description.Location = new Point(3, 645);
            Description.Name = "Description";
            Description.Size = new Size(1726, 81);
            Description.TabIndex = 8;
            Description.Text = "Please insert a Disc";
            Description.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // CacheBar
            // 
            CacheBar.Anchor = AnchorStyles.Top;
            CacheBar.Location = new Point(397, 729);
            CacheBar.Name = "CacheBar";
            CacheBar.Size = new Size(937, 23);
            CacheBar.TabIndex = 9;
            CacheBar.Visible = false;
            // 
            // SettingButton
            // 
            SettingButton.Anchor = AnchorStyles.Right;
            SettingButton.BackColor = Color.FromArgb(35, 35, 35);
            SettingButton.Cursor = Cursors.Hand;
            SettingButton.FlatAppearance.BorderSize = 0;
            SettingButton.FlatStyle = FlatStyle.Flat;
            SettingButton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            SettingButton.ForeColor = Color.White;
            SettingButton.Location = new Point(1509, 913);
            SettingButton.Name = "SettingButton";
            SettingButton.Size = new Size(220, 50);
            SettingButton.TabIndex = 0;
            SettingButton.Text = "⚙️ Settings";
            SettingButton.UseVisualStyleBackColor = false;
            SettingButton.Click += SettingButton_Click;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.Black;
            BackgroundImageLayout = ImageLayout.Zoom;
            ClientSize = new Size(1740, 973);
            Controls.Add(MainTable);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            Name = "MainWindow";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Disc Reader";
            TransparencyKey = Color.Blue;
            WindowState = FormWindowState.Maximized;
            MainTable.ResumeLayout(false);
            MainTable.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)Disc).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel MainTable;
        private Label Description;
        private ProgressBar CacheBar;
        private PictureBox Disc;
        private Button SettingButton;
    }
}
