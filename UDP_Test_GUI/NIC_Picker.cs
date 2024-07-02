using System;
using SharpPcap;
using SharpPcap.LibPcap;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using UDP_Repeater_GUI;

namespace UDP_Test_GUI
{
    public partial class NIC_Picker : Form
    {

        public bool isValid;
        private Logger logger;
        public NIC_Picker()
        {
            InitializeComponent();

            PopulateNICs();

            logger = new Logger();

            isValid = true;

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

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
            }
        }

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
                        logger.LogNicChange(selected, row.Cells["typeColumn"].Value.ToString());
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
