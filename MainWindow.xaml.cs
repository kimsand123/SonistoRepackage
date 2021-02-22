using System;
using System.Collections.Generic;

using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using SonistoRepackage.InstallDetection;
using SonistoRepackage.Model;
using SonistoRepackage.Tests;
using SonistoRepackage.Utils;
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
    //3. pass is the filterFile. If any of the strings in this file exists in the element, remove it.
    //Show clean list of installed elements in listbox
    //kill user marked elements from lists
    //create the placeHolderStructure for the cleaned list
    //assign each element to an install package. Default is all
    //Create package list for each install package by copying the file from its installed position to is package position
    //Reset
    //Job done.

    public partial class MainWindow : Window
    {
        List<ItemForListbox> listBoxItems = new List<ItemForListbox>();
        CreateFolderStructure placeHolderStructure = new CreateFolderStructure();
        List<string> eventStringList = null;
        List<string> cleanList = null;
        List<string> placeHolderFoldersList = null;
        Log log = new Log();

        public MainWindow()
        {
            initializeApplication();
            //Read the ini file and store it in SettingsAndData Singleton
            InitializeComponent();
            //Create the filterfile if it does not exist

            string filterFile = Path.Combine(Directory.GetCurrentDirectory(), SettingsAndData.Instance.filterFile);
            if (!File.Exists(filterFile))
            {
                using (StreamWriter sw = File.CreateText(filterFile))
                {

                }
            }
        }

        private void initializeApplication()
        {
            try
            {
                string settingsFile = "";
                if (SettingsAndData.Instance.deployBuild)
                {

                    string _filePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
                    _filePath = Directory.GetParent(Directory.GetParent(_filePath).FullName).FullName;
                    settingsFile = _filePath + "\\SonistoRepackageSettings.txt";

                }
                else
                {
                    settingsFile = Directory.GetCurrentDirectory() + "\\SonistoRepackageSettings.txt";
                }
                List<string> settingsFileELements = File.ReadAllLines(settingsFile).ToList();

                List<PropertyInfo> settingsProperties = new List<PropertyInfo>();
                foreach (PropertyInfo property in SettingsAndData.Instance.GetType().GetProperties())
                {
                    settingsProperties.Add(property);
                }


                //Foreach element in settingsfile, compare the attribute with each propertyName in the SettingsAndData.Instance
                //If they are equal, transfer the value and break.
                foreach (string element in settingsFileELements)
                {
                    int dividerPosition = element.IndexOf("|");
                    string attribute = element.Substring(0, dividerPosition);
                    string value = element.Substring(dividerPosition + 1, element.Length - dividerPosition - 1);

                    foreach (PropertyInfo property in settingsProperties)
                    {
                        if (attribute == property.Name)
                        {
                            property.SetValue(SettingsAndData.Instance, value);
                            break;
                        }
                    }
                }
            } catch (Exception e)
            {
                //log.write(e, "Exception");
            }
        }

        private void btnCreateInstallData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CleanUpInstalledElementList cleanTheList = new CleanUpInstalledElementList();

                //testArea
                if (SettingsAndData.Instance.test)
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

                    log.write(eventStringList, "XXXXXXXXXXXXXXEventlistXXXXXXXXXXXX");
                    log.write(cleanList, "XXXXXXXXXXXXXXCleanlistXXXXXXXXXXXX");

                    //First User pass of data created by the install proces
                    //Creating the total folder structure.

                    placeHolderStructure.createPlaceHolderStructure(cleanList);
                    placeHolderFoldersList = placeHolderStructure.getFolders();
                    //FillListBox (the clean list, the actual folders)
                    FillListBox();


                    //Måske man kan bruge original eventlisten - cleanList til at forbedre på filtret.
                    //Den resulterende liste skal renses.

                    btnCreateInstallData.Background = (Brush)Application.Current.Resources["DoneColor"];
                }
            } catch (Exception ex)
            {
                log.write(ex, "Exception");
            }
        }

        //Method to fill up the listbox
        private void FillListBox()
        {
            try
            {
                listBoxItems = new List<ItemForListbox>();
                //If the file in the cleanlist exists, then it is a file, if not it is a folder.
                //If it is a folder, then the last part of the textfile is not to be considered a file, but a folder.
                for (int idx = 0; idx < cleanList.Count; idx++)
                {
                    if (File.Exists(cleanList[idx]))
                    {
                        listBoxItems.Add(new ItemForListbox() { choices = new InstallationPackageChoice(), keepKill = new KeepKill(), path = Path.GetDirectoryName(cleanList[idx]), file = Path.GetFileName(cleanList[idx]) });
                    }
                    else
                    {
                        listBoxItems.Add(new ItemForListbox() { choices = new InstallationPackageChoice(), keepKill = new KeepKill(), path = cleanList[idx] });
                    }
                }
                this.lstBoxInfoWindow.ItemsSource = listBoxItems;
            }
            catch (Exception e)
            {
                log.write(e, "Exception");
            }
        }

        // Try to execute the installer with another user... Does not work properly
        // TODO: get the above to work
        private void executeInnoInstaller(string path, string fileName)
        {
            // Use ProcessStartInfo class
            //Remember to lower the security and UAC to lowest level on the computer
            ProcessStartInfo installerProces = new ProcessStartInfo();
            installerProces.CreateNoWindow = true;
            installerProces.UseShellExecute = false;
            installerProces.FileName = "\"" + path + fileName + "\"";
            installerProces.WorkingDirectory = path;
            installerProces.WindowStyle = ProcessWindowStyle.Normal;
            //installerProces.UserName = "test";
            //installerProces.PasswordInClearText = "test";

            try
            {
                // Start the process with the info specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(installerProces))
                {
                    exeProcess.WaitForExit();
                    //int exitCode = exeProcess.ExitCode;
                }
            }
            catch (Exception e)
            {
                log.write(e, "Exception");
            }
        }

        private void btnFindInstaller_Click(object sender, RoutedEventArgs e)
        {
            // https://www.c-sharpcorner.com/UploadFile/mahesh/openfiledialog-in-wpf/
            // Create OpenFileDialog
            try
            {
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
                btnFindInstaller.Background = (Brush)Application.Current.Resources["DoneColor"];
            } catch (Exception ex)
            {
                log.write(ex, "Exception");
            }
        }

        private void btnKillMarkedFiles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int numberOfElementsInListBox = listBoxItems.Count;
                for (int idx = numberOfElementsInListBox - 1; idx > -1; idx--)
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

                btnKillMarkedFiles.Background = (Brush)Application.Current.Resources["ButtonDefaultColor"];
            } catch (Exception ex)
            {
                log.write(ex, "Exception");
            }
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

            try
            {
                Dictionary<string, List<PackageElement>> packageLists = new Dictionary<string, List<PackageElement>>();
                List<string> differentCombinations = new List<string>();//a string build to show the package combination like "bit32vst2" or "all"

                //Getting the different combinations strings that is existent in this install and 
                //put them in a list.
                for (int idx = 0; idx < listBoxItems.Count; idx++)
                {
                    string packageChoiceString = generatePackageChoiceString(listBoxItems[idx].choices);
                    if (!differentCombinations.Contains(packageChoiceString))
                    {
                        differentCombinations.Add(packageChoiceString);
                    }
                }
                //If user hasnt made any architecture/format choice only ALL is present in the list
                if (differentCombinations.Count == 1)
                {
                    MessageBox.Show("You need to choose an architecture and format for at least one of the files.", "No Arch/Form");
                    return;
                }

                //Initialize the packageLists on the basis of how many combination strings there has been discovered
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
                        if (differentCombinations[properPackageIndex] == "all")
                        {
                            foreach (KeyValuePair<string, List<PackageElement>> x in packageLists)
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
                        zipThePackageFolder(key);
                    }
                }

                //Reset the application for another plugin
                MessageBox.Show("Packages created. Prepare for new Plugin", "Done");
                ResetApplication();
            } catch (Exception ex)
            {
                log.write(ex, "Exception");
            }
        }

        private void zipThePackageFolder(string key)
        {
            try
            {
                string startPath = SettingsAndData.Instance.workingFolder + "\\" + key;
                string zipPath = SettingsAndData.Instance.workingFolder + "\\" + key + ".zip";
                ZipFile.CreateFromDirectory(startPath, zipPath);
            } catch (Exception ex)
            {
                log.write(ex, "Exception");
            }
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

            btnFindInstaller.Background = (Brush)Application.Current.Resources["ButtonDefaultColor"];
            btnKillMarkedFiles.Background = (Brush)Application.Current.Resources["ButtonDefaultColor"];
            btnCreateInstallData.Background = (Brush)Application.Current.Resources["ButtonDefaultColor"];
            btnCreatePackages.Background = (Brush)Application.Current.Resources["ButtonDefaultColor"];


            if (Directory.Exists(SettingsAndData.Instance.workingFolder))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    Arguments = SettingsAndData.Instance.workingFolder,
                    FileName = "explorer.exe"
                };

                Process.Start(startInfo);
            }
            else
            {
                MessageBox.Show(string.Format("{0} Directory does not exist!", SettingsAndData.Instance.workingFolder));
            }
        }

        private string generatePackageChoiceString(InstallationPackageChoice element)
        {
            //returns ex. "allbit32vst3"
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
                if (element.vst3)
                {
                    result = String.Concat(result, "vst3");
                }
                if (element.aax)
                {
                    result = String.Concat(result, "aax");
                }
                if (element.vst2)
                {
                    result = String.Concat(result, "vst2");
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
                Button button = sender as Button;
                button.Background = (Brush)Application.Current.Resources["DoneColor"];
            }
        }

        private void btnFilterfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ItemForListbox listBoxElement = (ItemForListbox)getListBoxElement(sender);
                WriteToFile(listBoxElement.file);
                Button button = sender as Button;
                button.Background = (Brush)Application.Current.Resources["DoneColor"];
            }
            catch (Exception ex)
            {
                log.write(ex, "Exception");
            }
        }

        private void btnFilterpath_Click(object sender, RoutedEventArgs e)
        {
            ItemForListbox listBoxElement = (ItemForListbox)getListBoxElement(sender);
            var window = new PathPopup();
            window.txtBxPathResult.Text = listBoxElement.path;
            Button button = sender as Button;
            button.Background = (Brush)Application.Current.Resources["DoneColor"];
            try
            {
                if (window.ShowDialog() == true)
                {
                    WriteToFile(window.pathResult);

                }
            } catch (Exception ex)
            {
                log.write(ex, "Exception");
            }
        }

        private void WriteToFile(string filterText)
        {
            try {
                string filterFile = Path.Combine(Directory.GetCurrentDirectory(), SettingsAndData.Instance.filterFile);
                using (StreamWriter sw = File.AppendText(filterFile))
                {
                    sw.WriteLine(filterText);
                }
            } catch (Exception ex)
            {
                log.write(ex, "Exception");
            }
        }
    
        private void chkBxKill_Checked(object sender, RoutedEventArgs e)
        {
            ItemForListbox listBoxElement = (ItemForListbox)getListBoxElement(sender);
            if (listBoxElement.keepKill.kill == false)
            {
                listBoxElement.keepKill.kill = true;
            }
            else
            {
                listBoxElement.keepKill.kill = false;
            }
            btnKillMarkedFiles.Background = Brushes.Red;

            
            
            

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


    }
}
