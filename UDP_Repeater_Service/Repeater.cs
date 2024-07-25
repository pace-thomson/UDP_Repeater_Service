﻿//----------------------------------------------------
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
//   1.0    7/3/24    Jade Pace Thomson   Initial Release
//---------------------------------------------------



using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Text;
using BackendClassNameSpace;
using System.Timers;
using SharpPcap;
using System.Net;
using System.Diagnostics;


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
                /// <summary> The specified time between packets, in seconds </summary>
        public static double timeoutThreshold;
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
            try
            {
                timer = new System.Timers.Timer(checkInterval);
                timer.Elapsed += OnTimerFiring;

                timer.AutoReset = true;  
                timer.Enabled = true;

                backendObject = BackendObject;

                mostRecentTimestamp = DateTime.Now;

                double frequencyDouble = Convert.ToDouble(backendObject.frequency);
                switch (backendObject.interval)
                {
                    case "minute":
                        timeoutThreshold = TimeSpan.FromMinutes(frequencyDouble).TotalSeconds;
                        break;
                    case "hour":
                        timeoutThreshold = TimeSpan.FromHours(frequencyDouble).TotalSeconds;
                        break;
                    case "day":
                        timeoutThreshold = TimeSpan.FromDays(frequencyDouble).TotalSeconds;
                        break;
                }
            }
            catch (Exception e)
            {
                BackendObject.ExceptionLogger(e);
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
        public static void OnTimerFiring(Object source, ElapsedEventArgs e)
        {
            if ((DateTime.Now - mostRecentTimestamp).TotalSeconds > timeoutThreshold)
            {
                // Logs the event with the number of events fired multiplied by the frequency
                // for example, with 5 minutes as the setting, the 3rd consecutive event would log 15 minutes
                consecutiveEventsFired++;
                mostRecentTimestamp = DateTime.Now;

                backendObject.InactivityLogger(consecutiveEventsFired, backendObject.frequency, backendObject.interval);
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
                /// <summary> Tells us if the send ip is multicast or not. </summary>
        private static bool isMulticast { get; set; }
                /// <summary> Times how long it takes to process packets. </summary>
        public static Stopwatch stopWatch { get; set; }


        /// <summary> 
        ///  Class Name: RepeaterClass  <br/> <br/>
        ///
        ///  Description: Sends the processed package out <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  byte[] <paramref name="messageBytes"/> - The byte array of the received packet <br/><br/>
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
                        s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 16); 

                        s.Connect(endPoint);

                        s.Send(messageBytes, messageBytes.Length, SocketFlags.None);

                        s.Close();
                    }
                }
                catch (Exception e)
                {
                    backendObject.ExceptionLogger(e);
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
                        backendObject.ExceptionLogger(e);
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
        ///  int <paramref name="payloadLength"/> - The length of the received packet's payload.s <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public static void SendToGUI(int payloadLength)
        {
            using (UdpClient sender = new UdpClient())
            {
                try
                {
                    string receiveIp = backendObject.receiveIp;
                    string receivePort = backendObject.receivePort.ToString();
                    string dataLength = payloadLength.ToString();

                    byte[] bytes = Encoding.ASCII.GetBytes(receiveIp + "," + receivePort + "," + dataLength);


                    sender.Send(bytes, bytes.Length, "127.0.0.1", 50000);

                }
                catch (Exception e)
                {
                    backendObject.ExceptionLogger(e);
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
        public static async void StartReceiver(CancellationToken token)
        {
            try
            {
                var tcs = new TaskCompletionSource<bool>();

                token.Register(() =>
                {
                    tcs.SetResult(true);
                });

                CaptureDeviceList devices = CaptureDeviceList.Instance;
                ICaptureDevice device = null;
                foreach (var dev in devices)
                {
                    if (dev.Description == backendObject.descriptionOfNIC)
                    {
                        device = dev;
                        break;
                    }
                }

                if (device == null)
                {
                    device = devices[0];
                    backendObject.WarningLogger($"No matching network device was found, {device.Description} is being used. " +
                                                "Ignore this warning if the system was just installed.");
                }

                    // Register our handler function to the 'packet arrival' event
                device.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrival);

                    // Open the device for capturing
                int readTimeoutMilliseconds = 1000;
                device.Open(DeviceModes.Promiscuous, readTimeoutMilliseconds);

                    // filters for out listening port
                device.Filter = $"udp port {backendObject.receivePort}";


                device.StartCapture();

                await tcs.Task;

                device.StopCapture();
                device.Close();

                return;
            }
            catch (Exception e)
            {
                backendObject.ExceptionLogger(e);
            }
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
            try
            {       // start timing for packet ingress/egress
                stopWatch.Start();

                    // get the whole packet
                RawCapture rawPacket = e.GetPacket();
                byte[] wholePacket = rawPacket.Data;

                    // get the actual data section of the packet
                byte[] dataSection = new byte[wholePacket.Length - 42];
                Array.Copy(wholePacket, 42, dataSection, 0, dataSection.Length);

                    // actual sending section
                SendMessageOut(dataSection);

                    // stop the stopwatch, time the packet handling perfomance and record it
                stopWatch.Stop();
                double ticks = (double)stopWatch.ElapsedTicks;
                backendObject.AddNewPacketTimeHandled(1000 * ticks / Stopwatch.Frequency);
                stopWatch.Reset();

                    // sending to GUI section
                SendToGUI(dataSection.Length);

                    // update last received packet time for inactivity checker
                timer.UpdateLastReceivedTime(DateTime.Now);

                    // increment the packets received counter for prometheus
                backendObject.IncrementTotalPacketsHandled();
            }
            catch (Exception ex)
            {
                backendObject.ExceptionLogger(ex);
            }
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
            try
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
            catch (Exception e)
            {
                backendObject.ExceptionLogger(e);
                return false;
            }
        }

        /// <summary> 
        ///  Class Name: RepeaterClass  <br/><br/> 
        ///
        ///  Description: The main function of the RepeaterClass. Starts and runs the StartReceiver funtion <br/>
        ///  in it's own task. Also initializes the backendOjbect and timer fields. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  Backend <paramref name="BackendObject"/> - The Backend object to supply confiiguraiton information <br/>
        ///  CancellationToken <paramref name="token"/> - A token that signals a configuration change was made, so this task needs to restart. <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public async static void main(Backend BackendObject, CancellationToken token)
        {
            try
            {
                backendObject = BackendObject;

                isMulticast = IsMulticastSetter(backendObject.sendIp);

                timer = new TimerClass(BackendObject);

                stopWatch = new Stopwatch();

                StartReceiver(token);

                return;
            }
            catch (Exception e)
            {
                backendObject.ExceptionLogger(e);
            }
        }
    }
}