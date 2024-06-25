//----------------------------------------------------
// File Name: InputForm.cs
// 
// Description: This file handles the input IP/Port
//              configuration form.
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
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace UDP_Repeater_GUI
{
    /// <summary>
    /// The class for the form with the inputs for reconfiguring the 
    /// ip's and ports. Comes up when the "Reconfigure IP's/Ports" button 
    /// is clicked on the main form.
    /// </summary>
    public partial class configDialog : Form
    {
            /// <summary>The main form's object.</summary>
        private gui_form theMainForm;
            /// <summary> Keeps track of if the user has provided valid ip/port/profile </summary>
        private bool inputValid;

        /// <summary> 
        ///  Class Name: configDialog  <br/><br/>
        ///
        ///  Description: Constructs the form. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  gui_form <paramref name="mainForm"/> - The main form's object. Passed in so that 
        ///  we can update it's "Current Configuration" section whenever a change is made.<br/><br/>
        ///  
        ///  Returns: A configDialog object.
        /// </summary>
        public configDialog(gui_form mainForm)
        {
            InitializeComponent();
            theMainForm = mainForm;
            inputValid = true;
        }

        /// <summary> 
        ///  Class Name: configDialog  <br/><br/>
        ///
        ///  Description: Handles the OK button on this form. It checks the input boxes to see <br/>
        ///  if their values are valid, and then sends them if they are. If not, opens a messagebox <br/>
        ///  that, when closed, also closes this form. I'm not sure how to get it to not do that<br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  object <paramref name="sender"/> - Necessary for handling the button, but I don't use it. <br/>
        ///  EventArgs <paramref name="e"/> - Necessary for handling the button, but I don't use it. <br/><br/>
        ///  
        ///  Returns: None
        /// </summary>
        private void dialogOkButton_Click(object sender, EventArgs e)
        {
            string ip = ip_field.Text;
            string port = port_field.Text;
            string mode = profileDropDown.Text;
                
            IPAddress address;              // this validates if the ip address is legit
            if (ip.Count(c => c == '.') == 3 && IPAddress.TryParse(ip, out address))
            {
                using (UdpClient sendRequest = new UdpClient())
                {
                    try
                    {
                        if (mode != "")
                        {
                            if (mode == "Receiving From" || mode == "Sending To")
                            {
                                theMainForm.UpdateCurrentConfigGroup(mode, ip, port);
                            }
                        }
                        else
                        {
                            MessageBox.Show("No profile chosen, please try again.");
                            inputValid = false;
                            return;
                        }

                        // we communicate back and forth with the Service with comma seperated strings
                        byte[] bytes = Encoding.ASCII.GetBytes(ip + "," + port + "," + mode);
                        sendRequest.Send(bytes, bytes.Length, "127.0.0.1", 50001);
                        Logger.LogConfigChange(mode, ip, port);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show($"Error sending data: {exception.Message}");
                        Logger.LogException(exception);
                        return;
                    }
                                // this section resets the inputs
                    ip_field.Text = "";
                    port_field.Text = "";      
                    sendRequest.Close();
                    inputValid = true;
                }
            }
            else
            {
                MessageBox.Show("Invalid IP or Port, please try again.");
                inputValid = false;
                return;
            }
        }

        /// <summary> 
        ///  Class Name: configDialog  <br/><br/>
        ///
        ///  Description: Handles the "Restore to Default Settings" button on this form. It just sends <br/>
        ///  a packet with throwaway values to tell the backend that reverting to default was <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  object <paramref name="sender"/> - Necessary for handling the button, but I don't use it. <br/>
        ///  EventArgs <paramref name="e"/> - Necessary for handling the button, but I don't use it. <br/><br/>
        ///  
        ///  Returns: None
        /// </summary>
        private void defaultButton_Click(object sender, EventArgs e)
        {
            using (UdpClient sendRequest = new UdpClient())
            {
                try
                {
                            // sends empty message to tell service that default setting were selected
                            // This doesn't even get read by the backend, it just is a placeholder
                    byte[] bytes = Encoding.ASCII.GetBytes(",,");
                    sendRequest.Send(bytes, bytes.Length, "127.0.0.1", 50001);
                    Logger.LogConfigChange("Reverted to Default", "N/A", "N/A");
                }
                catch (Exception exception)
                {
                    MessageBox.Show($"Error sending data: {exception.Message}");
                    Logger.LogException(exception);
                    return;
                }
                ip_field.Text = "";
                port_field.Text = "";
                sendRequest.Close();
            }
        }

        /// <summary> 
        ///  Class Name: configDialog  <br/><br/>
        ///
        ///  Description: If inputs are not valid, then it doesn't close the window. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  object <paramref name="sender"/> - Necessary for handling the button, but I don't use it. <br/>
        ///  FormClosingEventArgs <paramref name="e"/> - Form closing is cancelled if inputs aren't valid. <br/><br/>
        ///  
        ///  Returns: None
        /// </summary>

        private void configDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!inputValid)
            {
                e.Cancel = true;
            }
        }
    }
}
