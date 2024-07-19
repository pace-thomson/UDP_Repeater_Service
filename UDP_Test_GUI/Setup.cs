using System;
using SharpPcap;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using UDP_Repeater_GUI;

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

            logger = new Logger();

            isValid = true;

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        /// <summary> Fills out the data grid view with all the NIC's on the system </summary>
        private void PopulateNICs()
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

        /// <summary> Handles the done button click. Validates input and then sends to the Backend. </summary>
        private void doneButton_Click(object sender, EventArgs e)
        {
            int selectedRowCount = dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
            if (selectedRowCount == 1)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                string selected = row.Cells["descriptionColumn"].Value.ToString();

                using (UdpClient sendRequest = new UdpClient())
                {
                    try
                    {
                        byte[] bytes = Encoding.ASCII.GetBytes($"{selected},irrelevant,nic");
                        sendRequest.Send(bytes, bytes.Length, "127.0.0.1", 50001);
                        logger.LogNicChange(selected, row.Cells["macAddressColumn"].Value.ToString());
                    }
                    catch (Exception exception)
                    {
                        logger.LogException(exception);
                        return;
                    }
                    sendRequest.Close();
                }
            }
            else
            {
                MessageBox.Show("Please pick one row and then press Sumbit.");
                isValid = false;
                return;
            }
        }

        /// <summary> Checks if isValid is true, and either cancels the form closing or 
        /// lets it close and disposes of the logger objects eventLog.</summary>
        private void NIC_Picker_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isValid)
            {
                e.Cancel = true;
            }
            logger.eventLog.Dispose();
        }
    }
}
