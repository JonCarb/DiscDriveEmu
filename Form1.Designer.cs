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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            MainPanel = new TableLayoutPanel();
            Disc = new PictureBox();
            Description = new Label();
            MainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Disc).BeginInit();
            SuspendLayout();
            // 
            // MainPanel
            // 
            MainPanel.ColumnCount = 1;
            MainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            MainPanel.Controls.Add(Disc, 0, 0);
            MainPanel.Controls.Add(Description, 0, 1);
            MainPanel.Dock = DockStyle.Fill;
            MainPanel.Location = new Point(0, 0);
            MainPanel.Name = "MainPanel";
            MainPanel.RowCount = 2;
            MainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            MainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 302F));
            MainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            MainPanel.Size = new Size(2145, 1015);
            MainPanel.TabIndex = 1;
            // 
            // Disc
            // 
            Disc.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Disc.Image = (Image)resources.GetObject("Disc.Image");
            Disc.Location = new Point(3, 3);
            Disc.Name = "Disc";
            Disc.Size = new Size(2139, 707);
            Disc.SizeMode = PictureBoxSizeMode.Zoom;
            Disc.TabIndex = 2;
            Disc.TabStop = false;
            // 
            // Description
            // 
            Description.BackColor = Color.Transparent;
            Description.Dock = DockStyle.Top;
            Description.FlatStyle = FlatStyle.Flat;
            Description.Font = new Font("Verdana", 50.25F, FontStyle.Italic, GraphicsUnit.Point, 0);
            Description.ForeColor = SystemColors.Control;
            Description.Location = new Point(3, 713);
            Description.Name = "Description";
            Description.Size = new Size(2139, 103);
            Description.TabIndex = 1;
            Description.Text = "Please insert a Disc";
            Description.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = SystemColors.ActiveCaptionText;
            BackgroundImageLayout = ImageLayout.Zoom;
            ClientSize = new Size(2145, 1015);
            Controls.Add(MainPanel);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            Name = "MainWindow";
            Text = "Disc Reader";
            WindowState = FormWindowState.Maximized;
            Load += MainWindow_Load;
            MainPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)Disc).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private TableLayoutPanel MainPanel;
        private PictureBox Disc;
        private Label Description;
    }
}
