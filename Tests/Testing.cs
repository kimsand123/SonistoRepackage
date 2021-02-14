using SonistoRepackage.Model;
using SonistoRepackage.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonistoRepackage.Tests
{
    class Testing
    {
        public Testing()
        {

        }

        public void testInstallationPackageDTO()
        {
            InstallationPackageChoice packageChoices = new InstallationPackageChoice();
            packageChoices.all = true;
            packageChoices.bit32 = true;
            packageChoices.bit64 = true;
            packageChoices.vst2 = true;
            packageChoices.vst3 = true;
            packageChoices.aax = true;
            packageChoices.all = false;
            packageChoices.bit32 = false;
            packageChoices.bit64 = false;
            packageChoices.vst2 = false;
            packageChoices.vst3 = false;
            packageChoices.aax = false;
        }

        public void testRadioButtonPopUp()
        {

            InstallationPackagePopup popup = new InstallationPackagePopup();
            if ((bool)popup.ShowDialog() == true)
            {
                InstallationPackagePopup content = popup.Content as InstallationPackagePopup;
                InstallationPackageChoice packageChoices = content.packageChoices;

            }
        }
    }


}
