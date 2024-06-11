//----------------------------------------------------
// File Name: JsonData.cs
// 
// Description: This file contains all the methods for
//              the JsonData class. It also includes the
//              method for logging errors.
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

using System;
using System.Diagnostics;
using System.Threading;


    
namespace JsonDataNameSpace
{
    /// <summary>
    /// An oject that contains the configuration settings to be passed between 
    /// the functions that need it. It is updated as settings change. This class
    /// also houses the function that logs exceptions. 
    /// </summary>
    public class JsonData
    {
            /// <summary> The IP being listened to</summary>
        public string receiveIp { get; set; }
            /// <summary> The Port being listened to</summary>
        public int receivePort { get; set; }
            /// <summary> The IP being sent to</summary>
        public string sendIp { get; set; }
            /// <summary> The Port being sent to</summary>
        public int sendPort { get; set; }


        /// <summary> 
        ///  Class Name: JsonData  <br/><br/> 
        ///
        ///  Description: The JsonData Constructor <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  string <paramref name="receiveIp"/> - The IP being listened to. <br/>
        ///  string <paramref name="receivePort"/> - The Port being listened to. <br/>
        ///  string <paramref name="sendIp"/> - The IP being sent to. <br/>
        ///  string <paramref name="receivePort"/> - The Port being sent to. <br/><br/>
        ///                         
        /// </summary>
        public JsonData(string ReceiveIp, string ReceivePort, string SendIp, string SendPort)
        {
            this.receiveIp = ReceiveIp;
            this.receivePort = Convert.ToInt32(ReceivePort);
            this.sendIp = SendIp;
            this.sendPort = Convert.ToInt32(SendPort);
        }


        /// <summary> 
        ///  Class Name: JsonData  <br/><br/> 
        ///
        ///  Description: Returns if two JsonData objects have identical attributes <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  JsonData <paramref name="other"/> - The other JsonData object to compare with <br/><br/>
        ///  
        ///  Returns:  bool – Whether the two objects are equal.
        /// </summary>
        public bool Equals(JsonData other)
        {       
            return other != null &&
                   receiveIp == other.receiveIp &&
                   receivePort == other.receivePort &&
                   sendIp == other.sendIp &&
                   sendPort == other.sendPort;
        }

        /// <summary> 
        ///  Class Name: JsonData  <br/> <br/>
        ///
        ///  Description: Logs Exceptions into a Windows Event Log <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  Exception <paramref name="e"/> - An Exception to be logged <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public static void Logger(Exception e)
        {
                // Create the source and log, if it does not already exist.
            if (!EventLog.SourceExists("UDP_Repeater_Backend"))
            {
                EventLog.CreateEventSource("UDP_Repeater_Backend", "UDP Packet Repeater");
            }
                // Create an EventLog instance and assign its source.
            EventLog eventLog = new EventLog();
            Thread.Sleep(1000);     // this makes sure the Event log is created before being written to
            eventLog.Source = "UDP_Repeater_Backend";
                // Write an entry to the event log.
            eventLog.WriteEntry(e.ToString(), EventLogEntryType.Error, 722);
        }
    }


}

