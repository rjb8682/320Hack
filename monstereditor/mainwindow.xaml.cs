using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

            //addBlankRow();
        }

        private void addBlankRow()
        {
            RowDefinition rowDef = new RowDefinition();
            rowDef.Height = new GridLength(rowHeight);
            NameCol.RowDefinitions.Add(rowDef);
            TextBox blankName = new TextBox() { Text = "" };
            NameCol.Children.Add(blankName);
            Grid.SetRow(blankName, NameCol.Children.Count - 1);

            rowDef = new RowDefinition();
            rowDef.Height = new GridLength(rowHeight);
            SymbolCol.RowDefinitions.Add(rowDef);
            TextBox blankSymbol = new TextBox() { Text = "" };
            SymbolCol.Children.Add(blankSymbol);
            Grid.SetRow(blankSymbol, SymbolCol.Children.Count - 1);

            rowDef = new RowDefinition();
            rowDef.Height = new GridLength(rowHeight);
            Border color = new Border()
            {
                Background = new SolidColorBrush(Colors.White),
                Height = rowHeight,
                Width = Convert.ToDouble(MonsterGrid.ColumnDefinitions[2].ActualWidth),
                BorderBrush = new SolidColorBrush(Colors.Gray),
                BorderThickness = new Thickness(1),
                VerticalAlignment = VerticalAlignment.Center
            };
            ColorCol.RowDefinitions.Add(rowDef);
            ColorCol.Children.Add(color);
            Grid.SetRow(color, ColorCol.Children.Count - 1);

            rowDef = new RowDefinition();
            rowDef.Height = new GridLength(rowHeight);
            HealthCol.RowDefinitions.Add(rowDef);
            TextBox blankHealth = new TextBox() { Text = "" };
            HealthCol.Children.Add(blankHealth);
            Grid.SetRow(blankHealth, HealthCol.Children.Count - 1);

            rowDef = new RowDefinition();
            rowDef.Height = new GridLength(rowHeight);
            MinRoomCol.RowDefinitions.Add(rowDef);
            TextBox blankMinRoom = new TextBox() { Text = "" };
            MinRoomCol.Children.Add(blankMinRoom);
            Grid.SetRow(blankMinRoom, MinRoomCol.Children.Count - 1);

            rowDef = new RowDefinition();
            rowDef.Height = new GridLength(rowHeight);
            SpeedCol.RowDefinitions.Add(rowDef);
            TextBox blankSpeed = new TextBox() { Text = "" };
            SpeedCol.Children.Add(blankSpeed);
            Grid.SetRow(blankSpeed, SpeedCol.Children.Count - 1);

            rowDef = new RowDefinition();
            rowDef.Height = new GridLength(rowHeight);
            AttackCol.RowDefinitions.Add(rowDef);
            TextBox blankAttack = new TextBox() { Text = "" };
            AttackCol.Children.Add(blankAttack);
            Grid.SetRow(blankAttack, AttackCol.Children.Count - 1);

            rowDef = new RowDefinition();
            rowDef.Height = new GridLength(rowHeight);
            DeleteCol.RowDefinitions.Add(rowDef);
            Button deleteButton = new Button()
            {
                Content = "X",
                Width = 20,
                Height = 15,
                FontSize = 8,
                Background = new SolidColorBrush(Color.FromRgb(255, 105, 105)),
                BorderBrush = new SolidColorBrush(Colors.Gray),
                BorderThickness = new Thickness(1)
            };
            deleteButton.Click += deleteClick;
            DeleteCol.Children.Add(deleteButton);
            deleteButton.Tag = DeleteCol.Children.Count - 1;
            Grid.SetRow(deleteButton, DeleteCol.Children.Count - 1);
        }

        private void addMonster(_320Hack.Monster monster)
        {
            RowDefinition rowDef = new RowDefinition();
            rowDef.Height = new GridLength(rowHeight);
            NameCol.RowDefinitions.Add(rowDef);
            TextBox nameTextBox = new TextBox() { Text = monster.Name };
            nameTextBox.KeyUp += keyPressed;
            NameCol.Children.Add(nameTextBox);
            nameTextBox.Tag = "NameCol " + (NameCol.Children.Count - 1);
            Grid.SetRow(nameTextBox, NameCol.Children.Count - 1);

            rowDef = new RowDefinition();
            rowDef.Height = new GridLength(rowHeight);
            SymbolCol.RowDefinitions.Add(rowDef);
            TextBox symbolTextBox = new TextBox() { Text = monster.Symbol };
            symbolTextBox.KeyUp += keyPressed;
            SymbolCol.Children.Add(symbolTextBox);
            symbolTextBox.Tag = "SymbolCol " + (SymbolCol.Children.Count - 1);
            Grid.SetRow(symbolTextBox, SymbolCol.Children.Count - 1);

            var convertFromString = ColorConverter.ConvertFromString(monster.Color);
            if (convertFromString != null)
            {
                rowDef = new RowDefinition();
                rowDef.Height = new GridLength(rowHeight);
                Border colorBorder = new Border()
                {
                    Background = new SolidColorBrush((Color)convertFromString),
                    Height = rowHeight,
                    Width = Convert.ToDouble(MonsterGrid.ColumnDefinitions[2].ActualWidth),
                    BorderBrush = new SolidColorBrush(Colors.Gray),
                    BorderThickness = new Thickness(1),
                    VerticalAlignment = VerticalAlignment.Center
                };
                ColorCol.RowDefinitions.Add(rowDef);
                ColorCol.Children.Add(colorBorder);
                Grid.SetRow(colorBorder, ColorCol.Children.Count - 1);
            }
            rowDef = new RowDefinition();
            rowDef.Height = new GridLength(rowHeight);
            HealthCol.RowDefinitions.Add(rowDef);
            TextBox healthTextBox = new TextBox() { Text = monster.HP.ToString() };
            healthTextBox.KeyUp += keyPressed;
            HealthCol.Children.Add(healthTextBox);
            healthTextBox.Tag = "HealthCol " + (HealthCol.Children.Count - 1);
            Grid.SetRow(healthTextBox, HealthCol.Children.Count - 1);

            rowDef = new RowDefinition();
            rowDef.Height = new GridLength(rowHeight);
            MinRoomCol.RowDefinitions.Add(rowDef);
            TextBox minRoomTextBox = new TextBox() { Text = monster.MinimumRoom.ToString() };
            minRoomTextBox.KeyUp += keyPressed;
            MinRoomCol.Children.Add(minRoomTextBox);
            minRoomTextBox.Tag = "MinRoomCol " + (MinRoomCol.Children.Count - 1);
            Grid.SetRow(minRoomTextBox, MinRoomCol.Children.Count - 1);

            rowDef = new RowDefinition();
            rowDef.Height = new GridLength(rowHeight);
            SpeedCol.RowDefinitions.Add(rowDef);
            TextBox speedTextBox = new TextBox() { Text = monster.Speed.ToString() };
            speedTextBox.KeyUp += keyPressed;
            SpeedCol.Children.Add(speedTextBox);
            speedTextBox.Tag = "SpeedCol " + (SpeedCol.Children.Count - 1);
            Grid.SetRow(speedTextBox, SpeedCol.Children.Count - 1);

            rowDef = new RowDefinition();
            rowDef.Height = new GridLength(rowHeight);
            AttackCol.RowDefinitions.Add(rowDef);
            TextBox attackTextBox = new TextBox() { Text = monster.Attack.ToString() };
            attackTextBox.KeyUp += keyPressed;
            AttackCol.Children.Add(attackTextBox);
            attackTextBox.Tag = "AttackCol " + (AttackCol.Children.Count - 1);
            Grid.SetRow(attackTextBox, AttackCol.Children.Count - 1);

            rowDef = new RowDefinition();
            rowDef.Height = new GridLength(rowHeight);
            DeleteCol.RowDefinitions.Add(rowDef);
            Button deleteButton = new Button()
            {
                Content = "X",
                Width = 20,
                Height = 15,
                FontSize = 8,
                Background = new SolidColorBrush(Color.FromRgb(255, 105, 105)),
                BorderBrush = new SolidColorBrush(Colors.Gray),
                BorderThickness = new Thickness(1)
            };
            deleteButton.Click += deleteClick;
            DeleteCol.Children.Add(deleteButton);
            deleteButton.Tag = DeleteCol.Children.Count - 1;
            Grid.SetRow(deleteButton, DeleteCol.Children.Count - 1);
        }

        public void deleteClick(Object sender, RoutedEventArgs e)
        {
            int rowToDelete = (int)(sender as Button).Tag;
            String monsterName = (NameCol.Children[rowToDelete] as TextBox).Text;

            removeRow(rowToDelete);
            if (monsterName != "")
            {
                using (var db = new _320Hack.DbModel())
                {
                    var toRemove = (from m in db.Monsters where m.Name == monsterName select m).Single();
                    db.Monsters.Remove(toRemove);

                    db.SaveChanges();
                }
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

            NameCol.RowDefinitions.RemoveAt(rowToDelete);
            SymbolCol.RowDefinitions.RemoveAt(rowToDelete);
            ColorCol.RowDefinitions.RemoveAt(rowToDelete);
            HealthCol.RowDefinitions.RemoveAt(rowToDelete);
            MinRoomCol.RowDefinitions.RemoveAt(rowToDelete);
            SpeedCol.RowDefinitions.RemoveAt(rowToDelete);
            AttackCol.RowDefinitions.RemoveAt(rowToDelete);
            DeleteCol.RowDefinitions.RemoveAt(rowToDelete);
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
                    String tag = castToTextBox.Tag.ToString();
                    String[] tokens = tag.Split(' ');
                    int row = Convert.ToInt32(tokens[1]);

                    if (tokens[0] == "NameCol")
                    {
                        if (!validateName(castToTextBox.Text))
                        {
                            // Error in your validation. Change something on UI and disable save button eventually
                            castToTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                            SaveBorder.Background = new SolidColorBrush(Colors.Red);
                            castToTextBox.BorderThickness = new Thickness(1);
                            ErrorLabel.Content = "Errors: Please correct your errors before saving.";
                            SaveButton.IsEnabled = false;
                        }
                        else
                        {
                            castToTextBox.BorderThickness = new Thickness(0);
                            ErrorLabel.Content = "Errors: No Errors";
                            SaveBorder.Background = new SolidColorBrush(Colors.Green);
                            SaveButton.IsEnabled = true;
                        }
                    }
                    else if (tokens[0] == "SymbolCol")
                    {
                        if (!validateSymbol(castToTextBox.Text))
                        {
                            // Error in your validation. Change something on UI and disable save button eventually
                            castToTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                            SaveBorder.Background = new SolidColorBrush(Colors.Red);
                            castToTextBox.BorderThickness = new Thickness(1);
                            ErrorLabel.Content = "Errors: Please correct your errors before saving.";
                            SaveButton.IsEnabled = false;
                        }
                        else
                        {
                            castToTextBox.BorderThickness = new Thickness(0);
                            ErrorLabel.Content = "Errors: No Errors";
                            SaveBorder.Background = new SolidColorBrush(Colors.Green);
                            SaveButton.IsEnabled = true;
                        }
                    }
                    else if (tokens[0] == "HealthCol")
                    {
                        if (!validateNum(castToTextBox.Text))
                        {
                            // Error in your validation. Change something on UI and disable save button eventually
                            castToTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                            SaveBorder.Background = new SolidColorBrush(Colors.Red);
                            castToTextBox.BorderThickness = new Thickness(1);
                            ErrorLabel.Content = "Errors: Please correct your errors before saving.";
                            SaveButton.IsEnabled = false;
                        }
                        else
                        {
                            castToTextBox.BorderThickness = new Thickness(0);
                            ErrorLabel.Content = "Errors: No Errors";
                            SaveBorder.Background = new SolidColorBrush(Colors.Green);
                            SaveButton.IsEnabled = true;
                        }
                    }
                    else if (tokens[0] == "MinRoomCol")
                    {
                        if (!validateMinRoom(castToTextBox.Text))
                        {
                            // Error in your validation. Change something on UI and disable save button eventually
                            castToTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                            castToTextBox.BorderThickness = new Thickness(1);
                            SaveBorder.Background = new SolidColorBrush(Colors.Red);
                            ErrorLabel.Content = "Errors: Please correct your errors before saving.";
                            SaveButton.IsEnabled = false;
                        }
                        else
                        {
                            castToTextBox.BorderThickness = new Thickness(0);
                            ErrorLabel.Content = "Errors: No Errors";
                            SaveBorder.Background = new SolidColorBrush(Colors.Green);
                            SaveButton.IsEnabled = true;
                        }
                    }
                    else if (tokens[0] == "SpeedCol")
                    {
                        if (!validateNum(castToTextBox.Text))
                        {
                            // Error in your validation. Change something on UI and disable save button eventually
                            castToTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                            castToTextBox.BorderThickness = new Thickness(1);
                            SaveBorder.Background = new SolidColorBrush(Colors.Red);
                            ErrorLabel.Content = "Errors: Please correct your errors before saving.";
                            SaveButton.IsEnabled = false;
                        }
                        else
                        {
                            castToTextBox.BorderThickness = new Thickness(0);
                            ErrorLabel.Content = "Errors: No Errors";
                            SaveBorder.Background = new SolidColorBrush(Colors.Green);
                            SaveButton.IsEnabled = true;
                        }
                    }
                    else if (tokens[0] == "AttackCol")
                    {
                        if (!validateNum(castToTextBox.Text))
                        {
                            // Error in your validation. Change something on UI and disable save button eventually
                            castToTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                            SaveBorder.Background = new SolidColorBrush(Colors.Red);
                            castToTextBox.BorderThickness = new Thickness(1);
                            ErrorLabel.Content = "Errors: Please correct your errors before saving.";
                            SaveButton.IsEnabled = false;
                        }
                        else
                        {
                            castToTextBox.BorderThickness = new Thickness(0);
                            ErrorLabel.Content = "Errors: No Errors";
                            SaveBorder.Background = new SolidColorBrush(Colors.Green);
                            SaveButton.IsEnabled = true;
                        }
                    }
                }
            }
        }
    }
}
