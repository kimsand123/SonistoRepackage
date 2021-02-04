using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonistoRepackage.InstallDetection
{
    class CleanUpInstalledElementList
    {
        int installedElementsCount = 0;
        List<string> elements = new List<string>();
        string oldRenamed = "";
        bool registerRenamedActionInList = false;
        List<string> cleanedList = new List<string>();
        public CleanUpInstalledElementList()
        {

        }

        public List<string> doIt(List<string> installedELements)
        {
            elements = installedELements;
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
            return cleanedList;
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
            cleanedList.Add(element);
            installedElementsCount += 1;
        }

        private void renamed(string element)
        {
            //if (registerRenamedActionInList)
            string test = "";
            element = element.Replace("Renamed", "Created");
            test = cleanedList.FirstOrDefault(s => s.Contains(element));

            //if test = null then the there is no temp file
            //present for the rename, and it is added
                if (test==null)
            {
                cleanedList.Add(element);
                installedElementsCount += 1;
            }
            else
            {
                cleanedList.Remove(element);
                installedElementsCount -= 1;
            }
        }

        private void deleted(string element)
        {
            cleanedList.Remove(element);
            installedElementsCount -= 1;
        }

        private void changed(string element)
        {
            //Nothing happens yet.
        }


    }
}
