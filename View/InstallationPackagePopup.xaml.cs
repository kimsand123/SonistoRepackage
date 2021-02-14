using SonistoRepackage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SonistoRepackage.View
{
    /// <summary>
    /// Interaction logic for InstallationPackagePopup.xaml
    /// </summary>
    public partial class InstallationPackagePopup : UserControl
    {
        public InstallationPackageChoice packageChoices 
        {
            get {return packageChoices; }
        }

        public InstallationPackagePopup()
        {
            InitializeComponent();
            this.chkBxAll.IsChecked = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            chkBxAll_Checked(new object(), new RoutedEventArgs());
            this.Close();
        }

        public static InstallationPackageChoice choice()
        {
            InstallationPackagePopup dialog = new InstallationPackagePopup();
            dialog.Show();
            return dialog.packageChoices;
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).DialogResult = true;
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
            this.chkBx64Bit.IsChecked=false;
            packageChoices.bit32 = this.chkBx32Bit.IsChecked.HasValue;
            packageChoices.all = false;
            packageChoices.bit64 = false;
        }

        private void chkBx64Bit_Checked(object sender, RoutedEventArgs e)
        {
            this.chkBxAll.IsChecked = false;
            this.chkBx32Bit.IsChecked = false;
            packageChoices.bit64 = this.chkBx64Bit.IsChecked.HasValue;
            packageChoices.all = false;
            packageChoices.bit32 = false;
        }

        private void chkBxVst2_Checked(object sender, RoutedEventArgs e)
        {
            this.chkBxAll.IsChecked = false;
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
            packageChoices.aax = this.chkBxAax.IsChecked.HasValue;
            packageChoices.all = false;
            packageChoices.vst2 = false;
            packageChoices.vst3 = false;
        }
    }
}
