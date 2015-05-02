using System.Collections.Generic;
using System.Windows;

namespace AsciiLevelEditor
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class FileDialog : Window
    {
        private MainWindow _mainRef;
        public FileDialog(List<_320Hack.Room> rooms, MainWindow _mainRef)
        {
            InitializeComponent();
            this._mainRef = _mainRef;

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
