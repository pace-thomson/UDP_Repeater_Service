//----------------------------------------------------
// File Name: ReceiveFromGUI.cs
// 
// Description: This file contains the methods for 
//              receiving packets from the GUI and 
//              parsing the data to update the  
//              system configuration.
//
// Language:         Visual C#
// Target:           Windows PC
// Operating System: Windows 11 Enterprise
// Compiler:         Visual Studio .Net 2022
//
//          Change History:
//
// Version   Date          Author            Description
//   1.0    7/25/24    Jade Pace Thomson   Initial Release
//---------------------------------------------------


using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using BackendClassNameSpace;


namespace GUIreceiver
{
    /// <summary>
    /// Houses the functions that handle input from the user through the GUI. This input
    /// is used to reconfigure the program's settings to the user's input.
    /// </summary>
    internal class ReceiveFromGUI
    {
        /// <summary> 
        ///  Class Name: ReceiveFromGUI  <br/> <br/>
        ///
        ///  Description: Continually listens for communication from the GUI, until a packet is received. <br/>
        ///  Then, the payload of that packet is processed and broken into it's parts for reconfiguration. <br/><br/>
        ///
        ///  Inputs:  None <br/>
        ///  Backend <paramref name="backendObject"/> - The Backend object for logging. <br/><br/>
        ///  
        ///  Returns: string[] - An string array containing the new configuration information
        /// </summary>
        public static string[] ReceivingFromGUI(Backend backendObject)
        {
            bool socketOpen = false;
            int socketCantConnectCount = 0;
            UdpClient listener = new UdpClient();
            try
            {
                while (!socketOpen)
                {
                    try
                    {
                        listener = new UdpClient(63763);
                        socketOpen = true;
                    }
                    catch (SocketException) 
                    {
                        socketCantConnectCount++;
                        
                        if (socketCantConnectCount > 15)
                        {
                            backendObject.WarningLogger("Receiving from GUI port 63763 not open. " +
                                                        "Tried 15 times to connect without success");
                            break;
                        }
                        System.Threading.Thread.Sleep(1000); 
                    }
                }
                
                IPEndPoint senderEndPoint = new IPEndPoint(IPAddress.Any, 0);

                byte[] messageBytes = listener.Receive(ref senderEndPoint);

                string receivedData = Encoding.ASCII.GetString(messageBytes);
                string[] dataParts = receivedData.Split(',');

                listener.Close();
                return dataParts;
            }
            catch ( Exception e ) 
            {
                backendObject.ExceptionLogger(e);
                return "Exception met".Split(',');
            }
        }

        /// <summary> 
        ///  Class Name: ReceiveFromGUI  <br/><br/> 
        ///
        ///  Description: The main function of the ReceiveFromGUI class. Listens for information, and then updates <br/>
        ///  backendObject based on the request from the GUI.<br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  Backend <paramref name="backendObject"/> - The Backend object to supply configuration information <br/><br/>
        ///  
        ///  Returns:  Backend newbackendObject - A backendObject with updated values. 
        /// </summary>
        public static Backend main(Backend backendObject)
        {
            try
            {
                        // a new backendObject gets made to compare to the old one when this function returns
                Backend newbackendObject = new Backend(backendObject);

                        // waits for input from the GUI
                string[] dataParts = ReceivingFromGUI(backendObject);

                        // dataParts[2] is the change mode 
                switch (dataParts[2])
                {
                    case "Receiving From":
                        newbackendObject.receiveIp = dataParts[0];
                        newbackendObject.receivePort = int.Parse(dataParts[1]);
                        newbackendObject.change = Backend.changeType.receivingFrom;
                        break;
                    case "Sending To":
                        newbackendObject.sendIp = dataParts[0];
                        newbackendObject.sendPort = int.Parse(dataParts[1]);
                        newbackendObject.change = Backend.changeType.sendingTo;
                        break;
                    case "Default Send":
                        newbackendObject.sendIp = dataParts[0];
                        newbackendObject.sendPort = int.Parse(dataParts[1]);
                        newbackendObject.change = Backend.changeType.defaultSend;
                        break;                                                   
                    case "Default Receive":
                        newbackendObject.receiveIp = dataParts[0];
                        newbackendObject.receivePort = int.Parse(dataParts[1]);
                        newbackendObject.change = Backend.changeType.defaultRecieve;
                        break;                                                   
                    case "inactive":
                        newbackendObject.inactivityInterval = int.Parse(dataParts[0]);
                        newbackendObject.inactivityUnit = dataParts[1];
                        newbackendObject.change = Backend.changeType.inactive;
                        break;
                    case "setup":
                        newbackendObject.promEndpoint = dataParts[0];
                        newbackendObject.lokiEndpoint = dataParts[1];
                        newbackendObject.descriptionOfNIC = dataParts[3];
                        newbackendObject.change = Backend.changeType.setup;
                        backendObject.WarningLogger("Restarting service due to NIC/Monitoring reconfiguration.");
                        break;
                    default:
                        newbackendObject.change = Backend.changeType.restoreToDefaults;
                        break;
                }

                return newbackendObject;
            }
            catch (Exception e)
            {
                backendObject.ExceptionLogger(e);
                return null;
            }
        }
    }
}
