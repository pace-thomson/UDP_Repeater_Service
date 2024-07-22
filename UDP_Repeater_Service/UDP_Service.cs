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
using System.Linq;



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
                EventLog.GetEventLogs().First(x => x.Log == "UDP Packet Repeater").MaximumKilobytes = 256;
            }
            if (!EventLog.SourceExists("UDP_Repeater_Frontend"))
            {
                EventLog.CreateEventSource("UDP_Repeater_Frontend", "UDP Packet Repeater");
                EventLog.GetEventLogs().First(x => x.Log == "UDP Packet Repeater").MaximumKilobytes = 256;
            }
            outerBackendObject = new Backend();
        }

        public void DebuggerProcess()
        {
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
            TheMainProgram.main();
        }
        /// <summary> Logs that the service is stopping </summary>
        protected override void OnStop()
        {
            outerBackendObject.StartStopLogger("stop");
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
    ///  Returns:  Backend newbackendObject - A new Backend object.
    /// </summary>
    public static Backend SetConfig()
    {
        try
        {
                // actual path: "C:\\Windows\\SysWOW64\\UDP_Repeater_Config.json"
            string jsonString = File.ReadAllText("UDP_Repeater_Config.json");
            JObject jsonObject = JObject.Parse(jsonString);

            string receiveIp    =   (string)jsonObject["currentConfig"]["receiveFrom"]["ip"];
            string receivePort  =   (string)jsonObject["currentConfig"]["receiveFrom"]["port"];
            string sendIp       =   (string)jsonObject["currentConfig"]["sendTo"]["ip"];
            string sendPort     =   (string)jsonObject["currentConfig"]["sendTo"]["port"];
            int    frequency    =   ( int  )jsonObject["inactivitySettings"]["frequency"];
            string interval     =   (string)jsonObject["inactivitySettings"]["interval"];
            string promEndpoint =   (string)jsonObject["monitoring"]["prom"];
            string lokiEndpoint =   (string)jsonObject["monitoring"]["loki"];
            string nameOfNIC    =   (string)jsonObject["descriptionOfNIC"];


            Backend backendObject = new Backend(receiveIp, receivePort, sendIp, sendPort, frequency, interval, 
                                                promEndpoint, lokiEndpoint, nameOfNIC);

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
            File.WriteAllText("UDP_Repeater_Config.json", stringThing);
        }
        catch (Exception e)
        {
            backendObject.ExceptionLogger(e);
        }
    }

    /// <summary> 
    ///  Class Name: TheMainProgram  <br/><br/> 
    ///
    ///  Description: Restores the values in "UDP_Repeater_Config.json" to match those found in the new <paramref name="newbackendObject"/>. <br/><br/>
    ///
    ///  Inputs:  <br/>
    ///  Backend <paramref name="newbackendObject"/> - The Backend object to update with the new settings. <br/><br/>
    ///  
    ///  Returns:  None
    /// </summary>
    public static void UpdateConfigJson(Backend newbackendObject, Backend oldBackendObject)
    {
        try
        {
            string jsonString = File.ReadAllText("UDP_Repeater_Config.json");

            JObject jsonObject = JObject.Parse(jsonString);

                // Default changing configuration was chosen
            if (newbackendObject.sendPort == -1 || newbackendObject.receivePort == -1)
            {
                if (newbackendObject.sendPort == -1)    // if the reconfigure default RECEIVE was chosen
                {
                    jsonObject["defaultSettings"]["receiveFrom"]["ip"]    =   newbackendObject.receiveIp;
                    jsonObject["defaultSettings"]["receiveFrom"]["port"]  =   newbackendObject.receivePort.ToString();
                    newbackendObject.sendPort = oldBackendObject.sendPort;
                }
                else if (newbackendObject.receivePort == -1)    // if the reconfigure default SEND was chosen
                {
                    jsonObject["defaultSettings"]["sendTo"]["ip"]   =  newbackendObject.sendIp;
                    jsonObject["defaultSettings"]["sendTo"]["port"] =  newbackendObject.sendPort.ToString();
                    newbackendObject.receivePort = oldBackendObject.receivePort;
                }
            }
            else              // normal (non-default changing) configuration was chosen
            {
                jsonObject["currentConfig"]["receiveFrom"]["ip"]    =   newbackendObject.receiveIp;
                jsonObject["currentConfig"]["receiveFrom"]["port"]  =   newbackendObject.receivePort.ToString();
                jsonObject["currentConfig"]["sendTo"]["ip"]         =   newbackendObject.sendIp;
                jsonObject["currentConfig"]["sendTo"]["port"]       =   newbackendObject.sendPort.ToString();
            }
                            // always updates the inactivity Settings and monitoring endpoints
            jsonObject["inactivitySettings"]["frequency"]  =  newbackendObject.frequency.ToString();
            jsonObject["inactivitySettings"]["interval"]   =  newbackendObject.interval;
            jsonObject["monitoring"]["prom"]               =  newbackendObject.promEndpoint;
            jsonObject["monitoring"]["loki"]               =  newbackendObject.lokiEndpoint;
            jsonObject["descriptionOfNIC"]                 =  newbackendObject.descriptionOfNIC;    

            var stringThing = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
            File.WriteAllText("UDP_Repeater_Config.json", stringThing);
        }
        catch (Exception e)
        {
            newbackendObject.ExceptionLogger(e);
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
                else if (backendObject == null)
                {
                    continue;
                }
                else    // some settings were changed, so we need to update our things
                {
                    UpdateConfigJson(newbackendObject, backendObject);              // updates config.json
                    backendObject.UpdateWithNewBackendObject(newbackendObject);     // updates the original newbackendObject with the new valuess
                }
            }
        }
        catch (Exception e)
        {
            Backend.ExceptionLoggerStatic(e);
        }
    }
}