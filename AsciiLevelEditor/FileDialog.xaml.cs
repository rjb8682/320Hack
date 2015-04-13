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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class FileDialog : Window
    {
        private MainWindow mainRef;
        private String which;

        public FileDialog(MainWindow main, String which)
        {
            InitializeComponent();
            mainRef = main;
            this.which = which;
        }

        private void sendFileName(object sender, RoutedEventArgs e)
        {
            if (fileInput.Text != "")
            {
                if (which == "Export") mainRef.exportFile(fileInput.Text);
                else if (which == "Import") mainRef.importFile(fileInput.Text);
            }
            this.Close();
        }

        private void keyPressedInputBox(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) sendFileName(null, null);
        }
    }
}
