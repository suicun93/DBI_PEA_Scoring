using System;
using System.Windows.Forms;
using DBI_PEA_Scoring.UI;

namespace DBI_PEA_Scoring
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ImportMaterial());
        }
    }
}
