﻿using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;
using OverlayJson;

namespace PoeCompass
{
    public partial class MainForm : Form
    {
        string logPath = null;

        FileStream fileStream;
        StreamReader logStream;
        Regex zoneEntered;
        OverlayData[] jsonData;
        string currentDirectory;
        PictureBox[] listOfPictures = new PictureBox[10];
        private bool dragging = false;
        private Point pointClicked;
        private bool partTwo = false;
        string zoneName = null;

        public MainForm()
        {

            currentDirectory = Directory.GetCurrentDirectory();

            try
            {
                jsonData = OverlayData.FromJson(File.ReadAllText(String.Format("{0}\\{1}", currentDirectory, "configuration.json")));
            }
            catch (Exception e)
            {
                MessageBox.Show(
                  "The configuration file could not be found within the directory of the application (configuration.json)",
                  "PoeCompass - error while reading configuration file",
                  MessageBoxButtons.OK,
                  MessageBoxIcon.Error
                );

                Environment.Exit(0);
            }

            logPath = jsonData[0].clientLogFile.ToString();

            // Initialize the app 
            InitializeComponent();
            InitializeStream();

            // Define the regex to scan ...
            zoneEntered = new Regex(@"You have entered (.*)\.", RegexOptions.IgnoreCase);
            
        }

        private void InitializeStream()
        {
            if (File.Exists(logPath))
            {
                try
                {
                    fileStream = File.Open(logPath, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.ReadWrite);
                    logStream = new StreamReader(fileStream);
                } catch (Exception ex)
                {
                    MessageBox.Show("Could not open log file: " + ex.ToString());
                    Environment.Exit(0);
                }

                // Move to the end of the file
                fileStream.Seek(-512, SeekOrigin.End);

                // Start watching client log file
                zoneWatcher.Enabled = true;

            }
            else
            {
                MessageBox.Show("Could not open log file: " + logPath);
                Environment.Exit(0);
            }
        }
        
        private Tuple<string, object[], string> FindZoneName(string zoneName)
        {
            bool firstHit = false;
            foreach (var region in jsonData)
                if (region.clientLogFile != null) {
                    continue;
                } else
                {
                    foreach (var zone in region.Zone)
                    {
                        // if PartTwo is enabled, skip first entry...
                        if (zone.ZoneName.Equals(zoneName))
                        {
                            if (partTwo && !firstHit)
                            {
                                firstHit = true;
                            }
                            else
                            {
                                return Tuple.Create(region.Region, zone.ZoneSeed, zone.Note);
                            }

                        }
                    }
                }

            return Tuple.Create("", new object[0], "");
        }

        private void ReadNewLines_Timer(object sender, EventArgs e)
        {
            // Read new line every 100 ms and detect zone...
            string line = logStream.ReadToEnd();
            string image = null;

            Match m = zoneEntered.Match(line);

            if (m.Success)
            {
                // New zone has been entered - update graphics. 
                zoneName = m.Groups[1].ToString();
                ClearMap();

                // Attempt to find a corresponding zoneName 

                var seedList = FindZoneName(zoneName);
                if (seedList.Item2.Length > 0)
                {
                    var seed_no = 0;
                    foreach (var seed in seedList.Item2)
                    {

                        image = String.Format("{0}\\Overlays\\{1}\\{2}.png", currentDirectory, seedList.Item1, seed);
                        DrawMap(image, seedList.Item3, seed_no);

                        seed_no++;
                    }
                } else if (seedList.Item3 != null) 
                {
                    // Notes only - no map seed
                    image = String.Format("{0}\\Overlays\\no_overlay.png", currentDirectory);
                    DrawMapNoOverlay(image, seedList.Item3);
                }
                else
                {
                    // No overlay found.
                    image = String.Format("{0}\\Overlays\\no_overlay.png", currentDirectory);
                    DrawMapNoOverlay(image, "No information.");
                }
            }

        }

        private void DrawMapNoOverlay(string image, string note)
        {
            // Adding picture
            listOfPictures[0] = new PictureBox
            {
                Name = image,
                Size = new Size(128, 72),
                Image = Image.FromFile(image),
                Cursor = Cursors.Hand,
                Location = new Point(0, 0)

            };

            listOfPictures[0].MouseDown += new System.Windows.Forms.MouseEventHandler(this.overlayMap_MouseDown);
            listOfPictures[0].MouseUp += new System.Windows.Forms.MouseEventHandler(this.overlayMap_MouseUp);
            listOfPictures[0].MouseMove += new System.Windows.Forms.MouseEventHandler(this.overlayMap_MouseMove);
            listOfPictures[0].MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OverlayMap_MouseDoubleClick);

            this.Controls.Add(listOfPictures[0]);

            this.Width = 128;
            this.Height = 114;

            this.noteLabel.Location = new Point(0, 72);
            this.noteLabel.Text = note;
            this.noteLabel.Height = 42;
            this.noteLabel.Width = 128;

        }

        private void ClearMap()
        {
            foreach (var item in listOfPictures)
                this.Controls.Remove(item);

            this.Height = 0;
            this.Width = 0;

            this.noteLabel.Text = "No information.";
        }

        private void DrawMap(string path, string note, int seed)
        {

            listOfPictures[seed] = new PictureBox
            {
                Name = String.Format("pictureBox{0}", seed),
                Size = new Size(128, 72),
                Image = Image.FromFile(path),
                Cursor = Cursors.Hand,
                Location = new Point(128 * seed, 0)
                
            };

            listOfPictures[seed].MouseDown += new System.Windows.Forms.MouseEventHandler(this.overlayMap_MouseDown);
            listOfPictures[seed].MouseUp += new System.Windows.Forms.MouseEventHandler(this.overlayMap_MouseUp);
            listOfPictures[seed].MouseMove += new System.Windows.Forms.MouseEventHandler(this.overlayMap_MouseMove);
            listOfPictures[seed].MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OverlayMap_MouseDoubleClick);

            // Adding image to form. 
            this.Controls.Add(listOfPictures[seed]);

            // Resizing form
            this.Width = 128 + (128 * seed);
            this.Height = 114;

            // Resizing note label
            this.noteLabel.Location = new Point(0,72);
            this.noteLabel.Text = note;
            this.noteLabel.Width = 128 + (128 * seed);
            this.noteLabel.Height = 42;


        }

        private void OverlayMap_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            partTwo = !partTwo;
            if (partTwo) label1.Text = "P2";
            else label1.Text = "P1";

            // Redraw map 
            ClearMap();

            // This should be in a function.
            string image = null;
            var seedList = FindZoneName(zoneName);
            if (seedList.Item2.Length > 0)
            {
                var seed_no = 0;
                foreach (var seed in seedList.Item2)
                {

                    image = String.Format("{0}\\Overlays\\{1}\\{2}.png", currentDirectory, seedList.Item1, seed);
                    DrawMap(image, seedList.Item3, seed_no);

                    seed_no++;
                }
            }
            else if (seedList.Item3 != null)
            {
                image = String.Format("{0}\\Overlays\\no_overlay.png", currentDirectory);
                DrawMapNoOverlay(image, seedList.Item3);
            }
            else
            {
                // No overlay found.
                image = String.Format("{0}\\Overlays\\no_overlay.png", currentDirectory);
                DrawMapNoOverlay(image, "No information.");
            }
        }

        private void overlayMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point pointMoveTo;
                pointMoveTo = this.PointToScreen(new Point(e.X, e.Y));
                pointMoveTo.Offset(-pointClicked.X, -pointClicked.Y);
                this.Location = pointMoveTo;
            }
        }

        private void overlayMap_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void overlayMap_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                dragging = true;
                pointClicked = new Point(e.X, e.Y);
            }
            else
            {
                dragging = false;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Draw nothing.
            DrawMapNoOverlay(String.Format("{0}\\Overlays\\no_overlay.png", currentDirectory), "No information.");
        }
    }
}
