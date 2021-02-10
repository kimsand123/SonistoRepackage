﻿using System;
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
                EmptyFolder("C:\\Sonisto\\PackageFolder");  
            } else
            {
                Directory.CreateDirectory("C:\\Sonisto\\PackageFolder");
            }

            for (int idx = 0; idx < placeholderFolderStructure.Count; idx++) 
            {
                //string placeHolderDir = getPlaceholderDir(placeholderFolderStructure[idx]);
                //string relativePath = getRelativePath(placeholderFolderStructure[idx]);
                //if (!Directory.Exists("C:\\Sonisto\\PackageFolder\\"+ placeholderFolderStructure[idx]))
                //{
                //Directory.CreateDirectory("C:\\Sonisto\\PackageFolder\\" + placeHolderDir + "\"" + relativePath + "\"");
                Directory.CreateDirectory("C:\\Sonisto\\PackageFolder\\" + Path.GetDirectoryName(placeholderFolderStructure[idx]));
                File.Copy(eventList[idx], "C:\\Sonisto\\PackageFolder\\" + placeholderFolderStructure[idx]);
                //}
            }
        }

        // https://stackoverflow.com/questions/1288718/how-to-delete-all-files-and-folders-in-a-directory

        private bool EmptyFolder(string pathName)
        {
            bool errors = false;
            DirectoryInfo dir = new DirectoryInfo(pathName);

            foreach (FileInfo fi in dir.EnumerateFiles())
            {
                try
                {
                    fi.IsReadOnly = false;
                    fi.Delete();

                    //Wait for the item to disapear (avoid 'dir not empty' error).
                    while (fi.Exists)
                    {
                        System.Threading.Thread.Sleep(10);
                        fi.Refresh();
                    }
                }
                catch (IOException e)
                {
                    Debug.WriteLine(e.Message);
                    errors = true;
                }
            }

            foreach (DirectoryInfo di in dir.EnumerateDirectories())
            {
                try
                {
                    EmptyFolder(di.FullName);
                    di.Delete();

                    //Wait for the item to disapear (avoid 'dir not empty' error).
                    while (di.Exists)
                    {
                        System.Threading.Thread.Sleep(10);
                        di.Refresh();
                    }
                }
                catch (IOException e)
                {
                    Debug.WriteLine(e.Message);
                    errors = true;
                }
            }

            return !errors;
        }

        private string getRelativePath(string v)
        {
            throw new NotImplementedException();
        }

        private string getPlaceholderDir(string v)
        {
            throw new NotImplementedException();
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
            } else pathToLookFor = @"C:\Users\test";  if (path.Contains(pathToLookFor))
            {
                placeholder = "{home}";
                path = path.Replace(pathToLookFor, placeholder);
            } else pathToLookFor = @"C:\Program Files (x86)\Common Files\VST3"; if (path.Contains(pathToLookFor))
            {
                placeholder = "{plugin}";
                //placeholder = @"{VST3-32}";
                path = path.Replace(pathToLookFor, placeholder);
            } else pathToLookFor = @"C:\Program Files\Common Files\VST3"; if (path.Contains(pathToLookFor))
            {
                placeholder = "{plugin}";
                //placeholder = @"{VST3-64}";
                path = path.Replace(pathToLookFor, placeholder);
            } else pathToLookFor = @"\Library\Music\AAX"; if (path.Contains(pathToLookFor))
            {
                placeholder = "{plugin}";
                //placeholder = @"{AAX}";
                path = path.Replace(pathToLookFor, placeholder);
            }
            else pathToLookFor = @"\Library\Music\AU"; if (path.Contains(pathToLookFor))
            {
                placeholder = "{plugin}";
                //placeholder = @"{AU}";
                path = path.Replace(pathToLookFor, placeholder);
            }
            else pathToLookFor = @"C:\vst2-64"; if (path.Contains(pathToLookFor))
            {
                placeholder = "{plugin}";
                //placeholder = "{VST2-64}";
                path = path.Replace(pathToLookFor, placeholder);
            }
            else pathToLookFor = @"C:\vst2-32"; if (path.Contains(pathToLookFor))
            {
                placeholder = "{plugin}";
                //placeholder = "{VST2-32}";
                path = path.Replace(pathToLookFor, placeholder);
            } else
            {
                placeholder = "{absolute}";
                path = path.Replace("C:", placeholder);
            }
            return path;
        }
    }
}
