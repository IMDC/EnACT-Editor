﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AxShockwaveFlashObjects;
using System.Xml;

namespace EnACT
{
    /// <summary>
    /// A Flashplayer control designed to communicate with the swf that it has loaded.
    /// </summary>
    public partial class EngineView : AxShockwaveFlash
    {
        //A delegate (like a C function pointer) for handling the VideoLoaded event
        public delegate void VideoLoadedHandler(object sender, EventArgs e);
        //An event that is fired when the Flash Video is finished loading
        public event VideoLoadedHandler VideoLoaded;

        public EngineView() : base()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invokes the VideoLoaded event, provided it is not null
        /// </summary>
        /// <param name="e">Event Arguments</param>
        public virtual void OnVideoLoaded(EventArgs e)
        {
            Console.WriteLine("Event Fired!");
            if (VideoLoaded != null)
            {
                VideoLoaded(this, e);
            }
        }

        /// <summary>
        /// Pauses the video
        /// </summary>
        public void Pause()
        {
            CallFunction("<invoke name=\"" + "pause" + "\" returntype=\"xml\"></invoke>");
        }

        /// <summary>
        /// Plays the video
        /// </summary>
        public override void Play()
        {
            CallFunction("<invoke name=\"" + "play" + "\" returntype=\"xml\"></invoke>");
        }

        /// <summary>
        /// Toggles the play state of the video.
        /// </summary>
        public void TogglePlay()
        {
            CallFunction("<invoke name=\"" + "togglePlay" + "\" returntype=\"xml\"></invoke>");
        }

        /// <summary>
        /// Returns true or false if the Engine is playing a video or not.
        /// </summary>
        /// <returns>true or false</returns>
        public override Boolean IsPlaying()
        {
            String returnXML = CallFunction("<invoke name=\"" + "isPlaying" + "\" returntype=\"xml\"></invoke>");
            switch (returnXML)
            {
                case @"<true/>": return true;
                default: return false;
            }
        }

        /// <summary>
        /// Returns the length of the flash video in seconds
        /// </summary>
        /// <returns>A double</returns>
        public double VideoLength()
        {
            String returnString = CallFunction("<invoke name=\"" + "videoLength" + "\" returntype=\"xml\"></invoke>");

            //Turn the string into an xml doc
            XmlDocument xmlRequest = new XmlDocument();
            xmlRequest.LoadXml(returnString);

            //Get the value from the tag wrappers
            double length = Convert.ToDouble(xmlRequest["number"].InnerText);
            return length;
        }

        /// <summary>
        /// Handles the recieving of method calls from the .swf object.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event Arguments</param>
        private void EngineView_FlashCall(object sender, _IShockwaveFlashEvents_FlashCallEvent e)
        {
            Console.WriteLine(e.request);

            //Get the request
            XmlDocument xmlRequest = new XmlDocument();
            xmlRequest.LoadXml(e.request);

            //Get the name of the message
            String messageName = xmlRequest.FirstChild.Attributes[0].InnerText;

            switch (messageName)
            {
                case "Done Loading": OnVideoLoaded(EventArgs.Empty); break;
                default: Console.WriteLine("Unrecognized message: {0}", messageName); break;
            }
        }
    }//Class
}//Namespace
