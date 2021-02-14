using SonistoRepackage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonistoRepackage.InstallDetection
{
    class ItemForListbox
    {
        private InstallationPackageChoice _choices = new InstallationPackageChoice();
        public string path { get; set; }
        public string file { get; set; }
        public InstallationPackageChoice choices
        {
            get { return this._choices; }
            set
            {
                this._choices.all = true;
                this._choices.bit32 = false;
                this._choices.bit64 = false;
                this._choices.vst2 = false;
                this._choices.vst3 = false;
                this._choices.aax = false;
            }
        }
    }
}
