using System;
using SharpPcap;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using UDP_Repeater_GUI;
using Newtonsoft.Json.Linq;

namespace UDP_Test_GUI
{
    /// <summary>  </summary>
    public partial class Setup : Form
    {

            /// <summary> Tracks whether the user selection was valid </summary>
        public bool isValid;
            /// <summary> This forms Logger object </summary>
        private Logger logger;

        /// <summary> The NIC_Picker form constructor </summary>
        public Setup()
        {
            InitializeComponent();

            PopulateNICs();

            PopulateCurrentConfig();

            logger = new Logger();

            isValid = true;

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        /// <summary> Fills out the data grid view with all the NIC's on the system </summary>
        private void PopulateNICs()
        {
            try
            {
                var devices = CaptureDeviceList.Instance;
                foreach (ICaptureDevice dev in devices)
                {
                    int rowNum = dataGridView1.Rows.Add();
                    DataGridViewRow row = dataGridView1.Rows[rowNum];

                    row.Cells["nameColumn"].Value = dev.Name;
                    row.Cells["descriptionColumn"].Value = dev.Description;
                    row.Cells["macAddressColumn"].Value = dev.MacAddress;
                    if (dev.MacAddress == null)
                    {
                        row.Cells["macAddressColumn"].Value = "N/A";
                    }
                }
            }
            catch (Exception ex) { logger.LogException(ex); }
        }

        private void PopulateCurrentConfig()
        {
            string jsonString = File.ReadAllText("C:\\Windows\\SysWOW64\\UDP_Repeater_Config.json");
            JObject jsonObject = JObject.Parse(jsonString);

            promLabel.Text = (string)jsonObject["monitoring"]["prom"];
            lokiLabel.Text = (string)jsonObject["monitoring"]["loki"];
            nicLabel.Text = (string)jsonObject["descriptionOfNIC"];
        }

        /// <summary> Handles the done button click. Validates input and then sends to the Backend. </summary>
        private void doneButton_Click(object sender, EventArgs e)
        {
            try
            {
                isValid = true;
                int selectedRowCount = dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
                if (selectedRowCount == 1)
                {
                    if (promEndpoint.Text != "" && lokiEndpoint.Text != "")
                    {
                        DataGridViewRow row = dataGridView1.SelectedRows[0];
                        string nic = row.Cells["descriptionColumn"].Value.ToString();
                        string prom = promEndpoint.Text;
                        string loki = lokiEndpoint.Text;

                        using (UdpClient sendRequest = new UdpClient())
                        {
                            byte[] bytes = Encoding.ASCII.GetBytes($"{prom},{loki},setup,{nic}");
                            sendRequest.Send(bytes, bytes.Length, "127.0.0.1", 50001);

                            logger.LogNicChange(nic, row.Cells["macAddressColumn"].Value.ToString());
                            logger.LogMonitoringChange(prom, loki);
                           
                            sendRequest.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please fill in both endpoint fields.");
                        isValid = false;
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Please only pick one row.");
                    isValid = false;
                    return;
                }
            }
            catch (Exception ex) { logger.LogException(ex); }
        }

        /// <summary> Checks if isValid is true, and either cancels the form closing or 
        /// lets it close and disposes of the logger objects eventLog.</summary>
        private void NIC_Picker_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isValid)
            {
                e.Cancel = true;
                return;
            }
            logger.eventLog.Dispose();
        }
    }
}
