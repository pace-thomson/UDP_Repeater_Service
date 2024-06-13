//----------------------------------------------------
// File Name: UDP_Service.cs
// 
// Description: This file contains the methods for 
//              initializing the service and dealing
//              with configuration set up. 
//
// Language:         Visual C#
// Target:           Windows PC
// Operating System: Windows 10 Enterprise
// Compiler:         Visual Studio .Net 2022
//
//          Change History:
//
// Version  Date    Author          Description
// 1.0      ---     Jade Thomson    Initial Release
//---------------------------------------------------

using System;
using System.ServiceProcess;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Repeater;
using GUIreceiver;
using JsonDataNameSpace;
using Newtonsoft.Json;
using System.Threading;
using System.IO;



namespace UDP_Repeater_Service
{
    /// <summary>
    /// Houses all of the functionality that sets up the service and how it is constructed. 
    /// Inherits from ServiceBase
    /// </summary>
    public partial class UDP_Service : ServiceBase
    {
        /// <summary> 
        ///  Class Name: UDP_Service  <br/><br/> 
        ///  Parent Class: ServiceBase  <br/><br/> 
        ///
        ///  Description: Runs the main part of the whole repeater service. Turns on the repeater thread <br/>
        ///  and then continutes listening until input is received from the GUI. At which point, the "UDP_Repeater_Config.json <br/>
        ///  and the JsonData Object is updated. It then restarts the repeater thread with the updated settings. <br/><br/>
        ///
        ///  Inputs: None <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public UDP_Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            TheMainProgram.main();
        }

        protected override void OnStop()
        {
        }
    }
}


/// <summary>
/// Houses all of the functionality that sets up and runs the program
/// </summary>
class TheMainProgram
{
    /// <summary> 
    ///  Class Name: TheMainProgram  <br/><br/> 
    ///
    ///  Description: Creates "UDP_Repeater_Config.json" if it doesn't already exist. Then it creates a JsonData object and <br/>
    ///  updates it's values to match those found under currentConfig within "UDP_Repeater_Config.json".<br/><br/>
    ///
    ///  Inputs: None <br/><br/>
    ///  
    ///  Returns:  JsonData jsonData - A new JsonData object.
    /// </summary>
    public static JsonData SetConfig()
    {
        if (!File.Exists("UDP_Repeater_Config.json"))
        {
            string defaults = @"
                {
                    ""currentConfig"": {
                        ""receiveFrom"": {
                            ""ip"": ""127.0.0.255"",
                            ""port"": ""7654""
                        },
                        ""sendTo"": {
                            ""ip"": ""132.58.202.157"",
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
                    }
                }";

            // Write the JSON string to a file
            File.WriteAllText("UDP_Repeater_Config.json", defaults);
        }

        Console.WriteLine("Current IP and Port Configuration: ");
        string jsonString = File.ReadAllText("UDP_Repeater_Config.json");


        JObject jsonObject = JObject.Parse(jsonString);

        string receiveIp = (string)jsonObject["currentConfig"]["receiveFrom"]["ip"];
        string receivePort = (string)jsonObject["currentConfig"]["receiveFrom"]["port"];
        string sendIp = (string)jsonObject["currentConfig"]["sendTo"]["ip"];
        string sendPort = (string)jsonObject["currentConfig"]["sendTo"]["port"];

        JsonData jsonData = new JsonData(receiveIp, receivePort, sendIp, sendPort);

        Console.WriteLine(jsonString);
        return jsonData;
    }


    /// <summary> 
    ///  Class Name: TheMainProgram  <br/><br/> 
    ///
    ///  Description: Restores the values in <paramref name="jsonData"/> to match those found in "UDP_Repeater_Config.json"  <br/>
    ///  under defaultSettings. Also updates the settings under currentConfig the same way.<br/><br/>
    ///
    ///  Inputs:  <br/>
    ///  JsonData <paramref name="jsonData"/> - The JsonData update with the default settings. <br/><br/>
    ///  
    ///  Returns:  None
    /// </summary>
    public static void RestoreToDefaults(JsonData jsonData)
    {
        string jsonString = File.ReadAllText("UDP_Repeater_Config.json");


        JObject jsonObject = JObject.Parse(jsonString);

        string receiveIp = (string)jsonObject["defaultSettings"]["receiveFrom"]["ip"];
        string receivePort = (string)jsonObject["defaultSettings"]["receiveFrom"]["port"];
        string sendIp = (string)jsonObject["defaultSettings"]["sendTo"]["ip"];
        string sendPort = (string)jsonObject["defaultSettings"]["sendTo"]["port"];

        jsonObject["currentConfig"]["receiveFrom"]["ip"] = receiveIp;
        jsonObject["currentConfig"]["receiveFrom"]["port"] = receivePort;
        jsonObject["currentConfig"]["sendTo"]["ip"] = sendIp;
        jsonObject["currentConfig"]["sendTo"]["port"] = sendPort;


        jsonData.receiveIp = receiveIp;
        jsonData.receivePort = int.Parse(receivePort);
        jsonData.sendIp = sendIp;
        jsonData.sendPort = int.Parse(sendPort);

        var stringThing = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
        File.WriteAllText("UDP_Repeater_Config.json", jsonString);
    }

    /// <summary> 
    ///  Class Name: TheMainProgram  <br/><br/> 
    ///
    ///  Description: Restores the values in "UDP_Repeater_Config.json" to match those found in the new <paramref name="jsonData"/>. <br/><br/>
    ///
    ///  Inputs:  <br/>
    ///  JsonData <paramref name="jsonData"/> - The JsonData update with the new settings. <br/><br/>
    ///  
    ///  Returns:  None
    /// </summary>
    public static void UpdateConfigJson(JsonData jsonData)
    {
        string jsonString = File.ReadAllText("UDP_Repeater_Config.json");

        JObject jsonObject = JObject.Parse(jsonString);

        // This checks to see if the selected option was to to change the defaults
        if (jsonData.sendPort == -1 || jsonData.receivePort == -1)
        {
            if (jsonData.sendPort == -1)    // if the reconfigure default RECEIVE was chosen
            {
                jsonObject["defaultSettings"]["receiveFrom"]["ip"] = jsonData.receiveIp;
                jsonObject["defaultSettings"]["receiveFrom"]["port"] = jsonData.receivePort.ToString();
            }
            else if (jsonData.receivePort == -1)    // if the reconfigure default SEND was chosen
            {
                jsonObject["defaultSettings"]["sendTo"]["ip"] = jsonData.sendIp;
                jsonObject["defaultSettings"]["sendTo"]["port"] = jsonData.sendPort.ToString();
            }
        }
        else              // normal (non-default changing) configuration was chosen
        {
            jsonObject["currentConfig"]["receiveFrom"]["ip"] = jsonData.receiveIp;
            jsonObject["currentConfig"]["receiveFrom"]["port"] = jsonData.receivePort.ToString();
            jsonObject["currentConfig"]["sendTo"]["ip"] = jsonData.sendIp;
            jsonObject["currentConfig"]["sendTo"]["port"] = jsonData.sendPort.ToString();
        }

        var stringThing = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
        File.WriteAllText("UDP_Repeater_Config.json", stringThing);
    }




    /// <summary> 
    ///  Class Name: TheMainProgram  <br/><br/> 
    ///
    ///  Description: Runs the main part of the whole repeater service. Turns on the repeater thread <br/>
    ///  and then continutes listening until input is received from the GUI. At which point, the "UDP_Repeater_Config.json <br/>
    ///  and the JsonData Object is updated. It then restarts the repeater thread with the updated settings. <br/><br/>
    ///
    ///  Inputs: None <br/><br/>
    ///  
    ///  Returns:  None
    /// </summary>
    public static async void main()
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        JsonData jsonData = SetConfig();

        while (true)
        {
            // Starts the Sending/Receiving thread
            Thread repeaterThread = new Thread(() => RepeaterClass.main(jsonData, cts.Token));
            repeaterThread.Start();

            // Use Task<T> to call the method asynchronously and get the result
            Task<JsonData> receiveFromGUITask = Task.Run(() => ReceiveFromGUI.main(jsonData));
            JsonData newjsonData = await receiveFromGUITask; // Await the task to get the result

            cts.Cancel();                           // Signal the send thread to stop
            repeaterThread.Join();                  // Wait for the send thread to complete
            cts = new CancellationTokenSource();    // Reset the cancellation token for the next iteration

            
            if (jsonData.Equals(newjsonData))       // this checks to see if the option was to restore defaults, if not it updates jsonData
            {
                RestoreToDefaults(jsonData);
            }
            else
            {
                UpdateConfigJson(newjsonData);
                jsonData = SetConfig();             // updates jsonData to match what's in config.json
            }
        }
    }
}