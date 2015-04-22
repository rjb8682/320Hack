using System.Windows;

namespace AsciiLevelEditor
{
    /// <summary>
    /// Interaction logic for ConfirmationDialog.xaml
    /// </summary>
    public partial class ConfirmationDialog
    {
        private readonly MainWindow _mainRef;

        public ConfirmationDialog(MainWindow main)
        {
            InitializeComponent();
            _mainRef = main;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _mainRef.ClearLevel(null, null);
        }

        private void CancelButton(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
