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
// Version   Date          Author            Description
//   1.0    8/3/24    Jade Pace Thomson     Initial Release
//---------------------------------------------------



using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using BackendClassNameSpace;
using System.Timers;
using System.Net;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using GUIreceiver;
using Newtonsoft.Json.Linq;


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
                /// <summary> A Backend object that we use to get the inactivity settings and logging. </summary>
        private static Backend backendObject;


        /// <summary> 
        ///  Class Name: TimerClass  <br/><br/> 
        ///
        ///  Description: The TimerClass object Constructor <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  Backend <paramref name="BackendObject"/> - The Backend object that we use to get 
        ///                                             the inactivity settings and logging. <br/><br/> 
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

                double intervalDouble = Convert.ToDouble(backendObject.inactivityInterval);
                switch (backendObject.inactivityUnit)
                {
                    case "minute":
                        timeoutThreshold = TimeSpan.FromMinutes(intervalDouble).TotalSeconds;
                        break;
                    case "hour":
                        timeoutThreshold = TimeSpan.FromHours(intervalDouble).TotalSeconds;
                        break;
                    case "day":
                        timeoutThreshold = TimeSpan.FromDays(intervalDouble).TotalSeconds;
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
        ///  Description: Checks if we are in a period of inactivity, and then logs or not. <br/><br/>
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
                // Logs the event with the number of events fired multiplied by the inactivityInterval
                // for example, with 5 minutes as the setting, the 3rd consecutive event would log 15 minutes
                consecutiveEventsFired++;
                mostRecentTimestamp = DateTime.Now;

                backendObject.InactivityLogger(consecutiveEventsFired, backendObject.inactivityInterval, backendObject.inactivityUnit);
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
            /// <summary> Times how long it takes to process packets. </summary>
        public static Stopwatch stopWatch { get; set; }

            /// <summary> Tells us if the send ip is multicast or not. </summary>
        private static bool isMulticast { get; set; }
            /// <summary> The sending endpoint object. </summary>
        public IPEndPoint endPoint { get; set; }
            /// <summary> The multicast sender, which is a Socket object. </summary>
        public Socket multicastSender { get; set; }
            /// <summary> The other sender, which is a UdpClient object. </summary>
        public UdpClient otherSender { get; set; }
            /// <summary> The GUI sender. Handles all of the sending to the GUI. </summary>
        public UdpClient guiSender { get; set; }

            /// <summary> The raw socket that we listen on. </summary>
        private Socket listenerSocket { get; set; }
            /// <summary> The buffer that we put the newly received packets into. </summary>
        private byte[] buffer { get; set; }
            /// <summary> Counts our object disposed frequency in SendToGUI. One can happen 
            /// when we reconfigure. Any more means there is another issue. </summary>
        private int guiSenderDisposedCounter { get; set; }
            /// <summary> Counts our object disposed frequency in SendMessageOut. One can happen 
            /// when we reconfigure. Any more means there is another issue. </summary>
        private int mainSenderDisposedCounter { get; set; }
            /// <summary> In ParsePacket, tells us if we need to filter for remote IP or not. </summary>
        private bool isReceiveIpSet { get; set; }


        /// <summary> 
        ///  Class Name: RepeaterClass  <br/><br/> 
        ///
        ///  Description: The RepeaterClass object Constructor <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  Backend <paramref name="BackendObject"/> - The Backend object that we use to get our sending 
        ///                                             settings and error logging. <br/><br/> 
        /// </summary>
        /// <returns>A RepeaterClass Object</returns>
        public RepeaterClass(Backend BackendObject, CancellationToken originalToken)
        {
            backendObject = BackendObject;
            timer = new TimerClass(BackendObject);
            stopWatch = new Stopwatch();

            isMulticast = IsMulticastSetter();

            guiSender = new UdpClient("127.0.0.1", 56722);

            buffer = new byte[1028];
            guiSenderDisposedCounter = 0;
            mainSenderDisposedCounter = 0;

            if (backendObject.receiveIp == "0.0.0.0") isReceiveIpSet = false;
            else isReceiveIpSet = true;
        }


        /// <summary> 
        ///  Class Name: RepeaterClass  <br/><br/>
        ///
        ///  Description: Checks if supplied ip string is multicast or not, and sets the corrresponding sender. <br/><br/>
        ///
        ///  Inputs: None <br/><br/>
        ///  
        ///  Returns:  bool - whether the ip is multicast or not. 
        /// </summary>
        public bool IsMulticastSetter()
        {
            try
            {
                IPAddress ip = IPAddress.Parse(backendObject.sendIp);
                this.endPoint = new IPEndPoint(ip, backendObject.sendPort);

                string[] split = backendObject.sendIp.Split('.');
                int firstOctect = int.Parse(split[0]);
                if (firstOctect >= 224 && firstOctect <= 239)
                {
                    this.multicastSender = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    this.multicastSender.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip));
                    this.multicastSender.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 16);
                    this.multicastSender.Connect(endPoint);
                    return true;
                }
                else
                {
                    this.otherSender = new UdpClient(backendObject.sendIp, backendObject.sendPort);
                    return false;
                }
            }
            catch (Exception e)
            {
                backendObject.ExceptionLogger(e);
                return false;
            }
        }

        public void CloseAllOurSockets()
        {
            try
            {
                if (multicastSender != null) multicastSender.Dispose();
                if (otherSender != null) otherSender.Dispose();
                if (guiSender != null) guiSender.Dispose();
                if (listenerSocket != null) listenerSocket.Dispose();
            }
            catch (Exception ex) { backendObject.ExceptionLogger(ex); }
        }

        /// <summary> 
        ///  Class Name: RepeaterClass  <br/><br/>
        ///
        ///  Description: Decides if we are sending unicast/broadcast or multicast and sends 
        ///  the packet payload out to the configured sending IP endpoint. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  byte[] <paramref name="messageBytes"/> - The payload of the received packet <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public void SendMessageOut(byte[] messageBytes)
        {
            try
            {
                if (isMulticast)
                {
                    multicastSender.Send(messageBytes, messageBytes.Length, SocketFlags.None);
                }
                else
                {
                    otherSender.Send(messageBytes, messageBytes.Length);
                }
            }
            catch (ObjectDisposedException) when (mainSenderDisposedCounter == 0)
            {
                    // This gets thrown when the cancellation token pops and the sockets get 
                    // closed but it still trys to send it's last packet
                mainSenderDisposedCounter++;
            }
            catch (Exception ex)
            {
                backendObject.ExceptionLogger(ex);
            }
        }

        /// <summary> 
        ///  Class Name: RepeaterClass  <br/> <br/>
        ///
        ///  Description: Sends packet information to the GUI. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  int <paramref name="payloadLength"/> - The length of the received packet's payload.s <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public void SendToGUI(int udpHeaderOffset, int payloadLength)
        {
            try
            {
                string sourceIP = $"{buffer[12]}.{buffer[13]}.{buffer[14]}.{buffer[15]}";
                string sourcePort = ((buffer[udpHeaderOffset] << 8) | buffer[udpHeaderOffset + 1]).ToString();
                string dataLength = payloadLength.ToString();

                byte[] bytes = Encoding.ASCII.GetBytes($"{sourceIP},{sourcePort},{dataLength}");

                guiSender.Send(bytes, bytes.Length);
            }
            catch (ObjectDisposedException) when (guiSenderDisposedCounter == 0)
            {
                    // This gets thrown when the cancellation token pops and the sockets get 
                    // closed but it still trys to send it's last packet
                guiSenderDisposedCounter++;
            }
            catch (Exception e)
            {
                backendObject.ExceptionLogger(e);
            }
        }

        /// <summary> 
        ///  Class Name: RepeaterClass  <br/><br/>
        ///
        ///  Description: Gets called if backendObject.ipAddressOfNIC isn't valid. It finds and sets the first valid <br/>
        ///  NIC to be the one we use for listening, and logs a warning. <br/><br/>
        ///
        ///  Inputs: None <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public void HandleBadNicIP()
        {
            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (var nic in networkInterface.GetIPProperties().UnicastAddresses)
                {
                    if (nic.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                            // we choose the first valid option and use that
                        listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Udp);
                        listenerSocket.Bind(new IPEndPoint(nic.Address, backendObject.receivePort));
                        backendObject.WarningLogger($"Invalid listening IP Address: {backendObject.ipAddressOfNIC}, currently listening on default NIC \n" +
                                                    $"Name: {networkInterface.Name} \n" +
                                                    $"IP Address: {nic.Address}");

                        backendObject.ipAddressOfNIC = nic.Address.ToString();

                        return;
                    }
                }
            }
        }


        /// <summary> 
        ///  Class Name: RepeaterClass  <br/><br/>
        ///
        ///  Description: Intializes the listening socket and begins the listening task. <br/>
        ///  This also registers the TaskCompletionSource and waits for it to know when we're reconfiguring <br/>
        ///  so we can stop listening and close the sockets. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  CancellationToken <paramref name="token"/> - A token that signals a configuration change was made, so this task need to end. <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public async void SetupAndStartListener(CancellationToken token)
        {
            try
            {
                try
                {
                    listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Udp);
                    listenerSocket.Bind(new IPEndPoint(IPAddress.Parse(backendObject.ipAddressOfNIC), backendObject.receivePort));
                }
                catch (FormatException)     // This gets thrown if the IP doesn't match one found on the machine's nics, so we set a default
                {
                    HandleBadNicIP();
                }

                listenerSocket.IOControl(IOControlCode.ReceiveAll, BitConverter.GetBytes(1), null);

                    // When the cancellation token pops, we close all our sockets
                token.Register(() => CloseAllOurSockets());

                try
                {
                    while (true)
                    {
                        listenerSocket.Receive(buffer);
                        Task.Run(() => PacketReceivedCallback());
                    }
                }
                catch (SocketException ex) when (ex.SocketErrorCode == SocketError.Interrupted)
                {
                        // This gets thrown because we dispose of the listening socket while 
                        // it's still listening to cancel the receive and get out of the loop
                    return;
                }
            }
            catch (Exception ex)
            {
                backendObject.ExceptionLogger(ex);
            }
        }


        /// <summary> 
        ///  Class Name: RepeaterClass  <br/><br/>
        ///
        ///  Description: Gets the payload from the buffer and sends it to the target ip endpoint and it's  <br/>
        ///  information to the GUI. This also times the packet handling time and reports it, increments total  <br/>
        ///  packets handled, and updates the last received packet time.  <br/><br/>
        ///
        ///  Inputs: None <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        private void PacketReceivedCallback()
        {
            try
            {
                stopWatch.Restart();

                int udpHeaderOffset = (buffer[0] & 0x0F) * 4;
                byte[] udpPayload = ParsePacket(udpHeaderOffset);

                if (udpPayload != null)     // if udpPayload is null, that means the src ip and dest port didn't match our receive from config
                {
                        // Send the paylod out
                    SendMessageOut(udpPayload);

                        // stop the stopwatch, time the packet handling perfomance and record it
                    stopWatch.Stop();
                    backendObject.AddNewPacketTimeHandled(stopWatch.Elapsed.TotalMilliseconds * 5);
                    stopWatch.Reset();

                        // Send the source IP and payload length
                    SendToGUI(udpHeaderOffset, udpPayload.Length);

                        // update last received packet time for inactivity checker
                    timer.UpdateLastReceivedTime(DateTime.Now);
                         
                        // increment the packets received counter for prometheus metric
                    backendObject.IncrementTotalPacketsHandled();
                }
            }
            catch (Exception ex)
            {
                backendObject.ExceptionLogger(ex);
            }
        }
        /// <summary> 
        ///  Class Name: RepeaterClass  <br/><br/>
        ///
        ///  Description: First gets the source IP and destination port and checks if they are addressed correctly. If <br/>
        ///  they aren't this, returns null. Otherwise, it gets the packet payload as a byte[] and returns it. <br/><br/>
        ///
        ///  Inputs: None <br/><br/>
        ///     
        /// </summary>
        /// <returns> byte[]? - The packet's payload. This returns null if the packet wasn't for the right endpoint. </returns> 
        private byte[] ParsePacket(int udpHeaderOffset)
        {
            // Parse the IP header to find the source IP and destination port
            int destinationPort = (buffer[udpHeaderOffset + 2] << 8) | buffer[udpHeaderOffset + 3];

            if (isReceiveIpSet)
            {
                string sourceIP = $"{buffer[12]}.{buffer[13]}.{buffer[14]}.{buffer[15]}";
                if (sourceIP != backendObject.receiveIp || destinationPort != backendObject.receivePort)
                {
                    return null;
                }
            }
            else
            {
                if (destinationPort != backendObject.receivePort)
                {
                    return null;
                }
            }

            int udpLength = (buffer[udpHeaderOffset + 4] << 8) | buffer[udpHeaderOffset + 5];

                // Extract the payload offset and length
            int udpPayloadOffset = udpHeaderOffset + 8;
            int udpPayloadLength = udpLength - 8;

                // Copy the payload out of the buffer and return it
            byte[] udpPayload = new byte[udpPayloadLength];
            Array.Copy(buffer, udpPayloadOffset, udpPayload, 0, udpPayloadLength);
            return udpPayload;
        }


        /// <summary> 
        /// Class Name: RepeaterClass  <br/><br/> 
        ///
        /// Description: The main function of the RepeaterClass. Starts and runs the StartReceiver funtion <br/>
        /// in it's own task. Also initializes the backendOjbect and timer fields. <br/><br/>
        ///
        /// Inputs:  <br/>
        /// Backend <paramref name="BackendObject"/> - The Backend object to supply configuraiton information <br/>
        /// CancellationToken <paramref name="token"/> - A token that signals a configuration change was made, so this task needs to restart. <br/><br/>
        /// 
        /// Returns:  None
        /// </summary>
        public static void main(Backend BackendObject, CancellationToken token)
        {
            try
            {
                RepeaterClass repeaterObject = new RepeaterClass(BackendObject, token);

                repeaterObject.SetupAndStartListener(token);

                return;
            }
            catch (Exception e)
            {
                BackendObject.ExceptionLogger(e);
            }
        }
    }
}
