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
// Operating System: Windows 10 Enterprise
// Compiler:         Visual Studio .Net 2022
//
//          Change History:
//
// Version  Date    Author          Description
// 1.0      ---     Jade Thomson    Initial Release
//---------------------------------------------------


using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using JsonDataNameSpace;

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
        ///  Description: Continually listens for communication from the GUI, until some is received. <br/>
        ///  Then, the payload of that communicaiton is processed and broken into it's parts. <br/><br/>
        ///
        ///  Inputs:  None <br/><br/>
        ///  
        ///  Returns: string[] - An array of strings containing the new confiuration information
        /// </summary>
        public static string[] ReceivingFromGUI()
        {
            try
            {
                UdpClient listener = new UdpClient(50001);
                IPEndPoint senderEndPoint = new IPEndPoint(IPAddress.Any, 0);


                byte[] messageBytes = listener.Receive(ref senderEndPoint);

                string receivedData = Encoding.ASCII.GetString(messageBytes);
                string[] dataParts = receivedData.Split(',');

                listener.Close();
                return dataParts;
            }
            catch ( Exception e ) 
            {
                JsonData.Logger(e);
                return "Exception met".Split(',');
            }
        }
        /// <summary> 
        ///  Class Name: ReceiveFromGUI  <br/><br/> 
        ///
        ///  Description: The main function of the ReceiveFromGUI class. Listens for information, and then updates <br/>
        ///  jsonData based on the request fromthe GUI.<br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  JsonData <paramref name="jsonData"/> - The JsonData object to supply configuraiton information <br/><br/>
        ///  
        ///  Returns:  JsonData newjsonData - The new and updated JsonData object.
        /// </summary>
        public static JsonData main(JsonData jsonData)
        {
            JsonData newjsonData = new JsonData(jsonData.receiveIp, 
                                                jsonData.receivePort.ToString(), 
                                                jsonData.sendIp, 
                                                jsonData.sendPort.ToString());

                    // This resets the send or receive data if those options are selected
                    // and does nothing if defaults is selected, which is handled where this is called
            string[] dataParts = ReceivingFromGUI();

                    // dataParts[2] is mode 
            switch (dataParts[2])
            {
                case "Receive":
                    newjsonData.receiveIp = dataParts[0];
                    newjsonData.receivePort = int.Parse(dataParts[1]);
                    break;
                case "Send":
                    newjsonData.sendIp = dataParts[0];
                    newjsonData.sendPort = int.Parse(dataParts[1]);
                    break;
                case "Default_Send":
                    newjsonData.sendIp = dataParts[0];
                    newjsonData.sendPort = int.Parse(dataParts[1]);
                    newjsonData.receivePort = -1;                       // setting the untouched port to -1 lets UpdateConfigJson 
                    break;                                              // know that we want to change the defaults
                case "Default_Receive":
                    newjsonData.receiveIp = dataParts[0];
                    newjsonData.receivePort = int.Parse(dataParts[1]);
                    newjsonData.sendPort = -1;                          // setting the untouched port to -1 lets UpdateConfigJson 
                    break;                                              // know that we want to change the defaults
            }

            return newjsonData; 
        }
    }
}
