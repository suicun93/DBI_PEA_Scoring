using System;
using System.Windows.Forms;
using DBI_PEA_Grading.UI;

namespace DBI_PEA_Grading
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ImportMaterial());
        }
    }
}