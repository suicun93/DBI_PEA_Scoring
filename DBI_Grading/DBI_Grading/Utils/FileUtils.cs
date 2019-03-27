using System;
using System.IO;
using System.Windows.Forms;

namespace DBI_Grading.Utils
{
    internal class FileUtils
    {
        public static string SaveFileToLocation()
        {
            var fbd = new FolderBrowserDialog();

            // Show the FolderBrowserDialog.
            var result = fbd.ShowDialog();
            if (result == DialogResult.OK)
                return fbd.SelectedPath;
            return "";
        }

        /// <summary>
        ///     Get link to a file by browser
        /// </summary>
        /// <returns>
        ///     if  user choose a file return its path
        ///     if  no choice return empty string
        /// </returns>
        /// <exception cref="File">
        ///     When no file was found
        /// </exception>
        public static string GetFileLocation()
        {
            // Displays an OpenFileDialog so the user can select a File.  
            var ofd = new OpenFileDialog();
            ofd.Filter = @"Data File|*.dat";
            ofd.Title = @"Select a Data File";
            ofd.Multiselect = false;
            // If the user clicked OK in the dialog and  
            // a .DAT file was selected, take the local path of it.  
            if (ofd.ShowDialog() == DialogResult.OK)
                return ofd.FileName;
            throw new Exception("No file was found");
        }


        /// <summary>
        ///     Get Folder location by browsing directory
        /// </summary>
        /// <returns>
        ///     Uri
        ///     If something goes wrong, return null
        /// </returns>
        /// ///
        /// <exception cref="File">
        ///     When no file was found
        /// </exception>
        public static string GetFolderLocation()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                //Show dialog select a folder
                var result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    //Get Files in folder
                    return fbd.SelectedPath;
                throw new Exception("No folder was found");
            }
        }
    }
}