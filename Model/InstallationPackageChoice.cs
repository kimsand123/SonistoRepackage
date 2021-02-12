using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonistoRepackage.Model
{
    class InstallationPackageChoice
    {
        public bool all
        {
            get
            {
                return all;
            }
            set
            {
                if (value == true)
                {
                    bit32 = false;
                    bit64 = false;
                    vst2 = false;
                    vst3 = false;
                    aax = false;
                }
            }
        }
        public bool bit32 {
            get
            { 
                return bit32; 
            }
            set 
            {
                if (value == true)
                {
                    all = false;
                }
            } 
        }

        public bool bit64
        {
            get
            {
                return bit64;
            }
            set
            {
                if (value == true)
                {
                    all = false;
                }
            }
        }
        public bool vst2
        {
            get
            {
                return vst2;
            }
            set
            {
                if (value == true)
                {
                    all = false;
                }
            }
        }
        public bool vst3
        {
            get
            {
                return vst3;
            }
            set
            {
                if (value == true)
                {
                    all = false;
                }
            }
        }
        public bool aax
        {
            get
            {
                return aax;
            }
            set
            {
                if (value == true)
                {
                    all = false;
                }
            }
        }

    }
}
