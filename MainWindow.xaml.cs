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
using System.Threading;
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

        ConvertStringToInstalledElement convertInstallList = new ConvertStringToInstalledElement();
        ConvertStringToInnoElement convertInnoList = new ConvertStringToInnoElement();
        CleanUpInstalledElementList cleanTheList = new CleanUpInstalledElementList();
        List<string> filterList = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnCompare_Click(object sender, RoutedEventArgs e)
        {
            //Start recording
            //Start install file
            //Stop recording
            //Convert recording into dictionary of installed elements
            //by comparing each element to the filter dictionary, and putting
            //the installed element into the dic at the same spot as the filter element
            //when installed elements dic and filter dic has same length
            //job done.

            Detection fileDetector = new Detection();
            Thread recorder = new Thread(new ThreadStart(fileDetector.InstanceMethod));
            //Start thread
            recorder.Start();
            executeInnoInstaller(this.txtBxPath.Text, this.txtBxInstaller.Text);
            fileDetector.stop();

            //End Thread
            recorder.Abort();
            Dictionary<int, WatchedElement> watchedElements = fileDetector.getWatchedElements();
            List<string> eventStringList = fileDetector.getEventList();
            List<string> cleanList = cleanTheList.doIt(eventStringList);
            Dictionary<int, FilterElement> filterElementsDic = createFilterElementDic();
            Dictionary<int, InstalledElement> installedElementsDic = createInstalledElementDic(cleanList);


            //compare()


        }

        private Dictionary<int, FilterElement> createFilterElementDic ()
        {
            Dictionary<int, FilterElement> filterElements = new Dictionary<int, FilterElement>();
            ConvertStringToInnoElement innoElementConverter = new ConvertStringToInnoElement();
            Dictionary<int, FilterElement> eventList = new Dictionary<int, FilterElement>();
            FilterElement filterElement;
            int idx = 0;
            foreach(string element in filterList)
            {
                filterElement = innoElementConverter.convertElement(element);
                if (filterElement != null)
                {
                    eventList.Add(idx, filterElement);
                    idx += 1;
                }
            }
            return filterElements;
        }



        private Dictionary<int, InstalledElement> createInstalledElementDic (List<string> cleanedList)
        {
            Dictionary<int, InstalledElement> eventList = new Dictionary<int, InstalledElement>();
            ConvertStringToInstalledElement installedElementConverter = new ConvertStringToInstalledElement();
            //converting the cleanList into a Dictionary of installedElements
            InstalledElement installedElement;
            int idx = 0;
            foreach (string element in cleanedList)
            {
                installedElement = installedElementConverter.convertElement(element);
                if (installedElement != null)
                {
                    /*if (eventList.ContainsValue(installedElement))
                    {
                        var item = eventList.First(elementForErase => elementForErase.Value == installedElement);
                        eventList.Remove(item.Key);
                    }*/
                    eventList.Add(idx, installedElement);
                    idx += 1;
                }
            }
            return eventList; 
        }

        private void btnCreateFilter_Click(object sender, RoutedEventArgs e)
        {

            executeInnounp(this.txtBxPath.Text, this.txtBxInstaller.Text);
        }

        private void executeInnoInstaller(string path, string fileName)
        {
            // Use ProcessStartInfo class
            ProcessStartInfo installerProces = new ProcessStartInfo();
            installerProces.CreateNoWindow = false;
            installerProces.UseShellExecute = false;
            installerProces.FileName = "\"" + fileName + "\"";
            installerProces.WorkingDirectory = "\"" + path + "\"";
            installerProces.WindowStyle = ProcessWindowStyle.Normal;
            //innounpProces.RedirectStandardOutput = true;

            //innounpProces.Arguments = "-v \"" + filename + "\"";

            //Executing process using working user.
            /*installerProces.UserName = "test";
            string rawPassword = "test";
            System.Security.SecureString encPassword = new System.Security.SecureString();
            foreach (Char c in rawPassword)
            {
                encPassword.AppendChar(c);
            }
            installerProces.Password = encPassword;*/


            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(installerProces))
                {
                    exeProcess.WaitForExit();
                    //int exitCode = exeProcess.ExitCode;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void executeInnounp(string path, string filename)
        {
            // run innounp.exe -v installerfile -> filter.txt
            // Read filter.txt, and put the filenames into a Dictionary
            
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
                                //filterElements.Add(counter, convertInnoList.convertElement(tmpLine));
                                line = line + tmpLine + "\n";
                            }
                        }
                        counter += 1;
                    }
                    exeProcess.WaitForExit();
                    //Removing the last install_script.iss entry
                    //filterElements.Remove(counter - 2);
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

        private void btnCreateJson_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
