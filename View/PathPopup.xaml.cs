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
    /// Interaction logic for PathPopup.xaml
    /// </summary>
    public partial class PathPopup : Window
    {
        public string pathResult { get; set; }
        public PathPopup()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.pathResult = this.txtBxPathResult.Text;
        }
    }
}
