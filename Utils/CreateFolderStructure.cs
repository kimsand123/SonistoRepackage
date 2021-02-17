using SonistoRepackage.Model;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SonistoRepackage
{
    class CreateFolderStructure
    {
        List<string> eventList;
        List<string> placeHolderFolderStructure;
        public CreateFolderStructure()
        {
        }

        public void createPlaceHolderStructure(List<string> eventList)
        {
            this.eventList = eventList;
            // for each of the elements in the eventlist passed to the object
            // create the placeholderpath
            List<string> placeHolderFolderStructure = new List<string>();
            foreach (string element in eventList)
            {
                //add the replaced path to the placeholder structure.
                placeHolderFolderStructure.Add(createPlaceholderPath(element));
            }
            this.placeHolderFolderStructure = placeHolderFolderStructure;
        }

        public void prepareWorkingFolder()
        {
            //Create the placeholder structure on disk
            //If package directory already exists, empty it just to be sure.
            if (Directory.Exists(SettingsAndData.Instance.workingFolder))
            {
                EmptyFolder(SettingsAndData.Instance.workingFolder);
            }
            else
            {
                //Otherwise create the packagefolder
                Directory.CreateDirectory(SettingsAndData.Instance.workingFolder);
            }
        }

        public void CreateFolders(string topfolder, List<PackageElement> placeholderFolderStructure)
        {
            Directory.CreateDirectory(SettingsAndData.Instance.workingFolder + "\\" + topfolder);
            string copyFolder = SettingsAndData.Instance.workingFolder + "\\" + topfolder;
            for (int idx = 0; idx < placeholderFolderStructure.Count; idx++) 
            {
                //Create the folder from the placeholderstructure
                //Problem when creating folders with no content or with content with no suffix.
                //If the file in the eventlist exists, then it is a file, if not it is a folder.
                if (File.Exists(eventList[idx])){
                    Directory.CreateDirectory(copyFolder + "\\" + Path.GetDirectoryName(placeholderFolderStructure[idx].placeHolderPath));
                    File.Copy(placeholderFolderStructure[idx].realPath, copyFolder + "\\" + placeholderFolderStructure[idx].placeHolderPath);
                } else
                {
                    Directory.CreateDirectory(copyFolder + "\\" + placeholderFolderStructure[idx].placeHolderPath);
                }
            }
        }

        // https://stackoverflow.com/questions/1288718/how-to-delete-all-files-and-folders-in-a-directory
        private bool EmptyFolder(string pathName)
        {
            bool errors = false;
            DirectoryInfo dir = new DirectoryInfo(pathName);

            foreach (FileInfo fi in dir.EnumerateFiles())
            {
                try
                {
                    fi.IsReadOnly = false;
                    fi.Delete();

                    //Wait for the item to disapear (avoid 'dir not empty' error).
                    while (fi.Exists)
                    {
                        System.Threading.Thread.Sleep(10);
                        fi.Refresh();
                    }
                }
                catch (IOException e)
                {
                    Debug.WriteLine(e.Message);
                    errors = true;
                }
            }

            foreach (DirectoryInfo di in dir.EnumerateDirectories())
            {
                try
                {
                    EmptyFolder(di.FullName);
                    di.Delete();

                    //Wait for the item to disapear (avoid 'dir not empty' error).
                    while (di.Exists)
                    {
                        System.Threading.Thread.Sleep(10);
                        di.Refresh();
                    }
                }
                catch (IOException e)
                {
                    Debug.WriteLine(e.Message);
                    errors = true;
                }
            }

            return !errors;
        }

        internal List<string> getFolders()
        {
            return placeHolderFolderStructure;
        }


        //Replace the relative paths with the placeholderfolders
        public string createPlaceholderPath(string element)
        {
            string placeholder = "";
            string path = element;
            string pathToLookFor = "";

            pathToLookFor = SettingsAndData.Instance.userDocFolder;
            if (path.Contains(pathToLookFor))
            {
                placeholder = "{userdocs}";
                path = path.Replace(pathToLookFor, placeholder);
            } else pathToLookFor = SettingsAndData.Instance.homeFolder;  
            if (path.Contains(pathToLookFor))
            {
                placeholder = "{home}";
                path = path.Replace(pathToLookFor, placeholder);
            } else pathToLookFor = SettingsAndData.Instance.pluginVst3_32Folder; 
            if (path.Contains(pathToLookFor))
            {
                placeholder = "{plugin}";
                //placeholder = @"{VST3-32}";
                path = path.Replace(pathToLookFor, placeholder);
            } else pathToLookFor = SettingsAndData.Instance.pluginVst3_64Folder; 
            if (path.Contains(pathToLookFor))
            {
                placeholder = "{plugin}";
                //placeholder = @"{VST3-64}";
                path = path.Replace(pathToLookFor, placeholder);
            } else pathToLookFor = SettingsAndData.Instance.pluginAaxFolder; 
            if (path.Contains(pathToLookFor))
            {
                placeholder = "{plugin}";
                //placeholder = @"{AAX}";
                path = path.Replace(pathToLookFor, placeholder);
            }
            else pathToLookFor = SettingsAndData.Instance.pluginAuFolder; 
            if (path.Contains(pathToLookFor))
            {
                placeholder = "{plugin}";
                //placeholder = @"{AU}";
                path = path.Replace(pathToLookFor, placeholder);
            }
            else pathToLookFor = SettingsAndData.Instance.pluginVst2_64Folder; 
            if (path.Contains(pathToLookFor))
            {
                placeholder = "{plugin}";
                //placeholder = "{VST2-64}";
                path = path.Replace(pathToLookFor, placeholder);
            }
            else pathToLookFor = SettingsAndData.Instance.pluginVst2_32Folder; 
            if (path.Contains(pathToLookFor))
            {
                placeholder = "{plugin}";
                //placeholder = "{VST2-32}";
                path = path.Replace(pathToLookFor, placeholder);
            } else
            {
                placeholder = "{absolute}";
                path = path.Replace(SettingsAndData.Instance.absoluteFolder, placeholder);
            }
            return path;
        }
    }
}
