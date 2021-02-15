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
            this.packageChoices = listBoxEntryChoices;

            InitializeComponent();
            setTheCurrentValues();

        }

        private void setTheCurrentValues()
        {
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
            sender = null;
            this.DialogResult = false;
            this.Close();
        }

        public InstallationPackageChoice getChoices()
        {
            return this.packageChoices;
        }
        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            sender = this.packageChoices;
            this.DialogResult=true;
            this.Close();
        }

        private void clearAllPackages(object sender, RoutedEventArgs e)
        {
            this.chkBxAll.IsChecked = false;
            packageChoices.all = false;
        }
        private void chkBxAll_Checked(object sender, RoutedEventArgs e)
        {
            this.chkBx32Bit.IsChecked = false;
            this.chkBx64Bit.IsChecked = false;
            this.chkBxVst2.IsChecked = false;
            this.chkBxVst3.IsChecked = false;
            this.chkBxAax.IsChecked = false;
            this.chkBxAll.IsChecked = true;
            packageChoices.all = this.chkBxAll.IsChecked.HasValue;
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
            packageChoices.bit32 = this.chkBx32Bit.IsChecked.HasValue;
            packageChoices.all = false;
            packageChoices.bit64 = false;
        }

        private void chkBx64Bit_Checked(object sender, RoutedEventArgs e)
        {
            this.chkBxAll.IsChecked = false;
            this.chkBx32Bit.IsChecked = false;
            this.chkBx64Bit.IsChecked = true;
            packageChoices.bit64 = this.chkBx64Bit.IsChecked.HasValue;
            packageChoices.all = false;
            packageChoices.bit32 = false;
        }

        private void chkBxVst2_Checked(object sender, RoutedEventArgs e)
        {
            this.chkBxAll.IsChecked = false;
            this.chkBxVst2.IsChecked = false;
            this.chkBxVst3.IsChecked = false;
            this.chkBxAax.IsChecked = false;
            packageChoices.vst2 = this.chkBxVst2.IsChecked.HasValue;
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
            packageChoices.vst3 = this.chkBxVst3.IsChecked.HasValue;
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
            packageChoices.aax = this.chkBxAax.IsChecked.HasValue;
            packageChoices.all = false;
            packageChoices.vst2 = false;
            packageChoices.vst3 = false;
        }
    }
}
