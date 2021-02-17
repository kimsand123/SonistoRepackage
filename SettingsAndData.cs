using System;
using System.Collections.Generic;
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
        public const bool TEST = false;

        public string filterFile = @"C:\Sonisto\RepackageFilter.txt";
        public string workingFolder = @"C:\Sonisto\PackageFolders\";
        public string userDocFolder = @"C:\Users\test\Documents";
        public string homeFolder = @"C:\Users\test";
        public string pluginVst3_32Folder = @"C:\Program Files (x86)\Common Files\VST3";
        public string pluginVst3_64Folder = @"C:\Program Files\Common Files\VST3";
        public string pluginAaxFolder = @"\Library\Music\AAX";
        public string pluginAuFolder = @"\Library\Music\AU";

    }
}

