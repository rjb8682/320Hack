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
        private double titleFontSize = 16.0;
        private Map gameLevel;

        public static int UP = 9000;
        public static int DOWN = 9001;
        public static int LEFT = 9002;
        public static int RIGHT = 9003;

        public static Char floor = '·';
        public static Char horizWall = '—';

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
                    if (c == '.')
                    {
                        currentRow.Add(floor);
                    }
                    else if (c == '-')
                    {
                        currentRow.Add(horizWall);
                    }
                    else
                    {
                        currentRow.Add(c);
                    }
                }
            }
            levelMap.Add(currentRow);

            gameLevel = new Map(convertToTiles(levelMap));
            update();

        }

        public void update()
        {
            gameArea.Text = gameLevel.printMap();
        }

        public List<List<Tile>> convertToTiles(List<List<Char>> map)
        {
            List<List<Tile>> toReturn = new List<List<Tile>>();

            foreach (List<Char> list in map)
            {
                List<Tile> currentRow = new List<Tile>();
                foreach (Char c in list)
                {
                    currentRow.Add(new Tile(c));
                }
                toReturn.Add(currentRow);
            }

            return toReturn;
        }

        private void keyPressed(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.W || e.Key == Key.Up ||
                e.Key == Key.S || e.Key == Key.Down ||
                e.Key == Key.A || e.Key == Key.Left ||
                e.Key == Key.D || e.Key == Key.Right) && !textEntry.IsFocused)
            {
                movePlayer(e.Key);
                update();
            }
        }

        public void movePlayer(Key k)
        {
            if (k == Key.Up || k == Key.W)
            {
                gameLevel.movePlayer(UP);
            }
            else if (k == Key.Down || k == Key.S)
            {
                gameLevel.movePlayer(DOWN);
            }
            else if (k == Key.Left || k == Key.A)
            {
                gameLevel.movePlayer(LEFT);
            }
            else if (k == Key.Right || k == Key.D)
            {
                gameLevel.movePlayer(RIGHT);
            }
        }

        private void textEntered(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                String response = processInput(textEntry.Text);
                textEntry.Clear();
                outputPanel.Text = response;
                gameArea.IsEnabled = true;
                Keyboard.Focus(gameArea);
                gameArea.IsEnabled = false;
            }
        }

        public String processInput(String text)
        {
            if (text[0] != '/')
            {
                return "Not a command. Input /help to see a list of commands";
            }
            else
            {
                if (text == "/help")
                {
                    // Will eventually open a help
                    return getHelpString();
                }
                else
                {
                    return "";
                }
            }
        }

        private String getHelpString()
        {
            String result = "";

            result += "Movement:\n";
            result += "\t WASD or Arrow Keys to move the player\n";
            
            return result;
        }

    }
}
