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
using System.Windows.Shapes;

namespace _320Hack
{
    /// <summary>
    /// Interaction logic for startGame.xaml
    /// </summary>
    public partial class startGame : Window
    {
        private int newPlayerId;
        private Player player;

        public startGame()
        {
            InitializeComponent();

            using (var db = new DbModel())
            {
                player = (from p in db.Player
                          orderby p.Id descending
                          select p).First();
            }

            newPlayerId = player.Id + 1;

            // TODO delete the player.Id check for release
            if (player.Id == MainWindow.TEST_PLAYER_ID)
            {
                // TODO If no player is available, dialog for adding one (plus add to database etc.)
                continueButton.Width = 0;
                newGameButton.Width = 484;
                newGameButton.Margin = new Thickness(10, 163, 10, 10);
                playerNameTextBox.Width = 484;
                playerNameTextBox.Margin = new Thickness(10, 123, 10, 54);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main;
            if (e.Source.Equals(newGameButton))
            {
                main = genNewPlayer();
            }
            else
            {
                main = new MainWindow(player, false);
            }
            this.Close();
            main.Show();
        }

        private MainWindow genNewPlayer()
        {
            // Start new game
            Console.WriteLine("Starting New Game");

            player = new Player
            {
                Id = newPlayerId,
                Name = playerNameTextBox.Text,
                CurrentRoom = Player.startRoomId,
                Row = Player.startRow,
                Col = Player.startCol,
                Experience = 0,
                Health = Player.maxHealth
            };

            using (var db = new DbModel())
            {
                db.Player.Attach(player);
                db.Entry(player).State = System.Data.Entity.EntityState.Added;

                List<Room> allRooms = (from r in db.Rooms select r).ToList();
                foreach (Room r in allRooms)
                {
                    r.reset();
                    db.Rooms.Attach(r);
                    db.Entry(r).State = System.Data.Entity.EntityState.Modified;
                }

                List<MonsterInstance> monsters = (from ms in db.MonsterInstances select ms).ToList();
                Random random = new Random();
                foreach (MonsterInstance m in monsters)
                {
                    m.place(db, random);
                }

                db.SaveChanges();
                return new MainWindow(player, true);
            }
        }

        private void focused(object sender, RoutedEventArgs e)
        {
            playerNameTextBox.Text = "";
        }
    }
}
