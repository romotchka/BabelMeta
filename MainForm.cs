/*
 * Classical Metadata Converter
 * Copyright 2015 - Romain Carbou
 * romain.carbou@solstice-music.com
 */

using MetadataConverter.Modules.Import;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MetadataConverter
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            _viewModel = new MainFormViewModel();

            this.InputProgressBar.DataBindings.Add(new Binding("Maximum", _viewModel, "InputProgressBarMax"));
            this.InputProgressBar.DataBindings.Add(new Binding("Value", _viewModel, "InputProgressBarValue"));
    
        }

        private MainFormViewModel _viewModel;

        /* FILE MENU */

        /// <summary>
        /// File -> Open
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            Stream stream = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.RestoreDirectory = true;
            switch (this.InputFormat.TabIndex)
            {
                case 0:
                    openFileDialog.Filter = "XML files (*.xml)|*.xml";
                    break;

                default:
                    openFileDialog.Filter = "All files (*.*)|*.*";
                    break;
            }

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((stream = openFileDialog.OpenFile()) == null)
                    {
                        this.NotificationZone.Text += "Error: Null file stream.\n";
                        return;
                    }

                    this.NotificationZone.Text += "Opening file stream.\n";

                    // Call appropriate parser, depending on input format
                    switch (this.InputFormat.TabIndex)
                    {
                        case 0:
                            this.InputProgressBar.Visible = true;
                            this.OutputProgressBar.Visible = false;
                            SolsticeDefaultExcel2003Xml.Instance.Parse(stream, _viewModel);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void quitToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
        


        /* ACTION MENU */

        /// <summary>
        /// Action -> Convert to SQL -> Solstice Legacy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void solsticeLegacyToolStripMenuItem_Click(object sender, System.EventArgs e)
        {

        }

        /// <summary>
        /// Action -> Convert to SQL -> Solstice Igniter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void solsticeIgniterToolStripMenuItem_Click(object sender, System.EventArgs e)
        {

        }

        /// <summary>
        /// Action -> Convert to XML -> Fuga
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fugaToolStripMenuItem_Click(object sender, System.EventArgs e)
        {

        }

    }
}
