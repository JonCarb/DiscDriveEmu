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
            MainContainer = new TableLayoutPanel();
            SuspendLayout();
            // 
            // MainContainer
            // 
            MainContainer.AutoSize = true;
            MainContainer.ColumnCount = 2;
            MainContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 386F));
            MainContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            MainContainer.Dock = DockStyle.Fill;
            MainContainer.Location = new Point(0, 0);
            MainContainer.Name = "MainContainer";
            MainContainer.RowCount = 2;
            MainContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 53.77778F));
            MainContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 46.22222F));
            MainContainer.Size = new Size(800, 450);
            MainContainer.TabIndex = 0;
            // 
            // Settings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(MainContainer);
            Name = "Settings";
            Text = "Settings";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel MainContainer;
    }
}