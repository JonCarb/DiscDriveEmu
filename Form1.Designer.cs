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
            MainPanel = new TableLayoutPanel();
            Description = new Label();
            Disc = new PictureBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            SettingButton = new Button();
            MainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Disc).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // MainPanel
            // 
            MainPanel.ColumnCount = 1;
            MainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            MainPanel.Controls.Add(Description, 0, 1);
            MainPanel.Controls.Add(Disc, 0, 0);
            MainPanel.Dock = DockStyle.Fill;
            MainPanel.Location = new Point(0, 0);
            MainPanel.Name = "MainPanel";
            MainPanel.RowCount = 2;
            MainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            MainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 279F));
            MainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            MainPanel.Size = new Size(2145, 945);
            MainPanel.TabIndex = 1;
            // 
            // Description
            // 
            Description.BackColor = Color.Transparent;
            Description.Dock = DockStyle.Top;
            Description.FlatStyle = FlatStyle.Flat;
            Description.Font = new Font("Verdana", 50.25F, FontStyle.Italic, GraphicsUnit.Point, 0);
            Description.ForeColor = SystemColors.Control;
            Description.Location = new Point(3, 666);
            Description.Name = "Description";
            Description.Size = new Size(2139, 86);
            Description.TabIndex = 1;
            Description.Text = "Please insert a Disc";
            Description.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Disc
            // 
            Disc.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Disc.Image = Properties.Resources.spinningDisc;
            Disc.Location = new Point(3, 3);
            Disc.Name = "Disc";
            Disc.Size = new Size(2139, 660);
            Disc.SizeMode = PictureBoxSizeMode.Zoom;
            Disc.TabIndex = 2;
            Disc.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 1073F));
            tableLayoutPanel1.Controls.Add(SettingButton, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Bottom;
            tableLayoutPanel1.Location = new Point(0, 945);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(2145, 70);
            tableLayoutPanel1.TabIndex = 2;
            // 
            // SettingButton
            // 
            SettingButton.BackColor = Color.FromArgb(30, 30, 30);
            SettingButton.Cursor = Cursors.Hand;
            SettingButton.Dock = DockStyle.Left;
            SettingButton.FlatAppearance.BorderSize = 0;
            SettingButton.FlatStyle = FlatStyle.Flat;
            SettingButton.Font = new Font("Yu Gothic", 10F, FontStyle.Bold);
            SettingButton.ForeColor = Color.White;
            SettingButton.Location = new Point(3, 3);
            SettingButton.Name = "SettingButton";
            SettingButton.Size = new Size(120, 64);
            SettingButton.TabIndex = 0;
            SettingButton.Text = "Settings";
            SettingButton.UseVisualStyleBackColor = false;
            SettingButton.Click += SettingButton_Click;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = SystemColors.ActiveCaptionText;
            BackgroundImageLayout = ImageLayout.Zoom;
            ClientSize = new Size(2145, 1015);
            Controls.Add(MainPanel);
            Controls.Add(tableLayoutPanel1);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            Name = "MainWindow";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Disc Reader";
            WindowState = FormWindowState.Maximized;
            MainPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)Disc).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private TableLayoutPanel MainPanel;
        private PictureBox Disc;
        private Label Description;
        private TableLayoutPanel tableLayoutPanel1;
        private Button SettingButton;
    }
}
