using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using JetBrains.Annotations;
using Microsoft.Win32;
using SonistoRepackage.InstallDetection;

namespace SonistoRepackage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {

        int numberOfEntriesInList = 0;

        Dictionary<int, InstalledElement> eventList = new Dictionary<int, InstalledElement>();
        Dictionary<int, FilterElement> filterElements = new Dictionary<int, FilterElement>();
        ConvertStringToInstalledElement convertInstallList = new ConvertStringToInstalledElement();
        ConvertStringToInnoElement convertInnoList = new ConvertStringToInnoElement();
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnCreateJson_Click(object sender, RoutedEventArgs e)
        {
            var drives2 = DriveInfo.GetDrives();
            FileSystemWatcher watcher = new FileSystemWatcher();
            //Setting up the watcher for each fixed drive
            foreach (DriveInfo drive in drives2)
            {
                //Getting the fixed drives only. Not network drives
                if (drive.DriveType == DriveType.Fixed)
                {

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

                    //Loop until eventlist is filled with the files of filterelements
                    while(eventList.Count < filterElements.Count)
                    {

                    }
                    watcher.EnableRaisingEvents = false;
                }
            }
        }
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            //reference
            //https://stackoverflow.com/questions/40449973/how-to-modify-file-access-control-in-net-core

            string user = Environment.UserName;
            string owner = "";
            string text = "";
            try
            {
                owner = (new FileInfo(e.FullPath).GetAccessControl().GetOwner(typeof(SecurityIdentifier)).Translate(typeof(NTAccount)) as NTAccount).Value;

                if (owner.Contains(user))
                {
                    text = " |File:" + e.FullPath + " |Action:" + e.ChangeType + " |Owner:" + owner + "\n";
                    eventList.Add(numberOfEntriesInList, convertInstallList.convertElement(e.FullPath));
                    //OnPropertyChanged("textLine");

                    //eventList.Add("|File:" + e.FullPath + "|Action:" + e.ChangeType + "|Owner:" + owner);
                    numberOfEntriesInList += 1;
                }
            }
            catch (Exception ex)
            {
                //eventList.Remove("|File:" + e.FullPath + "|Action:" + e.ChangeType + "|Owner:" + owner + "\n");
                //numberOfEntriesInList -= 1;
            }
        }
        private void OnRenamed(object source, RenamedEventArgs e)
        {
            string user = Environment.UserName;
            string owner = "";
            string text = "";
            try
            {
                owner = (new FileInfo(e.FullPath).GetAccessControl().GetOwner(typeof(SecurityIdentifier)).Translate(typeof(NTAccount)) as NTAccount).Value;

                if (owner.Contains(user))
                {
                    /*int to = e.FullPath.Length - 1;
                    int lastOccurance = e.FullPath.LastIndexOf(@"\");
                    string filename = e.FullPath.Substring(lastOccurance + 1, to - lastOccurance);
                    string path = e.FullPath.Replace(filename, "");*/

                    text = " |File:" + e.FullPath + " |Action:" + e.ChangeType + " |Owner:" + owner + "\n";
                    eventList.Add(numberOfEntriesInList, convertInstallList.convertElement(e.FullPath));
                    //eventList.Add(" |File:" + e.FullPath + " |Action:" + e.ChangeType + " |Owner:" + owner);
                    numberOfEntriesInList += 1;
                }
            }
            catch (Exception ex)
            {
                //eventList.Remove("|File:" + e.FullPath + "|Action:" + e.ChangeType + "|Owner:" + owner);
                //numberOfEntriesInList -= 1;
            }
        }

        private void btnCreateFilter_Click(object sender, RoutedEventArgs e)
        {
            // run innounp.exe -v installerfile -> filter.txt
            executeInnounp(this.txtBxPath.Text, this.txtBxInstaller.Text);
            // Read filter.txt, and put the filenames into a Dictionary
        }

        private void executeInnounp(string path, string filename)
        {
            {
                // Use ProcessStartInfo class
                ProcessStartInfo innounpProces = new ProcessStartInfo();
                innounpProces.CreateNoWindow = false;
                innounpProces.UseShellExecute = false;
                innounpProces.FileName = "innounp.exe";
                innounpProces.WorkingDirectory = path;
                innounpProces.WindowStyle = ProcessWindowStyle.Normal;
                innounpProces.RedirectStandardOutput = true;
                innounpProces.Arguments = "-v \"" + filename + "\"";                 
                
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
                            if ( counter > 2)
                            {
                                if (tmpLine.Contains(":"))
                                {
                                    filterElements.Add(counter, convertInnoList.convertElement(tmpLine));
                                    line = line + tmpLine + "\n";
                                }
                            }
                            counter += 1;
                        }
                        exeProcess.WaitForExit();
                        //Removing the last install_script.iss entry
                        filterElements.Remove(counter - 2);
                        //Getting the filename, path

                        //Creating filename of filter file
                        int idx = filename.IndexOf(".");
                        string filterFileName = filename.Substring(0, idx) + "_filter.txt";
                        string filterFile = path + filterFileName;

                        //if file exists delete. Create filter file using data from line
                        if (File.Exists(path + filterFileName))
                        {
                            File.Delete(path + filterFileName);
                        }
                        File.WriteAllText(path + filterFileName, line);
                        //int exitCode = exeProcess.ExitCode;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private void btnFindInstaller_Click(object sender, RoutedEventArgs e)
        {
            // https://www.c-sharpcorner.com/UploadFile/mahesh/openfiledialog-in-wpf/
            // Create OpenFileDialog
            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.DefaultExt = ".exe";
            openFileDlg.Filter = "Inno executable (.exe)| *.exe";
            openFileDlg.InitialDirectory = @"C:\Temp\";

            // Launch OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = openFileDlg.ShowDialog();
            // Get the selected file name and display in a TextBox.
            // Load content of file in a TextBlock
            if (result == true)
            {
                string file = openFileDlg.FileName;
                int to = file.Length - 1;
                int lastOccurance = file.LastIndexOf(@"\");
                string filename = file.Substring(lastOccurance+1, to-lastOccurance);
                string path = file.Replace(filename, "");
                this.txtBxPath.Text = path;
                this.txtBxInstaller.Text = filename;
            }
        }

        private void btnSelectJsonPath_Click(object sender, RoutedEventArgs e)
        {
            // https://www.c-sharpcorner.com/UploadFile/mahesh/openfiledialog-in-wpf/
            // Create OpenFileDialog
            SaveFileDialog saveFileDlg = new SaveFileDialog();
            saveFileDlg.DefaultExt = ".json";
            saveFileDlg.Filter = "Sonisto json (.json)| *.json";
            saveFileDlg.Title = "Select folder and filename for Sonisto JSON";

            // Launch OpenFileDialog by calling ShowDialog method
            //Nullable<bool> result = saveFileDlg.ShowDialog();
            saveFileDlg.ShowDialog();

            if (saveFileDlg.FileName != "")
            {
                string file = saveFileDlg.FileName;
                int to = file.Length - 1;
                int lastOccurance = file.LastIndexOf(@"\");
                string filename = file.Substring(lastOccurance + 1, to - lastOccurance);
                string path = file.Replace(filename, "");
                this.txtBxJsonPath.Text = path;
                this.txtBxJsonFileName.Text = filename;
            }
        }

        private void txtBxLogfile_GotFocus(object sender, RoutedEventArgs e)
        {
            // https://www.c-sharpcorner.com/UploadFile/mahesh/openfiledialog-in-wpf/
            // Create OpenFileDialog
            SaveFileDialog saveFileDlg = new SaveFileDialog();
            saveFileDlg.DefaultExt = ".log";
            saveFileDlg.Filter = "JsonInstall log (.log)| *.log";
            saveFileDlg.Title = "Select folder and filename for Sonisto Json logfile";

            // Launch OpenFileDialog by calling ShowDialog method
            //Nullable<bool> result = saveFileDlg.ShowDialog();
            saveFileDlg.ShowDialog();

            if (saveFileDlg.FileName != "")
            {
                this.txtBxLogfile.Text = saveFileDlg.FileName;
            }
        }
    }
}
