//----------------------------------------------------
// File Name: Backend.cs
// 
// Description: This file contains all the methods for
//              the Backend class. Which includes all
//              the configuration settings as data members.
//              It also includes the methods for logging errors.
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

using System;
using System.Diagnostics;
using System.Threading;


    
namespace BackendClassNameSpace
{
    /// <summary>
    /// An oject that contains all of the configuration settings to be passed between 
    /// the functions that need it. It is updated as settings change. This class
    /// also houses the function that logs exceptions. 
    /// </summary>
    public class Backend
    {
            /// <summary> The IP being listened to</summary>
        public string receiveIp { get; set; }
            /// <summary> The Port being listened to</summary>
        public int receivePort { get; set; }
            /// <summary> The IP being sent to</summary>
        public string sendIp { get; set; }
            /// <summary> The Port being sent to</summary>
        public int sendPort { get; set; }

            /// <summary> The Frequency (number) at which the service reports inactivity</summary>
        public int frequency { get; set; }
            /// <summary> The Interval (minute, day, hour) at which the service reports inactivity</summary>
        public string interval { get; set; }


        /// <summary> 
        ///  Class Name: Backend  <br/><br/> 
        ///
        ///  Description: The Backend Constructor <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  string <paramref name="ReceiveIp"/> - The IP being listened to. <br/>
        ///  string <paramref name="ReceivePort"/> - The Port being listened to. <br/>
        ///  string <paramref name="SendIp"/> - The IP being sent to. <br/>
        ///  string <paramref name="SendPort"/> - The Port being sent to. <br/>
        ///  int    <paramref name="newFrequency"/> - The Frequency (number) at which the service reports inactivity <br/>
        ///  string <paramref name="newInterval"/> - The Interval (minute, day, hour) at which the service reports inactivity <br/><br/> 
        ///  
        /// Returns: A Backend Object
        /// </summary>
        public Backend(string ReceiveIp, string ReceivePort, string SendIp, string SendPort, int newFrequency, string newInterval)
        {
            this.receiveIp = ReceiveIp;
            this.receivePort = Convert.ToInt32(ReceivePort);
            this.sendIp = SendIp;
            this.sendPort = Convert.ToInt32(SendPort);
            this.frequency = newFrequency;
            this.interval = newInterval;
        }


        /// <summary> 
        ///  Class Name: Backend  <br/><br/> 
        ///
        ///  Description: Returns if two Backend objects have identical attributes <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  Backend <paramref name="other"/> - The other Backend object to compare with <br/><br/>
        ///  
        ///  Returns:  bool – Whether the two objects are equal.
        /// </summary>
        public bool Equals(Backend other)
        {       
            return other != null &&
                   receiveIp == other.receiveIp &&
                   receivePort == other.receivePort &&
                   sendIp == other.sendIp &&
                   sendPort == other.sendPort &&
                   frequency == other.frequency &&
                   interval == other.interval;
        }

        /// <summary> 
        ///  Class Name: Backend  <br/> <br/>
        ///
        ///  Description: Logs Exceptions into the "UDP Packet Repeater" Windows Event Log <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  Exception <paramref name="e"/> - An Exception to be logged <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public static void ExceptionLogger(Exception e)
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


            // Get stack trace for the exception with source file information
            var st = new StackTrace(e, true);
            // Get the top stack frame
            var frame = st.GetFrame(0);
            // Get the line number from the stack frame
            string lineNum = frame.GetFileLineNumber().ToString();
            string fileName = frame.GetFileName();

            string message = String.Format($"{e.Message} At line {lineNum} in {fileName}");

            // Write an entry to the event log.
            eventLog.WriteEntry(message, EventLogEntryType.Error);
        }


        /// <summary> 
        ///  Class Name: Backend  <br/><br/>
        ///
        ///  Description: Logs Inactive Periods into the "UDP Packet Repeater" Windows Event Log <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  int <paramref name="consecutiveEvents"/> - How many consecutive event have fired. <br/>
        ///  int <paramref name="frequency"/> - The Backend object's frequency field <br/>
        ///  string <paramref name="interval"/> - The Backend object's interval field <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public static void InactivityLogger(int consecutiveEvents, int frequency, string interval)
        {
            // Create the source and log, if it does not already exist.
            if (!EventLog.SourceExists("UDP_Repeater_Backend"))
            {
                EventLog.CreateEventSource("UDP_Repeater_Backend", "UDP Packet Repeater");
            }
            // Create an EventLog instance and assign its source.
            EventLog eventLog = new EventLog();
            Thread.Sleep(100);     // this makes sure the Event log is created before being written to
            eventLog.Source = "UDP_Repeater_Backend";


            // this whole section is just to find if the interval word in the log message should have an 's' or not 
            // this has to be so much more complicated than it should be
            int totalTime = consecutiveEvents * frequency;
            string message;
            interval += "s";

            if (totalTime > 1 && frequency > 1)
            {
                message = string.Format("It has been {0} {1} since last packet was received. ", totalTime, interval);
                message += string.Format("The Service is currenty configured to log inactivity every {0} {1}.", frequency, interval);
            }
            else if (totalTime > 1 && frequency == 1)
            {
                message = string.Format("It has been {0} {1} since last packet was received. ", totalTime, interval);
                interval = interval.Remove(interval.Length - 1);
                message += string.Format("The Service is currenty configured to log inactivity every {0} {1}.", frequency, interval);
            }
            else
            {
                interval = interval.Remove(interval.Length - 1);
                message = string.Format("It has been {0} {1} since last packet was received. ", totalTime, interval);
                message += string.Format("The Service is currenty configured to log inactivity every {0} {1}.", frequency, interval);
            }

            // Write an entry to the event log.
            eventLog.WriteEntry(message, EventLogEntryType.Warning);
        }
    }
}

