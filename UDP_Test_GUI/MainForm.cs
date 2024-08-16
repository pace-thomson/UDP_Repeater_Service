//----------------------------------------------------
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
// Version   Date          Author            Description
//   1.0    8/3/24    Jade Pace Thomson     Initial Release
//---------------------------------------------------


using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UDP_Test_GUI;


namespace UDP_Repeater_GUI
{
    public partial class MainForm : Form
    {
            /// <summary> The UDP object for receiving </summary>
        private UdpClient udpClient;
            /// <summary> How it stays listening and updating without recursion </summary>
        private bool isListening;
            /// <summary> Tracks what number packet is being received </summary> 
        private UInt64 totalPacketCounter;
            /// <summary> Puts the icon in the system tray </summary>
        private NotifyIcon sysTrayIcon;
            /// <summary> Our object for logging. </summary>
        public Logger logger;
            /// <summary> Our timer object so we can release it when the form closes. </summary>
        private Timer timer;
            /// <summary> How we monitor our service for its running status. </summary>
        public ServiceController ourServiceMonitor;

        /// <summary> 
        ///  Class Name: MainForm  <br/><br/>
        ///
        ///  Description: Constructs the main form. <br/><br/>
        ///
        ///  Inputs: None <br/><br/>
        ///  
        ///  Returns: MainForm - A MainForm object.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

                // double unhandled exception handling, just in case
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(ThreadExceptionHandlerFunction);

            logger = new Logger();
            try
            {
                timer = SetupTimerForServiceStatus();
                logger.StartStopLogger("start");
                ourServiceMonitor = new ServiceController("UDP_Repeater_Service");
                totalPacketCounter = 0;
                isListening = false;
                
                GetUserSetupChoices();          // this happens in the constructor, so this form will appear first if it's needed

                InitializeUDPListener();        // starts us continually listening

                HandleSysTrayIcon();            // sets up system tray icon 

                UpdateCurrentConfigGroup();     // populates the current config group with the settings in config.json
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
            
        }

        /// <summary> 
        ///  Class Name: MainForm  <br/><br/>
        ///
        ///  Description: Sets up the sys tray icon and it's behavior. <br/><br/>
        ///
        ///  Inputs: None <br/><br/>
        ///  
        ///  Returns: None
        /// </summary>
        private void HandleSysTrayIcon()
        {
            sysTrayIcon = new NotifyIcon();

                // The Icon property sets the icon that will appear
                // in the systray for this application.
            sysTrayIcon.Icon = new Icon("jt4_logo.ico");

                // The Text property sets the text that will be displayed,
                // in a tooltip, when the mouse hovers over the systray icon.
            sysTrayIcon.Text = "UDP Packet Repeater";
            sysTrayIcon.Visible = true;

                // Handle the Click event to activate the form.
            sysTrayIcon.Click += new EventHandler(notifyIcon1_Click);
        }
        /// <summary> 
        ///  Class Name: MainForm  <br/><br/>
        ///
        ///  Description: Handles clicking of the system tray icon for the gui. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  object <paramref name="Sender"/> - Whoever sent this, I think? <br/>
        ///  EventArgs <paramref name="e"/> - The icon click event arg. <br/><br/>
        ///  
        ///  Returns: None
        /// </summary>
        private void notifyIcon1_Click(object Sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
                this.BringToFront();
                this.Focus();
            }
            else
            {
                WindowState = FormWindowState.Minimized;
            }
        }

        /// <summary> 
        ///  Class Name: MainForm  <br/><br/>
        ///
        ///  Description: Checks if UDP_Repeater_Config.json is less than 30 <br/>
        ///  seconds old, and displays the setup form if it isn't. <br/><br/>
        ///
        ///  Inputs: None <br/><br/>
        ///  
        ///  Returns: None
        /// </summary>
        public void GetUserSetupChoices()
        {
            bool configJsonExists = logger.WaitForConfigJson();
            if (!configJsonExists)
            {
                return;
            }
                
                // if the file was just created, it means that it doesn't have "Setup" values and we just installed
            double creationDiff = (DateTime.Now - File.GetCreationTime("C:\\Windows\\SysWOW64\\UDP_Repeater_Config.json")).TotalSeconds;
            if (creationDiff > 10)
            {
                return;
            }

            using (Setup setupForm = new Setup(this))
            {
                DialogResult result = setupForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                        // This has to restart because reconfiguring the lokiLogger doesn't really work
                    Application.Restart();
                    Environment.Exit(0);
                }
            }
        }
        

        /// <summary> 
        ///  Class Name: MainForm  <br/><br/>
        ///
        ///  Description: Overloaded version 1/4. Updates the current configuration group. <br/>
        ///  This one is called in the Mainform constructor. <br/><br/>
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
            currentInterval.Text     =  (string)jsonObject["inactivitySettings"]["inactivityInterval"];
            currentTimeUnit.Text     =  FirstLetterCapital((string)jsonObject["inactivitySettings"]["inactivityUnit"]);

            if (currentInterval.Text != "1") currentTimeUnit.Text += "s";
            if (currentReceiveIp.Text == "0.0.0.0") currentReceiveIp.Text = "Any  -  (0.0.0.0)";
        }
        /// <summary> 
        ///  Class Name: MainForm  <br/><br/>
        ///
        ///  Description: Overloaded version 2/4. Updates the current configuration group. <br/>
        ///  This one updates the ip/port values of the group. It's updated this way so <br/>
        ///  this doesn't have to wait for the backend to update "UDP_Repeater_Config.json". <br/>
        ///  This function gets called right after the user inputs a new ip/port setting. <br/><br/>
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
            if (mode == "Receiving From")
            {
                if (ip == "0.0.0.0") currentReceiveIp.Text = "Any  -  (0.0.0.0)";
                else currentReceiveIp.Text = ip;
                currentReceivePort.Text = port;
            }
            else if (mode == "Sending To")
            {
                currentSendIp.Text = ip;
                currentSendPort.Text = port;
            }
        }
        /// <summary> 
        ///  Class Name: MainForm  <br/><br/>
        ///
        ///  Description: Overloaded version 3/4. Updates the current configuration group. <br/>
        ///  This one updates the interval and interval values of the group. It's updated this <br/>
        ///  way so this doesn't have to wait for the backend to update "UDP_Repeater_Config.json". <br/>
        ///  This function gets called right after the user inputs new inactivity settings. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  string <paramref name="interval"/> - The new interval value. <br/>
        ///  string <paramref name="interval"/> - The new interval value. <br/><br/>
        ///  
        ///  Returns: None
        /// </summary>
        public void UpdateCurrentConfigGroup(string interval, string unit)
        {
            if (interval != "1") 
            {
                unit += "s";
            }
            currentInterval.Text = interval;
            currentTimeUnit.Text = unit;
        }
        /// <summary> 
        ///  Class Name: MainForm  <br/><br/>
        ///
        ///  Description: (Kind of) Overloaded version 4/4. Updates the current configuration group. <br/>
        ///  This one is for when the settings are returned to default. <br/><br/>
        ///
        ///  Inputs: None <br/><br/>
        ///  
        ///  Returns: None
        /// </summary>
        public void UpdateCurrentConfigGroupWithDefaults()
        {
            string jsonString = File.ReadAllText("C:\\Windows\\SysWOW64\\UDP_Repeater_Config.json");
            JObject jsonObject = JObject.Parse(jsonString);

            currentReceiveIp.Text   = (string)jsonObject["defaultSettings"]["receiveFrom"]["ip"];
            currentReceivePort.Text = (string)jsonObject["defaultSettings"]["receiveFrom"]["port"];
            currentSendIp.Text      = (string)jsonObject["defaultSettings"]["sendTo"]["ip"];
            currentSendPort.Text    = (string)jsonObject["defaultSettings"]["sendTo"]["port"];
        }


        /// <summary> 
        ///  Class Name: MainForm  <br/><br/>
        ///
        ///  Description: Intializes the UDP Client listening and holds the infinite listening loop. <br/><br/>
        ///
        ///  Inputs: None <br/><br/>
        ///  
        ///  Returns: None
        /// </summary>
        private async void InitializeUDPListener()
        {
            try
            {
                if (!isListening)
                {
                    isListening = true;
                    udpClient = new UdpClient(56722);
                    await ReceiveDataAsync();
                }
            }
            catch (Exception e)
            {
                logger.LogException(e);
            }
        }

        /// <summary> 
        ///  Class Name: MainForm  <br/><br/>
        ///
        ///  Description: This listens until a packet is received, then calls ProcessReceivedData to
        ///  to handle the new packet. <br/><br/>
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
                catch (ObjectDisposedException e)
                {
                    // Handle if the UDP client is disposed
                    isListening = false;
                    logger.LogException(e);
                }
                catch (Exception ex)
                {
                    // Handle other exceptions
                    MessageBox.Show($"Error receiving data: {ex.Message}");
                    logger.LogException(ex);
                }
            }
        }

        /// <summary> 
        ///  Class Name: MainForm  <br/><br/>
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
            try
            {
                    // we always send out packets back and forth with comma seperated stringss
                string receivedData = Encoding.ASCII.GetString(receivedBytes);
                string[] dataParts = receivedData.Split(',');

                    // Updates data grid view from a different thread
                packetsHandledDisplay.Invoke((MethodInvoker)delegate
                {
                    DataGridViewRow row;    // the new row to populate

                        // this caps the table at most recent 250 packets
                    if (packetsHandledDisplay.Rows.Count >= 250)
                    {
                        packetsHandledDisplay.Rows.Add();
                        packetsHandledDisplay.Rows.RemoveAt(250);
                        row = packetsHandledDisplay.Rows[249];
                    }
                    else
                    {
                        int rowNum = packetsHandledDisplay.Rows.Add();
                        row = packetsHandledDisplay.Rows[rowNum];
                    }

                        // Updates DataGridView with received data
                    row.Cells["indexColumn"].Value = (totalPacketCounter + 1);
                    row.Cells["ipColumn"].Value = dataParts[0];
                    row.Cells["portColumn"].Value = dataParts[1];
                    row.Cells["payloadColumn"].Value = dataParts[2];
                    row.Cells["timeColumn"].Value = DateTime.Now.ToString();
                    totalPacketCounter++;

                        // changes sort direction to keep the most recent packet at the top.
                        // MAKE SURE IT'S SORTING INTEGERS AND NOT STRINGS.
                    packetsHandledDisplay.Sort(packetsHandledDisplay.Columns[0], ListSortDirection.Descending);
                });

                    // updates packet counter
                packetCounter.Text = totalPacketCounter.ToString();
            }
            catch (Exception e) { logger.LogException(e); }
        }

        /// <summary> 
        ///  Class Name: MainForm  <br/><br/>
        ///
        ///  Description: Sets ups the timer that handles checking for the service running status. <br/>
        ///  The object this returns isn't used but we keep it to dispose of when the GUI is exiting. <br/><br/>
        ///
        ///  Inputs: None <br/><br/>
        ///  
        ///  Returns: Timer - The timer object with the Event handler for checking service status.
        /// </summary>
        private Timer SetupTimerForServiceStatus()
        {
            try
            {
                        // this is the System.Windows.Forms.Timer, not System.Timers.Timer
                        // if i use the latter option, it gets mad at multi-threading
                        // using the windows forms timer makes it work
                Timer timer = new Timer();  

                timer.Enabled = true;

                timer.Tick += new EventHandler(TimerEventProcessor);

                timer.Interval = 5000;      // checks every 5 seconds

                timer.Start();

                return timer;
            }
            catch (Exception e) 
            {
                logger.LogException (e);
                return null;
            }
        }

        /// <summary> 
        ///  Class Name: MainForm  <br/><br/>
        ///
        ///  Description: Checks if the service side is running and displays the corresponding label <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  object <paramref name="source"/> - Not used here. <br/>
        ///  EventArgs <paramref name="e"/> - The event firing that called this. <br/><br/>
        ///  
        ///  Returns: None
        /// </summary>
        public void TimerEventProcessor(Object source, EventArgs e)
        {
            try
            {
                ourServiceMonitor.Refresh();
                switch (ourServiceMonitor.Status)
                {
                    case ServiceControllerStatus.Running:
                        statusLabel.Text = "Running";
                        statusLabel.ForeColor = Color.Green;
                        break;
                    case ServiceControllerStatus.Stopped:
                        statusLabel.Text = "Not Running";
                        statusLabel.ForeColor = Color.Red;
                        break;
                    case ServiceControllerStatus.StartPending:
                        statusLabel.Text = "Start Pending";
                        statusLabel.ForeColor = Color.Purple;
                        break;
                    case ServiceControllerStatus.StopPending:
                        statusLabel.Text = "Stop Pending";
                        statusLabel.ForeColor = Color.Purple;
                        break;
                }
            }
            catch (InvalidOperationException notFound)
            {
                statusLabel.Text = "Not Found";
                statusLabel.ForeColor = Color.DarkRed;
                logger.LogException(notFound);
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        

        /// <summary> 
        ///  Class Name: MainForm  <br/><br/>
        ///
        ///  Description: If the user tries to close, this displays a message box to ask if they're sure. <br/>
        ///               Otherwise it lets the form close and handles disposing. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  object <paramref name="sender"/> - I don't use this, i'm not sure. <br/>
        ///  FormClosingEventArgs <paramref name="e"/> - The form closed event arg. <br/><br/>
        ///  
        ///  Returns: None
        /// </summary>
        private void gui_form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure you want to close this Interface? \n" +
                                                            "You will lose the information about packets gathered since the interface started.",
                                                            "Closing Confirmation",
                                                            MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    DisposeOfStuff();
                }
                else
                {
                    e.Cancel = true;
                    WindowState = FormWindowState.Minimized;
                }
            }
            else   // Covers for when the system or anything else closes the GUI
            {
                DisposeOfStuff();
            }
        }
        /// <summary> 
        ///  Class Name: MainForm  <br/><br/>
        ///
        ///  Description: Disposes of all of the disposable objects. Called when this form is closing. <br/><br/>
        ///
        ///  Inputs:  <br/><br/>
        ///  
        ///  Returns: None
        /// </summary>
        private void DisposeOfStuff()
        {
            logger.StartStopLogger("stop");
            System.Threading.Thread.Sleep(1000);
            if (logger.eventLog != null)
            {
                logger.eventLog.Dispose();
            }
            if (logger.meterProvider != null)
            {
                logger.meterProvider.Dispose();
            }
            timer.Dispose();
        }


        /// <summary> 
        ///  Class Name: MainForm  <br/><br/>
        ///
        ///  Description: Opens the IP/Port reconfiguration form. If the InputForm's
        ///  "Reconfigure NIC or Monitoring" button is clicked, that form closes and 
        ///  we open the Setup here. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  object <paramref name="sender"/> - I don't use this, i'm not sure. <br/>
        ///  EventArgs <paramref name="e"/> - The event arg. <br/><br/>
        ///  
        ///  Returns: None
        /// </summary>
        private void showDialogbutton_Click(object sender, EventArgs e)
        {
            using (InputForm messageDialog = new InputForm(this))
            {
                DialogResult response = messageDialog.ShowDialog();
                    // abort is the result of clicking the "Reconfigure NIC or Monitoring"
                    // button, so we open that form
                if (response == DialogResult.Abort)     
                {
                    using (Setup setupForm = new Setup(this))
                    {
                        DialogResult result = setupForm.ShowDialog();
                            // OK is the result of a successful setupForm "Submit" button press, so we restart the gui because 
                            // the loki logging doesn't like being reconfigured when its already been assigned. 
                            // The same thing happens to the Service for that reason
                        if (result == DialogResult.OK)
                        {
                            System.Threading.Thread.Sleep(1000);
                                // This does call the FormClosing event with CloseReason.ApplicationExitCall 
                            Application.Restart();      
                            Environment.Exit(0);
                        }
                    }
                }
                else
                {
                    return;
                }
            }
        }

        /// <summary> 
        ///  Class Name: MainForm  <br/><br/>
        ///
        ///  Description: Opens the Log form (not as a dialog). <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  object <paramref name="sender"/> - I don't use this, i'm not sure. <br/>
        ///  EventArgs <paramref name="e"/> - The event arg. <br/><br/>
        ///  
        ///  Returns: None
        /// </summary>
        private void logButton_Click(object sender, EventArgs e)
        {
            LogForm logForm = new LogForm(this);
            logForm.Show();
        }

        /// <summary>
        /// Capitlizes the first letter of the string. 
        /// </summary>
        /// <returns> <paramref name="str"/> with the first letter capitilized.</returns>
        private string FirstLetterCapital(string str)
        {
            return Char.ToUpper(str[0]) + str.Remove(0, 1);
        }

        /// <summary> 
        ///  Class Name: MainForm  <br/><br/>
        ///
        ///  Description: Catches and logs any unhandled exceptions. I saw that you need both of these <br/>
        ///  functions with windows forms to really catch everything. They're identical inside. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  object <paramref name="sender"/> - I don't use this, i'm not sure. <br/>
        ///  UnhandledExceptionEventArgs <paramref name="e"/> - The unhandled exception <br/><br/>
        ///  
        ///  Returns: None
        /// </summary>
        public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            string message = String.Format($"Error Message: {ex.Message} \n" +
                                           $"Error location: Frontend/User Interface \n" +
                                           $"{ex.StackTrace}");

            using (EventLog tempLog = new EventLog("UDP Packet Repeater"))
            {
                tempLog.Source = "UDP_Repeater_Frontend";
                tempLog.WriteEntry(message, EventLogEntryType.Error, 2);  // 2 is our id for Frontend errors
            }
        }
        /// <summary> 
        ///  Class Name: MainForm  <br/><br/>
        ///
        ///  Description: Catches and logs any unhandled exceptions. I saw that you need both of these <br/>
        ///  functions with windows forms to really catch everything. They're identical inside. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  object <paramref name="sender"/> - I don't use this, i'm not sure. <br/>
        ///  UnhandledExceptionEventArgs <paramref name="e"/> - The unhandled exception <br/><br/>
        ///  
        ///  Returns: None
        /// </summary>
        private static void ThreadExceptionHandlerFunction(object sender, System.Threading.ThreadExceptionEventArgs t)
        {
            Exception ex = t.Exception;
            string message = String.Format($"Error Message: {ex.Message} \n" +
                                           $"Error location: Frontend/User Interface \n" +
                                           $"{ex.StackTrace}");

            using (EventLog tempLog = new EventLog("UDP Packet Repeater"))
            {
                tempLog.Source = "UDP_Repeater_Frontend";
                tempLog.WriteEntry(message, EventLogEntryType.Error, 2);  // 2 is our id for Frontend errors
            }
        }
    }
}
