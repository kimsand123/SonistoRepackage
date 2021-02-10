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


        Dictionary<int, WatchedElement> watchedList = new Dictionary<int, WatchedElement>();

        public Detection()
        {
            
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
                owner = (new FileInfo(e.FullPath).GetAccessControl().GetOwner(typeof(SecurityIdentifier)).Translate(typeof(NTAccount)) as NTAccount).Value;
                //TODO: "Administratorer" er sprog afhængigt. Lav OS languagecheck og brug konstant.

                if (owner.Contains("test"))
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

            string user = Environment.UserName;
            WatchedElement watchedElement = new WatchedElement();
            string owner = "";

            /*switch (e.ChangeType)
            {
                case WatcherChangeTypes.Changed:
                    try
                    {
                        owner = (new FileInfo(e.FullPath).GetAccessControl().GetOwner(typeof(SecurityIdentifier)).Translate(typeof(NTAccount)) as NTAccount).Value;

                        if (owner.Contains("BUILTIN\\" + "Administratorer"))
                        {
                            eventList.Add(">" + e.OldFullPath + "<" + e.ChangeType + ":" + owner);
                            eventList.Add(">" + e.FullPath + "<" + e.ChangeType + ":" + owner);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    break;
                case WatcherChangeTypes.Deleted:
                    try
                    {
                        owner = (new FileInfo(e.FullPath).GetAccessControl().GetOwner(typeof(SecurityIdentifier)).Translate(typeof(NTAccount)) as NTAccount).Value;

                        if (owner.Contains("BUILTIN\\" + "Administratorer"))
                        {
                            eventList.Add(">" + e.FullPath + "<Deleted" + e.ChangeType + ":" + owner);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    break;

            }*/
            try
            {
                owner = (new FileInfo(e.FullPath).GetAccessControl().GetOwner(typeof(SecurityIdentifier)).Translate(typeof(NTAccount)) as NTAccount).Value;

                if (owner.Contains("test"))
                {
                    eventList.Add(">" + e.OldFullPath + "<" + e.ChangeType + ":" + owner);
                    eventList.Add(">" + e.FullPath + "<" + e.ChangeType + ":" + owner);
                }
            }
            catch (Exception ex)
            {
            }




        }
        public Dictionary<int, WatchedElement> getWatchedElements()
        {
            return watchedList;
        }
    }
}
