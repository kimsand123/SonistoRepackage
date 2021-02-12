using SonistoRepackage.Model;
using System;
using System.Linq;
using System.Windows;


namespace SonistoRepackage.View
{
    /// <summary>
    /// Interaction logic for InstallationPackagePopup.xaml
    /// </summary>
    public partial class InstallationPackagePopup : Window
    {
        InstallationPackageChoice packageChoices = new InstallationPackageChoice();

        private bool[] _modeArray = new bool[] { true, false, false, false, false, false };
        public bool[] ModeArray
        { 
            get 
            { 
                return _modeArray; 
            } 
        }

        public int SelectedMode
        {
            get 
            { 
                return Array.IndexOf(_modeArray, true);
            }
        }

        public bool[] ClickResult;

        public InstallationPackagePopup()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ClickResult = null;
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
            if (this.rdBtn32Bit.IsChecked==true || this.rdBtn64Bit.IsChecked==true)
            {
                if(this.rdBtnVst2.IsChecked==true || this.rdBtnVst3.IsChecked==true || this.rdBtnAax.IsChecked == true)
                {

                }
            } else
            {
                this.rdBtnAll.IsChecked = true;
            }
            ClickResult = _modeArray;
            this.Close();
        }
        private void rdBtnAll_GotFocus(object sender, RoutedEventArgs e)
        {
            for(int idx = 0; idx <_modeArray.Count();idx++) 
            {
                _modeArray[idx] = false;
            }
            _modeArray[0] = true;
        }

        private void clearAllPackages(object sender, RoutedEventArgs e)
        {
            _modeArray[0] = false;
        }
    }
}
