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
using System.Reflection;

namespace _320Hack
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const double titleFontSize = 16.0;
        private readonly Map gameLevel;

        public Key[] keys = {Key.NumPad8, Key.NumPad2, Key.NumPad4, Key.NumPad6, Key.NumPad7, Key.NumPad9, Key.NumPad1, Key.NumPad3,
                                Key.K, Key.J, Key.H, Key.L, Key.Y, Key.U, Key.B, Key.N, Key.Left, Key.Right, Key.Up, Key.Down};

        public const int TEST_PLAYER_ID = 1;
        public const int INVALID_POSITION = -1;

        public static char floor = '·';
        public static char playerChar = '@';
        public static char horizWall = '—';
        public static char door = '#';

        public const int monsterModifier = 5;

        public Player player;

        public static Boolean LookingAtHelpMenu = false;
        public bool newGame;
        private HelpMenu help;

        public double mainTop;
        public double mainLeft;
        public double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
        public double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;

        public MainWindow(Player player, bool newGame)
        {
            InitializeComponent();
            gameArea.FontSize = titleFontSize;
            mainLeft = ((screenWidth - this.ActualWidth) / 2);
            mainTop = ((screenHeight - this.ActualHeight) / 2);
            Application.Current.MainWindow.Left = mainLeft + 200;
            Application.Current.MainWindow.Top = mainTop;
            this.newGame = newGame;

            this.player = player;

            if (player.isDead() || this.newGame)
            {
                revive();
            }

            gameLevel = new Map(player);
            if (this.newGame)
            {
                startGameSequence();
            }
            else
            {
                update();
            }
        }

        public void startGameSequence()
        {
            // This will be for the "story" of the game
            Console.WriteLine("Story Sequence");
            gameArea.Text = new StreamReader("../../GameData/beginning.txt").ReadToEnd();
        }

        public void update()
        {
            gameLevel.printMap(gameArea);
            healthBar.Value = player.Health;
            healthBar.Maximum = player.maxHealth;
            if (player.isDead())
            {
                showDeathStuff();
            }
            levelBar.Value = 100 * player.getFracToNextLevel();
            statPanel.Text = player.getInfo();
        }

        // TODO Make monster history taly up as you go
        public void showDeathStuff()
        {
            using (var db = new DbModel())
            {

                var killedMonsters = (from m in db.MonsterHistory
                                      where m.PlayerId == player.Id
                                      orderby m.Count descending
                                      select m).ToList();

                var sum = killedMonsters.Sum(m => m.Count);
                deathArea.Text += "\nYou have killed " + sum + " monster" + (sum != 1 ? "s" : "") + ":\n\n";

                var count = 0;
                foreach (var killedMonster in killedMonsters)
                {
                    var monster = (from m in db.Monsters
                                       where m.Id == killedMonster.MonsterId
                                       select m).First();

                    for (var i = 0; i < killedMonster.Count; i++)
                    {
                        deathArea.Inlines.Add(new Run(monster.Symbol)
                        {
                            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(monster.Color))
                        });

                        count++;
                        if (count != 10) continue;

                        deathArea.Inlines.Add("\n");
                        count = 0;
                    }
                    deathArea.Inlines.Add("\n");
                    count = 0;
                }
            }
            outputPanel.Children.Add(getTextBox("Would you like to continue your adventure? (y/n)"));
        }

        private void keyPressed(object sender, KeyEventArgs e)
        {
            Console.WriteLine("Key pressed: " + e.Key);

            if (player.isDead())
            {
                switch (e.Key)
                {
                    case Key.Y:
                        revive();
                        gameLevel.setupState(player.CurrentRoom);
                        outputPanel.Children.Clear();
                        deathArea.Text = "";
                        update();
                        break;
                    case Key.N:
                        new startGame().Show();
                        Close();
                        break;
                    default:
                        return;
                }
            }
            else if (e.Key == Key.OemTilde)
            {
                outputPanel.Children.Add(getTextBox(player.awardExperience(1)));
                update();
            }
            else if (keys.Contains(e.Key) && !textEntry.IsFocused && !player.isDead())
            {
                movePlayer(e.Key);
                update();
            }
            else if (e.Key == Key.OemQuestion) {
                Keyboard.Focus(textEntry);
            }
            else if (e.Key == Key.OemPeriod && !player.isDead())
            {
                gameLevel.movePlayer(0,0, true);
                update();
            }
            else if (e.Key == Key.OemComma && !player.isDead())
            {
                gameLevel.movePlayer(0, 0, true, true);
                update();
            }
            else if (newGame && (e.Key == Key.Space))
            {
                update();
            }
        }

        public void revive()
        {
            using (var db = new DbModel())
            {
                player.revive();
                db.Player.Attach(player);
                db.Entry(player).State = System.Data.Entity.EntityState.Modified;

                var rooms = (from r in db.Rooms select r).ToList();
                var random = new Random();

                deleteMonsters(db);

                foreach (var r in rooms)
                {
                    var monsters = (from ms in db.Monsters where ms.MinimumRoom <= r.Id select ms).ToList();
                    for (var i = 0; i < monsterModifier * r.Id; i++)
                    {
                        var template = monsters[random.Next(0, monsters.Count)];
                        var m = new MonsterInstance(template, player.getLevel(), r.Id);
                        m.place(db, random);
                    }
                }

                db.SaveChanges();
            }
        }

        public void deleteMonsters(DbModel db)
        {
            var otherMonsters = (from ms in db.MonsterInstances select ms).ToList();
            foreach (var m in otherMonsters)
            {
                db.MonsterInstances.Attach(m);
                db.Entry(m).State = System.Data.Entity.EntityState.Deleted;
            }
            db.SaveChanges();
        }

        public void movePlayer(Key k)
        {
            Console.WriteLine("Moving player: " + k);
            string responseFromMoving;

            switch (k)
            {
                case Key.K:
                case Key.NumPad8:
                case Key.Up:
                    responseFromMoving = gameLevel.movePlayer(-1, 0);
                    break;
                case Key.J:
                case Key.NumPad2:
                case Key.Down:
                    responseFromMoving = gameLevel.movePlayer(+1, 0);
                    break;
                case Key.H:
                case Key.NumPad4:
                case Key.Left:
                    responseFromMoving = gameLevel.movePlayer(0, -1);
                    break;
                case Key.L:
                case Key.NumPad6:
                case Key.Right:
                    responseFromMoving = gameLevel.movePlayer(0, +1);
                    break;
                case Key.Y:
                case Key.NumPad7:
                    responseFromMoving = gameLevel.movePlayer(-1, -1);
                    break;
                case Key.U:
                case Key.NumPad9:
                    responseFromMoving = gameLevel.movePlayer(-1, +1);
                    break;
                case Key.B:
                case Key.NumPad1:
                    responseFromMoving = gameLevel.movePlayer(+1, -1);
                    break;
                case Key.N:
                case Key.NumPad3:
                    responseFromMoving = gameLevel.movePlayer(+1, +1);
                    break;
                default:
                    return;
            }

            if (responseFromMoving == "") return;

            foreach (var s in responseFromMoving.Split('.'))
            {
                if (s == "") continue;

                outputPanel.Children.Add(getTextBox(s));
                OutputScrollViewer.ScrollToBottom();
            }
        }

        private void textEntered(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            var response = processInput(textEntry.Text);
            textEntry.Clear();
            outputPanel.Children.Add(getTextBox(response));
            gameArea.IsEnabled = true;
            Keyboard.Focus(gameArea);
            gameArea.IsEnabled = false;
        }

        public string processInput(string text)
        {
            switch (text)
            {
                case "/help":
                    textEntry.Clear();
                    help = new HelpMenu
                    {
                        ShowInTaskbar = false,
                        Left = mainLeft - (help.ActualWidth / 2),
                        Top = mainTop
                    };

                    help.ShowDialog();
                    break;
                case "/quit":
                    textEntry.Clear();
                    new startGame().Show();
                    Close();
                    break;
                default:
                    return "Not a command. Input /help to see a list of commands";
            }
            return "";
        }

        private void isClosed(object sender, EventArgs e)
        {
            //gameLevel.reset();
        }

        private TextBox getTextBox(String text)
        {
            var bgColor = ColorConverter.ConvertFromString("#00FFFFFF");
            var fgColor = ColorConverter.ConvertFromString("#FFE6E6E6");

            return new TextBox
            {
                Text = text,
                Background = new SolidColorBrush((Color?) bgColor ?? Colors.Transparent),
                FontSize = 14,
                Foreground = new SolidColorBrush((Color?) fgColor ?? Colors.DarkGray),
                BorderThickness = new Thickness(0),
                IsReadOnly = true
            };
        }

    }
}
