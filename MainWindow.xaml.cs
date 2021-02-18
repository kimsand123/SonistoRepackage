using System;
using System.Collections.Generic;

using System.Diagnostics;
using System.IO;
using System.Threading;

using System.Windows;
using System.Windows.Controls;

using Microsoft.Win32;
using SonistoRepackage.InstallDetection;
using SonistoRepackage.Model;
using SonistoRepackage.Tests;
using SonistoRepackage.View;

using ItemForListbox = SonistoRepackage.InstallDetection.ItemForListbox;
using Path = System.IO.Path;

namespace SonistoRepackage
{
    //Find the installer
    //Start recording
    //Start install plugin
    //When installation is done.
    //Stop recording
    //Take list of elements created during installation and 
    //pass them through filters. 
    //1. pass is if the element is created or renamed they pass through the filter
    //2. pass is if the file still exists, and was not a temporary file.
    //Show clean list of installed elements in listbox
    //kill user marked elements from lists
    //create the placeHolderStructure for the cleaned list
    //assign each element to an install package. Default is all
    //Create package list for each install package
    //create and copy the filestructure from their real placement to their package placement
    //Job done.

    public partial class MainWindow : Window
    {

        List<ItemForListbox> listBoxItems = new List<ItemForListbox>();
        CreateFolderStructure placeHolderStructure = new CreateFolderStructure();
        List<string> eventStringList = null;
        List<string> cleanList = null;
        List<string> placeHolderFoldersList = null;

        public MainWindow()
        {
            InitializeComponent();
            if (!File.Exists(SettingsAndData.Instance.filterFile))
            {
                using (StreamWriter sw = File.CreateText(SettingsAndData.Instance.filterFile))
                {

                }
            }
        }

        private void btnCreateJson_Click(object sender, RoutedEventArgs e)
        {
            CleanUpInstalledElementList cleanTheList = new CleanUpInstalledElementList();

            //testArea
            if (SettingsAndData.TEST)
            {
                Testing tests = new Testing();


                //tests.testInstallationPackageDTO();
                //tests.testRadioButtonPopUp();

            }
            //testArea ending
            else
            {
                ConvertStringToInstalledElement installedElementConverter = new ConvertStringToInstalledElement();
                Detection fileDetector = new Detection();
                Thread recorder = new Thread(new ThreadStart(fileDetector.InstanceMethod));

                //Getting data from the installation proces
                //Start thread
                recorder.Start();
                executeInnoInstaller(this.txtBxPath.Text, this.txtBxInstaller.Text);
                fileDetector.stop();
                //End Thread
                recorder.Abort();
                //Getting data from the installation proces

                eventStringList = fileDetector.getEventList();
                cleanList = cleanTheList.doIt(eventStringList);

                //First User pass of data created by the install proces
                //Creating the total folder structure.

                placeHolderStructure.createPlaceHolderStructure(cleanList);
                placeHolderFoldersList = placeHolderStructure.getFolders();
                //FillListBox (the clean list, the actual folders)
                FillListBox();


                //Måske man kan bruge original eventlisten - cleanList til at forbedre på filtret.
                //Den resulterende liste skal renses.
            }
        }

        //Method to fill up the listbox
        private void FillListBox()
        {
            listBoxItems = new List<ItemForListbox>();
            //If the file in the cleanlist exists, then it is a file, if not it is a folder.
            //If it is a folder, then the last part of the textfile is not to be considered a file, but a folder.
            for (int idx = 0; idx < cleanList.Count; idx++)
            {
                if (File.Exists(cleanList[idx]))
                {
                        listBoxItems.Add(new ItemForListbox() {choices=new InstallationPackageChoice(), keepKill = new KeepKill(), path=Path.GetDirectoryName(cleanList[idx]), file=Path.GetFileName(cleanList[idx])}); 
                }
                else
                {
                    //listBoxItems.Add(new ItemForListbox() { choices = new InstallationPackageChoice(), keepKill = new KeepKill(), path = Path.GetDirectoryName(cleanList[idx]), file = "" });
                    listBoxItems.Add(new ItemForListbox() { choices = new InstallationPackageChoice(), keepKill = new KeepKill(), path = cleanList[idx] });
                }
            }
            this.lstBoxInfoWindow.ItemsSource = listBoxItems;
        }

        // Try to execute the installer with another user... Does not work properly
        // TODO: get the above to work
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
                string filename = file.Substring(lastOccurance + 1, to - lastOccurance);
                string path = file.Replace(filename, "");
                this.txtBxPath.Text = path;
                this.txtBxInstaller.Text = filename;
            }
        }

        private void btnKillMarkedFiles_Click(object sender, RoutedEventArgs e)
        {
            int numberOfElementsInListBox = listBoxItems.Count;
            for (int idx = 0; idx < numberOfElementsInListBox; idx++)
            {
                if (listBoxItems[idx].keepKill.kill == true)
                {
                    listBoxItems.RemoveAt(idx);
                    cleanList.RemoveAt(idx);
                    placeHolderFoldersList.RemoveAt(idx);
                    numberOfElementsInListBox -= 1;
                }
            }
            FillListBox();
        }

        private void btnCreatePackages_Click(object sender, RoutedEventArgs e)
        {
            //Find all the kombinations of arcchitecture and format in the listBoxItems
            //create a list 
            //For each element in listBoxItems
            //get the architecture and format of the plugin 
            //
            //if it is .all then it will always be added
            //check if this kombination has been before.
            //if so add the element to the proper kombination list, with the belonging element from cleanlist
            //

            Dictionary<string, List<PackageElement>> packageLists = new Dictionary<string, List<PackageElement>>();
            List<string> differentCombinations = new List<string>();//a string build to show the package combination like "bit32vst2" or "all"

            //Getting the different combinations strings that is existent in this install and 
            //put them in a list to use later so that it is converted to an integer index.
            for (int idx = 0; idx < listBoxItems.Count; idx++)
            {
                string packageChoiceString = generatePackageChoiceString(listBoxItems[idx].choices);
                if (!differentCombinations.Contains(packageChoiceString))
                {
                    differentCombinations.Add(packageChoiceString);
                }
            }

            //Initialize the packageLists on the basis of how many combination strings there has been discovered
            //so the first packagelist will be recognized on the first combination string.
            for (int idx = 0; idx < differentCombinations.Count; idx++)
            {
                packageLists.Add(differentCombinations[idx], new List<PackageElement>());
            }

            //disperse the original list into the proper packagelists on the basis of the index of the combinationlist
            for (int idx = 0; idx < listBoxItems.Count; idx++)
            {
                PackageElement packageElement = new PackageElement();
                packageElement.placeHolderPath = placeHolderFoldersList[idx];
                packageElement.realPath = cleanList[idx];

                List<PackageElement> list = null;
                //converting the combinationstring into the proper int.
                int properPackageIndex = differentCombinations.IndexOf(generatePackageChoiceString(listBoxItems[idx].choices));

                //If the key in the packaglist contains the packagecombination string
                if (packageLists.ContainsKey(differentCombinations[properPackageIndex]))
                {
                    //if all add it to all the lists.
                    if(differentCombinations[properPackageIndex] == "all")
                    {
                        foreach(KeyValuePair<string, List<PackageElement>> x in packageLists)
                        {
                            List<PackageElement> y = x.Value;
                            y.Add(packageElement);
                        }
                    }
                    else //add the element to the proper list
                    {
                        list = packageLists[differentCombinations[properPackageIndex]];
                        list.Add(packageElement);
                    }
                }
            }

            //clear the working folder
            placeHolderStructure.prepareWorkingFolder();

            //For each package list, create and copy the files and folders from their real position to their package positions.
            foreach (KeyValuePair<string, List<PackageElement>> installPackage in packageLists) 
            {
                string key = installPackage.Key;
                if (key != "all")
                {
                    placeHolderStructure.CreateFolders(key, installPackage.Value);
                }
            }
            


            MessageBox.Show("Packages created. Prepare for new Plugin", "Done");
            ResetApplication();


        }

        private void ResetApplication()
        {

            listBoxItems.Clear(); 
            eventStringList.Clear();
            cleanList.Clear();
            placeHolderFoldersList.Clear();

            FillListBox();
            this.txtBxInstaller.Text = "";
            this.txtBxPath.Text = "";

        }

        private string generatePackageChoiceString(InstallationPackageChoice element)
        {
            string result = "";
            if (element.all)
            {
                result = String.Concat("all");
            }
            else
            {
                if (element.bit32)
                {
                    result = String.Concat("bit32");
                }
                if (element.bit64)
                {
                    result = String.Concat(result, "bit64");
                }
                if (element.vst2)
                {
                    result = String.Concat(result, "vst2");
                }
                if (element.vst3)
                {
                    result = String.Concat(result, "vst3");
                }
                if (element.aax)
                {
                    result = String.Concat(result, "aax");
                }
            }
            return result;
        }


        //ListBox controls
        private void btnPackageVersion_Click(object sender, RoutedEventArgs e)
        {
            ItemForListbox listBoxElement = (ItemForListbox)getListBoxElement(sender);
            InstallationPackageChoice elementChoice = listBoxElement.choices;
            InstallationPackagePopup popup = new InstallationPackagePopup(elementChoice);

            if ((bool)popup.ShowDialog() && popup.DialogResult.Value == true)
            {
                listBoxElement.choices = popup.getChoices();
            }
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
            window.txtBxPathResult.Text = listBoxElement.path;

            if (window.ShowDialog() == true)
            {
                WriteToFile(window.pathResult);
            }
        }

        private void WriteToFile(string filterText)
        {
            using (StreamWriter sw = File.AppendText(SettingsAndData.Instance.filterFile))
            {
                sw.WriteLine(filterText);
            }
        }

        private object getListBoxElement(object sender)
        {
            int index = 0;
            switch (sender.GetType().Name)
            {
                case "Button":
                    Button button = sender as Button;
                    index = this.lstBoxInfoWindow.Items.IndexOf(button.DataContext);
                    break;
                case "CheckBox":
                    CheckBox box = sender as CheckBox;
                    index = this.lstBoxInfoWindow.Items.IndexOf(box.DataContext);
                    break;
            }
            return this.lstBoxInfoWindow.Items[index];
        }

        private void chkBxKeep_Checked(object sender, RoutedEventArgs e)
        {
            ItemForListbox listBoxElement = (ItemForListbox)getListBoxElement(sender);
            listBoxElement.keepKill.keep = true;          
        }

        private void chkBxKill_Checked(object sender, RoutedEventArgs e)
        {
            ItemForListbox listBoxElement = (ItemForListbox)getListBoxElement(sender);
            listBoxElement.keepKill.kill = true;
        }

        //Code concerning user handling of process SAVE UNTIL IDEA ABANDONED

        /*private System.Security.SecureString getSecurePassword(string passwordText)
        {
            System.Security.SecureString encPassword = new System.Security.SecureString();

            foreach (System.Char c in passwordText)
            {
                encPassword.AppendChar(c);
            }

            return encPassword;
        }*/


        //Code to die-----------------------------------------------------------------------------------
        //Code to die-----------------------------------------------------------------------------------
        //Code to die-----------------------------------------------------------------------------------
        //Code to die-----------------------------------------------------------------------------------
        //Code to die-----------------------------------------------------------------------------------
        //Code to die-----------------------------------------------------------------------------------


        /*  private void txtBxLogfile_GotFocus(object sender, RoutedEventArgs e)
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
          }*/

        /* private void btnSelectJsonPath_Click(object sender, RoutedEventArgs e)
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
         }*/

        /* private void btnCreateFilter_Click(object sender, RoutedEventArgs e)
         {
             // run innounp.exe -v installerfile -> filter.txt
             // Read filter.txt, and put the filenames into a Dictionary
             //executeInnounp(this.txtBxPath.Text, this.txtBxInstaller.Text);
         }*/

        /*private void executeInnounp(string path, string filename)
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
        }*/
    }




}
