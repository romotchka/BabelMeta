/*
 * Babel Meta
 * Copyright 2015 - Romain Carbou
 * romain.carbou@solstice-music.com
 */

using BabelMeta.Model;
using BabelMeta.Modules.Control;
using BabelMeta.Modules.Export;
using BabelMeta.Modules.Import;
using BabelMeta.AppConfig;
using BabelMeta.Helpers;
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
using System.Runtime.Serialization.Formatters.Binary;
using BabelMeta.Model.Config;

namespace BabelMeta
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

            InputFormat.SelectedIndex = 0;
    
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
            openFileDialog.Title = "Open a default file";
            openFileDialog.RestoreDirectory = true;

            if (InputFormat.SelectedItem == null)
            {
                Notify("No format selected.");
                return;
            }

            switch (InputFormat.SelectedItem.ToString().ToLower())
            {
                case "xml":
                    openFileDialog.Filter = "XML files (*.xml)|*.xml";
                    break;
                case "excel":
                    openFileDialog.Filter = "Excel files (*.xls, *.xlsx)|*.xls*";
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

                    InputProgressBar.Value = 0;
                    InputProgressBar.Visible = true;
                    OutputProgressBar.Visible = false;

                    // Call appropriate parser, depending on input format
                    ReturnCodes r = DefaultCatalogReader.Instance.Parse(openFileDialog, InputFormat.SelectedItem.ToString(), _viewModel);

                    if (r == ReturnCodes.Ok)
                    {
                        Notify("Catalog parsing done.");
                    }
                }
                catch (Exception ex)
                {
                    Notify("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void DeserializeBmdFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            Stream stream;
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            stream = File.Open(filePath, FileMode.Open);
            binaryFormatter = new BinaryFormatter();

            switch ((filePath.GetFileNameFromFullPath().ToLower().Split('.'))[0])
            {
                case "settings":
                    CatalogContext.Instance.Settings = (CatalogSettings)binaryFormatter.Deserialize(stream);
                    break;

                case "langs":
                    CatalogContext.Instance.Langs = (List<Lang>)binaryFormatter.Deserialize(stream);
                    break;

                case "tags":
                    CatalogContext.Instance.Tags = (List<Tag>)binaryFormatter.Deserialize(stream);
                    break;

                case "roles":
                    CatalogContext.Instance.Roles = (List<Role>)binaryFormatter.Deserialize(stream);
                    break;

                case "qualities":
                    CatalogContext.Instance.Qualities = (List<Quality>)binaryFormatter.Deserialize(stream);
                    break;

                case "artists":
                    CatalogContext.Instance.Artists = (List<Artist>)binaryFormatter.Deserialize(stream);
                    break;

                case "works":
                    CatalogContext.Instance.Works = (List<Work>)binaryFormatter.Deserialize(stream);
                    break;

                case "isrcs":
                    CatalogContext.Instance.Isrcs = (List<Isrc>)binaryFormatter.Deserialize(stream);
                    break;

                case "albums":
                    CatalogContext.Instance.Albums = (List<Album>)binaryFormatter.Deserialize(stream);
                    break;
            }

            stream.Close();

        }


        /// <summary>
        /// Deserializes a CatalogContext from file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            fbd.Description = "Select a root folder to load session files.";
            fbd.ShowDialog();

            if (string.IsNullOrEmpty(fbd.SelectedPath))
            {
                return;
            }

            List<string> files = Directory.GetFiles(fbd.SelectedPath, "*.bmd").ToList();

            CatalogContext.Instance.Init();
            CatalogContext.Instance.Initialized = false;

            try
            {
                foreach (var file in files)
                {
                    DeserializeBmdFile(file);
                }

                CatalogContext.Instance.Initialized = true;

                Notify("Session loaded.");
            }
            catch (Exception)
            {
                Notify("Session not readable.");
            }


        }

        /// <summary>
        /// Serializes a CatalogContext from file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            fbd.Description = "Select a root folder to save session files.";
            fbd.ShowDialog();

            if (string.IsNullOrEmpty(fbd.SelectedPath))
            {
                return;
            }
            if ((fbd.SelectedPath.ToCharArray())[fbd.SelectedPath.Length - 1] != '\\')
            {
                fbd.SelectedPath += "\\";
            }

            Stream stream;
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            try
            {
                // Settings
                stream = File.Open(fbd.SelectedPath + "Settings.bmd", FileMode.Create);

                binaryFormatter.Serialize(stream, CatalogContext.Instance.Settings);
                stream.Close();

                // Langs
                stream = File.Open(fbd.SelectedPath + "Langs.bmd", FileMode.Create);

                binaryFormatter.Serialize(stream, CatalogContext.Instance.Langs);
                stream.Close();

                // Tags
                stream = File.Open(fbd.SelectedPath + "Tags.bmd", FileMode.Create);

                binaryFormatter.Serialize(stream, CatalogContext.Instance.Tags);
                stream.Close();

                // Roles
                stream = File.Open(fbd.SelectedPath + "Roles.bmd", FileMode.Create);

                binaryFormatter.Serialize(stream, CatalogContext.Instance.Roles);
                stream.Close();

                // Qualities
                stream = File.Open(fbd.SelectedPath + "Qualities.bmd", FileMode.Create);

                binaryFormatter.Serialize(stream, CatalogContext.Instance.Qualities);
                stream.Close();

                // Artists
                stream = File.Open(fbd.SelectedPath + "Artists.bmd", FileMode.Create);

                binaryFormatter.Serialize(stream, CatalogContext.Instance.Artists);
                stream.Close();

                // Works
                stream = File.Open(fbd.SelectedPath + "Works.bmd", FileMode.Create);

                binaryFormatter.Serialize(stream, CatalogContext.Instance.Works);
                stream.Close();

                // Isrcs
                stream = File.Open(fbd.SelectedPath + "Isrcs.bmd", FileMode.Create);

                binaryFormatter.Serialize(stream, CatalogContext.Instance.Isrcs);
                stream.Close();

                // Albums
                stream = File.Open(fbd.SelectedPath + "Albums.bmd", FileMode.Create);

                binaryFormatter.Serialize(stream, CatalogContext.Instance.Albums);
                stream.Close();

                Notify("Session saved.");
            }

            catch
            {
                Notify("Session not saved: I/O error on disk.");
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
                string message = "The imported model is corrupted:";
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
            Notify("Not implemented yet.");
        }

        /// <summary>
        /// Action -> Convert to SQL -> Solstice Igniter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void solsticeIgniterToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            Notify("Not implemented yet.");
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

            if (string.IsNullOrEmpty(fbd.SelectedPath))
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
        private void Notify(string message)
        {
            if (string.IsNullOrEmpty(message))
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
