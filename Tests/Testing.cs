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
            object sender = new object();
            InstallationPackageChoice testChoiceFromListBox = new InstallationPackageChoice();
            testChoiceFromListBox.aax = true;
            testChoiceFromListBox.bit32 = true;
            InstallationPackageChoice resultChoice = new InstallationPackageChoice();
            InstallationPackagePopup popup = new InstallationPackagePopup(testChoiceFromListBox);
            if ((bool)popup.ShowDialog() && popup.DialogResult.Value == true)
            {
                resultChoice = popup.getChoices();
            }
        }
    }
}
