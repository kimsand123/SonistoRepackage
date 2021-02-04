using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonistoRepackage.InstallDetection
{
    public class WatchedElement
    {
        public string fileAction { get; set; }
        public string path { get; set; }
        public string drive { get; set; }
        public string fileName { get; set; }
        public string extension { get; set; }
        public string owner { get; set; }
    }
}
