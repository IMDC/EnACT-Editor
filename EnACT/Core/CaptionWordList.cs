﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnACT
{
    /// <summary>
    /// A List for Containing CaptionWords
    /// </summary>
    public class CaptionWordList : List<EditorCaptionWord>
    {
        #region Fields and Properties
        /// <summary>
        /// The width of spacing between words.
        /// </summary>
        public const int SPACE_WIDTH = 1;

        /// <summary>
        /// Gets or sets the CaptionWordList as a String.
        /// </summary>
        public String AsString
        {
            set { Feed(value); }
            get { return GetAsString(); }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a CaptionWordList with a default capacity.
        /// </summary>
        public CaptionWordList() : base() {}

        /// <summary>
        /// Constructs a CaptionWordList and feeds in a line of text.
        /// </summary>
        /// <param name="line">The line of Text to feed in.</param>
        public CaptionWordList(String line) : this()
        {
            Feed(line);
        }
        #endregion

        #region AsString Setter and Getter
        /// <summary>
        /// Clears the list, then feeds a string into the list and turns it into CaptionWords.
        /// </summary>
        /// <param name="line">The string to turn into a list of CaptionWords.</param>
        public void Feed(String line)
        {
            //Remove the previous line from the Words
            Clear();

            //Split line up and add each word to the wordlist.
            String[] words = line.Split(); //Separate by spaces

            int cumulativePosition = 0;
            EditorCaptionWord cw;

            foreach (String word in words)
            {
                if (word != "")
                {
                    cw = new EditorCaptionWord(word, cumulativePosition);
                    Add(cw);
                    cumulativePosition += cw.Length + SPACE_WIDTH;
                }
            }
        }

        /// <summary>
        /// Turns the list into a single String.
        /// </summary>
        /// <returns>A string containing all the CaptionWords in the list.</returns>
        public String GetAsString()
        {
            //Stringbuilder is faster than String when it comes to appending text.
            StringBuilder s = new StringBuilder();
            //For every element but the last
            for (int i = 0; i < Count - 1; i++)
            {
                s.Append(this[i].ToString());
                s.Append(" ");
            }
            //Append the last element without adding a space after it
            if (0 < Count)
                s.Append(this[Count - 1].ToString());

            return s.ToString();
        }
        #endregion

        #region Methods
 
        /// <summary>
        /// Returns the CaptionWordList represented as a String.
        /// </summary>
        /// <returns>The CaptionWordList represented as a String.</returns>
        public override string ToString()
        {
            return GetAsString();
        }
        #endregion
    }
}