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
using System.Linq;



namespace UDP_Repeater_GUI
{
    /// <summary>
    /// This class handles all of the logging for the front end.
    /// </summary>
    internal class Logger
    {
        public EventLog eventLog;

        public Logger()
        {
                // Create an EventLog instance and assign its source.
            eventLog = new EventLog();
            eventLog.Source = "UDP_Repeater_Frontend";
        }

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
        public void LogException(Exception e)
        {
            string[] formattedStackString = e.StackTrace.Split('\n');

            string message = String.Format($"Error Message: {e.Message} \n" +
                                           $"Error location: Frontend/User Interface \n" + 
                                           $"{formattedStackString.Last().TrimStart()}");

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
        public void LogConfigChange(string editType, string ip, string port)
        {
            string message = String.Format("The \"{0}\" settings were changed. \n" +
                                            "IP Address: {1}    " +
                                            "Port: {2}", editType, ip, port);

            eventLog.WriteEntry(message, EventLogEntryType.Information, 6);  // 6 is id for ip/port config change
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
        public void LogInactivityChange(int frequency, string interval)
        {
            string message = String.Format("The \"Inactivity\" settings were changed. \n" +
                                           "Frequency: {0}    " +
                                           "Interval: {1}", frequency, interval);

            eventLog.WriteEntry(message, EventLogEntryType.Information, 7);  // 7 is an inactivity config change
        }

        /// <summary> 
        ///  Class Name: Logger  <br/><br/>
        ///
        ///  Description: Logs NIC changes into the event log. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  string <paramref name="description"/> - The new NIC's description. <br/>
        ///  string <paramref name="type"/> - The new NIC's interface type. <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public void LogNicChange(string description, string macAddress)
        {
            string message = String.Format("The Network Interface Listening Card was changed. \n" +
                                           "Description: {0}    " +
                                           "Mac Address: {1}", description, macAddress);

            eventLog.WriteEntry(message, EventLogEntryType.Information, 8);  // 8 is a NIC config change
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
        public void StartStopLogger(string mode)
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

            eventLog.WriteEntry(message, EventLogEntryType.Information, 5);     // 5 is id for frontend start/stop
        }
    }
}
