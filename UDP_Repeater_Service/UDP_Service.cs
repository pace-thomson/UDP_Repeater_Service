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
//   1.0    8/3/24    Jade Pace Thomson     Initial Release
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
            // Our backend object for start/stop logging.
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

        /// <summary> 
        ///  Class Name: UDP_Service  <br/><br/> 
        ///  Parent Class: ServiceBase  <br/><br/> 
        ///
        ///  Description: When windows starts this service, the service manager calls this method. <br/>
        ///  This logs the service starting and then calls the main method for the service. <br/><br/>
        ///
        ///  Inputs: <br/>
        ///  string[] <paramref name="args"/> - The arguements for starting the service. <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        protected override void OnStart(string[] args)
        {
            outerBackendObject.StartStopLogger("start");
            TheMainProgram.main();
        }
        /// <summary> 
        ///  Class Name: UDP_Service  <br/><br/> 
        ///  Parent Class: ServiceBase  <br/><br/> 
        ///
        ///  Description: When windows stops this service, the service manager calls this method. <br/>
        ///  This logs that the service is stopping and waits two seconds to make sure it has time. <br/><br/>
        ///
        ///  Inputs: None <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        protected override void OnStop()
        {
            outerBackendObject.StartStopLogger("stop");
            Thread.Sleep(2000);
        }
    }
}


/// <summary>
/// Houses all of the functionality that sets up and runs the Packet Repeater Service
/// </summary>
class TheMainProgram
{
    /// <summary> 
    ///  Class Name: TheMainProgram  <br/><br/> 
    ///
    ///  Description: Reads from "UDP_Repeater_Config.json" and creates a Backend Object with the values <br/>
    ///  read from the file. <br/><br/>
    ///
    ///  Inputs: None <br/><br/>
    ///  
    ///  Returns:  Backend newbackendObject - The main Backend objects.
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
            int    interval     =   ( int  )jsonObject["inactivitySettings"]["inactivityInterval"];
            string unit         =   (string)jsonObject["inactivitySettings"]["inactivityUnit"];
            string promEndpoint =   (string)jsonObject["monitoring"]["prom"];
            string lokiEndpoint =   (string)jsonObject["monitoring"]["loki"];
            string ipOfNIC      =   (string)jsonObject["ipAddressOfNIC"];


            Backend backendObject = new Backend(receiveIp, receivePort, sendIp, sendPort, interval, unit, 
                                                promEndpoint, lokiEndpoint, ipOfNIC);

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
    ///  Description: Updates the values in "UDP_Repeater_Config.json" to match those found in <paramref name="newbackendObject"/>. 
    ///               If not, it updates the currentConfig values with the dafault settings. <br/><br/>
    ///
    ///  Inputs:  <br/>
    ///  Backend <paramref name="newbackendObject"/> - The Backend object with the new settings tp update config.json. <br/>
    ///  Backend <paramref name="originalBackendObject"/> - The Backend object just for error logging. <br/><br/>
    ///  
    ///  Returns:  None
    /// </summary>
    public static void UpdateConfigJson(Backend newbackendObject, Backend originalBackendObject)
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
                    jsonObject["inactivitySettings"]["inactivityInterval"]  = newbackendObject.inactivityInterval.ToString();
                    jsonObject["inactivitySettings"]["inactivityUnit"]      = newbackendObject.inactivityUnit;
                    break;
                case Backend.changeType.setup:
                    jsonObject["monitoring"]["prom"] = newbackendObject.promEndpoint;
                    jsonObject["monitoring"]["loki"] = newbackendObject.lokiEndpoint;
                    jsonObject["ipAddressOfNIC"]     = newbackendObject.ipAddressOfNIC;
                    break;
                case Backend.changeType.restoreToDefaults:
                    jsonObject["currentConfig"]["receiveFrom"]["ip"]    =   (string)jsonObject["defaultSettings"]["receiveFrom"]["ip"];
                    jsonObject["currentConfig"]["receiveFrom"]["port"]  =   (string)jsonObject["defaultSettings"]["receiveFrom"]["port"];
                    jsonObject["currentConfig"]["sendTo"]["ip"]         =   (string)jsonObject["defaultSettings"]["sendTo"]["ip"];
                    jsonObject["currentConfig"]["sendTo"]["port"]       =   (string)jsonObject["defaultSettings"]["sendTo"]["port"];

                    newbackendObject.receiveIp    = (string)jsonObject["defaultSettings"]["receiveFrom"]["ip"];
                    newbackendObject.receivePort  =  (int)  jsonObject["defaultSettings"]["receiveFrom"]["port"];
                    newbackendObject.sendIp       = (string)jsonObject["defaultSettings"]["sendTo"]["ip"];
                    newbackendObject.sendPort     =  (int)  jsonObject["defaultSettings"]["sendTo"]["port"];
                    break;
            }

            string stringThing = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
            File.WriteAllText("UDP_Repeater_Config.json", stringThing);
        }
        catch (Exception e)
        {
            originalBackendObject.ExceptionLogger(e);
        }
    }

    /// <summary> 
    ///  Class Name: TheMainProgram  <br/><br/> 
    ///
    ///  Description: Catches any unhandled exeptions for the service and logs them. <br/><br/>
    ///
    ///  Inputs:  <br/>
    ///  object <paramref name="sender"/> - Not used here. <br/>
    ///  UnhandledExceptionEventArgs <paramref name="e"/> - The unhandled exception. <br/><br/>
    ///  
    ///  Returns:  None
    /// </summary>
    public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Exception ex = (Exception)e.ExceptionObject;

        Backend.ExceptionLoggerStatic(ex);
    }


    /// <summary> 
    ///  Class Name: TheMainProgram  <br/><br/> 
    ///
    ///  Description: Runs the main part of the whole repeater service. Turns on the repeater thread asynchronously <br/>
    ///  and then continuely listens until a new configuraton is received from the GUI. At which point, <br/>
    ///  the "UDP_Repeater_Config.json and the Backend Object is updated. It then restarts the repeater <br/>
    ///  thread with the updated settings, or restarts the whole service if the "setup" was reconfigured. <br/><br/>
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


                    // Use Task<Backend> to call the ReceiveFromGUI.main() method asynchronously and get the backend object it returns
                Task<Backend> receiveFromGUITask = Task.Run(() => ReceiveFromGUI.main(backendObject));
                Backend newbackendObject = await receiveFromGUITask;

                cts.Cancel();                           // Signal the send thread to stop
                repeaterThread.Join();                  // Wait for the send thread to complete
                cts = new CancellationTokenSource();    // Reset the cancellation token for the next iteration

                if (newbackendObject != null)
                {
                    UpdateConfigJson(newbackendObject, backendObject);
                    if (newbackendObject.change == Backend.changeType.setup)
                    {
                        Thread.Sleep(1000);
                        Environment.Exit(1);   // forces restart because loki/prom change is weird and this service is set to restart on failure
                    }
                    backendObject.UpdateWithNewBackendObject(newbackendObject);     // updates the original backendObject with the new values
                }
            }
        }
        catch (Exception e)
        {
            Backend.ExceptionLoggerStatic(e);
        }
    }
}