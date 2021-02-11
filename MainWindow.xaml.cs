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
using SonistoRepackage.View;
using ItemForListbox = SonistoRepackage.InstallDetection.ItemForListbox;
using Path = System.IO.Path;

namespace SonistoRepackage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class btnFilterPath_Click : Window
    {

        int numberOfEntriesInList = 0;

        Dictionary<int, InstalledElement> eventList = new Dictionary<int, InstalledElement>();
        Dictionary<int, FilterElement> filterElements = new Dictionary<int, FilterElement>();
        ConvertStringToInstalledElement convertInstallList = new ConvertStringToInstalledElement();
        ConvertStringToInnoElement convertInnoList = new ConvertStringToInnoElement();
        CleanUpInstalledElementList cleanTheList = new CleanUpInstalledElementList();
        string FILTERFILE = @"C:\Sonisto\RepackageFilter.txt";

        public btnFilterPath_Click()
        {
            InitializeComponent();
            if (!File.Exists(FILTERFILE))
            {
                using (StreamWriter sw = File.CreateText(FILTERFILE))
                {

                }
            }
        }

        private void btnCreateJson_Click(object sender, RoutedEventArgs e)
        {
            //Start recording
            //Start install file
            //Stop recording
            //Convert recording into dictionary of installed elements
            //by comparing each element to the filter dictionary, and putting
            //the installed element into the dic at the same spot as the filter element
            //when installed elements dic and filter dic has same length
            //job done.
            ConvertStringToInstalledElement installedElementConverter = new ConvertStringToInstalledElement();
            Detection fileDetector = new Detection();
            Thread recorder = new Thread(new ThreadStart(fileDetector.InstanceMethod));
            InstalledElement installedElement = new InstalledElement();

            //Start thread
            recorder.Start();

            executeInnoInstaller(this.txtBxPath.Text, this.txtBxInstaller.Text);
            fileDetector.stop();

            //End Thread
            recorder.Abort();
            //Dictionary<int, WatchedElement> watchedElements = fileDetector.getWatchedElements();
            List<string> eventStringList = fileDetector.getEventList();
            List<string> cleanList = cleanTheList.doIt(eventStringList);
            CreateFolderStructure folders = new CreateFolderStructure(cleanList);
            List<string> foldersList = folders.getFolders();
            FillListBox(cleanList, folders.getFolders());

        }

        private void FillListBox(List<string> clean, List<string> placeHolderFolders)
        {
            List<ItemForListbox> listBoxItems = new List<ItemForListbox>();



            //If the file in the cleanlist exists, then it is a file, if not it is a folder.
            for (int idx = 0; idx < clean.Count; idx++)
            {
                if (File.Exists(clean[idx]))
                {
                        listBoxItems.Add(new ItemForListbox() {path=Path.GetDirectoryName(clean[idx]), file=Path.GetFileName(clean[idx])}); 
                    
                }
                else
                {
                        listBoxItems.Add(new ItemForListbox() { path = Path.GetDirectoryName(clean[idx]), file = "" });
                }
            }

            this.lstBoxInfoWindow.ItemsSource = listBoxItems;
        }

        private void btnCreateFilter_Click(object sender, RoutedEventArgs e)
        {
            // run innounp.exe -v installerfile -> filter.txt
            // Read filter.txt, and put the filenames into a Dictionary
            executeInnounp(this.txtBxPath.Text, this.txtBxInstaller.Text);
        }

        private void executeInnoInstaller(string path, string fileName)
        {
            // Use ProcessStartInfo class
            //Remember to lower the security og UAC to lowest level on the computer
            ProcessStartInfo installerProces = new ProcessStartInfo();
            installerProces.CreateNoWindow = true;
            installerProces.UseShellExecute = false;
            installerProces.FileName = "\"" + path + fileName + "\"";
            installerProces.WorkingDirectory = path;
            installerProces.WindowStyle = ProcessWindowStyle.Normal;
            installerProces.UserName = "test";
            installerProces.PasswordInClearText = "test";

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
        private System.Security.SecureString getSecurePassword(string passwordText)
        {
            System.Security.SecureString encPassword = new System.Security.SecureString();

            foreach (System.Char c in passwordText)
            {
                encPassword.AppendChar(c);
            }

            return encPassword;
        }

        private void executeInnounp(string path, string filename)
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

        private void btnPackageVersion_Click(object sender, RoutedEventArgs e)
        {

        }



        private void btnFilterfile_Click(object sender, RoutedEventArgs e)
        {
            ItemForListbox listBoxElement = (ItemForListbox)getListBoxElement(sender);
            WriteToFile(listBoxElement.file);
        }
        private void btnFilterpath_Click(object sender, RoutedEventArgs e)
        {
            ItemForListbox listBoxElement = (ItemForListbox)getListBoxElement(sender);
            var window = new PathPopup();
            if (window.ShowDialog() == true)
            {
                WriteToFile(window.pathResult);
            }
        }

        private object getListBoxElement(object sender)
        {
            Button button = sender as Button;
            int index = this.lstBoxInfoWindow.Items.IndexOf(button.DataContext);
            return this.lstBoxInfoWindow.Items[index];
        }

        private void WriteToFile(string filterText)
        {
            using (StreamWriter sw = File.AppendText(FILTERFILE))
            {
                sw.WriteLine(filterText);
            }
        }


    }
}
