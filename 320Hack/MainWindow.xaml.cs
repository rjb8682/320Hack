﻿using System;
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

        public Key[] keys = {Key.NumPad8, Key.NumPad2, Key.NumPad4, Key.NumPad6, Key.NumPad7, Key.NumPad9, Key.NumPad1, Key.NumPad3,
                                Key.K, Key.J, Key.H, Key.L, Key.Y, Key.U, Key.B, Key.N,};

        public static double MAINHEIGHT = 926;
        public static double MAINWIDTH = 1050;

        public const int TEST_PLAYER_ID = 1;
        public const int INVALID_POSITION = -1;

        public static Char floor = '·';
        public static Char playerChar = '@';
        public static Char horizWall = '—';
        public static Char door = '#';

        public Player player;

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

            using (var db = new DbModel())
            {
                player = (from p in db.Player
                          orderby p.Id descending
                          select p).First();
            }

            // TODO delete the player.Id check for release
            if (player.isDead() || player.Id == TEST_PLAYER_ID)
            {
                // TODO If no player is available, dialog for adding one (plus add to database etc.)
                Console.WriteLine("You need a new player");
            }

            gameLevel = new Map(player);
            update();
        }

        public void update()
        {
            gameArea.Text = gameLevel.printMap();
            healthBar.Value = player.Health;
            levelBar.Value = 100 * player.getFracToNextLevel();
            outputPanel.Text = player.getInfo();
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
                gameLevel.movePlayer(-1, 0);
            }
            else if (k == Key.J || k == Key.NumPad2)
            {
                gameLevel.movePlayer(+1, 0);
            }
            else if (k == Key.H || k == Key.NumPad4)
            {
                gameLevel.movePlayer(0, -1);
            }
            else if (k == Key.L || k == Key.NumPad6)
            {
                gameLevel.movePlayer(0, +1);
            }
            else if (k == Key.Y || k == Key.NumPad7)
            {
                gameLevel.movePlayer(-1, -1);
            }
            else if (k == Key.U || k == Key.NumPad9)
            {
                gameLevel.movePlayer(-1, +1);
            }
            else if (k == Key.B || k == Key.NumPad1)
            {
                gameLevel.movePlayer(+1, -1);
            }
            else if (k == Key.N || k == Key.NumPad3)
            {
                gameLevel.movePlayer(+1, +1);
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
            if (text != "" && text[0] != '/')
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

        private void isClosed(object sender, EventArgs e)
        {
            //gameLevel.reset();
        }

    }
}
