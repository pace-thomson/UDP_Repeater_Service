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
// Version   Date          Author            Description
//   1.0    7/25/24    Jade Pace Thomson   Initial Release
//---------------------------------------------------


using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;



namespace UDP_Repeater_GUI
{
    /// <summary>
    /// The class for the form that shows the logs. It also has some
    /// input fields for changing the inactivity period. This form comes 
    /// up when the "System Log" button is clicked on the main form.
    /// </summary>
    public partial class LogForm : Form
    {
            /// <summary> The main form's object. </summary>
        private MainForm theMainForm;
            /// <summary> Our own eventLog object to watch and read from. </summary>
        public EventLog eventLogForReading;


        /// <summary> 
        ///  Class Name: LogForm  <br/><br/>
        ///
        ///  Description: Constructs the form. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  MainForm <paramref name="mainForm"/> - The main form's object. Passed in so that 
        ///  we can update it's "Current Configuration" section whenever a change is made.<br/><br/>
        ///  
        ///  Returns: A LogForm object.
        /// </summary>
        public LogForm(MainForm mainForm)
        {
            InitializeComponent();

            eventLogForReading = SetCheckerForLogChanges();

            theMainForm = mainForm;

                // this makes sure that the rows can fit if there's multiple lines of text
            logDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

                // hides the logs until the they're finished loading
            logDataGridView.Visible = false;
        }

        /// <summary> 
        ///  Class Name: LogForm  <br/><br/>
        ///
        ///  Description: Reads from the Windows Event Log "UDP Packet Repeater" (where we store logs locally), and <br/>
        ///               then populates the logDataGridView with all of the logs, sorted by newest first. <br/><br/>
        ///
        ///  Inputs:  None <br/><br/>
        ///  
        ///  Returns: None
        /// </summary>
        public void PopulateTable()
        {
            try
            {
                foreach (EventLogEntry entry in eventLogForReading.Entries)
                {
                    AddNewRow(entry);
                }

                    // sort by newest entry first
                logDataGridView.Sort(logDataGridView.Columns["timeStampColumn"], ListSortDirection.Descending);
            }
            catch (Exception e)
            {
                theMainForm.logger.LogException(e);
            }
        }

        /// <summary> 
        ///  Class Name: LogForm  <br/><br/>
        ///
        ///  Description: Handles the Send button for the reconfigure inactivity section. It <br/>
        ///               checks the input fields to see if their values are valid, and then sends them <br/>
        ///               if they are. If not, it opens a messagebox telling the user to try again. <br/><br/>
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
                                // so that we can use it to display 
                        theMainForm.UpdateCurrentConfigGroup(frequency.ToString(), interval);
                        theMainForm.logger.LogInactivityChange(frequency, interval);

                        interval = interval.ToLower();

                                // interval shoudn't be uppcase here because the backend wants it in lowercase
                        byte[] bytes = Encoding.ASCII.GetBytes(frequency + "," + interval + "," + "inactive");
                        sendRequest.Send(bytes, bytes.Length, "127.0.0.1", 50001);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show($"Error sending data: {exception.Message}");
                        theMainForm.logger.LogException(exception);
                        return;
                    }
                            // this section resets the inputs
                    inactivityInputBox.Value = 1;
                    inactivityDropdown.SelectedIndex = -1;    // deselects in the drop down
                    sendRequest.Close();
                }
            }
            else
            {
                MessageBox.Show("Invalid Entries. Please try again.");
            }
        }

        /// <summary> 
        ///  Class Name: LogForm  <br/><br/>
        ///
        ///  Description: Sets up events for the event log getting new entries. <br/><br/>
        ///
        ///  Inputs: None <br/><br/>
        ///  </summary>
        ///  <returns> EventLog - This form's special eventLogForReading object for watching for log entries.</returns>
        public EventLog SetCheckerForLogChanges()
        {
            try
            {
                EventLog eventLog = new EventLog("UDP Packet Repeater");
                eventLog.Source = "UDP_Repeater_Frontend";

                eventLog.EntryWritten += new EntryWrittenEventHandler(OnEntryWritten);
                eventLog.EnableRaisingEvents = true;
                return eventLog;
            }
            catch ( Exception e)
            {
                theMainForm.logger.LogException(e);
                return null;
            }
        }

        /// <summary> 
        ///  Class Name: LogForm  <br/><br/>
        ///
        ///  Description: Gets called whenever a new entry is writen to the event log, which fires <br/>
        ///  and calls this function to add the new entry to our data grid view <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  object <paramref name="source"/> - Necessary for handling the button, but I don't use it. <br/>
        ///  EntryWrittenEventArgs <paramref name="e"/> - Necessary for handling the button, but I don't use it. <br/><br/>
        ///  </summary>
        ///  
        ///  <returns> None </returns> 
        public void OnEntryWritten(object source, EntryWrittenEventArgs e)
        {
            Invoke(new Action(() => AddNewRow(e.Entry)));
            Invoke(new Action(() => logDataGridView.Sort(logDataGridView.Columns["timeStampColumn"], ListSortDirection.Descending)));
        }

        /// <summary> 
        ///  Class Name: LogForm  <br/><br/>
        ///
        ///  Description: Gets called wherever we add new entries to the data grid view. <br/>
        ///  Uses a switch case to see what kind of entry it is, according to our id scheme. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  EventLogEntry <paramref name="entry"/> - The log entry to add to our data grid view. <br/><br/>
        ///  </summary>
        ///  
        ///  <returns> None </returns>
        public void AddNewRow(EventLogEntry entry)
        {
            int rowNum = logDataGridView.Rows.Add();
            DataGridViewRow row = logDataGridView.Rows[rowNum];

            switch (entry.EventID)
            {
                case 1:
                    row.Cells["entryTypeColumn"].Value = "Error/Exception";
                    row.Cells["entryTypeColumn"].Style.BackColor = Color.Crimson;
                    row.Cells["logOriginColumn"].Value = "Service";
                    break;
                case 2:
                    row.Cells["entryTypeColumn"].Value = "Error/Exception";
                    row.Cells["entryTypeColumn"].Style.BackColor = Color.Crimson;
                    row.Cells["logOriginColumn"].Value = "Interface";
                    break;
                case 3:
                    row.Cells["entryTypeColumn"].Value = "Inactive Period";
                    row.Cells["logOriginColumn"].Value = "General";
                    break;
                case 4:
                    row.Cells["entryTypeColumn"].Value = "Start/Stop";
                    row.Cells["logOriginColumn"].Value = "Service";
                    break;
                case 5:
                    row.Cells["entryTypeColumn"].Value = "Start/Stop";
                    row.Cells["logOriginColumn"].Value = "Interface";
                    break;
                case 6:
                    row.Cells["entryTypeColumn"].Value = "IP/Port Change";
                    row.Cells["logOriginColumn"].Value = "General";
                    break;
                case 7:
                    row.Cells["entryTypeColumn"].Value = "Inactivity Change";
                    row.Cells["logOriginColumn"].Value = "General";
                    break;
                case 8:
                    row.Cells["entryTypeColumn"].Value = "NIC Change";
                    row.Cells["logOriginColumn"].Value = "General";
                    break;
                case 9:
                    row.Cells["entryTypeColumn"].Value = "Warning";
                    row.Cells["entryTypeColumn"].Style.BackColor = Color.Yellow;
                    row.Cells["logOriginColumn"].Value = "Service";
                    break;
                case 10:
                    row.Cells["entryTypeColumn"].Value = "Monitoring Change";
                    row.Cells["logOriginColumn"].Value = "General";
                    break;
                case 11:
                    row.Cells["entryTypeColumn"].Value = "Warning";
                    row.Cells["entryTypeColumn"].Style.BackColor = Color.Yellow;
                    row.Cells["logOriginColumn"].Value = "Interface";
                    break;
            }
            row.Cells["messageColumn"].Value = entry.Message;
            row.Cells["timeStampColumn"].Value = entry.TimeWritten;
        }


        /// <summary> 
        ///  Class Name: LogForm  <br/><br/>
        ///
        ///  Description: Finishes the controls rendering and then populates the table while the <br/>
        ///  loading label is shown. When done, it then hides the loading label and shows the table. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  object <paramref name="sender"/> - Necessary for handling the button, but I don't use it. <br/>
        ///  EventArgs <paramref name="e"/> - Necessary for handling the button, but I don't use it. <br/><br/>
        ///  </summary>
        ///  
        ///  <returns> None </returns>
        private void LogForm_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();
            PopulateTable();
            loadingLabel.Visible = false;
            logDataGridView.Visible = true;
        }

        /// <summary> 
        ///  Class Name: LogForm  <br/><br/>
        ///
        ///  Description: if eventLogForReading isn't null, disposes of it. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  object <paramref name="sender"/> - Necessary for handling the button, but I don't use it. <br/>
        ///  EventArgs <paramref name="e"/> - Necessary for handling the button, but I don't use it. <br/><br/>
        ///  </summary>
        ///  
        ///  <returns> None </returns>
        private void LogForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (eventLogForReading != null)
            {
                eventLogForReading.Dispose();
            }
        }
    }
}
