using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonistoRepackage.Utils
{
    class Log
    {
        public Log()
        {

        }

        public void write(Exception e)
        {
            string filterFile = Path.Combine(Directory.GetCurrentDirectory(), SettingsAndData.Instance.logFile);
            using (StreamWriter sw = File.AppendText(filterFile))
            {
                sw.WriteLine(e.Message);
            }
        }

        public void write(string message)
        {
            string filterFile = Path.Combine(Directory.GetCurrentDirectory(), SettingsAndData.Instance.logFile);
            using (StreamWriter sw = File.AppendText(filterFile))
            {
                sw.WriteLine(message);
            }
        }

    }
}
