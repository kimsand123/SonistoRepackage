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
using SonistoRepackage.InstallDetection;

namespace SonistoRepackage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private static readonly object synchLock = new object();

        ObservableCollection<String> eventList = new ObservableCollection<string>();
        int totalNumberOfActivities = 0;
        int numberOfEntriesInList = 0;


        Dictionary<string, FilterElement> filterElements = new Dictionary<string, FilterElement>();

        public MainWindow()
        {
            InitializeComponent();
            //rtbInfoWindow.Document.Blocks.Add(new Paragraph(new Run("test")));
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
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

                    while(eventList.Count < filterElements.Count)
                    {

                    }


                }
            }



            /*Detection installDetection = new Detection();
            List<string> eventList = installDetection.start();
            foreach (String evnt in eventList)
            {
                rtbInfoWindow.AppendText(evnt + "\n");
            }*/
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
                    lock (synchLock)
                    {
                        rtbInfoWindow.Document.Blocks.Add(new Paragraph(new Run(text)));
                        /*rtbInfoWindow.Document.Blocks.Add(new Paragraph(new Run(text)));
                        rtbInfoWindow.Focus();
                        rtbInfoWindow.ScrollToEnd();
                        rtbInfoWindow.UpdateLayout();*/
                    }

                    //OnPropertyChanged("textLine");

                    //eventList.Add("|File:" + e.FullPath + "|Action:" + e.ChangeType + "|Owner:" + owner);
                    totalNumberOfActivities += 1;
                    numberOfEntriesInList += 1;
                }
            }
            catch (Exception ex)
            {
                eventList.Remove("|File:" + e.FullPath + "|Action:" + e.ChangeType + "|Owner:" + owner + "\n");
                numberOfEntriesInList -= 1;
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

                    text = " |File:" + e.FullPath + " |Action:" + e.ChangeType + " |Owner:" + owner + "\n";
                    var paragraph = new Paragraph();
                    //paragraph.Inlines.Add(new Run(text));
                    rtbInfoWindow.Document.Blocks.Add(new Paragraph(new Run(text)));
                   /* rtbInfoWindow.Document.Blocks.Add(paragraph);
                    rtbInfoWindow.Focus();
                    rtbInfoWindow.ScrollToEnd();*/
                    //OnPropertyChanged("textLine");
                    //eventList.Add(" |File:" + e.FullPath + " |Action:" + e.ChangeType + " |Owner:" + owner);
                    totalNumberOfActivities += 1;
                    numberOfEntriesInList += 1;
                }
            }
            catch (Exception ex)
            {
                eventList.Remove("|File:" + e.FullPath + "|Action:" + e.ChangeType + "|Owner:" + owner);
                numberOfEntriesInList -= 1;
            }


        }

        private void rtbInfoWindow_TextChanged(object sender, TextChangedEventArgs e)
        {

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

                string arguments = "-v \"" + filename + "\"";
                
                innounpProces.Arguments = arguments;                
                
                try
                {
                    // Start the process with the info we specified.
                    // Call WaitForExit and then the using statement will close.
                    using (Process exeProcess = Process.Start(innounpProces))

                    {
                        string line = "";
                        while (!exeProcess.StandardOutput.EndOfStream)
                        {
                            line = line + exeProcess.StandardOutput.ReadLine() + "\n";
                        }
                        exeProcess.WaitForExit();
                        int idx = filename.IndexOf(".");
                        string filterFileName = filename.Substring(0, idx) + "_filter.txt";
                        string filterFile = path + filterFileName;
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
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
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

        }


        /*protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;*/
    }
}
