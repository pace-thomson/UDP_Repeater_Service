﻿//----------------------------------------------------
// File Name: MainForm.cs
// 
// Description: This file handles the main form of the
//              GUI. This is the landing page of the 
//              front end. It displays the packets sent
//              log and buttons that open the other two
//              forms.
//
// Language:         Visual C#
// Target:           Windows PC
// Operating System: Windows 11 Enterprise
// Compiler:         Visual Studio .Net 2022
//
// Change History:
//
// Version  Date        Author           Description
// 1.0      ---     Jade Pace Thomson    Initial Release
//---------------------------------------------------


using System;
using System.ComponentModel;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json.Linq;


namespace UDP_Test_GUI
{
    public partial class gui_form : Form
    {
            /// <summary> The UDP object for receiving </summary>
        private UdpClient udpClient;
            /// <summary> How it stays listening and updating without recursion </summary>
        private bool isListening = false;
            /// <summary> Tracks what number packet is being received </summary> 
        private int index = 0;
            /// <summary> Puts the icon in the system tray </summary>
        private NotifyIcon notifyIcon1;

        /// <summary> 
        ///  Class Name: gui_form  <br/><br/>
        ///
        ///  Description: Constructs the main form. <br/><br/>
        ///
        ///  Inputs: None <br/><br/>
        ///  
        ///  Returns: A gui_form object.
        /// </summary>
        public gui_form()
        {
            InitializeComponent();
                // starts the listening
            InitializeUDPListener();


            notifyIcon1 = new NotifyIcon();

                // The Icon property sets the icon that will appear
                // in the systray for this application.
            notifyIcon1.Icon = new Icon("jt4-logo.ico");

                // The Text property sets the text that will be displayed,
                // in a tooltip, when the mouse hovers over the systray icon.
            notifyIcon1.Text = "UDP Packet Repeater";
            notifyIcon1.Visible = true;

                // Handle the Click event to activate the form.
            notifyIcon1.Click += new EventHandler(notifyIcon1_Click);

                // gui starts minimized but running
            WindowState = FormWindowState.Minimized;
                
            UpdateCurrentConfigGroup();
        }

        /// <summary> 
        ///  Class Name: gui_form  <br/><br/>
        ///
        ///  Description: Overloaded version 1/3. Updates the current configuration group. <br/>
        ///  This one is used for the initial setting of the values in the group. <br/><br/>
        ///
        ///  Inputs: None <br/><br/>
        ///  
        ///  Returns: None
        /// </summary>
        public void UpdateCurrentConfigGroup()
        {
            string jsonString = File.ReadAllText("C:\\Windows\\SysWOW64\\UDP_Repeater_Config.json");
            JObject jsonObject = JObject.Parse(jsonString);

            currentReceiveIp.Text    =  (string)jsonObject["currentConfig"]["receiveFrom"]["ip"];
            currentReceivePort.Text  =  (string)jsonObject["currentConfig"]["receiveFrom"]["port"];
            currentSendIp.Text       =  (string)jsonObject["currentConfig"]["sendTo"]["ip"];
            currentSendPort.Text     =  (string)jsonObject["currentConfig"]["sendTo"]["port"];
            currentFrequency.Text    =  (string)jsonObject["inactivitySettings"]["frequency"];
            currentInterval.Text     =  (string)jsonObject["inactivitySettings"]["interval"];
        }

        /// <summary> 
        ///  Class Name: gui_form  <br/><br/>
        ///
        ///  Description: Overloaded version 2/3. Updates the current configuration group. <br/>
        ///  This one updates the ip/port values of the group. It's updated this way so <br/>
        ///  this doesn't have to wait for the backend to update "UDP_Repeater_Config.json". <br/>
        ///  This function gets called right after the user inputs a new setting. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  string <paramref name="mode"/> - What profile was changed. <br/>
        ///  string <paramref name="ip"/> - The new IP Address. <br/>
        ///  string <paramref name="port"/> - The new Port. <br/><br/>
        ///  
        ///  Returns: None
        /// </summary>
        public void UpdateCurrentConfigGroup(string mode, string ip, string port)
        {
            if (mode == "Receive")
            {
                currentReceiveIp.Text = ip;
                currentReceivePort.Text = port;
            }
            else if (mode == "Send")
            {
                currentSendIp.Text = ip;
                currentSendPort.Text = port;
            }
        }

        /// <summary> 
        ///  Class Name: gui_form  <br/><br/>
        ///
        ///  Description: Overloaded version 3/3. Updates the current configuration group. <br/>
        ///  This one updates the frequency and interval values of the group. It's updated this <br/>
        ///  way so this doesn't have to wait for the backend to update "UDP_Repeater_Config.json". <br/>
        ///  This function gets called right after the user inputs a new setting. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  string <paramref name="frequency"/> - The new frequency value. <br/>
        ///  string <paramref name="interval"/> - The new interval value. <br/><br/>
        ///  
        ///  Returns: None
        /// </summary>
        public void UpdateCurrentConfigGroup(string frequency, string interval)
        {
            if (frequency == "1") 
            {
                interval += "s";
            }
            currentFrequency.Text = frequency;
            currentInterval.Text = interval;
        }


        /// <summary> 
        ///  Class Name: gui_form  <br/><br/>
        ///
        ///  Description: Intializes the UDP Client listening and holds the infinite listening loop. <br/><br/>
        ///
        ///  Inputs: None <br/><br/>
        ///  
        ///  Returns: None
        /// </summary>
        private async void InitializeUDPListener()
        {
            if (!isListening)
            {
                isListening = true;
                udpClient = new UdpClient(50000);
                await ReceiveDataAsync();
            }
        }

        /// <summary> 
        ///  Class Name: gui_form  <br/><br/>
        ///
        ///  Description: This listens until a packet is received. <br/><br/>
        ///
        ///  Inputs: None <br/><br/>
        ///  
        ///  Returns: Task
        /// </summary>
        private async Task ReceiveDataAsync()
        {
            while (isListening)
            {
                try
                {
                    UdpReceiveResult result = await udpClient.ReceiveAsync();
                    ProcessReceivedData(result.Buffer);
                }
                catch (ObjectDisposedException)
                {
                        // Handle if the UDP client is disposed
                    isListening = false;
                }
                catch (Exception ex)
                {
                        // Handle other exceptions
                    MessageBox.Show($"Error receiving data: {ex.Message}");
                    Logger.LogException(ex);
                }
            }
        }

        /// <summary> 
        ///  Class Name: gui_form  <br/><br/>
        ///
        ///  Description: This processes the packet sent from the backend and then <br/>
        ///  adds a new row in the data grid view with the proccessed data. <br/><br/>
        ///
        ///  Inputs: <br/>
        ///  byte[] <paramref name="receivedBytes"/> - The bytes received from the backend.<br/><br/>
        ///  
        ///  Returns: None
        /// </summary>
        private void ProcessReceivedData(byte[] receivedBytes)
        {
            string receivedData = Encoding.ASCII.GetString(receivedBytes);
            string[] dataParts = receivedData.Split(',');
            
                // Updates GUI from a different thread
            dataGridView1.Invoke((MethodInvoker)delegate {

                DataGridViewRow row;

                if (dataGridView1.Rows.Count >= 250)
                {
                            // this caps the table at most recent 250 packets   
                    dataGridView1.Rows.Add();
                    dataGridView1.Rows.RemoveAt(250);
                    row = dataGridView1.Rows[249];
                } 
                else
                {
                    int rowNum = dataGridView1.Rows.Add();
                    row = dataGridView1.Rows[rowNum];
                }

                // Updates DataGridView with received data

                row.Cells["indexColumn"].Value = (index+1);
                row.Cells["ipColumn"].Value = dataParts[0];
                row.Cells["portColumn"].Value = dataParts[1];
                row.Cells["payloadColumn"].Value = dataParts[2];
                row.Cells["timeColumn"].Value = DateTime.Now.ToString();
                index++;

            });
                // changes sort direction to keep the most recent packet at the top.
                // MAKE SURE IT'S SORTING INTEGERS AND NOT STRINGS.
            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Descending);

                // updates packet counter
            packet_counter.Text = index.ToString();
        }


        /// <summary> 
        ///  Class Name: gui_form  <br/><br/>
        ///
        ///  Description: Opens a messagebox asking if the user is sure they want to close the GUI. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  FormClosingEventArgs <paramref name="e"/> - The form closing event arg. <br/><br/>
        ///  
        ///  Returns: None
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to close?",
                                "UDP Packet Repeater",
                                 MessageBoxButtons.YesNo,
                                 MessageBoxIcon.Information) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        /// <summary> 
        ///  Class Name: gui_form  <br/><br/>
        ///
        ///  Description: Handles clicking of the system tray icon. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  object <paramref name="Sender"/> - Whoever sent this i think? <br/>
        ///  FormClosingEventArgs <paramref name="e"/> - The form closing event arg. <br/><br/>
        ///  
        ///  Returns: None
        /// </summary>
        private void notifyIcon1_Click(object Sender, EventArgs e)
        {
            // Show the form when the user clicks on the notify icon.

            // Set the WindowState to normal if the form is minimized.
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
                this.Focus();

            }

            else if (WindowState == FormWindowState.Normal)
            {
                if (!this.Focused)
                {
                    this.Focus();
                }
                else if (this.Focused)
                {
                    WindowState = FormWindowState.Minimized;
                }
            }
                
            // Activate the form
            Activate();
        }


        /// <summary> 
        ///  Class Name: gui_form  <br/><br/>
        ///
        ///  Description: Opens the IP/Port reconfiguration form. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  object <paramref name="Sender"/> - I don't use this, i'm not sure. <br/>
        ///  EventArgs <paramref name="e"/> - The event arg. <br/><br/>
        ///  
        ///  Returns: None
        /// </summary>
        private void showDialogbutton_Click(object sender, EventArgs e)
        {
            configDialog messageDialog = new configDialog(this);
            var response = messageDialog.ShowDialog();
        }

        /// <summary> 
        ///  Class Name: gui_form  <br/><br/>
        ///
        ///  Description: Opens the Log form. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  object <paramref name="Sender"/> - I don't use this, i'm not sure. <br/>
        ///  EventArgs <paramref name="e"/> - The event arg. <br/><br/>
        ///  
        ///  Returns: None
        /// </summary>
        private void logButton_Click(object sender, EventArgs e)
        {
            LogForm logForm = new LogForm(this);
            logForm.ShowDialog();
        }
    }
}