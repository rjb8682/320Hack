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

namespace AsciiLevelEditor
{
    /// <summary>
    /// Interaction logic for ConfirmationDialog.xaml
    /// </summary>
    public partial class ConfirmationDialog : Window
    {
        private MainWindow mainRef;

        public ConfirmationDialog(MainWindow main)
        {
            InitializeComponent();
            mainRef = main;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mainRef.clearLevel(null, null);
        }

        private void CancelButton(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
