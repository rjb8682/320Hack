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

        public const int monsterModifier = 5;

        public Player player;

        public static Boolean LookingAtHelpMenu = false;
        public bool newGame;
        private HelpMenu help;

        public double mainLeft = (System.Windows.SystemParameters.PrimaryScreenWidth - MAINWIDTH) / 2;
        public double mainTop = (System.Windows.SystemParameters.PrimaryScreenHeight - MAINHEIGHT) / 2;

        public MainWindow(Player player, bool newGame)
        {
            // This is for making the splash screen appear for longer
            // System.Threading.Thread.Sleep(2000);
            InitializeComponent();
            gameArea.FontSize = titleFontSize;
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

                List<MonsterHistory> killedMonsters = (from m in db.MonsterHistory
                                                       where m.PlayerId == player.Id
                                                       orderby m.Count descending
                                                       select m).ToList();

                int sum = 0;
                foreach (MonsterHistory m in killedMonsters)
                {
                    sum += m.Count;
                }
                deathArea.Text += "\nYou have killed " + sum + " monster" + (sum > 0 ? "s" : "") + ":\n\n";

                int count = 0;
                foreach (MonsterHistory killedMonster in killedMonsters)
                {
                    Monster monster = (from m in db.Monsters
                                       where m.Id == killedMonster.MonsterId
                                       select m).First();

                    for (int i = 0; i < killedMonster.Count; i++)
                    {
                        deathArea.Inlines.Add(new Run(monster.Symbol)
                        {
                            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(monster.Color))
                        });

                        count++;
                        if (count == 10)
                        {
                            deathArea.Inlines.Add("\n");
                            count = 0;
                        }
                    }
                    deathArea.Inlines.Add("\n");
                    count = 0;
                }
            }
            outputPanel.Text += "Would you like to continue your adventure? (y/n)";
        }

        private void keyPressed(object sender, KeyEventArgs e)
        {
            if (player.isDead())
            {
                if (e.Key == Key.Y)
                {
                    revive();
                    gameLevel.setupState(player.CurrentRoom);
                    outputPanel.Text = "";
                    deathArea.Text = "";
                    update();
                }
                else if (e.Key == Key.N)
                {
                    new startGame().Show();
                    this.Close();
                }
            }
            else if (e.Key == Key.OemTilde)
            {
                player.awardExperience(1);
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
                gameLevel.movePlayer(0,0, true, false);
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

                List<Monster> monsters;
                List<Room> rooms = (from r in db.Rooms select r).ToList();
                Random random = new Random();

                deleteMonsters(db);

                foreach (Room r in rooms)
                {
                    monsters = (from ms in db.Monsters where ms.MinimumRoom <= r.Id select ms).ToList();
                    for (int i = 0; i < monsterModifier * r.Id; i++)
                    {
                        Monster template = monsters[random.Next(0, monsters.Count)];
                        MonsterInstance m = new MonsterInstance()
                        {
                            Color = template.Color,
                            Name = template.Name,
                            Symbol = template.Symbol,
                            MonsterId = template.Id,
                            RoomId = r.Id,
                            Power = (int) (template.Attack + player.getLevel() * 0.75) + (5 * r.Id),
                            Speed = template.Speed,
                            SpeedTowardNextTurn = 0
                        };
                        m.place(db, random);
                    }
                }

                db.SaveChanges();
            }
        }

        public void deleteMonsters(DbModel db)
        {
            List<MonsterInstance> otherMonsters = (from ms in db.MonsterInstances select ms).ToList();
            foreach (MonsterInstance m in otherMonsters)
            {
                db.MonsterInstances.Attach(m);
                db.Entry(m).State = System.Data.Entity.EntityState.Deleted;
            }
            db.SaveChanges();
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
