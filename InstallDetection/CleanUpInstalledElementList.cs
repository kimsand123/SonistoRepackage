using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            foreach (string element in firstPassList)
            {
                string curFile = getFileFromEvent(element);
                if (File.Exists(curFile)) 
                {
                    cleanedList.Add(curFile);
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
            //if (registerRenamedActionInList)
            string test = "";
            element = element.Replace("Renamed", "Created");
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
