using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SonistoRepackage.View
{
    /// <summary>
    /// Interaction logic for InstallationPackagePopup.xaml
    /// </summary>
    public partial class InstallationPackagePopup : Window
    {

        public InstallationPackagePopup()
        {
            InitializeComponent();
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
            
            registerButtons();

        }
        private void rdBtnAll_GotFocus(object sender, RoutedEventArgs e)
        {
            this.rdBtn32Bit.IsChecked = false;
            this.rdBtn64Bit.IsChecked = false;
            this.rdBtnAax.IsChecked = false;
            this.rdBtnVst2.IsChecked = false;
            this.rdBtnVst3.IsChecked = false;
        }

        private void clearAllPackages(object sender, RoutedEventArgs e)
        {
            this.rdBtnAll.IsChecked = false;
        }

        private void registerButtons()
        {

        }
    }
}
