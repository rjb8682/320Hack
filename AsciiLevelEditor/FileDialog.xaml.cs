using System.Collections.Generic;
using System.Windows;

namespace AsciiLevelEditor
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class FileDialog : Window
    {
        private readonly MainWindow _mainRef;

        public FileDialog(MainWindow main, List<_320Hack.Room> rooms)
        {
            InitializeComponent();
            _mainRef = main;

            foreach (var room in rooms)
            {
                LevelListBox.Items.Add("Level " + room.Id);
            }
        }

        private void SendFileName(object sender, RoutedEventArgs e)
        {
            _mainRef.ImportFile(LevelListBox.SelectedIndex + 1);
            this.Close();
        }
    }
}
