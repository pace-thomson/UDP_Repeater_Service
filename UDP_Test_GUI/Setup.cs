//----------------------------------------------------
// File Name: Setup.cs
// 
// Description: This file handles the form for selecting
//              a NIC to listen on and what monitoring  
//              endpoints the user wants to use. This form
//              is opened automatically on GUI startup if 
//              config.json was created in the last 30sec.
//              It can also be opened via a button on the 
//              input form.
//
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


using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using UDP_Repeater_GUI;
using Newtonsoft.Json.Linq;
using System.Net.NetworkInformation;


namespace UDP_Test_GUI
{
    /// <summary> The form for configuring settings for the system's monitoring endpoints and selecting what NIC to
    /// listen on. This is opened if UDP_Repeater_Config.json is new or if the user opens it off of the input form. </summary>
    public partial class Setup : Form
    {
            /// <summary> Tracks whether the user's selection/inputs were valid </summary>
        public bool isValidInput;
            /// <summary> The main form's object. </summary>
        private MainForm theMainForm;
            /// <summary> The uri string for the prometheus endpoint that this form will return. </summary>
        public string prom;
            /// <summary> The uri string for the prometheus endpoint that this form will return. </summary>
        public string loki;

        /// /// <summary> 
        ///  Class Name: Setup  <br/><br/>
        ///
        ///  Description: The Setup form constructor. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  MainForm <paramref name="TheMainForm"/> - The main form's object. Passed in so that 
        ///  we can have logging. <br/><br/>
        ///  
        ///  Returns: A Setup object.
        /// </summary>
        public Setup(MainForm TheMainForm)
        {
            InitializeComponent();

            PopulateCurrentConfig();

            PopulateNICs();

            this.theMainForm = TheMainForm;

            this.isValidInput = true;

            listOfNICs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        /// <summary> 
        ///  Class Name: Setup  <br/><br/>
        ///
        ///  Description: Fills out the data grid view with all the NIC's on the system so the user can select one. <br/><br/>
        ///
        ///  Inputs:  None <br/><br/>
        ///  
        ///  Returns: None
        /// </summary>
        private void PopulateNICs()
        {
            try
            {
                foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
                {
                    foreach (var nic in networkInterface.GetIPProperties().UnicastAddresses)
                    {
                        if (nic.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            int rowNum = listOfNICs.Rows.Add();
                            DataGridViewRow row = listOfNICs.Rows[rowNum];

                            row.Cells["nameColumn"].Value = networkInterface.Name;
                            row.Cells["ipAddressColumn"].Value = nic.Address.ToString();
                            row.Cells["descriptionColumn"].Value = networkInterface.Description;
                        }
                    }
                }
            }
            catch (Exception ex) { theMainForm.logger.LogException(ex); }
        }

        /// <summary> 
        ///  Class Name: Setup <br/><br/>
        ///
        ///  Description: Polulates this form's current configuration section with the system's NIC and endpoints. <br/><br/>
        ///
        ///  Inputs:  None 
        ///  </summary> 
        ///  <returns> void </returns>
        private void PopulateCurrentConfig()
        {
            string jsonString = File.ReadAllText("C:\\Windows\\SysWOW64\\UDP_Repeater_Config.json");
            JObject jsonObject = JObject.Parse(jsonString);

            promTextbox.Text = (string)jsonObject["monitoring"]["prom"];
            lokiTextbox.Text = (string)jsonObject["monitoring"]["loki"];
            nicTextbox.Text  = (string)jsonObject["ipAddressOfNIC"];
        }

        /// <summary> 
        ///  Class Name: Setup <br/><br/>
        ///
        ///  Description: Handles the "done" button click. Validates input and then sends to the Backend. <br/>
        ///  If input isn't valid, displays a specific messageBox.<br/><br/>
        ///
        ///  Inputs:  None 
        ///  </summary> 
        ///  <returns> void </returns>
        private void doneButton_Click(object sender, EventArgs e)
        {
            try
            {
                isValidInput = true;
                int selectedRowCount = listOfNICs.Rows.GetRowCount(DataGridViewElementStates.Selected);

                this.prom = promEndpoint.Text.Trim();
                this.loki = lokiEndpoint.Text.Trim();

                if (selectedRowCount == 1)
                {
                    if (prom != "" && loki != "")
                    {
                        if (Uri.IsWellFormedUriString(prom, UriKind.Absolute) &&
                            Uri.IsWellFormedUriString(loki, UriKind.Absolute))
                        {
                            DataGridViewRow row = listOfNICs.SelectedRows[0];
                            string name = row.Cells["nameColumn"].Value.ToString();
                            string ip = row.Cells["ipAddressColumn"].Value.ToString();
                            string description = row.Cells["descriptionColumn"].Value.ToString();

                            using (UdpClient sendRequest = new UdpClient())
                            {
                                byte[] bytes = Encoding.ASCII.GetBytes($"{this.prom},{this.loki},setup,{ip}");
                                sendRequest.Send(bytes, bytes.Length, "127.0.0.1", 63763);

                                theMainForm.logger.LogNicChange(name, ip);
                                theMainForm.logger.LogMonitoringChange(this.prom, this.loki);

                                sendRequest.Close();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Invalid uri endpoint input. Please try again.");
                            isValidInput = false;
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please fill in both endpoint fields.");
                        isValidInput = false;
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Please pick one Network Interface Card.");
                    isValidInput = false;
                    return;
                }
            }
            catch (Exception ex) { theMainForm.logger.LogException(ex); }
        }

        /// <summary> 
        ///  Class Name: Setup <br/><br/>
        ///
        ///  Description: Checks if isValidInput is true, and either cancels the form closing or lets it close. <br/><br/>
        ///
        ///  Inputs:  None 
        ///  </summary> 
        ///  <returns> void </returns>
        private void Setup_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isValidInput)
            {
                e.Cancel = true;
                isValidInput = true;
                return;
            }
        }
    }
}
