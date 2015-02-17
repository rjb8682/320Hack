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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _320Hack
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double titleFontSize = 20.0;

        public MainWindow()
        {
            // This is for making the splash screen appear for longer
            // System.Threading.Thread.Sleep(2000);
            InitializeComponent();

            gameArea.FontSize = titleFontSize;
            gameArea.Text = "Hello World";
        }
    }
}
