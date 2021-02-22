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


        public void logInitialize()
        {
            if (SettingsAndData.Instance.logFile != null)
            {
                string logFile = Path.Combine(Directory.GetCurrentDirectory(), SettingsAndData.Instance.logFile);
                if (File.Exists(logFile))
                {
                    File.Delete(logFile);
                }
            }
        }
        public void write(Exception e, string header)
        {

            string filterFile = Path.Combine(Directory.GetCurrentDirectory(), SettingsAndData.Instance.logFile);
            using (StreamWriter sw = File.AppendText(filterFile))
            {
                sw.WriteLine(header);
                sw.WriteLine(e.Message);
            }
        }

        public void write(string message, string header)
        {
            string filterFile = Path.Combine(Directory.GetCurrentDirectory(), SettingsAndData.Instance.logFile);
            using (StreamWriter sw = File.AppendText(filterFile))
            {
                sw.WriteLine(header);
                sw.WriteLine(message);
            }
        }

        public void write (List<string> list, string header)
        {
            string filterFile = Path.Combine(Directory.GetCurrentDirectory(), SettingsAndData.Instance.logFile);
            using (StreamWriter sw = File.AppendText(filterFile))
            {
                sw.WriteLine(header);
                foreach (string element in list)
                {
                    sw.WriteLine(element);
                }

            }
        }

    }
}
