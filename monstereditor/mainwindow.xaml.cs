using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace MonsterEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private List<_320Hack.Monster> _monsters;
        private const int rowHeight = 20;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var db = new _320Hack.DbModel();
            _monsters = (from m in db.Monsters select m).ToList();

            foreach (var monster in _monsters)
            {
                addMonster(monster);
            }

            addBlankRow();
        }

        private void addBlankRow()
        {
            TextBox blankName = new TextBox() { Text = "" };
            NameCol.Children.Add(blankName);
            blankName.Tag = "NameCol " + (NameCol.Children.Count - 1);
            blankName.Height = rowHeight;
            blankName.GotFocus += focused;
            blankName.KeyUp += keyPressed;

            TextBox blankSymbol = new TextBox() { Text = "" };
            SymbolCol.Children.Add(blankSymbol);
            blankSymbol.Tag = "SymbolCol " + (SymbolCol.Children.Count - 1);
            blankSymbol.Height = rowHeight;
            blankSymbol.GotFocus += focused;
            blankSymbol.KeyUp += keyPressed;

            ColorPicker color = new ColorPicker();
            color.SelectedColor = (Color)ColorConverter.ConvertFromString("#FFFFFFFF");
            color.Tag = "ColorCol " + (ColorCol.Children.Count - 1);
            color.Height = rowHeight;
            ColorCol.Children.Add(color);

            TextBox blankHealth = new TextBox() { Text = "" };
            HealthCol.Children.Add(blankHealth);
            blankHealth.Tag = "HealthCol " + (HealthCol.Children.Count - 1);
            blankHealth.Height = rowHeight;
            blankHealth.GotFocus += focused;
            blankHealth.KeyUp += keyPressed;

            TextBox blankMinRoom = new TextBox() { Text = "" };
            MinRoomCol.Children.Add(blankMinRoom);
            blankMinRoom.Tag = "MinRoomCol " + (MinRoomCol.Children.Count - 1);
            blankMinRoom.Height = rowHeight;
            blankMinRoom.GotFocus += focused;
            blankMinRoom.KeyUp += keyPressed;

            TextBox blankSpeed = new TextBox() { Text = "" };
            SpeedCol.Children.Add(blankSpeed);
            blankSpeed.Tag = "SpeedCol " + (SpeedCol.Children.Count - 1);
            blankSpeed.Height = rowHeight;
            blankSpeed.GotFocus += focused;
            blankSpeed.KeyUp += keyPressed;

            TextBox blankAttack = new TextBox() { Text = "" };
            AttackCol.Children.Add(blankAttack);
            blankAttack.Tag = "AttackCol " + (AttackCol.Children.Count - 1);
            blankAttack.Height = rowHeight;
            blankAttack.GotFocus += focused;
            blankAttack.KeyUp += keyPressed;

            Button deleteButton = new Button()
            {
                Content = "X",
                Width = 20,
                Height = rowHeight,
                FontSize = 8,
                Background = new SolidColorBrush(Color.FromRgb(255, 105, 105)),
                BorderBrush = new SolidColorBrush(Colors.Gray),
                BorderThickness = new Thickness(1)
            };
            deleteButton.Click += deleteClick;
            DeleteCol.Children.Add(deleteButton);
            deleteButton.Tag = DeleteCol.Children.Count - 1;
        }

        private void addMonster(_320Hack.Monster monster)
        {
            TextBox nameTextBox = new TextBox() { Text = monster.Name };
            nameTextBox.KeyUp += keyPressed;
            nameTextBox.Height = rowHeight;
            NameCol.Children.Add(nameTextBox);
            nameTextBox.Tag = "NameCol " + (NameCol.Children.Count - 1);

            TextBox symbolTextBox = new TextBox() { Text = monster.Symbol };
            symbolTextBox.KeyUp += keyPressed;
            symbolTextBox.Height = rowHeight;
            SymbolCol.Children.Add(symbolTextBox);
            symbolTextBox.Tag = "SymbolCol " + (SymbolCol.Children.Count - 1);

            var convertFromString = ColorConverter.ConvertFromString(monster.Color);
            if (convertFromString != null)
            {
                ColorPicker color = new ColorPicker();
                color.SelectedColor = (Color)convertFromString;
                color.Height = rowHeight;
                color.Tag = "ColorCol " + (ColorCol.Children.Count - 1);
                ColorCol.Children.Add(color);

            }

            TextBox healthTextBox = new TextBox() { Text = monster.HP.ToString() };
            healthTextBox.KeyUp += keyPressed;
            healthTextBox.Height = rowHeight;
            HealthCol.Children.Add(healthTextBox);
            healthTextBox.Tag = "HealthCol " + (HealthCol.Children.Count - 1);

            TextBox minRoomTextBox = new TextBox() { Text = monster.MinimumRoom.ToString() };
            minRoomTextBox.KeyUp += keyPressed;
            minRoomTextBox.Height = rowHeight;
            MinRoomCol.Children.Add(minRoomTextBox);
            minRoomTextBox.Tag = "MinRoomCol " + (MinRoomCol.Children.Count - 1);

            TextBox speedTextBox = new TextBox() { Text = monster.Speed.ToString() };
            speedTextBox.KeyUp += keyPressed;
            speedTextBox.Height = rowHeight;
            SpeedCol.Children.Add(speedTextBox);
            speedTextBox.Tag = "SpeedCol " + (SpeedCol.Children.Count - 1);

            TextBox attackTextBox = new TextBox() { Text = monster.Attack.ToString() };
            attackTextBox.KeyUp += keyPressed;
            attackTextBox.Height = rowHeight;
            AttackCol.Children.Add(attackTextBox);
            attackTextBox.Tag = "AttackCol " + (AttackCol.Children.Count - 1);

            Button deleteButton = new Button()
            {
                Content = "X",
                Width = 20,
                Height = rowHeight,
                FontSize = 8,
                Background = new SolidColorBrush(Color.FromRgb(255, 105, 105)),
                BorderBrush = new SolidColorBrush(Colors.Gray),
                BorderThickness = new Thickness(1)
            };
            deleteButton.Click += deleteClick;
            DeleteCol.Children.Add(deleteButton);
            deleteButton.Tag = DeleteCol.Children.Count - 1;
        }

        public void deleteClick(Object sender, RoutedEventArgs e)
        {
            int rowToDelete = Convert.ToInt32((sender as Button).Tag.ToString());
            String monsterName = (NameCol.Children[rowToDelete] as TextBox).Text;

            removeRow(rowToDelete);
            if (monsterName != "")
            {
                using (var db = new _320Hack.DbModel())
                {
                    var toRemove = (from m in db.Monsters where m.Name == monsterName select m).SingleOrDefault();
                    if (toRemove != null)
                    {
                        db.Monsters.Remove(toRemove);
                    }
                    db.SaveChanges();
                }
            }
            else
            {
                if (!blankRow(NameCol.Children.Count - 1)) addBlankRow();
            }

            for (int i = 0; i < NameCol.Children.Count; i++)
            {
                validateRow(i);
            }
        }

        private void removeRow(int rowToDelete)
        {
            NameCol.Children.RemoveAt(rowToDelete);
            SymbolCol.Children.RemoveAt(rowToDelete);
            ColorCol.Children.RemoveAt(rowToDelete);
            HealthCol.Children.RemoveAt(rowToDelete);
            MinRoomCol.Children.RemoveAt(rowToDelete);
            SpeedCol.Children.RemoveAt(rowToDelete);
            AttackCol.Children.RemoveAt(rowToDelete);
            DeleteCol.Children.RemoveAt(rowToDelete);

            for (int i = rowToDelete; i < NameCol.Children.Count; i++)
            {
                (DeleteCol.Children[i] as Button).Tag = (int)((DeleteCol.Children[i] as Button).Tag) - 1;
            }
        }

        private bool validateEverything()
        {
            // Don't validate the last empty row.
            for (int i = 0; i < NameCol.Children.Count - 1; i++)
            {
                bool valid = validateRow(i);
                if (!valid) return false;
            }
            return true;
        }

        public bool validateRow(int row)
        {
            if (!validateName((NameCol.Children[row] as TextBox).Text))
            {
                // TODO: Throw Name Error
                return false;
            }
            if (!validateSymbol((SymbolCol.Children[row] as TextBox).Text))
            {
                // TODO: Throw Symbol Error
                return false;
            }
            if (!validateNum((HealthCol.Children[row] as TextBox).Text))
            {
                // TODO: Throw Health Error
                return false;
            }
            if (!validateMinRoom((MinRoomCol.Children[row] as TextBox).Text))
            {
                // TODO: Throw MinimumRoom Error
                return false;
            }
            if (!validateNum((SpeedCol.Children[row] as TextBox).Text))
            {
                // TODO: Throw Speed Error
                return false;
            }
            if (!validateNum((AttackCol.Children[row] as TextBox).Text))
            {
                // TODO: Throw AttackError
                return false;
            }
            SaveBorder.Background = new SolidColorBrush(Colors.Green);
            SaveButton.IsEnabled = true;
            return true;
        }

        private bool validateName(String name)
        {
            return name != null && name != "";
        }

        private bool validateSymbol(String sym)
        {
            return sym != null && sym.Count() == 1;
        }

        private bool validateMinRoom(String room)
        {
            try
            {
                int roomNum = Convert.ToInt32(room);
                using (var db = new _320Hack.DbModel())
                {
                    int numRoomsInGame = db.Rooms.Count();
                    return roomNum >= 1 && roomNum <= numRoomsInGame;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private bool validateNum(String num)
        {
            try
            {
                int numConverted = Convert.ToInt32(num);
                return numConverted >= 1;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private void keyPressed(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var castToTextBox = sender as TextBox;
            if (castToTextBox != null)
            {
                if (castToTextBox.IsFocused)
                {
                    String[] tokens = castToTextBox.Tag.ToString().Split(' ');
                    String tag = tokens[0];

                    if (tag == "NameCol")
                    {
                        if (!validateName(castToTextBox.Text))
                        {
                            // Error in your validation. Change something on UI and disable save button eventually
                            errorInField(castToTextBox);
                        }
                        else
                        {
                            noErrorsInField(castToTextBox);
                        }
                    }
                    else if (tag == "SymbolCol")
                    {
                        if (!validateSymbol(castToTextBox.Text))
                        {
                            // Error in your validation. Change something on UI and disable save button eventually
                            errorInField(castToTextBox);
                        }
                        else
                        {
                            noErrorsInField(castToTextBox);
                        }
                    }
                    else if (tag == "HealthCol")
                    {
                        if (!validateNum(castToTextBox.Text))
                        {
                            // Error in your validation. Change something on UI and disable save button eventually
                            errorInField(castToTextBox);
                        }
                        else
                        {
                            noErrorsInField(castToTextBox);
                        }
                    }
                    else if (tag == "MinRoomCol")
                    {
                        if (!validateMinRoom(castToTextBox.Text))
                        {
                            // Error in your validation. Change something on UI and disable save button eventually
                            errorInField(castToTextBox);
                        }
                        else
                        {
                            noErrorsInField(castToTextBox);
                        }
                    }
                    else if (tag == "SpeedCol")
                    {
                        if (!validateNum(castToTextBox.Text))
                        {
                            // Error in your validation. Change something on UI and disable save button eventually
                            errorInField(castToTextBox);
                        }
                        else
                        {
                            noErrorsInField(castToTextBox);
                        }
                    }
                    else if (tag == "AttackCol")
                    {
                        if (!validateNum(castToTextBox.Text))
                        {
                            // Error in your validation. Change something on UI and disable save button eventually
                            errorInField(castToTextBox);
                        }
                        else
                        {
                            noErrorsInField(castToTextBox);
                        }
                    }
                }
            }
        }

        private void noErrorsInField(TextBox castToTextBox)
        {
            castToTextBox.BorderBrush = new SolidColorBrush(Colors.Gray);
            ErrorLabel.Content = "Errors: No Errors";
            castToTextBox.ToolTip = null;
            SaveBorder.Background = new SolidColorBrush(Colors.Green);
            SaveButton.IsEnabled = true;
        }

        private void errorInField(TextBox castToTextBox)
        {
            castToTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
            SaveBorder.Background = new SolidColorBrush(Colors.Red);
            castToTextBox.BorderThickness = new Thickness(1);
            ErrorLabel.Content = "Errors: Hover over the errors to see what's wrong";
            SaveButton.IsEnabled = false;
            if (castToTextBox.Tag.ToString() == "NameCol")
            {
                castToTextBox.ToolTip = "Name cannot be empty.";
            }
            else if (castToTextBox.Tag.ToString() == "SymbolCol")
            {
                castToTextBox.ToolTip = "Symbol must only be one character.";
            }
            else if (castToTextBox.Tag.ToString() == "MinRoomCol")
            {
                int numRooms = 1;
                using (var db = new _320Hack.DbModel())
                {
                    numRooms = db.Rooms.Count();
                }
                castToTextBox.ToolTip = "MinRoom must be within interval [1-" + numRooms + "]";
            }
            else if (castToTextBox.Tag.ToString() == "HealthCol")
            {
                castToTextBox.ToolTip = "Health must be a number greater than 0.";
            }
            else if (castToTextBox.Tag.ToString() == "SpeedCol")
            {
                castToTextBox.ToolTip = "Speed must be a number greater than 0.";
            }
            else if (castToTextBox.Tag.ToString() == "AttackCol")
            {
                castToTextBox.ToolTip = "Attack must be a number greater than 0.";
            }
        }

        private void focused(object sender, RoutedEventArgs e)
        {
            int selectedRow = Convert.ToInt32((sender as TextBox).Tag.ToString().Split(' ')[1]);
            if (blankRow(selectedRow))
            {
                addBlankRow();
                TextBox newName = (NameCol.Children[selectedRow] as TextBox);
                newName.Text = "Example";
                TextBox newSymbol = (SymbolCol.Children[selectedRow] as TextBox);
                newSymbol.Text = "%";
                TextBox newHealth = (HealthCol.Children[selectedRow] as TextBox);
                newHealth.Text = "1";
                TextBox newMinRoom = (MinRoomCol.Children[selectedRow] as TextBox);
                newMinRoom.Text = "1";
                TextBox newSpeed = (SpeedCol.Children[selectedRow] as TextBox);
                newSpeed.Text = "500";
                TextBox newAttack = (AttackCol.Children[selectedRow] as TextBox);
                newAttack.Text = "20";
            }
        }

        private bool blankRow(int row)
        {
            bool name = (NameCol.Children[row] as TextBox).Text == "";
            bool symbol = (SymbolCol.Children[row] as TextBox).Text == "";
            bool color = ((ColorCol.Children[row] as ColorPicker).SelectedColor.ToString() == "#FFFFFFFF");
            bool health = (HealthCol.Children[row] as TextBox).Text == "";
            bool minRoom = (MinRoomCol.Children[row] as TextBox).Text == "";
            bool speed = (SpeedCol.Children[row] as TextBox).Text == "";
            bool attack = (AttackCol.Children[row] as TextBox).Text == "";

            return name && symbol && color && health && minRoom && speed && attack;
        }

        public void saveDb(Object sender, RoutedEventArgs e)
        {
            if (validateEverything())
            {
                // Don't write the last empty row.
                for (int row = 0; row < NameCol.Children.Count - 1; row++)
                {
                    SaveRowToDb(row);
                }
                SaveButton.Content = "Saved!";
            }
        }

        public void SaveRowToDb(int row)
        {
            using (var db = new _320Hack.DbModel())
            {
                String monsterName = (NameCol.Children[row] as TextBox).Text;
                var monster = (from m in db.Monsters where m.Name == monsterName select m).SingleOrDefault();
                bool shouldCreate = monster == null;
                if (shouldCreate)
                {
                    monster = new _320Hack.Monster { Name = monsterName };
                }
                monster.Symbol = (SymbolCol.Children[row] as TextBox).Text;
                monster.Color = (ColorCol.Children[row] as ColorPicker).SelectedColorText;
                monster.HP = Convert.ToInt32((HealthCol.Children[row] as TextBox).Text);
                monster.MinimumRoom = Convert.ToInt32((MinRoomCol.Children[row] as TextBox).Text);
                monster.Speed = Convert.ToInt32((SpeedCol.Children[row] as TextBox).Text);
                monster.Attack = Convert.ToInt32((AttackCol.Children[row] as TextBox).Text);

                db.Entry(monster).State = shouldCreate ? System.Data.Entity.EntityState.Added : System.Data.Entity.EntityState.Modified;

                db.SaveChanges();
            }
        }
    }
}
