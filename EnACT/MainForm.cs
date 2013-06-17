﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace EnACT
{
    public partial class MainForm : Form
    {
        #region Fields and Properties
        /// <summary>
        /// Direction used by MoveRow buttons to determine whether to move the row up or down
        /// </summary>
        private enum MoveDirection { Up, Down };

        /// <summary>
        /// The controller for enact.
        /// </summary>
        public EngineController Controller { set; get; }

        /// <summary>
        /// A set of Speaker objects, each speaker being mapped to by its name
        /// </summary>
        public Dictionary<String, Speaker> SpeakerSet { set; get; }

        /// <summary>
        /// A list of captions retrieved from a transcript file.
        /// </summary>
        public List<Caption> CaptionList { set; get; }

        /// <summary>
        /// The object that represents the EnACT engine xml settings file
        /// </summary>
        public SettingsXML Settings { set; get; }
        #endregion

        #region Constructor and Init Methods
        /// <summary>
        /// Constructs a Mainform object. Initializes all components
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            //Set up the Controller
            InitController();

            //Set references from controller.
            this.SpeakerSet = Controller.SpeakerSet;
            this.CaptionList = Controller.CaptionList;
            this.Settings = Controller.Settings;
        }

        /// <summary>
        /// Constructs and initializes the controller
        /// </summary>
        private void InitController()
        {
            Controller = new EngineController();

            Controller.CaptionView       = this.CaptionView;
            Controller.EngineView        = this.EngineView;
            Controller.PlayheadLabel     = this.PlayheadLabel;
            Controller.PlayheadTimer     = this.PlayheadTimer;
            Controller.Timeline          = this.Timeline;
            Controller.TrackBar_Timeline = this.TrackBar_Timeline;

            Controller.InitControls();
        }
        #endregion

        /// <summary>
        /// Populates the Caption Table with captions read in from a script file.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event Args</param>
        private void PopulateButton_Click(object sender, EventArgs e)
        {
            CaptionView.UpdateView();
        }

        private void ParseText(object sender, EventArgs e)
        {
            TextParser t = new TextParser(SpeakerSet, CaptionList);
            t.ParseScriptFile(Paths.TestScript);
            CaptionView.UpdateView();
        }

        /// <summary>
        /// Writes the three EnACT xml files.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        private void WriteXML(object sender, EventArgs e)
        {
            EnactXMLWriter w = new EnactXMLWriter(SpeakerSet,CaptionList,Settings);
            w.WriteAll();
        }

        /// <summary>
        /// Inserts a new row where the user has selected in the 
        /// caption View.
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">Event Arguments</param>
        private void Button_InsertRow_Click(object sender, EventArgs e)
        {
            CaptionView.AddRow();
        }

        /// <summary>
        /// Deletes all rows in the table that contain a cell that is
        /// currently selected.
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">The Event Arguments</param>
        private void Button_DeleteRow_Click(object sender, EventArgs e)
        {
            CaptionView.DeleteSelectedRows();
        }

        /// <summary>
        /// Moves the currently selected row up one position in the Caption Table
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event Arguments</param>
        private void Button_MoveRowUp_Click(object sender, EventArgs e)
        {
            CaptionView.MoveRowUp();
        }

        /// <summary>
        /// Moves the currently selected row down one position in the Caption Table
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event Arguments</param>
        private void Button_MoveRowDown_Click(object sender, EventArgs e)
        {
            CaptionView.MoveRowDown();
        }

        /// <summary>
        /// Toggles the video playing state between playing and paused
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event Arguments</param>
        private void TogglePlay(object sender, EventArgs e)
        {
            Controller.TogglePlay();
        }

        #region Jorge
        private void JorgeButton_Click(object sender, EventArgs e)
        {
            JorgeForm TheJorgeForm = new JorgeForm(SpeakerSet,CaptionList,Settings, this);
            TheJorgeForm.Show();
        }

        public void JorgeMethod(String SRTPath, String OutFolderPath)
        {
            TextParser t = new TextParser(SpeakerSet, CaptionList);
            t.ParseSRTFile(@SRTPath);
            PopulateButton_Click(null, null);

            EnactXMLWriter w = new EnactXMLWriter(
                OutFolderPath + @"\speakers.xml", SpeakerSet,
                OutFolderPath + @"\dialogues.xml", CaptionList,
                OutFolderPath + @"\Settings.xml", Settings);
            w.WriteAll();
        }
        #endregion

        private void Button_ParseESR_Click(object sender, EventArgs e)
        {
            TextParser t = new TextParser(SpeakerSet, CaptionList);
            t.ParseESRFile(Paths.TestESR);
            CaptionView.UpdateView();
        }

        private void DebugButton_Click(object sender, EventArgs e)
        {
            if (CaptionView.UserInputEnabled)
                CaptionView.UserInputEnabled = false;
            else
                CaptionView.UserInputEnabled = true;
        }

        /// <summary>
        /// Toggles drawing of caption location labels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_ShowLabels_Click(object sender, EventArgs e)
        {
            Timeline.ToggleDrawLocationLabels();
        }

        private void Button_ZoomTimelineIn_Click(object sender, EventArgs e)
        {
            Timeline.ZoomIn();
        }

        private void Button_ZoomTimelineOut_Click(object sender, EventArgs e)
        {
            Timeline.ZoomOut();
        }

        private void Button_ZoomReset_Click(object sender, EventArgs e)
        {
            Timeline.ZoomReset();
        }
    }//Class
}//Namespace
