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
// Version  Date        Author           Description
// 1.0      ---     Jade Pace Thomson    Initial Release
//---------------------------------------------------



using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Text;
using BackendClassNameSpace;
using System;
using System.Timers;


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

                Console.WriteLine("\n\n Logged a new Inactive Period at {0}\n consecutiveEventsFired = {1}\n", DateTime.Now, consecutiveEventsFired);
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
        /// <summary> 
        ///  Class Name: RepeaterClass  <br/> <br/>
        ///
        ///  Description: Sends the processed package out <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  byte[] <paramref name="messageBytes"/> - The byte array payload of the received packet <br/>
        ///  Backend <paramref name="backendObject"/> - The Backend object to supply our send IP and Port <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public static void SendMessageOut(byte[] messageBytes, Backend backendObject)
        {
            using (UdpClient sender = new UdpClient())
            {
                try
                {
                    sender.Send(messageBytes, messageBytes.Length, backendObject.sendIp, backendObject.sendPort);
                }
                catch (Exception e)
                {
                    Backend.Logger(e);
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
                    Backend.Logger(e);
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
        ///  Backend <paramref name="backendObject"/> - The Backend object to supply our configuration information <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public static void SendToGUI(byte[] messageBytes, Backend backendObject)
        {
            using (UdpClient sender = new UdpClient())
            {
                try
                {
                    string ipSentTo = backendObject.sendIp;
                    string portSentTo = backendObject.sendPort.ToString();
                    string dataLength = messageBytes.Length.ToString();

                    byte[] bytes = Encoding.ASCII.GetBytes(ipSentTo + "," + portSentTo + "," + dataLength);


                    sender.Send(bytes, bytes.Length, "127.0.0.1", 50000);

                }
                catch (Exception e)
                {
                    Backend.Logger(e);
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
        ///  Backend <paramref name="backendObject"/> - The Backend object to supply IP and Port Configurations. <br/>
        ///  TimerClass <paramref name="timer"/> - The timer oject, so that we can update it when we send new packets. <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public static void StartReceiver(Backend backendObject, CancellationToken token, TimerClass timer)
        {
            Thread.Sleep(1000);     // This HAS TO STAY or else the old port won't be closed by the time this runs

            UdpClient listener = new UdpClient(backendObject.receivePort);
            IPEndPoint senderEndPoint = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                while (true)
                {
                            // checks each time to make sure calling function hasn't told this one to stop
                    if (token.IsCancellationRequested)      
                    {
                        listener.Close();
                        return;
                    }

                    byte[] messageBytes = listener.Receive(ref senderEndPoint);

                            // actual sending section
                    SendMessageOut(messageBytes, backendObject);

                            // sending to GUI section
                    SendToGUI(messageBytes, backendObject);

                            // updates our most received packet time 
                    timer.UpdateLastReceivedTime(DateTime.Now);

                }
            }
            catch (Exception e)
            {
                Backend.Logger(e);
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
        ///  Backend <paramref name="backendObject"/> - The Backend object to supply confiiguraiton information <br/>
        ///  CancellationToken <paramref name="token"/> - A token that signal a configuration change was made, so this task need to restart. <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public async static void main(Backend backendObject, CancellationToken token)
        {
            TimerClass timer = new TimerClass(backendObject);

            Task.Run(() => StartReceiver(backendObject, token, timer));

            Thread.Sleep(500);
            Thread test_sender = new Thread(() => {
                for (int i = 0; i < 20; i++)
                {
                    if (token.IsCancellationRequested)
                    {
                        Backend.Logger(new Exception("Cancellation Requested"));
                        break;
                    }
                    TestingSender();
                    Thread.Sleep(1000);
                }
            });
            test_sender.Start();

            return;
        }
    }
}