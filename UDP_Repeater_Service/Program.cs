//----------------------------------------------------
// File Name: Program.cs
// 
// Description: This file just is the entry point for 
//              the service. It creates a new instance
//              of the service class and runs it.
//
// Language:         Visual C#
// Target:           Windows PC
// Operating System: Windows 11 Enterprise
// Compiler:         Visual Studio .Net 2022
//
// Change History:
//
// Version   Date         Author            Description
//   1.0    6/21/24   Jade Pace Thomson   Initial Release
//---------------------------------------------------

using System;
using System.ServiceProcess;


namespace UDP_Repeater_Service
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var myService = new UDP_Service();

            if (Environment.UserInteractive)
            {
                myService.DebuggerProcess();
            }
            else
            {
                ServiceBase.Run(myService);
            }
        }
    }


}
