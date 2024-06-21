using System;
using System.Windows.Forms;
using System.IO;

namespace UDP_Repeater_GUI
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!File.Exists("Repeater_GUI_Log.txt"))
            {
                File.Create("Repeater_GUI_Log.txt");
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new gui_form());
        }
    }
}
