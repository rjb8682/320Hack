using System;
using System.Collections.Generic;
using System.Collections;
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
using System.IO;

namespace _320Hack
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double titleFontSize = 16.0;

        public MainWindow()
        {
            // This is for making the splash screen appear for longer
            // System.Threading.Thread.Sleep(2000);
            InitializeComponent();
            gameArea.FontSize = titleFontSize;

            StreamReader levelReader = new StreamReader("../../Levels/level1.map");
            String fullLevel = levelReader.ReadToEnd();

            List<List<Char>> levelMap = new List<List<Char>>();
            List<Char> currentRow = new List<Char>();

            foreach (Char c in fullLevel)
            {
                if (c == '\n')
                {
                    levelMap.Add(currentRow);
                    currentRow = new List<Char>();
                }
                else
                {
                    currentRow.Add(c);
                }
            }
            levelMap.Add(currentRow);

            Map gameLevel = new Map(levelMap);

            gameArea.Text = gameLevel.printMap();

        }
    }
}
