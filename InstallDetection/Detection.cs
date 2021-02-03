using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SonistoRepackage.InstallDetection
{
    public class Detection
    {
        bool startRecord = false;
        List<String> eventList = new List<string>();
        int totalNumberOfActivities = 0;
        int numberOfEntriesInList = 0;

        public Detection()
        {
            
        }

        public void InstanceMethod()
        {
            startRecord = true;
            var drives2 = DriveInfo.GetDrives();
            FileSystemWatcher watcher = new FileSystemWatcher();
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


            string owner = "";
            try
            {
                owner = (new FileInfo(e.FullPath).GetAccessControl().GetOwner(typeof(SecurityIdentifier)).Translate(typeof(NTAccount)) as NTAccount).Value;

                if (owner.Contains("BUILTIN\\" + "Administratorer"))
                {
                    eventList.Add("SoniFile:" + e.FullPath + "SoniAction:" + e.ChangeType + "SoniOwner:" + owner);
                    totalNumberOfActivities += 1;
                    numberOfEntriesInList += 1;
                }

            }
            catch (Exception ex)
            {
                /*eventList.Remove("| File:" + e.FullPath + " | Action:" + e.ChangeType + " | Owner:" + owner);
                numberOfEntriesInList -= 1;*/
            }
        }
        private void OnRenamed(object source, RenamedEventArgs e)
        {
            string user = Environment.UserName;
            string owner = "";
            try
            {
                owner = (new FileInfo(e.FullPath).GetAccessControl().GetOwner(typeof(SecurityIdentifier)).Translate(typeof(NTAccount)) as NTAccount).Value;

                if (owner.Contains("BUILTIN\\" + "Administratorer"))
                {
                    eventList.Add("|SoniFile:" + e.FullPath + "|SoniAction:" + e.ChangeType + "|SoniOwner:" + owner);
                    totalNumberOfActivities += 1;
                    numberOfEntriesInList += 1;
                }
            }
            catch (Exception ex)
            {
                //eventList.Remove("|File:" + e.FullPath + "|Action:" + e.ChangeType + "|Owner:" + owner);
                //numberOfEntriesInList -= 1;
            }


        }
    }
}
