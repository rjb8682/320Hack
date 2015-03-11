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

        public static int UP_LEFT = 9004;
        public static int DOWN_LEFT = 9005;
        public static int UP_RIGHT = 9006;
        public static int DOWN_RIGHT = 9007;

        public Key[] keys = {Key.NumPad8, Key.NumPad2, Key.NumPad4, Key.NumPad6, Key.NumPad7, Key.NumPad9, Key.NumPad1, Key.NumPad3,
                                Key.K, Key.J, Key.H, Key.L, Key.Y, Key.U, Key.B, Key.N,};

        public static double MAINHEIGHT = 926;
        public static double MAINWIDTH = 1050;

        public static Char floor = '·';
        public static Char player = '@';
        public static Char horizWall = '—';

        public static Boolean LookingAtHelpMenu = false;
        private HelpMenu help;

        public double mainLeft = (System.Windows.SystemParameters.PrimaryScreenWidth - MAINWIDTH) / 2;
        public double mainTop = (System.Windows.SystemParameters.PrimaryScreenHeight - MAINHEIGHT) / 2;

        public MainWindow()
        {
            // This is for making the splash screen appear for longer
            // System.Threading.Thread.Sleep(2000);
            InitializeComponent();
            gameArea.FontSize = titleFontSize;
            Application.Current.MainWindow.Left = mainLeft + 200;
            Application.Current.MainWindow.Top = mainTop;

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
                    Tile t = new Tile(c);
                    t.Seen = false;
                    currentRow.Add(t);
                }
                toReturn.Add(currentRow);
            }

            return toReturn;
        }

        private void keyPressed(object sender, KeyEventArgs e)
        {
            if (keys.Contains(e.Key) && !textEntry.IsFocused)
            {
                movePlayer(e.Key);
                update();
            }
            else if (e.Key == Key.OemQuestion) {
                Keyboard.Focus(textEntry);
            }
        }

        public void movePlayer(Key k)
        {
            if (k == Key.K || k == Key.NumPad8)
            {
                gameLevel.movePlayer(UP);
            }
            else if (k == Key.J || k == Key.NumPad2)
            {
                gameLevel.movePlayer(DOWN);
            }
            else if (k == Key.H || k == Key.NumPad4)
            {
                gameLevel.movePlayer(LEFT);
            }
            else if (k == Key.L || k == Key.NumPad6)
            {
                gameLevel.movePlayer(RIGHT);
            }
            else if (k == Key.Y || k == Key.NumPad7)
            {
                gameLevel.movePlayer(UP_LEFT);
            }
            else if (k == Key.U || k == Key.NumPad9)
            {
                gameLevel.movePlayer(UP_RIGHT);
            }
            else if (k == Key.B || k == Key.NumPad1)
            {
                gameLevel.movePlayer(DOWN_LEFT);
            }
            else if (k == Key.N || k == Key.NumPad3)
            {
                gameLevel.movePlayer(DOWN_RIGHT);
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
                    textEntry.Clear();
                    help = new HelpMenu();
                    help.ShowInTaskbar = false;
                    help.Left = mainLeft - (help.Width / 2);
                    help.Top = mainTop;
                    help.Owner = Application.Current.MainWindow;
                    help.ShowDialog();
                }
                return "";
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
