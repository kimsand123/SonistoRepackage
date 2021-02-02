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
        FilterElement elementObject = new FilterElement();
        public ConvertStringToInnoElement()
        {

        }

        public FilterElement convertElement(string elementString)
        {
            if (elementString.Contains("{"))
            {

                int leftBracketIndex = elementString.IndexOf("{");
                int rightBracketIndex = elementString.IndexOf("}");
                int to = elementString.Length - 1;
                int lastOccuranceOfSlash = elementString.LastIndexOf(@"\");
                int typeIndex = elementString.LastIndexOf(@".");

                elementObject.drive = elementString.Substring(leftBracketIndex, rightBracketIndex-leftBracketIndex+1);
                elementObject.fileName = elementString.Substring(lastOccuranceOfSlash + 1, to - lastOccuranceOfSlash);
                elementObject.path = elementString.Substring(rightBracketIndex, lastOccuranceOfSlash + 1);
                elementObject.fileType = elementString.Substring(typeIndex, to-typeIndex+1);
                return elementObject;
            }
            return null;
        }
    }
}
