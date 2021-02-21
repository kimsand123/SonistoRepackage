using SonistoRepackage.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;

namespace SonistoRepackage.InstallDetection
{
    public class Detection
    {
        Log log = new Log();
        bool startRecord = false;
        List<String> eventList = new List<string>();
        ///string groupOwner = 

        public Detection()
        {
            //ConsoleManager.Show();
        }
        public void InstanceMethod()
        {
            startRecord = true;
            var drives2 = DriveInfo.GetDrives();
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.InternalBufferSize = 65536;
            //Setting up the watcher for each fixed drive
            foreach (DriveInfo drive in drives2)
            {
                //Getting the fixed drives only. Not network drives
                if (drive.DriveType == DriveType.Fixed)
                {
                    // look for drive and directories, 
                    watcher.Path = drive.Name;
                    watcher.IncludeSubdirectories = true;
                    watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                    watcher.Filter = "*.*";

                    //Eventhandlers being added 
                    watcher.Changed += new FileSystemEventHandler(OnChanged);
                    watcher.Created += new FileSystemEventHandler(OnChanged);
                    watcher.Deleted += new FileSystemEventHandler(OnChanged);
                    watcher.Renamed += new RenamedEventHandler(OnRenamed);

                    //Start watching
                    watcher.EnableRaisingEvents = true;
                }
            }
            while (startRecord) { }
            watcher.EnableRaisingEvents = false;
        }

        public List<String> getEventList()
        {
            return eventList;
        }

        public void stop()
        {
            startRecord = false;
        }
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            //reference
            //https://stackoverflow.com/questions/40449973/how-to-modify-file-access-control-in-net-core


            //string user = Environment.UserName;
            // WindowsPrincipal myPrincipal = (WindowsPrincipal)Thread.CurrentPrincipal;
            WatchedElement watchedElement = new WatchedElement();
            //Test
            string owner = "";
            try
            {
                owner = File.GetAccessControl(e.FullPath).GetOwner(typeof(SecurityIdentifier)).Translate(typeof(NTAccount)).ToString();
                log.write(owner);
                if (owner.Contains("Administratorer")|| owner.Contains("Administrators"))
                //if (owner.Contains("test"))
                {
                    eventList.Add(">" + e.FullPath + "<" + e.ChangeType + ":" + owner);
                }
            }
            catch (Exception ex)
            {
            }
            
        }
        private void OnRenamed(object source, RenamedEventArgs e)
        {
            //When renaming there will always be two entries in the dictionary
            //first the oldpath, after that the new path.

            string owner = "";
            try
            {
                owner = File.GetAccessControl(e.FullPath).GetOwner(typeof(SecurityIdentifier)).Translate(typeof(NTAccount)).ToString();
                log.write(owner);
                if (owner.Contains("Administratorer") || owner.Contains("Administrators"))
                //if (owner.Contains("test"))
                {
                    eventList.Add(">" + e.OldFullPath + "<" + e.ChangeType + ":" + owner);
                    eventList.Add(">" + e.FullPath + "<" + e.ChangeType + ":" + owner);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
