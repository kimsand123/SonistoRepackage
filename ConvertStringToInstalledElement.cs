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
        string elementString = "";
        InstalledElement elementObject = new InstalledElement();
        public ConvertStringToInstalledElement()
        {

        }

        public InstalledElement convertElement(string elementString)
        {
            this.elementString = elementString;
            int to = this.elementString.Length - 1;
            int lastOccuranceOfSlash = this.elementString.LastIndexOf(@"\");
            int typeIndex = this.elementString.LastIndexOf(@".");

            elementObject.drive = this.elementString.Substring(0, 1);
            elementObject.fileName = this.elementString.Substring(lastOccuranceOfSlash + 1, to - lastOccuranceOfSlash);
            elementObject.path = this.elementString.Substring(2, to - elementObject.fileName.Length);
            elementObject.fileType = this.elementString.Substring(typeIndex, to);
            return elementObject;
        }
    }
}
