using SonistoRepackage.InstallDetection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonistoRepackage
{
    class ConvertStringToInstalledElement
    {
        bool useRenameEntry = false;
        public ConvertStringToInstalledElement()
        {

        }

        public InstalledElement convertElement(string elementString, Dictionary<int, FilterElement> filterElements)
        {
            InstalledElement elementObject = new InstalledElement();
            //Getting the filestring from the elementstring
            int actionColonIdx = elementString.LastIndexOf(@":");
            int changeTypeIdx = elementString.IndexOf(@"<");
            int stringLength = elementString.Length - 1;
            string fileString = elementString.Substring(1, stringLength - (stringLength - changeTypeIdx + 1));
            string changeType = elementString.Substring(changeTypeIdx + 1, actionColonIdx - changeTypeIdx - 1);
            
            //getting drive, path, file and type of the filestring
            int firstSlashIdx = fileString.IndexOf(@"\");
            int lastSlashIdx = fileString.LastIndexOf(@"\");

            InstalledElement installedElement = new InstalledElement();
            //filterElement.fileName = this.elementString.Substring(lastOccuranceOfSlash + 1, to - lastOccuranceOfSlash);
            //if (filterElements.ContainsValue(filterElement))
            //{
            //Build the elementObject
            elementObject.drive = fileString.Substring(0, 2);
            elementObject.fileName = fileString.Substring(lastSlashIdx + 1, fileString.Length - lastSlashIdx - 1);
            elementObject.path = fileString.Substring(2, lastSlashIdx - firstSlashIdx + 1);
            int fileTypeIdx = fileString.LastIndexOf(@".");
            if (fileTypeIdx > 0)
            {
                elementObject.fileType = fileString.Substring(fileTypeIdx, fileString.Length - fileTypeIdx);
            }
            else
            {
                elementObject.fileType = "";
            }
            useRenameEntry = false;
            return elementObject;
        }
    }
}
