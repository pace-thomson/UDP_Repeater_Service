﻿//----------------------------------------------------
// File Name: Backend.cs
// 
// Description: This file contains all the methods for
//              the Backend class. Which includes all
//              the configuration settings as fields.
//              It also includes the methods for logging errors.
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



namespace BackendClassNameSpace
{
    /// <summary>
    /// An oject that contains all of the configuration settings to be passed between 
    /// the functions that need it. It is updated as settings change. This class
    /// also houses the functions for logging. 
    /// </summary>
    public class Backend
    {
            /// <summary> The IP being listened to</summary>
        public string receiveIp;
            /// <summary> The Port being listened to</summary>
        public int receivePort;
            /// <summary> The IP being sent to</summary>
        public string sendIp;
            /// <summary> The Port being sent to</summary>
        public int sendPort;
            /// <summary> The Frequency (number) at which the service reports inactivity</summary>
        public int frequency;
            /// <summary> The Interval (minute, day, hour) at which the service reports inactivity</summary>
        public string interval;
            /// <summary> The uri string for the endpoint that our prometheus metrics will be sent to. </summary>
        public string promEndpoint;
            /// <summary> The uri string for the endpoint that our loki logs will be sent to. </summary>
        public string lokiEndpoint;
            /// <summary> The device.Description of the network card that we're listening on. </summary>
        public string descriptionOfNIC;

            /// <summary> How we keep track of the change type. Only used with newBackendObject. </summary>
        public changeType change;
            /// <summary> All the different change types. </summary>
        public enum changeType
        {
            receivingFrom,
            sendingTo,
            defaultSend,
            defaultRecieve,
            inactive,
            setup,
            restoreToDefaults
        }

            /// <summary> Our machine-local windows event log. </summary>
        public EventLog eventLog;
            /// <summary> The loki log that our logs get sent to, in addition to the windows log.
            ///           local: http://localhost:3100       
            ///           sandbox: http://172.18.46.211:3100
            /// </summary>
        public ILogger lokiLogger; // 

            /// <summary> The main meter provider for all of the prometheus metrics. 
            ///           local: http://localhost:9090/api/v1/otlp/v1/metrics  
            ///           sandbox: http://172.18.46.211:9090/api/v1/otlp/v1/metrics
            /// </summary>
        public MeterProvider meterProvider;
            /// <summary> Our Meter object (the base for all of the metric instrumentation) </summary>
        public Meter myMeter;
            /// <summary> The counter for packets handled </summary>
        public Counter<long> TotalPacketsHandled;
            /// <summary> Tracks the memory use of the backend. </summary>
        public ObservableGauge<double> processMemory;
            /// <summary> Tracks average time for packet ingress/egress </summary>
        public Histogram<double> packetHandling;


        /// <summary> 
        ///  Class Name: Backend  <br/><br/> 
        ///
        ///  Description: Overload 1/3. The main Backend Constructor. Initializes all of the fields. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  string <paramref name="ReceiveIp"/>    -    The IP being listened to. <br/>
        ///  string <paramref name="ReceivePort"/>  -    The Port being listened to. <br/>
        ///  string <paramref name="SendIp"/>       -    The IP being sent to. <br/>
        ///  string <paramref name="SendPort"/>     -    The Port being sent to. <br/>
        ///  int    <paramref name="newFrequency"/> -    The Frequency (number) at which the service reports inactivity <br/>
        ///  string <paramref name="newInterval"/>  -    The Interval (minute, day, hour) at which the service reports inactivity <br/>
        ///  string <paramref name="NameOfNIC"/>    -    The name of the NIC we're listening on <br/><br/> 
        ///  
        /// Returns: A Backend Object
        /// </summary>
        public Backend(string ReceiveIp, string ReceivePort, string SendIp, string SendPort, int newFrequency, string newInterval, 
                        string PromEndpoint, string LokiEndpoint, string NameOfNIC)
        {
                // system configuration set up fields
            this.receiveIp = ReceiveIp;
            this.receivePort = Convert.ToInt32(ReceivePort);
            this.sendIp = SendIp;
            this.sendPort = Convert.ToInt32(SendPort);
            this.frequency = newFrequency;
            this.interval = newInterval;
            this.promEndpoint = PromEndpoint;
            this.lokiEndpoint = LokiEndpoint;
            this.descriptionOfNIC = NameOfNIC;

                // windows event logger set up fields
            this.eventLog = new EventLog("UDP Packet Repeater");
            eventLog.Source = "UDP_Repeater_Backend";

            try
            {
                if (!Uri.IsWellFormedUriString(this.promEndpoint, UriKind.Absolute) ||
                    !Uri.IsWellFormedUriString(this.lokiEndpoint, UriKind.Absolute))
                {
                    throw new UriFormatException();
                }

                    // Loki event logger set up fields
                const string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} \t Backend/Service \t {Level} \n{Message}";
                this.lokiLogger = new LoggerConfiguration()
                                  .WriteTo.GrafanaLoki
                                  (
                                      this.lokiEndpoint,
                                      labels: new List<LokiLabel>
                                      {
                                      new LokiLabel(){ Key = "RepeaterSide", Value = "Backend/Service" },
                                      new LokiLabel(){ Key = "MachineName", Value = Environment.MachineName },
                                      new LokiLabel(){ Key = "User", Value = Environment.UserName }
                                      },
                                      textFormatter: new MessageTemplateTextFormatter(outputTemplate, null)
                                  )
                                  .Enrich.FromLogContext()
                                  .CreateLogger();

                    // Prometheus set up fields
                this.meterProvider = Sdk.CreateMeterProviderBuilder()
                                    .AddMeter("JT4.Repeater.MyLibrary")
                                    .AddOtlpExporter((exporterOptions, metricReaderOptions) =>
                                    {
                                        exporterOptions.Endpoint = new Uri(this.promEndpoint);
                                        exporterOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
                                        metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 5000;
                                    })
                                    .Build();
                this.myMeter = new Meter("JT4.Repeater.MyLibrary", "1.0");
                this.TotalPacketsHandled = myMeter.CreateCounter<long>("TotalPacketsHandled");
                this.processMemory = myMeter.CreateObservableGauge("backendMemory", () => GetProcessMemory());
                this.packetHandling = myMeter.CreateHistogram<double>("packetHandling");
            }
            catch (UriFormatException) 
            {
                eventLog.WriteEntry("Invalid endpoint configured, no monitoring currently.", EventLogEntryType.Warning, 9);
            }
        }


        /// <summary> 
        ///  Class Name: Backend  <br/><br/> 
        ///
        ///  Description: Overload 2/3. The Backend Constructor that just includes the config fields. 
        ///                             Used to construct the "newBackendObject". <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  Backend <paramref name="originalBackendObject"/> - The original backendObject to copy values from. <br/><br/> 
        ///  
        /// Returns: A Backend Object
        /// </summary>
        public Backend(Backend originalBackendObject)
        {
            this.receiveIp          =   originalBackendObject.receiveIp;
            this.receivePort        =   originalBackendObject.receivePort;
            this.sendIp             =   originalBackendObject.sendIp;
            this.sendPort           =   originalBackendObject.sendPort;
            this.frequency          =   originalBackendObject.frequency;
            this.interval           =   originalBackendObject.interval;
            this.promEndpoint       =   originalBackendObject.promEndpoint;
            this.lokiEndpoint       =   originalBackendObject.lokiEndpoint;
            this.descriptionOfNIC   =   originalBackendObject.descriptionOfNIC;
        }

        /// <summary> 
        ///  Class Name: Backend  <br/><br/> 
        ///
        ///  Description: Overload 3/3. The Backend Constructor for the start stop logging, so this <br/>
        ///               only initializes the two logging memebers. <br/><br/>
        ///
        ///  Inputs: None <br/><br/> 
        ///  
        /// Returns: A Backend Object
        /// </summary>
        public Backend()
        {
            
            if (!File.Exists("UDP_Repeater_Config.json"))   // actual path: "C:\\Windows\\SysWOW64\\UDP_Repeater_Config.json"
            {
                // if UDP_Repeater_Config.json doense't exist, it makes it and then poplulates it with this json string
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
                            ""ip"": ""127.0.0.1"",
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
                    ""monitoring"": {
                        ""prom"": ""Not Configured Yet"",
                        ""loki"": ""Not Configured Yet""
                    },
                    ""descriptionOfNIC"":""Not Configured Yet"" 
                }";

                // Write the JSON string to a file
                File.WriteAllText("UDP_Repeater_Config.json", defaults);
            }

                // get the loki endpoint form config.json
            string jsonString = File.ReadAllText("UDP_Repeater_Config.json");
            JObject jsonObject = JObject.Parse(jsonString);
            this.lokiEndpoint = (string)jsonObject["monitoring"]["loki"];

                // windows event logger set up
            this.eventLog = new EventLog("UDP Packet Repeater");
            eventLog.Source = "UDP_Repeater_Backend";

            try
            {
                if (!Uri.IsWellFormedUriString(this.lokiEndpoint, UriKind.Absolute))
                {
                    throw new UriFormatException();
                }

                // Loki event logger set up
                const string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} \t Backend/Service \t {Level} \n{Message}";
                this.lokiLogger = new LoggerConfiguration()
                                  .WriteTo.GrafanaLoki
                                  (
                                      this.lokiEndpoint,
                                      labels: new List<LokiLabel>
                                      {
                                          new LokiLabel(){ Key = "RepeaterSide", Value = "Backend/Service" },
                                          new LokiLabel(){ Key = "MachineName", Value = Environment.MachineName },
                                          new LokiLabel(){ Key = "User", Value = Environment.UserName }
                                      },
                                      textFormatter: new MessageTemplateTextFormatter(outputTemplate, null)
                                  )
                                  .Enrich.FromLogContext()
                                  .CreateLogger();
            }
            catch (UriFormatException) 
            {
                eventLog.WriteEntry("Invalid endpoint configured, no monitoring currently.", EventLogEntryType.Warning, 9);
            }
        }

        /// <summary> 
        ///  Class Name: Backend  <br/><br/> 
        ///
        ///  Description: Updates the main backendObject with updated settings. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  Backend <paramref name="newBackendObject"/> - The other Backend object to compare with <br/><br/>
        ///  
        ///  Returns: None
        /// </summary>
        public void UpdateWithNewBackendObject(Backend newBackendObject)
        {
            this.receiveIp = newBackendObject.receiveIp;
            this.receivePort = newBackendObject.receivePort;
            this.sendIp = newBackendObject.sendIp;
            this.sendPort = newBackendObject.sendPort;
            this.frequency = newBackendObject.frequency;
            this.interval = newBackendObject.interval;
        }


        /// <summary> Calculates and returns the current process memroy in bytes. </summary>
        public static double GetProcessMemory()
        {
            long bytes = Process.GetCurrentProcess().PrivateMemorySize64;
            return (bytes / 1024.0) / 1024.0;
        }
        /// <summary> Increments total packets handled for our prometheus metrics. </summary>
        public void IncrementTotalPacketsHandled()
        {
            if (TotalPacketsHandled == null)
            {
                return;
            }
            TotalPacketsHandled.Add(1);
        }
        /// <summary> Inputs the metric for our packet ingress/egress calculation. </summary>
        public void AddNewPacketTimeHandled(double stopWatchTime)
        {
            if (packetHandling == null)
            {
                return;
            }
            packetHandling.Record(stopWatchTime);
            if (stopWatchTime >= 4)
            {
                WarningLogger($"Packet handling time of {stopWatchTime}ms was greater than or equal to 4 milliseconds.");
            }
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
        public void ExceptionLogger(Exception e)
        {
            string[] formattedStackString = e.StackTrace.Split('\n');

            string message = String.Format($"Message: {e.Message} \n" +
                                           $"Stack trace: {formattedStackString.Last().TrimStart()}");

            eventLog.WriteEntry(message, EventLogEntryType.Error, 1);  // 1 is our id for backend errors
            if (lokiLogger != null)
            {
                lokiLogger.Error(message);
            }
        }

        /// <summary> 
        ///  Class Name: Backend  <br/> <br/>
        ///
        ///  Description: Logs Exceptions into the "UDP Packet Repeater" Windows Event Log without an object instance. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  Exception <paramref name="e"/> - An Exception to be logged <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public static void ExceptionLoggerStatic(Exception e)
        {
            string[] formattedStackString = e.StackTrace.Split('\n');


            string message = String.Format($"Message: {e.Message} \n" +
                                           $"location: Backend/Service. \n" +
                                           $"{formattedStackString.Last().TrimStart()}");

            using (EventLog logg = new EventLog("UDP Packet Repeater"))
            {
                logg.Source = "UDP_Repeater_Backend";

                // Write an entry to the event log.
                logg.WriteEntry(message, EventLogEntryType.Error, 1);  // 1 is our id for backend errors


                string jsonString = File.ReadAllText("UDP_Repeater_Config.json");
                JObject jsonObject = JObject.Parse(jsonString);
                string temporaryLokiEndpoint = (string)jsonObject["monitoring"]["loki"];
                const string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} \t Backend/Service \t {Level} {NewLine}{Message}";
                try
                {
                    var temporaryLokiLogger = new LoggerConfiguration()
                                  .WriteTo.GrafanaLoki
                                  (
                                      temporaryLokiEndpoint,
                                      labels: new List<LokiLabel>
                                      {
                                      new LokiLabel(){ Key = "RepeaterSide", Value = "Backend/Service" },
                                      new LokiLabel(){ Key = "MachineName", Value = Environment.MachineName },
                                      new LokiLabel(){ Key = "User", Value = Environment.UserName }
                                      },
                                      textFormatter: new MessageTemplateTextFormatter(outputTemplate, null)
                                  )
                                  .Enrich.FromLogContext()
                                  .CreateLogger();

                    temporaryLokiLogger.Error(message);
                    temporaryLokiLogger.Dispose();
                }
                catch (UriFormatException)
                {
                    logg.WriteEntry("Invalid endpoint configured, no monitoring currently.", EventLogEntryType.Warning, 9);
                }
            }
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
        public void InactivityLogger(int consecutiveEvents, int frequency, string interval)
        {
                // this whole fields is just to find if the interval word in the log eventLogMessage should have an 's' or not 
                // this has to be so much more complicated than it should be
            int totalTime = consecutiveEvents * frequency;
            string message;
            interval += "s";

            if (totalTime > 1 && frequency > 1)
            {
                message = String.Format("It has been {0} {1} since last packet was received. ", totalTime, interval);
                message += String.Format("The Service is currently configured to log inactivity every {0} {1}.", frequency, interval);
            }
            else if (totalTime > 1 && frequency == 1)
            {
                message = String.Format("It has been {0} {1} since last packet was received. ", totalTime, interval);
                interval = interval.Remove(interval.Length - 1);
                message += String.Format("The Service is currently configured to log inactivity every {0} {1}.", frequency, interval);
            }
            else
            {
                interval = interval.Remove(interval.Length - 1);
                message = String.Format("It has been {0} {1} since last packet was received. ", totalTime, interval);
                message += String.Format("The Service is currently configured to log inactivity every {0} {1}.", frequency, interval);
            }

            // Write an entry to the event log.
            eventLog.WriteEntry(message, EventLogEntryType.Warning, 3);     // 3 is the id for backend inactivity

            if (lokiLogger != null)
            {
                lokiLogger.Warning(message);
            }
        }

        /// <summary> 
        ///  Class Name: Backend  <br/><br/>
        ///
        ///  Description: Logs whever the service backend starts or stops. <br/><br/>
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
                eventLogMessage = "Repeater Service started.";
                lokiLogMessage = "Repeater Service \u001b[32mstarted\u001b[0m.";
            }
            else if (mode == "stop")
            {
                eventLogMessage = "Repeater Service stopped.";
                lokiLogMessage = "Repeater Service \u001B[31mstopped\u001B[0m.";
            }

            eventLog.WriteEntry(eventLogMessage, EventLogEntryType.Information, 4);     // 4 is id for backend start/stop

            if (lokiLogger != null)
            {
                lokiLogger.Information(lokiLogMessage);
            }
        }

        /// <summary> 
        ///  Class Name: Backend  <br/><br/>
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
            string actual = String.Format($"Message: {message} \n" +
                                          $"location: Backend/Service.");

            eventLog.WriteEntry(message, EventLogEntryType.Warning, 9);     // 9 is id for backend general warnings

            if (lokiLogger != null)
            {
                lokiLogger.Warning(message);
            }
        }
    }
}

