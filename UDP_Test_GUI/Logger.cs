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
// Version   Date          Author            Description
//   1.0    7/25/24    Jade Pace Thomson   Initial Release
//---------------------------------------------------


using Newtonsoft.Json.Linq;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using Serilog;
using Serilog.Formatting.Display;
using Serilog.Sinks.Grafana.Loki;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;



namespace UDP_Repeater_GUI
{
    /// <summary>
    /// This class handles all of the logging for the front end.
    /// </summary>
    public class Logger
    {
            /// <summary> Our event log object. </summary>
        public EventLog eventLog;
            /// <summary> The loki log that our logs get sent to, in addition to the windows log. </summary>
        public ILogger lokiLogger;

            /// <summary> Our Meter object (the base for all of the metric instrumentation) </summary>
        private Meter myMeter;
            /// <summary> The main meter provider </summary>
        public MeterProvider meterProvider;
            /// <summary> Tracks the memory use of the gui </summary>
        public ObservableGauge<double> processMemory;


        public Logger()
        {
            this.eventLog = new EventLog("UDP Packet Repeater");
            this.eventLog.Source = "UDP_Repeater_Frontend";

            GetEndpoints(out string promURI, out string lokiURI);

            try
            {
                if (!Uri.IsWellFormedUriString(promURI, UriKind.Absolute) ||
                    !Uri.IsWellFormedUriString(lokiURI, UriKind.Absolute))
                {
                    throw new UriFormatException();
                }

                const string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} \t Frontend/Interface \t {Level} {NewLine}{Message}";

                this.lokiLogger = new LoggerConfiguration()
                                .WriteTo.GrafanaLoki
                                (
                                    lokiURI,
                                    labels: new List<LokiLabel>
                                    {
                                    new LokiLabel(){ Key = "RepeaterSide", Value = "Frontend/Interface" },
                                    new LokiLabel(){ Key = "MachineName", Value = Environment.MachineName },
                                    new LokiLabel(){ Key = "User", Value = Environment.UserName }
                                    },
                                    textFormatter: new MessageTemplateTextFormatter(outputTemplate, null)
                                )
                                .Enrich.FromLogContext()
                                .CreateLogger();


                this.meterProvider = Sdk.CreateMeterProviderBuilder()
                                    .AddMeter("JT4.Repeater.MyLibrary")
                                    .AddOtlpExporter((exporterOptions, metricReaderOptions) =>
                                    {
                                        exporterOptions.Endpoint = new Uri(promURI);
                                        exporterOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
                                        metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 5000;
                                    })
                                    .Build();
                this.myMeter = new Meter("JT4.Repeater.MyLibrary", "1.0");
                this.processMemory = myMeter.CreateObservableGauge("frontendMemory", () => GetProcessMemory());
            }
            catch (UriFormatException) 
            {
                eventLog.WriteEntry("Invalid endpoint configured, no monitoring currently.", EventLogEntryType.Warning, 9);
            }
        }

        /// <summary> 
        ///  Class Name: Logger  <br/> <br/>
        ///
        ///  Description: Calculates and returns the current process memory used in bytes. <br/><br/>
        ///
        ///  Inputs: None <br/><br/>
        ///  
        ///  Returns:  long - the process memory
        /// </summary>
        public static double GetProcessMemory()
        {
            long bytes = Process.GetCurrentProcess().PrivateMemorySize64;
            return (bytes / 1024.0) / 1024.0;
        }


        /// <summary> 
        ///  Class Name: Logger  <br/> <br/>
        ///
        ///  Description: Gets the montoring endpoints from the config.json file. <br/><br/>
        ///
        ///  Inputs: <br/>
        ///  out string <paramref name="prom"/> - The out string for the prometheus endpoint <br/>
        ///  out string <paramref name="loki"/> - The out string for the loki endpoint <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public void GetEndpoints(out string prom, out string loki)
        {
            try
            {
                bool configJsonExists = WaitForConfigJson();
                if (!configJsonExists)
                {
                    prom = "Couldn't read from config.json";
                    loki = "Couldn't read from config.json";
                    return;
                }

                string jsonString = File.ReadAllText("C:\\Windows\\SysWOW64\\UDP_Repeater_Config.json");
                JObject jsonObject = JObject.Parse(jsonString);

                prom = (string)jsonObject["monitoring"]["prom"];
                loki = (string)jsonObject["monitoring"]["loki"];
            }
            catch (Exception ex)
            {
                prom = "An error occured getting endpoints.";
                loki = "An error occured getting endpoints.";
                this.LogException(ex);
            }
        }

        /// <summary> 
        ///  Class Name: Logger  <br/> <br/>
        ///
        ///  Description: Gets the montoring endpoints from the config.json file. <br/><br/>
        ///
        ///  Inputs: None <br/><br/>
        ///  
        ///  Returns:  Bool - False if this function tried 15 times with 1 second in between and it <br/>
        ///  still didn't exist. True if it does exist.
        /// </summary>
        public bool WaitForConfigJson()
        {
            try
            {
                int attemptCount = 0;
                while (!File.Exists("C:\\Windows\\SysWOW64\\UDP_Repeater_Config.json"))
                {
                    attemptCount++;
                    if (attemptCount > 15)
                    {
                        string message = "Cannot read from configuration json file. Tried 15 times without success.";
                        WarningLogger(message);

                        return false;
                    }
                    System.Threading.Thread.Sleep(1000);
                }
                return true;
            }
            catch (Exception ex)
            {
                this.LogException(ex);
                return false;
            }
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

            string message = String.Format($"Message: {e.Message} \n" +
                                           $"Stack trace: {formattedStackString.Last().TrimStart()}");

            eventLog.WriteEntry(message, EventLogEntryType.Error, 2);       // 2 is id for frontend errors
            if (lokiLogger != null)
            {
                lokiLogger.Error(message);
            }
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
            string message;
            if (editType == "Reverted to Default")
            {
                message = "System IP and Port configurations were returned to their default settings.";
            }
            else
            {
                message = $"The {editType} settings were changed. \n" +
                          $"IP Address: {ip} \n" +
                          $"Port: {port}";
            }

            eventLog.WriteEntry(message, EventLogEntryType.Information, 6);  // 6 is id for ip/port config change
            if (lokiLogger != null)
            {
                lokiLogger.Information(message);
            }
        }


        /// <summary> 
        ///  Class Name: Logger  <br/><br/>
        ///
        ///  Description: Logs Inactivity settings changes into the event log. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  int <paramref name="interval"/> - The new interval value. <br/>
        ///  string <paramref name="unit"/> - The new unit value. <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public void LogInactivityChange(int interval, string unit)
        {
            string message = String.Format("The Inactivity settings were changed. \n" +
                                           "Interval: {0} \n" +
                                           "Unit: {1}", interval, unit);

            eventLog.WriteEntry(message, EventLogEntryType.Information, 7);  // 7 is an inactivity config change
            if (lokiLogger != null)
            {
                lokiLogger.Information(message);
            }
        }

        /// <summary> 
        ///  Class Name: Logger  <br/><br/>
        ///
        ///  Description: Logs NIC changes into the event log. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  string <paramref name="name"/> - The new NIC's name. <br/>
        ///  string <paramref name="type"/> - The new NIC's interface type. <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public void LogNicChange(string name, string macAddress)
        {
            string message = String.Format("The listening Network Interface Card was changed. \n" +
                                           "Name: {0} \n" +
                                           "Mac Address: {1}", name, macAddress);

            eventLog.WriteEntry(message, EventLogEntryType.Information, 8);  // 8 is a NIC config change
            if (lokiLogger != null)
            {
                lokiLogger.Information(message);
            }
        }

        /// <summary> 
        ///  Class Name: Logger  <br/><br/>
        ///
        ///  Description: Logs NIC changes into the event log. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  string <paramref name="promEndpoint"/> - The uri for the prometheus endpoint. <br/>
        ///  string <paramref name="lokiEndpoint"/> - The uri for the loki endpoint. <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public void LogMonitoringChange(string promEndpoint, string lokiEndpoint)
        {
            string message = $"The monitoring endpoints have changed. \n" +
                             $"Prometheus: {promEndpoint} \n" +
                             $"Loki: {lokiEndpoint}";

            eventLog.WriteEntry(message, EventLogEntryType.Information, 10);  // 10 is a NIC config change

            if (lokiLogger != null)
            {
                lokiLogger.Information(message);
            }
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
            string eventLogMessage = "";
            string lokiLogMessage = "";

            if (mode == "start")
            {
                eventLogMessage = "User Interface started.";
                lokiLogMessage = "User Interface \u001b[32mstarted\u001b[0m.";
            }                                         
            else if (mode == "stop")                  
            {
                eventLogMessage = "User Interface stopped.";
                lokiLogMessage = "User Interface \u001B[31mstopped\u001B[0m.";
            }

            eventLog.WriteEntry(eventLogMessage, EventLogEntryType.Information, 5);     // 5 is id for frontend start/stop

            if (lokiLogger != null)
            {
                lokiLogger.Information(lokiLogMessage);
            }
        }

        /// <summary> 
        ///  Class Name: Logger  <br/><br/>
        ///
        ///  Description: Logs general warnings. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  string <paramref name="message"/> - The warning eventLogMessage to log. <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public void WarningLogger(string message)
        {
            string actual = $"Message: {message}";

            eventLog.WriteEntry(message, EventLogEntryType.Warning, 11);     // 11 is id for backend general warnings

            if (lokiLogger != null)
            {
                lokiLogger.Warning(message);
            }
        }
    }
}
