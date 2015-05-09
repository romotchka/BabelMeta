/*
 * Metadata Converter
 * Copyright 2015 - Romain Carbou
 * romain.carbou@solstice-music.com
 */

namespace MetadataConverter
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
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            actionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            checkIntegrityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            convertToSQLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            solsticeLegacyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            solsticeIgniterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            convertToXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            fugaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            InputGroupBox = new System.Windows.Forms.GroupBox();
            CheckedPicto = new System.Windows.Forms.PictureBox();
            WarningPicto = new System.Windows.Forms.PictureBox();
            InputProgressBar = new System.Windows.Forms.ProgressBar();
            FilterArtistCheckBox = new System.Windows.Forms.CheckBox();
            FilterWorkCheckBox = new System.Windows.Forms.CheckBox();
            DuplicatesCheckBox = new System.Windows.Forms.CheckBox();
            ReferentialIntegrityCheckBox = new System.Windows.Forms.CheckBox();
            InputFormat = new System.Windows.Forms.ListBox();
            OutputGroupBox = new System.Windows.Forms.GroupBox();
            OutputProgressBar = new System.Windows.Forms.ProgressBar();
            InsertFormat = new System.Windows.Forms.ListBox();
            CurlyDoubleQuoteCheckBox = new System.Windows.Forms.CheckBox();
            CurlySimpleQuoteCheckBox = new System.Windows.Forms.CheckBox();
            NotificationZone = new System.Windows.Forms.TextBox();
            CompanyLogo = new System.Windows.Forms.PictureBox();
            menuStrip1.SuspendLayout();
            InputGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(CheckedPicto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(WarningPicto)).BeginInit();
            OutputGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(CompanyLogo)).BeginInit();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            toolStripMenuItem1,
            actionToolStripMenuItem,
            toolStripMenuItem2});
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new System.Drawing.Size(479, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            openToolStripMenuItem,
            quitToolStripMenuItem});
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new System.Drawing.Size(37, 20);
            toolStripMenuItem1.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += new System.EventHandler(openToolStripMenuItem_Click);
            // 
            // quitToolStripMenuItem
            // 
            quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            quitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            quitToolStripMenuItem.Text = "Quit";
            quitToolStripMenuItem.Click += new System.EventHandler(quitToolStripMenuItem_Click);
            // 
            // actionToolStripMenuItem
            // 
            actionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            checkIntegrityToolStripMenuItem,
            convertToSQLToolStripMenuItem,
            convertToXMLToolStripMenuItem});
            actionToolStripMenuItem.Name = "actionToolStripMenuItem";
            actionToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            actionToolStripMenuItem.Text = "Action";
            // 
            // checkIntegrityToolStripMenuItem
            // 
            checkIntegrityToolStripMenuItem.Name = "checkIntegrityToolStripMenuItem";
            checkIntegrityToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            checkIntegrityToolStripMenuItem.Text = "Check integrity";
            checkIntegrityToolStripMenuItem.Click += new System.EventHandler(checkIntegrityToolStripMenuItem_Click);
            // 
            // convertToSQLToolStripMenuItem
            // 
            convertToSQLToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            solsticeLegacyToolStripMenuItem,
            solsticeIgniterToolStripMenuItem});
            convertToSQLToolStripMenuItem.Name = "convertToSQLToolStripMenuItem";
            convertToSQLToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            convertToSQLToolStripMenuItem.Text = "Convert to SQL";
            // 
            // solsticeLegacyToolStripMenuItem
            // 
            solsticeLegacyToolStripMenuItem.Name = "solsticeLegacyToolStripMenuItem";
            solsticeLegacyToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            solsticeLegacyToolStripMenuItem.Text = "Solstice Legacy";
            solsticeLegacyToolStripMenuItem.Click += new System.EventHandler(solsticeLegacyToolStripMenuItem_Click);
            // 
            // solsticeIgniterToolStripMenuItem
            // 
            solsticeIgniterToolStripMenuItem.Name = "solsticeIgniterToolStripMenuItem";
            solsticeIgniterToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            solsticeIgniterToolStripMenuItem.Text = "Solstice Igniter";
            solsticeIgniterToolStripMenuItem.Click += new System.EventHandler(solsticeIgniterToolStripMenuItem_Click);
            // 
            // convertToXMLToolStripMenuItem
            // 
            convertToXMLToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            fugaToolStripMenuItem});
            convertToXMLToolStripMenuItem.Name = "convertToXMLToolStripMenuItem";
            convertToXMLToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            convertToXMLToolStripMenuItem.Text = "Convert to XML";
            // 
            // fugaToolStripMenuItem
            // 
            fugaToolStripMenuItem.Name = "fugaToolStripMenuItem";
            fugaToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            fugaToolStripMenuItem.Text = "Fuga";
            fugaToolStripMenuItem.Click += new System.EventHandler(fugaToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            helpToolStripMenuItem,
            aboutToolStripMenuItem});
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new System.Drawing.Size(24, 20);
            toolStripMenuItem2.Text = "?";
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            aboutToolStripMenuItem.Text = "About";
            // 
            // InputGroupBox
            // 
            InputGroupBox.AutoSize = true;
            InputGroupBox.Controls.Add(CheckedPicto);
            InputGroupBox.Controls.Add(WarningPicto);
            InputGroupBox.Controls.Add(InputProgressBar);
            InputGroupBox.Controls.Add(FilterArtistCheckBox);
            InputGroupBox.Controls.Add(FilterWorkCheckBox);
            InputGroupBox.Controls.Add(DuplicatesCheckBox);
            InputGroupBox.Controls.Add(ReferentialIntegrityCheckBox);
            InputGroupBox.Controls.Add(InputFormat);
            InputGroupBox.Location = new System.Drawing.Point(12, 42);
            InputGroupBox.Name = "InputGroupBox";
            InputGroupBox.Size = new System.Drawing.Size(430, 126);
            InputGroupBox.TabIndex = 1;
            InputGroupBox.TabStop = false;
            InputGroupBox.Text = "Input Options";
            // 
            // CheckedPicto
            // 
            CheckedPicto.Image = ((System.Drawing.Image)(resources.GetObject("CheckedPicto.Image")));
            CheckedPicto.Location = new System.Drawing.Point(372, 75);
            CheckedPicto.Name = "CheckedPicto";
            CheckedPicto.Size = new System.Drawing.Size(32, 32);
            CheckedPicto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            CheckedPicto.TabIndex = 4;
            CheckedPicto.TabStop = false;
            CheckedPicto.Visible = false;
            // 
            // WarningPicto
            // 
            WarningPicto.Image = ((System.Drawing.Image)(resources.GetObject("WarningPicto.Image")));
            WarningPicto.Location = new System.Drawing.Point(372, 75);
            WarningPicto.Name = "WarningPicto";
            WarningPicto.Size = new System.Drawing.Size(32, 32);
            WarningPicto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            WarningPicto.TabIndex = 4;
            WarningPicto.TabStop = false;
            WarningPicto.Visible = false;
            // 
            // InputProgressBar
            // 
            InputProgressBar.Location = new System.Drawing.Point(6, 88);
            InputProgressBar.Name = "InputProgressBar";
            InputProgressBar.Size = new System.Drawing.Size(360, 10);
            InputProgressBar.Step = 1;
            InputProgressBar.TabIndex = 5;
            InputProgressBar.Visible = false;
            // 
            // FilterArtistCheckBox
            // 
            FilterArtistCheckBox.AutoSize = true;
            FilterArtistCheckBox.Location = new System.Drawing.Point(248, 42);
            FilterArtistCheckBox.Name = "FilterArtistCheckBox";
            FilterArtistCheckBox.Size = new System.Drawing.Size(127, 17);
            FilterArtistCheckBox.TabIndex = 4;
            FilterArtistCheckBox.Text = "Filter artists not in use";
            FilterArtistCheckBox.UseVisualStyleBackColor = true;
            // 
            // FilterWorkCheckBox
            // 
            FilterWorkCheckBox.AutoSize = true;
            FilterWorkCheckBox.Location = new System.Drawing.Point(248, 19);
            FilterWorkCheckBox.Name = "FilterWorkCheckBox";
            FilterWorkCheckBox.Size = new System.Drawing.Size(128, 17);
            FilterWorkCheckBox.TabIndex = 3;
            FilterWorkCheckBox.Text = "Filter works not in use";
            FilterWorkCheckBox.UseVisualStyleBackColor = true;
            // 
            // DuplicatesCheckBox
            // 
            DuplicatesCheckBox.AutoSize = true;
            DuplicatesCheckBox.Checked = true;
            DuplicatesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            DuplicatesCheckBox.Location = new System.Drawing.Point(6, 65);
            DuplicatesCheckBox.Name = "DuplicatesCheckBox";
            DuplicatesCheckBox.Size = new System.Drawing.Size(108, 17);
            DuplicatesCheckBox.TabIndex = 2;
            DuplicatesCheckBox.Text = "Check duplicates";
            DuplicatesCheckBox.UseVisualStyleBackColor = true;
            // 
            // ReferentialIntegrityCheckBox
            // 
            ReferentialIntegrityCheckBox.AutoSize = true;
            ReferentialIntegrityCheckBox.Checked = true;
            ReferentialIntegrityCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            ReferentialIntegrityCheckBox.Location = new System.Drawing.Point(6, 42);
            ReferentialIntegrityCheckBox.Name = "ReferentialIntegrityCheckBox";
            ReferentialIntegrityCheckBox.Size = new System.Drawing.Size(145, 17);
            ReferentialIntegrityCheckBox.TabIndex = 1;
            ReferentialIntegrityCheckBox.Text = "Check referential integrity";
            ReferentialIntegrityCheckBox.UseVisualStyleBackColor = true;
            // 
            // InputFormat
            // 
            InputFormat.FormattingEnabled = true;
            InputFormat.Items.AddRange(new object[] {
            "Default XML"});
            InputFormat.Location = new System.Drawing.Point(6, 19);
            InputFormat.Name = "InputFormat";
            InputFormat.Size = new System.Drawing.Size(209, 17);
            InputFormat.TabIndex = 0;
            // 
            // OutputGroupBox
            // 
            OutputGroupBox.AutoSize = true;
            OutputGroupBox.Controls.Add(OutputProgressBar);
            OutputGroupBox.Controls.Add(InsertFormat);
            OutputGroupBox.Controls.Add(CurlyDoubleQuoteCheckBox);
            OutputGroupBox.Controls.Add(CurlySimpleQuoteCheckBox);
            OutputGroupBox.Location = new System.Drawing.Point(12, 174);
            OutputGroupBox.Name = "OutputGroupBox";
            OutputGroupBox.Size = new System.Drawing.Size(430, 95);
            OutputGroupBox.TabIndex = 2;
            OutputGroupBox.TabStop = false;
            OutputGroupBox.Text = "Output Options";
            // 
            // OutputProgressBar
            // 
            OutputProgressBar.Location = new System.Drawing.Point(6, 66);
            OutputProgressBar.Name = "OutputProgressBar";
            OutputProgressBar.Size = new System.Drawing.Size(360, 10);
            OutputProgressBar.Step = 1;
            OutputProgressBar.TabIndex = 3;
            OutputProgressBar.Visible = false;
            // 
            // InsertFormat
            // 
            InsertFormat.FormattingEnabled = true;
            InsertFormat.Items.AddRange(new object[] {
            "Use INSERT in SQL",
            "Use INSERT/REPLACE in SQL"});
            InsertFormat.Location = new System.Drawing.Point(6, 20);
            InsertFormat.Name = "InsertFormat";
            InsertFormat.Size = new System.Drawing.Size(209, 17);
            InsertFormat.TabIndex = 2;
            // 
            // CurlyDoubleQuoteCheckBox
            // 
            CurlyDoubleQuoteCheckBox.AutoSize = true;
            CurlyDoubleQuoteCheckBox.Location = new System.Drawing.Point(248, 43);
            CurlyDoubleQuoteCheckBox.Name = "CurlyDoubleQuoteCheckBox";
            CurlyDoubleQuoteCheckBox.Size = new System.Drawing.Size(158, 17);
            CurlyDoubleQuoteCheckBox.TabIndex = 1;
            CurlyDoubleQuoteCheckBox.Text = "Enforce curly double quotes";
            CurlyDoubleQuoteCheckBox.UseVisualStyleBackColor = true;
            // 
            // CurlySimpleQuoteCheckBox
            // 
            CurlySimpleQuoteCheckBox.AutoSize = true;
            CurlySimpleQuoteCheckBox.Location = new System.Drawing.Point(248, 20);
            CurlySimpleQuoteCheckBox.Name = "CurlySimpleQuoteCheckBox";
            CurlySimpleQuoteCheckBox.Size = new System.Drawing.Size(155, 17);
            CurlySimpleQuoteCheckBox.TabIndex = 0;
            CurlySimpleQuoteCheckBox.Text = "Enforce curly simple quotes";
            CurlySimpleQuoteCheckBox.UseVisualStyleBackColor = true;
            // 
            // NotificationZone
            // 
            NotificationZone.Location = new System.Drawing.Point(12, 280);
            NotificationZone.Multiline = true;
            NotificationZone.Name = "NotificationZone";
            NotificationZone.Size = new System.Drawing.Size(360, 64);
            NotificationZone.TabIndex = 3;
            // 
            // CompanyLogo
            // 
            CompanyLogo.Image = ((System.Drawing.Image)(resources.GetObject("CompanyLogo.Image")));
            CompanyLogo.Location = new System.Drawing.Point(378, 280);
            CompanyLogo.Name = "CompanyLogo";
            CompanyLogo.Size = new System.Drawing.Size(64, 64);
            CompanyLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            CompanyLogo.TabIndex = 4;
            CompanyLogo.TabStop = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new System.Drawing.Size(479, 369);
            Controls.Add(CompanyLogo);
            Controls.Add(NotificationZone);
            Controls.Add(OutputGroupBox);
            Controls.Add(InputGroupBox);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            Text = "Metadata Converter";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            InputGroupBox.ResumeLayout(false);
            InputGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(CheckedPicto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(WarningPicto)).EndInit();
            OutputGroupBox.ResumeLayout(false);
            OutputGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(CompanyLogo)).EndInit();
            ResumeLayout(false);
            PerformLayout();

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
    }
}

