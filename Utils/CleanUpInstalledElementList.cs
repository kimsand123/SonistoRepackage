using System;
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
        List<string> filterFileElements = File.ReadAllLines(SettingsAndData.Instance.filterFile).ToList();
        public CleanUpInstalledElementList()
        {

        }

        public List<string> doIt(List<string> installedELements)
        {
            elements = installedELements;

            firstPass();
            secondPass();
            //Could have chosen to put the filterFile pass into the detection class, but im not sure that the program can keep up the detection if 
            //the filterFile becomes too large.
            thirdPass();

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
                string fileAndPath = getFileAndPathFromEvent(element);
                //If event is either a valid file or directory, and they still exist
                if (Directory.Exists(fileAndPath) || File.Exists(fileAndPath))
                {
                    //TODO: figure out if the event is a part of a path in other events. If so do not add it.
                    if (checkFileAndPathForRelevans(fileAndPath))
                    {
                        cleanedList.Add(fileAndPath);
                    }
                }
            }
        }

        private void thirdPass()
        {
            //foreach element in the cleanedList check if that element contains one of the lines in the filter.
            //if it does, it should be deleted from the cleanedList, 
            for (int cleanListIdx = cleanedList.Count-1; cleanListIdx > -1; cleanListIdx--)
            {
                for (int filterListIdx = 0; filterListIdx < filterFileElements.Count; filterListIdx++) 
                { 
                    if (cleanedList[cleanListIdx].Contains(filterFileElements[filterListIdx]))
                    {
                        cleanedList.RemoveAt(cleanListIdx);
                        break;
                    }
                }
            }
            
            foreach (string cleanListElement in cleanedList)
            {
                foreach (string filterElement in filterFileElements)
                {
                    if (cleanListElement.Contains(filterElement))
                    {

                    }
                }

            }
        }

        private bool checkFileAndPathForRelevans(string fileAndPath)
        {
            //for hvert element i firstPassListen
            //undersøg om elementet er i filterpass filen, hvis den er skal det sorteres fra
            //returner false
            //hvis ikke check hvor mange gange fileAndPath indeholdes i firstpasslisten.
            //hvis det er der flere gange, så er det fordi det er en instans af events som bare er en 
            //del af en path. Derfor skal der returneres falsk da den skal sorteres fra.
            int nrOfApperances = 0;
            foreach (string element in firstPassList)
            {
                if (element.Contains(fileAndPath))
                {
                    nrOfApperances += 1;
                }
            }

            //filter if the fileAndPath is in the firstPassList more than once. i.e. is just a part of a path 
            if (nrOfApperances > 1)
            {
                return false;
            }
            return true;
        }

        private string getFileAndPathFromEvent(string element)
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
