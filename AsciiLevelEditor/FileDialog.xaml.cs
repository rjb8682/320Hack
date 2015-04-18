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
        private List<_320Hack.Room> rooms;

        public FileDialog(MainWindow main, String which, List<_320Hack.Room> rooms)
        {
            InitializeComponent();
            mainRef = main;
            this.which = which;

            if (this.which == "Export")
            {
                FileTextBox.Visibility = Visibility.Visible;
                LevelListBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.rooms = rooms;

                foreach (_320Hack.Room room in rooms)
                {
                    LevelListBox.Items.Add("Level " + room.Id);
                }
            }
        }

        private void sendFileName(object sender, RoutedEventArgs e)
        {
            if (which == "Export")
            {

            }
            else
            {
                mainRef.importFile(LevelListBox.SelectedIndex + 1);
            }
            this.Close();
        }

        private void keyPressedInputBox(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) sendFileName(null, null);
        }
    }
}
