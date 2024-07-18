//----------------------------------------------------
// File Name: UDP_Service.cs
// 
// Description: This file contains the methods for 
//              initializing the service and dealing
//              with configuration set up. 
//
// Language:         Visual C#
// Target:           Windows PC
// Operating System: Windows 11 Enterprise
// Compiler:         Visual Studio .Net 2022
//
//          Change History:
//
// Version   Date         Author            Description
//   1.0    7/3/24    Jade Pace Thomson   Initial Release
//---------------------------------------------------

using System;
using System.ServiceProcess;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Repeater;
using GUIreceiver;
using BackendClassNameSpace;
using Newtonsoft.Json;
using System.Threading;
using System.IO;
using System.Diagnostics;



namespace UDP_Repeater_Service
{
    /// <summary>
    /// Houses all of the functionality that sets up the service and how it is constructed. 
    /// Inherits from ServiceBase
    /// </summary>
    public partial class UDP_Service : ServiceBase
    {
        private Backend outerBackendObject;

        /// <summary> 
        ///  Class Name: UDP_Service  <br/><br/> 
        ///  Parent Class: ServiceBase  <br/><br/> 
        ///
        ///  Description: Creates the service side of things. This was auto-generated as part of this template. <br/><br/>
        ///
        ///  Inputs: None <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public UDP_Service()
        {
            InitializeComponent();
            if (!EventLog.SourceExists("UDP_Repeater_Backend"))
            {
                EventLog.CreateEventSource("UDP_Repeater_Backend", "UDP Packet Repeater");
            }
            if (!EventLog.SourceExists("UDP_Repeater_Frontend"))
            {
                EventLog.CreateEventSource("UDP_Repeater_Frontend", "UDP Packet Repeater");
            }
            outerBackendObject = new Backend();
        }

        public void DebuggerProcess()
        {
            outerBackendObject.isRunning.Add(1);
            outerBackendObject.StartStopLogger("start");
            TheMainProgram.main();
        }

        /// <summary> 
        ///  Class Name: UDP_Service  <br/><br/> 
        ///  Parent Class: ServiceBase  <br/><br/> 
        ///
        ///  Description: When windows starts this service, it runs the backend program. Through this<br/>
        ///  This also logs the service starting. This was auto-generated as part of this template. <br/><br/>
        ///
        ///  Inputs: None <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        protected override void OnStart(string[] args)
        {
            outerBackendObject.StartStopLogger("start");
            outerBackendObject.isRunning.Add(1);
            TheMainProgram.main();
        }
        /// <summary> Logs that the service is stopping </summary>
        protected override void OnStop()
        {
            outerBackendObject.StartStopLogger("stop");
            outerBackendObject.isRunning.Add(-1);
            Thread.Sleep(1000);
        }
    }
}


/// <summary>
/// Houses all of the functionality that sets up and runs the program Repeater Service
/// </summary>
class TheMainProgram
{
    /// <summary> 
    ///  Class Name: TheMainProgram  <br/><br/> 
    ///
    ///  Description: Creates "UDP_Repeater_Config.json" if it doesn't already exist. Then it creates a Backend object and <br/>
    ///  updates it's values to match those found under currentConfig within "UDP_Repeater_Config.json".<br/><br/>
    ///
    ///  Inputs: None <br/><br/>
    ///  
    ///  Returns:  Backend backendObject - A new Backend object.
    /// </summary>
    public static Backend SetConfig()
    {
        try
        {
            if (!File.Exists("UDP_Repeater_Config.json"))   // actual path: "C:\\Windows\\SysWOW64\\UDP_Repeater_Config.json"
            {
                // if UDP_Repeater_Config.json doense't exist, it make it and then poplulates it with this string
                string defaults = @"        
                {
                    ""currentConfig"": {
                        ""receiveFrom"": {
                            ""ip"": ""172.18.46.213"",
                            ""port"": ""763""
                        },
                        ""sendTo"": {
                            ""ip"": ""172.18.46.213"",
                            ""port"": ""722""
                        }
                    },

                    ""defaultSettings"": {
                        ""receiveFrom"": {
                            ""ip"": ""127.0.0.255"",
                            ""port"": ""7654""
                        },
                        ""sendTo"": {
                            ""ip"": ""132.58.202.157"",
                            ""port"": ""722""
                        }
                    },

                    ""inactivitySettings"": {
                        ""frequency"": ""5"",
                        ""interval"": ""minute""
                    },
                    ""descriptionOfNIC"":""Temporary""
                }";

                // Write the JSON string to a file
                File.WriteAllText("UDP_Repeater_Config.json", defaults);
            }

            string jsonString = File.ReadAllText("UDP_Repeater_Config.json");


            JObject jsonObject = JObject.Parse(jsonString);

            string receiveIp    =   (string)jsonObject["currentConfig"]["receiveFrom"]["ip"];
            string receivePort  =   (string)jsonObject["currentConfig"]["receiveFrom"]["port"];
            string sendIp       =   (string)jsonObject["currentConfig"]["sendTo"]["ip"];
            string sendPort     =   (string)jsonObject["currentConfig"]["sendTo"]["port"];
            int frequency       =   ( int  )jsonObject["inactivitySettings"]["frequency"];
            string interval     =   (string)jsonObject["inactivitySettings"]["interval"];
            string nameOfNIC    =   (string)jsonObject["descriptionOfNIC"];


            Backend backendObject = new Backend(receiveIp, receivePort, sendIp, sendPort, frequency, interval, nameOfNIC);

            return backendObject;
        }
        catch (Exception e)
        {
            Backend.ExceptionLoggerStatic(e);
            return null;
        }
    }


    /// <summary> 
    ///  Class Name: TheMainProgram  <br/><br/> 
    ///
    ///  Description: Restores the values in <paramref name="backendObject"/> to match those found in "UDP_Repeater_Config.json"  <br/>
    ///  under defaultSettings. Also updates the settings under currentConfig in "UDP_Repeater_Config.json" the same way.<br/><br/>
    ///
    ///  Inputs:  <br/>
    ///  Backend <paramref name="backendObject"/> - The Backend object to update with the default settings. <br/><br/>
    ///  
    ///  Returns:  None
    /// </summary>
    public static void RestoreToDefaults(Backend backendObject)
    {
        try
        {
            string jsonString = File.ReadAllText("UDP_Repeater_Config.json");


            JObject jsonObject = JObject.Parse(jsonString);

            string receiveIp   =  (string)jsonObject["defaultSettings"]["receiveFrom"]["ip"];
            string receivePort =  (string)jsonObject["defaultSettings"]["receiveFrom"]["port"];
            string sendIp      =  (string)jsonObject["defaultSettings"]["sendTo"]["ip"];
            string sendPort    =  (string)jsonObject["defaultSettings"]["sendTo"]["port"];

            jsonObject["currentConfig"]["receiveFrom"]["ip"]   =  receiveIp;
            jsonObject["currentConfig"]["receiveFrom"]["port"] =  receivePort;
            jsonObject["currentConfig"]["sendTo"]["ip"]        =  sendIp;
            jsonObject["currentConfig"]["sendTo"]["port"]      =  sendPort;


            backendObject.receiveIp    =  receiveIp;
            backendObject.receivePort  =  int.Parse(receivePort);
            backendObject.sendIp       =  sendIp;
            backendObject.sendPort     =  int.Parse(sendPort);

            var stringThing = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
            File.WriteAllText("UDP_Repeater_Config.json", jsonString);
        }
        catch (Exception e)
        {
            backendObject.ExceptionLogger(e);
        }
    }

    /// <summary> 
    ///  Class Name: TheMainProgram  <br/><br/> 
    ///
    ///  Description: Restores the values in "UDP_Repeater_Config.json" to match those found in the new <paramref name="backendObject"/>. <br/><br/>
    ///
    ///  Inputs:  <br/>
    ///  Backend <paramref name="backendObject"/> - The Backend object to update with the new settings. <br/><br/>
    ///  
    ///  Returns:  None
    /// </summary>
    public static void UpdateConfigJson(Backend backendObject)
    {
        try
        {
            string jsonString = File.ReadAllText("UDP_Repeater_Config.json");

            JObject jsonObject = JObject.Parse(jsonString);

            // This checks to see if the selected option was to to change the defaults
            if (backendObject.sendPort == -1 || backendObject.receivePort == -1)
            {
                if (backendObject.sendPort == -1)    // if the reconfigure default RECEIVE was chosen
                {
                    jsonObject["defaultSettings"]["receiveFrom"]["ip"]    =   backendObject.receiveIp;
                    jsonObject["defaultSettings"]["receiveFrom"]["port"]  =   backendObject.receivePort.ToString();
                }
                else if (backendObject.receivePort == -1)    // if the reconfigure default SEND was chosen
                {
                    jsonObject["defaultSettings"]["sendTo"]["ip"]   =  backendObject.sendIp;
                    jsonObject["defaultSettings"]["sendTo"]["port"] =  backendObject.sendPort.ToString();
                }
            }
            else              // normal (non-default changing) configuration was chosen
            {
                jsonObject["currentConfig"]["receiveFrom"]["ip"]    =   backendObject.receiveIp;
                jsonObject["currentConfig"]["receiveFrom"]["port"]  =   backendObject.receivePort.ToString();
                jsonObject["currentConfig"]["sendTo"]["ip"]         =   backendObject.sendIp;
                jsonObject["currentConfig"]["sendTo"]["port"]       =   backendObject.sendPort.ToString();
            }
                            // always updates the inactivitySettings
            jsonObject["inactivitySettings"]["frequency"]  =  backendObject.frequency.ToString();
            jsonObject["inactivitySettings"]["interval"]   =  backendObject.interval;
            jsonObject["descriptionOfNIC"]                 =  backendObject.descriptionOfNIC;    

            var stringThing = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
            File.WriteAllText("UDP_Repeater_Config.json", stringThing);
        }
        catch (Exception e)
        {
            backendObject.ExceptionLogger(e);
        }
    }

    public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        string message = String.Format($"Unhandled Error Message: {e} \n" +
                                       $"Error location: Backend/Service.");
        
        EventLog eventLog = new EventLog("UDP Packet Repeater");
        eventLog.Source = "UDP_Repeater_Backend";

        eventLog.WriteEntry(message, EventLogEntryType.Error, 1);  // 1 is our id for backend errors
    }



    /// <summary> 
    ///  Class Name: TheMainProgram  <br/><br/> 
    ///
    ///  Description: Runs the main part of the whole repeater service. Turns on the repeater thread <br/>
    ///  and then continutes listening until a new configuraton is received from the GUI. At which point, <br/>
    ///  the "UDP_Repeater_Config.json and the Backend Object is updated. It then restarts the repeater <br/>
    ///  thread with the updated settings. <br/><br/>
    ///
    ///  Inputs: None <br/><br/>
    ///  
    ///  Returns:  None
    /// </summary>
    public static async void main()
    {
        AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        try
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Backend backendObject = SetConfig();

            while (true)
            {
                // Starts the Sending/Receiving thread
                Thread repeaterThread = new Thread(() => RepeaterClass.main(backendObject, cts.Token));
                repeaterThread.Start();


                // Use Task<Backend> to call the method asynchronously and get the backend object it returns
                Task<Backend> receiveFromGUITask = Task.Run(() => ReceiveFromGUI.main(backendObject));
                Backend newbackendObject = await receiveFromGUITask;

                cts.Cancel();                           // Signal the send thread to stop
                repeaterThread.Join();                  // Wait for the send thread to complete
                cts = new CancellationTokenSource();    // Reset the cancellation token for the next iteration


                // this checks to see if the option was to restore defaultst
                if (backendObject.Equals(newbackendObject))
                {
                    RestoreToDefaults(backendObject);
                }
                else    // otherwise, some settings were changed, so we need to update our things
                {
                    UpdateConfigJson(newbackendObject);  // updates config.json
                    backendObject = SetConfig();         // updates backendObject to match what's in config.json
                }
                newbackendObject.eventLog.Dispose();
            }
        }
        catch (Exception e)
        {
            Backend.ExceptionLoggerStatic(e);
        }
    }
}