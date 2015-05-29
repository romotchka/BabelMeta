/*
 * Babel Meta
 * Copyright 2015 - Romain Carbou
 * romain.carbou@solstice-music.com
 */

namespace BabelMeta
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkIntegrityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertToSQLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.solsticeLegacyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.solsticeIgniterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertToXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fugaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.InputGroupBox = new System.Windows.Forms.GroupBox();
            this.CheckedPicto = new System.Windows.Forms.PictureBox();
            this.WarningPicto = new System.Windows.Forms.PictureBox();
            this.InputProgressBar = new System.Windows.Forms.ProgressBar();
            this.FilterArtistCheckBox = new System.Windows.Forms.CheckBox();
            this.FilterWorkCheckBox = new System.Windows.Forms.CheckBox();
            this.DuplicatesCheckBox = new System.Windows.Forms.CheckBox();
            this.ReferentialIntegrityCheckBox = new System.Windows.Forms.CheckBox();
            this.InputFormat = new System.Windows.Forms.ListBox();
            this.OutputGroupBox = new System.Windows.Forms.GroupBox();
            this.OutputProgressBar = new System.Windows.Forms.ProgressBar();
            this.InsertFormat = new System.Windows.Forms.ListBox();
            this.CurlyDoubleQuoteCheckBox = new System.Windows.Forms.CheckBox();
            this.CurlySimpleQuoteCheckBox = new System.Windows.Forms.CheckBox();
            this.NotificationZone = new System.Windows.Forms.TextBox();
            this.CompanyLogo = new System.Windows.Forms.PictureBox();
            this.menuStrip1.SuspendLayout();
            this.InputGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CheckedPicto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WarningPicto)).BeginInit();
            this.OutputGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CompanyLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.actionToolStripMenuItem,
            this.toolStripMenuItem2});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(479, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.loadSessionToolStripMenuItem,
            this.saveSessionToolStripMenuItem,
            this.quitToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(37, 20);
            this.toolStripMenuItem1.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // loadSessionToolStripMenuItem
            // 
            this.loadSessionToolStripMenuItem.Name = "loadSessionToolStripMenuItem";
            this.loadSessionToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.loadSessionToolStripMenuItem.Text = "Load Session";
            this.loadSessionToolStripMenuItem.Click += new System.EventHandler(this.loadSessionToolStripMenuItem_Click);
            // 
            // saveSessionToolStripMenuItem
            // 
            this.saveSessionToolStripMenuItem.Name = "saveSessionToolStripMenuItem";
            this.saveSessionToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveSessionToolStripMenuItem.Text = "Save Session";
            this.saveSessionToolStripMenuItem.Click += new System.EventHandler(this.saveSessionToolStripMenuItem_Click);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // actionToolStripMenuItem
            // 
            this.actionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkIntegrityToolStripMenuItem,
            this.convertToSQLToolStripMenuItem,
            this.convertToXMLToolStripMenuItem});
            this.actionToolStripMenuItem.Name = "actionToolStripMenuItem";
            this.actionToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.actionToolStripMenuItem.Text = "Action";
            // 
            // checkIntegrityToolStripMenuItem
            // 
            this.checkIntegrityToolStripMenuItem.Name = "checkIntegrityToolStripMenuItem";
            this.checkIntegrityToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.checkIntegrityToolStripMenuItem.Text = "Check integrity";
            this.checkIntegrityToolStripMenuItem.Click += new System.EventHandler(this.checkIntegrityToolStripMenuItem_Click);
            // 
            // convertToSQLToolStripMenuItem
            // 
            this.convertToSQLToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.solsticeLegacyToolStripMenuItem,
            this.solsticeIgniterToolStripMenuItem});
            this.convertToSQLToolStripMenuItem.Name = "convertToSQLToolStripMenuItem";
            this.convertToSQLToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.convertToSQLToolStripMenuItem.Text = "Convert to SQL";
            // 
            // solsticeLegacyToolStripMenuItem
            // 
            this.solsticeLegacyToolStripMenuItem.Name = "solsticeLegacyToolStripMenuItem";
            this.solsticeLegacyToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.solsticeLegacyToolStripMenuItem.Text = "Solstice Legacy";
            this.solsticeLegacyToolStripMenuItem.Click += new System.EventHandler(this.solsticeLegacyToolStripMenuItem_Click);
            // 
            // solsticeIgniterToolStripMenuItem
            // 
            this.solsticeIgniterToolStripMenuItem.Name = "solsticeIgniterToolStripMenuItem";
            this.solsticeIgniterToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.solsticeIgniterToolStripMenuItem.Text = "Solstice Igniter";
            this.solsticeIgniterToolStripMenuItem.Click += new System.EventHandler(this.solsticeIgniterToolStripMenuItem_Click);
            // 
            // convertToXMLToolStripMenuItem
            // 
            this.convertToXMLToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fugaToolStripMenuItem});
            this.convertToXMLToolStripMenuItem.Name = "convertToXMLToolStripMenuItem";
            this.convertToXMLToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.convertToXMLToolStripMenuItem.Text = "Convert to XML";
            // 
            // fugaToolStripMenuItem
            // 
            this.fugaToolStripMenuItem.Name = "fugaToolStripMenuItem";
            this.fugaToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.fugaToolStripMenuItem.Text = "Fuga";
            this.fugaToolStripMenuItem.Click += new System.EventHandler(this.fugaToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(24, 20);
            this.toolStripMenuItem2.Text = "?";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // InputGroupBox
            // 
            this.InputGroupBox.AutoSize = true;
            this.InputGroupBox.Controls.Add(this.CheckedPicto);
            this.InputGroupBox.Controls.Add(this.WarningPicto);
            this.InputGroupBox.Controls.Add(this.InputProgressBar);
            this.InputGroupBox.Controls.Add(this.FilterArtistCheckBox);
            this.InputGroupBox.Controls.Add(this.FilterWorkCheckBox);
            this.InputGroupBox.Controls.Add(this.DuplicatesCheckBox);
            this.InputGroupBox.Controls.Add(this.ReferentialIntegrityCheckBox);
            this.InputGroupBox.Controls.Add(this.InputFormat);
            this.InputGroupBox.Location = new System.Drawing.Point(12, 42);
            this.InputGroupBox.Name = "InputGroupBox";
            this.InputGroupBox.Size = new System.Drawing.Size(430, 126);
            this.InputGroupBox.TabIndex = 1;
            this.InputGroupBox.TabStop = false;
            this.InputGroupBox.Text = "Input Options";
            // 
            // CheckedPicto
            // 
            this.CheckedPicto.Image = ((System.Drawing.Image)(resources.GetObject("CheckedPicto.Image")));
            this.CheckedPicto.Location = new System.Drawing.Point(372, 75);
            this.CheckedPicto.Name = "CheckedPicto";
            this.CheckedPicto.Size = new System.Drawing.Size(32, 32);
            this.CheckedPicto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.CheckedPicto.TabIndex = 4;
            this.CheckedPicto.TabStop = false;
            this.CheckedPicto.Visible = false;
            // 
            // WarningPicto
            // 
            this.WarningPicto.Image = ((System.Drawing.Image)(resources.GetObject("WarningPicto.Image")));
            this.WarningPicto.Location = new System.Drawing.Point(372, 75);
            this.WarningPicto.Name = "WarningPicto";
            this.WarningPicto.Size = new System.Drawing.Size(32, 32);
            this.WarningPicto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.WarningPicto.TabIndex = 4;
            this.WarningPicto.TabStop = false;
            this.WarningPicto.Visible = false;
            // 
            // InputProgressBar
            // 
            this.InputProgressBar.Location = new System.Drawing.Point(6, 88);
            this.InputProgressBar.Name = "InputProgressBar";
            this.InputProgressBar.Size = new System.Drawing.Size(360, 10);
            this.InputProgressBar.Step = 1;
            this.InputProgressBar.TabIndex = 5;
            this.InputProgressBar.Visible = false;
            // 
            // FilterArtistCheckBox
            // 
            this.FilterArtistCheckBox.AutoSize = true;
            this.FilterArtistCheckBox.Location = new System.Drawing.Point(248, 42);
            this.FilterArtistCheckBox.Name = "FilterArtistCheckBox";
            this.FilterArtistCheckBox.Size = new System.Drawing.Size(127, 17);
            this.FilterArtistCheckBox.TabIndex = 4;
            this.FilterArtistCheckBox.Text = "Filter artists not in use";
            this.FilterArtistCheckBox.UseVisualStyleBackColor = true;
            // 
            // FilterWorkCheckBox
            // 
            this.FilterWorkCheckBox.AutoSize = true;
            this.FilterWorkCheckBox.Location = new System.Drawing.Point(248, 19);
            this.FilterWorkCheckBox.Name = "FilterWorkCheckBox";
            this.FilterWorkCheckBox.Size = new System.Drawing.Size(128, 17);
            this.FilterWorkCheckBox.TabIndex = 3;
            this.FilterWorkCheckBox.Text = "Filter works not in use";
            this.FilterWorkCheckBox.UseVisualStyleBackColor = true;
            // 
            // DuplicatesCheckBox
            // 
            this.DuplicatesCheckBox.AutoSize = true;
            this.DuplicatesCheckBox.Checked = true;
            this.DuplicatesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.DuplicatesCheckBox.Location = new System.Drawing.Point(6, 65);
            this.DuplicatesCheckBox.Name = "DuplicatesCheckBox";
            this.DuplicatesCheckBox.Size = new System.Drawing.Size(108, 17);
            this.DuplicatesCheckBox.TabIndex = 2;
            this.DuplicatesCheckBox.Text = "Check duplicates";
            this.DuplicatesCheckBox.UseVisualStyleBackColor = true;
            // 
            // ReferentialIntegrityCheckBox
            // 
            this.ReferentialIntegrityCheckBox.AutoSize = true;
            this.ReferentialIntegrityCheckBox.Checked = true;
            this.ReferentialIntegrityCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ReferentialIntegrityCheckBox.Location = new System.Drawing.Point(6, 42);
            this.ReferentialIntegrityCheckBox.Name = "ReferentialIntegrityCheckBox";
            this.ReferentialIntegrityCheckBox.Size = new System.Drawing.Size(145, 17);
            this.ReferentialIntegrityCheckBox.TabIndex = 1;
            this.ReferentialIntegrityCheckBox.Text = "Check referential integrity";
            this.ReferentialIntegrityCheckBox.UseVisualStyleBackColor = true;
            // 
            // InputFormat
            // 
            this.InputFormat.FormattingEnabled = true;
            this.InputFormat.Items.AddRange(new object[] {
            "Excel",
            "XML"});
            this.InputFormat.Location = new System.Drawing.Point(6, 19);
            this.InputFormat.Name = "InputFormat";
            this.InputFormat.Size = new System.Drawing.Size(209, 17);
            this.InputFormat.TabIndex = 0;
            // 
            // OutputGroupBox
            // 
            this.OutputGroupBox.AutoSize = true;
            this.OutputGroupBox.Controls.Add(this.OutputProgressBar);
            this.OutputGroupBox.Controls.Add(this.InsertFormat);
            this.OutputGroupBox.Controls.Add(this.CurlyDoubleQuoteCheckBox);
            this.OutputGroupBox.Controls.Add(this.CurlySimpleQuoteCheckBox);
            this.OutputGroupBox.Location = new System.Drawing.Point(12, 174);
            this.OutputGroupBox.Name = "OutputGroupBox";
            this.OutputGroupBox.Size = new System.Drawing.Size(430, 95);
            this.OutputGroupBox.TabIndex = 2;
            this.OutputGroupBox.TabStop = false;
            this.OutputGroupBox.Text = "Output Options";
            // 
            // OutputProgressBar
            // 
            this.OutputProgressBar.Location = new System.Drawing.Point(6, 66);
            this.OutputProgressBar.Name = "OutputProgressBar";
            this.OutputProgressBar.Size = new System.Drawing.Size(360, 10);
            this.OutputProgressBar.Step = 1;
            this.OutputProgressBar.TabIndex = 3;
            this.OutputProgressBar.Visible = false;
            // 
            // InsertFormat
            // 
            this.InsertFormat.FormattingEnabled = true;
            this.InsertFormat.Items.AddRange(new object[] {
            "Use INSERT in SQL",
            "Use INSERT/REPLACE in SQL"});
            this.InsertFormat.Location = new System.Drawing.Point(6, 20);
            this.InsertFormat.Name = "InsertFormat";
            this.InsertFormat.Size = new System.Drawing.Size(209, 17);
            this.InsertFormat.TabIndex = 2;
            // 
            // CurlyDoubleQuoteCheckBox
            // 
            this.CurlyDoubleQuoteCheckBox.AutoSize = true;
            this.CurlyDoubleQuoteCheckBox.Location = new System.Drawing.Point(248, 43);
            this.CurlyDoubleQuoteCheckBox.Name = "CurlyDoubleQuoteCheckBox";
            this.CurlyDoubleQuoteCheckBox.Size = new System.Drawing.Size(158, 17);
            this.CurlyDoubleQuoteCheckBox.TabIndex = 1;
            this.CurlyDoubleQuoteCheckBox.Text = "Enforce curly double quotes";
            this.CurlyDoubleQuoteCheckBox.UseVisualStyleBackColor = true;
            // 
            // CurlySimpleQuoteCheckBox
            // 
            this.CurlySimpleQuoteCheckBox.AutoSize = true;
            this.CurlySimpleQuoteCheckBox.Location = new System.Drawing.Point(248, 20);
            this.CurlySimpleQuoteCheckBox.Name = "CurlySimpleQuoteCheckBox";
            this.CurlySimpleQuoteCheckBox.Size = new System.Drawing.Size(155, 17);
            this.CurlySimpleQuoteCheckBox.TabIndex = 0;
            this.CurlySimpleQuoteCheckBox.Text = "Enforce curly simple quotes";
            this.CurlySimpleQuoteCheckBox.UseVisualStyleBackColor = true;
            // 
            // NotificationZone
            // 
            this.NotificationZone.Location = new System.Drawing.Point(12, 280);
            this.NotificationZone.Multiline = true;
            this.NotificationZone.Name = "NotificationZone";
            this.NotificationZone.Size = new System.Drawing.Size(360, 64);
            this.NotificationZone.TabIndex = 3;
            // 
            // CompanyLogo
            // 
            this.CompanyLogo.Image = ((System.Drawing.Image)(resources.GetObject("CompanyLogo.Image")));
            this.CompanyLogo.Location = new System.Drawing.Point(378, 280);
            this.CompanyLogo.Name = "CompanyLogo";
            this.CompanyLogo.Size = new System.Drawing.Size(64, 64);
            this.CompanyLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.CompanyLogo.TabIndex = 4;
            this.CompanyLogo.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(479, 369);
            this.Controls.Add(this.CompanyLogo);
            this.Controls.Add(this.NotificationZone);
            this.Controls.Add(this.OutputGroupBox);
            this.Controls.Add(this.InputGroupBox);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "BabelMeta Converter";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.InputGroupBox.ResumeLayout(false);
            this.InputGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CheckedPicto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WarningPicto)).EndInit();
            this.OutputGroupBox.ResumeLayout(false);
            this.OutputGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CompanyLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem actionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkIntegrityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertToSQLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertToXMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem solsticeLegacyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem solsticeIgniterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fugaToolStripMenuItem;
        private System.Windows.Forms.GroupBox InputGroupBox;
        private System.Windows.Forms.CheckBox DuplicatesCheckBox;
        private System.Windows.Forms.CheckBox ReferentialIntegrityCheckBox;
        private System.Windows.Forms.ListBox InputFormat;
        private System.Windows.Forms.GroupBox OutputGroupBox;
        private System.Windows.Forms.CheckBox FilterArtistCheckBox;
        private System.Windows.Forms.CheckBox FilterWorkCheckBox;
        private System.Windows.Forms.CheckBox CurlySimpleQuoteCheckBox;
        private System.Windows.Forms.PictureBox CheckedPicto;
        private System.Windows.Forms.PictureBox WarningPicto;
        private System.Windows.Forms.ProgressBar InputProgressBar;
        private System.Windows.Forms.ProgressBar OutputProgressBar;
        private System.Windows.Forms.ListBox InsertFormat;
        private System.Windows.Forms.CheckBox CurlyDoubleQuoteCheckBox;
        private System.Windows.Forms.TextBox NotificationZone;
        private System.Windows.Forms.PictureBox CompanyLogo;
        private System.Windows.Forms.ToolStripMenuItem loadSessionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveSessionToolStripMenuItem;
    }
}

