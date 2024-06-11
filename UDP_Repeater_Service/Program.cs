//----------------------------------------------------
// File Name: Program.cs
// 
// Description: This file just is the entry point for 
//              the service. It creates a new instance
//              of the service class and runs it.
//
// Language:         Visual C#
// Target:           Windows PC
// Operating System: Windows 10 Enterprise
// Compiler:         Visual Studio .Net 2022
//
// Change History:
//
// Version  Date    Author          Description
// 1.0      ---     Jade Thomson    Initial Release
//---------------------------------------------------

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
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new UDP_Service()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }


}
