using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            createPlaceHolderStructure();
        }


        //Workfolder is situated in Desktop

        public void createPlaceHolderStructure()
        {
            List<string> placeholderFolderStructure = new List<string>();
            foreach (string element in eventList)
            {
                placeholderFolderStructure.Add(createPlaceholderPath(element));
            }
            CreateFolders(placeholderFolderStructure);

        }

        private void CreateFolders(List<string> placeholderFolderStructure)
        {
            if (Directory.Exists("C:\\Sonisto\\PackageFolder"))
            {
                ProcessStartInfo innounpProces = new ProcessStartInfo();
                innounpProces.CreateNoWindow = false;
                innounpProces.UseShellExecute = false;
                innounpProces.FileName = "rmdir";
                innounpProces.WorkingDirectory = @"C:\Sonisto";
                innounpProces.WindowStyle = ProcessWindowStyle.Normal;
                innounpProces.RedirectStandardOutput = true;
                innounpProces.Arguments = "/Q /S \"C:\\Sonisto\\PackageFolder\"";
                try
                {
                    using (Process exeProcess = Process.Start(innounpProces))
                    {
                        exeProcess.WaitForExit();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            Directory.CreateDirectory("C:\\Sonisto\\PackageFolder");

            for (int idx = 0; idx < placeholderFolderStructure.Count - 1; idx++) 
            {
                string placeHolderDir = getPlaceholderDir(placeholderFolderStructure[idx]);
                string relativePath = getRelativePath(placeholderFolderStructure[idx]);
                if (!Directory.Exists("C:\\Sonisto\\PackageFolder\\"+ placeholderFolderStructure[idx]))
                {
                    //Directory.CreateDirectory("C:\\Sonisto\\PackageFolder\\" + placeHolderDir + "\"" + relativePath + "\"");
                    File.Copy(eventList[idx], placeholderFolderStructure[idx]);
                }
            }
        }

        //removing empty directories recursively
       /* private void removeDirectory(string pathWithoutFile)
        {
            string[] directories = Directory.GetDirectories(pathWithoutFile);
            foreach (string dir in directories)
            {
                string[] yetMoreDirectories = Directory.GetDirectories(dir);
                if (yetMoreDirectories!=null)
                {
                    Directory.Delete(dir);
                }
                else
                {
                    removeDirectory(dir);
                }
            }
        }*/

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
