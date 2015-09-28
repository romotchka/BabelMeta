/*
 *  Babel Meta - babelmeta.com
 * 
 *  The present software is licensed according to the MIT licence.
 *  Copyright (c) 2015 Romain Carbou (romain@babelmeta.com)
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in
 *  all copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 *  THE SOFTWARE. 
 */

namespace BabelMeta
{
    /// <summary>
    /// Application main window
    /// </summary>
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
            this.templatedWorkbookToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.catalogWizardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkIntegrityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ddexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fugaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rightsUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.DoubleSpacesCheckBox = new System.Windows.Forms.CheckBox();
            this.OutputProgressBar = new System.Windows.Forms.ProgressBar();
            this.InsertFormat = new System.Windows.Forms.ListBox();
            this.CurlyDoubleQuotesCheckBox = new System.Windows.Forms.CheckBox();
            this.CurlySimpleQuotesCheckBox = new System.Windows.Forms.CheckBox();
            this.NotificationZone = new System.Windows.Forms.TextBox();
            this.CompanyLogo = new System.Windows.Forms.PictureBox();
            this.DbGroupBox = new System.Windows.Forms.GroupBox();
            this.DbSaveChanges = new System.Windows.Forms.Button();
            this.DbDatabasePassword = new System.Windows.Forms.MaskedTextBox();
            this.DbDatabasePasswordLabel = new System.Windows.Forms.Label();
            this.DbDatabaseUser = new System.Windows.Forms.TextBox();
            this.DbDatabaseUserLabel = new System.Windows.Forms.Label();
            this.DbDatabaseName = new System.Windows.Forms.TextBox();
            this.DbDatabaseNameLabel = new System.Windows.Forms.Label();
            this.DbServerNameLabel = new System.Windows.Forms.Label();
            this.DbEngineTypeLabel = new System.Windows.Forms.Label();
            this.DbServerName = new System.Windows.Forms.TextBox();
            this.DbEngineType = new System.Windows.Forms.ListBox();
            this.testButton = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.InputGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CheckedPicto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WarningPicto)).BeginInit();
            this.OutputGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CompanyLogo)).BeginInit();
            this.DbGroupBox.SuspendLayout();
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
            this.menuStrip1.Size = new System.Drawing.Size(798, 24);
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
            this.openToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.templatedWorkbookToolStripMenuItem,
            this.catalogWizardToolStripMenuItem});
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // templatedWorkbookToolStripMenuItem
            // 
            this.templatedWorkbookToolStripMenuItem.Name = "templatedWorkbookToolStripMenuItem";
            this.templatedWorkbookToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.templatedWorkbookToolStripMenuItem.Text = "Templated Workbook";
            this.templatedWorkbookToolStripMenuItem.Click += new System.EventHandler(this.templatedWorkbookToolStripMenuItem_Click);
            // 
            // catalogWizardToolStripMenuItem
            // 
            this.catalogWizardToolStripMenuItem.Name = "catalogWizardToolStripMenuItem";
            this.catalogWizardToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.catalogWizardToolStripMenuItem.Text = "Catalog Wizard";
            this.catalogWizardToolStripMenuItem.Click += new System.EventHandler(this.catalogWizardToolStripMenuItem_Click);
            // 
            // loadSessionToolStripMenuItem
            // 
            this.loadSessionToolStripMenuItem.Name = "loadSessionToolStripMenuItem";
            this.loadSessionToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.loadSessionToolStripMenuItem.Text = "Load Session";
            this.loadSessionToolStripMenuItem.Click += new System.EventHandler(this.loadSessionToolStripMenuItem_Click);
            // 
            // saveSessionToolStripMenuItem
            // 
            this.saveSessionToolStripMenuItem.Name = "saveSessionToolStripMenuItem";
            this.saveSessionToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.saveSessionToolStripMenuItem.Text = "Save Session";
            this.saveSessionToolStripMenuItem.Click += new System.EventHandler(this.saveSessionToolStripMenuItem_Click);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // actionToolStripMenuItem
            // 
            this.actionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkIntegrityToolStripMenuItem,
            this.generateToolStripMenuItem});
            this.actionToolStripMenuItem.Name = "actionToolStripMenuItem";
            this.actionToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.actionToolStripMenuItem.Text = "Action";
            // 
            // checkIntegrityToolStripMenuItem
            // 
            this.checkIntegrityToolStripMenuItem.Name = "checkIntegrityToolStripMenuItem";
            this.checkIntegrityToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.checkIntegrityToolStripMenuItem.Text = "Check integrity";
            this.checkIntegrityToolStripMenuItem.Click += new System.EventHandler(this.checkIntegrityToolStripMenuItem_Click);
            // 
            // generateToolStripMenuItem
            // 
            this.generateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ddexToolStripMenuItem,
            this.fugaToolStripMenuItem,
            this.rightsUpToolStripMenuItem});
            this.generateToolStripMenuItem.Name = "generateToolStripMenuItem";
            this.generateToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.generateToolStripMenuItem.Text = "Generate";
            // 
            // ddexToolStripMenuItem
            // 
            this.ddexToolStripMenuItem.Name = "ddexToolStripMenuItem";
            this.ddexToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.ddexToolStripMenuItem.Text = "Ddex";
            this.ddexToolStripMenuItem.Click += new System.EventHandler(this.ddexToolStripMenuItem_Click);
            // 
            // fugaToolStripMenuItem
            // 
            this.fugaToolStripMenuItem.Name = "fugaToolStripMenuItem";
            this.fugaToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.fugaToolStripMenuItem.Text = "Fuga";
            this.fugaToolStripMenuItem.Click += new System.EventHandler(this.fugaToolStripMenuItem_Click);
            // 
            // rightsUpToolStripMenuItem
            // 
            this.rightsUpToolStripMenuItem.Name = "rightsUpToolStripMenuItem";
            this.rightsUpToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.rightsUpToolStripMenuItem.Text = "RightsUp";
            this.rightsUpToolStripMenuItem.Click += new System.EventHandler(this.rightsUpToolStripMenuItem_Click);
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
            "Excel Workbook",
            "Excel XML 2003"});
            this.InputFormat.Location = new System.Drawing.Point(6, 19);
            this.InputFormat.Name = "InputFormat";
            this.InputFormat.Size = new System.Drawing.Size(209, 17);
            this.InputFormat.TabIndex = 0;
            // 
            // OutputGroupBox
            // 
            this.OutputGroupBox.AutoSize = true;
            this.OutputGroupBox.Controls.Add(this.DoubleSpacesCheckBox);
            this.OutputGroupBox.Controls.Add(this.OutputProgressBar);
            this.OutputGroupBox.Controls.Add(this.InsertFormat);
            this.OutputGroupBox.Controls.Add(this.CurlyDoubleQuotesCheckBox);
            this.OutputGroupBox.Controls.Add(this.CurlySimpleQuotesCheckBox);
            this.OutputGroupBox.Location = new System.Drawing.Point(12, 174);
            this.OutputGroupBox.Name = "OutputGroupBox";
            this.OutputGroupBox.Size = new System.Drawing.Size(430, 128);
            this.OutputGroupBox.TabIndex = 2;
            this.OutputGroupBox.TabStop = false;
            this.OutputGroupBox.Text = "Output Options";
            // 
            // DoubleSpacesCheckBox
            // 
            this.DoubleSpacesCheckBox.AutoSize = true;
            this.DoubleSpacesCheckBox.Checked = true;
            this.DoubleSpacesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.DoubleSpacesCheckBox.Location = new System.Drawing.Point(248, 67);
            this.DoubleSpacesCheckBox.Name = "DoubleSpacesCheckBox";
            this.DoubleSpacesCheckBox.Size = new System.Drawing.Size(175, 17);
            this.DoubleSpacesCheckBox.TabIndex = 4;
            this.DoubleSpacesCheckBox.Text = "Enforce double spaces removal";
            this.DoubleSpacesCheckBox.UseVisualStyleBackColor = true;
            this.DoubleSpacesCheckBox.CheckedChanged += new System.EventHandler(this.DoubleSpacesCheckBox_CheckedChanged);
            // 
            // OutputProgressBar
            // 
            this.OutputProgressBar.Location = new System.Drawing.Point(6, 99);
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
            // CurlyDoubleQuotesCheckBox
            // 
            this.CurlyDoubleQuotesCheckBox.AutoSize = true;
            this.CurlyDoubleQuotesCheckBox.Checked = true;
            this.CurlyDoubleQuotesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CurlyDoubleQuotesCheckBox.Location = new System.Drawing.Point(248, 43);
            this.CurlyDoubleQuotesCheckBox.Name = "CurlyDoubleQuotesCheckBox";
            this.CurlyDoubleQuotesCheckBox.Size = new System.Drawing.Size(158, 17);
            this.CurlyDoubleQuotesCheckBox.TabIndex = 1;
            this.CurlyDoubleQuotesCheckBox.Text = "Enforce curly double quotes";
            this.CurlyDoubleQuotesCheckBox.UseVisualStyleBackColor = true;
            this.CurlyDoubleQuotesCheckBox.CheckedChanged += new System.EventHandler(this.CurlyDoubleQuotesCheckBox_CheckedChanged);
            // 
            // CurlySimpleQuotesCheckBox
            // 
            this.CurlySimpleQuotesCheckBox.AutoSize = true;
            this.CurlySimpleQuotesCheckBox.Checked = true;
            this.CurlySimpleQuotesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CurlySimpleQuotesCheckBox.Location = new System.Drawing.Point(248, 20);
            this.CurlySimpleQuotesCheckBox.Name = "CurlySimpleQuotesCheckBox";
            this.CurlySimpleQuotesCheckBox.Size = new System.Drawing.Size(155, 17);
            this.CurlySimpleQuotesCheckBox.TabIndex = 0;
            this.CurlySimpleQuotesCheckBox.Text = "Enforce curly simple quotes";
            this.CurlySimpleQuotesCheckBox.UseVisualStyleBackColor = true;
            this.CurlySimpleQuotesCheckBox.CheckedChanged += new System.EventHandler(this.CurlySimpleQuotesCheckBox_CheckedChanged);
            // 
            // NotificationZone
            // 
            this.NotificationZone.Location = new System.Drawing.Point(12, 298);
            this.NotificationZone.Multiline = true;
            this.NotificationZone.Name = "NotificationZone";
            this.NotificationZone.Size = new System.Drawing.Size(360, 64);
            this.NotificationZone.TabIndex = 3;
            // 
            // CompanyLogo
            // 
            this.CompanyLogo.Image = ((System.Drawing.Image)(resources.GetObject("CompanyLogo.Image")));
            this.CompanyLogo.Location = new System.Drawing.Point(378, 298);
            this.CompanyLogo.Name = "CompanyLogo";
            this.CompanyLogo.Size = new System.Drawing.Size(64, 64);
            this.CompanyLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.CompanyLogo.TabIndex = 4;
            this.CompanyLogo.TabStop = false;
            // 
            // DbGroupBox
            // 
            this.DbGroupBox.Controls.Add(this.testButton);
            this.DbGroupBox.Controls.Add(this.DbSaveChanges);
            this.DbGroupBox.Controls.Add(this.DbDatabasePassword);
            this.DbGroupBox.Controls.Add(this.DbDatabasePasswordLabel);
            this.DbGroupBox.Controls.Add(this.DbDatabaseUser);
            this.DbGroupBox.Controls.Add(this.DbDatabaseUserLabel);
            this.DbGroupBox.Controls.Add(this.DbDatabaseName);
            this.DbGroupBox.Controls.Add(this.DbDatabaseNameLabel);
            this.DbGroupBox.Controls.Add(this.DbServerNameLabel);
            this.DbGroupBox.Controls.Add(this.DbEngineTypeLabel);
            this.DbGroupBox.Controls.Add(this.DbServerName);
            this.DbGroupBox.Controls.Add(this.DbEngineType);
            this.DbGroupBox.Location = new System.Drawing.Point(448, 42);
            this.DbGroupBox.Name = "DbGroupBox";
            this.DbGroupBox.Size = new System.Drawing.Size(200, 320);
            this.DbGroupBox.TabIndex = 5;
            this.DbGroupBox.TabStop = false;
            this.DbGroupBox.Text = "Database Options";
            // 
            // DbSaveChanges
            // 
            this.DbSaveChanges.Location = new System.Drawing.Point(7, 236);
            this.DbSaveChanges.Name = "DbSaveChanges";
            this.DbSaveChanges.Size = new System.Drawing.Size(175, 24);
            this.DbSaveChanges.TabIndex = 10;
            this.DbSaveChanges.Text = "Save changes";
            this.DbSaveChanges.UseVisualStyleBackColor = true;
            this.DbSaveChanges.Click += new System.EventHandler(this.DbSaveChanges_Click);
            // 
            // DbDatabasePassword
            // 
            this.DbDatabasePassword.Location = new System.Drawing.Point(6, 208);
            this.DbDatabasePassword.Name = "DbDatabasePassword";
            this.DbDatabasePassword.PasswordChar = '*';
            this.DbDatabasePassword.Size = new System.Drawing.Size(176, 20);
            this.DbDatabasePassword.TabIndex = 9;
            // 
            // DbDatabasePasswordLabel
            // 
            this.DbDatabasePasswordLabel.AutoSize = true;
            this.DbDatabasePasswordLabel.Location = new System.Drawing.Point(5, 193);
            this.DbDatabasePasswordLabel.Name = "DbDatabasePasswordLabel";
            this.DbDatabasePasswordLabel.Size = new System.Drawing.Size(53, 13);
            this.DbDatabasePasswordLabel.TabIndex = 8;
            this.DbDatabasePasswordLabel.Text = "Password";
            // 
            // DbDatabaseUser
            // 
            this.DbDatabaseUser.Location = new System.Drawing.Point(6, 167);
            this.DbDatabaseUser.Name = "DbDatabaseUser";
            this.DbDatabaseUser.Size = new System.Drawing.Size(176, 20);
            this.DbDatabaseUser.TabIndex = 7;
            this.DbDatabaseUser.TextChanged += new System.EventHandler(this.DbDatabaseUser_TextChanged);
            // 
            // DbDatabaseUserLabel
            // 
            this.DbDatabaseUserLabel.AutoSize = true;
            this.DbDatabaseUserLabel.Location = new System.Drawing.Point(3, 152);
            this.DbDatabaseUserLabel.Name = "DbDatabaseUserLabel";
            this.DbDatabaseUserLabel.Size = new System.Drawing.Size(58, 13);
            this.DbDatabaseUserLabel.TabIndex = 6;
            this.DbDatabaseUserLabel.Text = "User name";
            // 
            // DbDatabaseName
            // 
            this.DbDatabaseName.Location = new System.Drawing.Point(6, 117);
            this.DbDatabaseName.Name = "DbDatabaseName";
            this.DbDatabaseName.Size = new System.Drawing.Size(176, 20);
            this.DbDatabaseName.TabIndex = 5;
            this.DbDatabaseName.TextChanged += new System.EventHandler(this.DbDatabaseName_TextChanged);
            // 
            // DbDatabaseNameLabel
            // 
            this.DbDatabaseNameLabel.AutoSize = true;
            this.DbDatabaseNameLabel.Location = new System.Drawing.Point(5, 102);
            this.DbDatabaseNameLabel.Name = "DbDatabaseNameLabel";
            this.DbDatabaseNameLabel.Size = new System.Drawing.Size(82, 13);
            this.DbDatabaseNameLabel.TabIndex = 4;
            this.DbDatabaseNameLabel.Text = "Database name";
            // 
            // DbServerNameLabel
            // 
            this.DbServerNameLabel.AutoSize = true;
            this.DbServerNameLabel.Location = new System.Drawing.Point(6, 59);
            this.DbServerNameLabel.Name = "DbServerNameLabel";
            this.DbServerNameLabel.Size = new System.Drawing.Size(67, 13);
            this.DbServerNameLabel.TabIndex = 3;
            this.DbServerNameLabel.Text = "Server name";
            // 
            // DbEngineTypeLabel
            // 
            this.DbEngineTypeLabel.AutoSize = true;
            this.DbEngineTypeLabel.Location = new System.Drawing.Point(6, 19);
            this.DbEngineTypeLabel.Name = "DbEngineTypeLabel";
            this.DbEngineTypeLabel.Size = new System.Drawing.Size(63, 13);
            this.DbEngineTypeLabel.TabIndex = 2;
            this.DbEngineTypeLabel.Text = "Engine type";
            // 
            // DbServerName
            // 
            this.DbServerName.Location = new System.Drawing.Point(6, 74);
            this.DbServerName.Name = "DbServerName";
            this.DbServerName.Size = new System.Drawing.Size(176, 20);
            this.DbServerName.TabIndex = 1;
            this.DbServerName.TextChanged += new System.EventHandler(this.DbServerName_TextChanged);
            // 
            // DbEngineType
            // 
            this.DbEngineType.FormattingEnabled = true;
            this.DbEngineType.Items.AddRange(new object[] {
            "MySql"});
            this.DbEngineType.Location = new System.Drawing.Point(6, 35);
            this.DbEngineType.Name = "DbEngineType";
            this.DbEngineType.Size = new System.Drawing.Size(176, 17);
            this.DbEngineType.TabIndex = 0;
            this.DbEngineType.SelectedIndexChanged += new System.EventHandler(this.DbEngineType_SelectedIndexChanged);
            // 
            // testButton
            // 
            this.testButton.Location = new System.Drawing.Point(9, 292);
            this.testButton.Name = "testButton";
            this.testButton.Size = new System.Drawing.Size(173, 23);
            this.testButton.TabIndex = 11;
            this.testButton.Text = "Test";
            this.testButton.UseVisualStyleBackColor = true;
            this.testButton.Click += new System.EventHandler(this.testButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(798, 369);
            this.Controls.Add(this.DbGroupBox);
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
            this.DbGroupBox.ResumeLayout(false);
            this.DbGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.MenuStrip menuStrip1;

        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        
        private System.Windows.Forms.GroupBox InputGroupBox;
        private System.Windows.Forms.CheckBox DuplicatesCheckBox;
        private System.Windows.Forms.CheckBox ReferentialIntegrityCheckBox;
        private System.Windows.Forms.ListBox InputFormat;
        
        private System.Windows.Forms.GroupBox OutputGroupBox;
        private System.Windows.Forms.CheckBox FilterArtistCheckBox;
        private System.Windows.Forms.CheckBox FilterWorkCheckBox;
        private System.Windows.Forms.PictureBox CheckedPicto;
        private System.Windows.Forms.PictureBox WarningPicto;
        private System.Windows.Forms.ProgressBar InputProgressBar;
        private System.Windows.Forms.ProgressBar OutputProgressBar;
        private System.Windows.Forms.ListBox InsertFormat;
        private System.Windows.Forms.TextBox NotificationZone;
        private System.Windows.Forms.PictureBox CompanyLogo;

        private System.Windows.Forms.ToolStripMenuItem loadSessionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveSessionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem actionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkIntegrityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ddexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fugaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rightsUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem templatedWorkbookToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem catalogWizardToolStripMenuItem;

        private System.Windows.Forms.CheckBox CurlyDoubleQuotesCheckBox;
        private System.Windows.Forms.CheckBox CurlySimpleQuotesCheckBox;
        private System.Windows.Forms.CheckBox DoubleSpacesCheckBox;

        private System.Windows.Forms.GroupBox DbGroupBox;
        private System.Windows.Forms.Label DbEngineTypeLabel;
        private System.Windows.Forms.ListBox DbEngineType;
        private System.Windows.Forms.Label DbServerNameLabel;
        private System.Windows.Forms.TextBox DbServerName;
        private System.Windows.Forms.Label DbDatabaseNameLabel;
        private System.Windows.Forms.TextBox DbDatabaseName;
        private System.Windows.Forms.Label DbDatabaseUserLabel;
        private System.Windows.Forms.TextBox DbDatabaseUser;
        private System.Windows.Forms.Label DbDatabasePasswordLabel;
        private System.Windows.Forms.MaskedTextBox DbDatabasePassword;
        private System.Windows.Forms.Button DbSaveChanges;
        private System.Windows.Forms.Button testButton;
    }
}

