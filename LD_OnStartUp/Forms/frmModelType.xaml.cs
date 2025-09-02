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

namespace LD_OnStartUp.Forms
{
    /// <summary>
    /// Interaction logic for frmModelType.xaml
    /// </summary>
    public partial class frmModelType : Window
    {
        public frmModelType()
        {
            InitializeComponent();
        }

        public string GetGroupProjectType()
        {
            if (rbnNew.IsChecked == true)
                return rbnNew.Content.ToString();
            else if (rbnExisting.IsChecked == true)
                return rbnExisting.Content.ToString();
            else
                return null;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
