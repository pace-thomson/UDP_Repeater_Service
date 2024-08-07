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
// Version   Date          Author            Description
//   1.0    7/25/24    Jade Pace Thomson   Initial Release
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
    /// The class for the form with the inputs for reconfiguring the system's 
    /// ip's and ports. Is created when the "Reconfigure Settings" button 
    /// is clicked on the main form.
    /// </summary>
    public partial class InputForm : Form
    {
            /// <summary>The main form's object.</summary>
        private MainForm theMainForm;
            /// <summary> Keeps track of if the user has provided valid ip/portString/profile </summary>
        private bool inputValid;

        /// <summary> 
        ///  Class Name: InputForm  <br/><br/>
        ///
        ///  Description: Constructs the form. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  MainForm <paramref name="mainForm"/> - The main form's object. Passed in so that <br/>
        ///  we can update it's "Current Configuration" section whenever a change is made. <br/>
        ///  Also we use it's logger object for all of our logging needs. <br/><br/>
        ///  
        ///  Returns: A InputForm form object.
        /// </summary>
        public InputForm(MainForm mainForm)
        {
            InitializeComponent();
            theMainForm = mainForm;
            inputValid = true;
        }

        /// <summary> 
        ///  Class Name: InputForm  <br/><br/>
        ///
        ///  Description: Handles the OK button on this form. It checks the input boxes to see <br/>
        ///  if their values are valid, and then sends them if they are. If not, opens a messagebox <br/>
        ///  that tells the user to try again. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  object <paramref name="sender"/> - Necessary for handling the button, but I don't use it. <br/>
        ///  EventArgs <paramref name="e"/> - Necessary for handling the button, but I don't use it. <br/><br/>
        ///  
        ///  Returns: None
        /// </summary>
        private void dialogOkButton_Click(object sender, EventArgs e)
        {
            try
            {
                string ip = ipTextbox.Text;
                string portString = portTextbox.Text;
                string mode = profileDropdown.Text;
                int portInt = 0;
                try
                {
                    portInt = int.Parse(portTextbox.Text);
                }
                catch (FormatException)     // gets called if portTextbox is empty
                {
                    MessageBox.Show("Please fill in all input fields.");
                    inputValid = false;
                    return;
                }

                IPAddress address;              
                if (ip.Count(c => c == '.') == 3 &&         // validates if the ip address is formatted correctly
                    IPAddress.TryParse(ip, out address) &&  // validates if it can make a valid ip address, and makes one if possible
                    portInt > 0 && portInt <= 65535)        // checks if the port number is in the valid range
                {
                    using (UdpClient sendRequest = new UdpClient())
                    {
                        if (mode == "")
                        {
                            MessageBox.Show("No profile chosen, please try again.");
                            inputValid = false;
                            return;
                        }
                        if (mode == "Receiving From" || mode == "Sending To")
                        {
                            theMainForm.UpdateCurrentConfigGroup(mode, ip, portString);
                        }

                        // we communicate back and forth with the Service with comma seperated strings
                        byte[] bytes = Encoding.ASCII.GetBytes(ip + "," + portString + "," + mode);
                        sendRequest.Send(bytes, bytes.Length, "127.0.0.1", 63763);


                        theMainForm.logger.LogConfigChange(mode, ip, portString);

                        // this section resets the inputs
                        ipTextbox.Text = "";
                        portTextbox.Text = "";

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
            catch (Exception ex) 
            { 
                theMainForm.logger.LogException(ex); 
            }
        }

        /// <summary> 
        ///  Class Name: InputForm  <br/><br/>
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
                    sendRequest.Send(bytes, bytes.Length, "127.0.0.1", 63763);
                    theMainForm.logger.LogConfigChange("Reverted to Default", "N/A", "N/A");
                    theMainForm.UpdateCurrentConfigGroupWithDefaults();
                }
                catch (Exception exception)
                {
                    MessageBox.Show($"Error sending data: {exception.Message}");
                    theMainForm.logger.LogException(exception);
                    return;
                }
                ipTextbox.Text = "";
                portTextbox.Text = "";
                sendRequest.Close();
                inputValid = true;
            }
        }

        /// <summary> 
        ///  Class Name: InputForm  <br/><br/>
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
                    // inputValid needs to be reset because we have already cancelled the form closing
                    // otherwise, pressing the "X" after this wounldn't close the form
                inputValid = true;  
                return;
            }
        }

        /// <summary> 
        ///  Class Name: InputForm  <br/><br/>
        ///
        ///  Description: Lets this form close because we are opening the setup form. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  object <paramref name="sender"/> - Necessary for handling the button, but I don't use it. <br/>
        ///  EventArgs <paramref name="e"/> - Necessary for handling the button, but I don't use it. <br/><br/>
        ///  
        ///  Returns: None
        /// </summary>
        private void reconfigNicButton_Click(object sender, EventArgs e)
        {
            inputValid = true;
        }
    }
}
