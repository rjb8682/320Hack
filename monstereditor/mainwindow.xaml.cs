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
                NameCol.Items.Add(monster.Name);
                SymbolCol.Items.Add(monster.Symbol);
                var convertFromString = ColorConverter.ConvertFromString(monster.Color);
                if (convertFromString != null)
                    ColorCol.Items.Add(new Border()
                    {
                        Background = new SolidColorBrush((Color)convertFromString),
                        Height = 15,
                        Width = Convert.ToDouble(MonsterGrid.ColumnDefinitions[2].ActualWidth) - 15,
                        Margin = new Thickness(0, 0, 0, 1.25),
                        BorderBrush = new SolidColorBrush(Colors.Gray),
                        BorderThickness = new Thickness(1)
                    });
                HealthCol.Items.Add(monster.HP);
                MinRoomCol.Items.Add(monster.MinimumRoom);
                SpeedCol.Items.Add(monster.Speed);
                AttackCol.Items.Add(monster.Attack);
            }
        }
    }
}
