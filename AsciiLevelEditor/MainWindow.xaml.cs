using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AsciiLevelEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const int MaxRows = 26;
        private const int MaxCols = 93;

        private int _currentRow;
        private int _currentCol;

        private int _currentColor;
        private readonly List<Color> _colors = new List<Color>() { Colors.Blue, Colors.Red, Colors.Yellow, Colors.Gray, Colors.Black, Colors.LawnGreen, Colors.Orange, Colors.Purple };
        private readonly List<char> _tileChars = new List<char>() { '|', '—', '+', '·', ' ', '\n' };
        private const int FloorIndex = 3;

        public int CurrentRoom = -1;

        // Edit this collection and it will effect the view as well
        public List<List<Button>> ButtonsInGrid;

        public MainWindow()
        {
            InitializeComponent();

            ButtonsInGrid = new List<List<Button>>();
        }

        private void ButtonGrid_OnLoaded(object sender, RoutedEventArgs e)
        {
            for (var i = 0; i < MaxRows; i++)
            {
                ButtonsInGrid.Add(new List<Button>());
                for (var j = 0; j < MaxCols; j++)
                {
                    var rowDef = new RowDefinition
                    {
                        Height = new GridLength(Math.Floor(ButtonGrid.ActualHeight)/MaxRows)
                    };
                    ButtonGrid.RowDefinitions.Add(rowDef);

                    var colDef = new ColumnDefinition
                    {
                        Width = new GridLength(Math.Floor(ButtonGrid.ActualWidth)/MaxCols)
                    };
                    ButtonGrid.ColumnDefinitions.Add(colDef);

                    var newButton = new Button
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Background = new SolidColorBrush(Colors.Black)
                    };
                    newButton.Click += GridClick;
                    newButton.SetValue(ToolTipService.IsEnabledProperty, false);
                    newButton.ToolTip = i + "," + j;

                    Grid.SetRow(newButton, i);
                    Grid.SetColumn(newButton, j);
                    ButtonsInGrid[i].Add(newButton);
                    ButtonGrid.Children.Add(newButton);
                }
            }
        }

        private void ColorSelector(object sender, RoutedEventArgs e)
        {
            var selectedColor = sender as Button;
            if (selectedColor == null) return;
            var background = selectedColor.Background as SolidColorBrush;
            if (background != null) _currentColor = _colors.IndexOf(background.Color);
        }

        private void GridClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null) button.Background = new SolidColorBrush(_colors[_currentColor]);
        }

        private void SaveLevel(object sender, RoutedEventArgs e)
        {
            var db = new _320Hack.DbModel();

            if (CurrentRoom == -1)
            {
                // TODO: Implement the ability to add another level into the game 

            }
            else
            {
                var stairs = (from s in db.Stairs where s.LivesIn == CurrentRoom select s).ToList();
                foreach (var s in stairs)
                {
                    db.Stairs.Remove(s);
                }

                var theMap = "";
                for (var i = 0; i < MaxRows; i++)
                {
                    for (var j = 0; j < MaxCols; j++)
                    {
                        var solidColorBrush = ButtonsInGrid[i][j].Background as SolidColorBrush;
                        if (solidColorBrush == null) continue;
                        var colorIndex = _colors.IndexOf(solidColorBrush.Color);

                        if (solidColorBrush.Color == Colors.Orange || solidColorBrush.Color == Colors.Purple)
                        {
                            // Found a door on the map. Figure out which direction and go from there
                            theMap += _tileChars[FloorIndex];
                            continue;
                        }

                        var newChar = _tileChars[colorIndex];
                        if (newChar == '\n')
                        {
                            theMap += '\r';
                            theMap += '\n';
                            break;
                        }
                        else
                        {
                            theMap += newChar;
                        }
                    }
                }

                var room = (from r in db.Rooms where r.Id == CurrentRoom select r).Single();
                room.Map = theMap;
                db.SaveChanges();
            }
        }

        private void ImportLevel(object sender, RoutedEventArgs e)
        {
            var db = new _320Hack.DbModel();
            var rooms = (from r in db.Rooms select r).ToList();

            var dialog = new FileDialog(this, rooms);
            dialog.Show();
        }

        public void ImportFile(int roomId)
        {
            CurrentRoom = roomId;
            var db = new _320Hack.DbModel();
            var level = (from r in db.Rooms where r.Id == roomId select r).Single();
            level.buildLevelChars();

            ClearLevel(null, null);

            for (var i = 0; i < MaxRows; i++)
            {
                if (i == level.LevelChars.Count) break;
                for (var j = 0; j < MaxCols; j++)
                {
                    if (j == level.LevelChars[i].Count) break;
                    char currentChar = level.LevelChars[i][j];
                    ButtonsInGrid[i][j].Background = new SolidColorBrush(_colors[_tileChars.IndexOf(currentChar)]);
                    if (currentChar == '\n') break;
                }
            }
            var query = (from s in db.Stairs where s.LivesIn == roomId select s);
            var stairs = query.ToList();

            foreach (_320Hack.Stair s in stairs)
            {
                int stairIndex = (s.LivesIn < s.ConnectsTo) ? 0 : 1;
                ButtonsInGrid[s.Row][s.Col].Background = new SolidColorBrush(_colors[stairIndex + 6]);
            }
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            int code = (int) e.Key;

            switch (code) {
                case (int)Key.Up:
                    _currentRow =  _currentRow < 0 ? _currentRow = 0 : _currentRow--;
                    ButtonsInGrid[_currentRow][_currentCol].Focus();
                    break;
                case (int)Key.Down:
                    _currentRow = _currentRow == MaxRows - 1 ? _currentRow = MaxRows - 1 : _currentRow++;
                    ButtonsInGrid[_currentRow][_currentCol].Focus();
                    break;
                case (int)Key.Left:
                    _currentCol = _currentCol < 0 ? _currentCol = 0 : _currentCol--;
                    ButtonsInGrid[_currentRow][_currentCol].Focus();
                    break;
                case (int)Key.Right:
                    _currentCol = _currentCol == MaxCols - 1 ? _currentCol = MaxCols - 1 : _currentCol++;
                    ButtonsInGrid[_currentRow][_currentCol].Focus();
                    break;
                case (int)Key.I:
                    ImportLevel(null, null);
                    break;
                case (int)Key.S:
                    SaveLevel(null, null);
                    break;
                case (int)Key.C:
                    ClearLevel(null, null);
                    break;
                case 1:
                    _currentColor = 0;
                    break;
                case 2:
                    _currentColor = 1;
                    break;
                case 3:
                    _currentColor = 2;
                    break;
                case 4:
                    _currentColor = 3;
                    break;
                case 5:
                    _currentColor = 4;
                    break;
                case 6:
                    _currentColor = 5;
                    break;
            }
        }

        public void ClearLevel(object sender, RoutedEventArgs f)
        {
            CurrentRoom = -1;
            for (var i = 0; i < MaxRows; i++)
            {
                for (var j = 0; j < MaxCols; j++)
                {
                    ButtonsInGrid[i][j].Background = new SolidColorBrush(Colors.Black);
                }
            }
        }
    }
}
