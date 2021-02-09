using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonistoRepackage
{
    class CreateFolderStructure
    {
        List<string> eventList;
        public CreateFolderStructure(List<string> eventList)
        {
            this.eventList = eventList;
            createFolders();
        }


        //Workfolder is situated in Desktop

        public void createFolders()
        {
            List<string> placeholderFolderStructure = new List<string>();
            foreach (string element in eventList)
            {
                placeholderFolderStructure.Add(createPlaceholderPath(element));
            }

        }

        public string createPlaceholderPath(string element)
        {
            string value = "";
            string placeholder = "";
            string path = element;
            string pathToLookFor = "";

     

            pathToLookFor = @"C:\Users\test\Documents";
            if (path.Contains(pathToLookFor))
            {
                placeholder = "{userdocs}";
                path = path.Replace(pathToLookFor, placeholder);
                //path.Remove(0,pathToLookFor.Length - 1);
                //path.Insert(0, placeholder);
            } else pathToLookFor = @"C:\Users\test";  if (path.Contains(pathToLookFor))
            {
                placeholder = "{home}";
                path = path.Replace(pathToLookFor, placeholder);
                //path.Remove(0, pathToLookFor.Length - 1);
                //path.Insert(0, placeholder);
            } else pathToLookFor = @"C:\Program Files (x86)\Common Files\VST3"; if (path.Contains(pathToLookFor))
            {
                //placeholder = "{plugin}";
                placeholder = @"{VST3-32}";
                path = path.Replace(pathToLookFor, placeholder);
                //path.Remove(0, pathToLookFor.Length - 1);
                //path.Insert(0, placeholder);
            } else pathToLookFor = @"C:\Program Files\Common Files\VST3"; if (path.Contains(pathToLookFor))
            {
                //placeholder = "{plugin}";
                placeholder = @"{VST3-64}";
                path = path.Replace(pathToLookFor, placeholder);
                //path.Remove(0, pathToLookFor.Length - 1);
                //path.Insert(0, placeholder);
            } else pathToLookFor = @"\Library\Music\AAX"; if (path.Contains(pathToLookFor))
            {
                //placeholder = "{plugin}";
                placeholder = @"{AAX}";
                path = path.Replace(pathToLookFor, placeholder);
                //path.Remove(0, pathToLookFor.Length - 1);
                //path.Insert(0, placeholder);
            }
            else pathToLookFor = @"\Library\Music\AU"; if (path.Contains(pathToLookFor))
            {
                //placeholder = "{plugin}";
                placeholder = @"{AU}";
                path = path.Replace(pathToLookFor, placeholder);
                //path.Remove(0, pathToLookFor.Length - 1);
                //path.Insert(0, placeholder);
            }
            else pathToLookFor = @"C:\vst2-64"; if (path.Contains(pathToLookFor))
            {
                //placeholder = "{VST2}";
                placeholder = "{VST2-64}";
                path = path.Replace(pathToLookFor, placeholder);
                //path.Remove(0, pathToLookFor.Length - 1);
                //path.Insert(0, placeholder);
            }
            else pathToLookFor = @"C:\vst2-32"; if (path.Contains(pathToLookFor))
            {
                //placeholder = "{VST2}";
                placeholder = "{VST2-32}";
                path = path.Replace(pathToLookFor, placeholder);
                //path.Remove(0, pathToLookFor.Length - 1);
                //path.Insert(0, placeholder);
            } else
            {
                placeholder = "{absolute}";
                path = path.Replace("C:", placeholder);
                //path.Remove(0, 2);
                //path.Insert(0, placeholder);
            }
            return path;
        }
    }
}
