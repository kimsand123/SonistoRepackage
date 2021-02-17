using SonistoRepackage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;


namespace SonistoRepackage.View
{
    /// <summary>
    /// Interaction logic for InstallationPackagePopup.xaml
    /// </summary>
    public partial class InstallationPackagePopup : Window
    {

        public InstallationPackageChoice packageChoices = new InstallationPackageChoice();

        public InstallationPackagePopup(InstallationPackageChoice listBoxEntryChoices)
        {
            //this.packageChoices = listBoxEntryChoices;
            copyPackageChoices(listBoxEntryChoices);
            InitializeComponent();
            setTheCurrentValues();

        }

        private void copyPackageChoices(InstallationPackageChoice listBoxEntryChoices)
        {
            //Works on the class instance of the choices class
            //So it is possible to cancel the last setting of the choices.
            packageChoices.all = listBoxEntryChoices.all;
            packageChoices.bit32 = listBoxEntryChoices.bit32;
            packageChoices.bit64 = listBoxEntryChoices.bit64;
            packageChoices.vst2 = listBoxEntryChoices.vst2;
            packageChoices.vst3 = listBoxEntryChoices.vst3;
            packageChoices.aax = listBoxEntryChoices.aax;
        }

        private void setTheCurrentValues()
        {
            //sets the current value of the chosen listbox elements choices in the user interface
            if (packageChoices.all)
            {
                chkBxAll_Checked(new object(), new RoutedEventArgs());
            }
            else
            {
                if (packageChoices.bit32)
                {
                    chkBx32Bit_Checked(new object(), new RoutedEventArgs());
                }
                if (packageChoices.bit64)
                {
                    chkBx64Bit_Checked(new object(), new RoutedEventArgs());
                }
                if (packageChoices.vst2)
                {
                    chkBxVst2_Checked(new object(), new RoutedEventArgs());
                }
                if (packageChoices.vst3)
                {
                    chkBxVst3_Checked(new object(), new RoutedEventArgs());
                }
                if (packageChoices.aax)
                {
                    chkBxAax_Checked(new object(), new RoutedEventArgs());
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            //TODO:Check if anything but all is checked, you need to have 1 checked in architecture and 1 in format.
            this.DialogResult = true;
            this.Close();
        }

        public InstallationPackageChoice getChoices()
        {
            return this.packageChoices;
        }


        //view and model data handling
        //should be done with mvvm, but cannot figure out the databinding.
        //needs to get on. 
        //TODO: make this work with MVVM databinding.
        private void chkBxAll_Checked(object sender, RoutedEventArgs e)
        {
            this.chkBx32Bit.IsChecked = false;
            this.chkBx64Bit.IsChecked = false;
            this.chkBxVst2.IsChecked = false;
            this.chkBxVst3.IsChecked = false;
            this.chkBxAax.IsChecked = false;
            this.chkBxAll.IsChecked = true;
            packageChoices.all = true;
            packageChoices.bit32 = false;
            packageChoices.bit64 = false;
            packageChoices.vst2 = false;
            packageChoices.vst3 = false;
            packageChoices.aax = false;
        }
        private void chkBx32Bit_Checked(object sender, RoutedEventArgs e)
        {
            this.chkBxAll.IsChecked = false;
            this.chkBx32Bit.IsChecked = true;
            this.chkBx64Bit.IsChecked = false;
            packageChoices.bit32 = true;
            packageChoices.all = false;
            packageChoices.bit64 = false;
        }

        private void chkBx64Bit_Checked(object sender, RoutedEventArgs e)
        {
            this.chkBxAll.IsChecked = false;
            this.chkBx32Bit.IsChecked = false;
            this.chkBx64Bit.IsChecked = true;
            packageChoices.bit64 = true;
            packageChoices.all = false;
            packageChoices.bit32 = false;
        }

        private void chkBxVst2_Checked(object sender, RoutedEventArgs e)
        {
            this.chkBxAll.IsChecked = false;
            this.chkBxVst2.IsChecked = true;
            this.chkBxVst3.IsChecked = false;
            this.chkBxAax.IsChecked = false;
            packageChoices.vst2 = true;
            packageChoices.all = false;
            packageChoices.vst3 = false;
            packageChoices.aax = false;
        }

        private void chkBxVst3_Checked(object sender, RoutedEventArgs e)
        {
            this.chkBxAll.IsChecked = false;
            this.chkBxVst2.IsChecked = false;
            this.chkBxVst3.IsChecked = true;
            this.chkBxAax.IsChecked = false;
            packageChoices.vst3 = true;
            packageChoices.all = false;
            packageChoices.vst2 = false;
            packageChoices.aax = false;
        }

        private void chkBxAax_Checked(object sender, RoutedEventArgs e)
        {
            this.chkBxAll.IsChecked = false;
            this.chkBxVst2.IsChecked = false;
            this.chkBxVst3.IsChecked = false;
            this.chkBxAax.IsChecked = true;
            packageChoices.aax = true;
            packageChoices.all = false;
            packageChoices.vst2 = false;
            packageChoices.vst3 = false;
        }
    }
}
