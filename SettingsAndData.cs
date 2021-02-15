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
        public const string FILTERFILE = @"C:\Sonisto\RepackageFilter.txt";
        public const bool TEST = false;
    }
}

