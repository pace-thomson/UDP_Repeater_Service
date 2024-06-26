//----------------------------------------------------
// File Name: InputForm.cs
// 
// Description: This file handles the logging form. It
//              also containts inputs for reconfiguring 
//              the inactivity settings.
//
// Language:         Visual C#
// Target:           Windows PC
// Operating System: Windows 11 Enterprise
// Compiler:         Visual Studio .Net 2022
//
// Change History:
//
// Version   Date         Author            Description
//   1.0    6/21/24   Jade Pace Thomson   Initial Release
//---------------------------------------------------


using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using System.Linq;


namespace UDP_Repeater_GUI
{
    /// <summary>
    /// The class for the form that shows the logs. It also has some
    /// input fields for changing the inactivity period. This form comes 
    /// up when the "System Log" button is clicked on the main form.
    /// </summary>
    public partial class LogForm : Form
    {
            /// <summary>The main form's object.</summary>
        private gui_form theMainForm;

        /// <summary> 
        ///  Class Name: LogForm  <br/><br/>
        ///
        ///  Description: Constructs the form. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  gui_form <paramref name="mainForm"/> - The main form's object. Passed in so that 
        ///  we can update it's "Current Configuration" section whenever a change is made.<br/><br/>
        ///  
        ///  Returns: A LogForm object.
        /// </summary>
        public LogForm(gui_form mainForm)
        {
            InitializeComponent();
            PopulateTable();
            theMainForm = mainForm;
                    // this makes sure that the rows can fit if there's multiple lines of text
            reconfigLog.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            SetCheckerForLogChanges();
        }

        /// <summary> 
        ///  Class Name: configDialog  <br/><br/>
        ///
        ///  Description: Reads from the Windows log "UDP Packet Repeater" (where the backend stores it's logs) and from  <br/>
        ///  "Repeater_GUI_Log.txt" (where the GUI stores it's logs), and then puts that data into a table on this form. <br/>
        ///  There is also a section with inputs for reconfiguring the inactivity frequency and interval. <br/><br/>
        ///
        ///  Inputs:  None <br/><br/>
        ///  
        ///  Returns: None
        /// </summary>
        public void PopulateTable()
        {
            // section for reading and inputting entries from "Repeater_GUI_Log.txt"
            var lines = File.ReadLines("Repeater_GUI_Log.txt");
            foreach (string line in lines)
            {
                string[] splitLine = line.Split(',');

                int rowNum = reconfigLog.Rows.Add();
                DataGridViewRow row = reconfigLog.Rows[rowNum];

                row.Cells["entryType"].Value = splitLine[0];
                row.Cells["messageColumn"].Value = splitLine[1];   
                row.Cells["timeStampColumn"].Value = DateTime.Parse(splitLine[2]);
            }

            // section for reading and inputting entries from "UDP Packet Repeater"
            EventLog eventLog = new EventLog("UDP Packet Repeater");
            foreach (EventLogEntry entry in eventLog.Entries)
            {
                int rowNum = reconfigLog.Rows.Add();
                DataGridViewRow row = reconfigLog.Rows[rowNum];

                if (entry.EntryType == EventLogEntryType.Warning)
                {
                    row.Cells["entryType"].Value = "Inactive Period";
                }
                else if (entry.EntryType == EventLogEntryType.Error)
                {
                    row.Cells["entryType"].Value = "Repeater Error";
                }

                row.Cells["messageColumn"].Value = entry.Message;
                row.Cells["timeStampColumn"].Value = entry.TimeWritten;
            }

                    // and finally, we sort by newest entry first
            reconfigLog.Sort(reconfigLog.Columns[2], ListSortDirection.Descending);
        }


        /// <summary> 
        ///  Class Name: configDialog  <br/><br/>
        ///
        ///  Description: Handles the Send button for the reconfigure inactivity section. It <br/>
        ///  checks the input fields to see if their values are valid, and then sends them <br/>
        ///  if they are. If not, it opens a messagebox telling the user to try again. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  object <paramref name="sender"/> - Necessary for handling the button, but I don't use it. <br/>
        ///  EventArgs <paramref name="e"/> - Necessary for handling the button, but I don't use it. <br/><br/>
        ///  
        ///  Returns: None
        /// </summary>
        private void inactivityButton_Click(object sender, EventArgs e)
        {
            int frequency = (int)inactivityInputBox.Value;
                    // trims the "(s)" off the end of the string
            string interval = inactivityDropdown.Text.TrimEnd('(', 's', ')'); 
            
            if (frequency > 0 && interval != "")
            {
                using (UdpClient sendRequest = new UdpClient())
                {
                    try
                    {
                                // we want interval to still have the first letter uppcase here
                                // so that we can use it to display prettily? idk if that's a word
                        theMainForm.UpdateCurrentConfigGroup(frequency.ToString(), interval);
                        Logger.LogInactivityChange(frequency, interval);

                        interval = interval.ToLower();

                                // we don't want interval to be uppcase here
                        byte[] bytes = Encoding.ASCII.GetBytes(frequency + "," + interval + "," + "inactive");
                        sendRequest.Send(bytes, bytes.Length, "127.0.0.1", 50001);

                                // updates table since we just made a new log entry             not needed now?
                        // AddNewRow();
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show($"Error sending data: {exception.Message}");
                        Logger.LogException(exception);
                        return;
                    }
                            // this section resets the inputs
                    inactivityInputBox.Value = 1;
                    inactivityDropdown.SelectedIndex = -1;
                    sendRequest.Close();
                }
            }
            else
            {
                MessageBox.Show("Invalid Entries. Please try again.");
            }
        }

        /// <summary> 
        ///  Class Name: configDialog  <br/><br/>
        ///
        ///  Description: Sets up events for the backend/frontend logs changing. Updates table for both events. <br/><br/>
        ///
        ///  Inputs: None <br/><br/>
        ///  
        ///  Returns: None
        /// </summary>
        public void SetCheckerForLogChanges()
        {
            EventLog eventLog = new EventLog();
            eventLog.Log = "UDP Packet Repeater";

            eventLog.EntryWritten += new EntryWrittenEventHandler(OnEntryWritten);
            eventLog.EnableRaisingEvents = true;

            var watcher = new FileSystemWatcher("C:\\Program Files (x86)\\UDP_Repeater_Service");
            watcher.Changed += new FileSystemEventHandler(OnTextFileChanged);
            watcher.EnableRaisingEvents = true;
            watcher.Filter = "*.txt";
        }

        /// <summary> When an entry to the backend log is written, repopulates table. </summary>
        public void OnEntryWritten(object source, EntryWrittenEventArgs e)
        {
            Invoke(new Action(() => AddNewRow(e.Entry)));
        }
        /// <summary> When an entry to the frontend log is written, repopulates table. </summary>
        public void OnTextFileChanged(object sender, FileSystemEventArgs e)
        {
            Invoke(new Action(() => AddNewRow()));
        }


        public void AddNewRow(EventLogEntry entry)
        {
            int rowNum = reconfigLog.Rows.Add();
            DataGridViewRow row = reconfigLog.Rows[rowNum];

            if (entry.EntryType == EventLogEntryType.Warning)
            {
                row.Cells["entryType"].Value = "Inactive Period";
            }
            else if (entry.EntryType == EventLogEntryType.Error)
            {
                row.Cells["entryType"].Value = "Repeater Error";
            }
            else 
            {
                row.Cells["entryType"].Value = "Service Start/Stop";
            }

            row.Cells["messageColumn"].Value = entry.Message;
            row.Cells["timeStampColumn"].Value = entry.TimeWritten;

            reconfigLog.Sort(reconfigLog.Columns[2], ListSortDirection.Descending);
        }

        public void AddNewRow()
        {
            string lastLine = File.ReadLines("Repeater_GUI_Log.txt").Last();

            string[] splitLine = lastLine.Split(',');

            int rowNum = reconfigLog.Rows.Add();
            DataGridViewRow row = reconfigLog.Rows[rowNum];

            row.Cells["entryType"].Value = splitLine[0];
            row.Cells["messageColumn"].Value = splitLine[1];
            row.Cells["timeStampColumn"].Value = DateTime.Parse(splitLine[2]);

            reconfigLog.Sort(reconfigLog.Columns[2], ListSortDirection.Descending);
        }
    }
}
