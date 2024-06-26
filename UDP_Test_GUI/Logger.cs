//----------------------------------------------------
// File Name: Logger.cs
// 
// Description: This file contains all the methods for
//              the Logger class. Which is the three methods
//              logging from the front end.
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


using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;


namespace UDP_Repeater_GUI
{
    /// <summary>
    /// This class handles all of the logging for the front end.
    /// </summary>
    internal class Logger
    {
        /// <summary> 
        ///  Class Name: Logger  <br/> <br/>
        ///
        ///  Description: Logs all of the GUI's Exceptions into the event log. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  Exception <paramref name="e"/> - An Exception to be logged <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public static void LogException(Exception e)
        {
            // Create an EventLog instance and assign its source.
            EventLog eventLog = new EventLog();
            eventLog.Source = "UDP_Repeater_Frontend";

            // Get stack trace for the exception with source file information
            var st = new StackTrace(e, true);
            // Get the top stack frame
            var frame = st.GetFrame(0);
            // Get the line number from the stack frame
            string fileName = frame.GetFileName();

            string message = String.Format($"{e.Message} in {fileName} of source code. This " +
                                           $"is an error in the frontend/user interface.");

            // Write an entry to the event log.
            eventLog.WriteEntry(message, EventLogEntryType.Error, 2);       // 2 is id for frontend errors
        }


        /// <summary> 
        ///  Class Name: Logger  <br/><br/>
        ///
        ///  Description: Logs IP/Port changes into the event log. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  string <paramref name="editType"/> - What profile was changed. <br/>
        ///  string <paramref name="ip"/> - The new IP Address. <br/>
        ///  string <paramref name="port"/> - The new Port. <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public static void LogConfigChange(string editType, string ip, string port)
        {
                // Create an EventLog instance and assign its source.
            EventLog eventLog = new EventLog();
            eventLog.Source = "UDP_Repeater_Frontend";


            string message = String.Format("The {0} settings were changed. New settings - " +
                                "IP Address: {1} and Port: {2}", editType, ip, port);

            string entry = "IP/Port Change" + ",\t" + message + ",\t" + DateTime.Now.ToString();

            eventLog.WriteEntry(entry, EventLogEntryType.Information, 6);  // 6 is id for ip/port config change
        }


        /// <summary> 
        ///  Class Name: Logger  <br/><br/>
        ///
        ///  Description: Logs Inactivity settings changes into the event log. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  int <paramref name="frequency"/> - The new frequency value. <br/>
        ///  string <paramref name="interval"/> - The new interval value. <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public static void LogInactivityChange(int frequency, string interval)
        {
            // Create an EventLog instance and assign its source.
            EventLog eventLog = new EventLog();
            eventLog.Source = "UDP_Repeater_Frontend";


            string message = String.Format("The Inactivity settings were changed. New settings - " +
                                                "Frequency: {0} and Interval: {1}", frequency, interval);

            string entry = "Inactivity Settings Change" + ",\t" + message + ",\t" + DateTime.Now.ToString();

            eventLog.WriteEntry(entry, EventLogEntryType.Information, 7);  // 7 is an inactivity config change
        }


        /// <summary> 
        ///  Class Name: Logger  <br/><br/>
        ///
        ///  Description: Logs whever the front end starts or stops. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  string <paramref name="mode"/> - If it's a start or stop being logged, mode can only be "start"
        ///                                   or "stop". <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public static void StartStopLogger(string mode)
        {
            string message = "";
            if (mode == "start")
            {
                message = String.Format("User Interface started.");
            }                                         
            else if (mode == "stop")                  
            {                                         
                message = String.Format("User Interface stopped.");
            }

            // Create an EventLog instance and assign its source.
            EventLog eventLog = new EventLog();
            eventLog.Source = "UDP_Repeater_Frontend";


            eventLog.WriteEntry(message, EventLogEntryType.Information, 5);     // 5 is id for frontend start/stop
        }
    }
}
