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
// Version  Date        Author           Description
// 1.0      ---     Jade Pace Thomson    Initial Release
//---------------------------------------------------


using System;
using System.IO;
using System.Linq;


namespace UDP_Test_GUI
{
    /// <summary>
    /// This class handles all of the logging for the front end.
    /// </summary>
    internal class Logger
    {
        /// <summary> 
        ///  Class Name: Logger  <br/><br/>
        ///
        ///  Description: Logs IP/Port changes into "Repeater_GUI_Log.txt". <br/><br/>
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
            var lineCount = File.ReadLines("Repeater_GUI_Log.txt").Count();

            using (StreamWriter stream = File.AppendText("Repeater_GUI_Log.txt"))
            {
                if (lineCount > 250)        // To make sure it doesn't get too big, it's capped at 250 entries
                {
                    var lines = File.ReadAllLines("Repeater_GUI_Log.txt");
                    File.WriteAllLines("Repeater_GUI_Log.txt", lines.Skip(1).ToArray());
                }

                string message = String.Format("The {0} settings were changed. New settings - " +
                                                "IP Address: {1} and Port: {2}", editType, ip, port);

                string entry = "IP/Port Change" + ",\t" + message + ",\t" + DateTime.Now.ToString();

                stream.WriteLine(entry);
            }
        }


        /// <summary> 
        ///  Class Name: Logger  <br/><br/>
        ///
        ///  Description: Logs Inactivity settings changes into "Repeater_GUI_Log.txt". <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  int <paramref name="frequency"/> - The new frequency value. <br/>
        ///  string <paramref name="interval"/> - The new interval value. <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public static void LogInactivityChange(int frequency, string interval)
        {
            var lineCount = File.ReadLines("Repeater_GUI_Log.txt").Count();

            using (StreamWriter stream = File.AppendText("Repeater_GUI_Log.txt"))
            {
                if (lineCount > 250)        // To make sure it doesn't get too big, it's capped at 250 entries
                {
                    var lines = File.ReadAllLines("Repeater_GUI_Log.txt");
                    File.WriteAllLines("Repeater_GUI_Log.txt", lines.Skip(1).ToArray());
                }

                string message = String.Format("The Inactivity settings were changed. New settings - " +
                                                "Frequency: {0} and Interval: {1}", frequency, interval);

                string entry = "Inactivity Settings Change" + ",\t" + message + ",\t" + DateTime.Now.ToString();
                stream.WriteLine(entry);
            }
        }

        /// <summary> 
        ///  Class Name: Logger  <br/> <br/>
        ///
        ///  Description: Logs all of the GUI's Exceptions into "Repeater_GUI_Log.txt". <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  Exception <paramref name="e"/> - An Exception to be logged <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public static void LogException(Exception e)
        {
            var lineCount = File.ReadLines("Repeater_GUI_Log.txt").Count();

            using (StreamWriter stream = File.AppendText("Repeater_GUI_Log.txt"))
            {
                if (lineCount > 250)        // To make sure it doesn't get too big, it's capped at 250 entries
                {
                    var lines = File.ReadAllLines("Repeater_GUI_Log.txt");
                    File.WriteAllLines("Repeater_GUI_Log.txt", lines.Skip(1).ToArray());
                }

                string message = String.Format("The frontend/user interface has suffered an error: " + e.Message);

                string entry = "Interface Error" + ",\t" + message + ",\t" + DateTime.Now.ToString();
                stream.WriteLine(entry);
            }
        }
    }
}
