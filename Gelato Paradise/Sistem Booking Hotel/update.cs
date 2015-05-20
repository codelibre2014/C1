using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using Ionic.Zip;
using teboweb;

namespace Sistem_Booking_Hotel
{
    class update
    {
        public static bool getUpdateInfo(string downloadsURL, string versionFile, string resourceDownloadFolder, int startLine, string version)
        {
            string updateChecked = "";

            //create download folder if it does not exist
            if (!Directory.Exists(resourceDownloadFolder))
            {

                Directory.CreateDirectory(resourceDownloadFolder);

            }

            //let's try and download update information from the web
            updateChecked = webdata.downloadFromWeb(downloadsURL, versionFile, resourceDownloadFolder);
       
            //if the download of the file was successful
            if (!updateChecked.Equals(""))
            {

                //get information out of download info file
               // return populateInfoFromWeb(versionFile, resourceDownloadFolder, startLine, version);
                return populateInfoFromWeb(updateChecked,version);
            }
            //there is a chance that the download of the file was not successful
            else
            {

                return false;

            }

        }

        /// <summary>Download file from the web immediately</summary>
        /// <param name="downloadsURL">URL to download file from</param>
        /// <param name="filename">Name of the file to download</param>
        /// <param name="downloadTo">Folder on the local machine to download the file to</param>
        /// <param name="unzip">Unzip the contents of the file</param>
        /// <returns>Void</returns>
        // public static void installUpdateNow(string downloadsURL, string filename, string downloadTo, bool unzip)
        //{

        //     bool downloadSuccess = webdata.downloadFromWeb(downloadsURL, filename, downloadTo);

        //    if (unzip)
        //    {
        //        unZip(downloadTo + filename, downloadTo);
        //    }

        //}


        /// <summary>Starts the update application passing across relevant information</summary>
        /// <param name="downloadsURL">URL to download file from</param>
        /// <param name="filename">Name of the file to download</param>
        /// <param name="destinationFolder">Folder on the local machine to download the file to</param>
        /// <param name="processToEnd">Name of the process to end before applying the updates</param>
        /// <param name="postProcess">Name of the process to restart</param>
        /// <param name="startupCommand">Command line to be passed to the process to restart</param>
        /// <param name="updater"></param>
        /// <returns>Void</returns>
        public static void installUpdateRestart(string downloadsURL, string filename, string destinationFolder, string processToEnd, string postProcess, string startupCommand, string updater)
        {

            string cmdLn = "";

            cmdLn += "|downloadFile|" + filename;
            cmdLn += "|URL|" + downloadsURL;
            cmdLn += "|destinationFolder|" + destinationFolder;
            cmdLn += "|processToEnd|" + processToEnd;
            cmdLn += "|postProcess|" + postProcess;
            cmdLn += "|command|" + @" / " + startupCommand;

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = updater;
            startInfo.Arguments = cmdLn;
            Process.Start(startInfo);

        }



        private static bool populateInfoFromWeb(string versionFile, string version)
        {
            version = (Convert.ToInt32(version) + 1).ToString();
            if (versionFile.Replace(".", "").Contains("BookIn-" + version))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool unZip(string file, string unZipTo)//, bool deleteZipOnCompletion)
        {
            try
            {
                // Specifying Console.Out here causes diagnostic msgs to be sent to the Console
                // In a WinForms or WPF or Web app, you could specify nothing, or an alternate
                // TextWriter to capture diagnostic messages. 

                using (ZipFile zip = ZipFile.Read(file))
                {
                    // This call to ExtractAll() assumes:
                    //   - none of the entries are password-protected.
                    //   - want to extract all entries to current working directory
                    //   - none of the files in the zip already exist in the directory;
                    //     if they do, the method will throw.
                    zip.ExtractAll(unZipTo);
                }

                //if (deleteZipOnCompletion) File.Delete(unZipTo + file);

            }
            catch (System.Exception)
            {
                return false;
            }

            return true;
        }
    }
}
