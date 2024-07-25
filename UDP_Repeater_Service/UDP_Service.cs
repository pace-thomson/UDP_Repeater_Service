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
// Version   Date          Author            Description
//   1.0    7/25/24    Jade Pace Thomson   Initial Release
//---------------------------------------------------

using BackendClassNameSpace;
using GUIreceiver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Repeater;
using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;



namespace UDP_Repeater_Service
{
    /// <summary>
    /// Houses all of the functionality that sets up the service and how it is constructed. 
    /// Inherits from ServiceBase
    /// </summary>
    public partial class UDP_Service : ServiceBase
    {
            // Our backend ojbect that lives up here and is used for start/stop logging.
        private Backend outerBackendObject;

        /// <summary> 
        ///  Class Name: UDP_Service  <br/><br/> 
        ///  Parent Class: ServiceBase  <br/><br/> 
        ///
        ///  Description: Creates the service side of things. This was auto-generated as part of this template. <br/> 
        ///               Checks for and/or creates the event log sources, as well as an outer backend ojbect. <br/><br/>
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
            Thread.Sleep(2000);
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
    ///  Description: Restores the values in "UDP_Repeater_Config.json" to match those found in the <paramref name="newbackendObject"/>, 
    ///               or it restores them to defaults. <br/><br/>
    ///
    ///  Inputs:  <br/>
    ///  Backend <paramref name="newbackendObject"/> - The Backend object to update with the new settings. <br/>
    ///  Backend <paramref name="oldBackendObject"/> - The Backend object just for error logging. <br/><br/>
    ///  
    ///  Returns:  None
    /// </summary>
    public static void UpdateConfigJson(Backend newbackendObject, Backend oldBackendObject)
    {
        try
        {
            string jsonString = File.ReadAllText("UDP_Repeater_Config.json");
            JObject jsonObject = JObject.Parse(jsonString);

            switch (newbackendObject.change)
            {
                case Backend.changeType.receivingFrom:
                    jsonObject["currentConfig"]["receiveFrom"]["ip"]   = newbackendObject.receiveIp;
                    jsonObject["currentConfig"]["receiveFrom"]["port"] = newbackendObject.receivePort.ToString();
                    break;
                case Backend.changeType.sendingTo:
                    jsonObject["currentConfig"]["sendTo"]["ip"]   = newbackendObject.sendIp;
                    jsonObject["currentConfig"]["sendTo"]["port"] = newbackendObject.sendPort.ToString();
                    break;
                case Backend.changeType.defaultSend:
                    jsonObject["defaultSettings"]["sendTo"]["ip"]   = newbackendObject.sendIp;
                    jsonObject["defaultSettings"]["sendTo"]["port"] = newbackendObject.sendPort.ToString();
                    break;
                case Backend.changeType.defaultRecieve:
                    jsonObject["defaultSettings"]["receiveFrom"]["ip"]   = newbackendObject.receiveIp;
                    jsonObject["defaultSettings"]["receiveFrom"]["port"] = newbackendObject.receivePort.ToString();
                    break;
                case Backend.changeType.inactive:
                    jsonObject["inactivitySettings"]["frequency"] = newbackendObject.frequency.ToString();
                    jsonObject["inactivitySettings"]["interval"]  = newbackendObject.interval;
                    break;
                case Backend.changeType.setup:
                    jsonObject["monitoring"]["prom"] = newbackendObject.promEndpoint;
                    jsonObject["monitoring"]["loki"] = newbackendObject.lokiEndpoint;
                    jsonObject["descriptionOfNIC"]   = newbackendObject.descriptionOfNIC;
                    break;
                case Backend.changeType.restoreToDefaults:
                    jsonObject["currentConfig"]["receiveFrom"]["ip"]    =   (string)jsonObject["defaultSettings"]["receiveFrom"]["ip"];
                    jsonObject["currentConfig"]["receiveFrom"]["port"]  =   (string)jsonObject["defaultSettings"]["receiveFrom"]["port"];
                    jsonObject["currentConfig"]["sendTo"]["ip"]         =   (string)jsonObject["defaultSettings"]["sendTo"]["ip"];
                    jsonObject["currentConfig"]["sendTo"]["port"]       =   (string)jsonObject["defaultSettings"]["sendTo"]["port"];
                    break;
            }

            string stringThing = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
            File.WriteAllText("UDP_Repeater_Config.json", stringThing);
        }
        catch (Exception e)
        {
            oldBackendObject.ExceptionLogger(e);
        }
    }

    public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Exception ex = (Exception)e.ExceptionObject;

        string message = String.Format($"Error Message: {ex.Message} \n" +
                                       $"Error location: Backend/Service \n" +
                                       $"{ex.StackTrace}");

        using (EventLog tempLog = new EventLog("UDP Packet Repeater"))
        {
            tempLog.Source = "UDP_Repeater_Backend";

            tempLog.WriteEntry(message, EventLogEntryType.Error, 1);  // 1 is our id for backend errors
        }
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

                
                UpdateConfigJson(newbackendObject, backendObject);              // updates config.json
                if (newbackendObject.change == Backend.changeType.setup)
                {
                    Environment.Exit(1); // forces restart because the loki/prom change is weird and this service is set to restart on failure
                }
                backendObject.UpdateWithNewBackendObject(newbackendObject);     // updates the original backendObject with the new values
            }
        }
        catch (Exception e)
        {
            Backend.ExceptionLoggerStatic(e);
        }
    }
}