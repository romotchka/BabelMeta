﻿/*
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

using BabelMeta.AppConfig;
using BabelMeta.Helpers;
using BabelMeta.Model;
using BabelMeta.Model.Config;
using BabelMeta.Modules;
using BabelMeta.Modules.Control;
using BabelMeta.Modules.Export;
using BabelMeta.Modules.Import;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

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

            FilterArtistCheckBox.DataBindings.Add(new Binding("Checked", _viewModel, "FilterArtistChecked"));
            FilterWorkCheckBox.DataBindings.Add(new Binding("Checked", _viewModel, "FilterWorkChecked"));

            InputFormat.SelectedIndex = 0;

            _viewModel.PropertyChanged += OnViewModelPropertyChanged;

        }

        /// <summary>
        /// Main property changed switch so as to reflect view model events in the layout.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName.ToLower())
            {
                case "notification":
                    Notify(_viewModel.Notification);
                    break;
            }
        }

        private readonly MainFormViewModel _viewModel;


        /* FILE MENU */

        /// <summary>
        /// File -> Open
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void openToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            Stream stream = null;
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = "c:\\",
                Title = "Open a default file",
                RestoreDirectory = true,
            };

            if (InputFormat.SelectedItem == null)
            {
                Notify("No format selected.");
                return;
            }

            switch (InputFormat.SelectedItem.ToString().ToFileFormatType())
            {
                case FileFormatType.ExcelWorkbook:
                    openFileDialog.Filter = "Excel files (*.xls, *.xlsx)|*.xls*";
                    break;
                case FileFormatType.ExcelXml2003:
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
                    Notify("Opening file stream.");

                    CatalogContext.Instance.Init();
                    CatalogContext.Instance.Initialized = false;

                    InputProgressBar.Value = 0;
                    InputProgressBar.Visible = true;
                    OutputProgressBar.Visible = false;

                    // Call appropriate parser, depending on input format
                    var r = await TemplatedCatalogReader.Instance.Parse(openFileDialog, InputFormat.SelectedItem.ToString().ToFileFormatType(), _viewModel);

                    if (r == ReturnCode.Ok)
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

        private void DeserializeBmdFile(String filePath)
        {
            if (String.IsNullOrEmpty(filePath))
            {
                return;
            }

            Stream stream = File.Open(filePath, FileMode.Open);
            var binaryFormatter = new BinaryFormatter();

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

                case "assets":
                    CatalogContext.Instance.Assets = (List<Asset>)binaryFormatter.Deserialize(stream);
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
            var fbd = new FolderBrowserDialog
            {
                Description = "Select a root folder to load session files.",
            };

            fbd.ShowDialog();

            if (String.IsNullOrEmpty(fbd.SelectedPath))
            {
                return;
            }


            var getFiles = Directory.GetFiles(fbd.SelectedPath, "*.bmd");
            if (getFiles.Count() != 9)
            {
                Notify("The session file structure is corrupted.");
            }

            var files = getFiles.ToList();

            CatalogContext.Instance.Init();
            CatalogContext.Instance.Initialized = false;

            try
            {
                foreach (var file in files)
                {
                    DeserializeBmdFile(file);
                }

                if (_viewModel.FilterArtistChecked) CatalogContext.Instance.FilterUnusedArtists();
                else
                if (_viewModel.FilterWorkChecked) CatalogContext.Instance.FilterUnusedWorks();

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
            var fbd = new FolderBrowserDialog
            {
                Description = "Select a root folder to save session files."
            };

            fbd.ShowDialog();

            if (String.IsNullOrEmpty(fbd.SelectedPath))
            {
                return;
            }
            if ((fbd.SelectedPath.ToCharArray())[fbd.SelectedPath.Length - 1] != '\\')
            {
                fbd.SelectedPath += "\\";
            }

            var binaryFormatter = new BinaryFormatter();

            try
            {
                // Settings
                Stream stream = File.Open(fbd.SelectedPath + "Settings.bmd", FileMode.Create);

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
                stream = File.Open(fbd.SelectedPath + "Assets.bmd", FileMode.Create);

                binaryFormatter.Serialize(stream, CatalogContext.Instance.Assets);
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

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
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
                    (CatalogContext.Instance.RedundantKeysChecked = ModelIntgrityChecker.Instance.CheckRedundantKeys())
                    && (CatalogContext.Instance.ReferentialIntegrityChecked = ModelIntgrityChecker.Instance.CheckReferentialIntegrity())
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
        private void solsticeLegacyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Notify("Not implemented yet.");
        }

        /// <summary>
        /// Action -> Convert to SQL -> Solstice Igniter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void solsticeIgniterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Notify("Not implemented yet.");
        }

        /// <summary>
        /// Action -> Convert to XML -> Fuga
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fugaToolStripMenuItem_Click(object sender, EventArgs e)
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

            var fbd = new FolderBrowserDialog
            {
                Description = "Select a root folder for output sub-folders and XML files.",
            };

            fbd.ShowDialog();

            if (String.IsNullOrEmpty(fbd.SelectedPath))
            {
                return;
            }
            var r = FugaXmlCatalogWriter.Instance.Generate(fbd.SelectedPath, _viewModel);

            if (r == ReturnCode.Ok)
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
        internal void Notify(String message)
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
