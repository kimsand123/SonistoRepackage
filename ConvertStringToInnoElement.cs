using SonistoRepackage.InstallDetection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonistoRepackage
{
    class ConvertStringToInnoElement
    {
        
        public ConvertStringToInnoElement()
        {

        }

        public FilterElement convertElement(string elementString)
        {
            FilterElement elementObject = new FilterElement();
            int leftBracketIndex = elementString.IndexOf("{");
            int rightBracketIndex = elementString.IndexOf("}");
            int fileLength = elementString.Length - 1;
            int lastOccuranceOfSlash = elementString.LastIndexOf(@"\");
            int typeIndex = elementString.LastIndexOf(@".");

            if (elementString.Contains("{"))
            {
                elementObject.generalFolder = elementString.Substring(leftBracketIndex, rightBracketIndex - leftBracketIndex + 1);
                elementObject.fileName = elementString.Substring(lastOccuranceOfSlash + 1, fileLength - lastOccuranceOfSlash);
                elementObject.path = elementString.Substring(rightBracketIndex + 1, lastOccuranceOfSlash - rightBracketIndex);
                //elementObject.fileType = elementString.Substring(typeIndex, fileLength - typeIndex + 1);
            }
            /*else
            {
                int lastDblSpaceIdx = elementString.IndexOf(" ");
                elementObject.generalFolder = "";
                elementObject.fileName = elementString.Substring(lastDblSpaceIdx + 1, fileLength - lastDblSpaceIdx);
            }*/
            elementObject.fileType = elementString.Substring(typeIndex, fileLength - typeIndex + 1);
            return elementObject;
        }
    }
}
