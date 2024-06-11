//----------------------------------------------------
// File Name: Repeater.cs
// 
// Description: This file contains the method that constantly 
//              is listening for UDP packets. It also 
//              contains the methods for sending the 
//              packets back out and information to the 
//              GUI/Front End.
//
// Language:         Visual C#
// Target:           Windows PC
// Operating System: Windows 10 Enterprise
// Compiler:         Visual Studio .Net 2022
//
// Change History:
//
// Version  Date    Author          Description
// 1.0      ---     Jade Thomson    Initial Release
//---------------------------------------------------




using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Text;
using JsonDataNameSpace;
using System;


namespace Repeater
{
    class RepeaterClass
    {
        /// <summary> 
        ///  Class Name: RepeaterClass  <br/> <br/>
        ///
        ///  Description: Sends the processed package out <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  byte[] <paramref name="messageBytes"/> - The byte array payload of the received packet <br/>
        ///  JsonData <paramref name="jsonData"/> - The JsonData object to supply our send IP and Port <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public static void SendMessageOut(byte[] messageBytes, JsonData jsonData)
        {
            using (UdpClient sender = new UdpClient())
            {
                try
                {
                    sender.Send(messageBytes, messageBytes.Length, jsonData.sendIp, jsonData.sendPort);
                }
                catch (Exception e)
                {
                    JsonData.Logger(e);
                }
                sender.Close();
            }
        }

                // just the testing sender, stands in for network that has the threats
        public static void TestingSender()
        {
            using (UdpClient sender = new UdpClient())
            {
                try
                {
                    byte[] bytes = Encoding.ASCII.GetBytes("987.6.5.4,4567,722 and whatever");

                    sender.Send(bytes, bytes.Length, "127.0.0.255", 7654);

                }
                catch (Exception e)
                {
                    JsonData.Logger(e);
                }
                sender.Close();
            }
        }


        /// <summary> 
        ///  Class Name: RepeaterClass  <br/> <br/>
        ///
        ///  Description: Sends packet information to the GUI <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  byte[] <paramref name="messageBytes"/> - The byte array payload of the received packet <br/>
        ///  JsonData <paramref name="jsonData"/> - The JsonData object to supply our configuration information <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public static void SendToGUI(byte[] messageBytes, JsonData jsonData)
        {
            using (UdpClient sender = new UdpClient())
            {
                try
                {
                    string ipSentTo = jsonData.sendIp;
                    string portSentTo = jsonData.sendPort.ToString();
                    string dataLength = messageBytes.Length.ToString();

                    byte[] bytes = Encoding.ASCII.GetBytes(ipSentTo + "," + portSentTo + "," + dataLength);


                    sender.Send(bytes, bytes.Length, "127.0.0.1", 50000);

                }
                catch (Exception e)
                {
                    JsonData.Logger(e);
                }
                sender.Close();
            }
        }

        /// <summary> 
        ///  Class Name: RepeaterClass  <br/><br/>
        ///
        ///  Description: Continually listens for packets. Whenver a packet<br/>
        ///  is received, one packet is sent out to the target machine, and another is sent to the GUI.<br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  CancellationToken <paramref name="token"/> - A token that signal a configuration change was made, so this task need to restart. <br/>
        ///  JsonData <paramref name="jsonData"/> - The JsonData object to supply IP and Port Configurations <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public static void StartReceiver(JsonData jsonData, CancellationToken token)
        {
            Thread.Sleep(1000);     // This HAS TO STAY or else the old port won't be closed by the time this runs

            UdpClient listener = new UdpClient(jsonData.receivePort);
            IPEndPoint senderEndPoint = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                while (true)
                {
                    byte[] messageBytes = listener.Receive(ref senderEndPoint);

                            // actual sending section
                    SendMessageOut(messageBytes, jsonData);

                            // sending to GUI section
                    SendToGUI(messageBytes, jsonData);

                            // checks each time to make sure calling function hasn't told this one to stop
                    if (token.IsCancellationRequested)      
                    {
                        listener.Close();
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                JsonData.Logger(e);
            }

            finally 
            { 
                listener.Close(); 
            }
            
        }

        /// <summary> 
        ///  Class Name: RepeaterClass  <br/><br/> 
        ///
        ///  Description: The main function of the RepeaterClass. Starts and runs the StartReceiver funtion in it's own task. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  JsonData <paramref name="jsonData"/> - The JsonData object to supply confiiguraiton information <br/>
        ///  CancellationToken <paramref name="token"/> - A token that signal a configuration change was made, so this task need to restart. <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public async static void main(JsonData jsonData, CancellationToken token)
        {
            Task.Run(() => StartReceiver(jsonData, token));

            Thread.Sleep(500);
            Thread test_sender = new Thread(() => {
                for (int i = 0; i < 20; i++)
                {
                    if (token.IsCancellationRequested)
                    {
                        JsonData.Logger(new Exception("Cancellation Requested"));
                        break;
                    }
                    TestingSender();
                    JsonData.Logger(new Exception("Test Loop Iteration: " + i));
                    Thread.Sleep(1000);
                }
            });
            test_sender.Start();

            return;
        }
    }
}