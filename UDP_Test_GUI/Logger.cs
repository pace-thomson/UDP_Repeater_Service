﻿//----------------------------------------------------
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
//   1.0    7/3/24    Jade Pace Thomson   Initial Release
//---------------------------------------------------


using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Collections.Generic;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Exporter;
using Serilog;
using Serilog.Sinks.Grafana.Loki;
using Serilog.Formatting.Display;



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
        public ILogger lokiLogger { get; set; }

            /// <summary> Our Meter object (the base for all of the metric instrumentation) </summary>
        private static readonly Meter myMeter = new Meter("JT4.Repeater.MyLibrary", "1.0");
            /// <summary> The main meter provider </summary>
        public MeterProvider meterProvider;
            /// <summary> Tracks the memory use of the gui </summary>
        public static readonly ObservableGauge<double> processMemory = myMeter.CreateObservableGauge("frontendMemory", () => GetProcessMemory());


        public Logger()
        {
            this.eventLog = new EventLog("UDP Packet Repeater");
            this.eventLog.Source = "UDP_Repeater_Frontend";

            const string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} \t Frontend/Interface \t {Level} {NewLine}{Message}";
            this.lokiLogger = new LoggerConfiguration()
                                .WriteTo.GrafanaLoki
                                (
                                    "http://172.18.46.211:3100",
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
                                    exporterOptions.Endpoint = new Uri("http://172.18.46.211:9090/api/v1/otlp/v1/metrics");
                                    exporterOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
                                    metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 5000;
                                })
                                .Build();
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
            return (bytes / 1024f) / 1024f;
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
            lokiLogger.Error(message);
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
                                            "IP Address: {1} \n" +
                                            "Port: {2}", editType, ip, port);

            eventLog.WriteEntry(message, EventLogEntryType.Information, 6);  // 6 is id for ip/port config change
            lokiLogger.Information(message);
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
                                           "Frequency: {0} \n" +
                                           "Interval: {1}", frequency, interval);

            eventLog.WriteEntry(message, EventLogEntryType.Information, 7);  // 7 is an inactivity config change
            lokiLogger.Information(message);
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
            string message = String.Format("The listening Network Interface Card was changed. \n" +
                                           "Description: {0} \n" +
                                           "Mac Address: {1}", description, macAddress);

            eventLog.WriteEntry(message, EventLogEntryType.Information, 8);  // 8 is a NIC config change
            lokiLogger.Information(message);
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
                message = "User Interface started.";
            }                                         
            else if (mode == "stop")                  
            {                                         
                message = "User Interface stopped.";
            }

            eventLog.WriteEntry(message, EventLogEntryType.Information, 5);     // 5 is id for frontend start/stop
            lokiLogger.Information(message);
        }
    }
}
