//----------------------------------------------------
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
// Version   Date         Author            Description
//   1.0    7/3/24    Jade Pace Thomson   Initial Release
//---------------------------------------------------

using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Linq;
using System.IO;
using Serilog;
using Serilog.Sinks.Grafana.Loki;
using System.Collections.Generic;
using System.Threading;
using Serilog.Formatting.Display;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Exporter;
using System.Diagnostics.Metrics;



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
            /// <summary> The device.Description of the network card that we're listening on. </summary>
        public string descriptionOfNIC;


            /// <summary> Our local windows event log. </summary>
        public EventLog eventLog;
            /// <summary> The loki log that our logs get sent to, in addition to the windows log. </summary>
        public ILogger lokiLogger;


            /// <summary> The main meter provider for all of the prometheus metrics </summary>
        public MeterProvider meterProvider;
            /// <summary> Our Meter object (the base for all of the metric instrumentation) </summary>
        public readonly Meter myMeter;
            /// <summary> The counter for packets handled </summary>
        public readonly Counter<long> TotalPacketsHandled;
            /// <summary> Tracks the memory use of the backend. </summary>
        public readonly ObservableGauge<long> processMemory;
            /// <summary> Tracks average time for packet ingress/egress </summary>
        public Histogram<long> packetHandling;





        /// <summary> 
        ///  Class Name: Backend  <br/><br/> 
        ///
        ///  Description: Overload 1/3. The Backend Constructor for when the service first starts. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  string <paramref name="ReceiveIp"/> - The IP being listened to. <br/>
        ///  string <paramref name="ReceivePort"/> - The Port being listened to. <br/>
        ///  string <paramref name="SendIp"/> - The IP being sent to. <br/>
        ///  string <paramref name="SendPort"/> - The Port being sent to. <br/>
        ///  int    <paramref name="newFrequency"/> - The Frequency (number) at which the service reports inactivity <br/>
        ///  string <paramref name="newInterval"/> - The Interval (minute, day, hour) at which the service reports inactivity <br/>
        ///  string <paramref name="NameOfNIC"/> - The name of the NIC we're listening on <br/><br/> 
        ///  
        /// Returns: A Backend Object
        /// </summary>
        public Backend(string ReceiveIp, string ReceivePort, string SendIp, string SendPort, int newFrequency, string newInterval, string NameOfNIC)
        {

                // system configuration set up section
            this.receiveIp = ReceiveIp;
            this.receivePort = Convert.ToInt32(ReceivePort);
            this.sendIp = SendIp;
            this.sendPort = Convert.ToInt32(SendPort);
            this.frequency = newFrequency;
            this.interval = newInterval;
            this.descriptionOfNIC = NameOfNIC;

                // windows event logger set up section
            this.eventLog = new EventLog("UDP Packet Repeater");
            eventLog.Source = "UDP_Repeater_Backend";
            eventLog.MaximumKilobytes = 256;


                // Loki event logger set up section
            const string outputTemplate = "Backend/Service \t {Level} \n{Message}";
            this.lokiLogger = new LoggerConfiguration()
                              .WriteTo.GrafanaLoki
                              (
                                  "http://172.18.46.211:3100",
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

                // Prometheus set up section
            this.meterProvider = Sdk.CreateMeterProviderBuilder()
                                .AddMeter("JT4.Repeater.MyLibrary")
                                .AddOtlpExporter((exporterOptions, metricReaderOptions) =>
                                {
                                    exporterOptions.Endpoint = new Uri("http://172.18.46.211:9090/api/v1/otlp/v1/metrics");
                                    exporterOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
                                    metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 5000;
                                })
                                .Build();
            this.myMeter = new Meter("JT4.Repeater.MyLibrary", "1.0");
            this.TotalPacketsHandled = myMeter.CreateCounter<long>("TotalPacketsHandled");
            this.processMemory = myMeter.CreateObservableGauge("backendMemory", () => GetProcessMemory());
            this.packetHandling = myMeter.CreateHistogram<long>("packetHandling");
        }


        /// <summary> 
        ///  Class Name: Backend  <br/><br/> 
        ///
        ///  Description: Overload 2/3. The Backend Constructor that doesn't take the name of the NIC <br/>
        ///               Instead, it just reads it from UDP_Repeater_Config.json and then assigns it. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  string <paramref name="ReceiveIp"/> - The IP being listened to. <br/>
        ///  string <paramref name="ReceivePort"/> - The Port being listened to. <br/>
        ///  string <paramref name="SendIp"/> - The IP being sent to. <br/>
        ///  string <paramref name="SendPort"/> - The Port being sent to. <br/>
        ///  int    <paramref name="newFrequency"/> - The Frequency (number) at which the service reports inactivity <br/>
        ///  string <paramref name="newInterval"/> - The Interval (minute, day, hour) at which the service reports inactivity <br/><br/> 
        ///  
        /// Returns: A Backend Object
        /// </summary>
        public Backend(string ReceiveIp, string ReceivePort, string SendIp, string SendPort, int newFrequency, string newInterval)
        {
                // system configuration set up section
            this.receiveIp = ReceiveIp;
            this.receivePort = Convert.ToInt32(ReceivePort);
            this.sendIp = SendIp;
            this.sendPort = Convert.ToInt32(SendPort);
            this.frequency = newFrequency;
            this.interval = newInterval;
            JObject jsonObject = JObject.Parse(File.ReadAllText("UDP_Repeater_Config.json"));
            this.descriptionOfNIC = (string)jsonObject["descriptionOfNIC"];


                // windows event logger set up section
            this.eventLog = new EventLog("UDP Packet Repeater");
            eventLog.Source = "UDP_Repeater_Backend";


                // Loki event logger set up section
            const string outputTemplate = "Backend/Service \t {Level} \n{Message}";
            this.lokiLogger = new LoggerConfiguration()
                              .WriteTo.GrafanaLoki
                              (
                                  "http://172.18.46.211:3100",
                                  labels: new List<LokiLabel>
                                  {
                                      new LokiLabel(){ Key = "RepeaterSide", Value = "Backend/Service" },
                                      new LokiLabel(){ Key = "MachineName", Value = Environment.MachineName },
                                      new LokiLabel(){ Key = "User", Value = Environment.UserName },
                                      new LokiLabel(){ Key = "Level", Value = "{Level}" }

                                  },
                                  textFormatter: new MessageTemplateTextFormatter(outputTemplate, null)
                              )
                              .Enrich.FromLogContext()
                              .CreateLogger();

                // Prometheus metric tracker section
            this.meterProvider = Sdk.CreateMeterProviderBuilder()
                                .AddMeter("JT4.Repeater.MyLibrary")
                                .AddOtlpExporter((exporterOptions, metricReaderOptions) =>
                                {
                                    exporterOptions.Endpoint = new Uri("http://172.18.46.211:9090/api/v1/otlp/v1/metrics");
                                    exporterOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
                                    metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 5000;
                                })
                                .Build();
            this.myMeter = new Meter("JT4.Repeater.MyLibrary", "1.0");
            this.TotalPacketsHandled = myMeter.CreateCounter<long>("TotalPacketsHandled");
            this.processMemory = myMeter.CreateObservableGauge("backendMemory", () => GetProcessMemory());
            this.packetHandling = myMeter.CreateHistogram<long>("packetHandling");
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
                // windows event logger set up
            this.eventLog = new EventLog("UDP Packet Repeater");
            eventLog.Source = "UDP_Repeater_Backend";

                // Loki event logger set up
            const string outputTemplate = "Backend/Service \t {Level} \n{Message}";

            this.lokiLogger = new LoggerConfiguration()
                              .WriteTo.GrafanaLoki
                              (
                                  "http://172.18.46.211:3100",
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
        //public void tester()
        //{
        //    Random random = new Random();
        //    while (true)
        //    {
        //        AddNewPacketTimeHandled(random.Next(0, 10));
        //        Thread.Sleep(random.Next(100, 10000));
        //    }
        //}

        /// <summary> Calculates and returns the current process memroy in bytes. </summary>
        public static long GetProcessMemory()
        {
            Process proc = Process.GetCurrentProcess();
            return proc.PrivateMemorySize64;
        }
        /// <summary> Increments total packets handled for our prometheus metrics. </summary>
        public void IncrementTotalPacketsHandled()
        {
            TotalPacketsHandled.Add(1);
        }
        /// <summary> Inputs the metric for our packet ingress/egress calculation. </summary>
        public void AddNewPacketTimeHandled(long stopWatchTime)
        {
            packetHandling.Record(stopWatchTime);
        }


        /// <summary> 
        ///  Class Name: Backend  <br/><br/> 
        ///
        ///  Description: Returns if two Backend objects have identical attributes <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  Backend <paramref name="other"/> - The other Backend object to compare with <br/><br/>
        ///  
        ///  Returns:  bool – Whether the two objects are equal.
        /// </summary>
        public bool Equals(Backend other)
        {       
            return other != null &&
                   receiveIp == other.receiveIp &&
                   receivePort == other.receivePort &&
                   sendIp == other.sendIp &&
                   sendPort == other.sendPort &&
                   frequency == other.frequency &&
                   interval == other.interval &&
                   descriptionOfNIC == other.descriptionOfNIC;
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

            string message = String.Format($"Error Message: {e.Message} \n" +
                                           $"Error location: Backend/Service. \n" +
                                           $"{formattedStackString.Last().TrimStart()}");

            eventLog.WriteEntry(message, EventLogEntryType.Error, 1);  // 1 is our id for backend errors
            lokiLogger.Error(message);
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
            EventLog logg = new EventLog("UDP Packet Repeater");
            logg.Source = "UDP_Repeater_Backend";

            string[] formattedStackString = e.StackTrace.Split('\n');


            string message = String.Format($"Error Message: {e.Message} \n" +
                                           $"Error location: Backend/Service. \n" +
                                           $"{formattedStackString.Last().TrimStart()}");

            // Write an entry to the event log.
            logg.WriteEntry(message, EventLogEntryType.Error, 1);  // 1 is our id for backend errors
            logg.Dispose();

            const string outputTemplate = "Backend/Service \t {Level} {NewLine}{Message}";
            var temporaryLokiLogger = new LoggerConfiguration()
                              .WriteTo.GrafanaLoki
                              (
                                  "http://172.18.46.211:3100",
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
                // this whole section is just to find if the interval word in the log message should have an 's' or not 
                // this has to be so much more complicated than it should be
            int totalTime = consecutiveEvents * frequency;
            string message;
            interval += "s";

            if (totalTime > 1 && frequency > 1)
            {
                message = String.Format("It has been {0} {1} since last packet was received. ", totalTime, interval);
                message += String.Format("The Service is currenty configured to log inactivity every {0} {1}.", frequency, interval);
            }
            else if (totalTime > 1 && frequency == 1)
            {
                message = String.Format("It has been {0} {1} since last packet was received. ", totalTime, interval);
                interval = interval.Remove(interval.Length - 1);
                message += String.Format("The Service is currenty configured to log inactivity every {0} {1}.", frequency, interval);
            }
            else
            {
                interval = interval.Remove(interval.Length - 1);
                message = String.Format("It has been {0} {1} since last packet was received. ", totalTime, interval);
                message += String.Format("The Service is currenty configured to log inactivity every {0} {1}.", frequency, interval);
            }

            // Write an entry to the event log.
            eventLog.WriteEntry(message, EventLogEntryType.Warning, 3);     // 3 is the id for backend inactivity

            lokiLogger.Warning(message);
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
            string message = "";
            if (mode == "start")
            {
                message = "Repeater Service started.";   
            } 
            else if (mode == "stop")
            {
                message = "Repeater Service stopped.";
            }

            eventLog.WriteEntry(message, EventLogEntryType.Information, 4);     // 4 is id for backend start/stop

            lokiLogger.Information(message);
        }

        /// <summary> 
        ///  Class Name: Backend  <br/><br/>
        ///
        ///  Description: Logs general warnings. <br/><br/>
        ///
        ///  Inputs:  <br/>
        ///  string <paramref name="message"/> - The message to log. <br/><br/>
        ///  
        ///  Returns:  None
        /// </summary>
        public void WarningLogger(string message)
        {
            eventLog.WriteEntry(message, EventLogEntryType.Warning, 9);     // 9 is id for backend general warnings

            lokiLogger.Warning(message);
        }
    }
}

