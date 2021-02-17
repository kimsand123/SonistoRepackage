using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SonistoRepackage.InstallDetection
{
    class CleanUpInstalledElementList
    {
        List<string> elements = new List<string>();
        List<string> firstPassList = new List<string>();
        List<string> cleanedList = new List<string>();
        public CleanUpInstalledElementList()
        {

        }

        public List<string> doIt(List<string> installedELements)
        {
            elements = installedELements;

            firstPass();
            secondPass();

            return cleanedList;
        }

        private void firstPass()
        {
            string action = "";

            foreach (string element in elements)
            {
                action = determineAction(element);
                switch (action)
                {
                    case "Created":
                        created(element);
                        break;
                    case "Renamed":
                        renamed(element);
                        break;
                    case "Deleted":
                        deleted(element);
                        break;
                    case "Changed":
                        changed(element);
                        break;
                }
            }
        }

        private void secondPass()
        {
            // An event can be 
            // .. Valid file
            // .. Valid folder
            // if the folder or file still exists after the installation process is over
            // It is a strong candidate to keep.

            foreach (string element in firstPassList)
            {
                //if the file in the firstpass list still exists then add it to the cleanedList.
                //
                string fileAndPath = getFileFromEvent(element);
                //If event is either a valid file or directory, and they still exist
                if (Directory.Exists(fileAndPath) || File.Exists(fileAndPath))
                {
                    cleanedList.Add(fileAndPath);
                }
            }
        }

        private string getFileFromEvent(string element)
        {
            //Getting the filestring from the elementstring
            int changeTypeIdx = element.IndexOf(@"<");
            int stringLength = element.Length - 1;
            string filePathInfo = element.Substring(1, stringLength - (stringLength - changeTypeIdx + 1));
            return filePathInfo;
        }

        private string determineAction (string element)
        {
            int actionColonIdx = element.LastIndexOf(@":");
            int changeTypeIdx = element.IndexOf(@"<");
            string action = element.Substring(changeTypeIdx + 1, actionColonIdx - changeTypeIdx - 1);
            return action;
        }

        private void created(string element)
        {
            firstPassList.Add(element);
        }

        private void renamed(string element)
        {
            //because a rename is shown as an oldpath and then directly followed by a new path
            //the first old path will be removed from the firstpasslist, and when the new comes,
            //which it will next time, it will be created.

            //if (registerRenamedActionInList)
            string test = "";
            element = element.Replace("Renamed", "Created");
            //check if the firstpasslist contains the event with the temp file. 
            test = firstPassList.FirstOrDefault(s => s.Contains(element));

            //if test = null then the there is no temp file
            //present for the rename, and it is added
                if (test==null)
            {
                firstPassList.Add(element);
            }
            else
            {
                firstPassList.Remove(element);
            }
        }

        private void deleted(string element)
        {
            firstPassList.Remove(element);
        }

        private void changed(string element)
        {
            //Nothing happens yet.
        }


    }
}
