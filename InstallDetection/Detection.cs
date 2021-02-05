using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace SonistoRepackage.InstallDetection
{
    public class Detection
    {
        bool startRecord = false;
        List<String> eventList = new List<string>();
        string processName = "";
        string workingDirectory = "";
        string filesData = "";


        Dictionary<int, WatchedElement> watchedList = new Dictionary<int, WatchedElement>();

        public Detection(string processName, string workingDirectory, string filesData)
        {
            this.processName = processName;
            this.workingDirectory = workingDirectory;
            this.filesData = filesData;
        }

        public void InstanceMethod()
        {
            handleSolution(processName, workingDirectory, filesData);
            //fileSystemWatcherSolution();

        }

        private void handleSolution(string processName, string workingDirectory, string filesData)
        {
            // Use ProcessStartInfo class
            ProcessStartInfo innounpProces = new ProcessStartInfo();
            innounpProces.CreateNoWindow = false;
            innounpProces.UseShellExecute = false;
            innounpProces.FileName = "handle64.exe";
            innounpProces.WorkingDirectory = workingDirectory;
            innounpProces.WindowStyle = ProcessWindowStyle.Normal;
            innounpProces.RedirectStandardOutput = true;
            innounpProces.Arguments = "-p \"" + processName + "\"";

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(innounpProces))
                {
                    string line = "";
                    string tmpLine;
                    int counter = 0;
                    while (!exeProcess.StandardOutput.EndOfStream)
                    {
                        tmpLine = exeProcess.StandardOutput.ReadLine();
                        //skip the first 3 lines of the datastream
                        if (counter > 2)
                        {
                            if (tmpLine.Contains(":"))
                            {
                                eventList.Add(tmpLine);
                                line = line + tmpLine + "\n";
                            }
                        }
                        counter += 1;
                    }
                    exeProcess.WaitForExit();

                    //Getting the filename, path

                    //Creating filename of filter file
                    int idx = filesData.IndexOf(".");
                    string filterFileName = filesData.Substring(0, idx) + "_filter.txt";
                    string filterFile = workingDirectory + filterFileName;

                    //if file exists delete. Create filter file using data from line
                    if (File.Exists(workingDirectory + filterFileName))
                    {
                        File.Delete(workingDirectory + filterFileName);
                    }
                    File.WriteAllText(workingDirectory + filterFileName, line);
                    //int exitCode = exeProcess.ExitCode;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void fileSystemWatcherSolution()
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

            WatchedElement watchedElement = new WatchedElement();
            //Test
            string owner = "";
            try
            {
                owner = (new FileInfo(e.FullPath).GetAccessControl().GetOwner(typeof(SecurityIdentifier)).Translate(typeof(NTAccount)) as NTAccount).Value;
                //TODO: "Administratorer" er sprog afhængigt. Lav OS languagecheck og brug konstant.

                if (owner.Contains("BUILTIN\\" + "Administratorer"))
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


        }
        public Dictionary<int, WatchedElement> getWatchedElements()
        {
            return watchedList;
        }
    }
}
