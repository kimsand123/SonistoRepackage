using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonistoRepackage
{
    // Adapted from https://csharpindepth.com/articles/singleton
    // fully lazy instantiation. Den bliver udløst første gang der er en reference til det statiske medlem af den 
    // indeholdte klasse i Instance, og den bliver kun udført en gang pr Appdomæne hvilket gør at det kun er en tråd
    // ad gangen der kan køre den. Disse to ting gør den lazy og threadsafe.

    public sealed class SettingsAndData
    {
        private SettingsAndData()
        {
        }
        public static SettingsAndData Instance { get { return get.instance; } }

        private class get
        {
            static get()
            {
            }
            internal static readonly SettingsAndData instance = new SettingsAndData();
        }
        public  bool test = false;
        public bool deployBuild = false;
        public string userDocFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents");
        public string homeFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        public string filterFile {get;set;}
        public string workingFolder { get; set; }
        //public string userDocFolder { get; set; }
        //public string homeFolder { get; set; }
        public string logFile { get; set; }
        public string pluginVst3_32Folder { get; set; }
        public string pluginVst3_64Folder { get; set; }
        public string pluginAaxFolder { get; set; }
        public string pluginAuFolder { get; set; }
        public string pluginVst2_32Folder { get; set; }
        public string pluginVst2_64Folder { get; set; }
        public string absoluteFolder { get; set; }

    }
}

