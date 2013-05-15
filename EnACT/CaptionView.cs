﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Reflection;

namespace EnACT
{
    class CaptionView : DataGridView
    {
        #region Constants
        /// <summary>
        /// Number column position (0)
        /// </summary>
        public const int NPOS = 0;
        /// <summary>
        /// Begin time column position (1)
        /// </summary>
        public const int BPOS = 1;
        /// <summary>
        /// End time column position (2)
        /// </summary>
        public const int EPOS = 2;
        /// <summary>
        /// Speaker name column position (3)
        /// </summary>
        public const int SPOS = 3;
        /// <summary>
        /// Caption text column position (4)
        /// </summary>
        public const int TPOS = 4;

        /// <summary>
        /// Number column name
        /// </summary>
        public const String NNAME = "Number";
        /// <summary>
        /// Begin time column name
        /// </summary>
        public const String BNAME = "Begin";
        /// <summary>
        /// End time column name
        /// </summary>
        public const String ENAME = "End";
        /// <summary>
        /// Speaker-name column name
        /// </summary>
        public const String SNAME = "Speaker";
        /// <summary>
        /// Caption text column name
        /// </summary>
        public const String TNAME = "Text";
        #endregion

        /// <summary>
        /// A set of Speaker objects, each speaker being mapped to by its name
        /// </summary>
        public Dictionary<String, Speaker> SpeakerSet { set; get; }

        /// <summary>
        /// A list of captions retrieved from a transcript file.
        /// </summary>
        public List<Caption> CaptionList { private set; get; }

        /// <summary>
        /// A caption list that can automatically update the CaptionView. Use this object
        /// instead of CaptionList when coding in CaptionView
        /// </summary>
        private BindingList<Caption> BindingList { set; get; }

        DataGridViewColumn NumberColumn;
        DataGridViewColumn BeginColumn;
        DataGridViewColumn EndColumn;
        DataGridViewColumn SpeakerColumn;
        DataGridViewColumn TextColumn;

        /// <summary>
        /// Sets the CaptionView's CaptionList and initializes it for Caption View.
        /// Returns the Captionlist.
        /// </summary>
        public List<Caption> CaptionSource
        {
            set
            {
                CaptionList = value;
                BindingList = new BindingList<Caption>(value);

                DataSource = BindingList;   //Bind list to view
            }
            get { return CaptionList; }
        }
        
        /// <summary>
        /// Constructs a new CaptionView
        /// </summary>
        public CaptionView()
        {
            //Set the view to select a whole row when you click on a column
            SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            //Don't create columns for each property.
            AutoGenerateColumns = false;

            //Init Columns
            NumberColumn = new DataGridViewTextBoxColumn();
            NumberColumn.Name = NNAME;
            NumberColumn.HeaderText = NNAME;
            NumberColumn.ReadOnly = true;   //Set Number column to read only

            BeginColumn = new DataGridViewTextBoxColumn();
            BeginColumn.Name = BNAME;
            BeginColumn.HeaderText = BNAME;
            BeginColumn.ValueType = typeof(Timestamp);
            BeginColumn.DataPropertyName = BNAME;
            BeginColumn.CellTemplate = new CaptionViewTimestampCell();

            EndColumn = new DataGridViewTextBoxColumn();
            EndColumn.Name = ENAME;
            EndColumn.HeaderText = ENAME;
            EndColumn.ValueType = typeof(Timestamp);
            EndColumn.DataPropertyName = ENAME;
            EndColumn.CellTemplate = new CaptionViewTimestampCell();

            SpeakerColumn = new DataGridViewTextBoxColumn();
            SpeakerColumn.Name = SNAME;
            SpeakerColumn.HeaderText = SNAME;
            SpeakerColumn.DataPropertyName = SNAME;
            SpeakerColumn.CellTemplate = new CaptionViewSpeakerCell();

            TextColumn = new DataGridViewTextBoxColumn();
            TextColumn.Name = TNAME;
            TextColumn.HeaderText = TNAME;
            TextColumn.DataPropertyName = TNAME;

            //Add Columns to View
            Columns.Add(NumberColumn);
            Columns.Add(BeginColumn);
            Columns.Add(EndColumn);
            Columns.Add(SpeakerColumn);
            Columns.Add(TextColumn);

            //Set the last column to fill up remaining space
            Columns[Columns.Count-1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        /// <summary>
        /// Adds a row to the CaptionsList at the index of the currently selected row
        /// </summary>
        public void AddRow()
        {
            //If the user has a row selected
            if(CurrentRow!=null)
                BindingList.Insert(CurrentRow.Index, new Caption());
            //Else there is no current selection or the table is empty
            else
                BindingList.Insert(0, new Caption());
        }

        /// <summary>
        /// Deletes the rows currently selected by the user
        /// </summary>
        public void DeleteSelectedRows()
        {
            /* TODO: .NET 4.5 has a SortedSet<T> object. If we upgrade to 4.5, 
            * use that instead of a list. Unfortunately it is not available
            * in .NET 3.5 or older.
            */
            List<Caption> cList = new List<Caption>();

            //Add the index of each selected row to a list and remove them
            foreach (DataGridViewRow r in SelectedRows)
            {
                cList.Add(BindingList[r.Index]);
            }

            //Start from the bottom of the list
            cList.Reverse();

            //Remove each caption
            foreach (Caption c in cList)
            {
                BindingList.Remove(c);
            }

            //Deselect everything
            //CaptionView.ClearSelection();
        }

        /// <summary>
        /// Moves a row up by 1 position, provided it can be moved up
        /// </summary>
        public void MoveRowUp()
        {
            if (CurrentRow == null)
                return;

            int index = CurrentRow.Index;
            if (0 < index)
            {
                SwapRows(index, index - 1);

                //Change selected row the new position
                CurrentCell = this[0, index - 1];
            }
        }

        /// <summary>
        /// Moves a row down by 1 position, provided it can be moved down
        /// </summary>
        public void MoveRowDown()
        {
            if (CurrentRow == null)
                return;

            int index = CurrentRow.Index;
            if (index < BindingList.Count - 1)
            {
                SwapRows(index, index + 1);

                //Change selected row the new position
                CurrentCell = this[0, index + 1];
            }
        }

        /// <summary>
        /// Swaps two rows with each other in BindingList. Note that this method does not check
        /// that the two indices are within the bounds of BindingList
        /// </summary>
        /// <param name="index1">The first index to swap</param>
        /// <param name="index2">The second index to swap</param>
        private void SwapRows(int index1, int index2)
        {
            Caption temp = BindingList[index1];
            BindingList[index1] = BindingList[index2];
            BindingList[index2] = temp;
        }

        /// <summary>
        /// Updates this CaptionView so that it displays the current list of items. This method
        /// should be called anytime CaptionList is updated outside of CaptionView.
        /// </summary>
        public void UpdateView()
        {
            BindingList.ResetBindings();
        }

        private void InitializeComponent()
        {
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // CaptionView
            // 
            this.CellParsing += new System.Windows.Forms.DataGridViewCellParsingEventHandler(this.CaptionView_CellParsing);
            this.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.CaptionView_CellFormatting);
            this.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.CaptionView_CellValidating);
            this.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.CaptionView_CellValuePushed);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        public void ValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //int Row = e.RowIndex;
            //int Column = e.ColumnIndex;
            //Caption c = BindingList[Row];
            //switch (Column)
            //{
            //    //Nothing should be done for Number
            //    case NPOS: break;
            //    //Set Begin value
            //    case BPOS:
            //        try { c.Begin = (String)Rows[Row].Cells[Column].Value; } //Attempt to set it
            //        catch (InvalidTimestampException)
            //        {
            //            Rows[Row].Cells[Column].Value = c.Begin; //Reset if invalid
            //        }
            //        break;
            //    //Set End value
            //    case EPOS:
            //        try { c.End = (String)Rows[Row].Cells[Column].Value; } //Attempt to set it
            //        catch (InvalidTimestampException)
            //        {
            //            Rows[Row].Cells[Column].Value = c.End; //Reset if invalid
            //        }
            //        break;
            //    //Change speakers
            //    case SPOS:
            //        //ModifySpeaker(Row);
            //        break;
            //    //Create a new WordList
            //    case TPOS:
            //        c.FeedWordList((String)Rows[Row].Cells[Column].Value);
            //        break;
            //    default:
            //        Console.WriteLine("No case found: {0}", Column);
            //        break;
            //}
        }

        private void CaptionView_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            switch (e.ColumnIndex)
            {
                case BPOS:
                case EPOS: break;
                case SPOS: 
                    Speaker s = new Speaker(e.Value.ToString());
                    SpeakerSet[s.Name] = s;
                    e.Value = s;
                break;
                default: break;
            }
            e.ParsingApplied = true;
        }

        private void CaptionView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

        }

        private void CaptionView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {

        }

        private void CaptionView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
        }
    }

    class CaptionViewTimestampCell : DataGridViewTextBoxCell
    {
        public CaptionViewTimestampCell() : base() { }

        public override object ParseFormattedValue(object formattedValue,
            DataGridViewCellStyle cellStyle, TypeConverter formattedValueTypeConverter,
            TypeConverter valueTypeConverter)
        {
            //return new Timestamp((String)formattedValue);
            Timestamp t;
            try
            {
                t = new Timestamp((String)formattedValue);
            }
            catch(InvalidTimestampException)
            {
                t = (Timestamp)Value;
            }
            return t;   
        }
    }

    class CaptionViewSpeakerCell : DataGridViewTextBoxCell
    {
        public CaptionViewSpeakerCell() : base() {}

        public override object  ParseFormattedValue(object formattedValue, 
            DataGridViewCellStyle cellStyle, TypeConverter formattedValueTypeConverter, 
            TypeConverter valueTypeConverter)
        {
            return new Speaker(formattedValue.ToString());//base.ParseFormattedValue(formattedValue, cellStyle, formattedValueTypeConverter, valueTypeConverter);
        }
    }

    //class TimestampTypeConverter : TypeConverter
    //{
    //    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    //    {
    //        if (sourceType is string)
    //            return true;
    //        else
    //            return base.CanConvertFrom(context, sourceType);
    //    }

    //    public override object ConvertFrom(ITypeDescriptorContext context, 
    //        System.Globalization.CultureInfo culture, object value)
    //    {
    //        if (value == null)
    //            return new Timestamp();

    //        if (value is String)
    //        {
    //            String vstring = value as String;
    //            return new Timestamp(vstring);
    //        }
    //        else
    //            return base.ConvertFrom(context, culture, value);
    //    }

    //    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    //    {
    //        if ((destinationType == typeof(string)) |
    //            (destinationType == typeof(InstanceDescriptor)))
    //            return true;
    //        else
    //        return base.CanConvertTo(context, destinationType);
    //    }

    //    public override object ConvertTo(ITypeDescriptorContext context, 
    //        System.Globalization.CultureInfo culture, object value, Type destinationType)
    //    {
    //        if (value != null && !(value is Timestamp))
    //            throw new Exception("Value is of wrong type");

    //        if (destinationType == typeof(String))
    //        {
    //            Timestamp t = value as Timestamp;
    //            return t.AsString;
    //        }

    //        if (destinationType == typeof(InstanceDescriptor))
    //        {
    //            if (value == null)
    //                return null;

    //            Timestamp t = value as Timestamp;

    //            MemberInfo mi = typeof(Timestamp).GetConstructor(new Type [] {typeof(String)} );
    //            object[] args = new object[] {t.AsString};

    //            if (mi != null)
    //                return new InstanceDescriptor(mi, args);
    //            else
    //                return null;
    //        }
    //        return base.ConvertTo(context, culture, value, destinationType);
    //    }
    //}
}
