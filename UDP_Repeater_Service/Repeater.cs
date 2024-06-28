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
// Operating System: Windows 11 Enterprise
// Compiler:         Visual Studio .Net 2022
//
// Change History:
//
// Version   Date         Author            Description
//   1.0    6/21/24   Jade Pace Thomson   Initial Release
//---------------------------------------------------



using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Text;
using BackendClassNameSpace;
using System;
using System.Timers;
using SharpPcap;
using System.Linq;
using System.Net;


namespace Repeater
{
    public class TimerClass
    {
                /// <summary>The actual Timer object.</summary>
        public static System.Timers.Timer timer;
                /// <summary>Keeps track of events fired in a row to caculate total time since last packet.</summary>
        public static int consecutiveEventsFired = 0;
                /// <summary>Check every 5 seconds</summary>
        public static int checkInterval = 5000;
                /// <summary> The specified time between packets </summary>
        public static int timeoutThreshold;
                /// <summary> This can either be the mose recent packet, or most recent log event </summary>
        public static DateTime mostRecentTimestamp;
                /// <summary> A Backend object that we use to get the inactivity settings. </summary>
        private static Backend backendObject;


        /// <summary> 
        ///  Class Name: TimerClass  <br/><br/> 
        ///
        ///  Description: The TimerClass Constructor <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  Backend <paramref name="BackendObject"/> - The Backend object that we use to get the inactivity settings. <br/><br/> 
        ///  
        /// Returns: A Timer Object
        /// </summary>
        public TimerClass(Backend BackendObject)
        {

            timer = new System.Timers.Timer(checkInterval);
            timer.Elapsed += OnTimedEvent;

            timer.AutoReset = true;     // Hook up the Elapsed event for the timer.

            timer.Enabled = true;

            backendObject = BackendObject;

            mostRecentTimestamp = DateTime.Now;

            double frequencyDouble = Convert.ToDouble(backendObject.frequency);
            switch (backendObject.interval)
            {
                case "minute":
                    timeoutThreshold = (int)TimeSpan.FromMinutes(frequencyDouble).TotalMilliseconds;
                    break;
                case "hour":
                    timeoutThreshold = (int)TimeSpan.FromHours(frequencyDouble).TotalMilliseconds;
                    break;
                case "day":
                    timeoutThreshold = (int)TimeSpan.FromDays(frequencyDouble).TotalMilliseconds;
                    break;
            }
        }

        /// <summary> 
        ///  Class Name: TimerClass  <br/><br/> 
        ///
        ///  Description: What we want to happen every time we hit a period of inactivity. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  Object <paramref name="source"/> - These are just here for the elapsed event thing. I don't use them. <br/> 
        ///  ElapsedEventArgs <paramref name="e"/> - These are just here for the elapsed event thing. I don't use them. <br/><br/> 
        ///  
        /// Returns: None 
        /// </summary>
        public static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if ((DateTime.Now - mostRecentTimestamp).TotalMilliseconds > timeoutThreshold)
            {
                // Logs the event with the number of events fired multiplied by the frequency
                // for example, with 5 minutes as the setting, the 3rd consecutive event would log 15 minutes
                consecutiveEventsFired++;
                mostRecentTimestamp = DateTime.Now;

                Backend.InactivityLogger(consecutiveEventsFired, backendObject.frequency, backendObject.interval);
            }
        }

        /// <summary> 
        ///  Class Name: TimerClass  <br/><br/> 
        ///
        ///  Description: Updates the last received packet time and resets the consecutiveEventsFired.
        ///               This is called after every packet is received. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  DateTime <paramref name="timeStamp"/> - The timestamp of the newly received packet. <br/><br/> 
        ///  
        /// Returns: None 
        /// </summary>
        public void UpdateLastReceivedTime(DateTime timeStamp)
        {
            mostRecentTimestamp = timeStamp;
            consecutiveEventsFired = 0;
        }
    }



    /// <summary>
    /// Houses all of the functionality of the packet receiving and sending. 
    /// </summary>
    class RepeaterClass
    {
                /// <summary> A Backend object that we use to get the configuration settings. </summary>
        public static Backend backendObject { get; set; }
                /// <summary> A timerClass object that we use to get the update the last received packet time. </summary>
        public static TimerClass timer { get; set; }
                /// <summary> W </summary>
        private static bool isMulticast { get; set; }


        /// <summary> 
        ///  Class Name: RepeaterClass  <br/> <br/>
        ///
        ///  Description: Sends the processed package out <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  byte[] <paramref name="messageBytes"/> - The byte array payload of the received packet <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public static void SendMessageOut(byte[] messageBytes)
        {
            IPAddress ip = IPAddress.Parse(backendObject.sendIp);
            IPEndPoint endPoint = new IPEndPoint(ip, backendObject.sendPort);

            if ( isMulticast )
            {
                try
                {
                    using (Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                    {
                        s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip));
                        s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 32); 

                        s.Connect(endPoint);

                        s.Send(messageBytes, messageBytes.Length, SocketFlags.None);

                        s.Close();
                    }
                }
                catch (Exception e)
                {
                    Backend.ExceptionLogger(e);
                }
                
            } 
            else
            {
                using (UdpClient sender = new UdpClient())
                {
                    try
                    {
                        sender.Send(messageBytes, messageBytes.Length, endPoint);
                    }
                    catch (Exception e)
                    {
                        Backend.ExceptionLogger(e);
                    }
                    sender.Close();
                }
            }
        }


        /// <summary> 
        ///  Class Name: RepeaterClass  <br/> <br/>
        ///
        ///  Description: Sends packet information to the GUI <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  byte[] <paramref name="messageBytes"/> - The byte array payload of the received packet <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public static void SendToGUI(byte[] messageBytes)
        {
            using (UdpClient sender = new UdpClient())
            {
                try
                {
                    string receiveIp = backendObject.receiveIp;
                    string receivePort = backendObject.receivePort.ToString();
                    string dataLength = messageBytes.Length.ToString();

                    byte[] bytes = Encoding.ASCII.GetBytes(receiveIp + "," + receivePort + "," + dataLength);


                    sender.Send(bytes, bytes.Length, "127.0.0.1", 50000);

                }
                catch (Exception e)
                {
                    Backend.ExceptionLogger(e);
                }
                sender.Close();
            }
        }

        /// <summary> 
        ///  Class Name: RepeaterClass  <br/><br/>
        ///
        ///  Description: Continually listens for packets in promiscuous mode. Whenver a packet is received, <br/>
        ///  a packet is sent out to the target machine, and another one is sent to the GUI. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  CancellationToken <paramref name="token"/> - A token that signal a configuration change was made, so this task need to restart. <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public static void StartReceiver(CancellationToken token)
        {

            Thread.Sleep(1000);     // This HAS TO STAY or else the old port won't be closed by the time this runs

            var devices = CaptureDeviceList.Instance;

                // If no devices were found, log an error
            if (devices.Count < 1)
            {
                Backend.ExceptionLogger(new Exception("No network devices found on current machine."));
                return;
            }

                // gets the ethernet device
            var device = devices.FirstOrDefault(dev => dev.Description.Contains("Ethernet Connection"));


                // Register our handler function to the 'packet arrival' event
            device.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrival);

                // Open the device for capturing
            int readTimeoutMilliseconds = 1000;
            device.Open(DeviceModes.Promiscuous, readTimeoutMilliseconds);

                // filters for out listening port
            device.Filter = String.Format("udp port {0}", backendObject.receivePort);

            Thread captureThread = new Thread(() =>
            {
                // Start the capturing process
                device.StartCapture();

                try
                {
                    while (!token.IsCancellationRequested)
                    {
                        Thread.Sleep(1000);
                    }
                }
                finally
                {
                    device.StopCapture();
                    device.Close();
                }
            });

            captureThread.Start();
            captureThread.Join();

            return;
        }

        /// <summary> 
        ///  Class Name: RepeaterClass  <br/><br/>
        ///
        ///  Description: Continually listens for packets in promiscuous mode. Whenver a packet is received, <br/>
        ///  a packet is sent out to the target machine, and another one is sent to the GUI. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  object <paramref name="sender"/> - The sender oject, I don't us it. <br/>
        ///  PacketCapture <paramref name="e"/> - The captured packet. <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        private static void device_OnPacketArrival(object sender, PacketCapture e)
        {
            RawCapture rawPacket = e.GetPacket();
            byte[] payload = rawPacket.Data;

                // actual sending section
            SendMessageOut(payload);

                // sending to GUI section
            SendToGUI(payload);

                // update last received
            timer.UpdateLastReceivedTime(DateTime.Now);
        }


        /// <summary> 
        ///  Class Name: RepeaterClass  <br/><br/>
        ///
        ///  Description: Checks if supplied ip is multicast or not. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  string <paramref name="ip"/> - The ip to check. <br/><br/>
        ///  
        ///  Returns:  bool - whether the ip is multicast or not. 
        /// </summary>
        public static bool IsMulticastSetter(string ip)
        {
            string[] split = ip.Split('.');
            int firstOctect = int.Parse(split[0]);
            if (firstOctect >= 224 && firstOctect <= 239)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary> 
        ///  Class Name: RepeaterClass  <br/><br/> 
        ///
        ///  Description: The main function of the RepeaterClass. Starts and runs the StartReceiver funtion <br/>
        ///  in it's own task. Also initializes the backendOjbect and timer data members. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  Backend <paramref name="BackendObject"/> - The Backend object to supply confiiguraiton information <br/>
        ///  CancellationToken <paramref name="token"/> - A token that signals a configuration change was made, so this task needs to restart. <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public async static void main(Backend BackendObject, CancellationToken token)
        {
            backendObject = BackendObject;

            isMulticast = IsMulticastSetter(backendObject.sendIp);

            timer = new TimerClass(BackendObject);

            Task.Run(() => StartReceiver(token));

            return;
        }
    }
}