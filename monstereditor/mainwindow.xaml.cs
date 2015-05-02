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
            NameCol.Children.Add(nameTextBox);
            Grid.SetRow(nameTextBox, NameCol.Children.Count - 1);

            rowDef = new RowDefinition();
            rowDef.Height = new GridLength(rowHeight);
            SymbolCol.RowDefinitions.Add(rowDef);
            TextBox symbolTextBox = new TextBox() { Text = monster.Symbol };
            SymbolCol.Children.Add(symbolTextBox);
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
            HealthCol.Children.Add(healthTextBox);
            Grid.SetRow(healthTextBox, HealthCol.Children.Count - 1);

            rowDef = new RowDefinition();
            rowDef.Height = new GridLength(rowHeight);
            MinRoomCol.RowDefinitions.Add(rowDef);
            TextBox minRoomTextBox = new TextBox() { Text = monster.MinimumRoom.ToString() };
            MinRoomCol.Children.Add(minRoomTextBox);
            Grid.SetRow(minRoomTextBox, MinRoomCol.Children.Count - 1);

            rowDef = new RowDefinition();
            rowDef.Height = new GridLength(rowHeight);
            SpeedCol.RowDefinitions.Add(rowDef);
            TextBox speedTextBox = new TextBox() { Text = monster.Speed.ToString() };
            SpeedCol.Children.Add(speedTextBox);
            Grid.SetRow(speedTextBox, SpeedCol.Children.Count - 1);

            rowDef = new RowDefinition();
            rowDef.Height = new GridLength(rowHeight);
            AttackCol.RowDefinitions.Add(rowDef);
            TextBox attackTextBox = new TextBox() { Text = monster.Attack.ToString() };
            AttackCol.Children.Add(attackTextBox);
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
    }
}
