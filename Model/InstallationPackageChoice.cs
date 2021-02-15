using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonistoRepackage.Model
{
    public class InstallationPackageChoice
    {
        // If all is true all other is faLse
        // if bit32 is true, all and bit64 are false
        // if bit 64 is true, all and bit32 are false
        // if vst2 is true, all and vst3 and aax are false
        // if vst3 is true, all and vst2 and aax are false
        // if aax is true, all and vst2 and vst3 are false
        private bool _all = true;
        private bool _bit32 = false;
        private bool _bit64 = false;
        private bool _vst2 = false;
        private bool _vst3 = false;
        private bool _aax = false;

        public bool all
        {
            get
            {
                return this._all;
            }
            set
            {
                this._all = value;
                if (this._all == true)
                {
                    this._bit32 = false;
                    this._bit64 = false;
                    this._vst2 = false;
                    this._vst3 = false;
                    this._aax = false;
                }
            }
        }
        public bool bit32 {
            get
            {
                return this._bit32;
            }
            set
            {
                this._bit32 = value;
                if (this._bit32 == true)
                {
                    this._bit64 = false;
                    this._all = false;
                }
            }
        }

        public bool bit64
        {
            get
            {
                return this._bit64;
            }
            set
            {
                this._bit64 = value;
                if (this._bit64 == true)
                {
                    this._bit32 = false;
                    this._all = false;
                }
            }
        }
        public bool vst2
        {
            get
            {
                return this._vst2;
            }
            set
            {
                this._vst2 = value;
                if (this._vst2 == true)
                {
                    this._vst3 = false;
                    this._aax = false;
                    this._all = false;
                }
            }
        }
        public bool vst3
        {
            get
            {
                return this._vst3;
            }
            set
            {
                this._vst3 = value;
                if (this._vst3 == true)
                {
                    this._vst2 = false;
                    this._aax = false;
                    this._all = false;
                }
            }
        }
        public bool aax
        {
            get
            {
                return this._aax;
            }
            set
            {
                this._aax = value;
                if (this._aax == true)
                {
                    this._vst2 = false;
                    this._vst3 = false;
                    this._all = false;
                }
            }
        }

    }
}
