/*
 * Babel Meta
 * Copyright 2015 - Romain Carbou
 * romain.carbou@solstice-music.com
 */

using MetadataConverter.Model;
using MetadataConverter.Modules.Control;
using MetadataConverter.Modules.Export;
using MetadataConverter.Modules.Import;
using MetadataConverter.AppConfig;
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

            InputProgressBar.DataBindings.Add(new Binding("Maximum", _viewModel, "InputProgressBarMax"));
            InputProgressBar.DataBindings.Add(new Binding("Value", _viewModel, "InputProgressBarValue"));

            CheckedPicto.DataBindings.Add(new Binding("Visible", _viewModel, "CheckedPictoVisibility"));
            WarningPicto.DataBindings.Add(new Binding("Visible", _viewModel, "WarningPictoVisibility"));
    
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
            openFileDialog.Title = "Open a default XML file";
            openFileDialog.RestoreDirectory = true;
            switch (InputFormat.TabIndex)
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
                        Notify("Error: Null file stream.");
                        return;
                    }

                    Notify("Opening file stream.");

                    CatalogContext.Instance.Init();
                    CatalogContext.Instance.Initialized = false;

                    // Call appropriate parser, depending on input format
                    switch (InputFormat.TabIndex)
                    {
                        case 0:
                            InputProgressBar.Value = 0;
                            InputProgressBar.Visible = true;
                            OutputProgressBar.Visible = false;
                            ReturnCodes r =  DefaultCatalogReader.Instance.Parse(stream, _viewModel);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Notify("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void quitToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            Close();
        }
        


        /* ACTION MENU */

        private void checkIntegrityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoCheckIntegrity();
        }

        private void DoCheckIntegrity()
        {
            if (
                    (CatalogContext.Instance.RedundantKeysChecked = ModelIntgrityChecker.Instance.CheckRedundantKeys()) == true
                    && (CatalogContext.Instance.ReferentialIntegrityChecked = ModelIntgrityChecker.Instance.CheckReferentialIntegrity()) == true
                )
            {
                Notify("The imported model is valid.");
            }
            else
            {
                String message = "The imported model is corrupted:";
                if (!CatalogContext.Instance.RedundantKeysChecked)
                {
                    message += " Redundant keys exist.";
                }
                if (!CatalogContext.Instance.ReferentialIntegrityChecked)
                {
                    message += " Some foreign keys are invalid.";
                }
                Notify(message);
            }
        }

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
            if (!CatalogContext.Instance.Initialized)
            {
                Notify("Catalog not present.");
                return;
            }
            DoCheckIntegrity();
            if (!CatalogContext.Instance.IntegrityChecked)
            {
                return;
            }

            FolderBrowserDialog fbd = new FolderBrowserDialog();

            fbd.Description = "Select a root folder for output sub-folders and XML files";
            fbd.ShowDialog();

            if (String.IsNullOrEmpty(fbd.SelectedPath))
            {
                return;
            }
            ReturnCodes r = FugaXmlCatalogWriter.Instance.Generate(fbd.SelectedPath, _viewModel);

            if (r == ReturnCodes.Ok)
            {
                Notify("Catalog export completed.");
            }
            else
            {
                Notify("An error occurred during export.");
            }
        }

        /// <summary>
        /// Append message on top of Notification Zone
        /// </summary>
        /// <param name="message"></param>
        private void Notify(String message)
        {
            if (String.IsNullOrEmpty(message))
            {
                return;
            }
            var current = NotificationZone.Text;
            NotificationZone.Clear();
            NotificationZone.AppendText(message + Environment.NewLine);
            NotificationZone.AppendText(current);
        }
    }
}
