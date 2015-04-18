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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AsciiLevelEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int MAX_ROWS = 26;
        private int MAX_COLS = 93;

        private int currentlyFocusedRow = 0;
        private int currentlyFocusedCol = 0;

        private int currentlySelectedColor = 0;
        private List<Color> colors = new List<Color>()  { Colors.Blue, Colors.Red, Colors.Yellow, Colors.Gray, Colors.Black, Colors.LawnGreen };
        private List<char> tileChars = new List<char>() { '|', '—', '+', '·', ' ', '\n' };

        public int currentlySelectedRoom = -1;

        // Edit this collection and it will effect the view as well
        public List<List<Button>> buttonsInGrid;

        public MainWindow()
        {
            InitializeComponent();

            buttonsInGrid = new List<List<Button>>();
        }

        private void ButtonGrid_OnLoaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < MAX_ROWS; i++)
            {
                buttonsInGrid.Add(new List<Button>());
                for (int j = 0; j < MAX_COLS; j++)
                {
                    RowDefinition rowDef = new RowDefinition();
                    rowDef.Height = new GridLength(Math.Floor(buttonGrid.ActualHeight) / MAX_ROWS);
                    buttonGrid.RowDefinitions.Add(rowDef);

                    ColumnDefinition colDef = new ColumnDefinition();
                    colDef.Width = new GridLength(Math.Floor(buttonGrid.ActualWidth) / MAX_COLS);
                    buttonGrid.ColumnDefinitions.Add(colDef);

                    Button newButton = new Button();
                    newButton.HorizontalAlignment = HorizontalAlignment.Stretch;
                    newButton.VerticalAlignment = VerticalAlignment.Stretch;
                    newButton.Background = new SolidColorBrush(Colors.Black);
                    newButton.Click += GridClick;
                    newButton.SetValue(ToolTipService.IsEnabledProperty, false);
                    newButton.ToolTip = i + "," + j;

                    Grid.SetRow(newButton, i);
                    Grid.SetColumn(newButton, j);
                    buttonsInGrid[i].Add(newButton);
                    buttonGrid.Children.Add(newButton);
                }
            }
        }

        private void colorSelector(object sender, RoutedEventArgs e)
        {
            Button selectedColor = sender as Button;
            currentlySelectedColor = colors.IndexOf((selectedColor.Background as SolidColorBrush).Color);
        }

        private void GridClick(object sender, RoutedEventArgs e)
        {
            (sender as Button).Background = new SolidColorBrush(colors[currentlySelectedColor]);
        }

        private void saveLevel(object sender, RoutedEventArgs e)
        {
            if (currentlySelectedRoom == -1)
            {
                // No room imported

            }
            else
            {
                String theMap = "";
                for (int i = 0; i < MAX_ROWS; i++)
                {
                    for (int j = 0; j < MAX_COLS; j++)
                    {
                        char newChar = tileChars[colors.IndexOf((buttonsInGrid[i][j].Background as SolidColorBrush).Color)];
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


                using (var db = new _320Hack.DbModel())
                {
                    _320Hack.Room room = (from r in db.Rooms where r.Id == currentlySelectedRoom select r).Single();
                    room.Map = theMap;
                    db.SaveChanges();
                }
            }
        }

        private void importLevel(object sender, RoutedEventArgs e)
        {
            var db = new _320Hack.DbModel();
            List<_320Hack.Room> rooms = (from r in db.Rooms select r).ToList();

            FileDialog dialog = new FileDialog(this, "Import", rooms);
            dialog.Show();
        }

        public void importFile(int roomId)
        {
            currentlySelectedRoom = roomId;
            var db = new _320Hack.DbModel();
            _320Hack.Room level = (from r in db.Rooms where r.Id == roomId select r).Single();
            level.buildLevelChars();

            clearLevel(null, null);

            for (int i = 0; i < MAX_ROWS; i++)
            {
                if (i == level.LevelChars.Count) break;
                for (int j = 0; j < MAX_COLS; j++)
                {
                    if (j == level.LevelChars[i].Count) break;
                    char currentChar = level.LevelChars[i][j];
                    buttonsInGrid[i][j].Background = new SolidColorBrush(colors[tileChars.IndexOf(currentChar)]);
                    if (currentChar == '\n') break;
                }
            }
        }

        private void keyPressed(object sender, KeyEventArgs e)
        {
            int code = (int) e.Key;

            switch (code) {
                case (int)Key.Up:
                    currentlyFocusedRow =  currentlyFocusedRow < 0 ? currentlyFocusedRow = 0 : currentlyFocusedRow--;
                    buttonsInGrid[currentlyFocusedRow][currentlyFocusedCol].Focus();
                    break;
                case (int)Key.Down:
                    currentlyFocusedRow = currentlyFocusedRow == MAX_ROWS - 1 ? currentlyFocusedRow = MAX_ROWS - 1 : currentlyFocusedRow++;
                    buttonsInGrid[currentlyFocusedRow][currentlyFocusedCol].Focus();
                    break;
                case (int)Key.Left:
                    currentlyFocusedCol = currentlyFocusedCol < 0 ? currentlyFocusedCol = 0 : currentlyFocusedCol--;
                    buttonsInGrid[currentlyFocusedRow][currentlyFocusedCol].Focus();
                    break;
                case (int)Key.Right:
                    currentlyFocusedCol = currentlyFocusedCol == MAX_COLS - 1 ? currentlyFocusedCol = MAX_COLS - 1 : currentlyFocusedCol++;
                    buttonsInGrid[currentlyFocusedRow][currentlyFocusedCol].Focus();
                    break;
                case (int)Key.I:
                    importLevel(null, null);
                    break;
                case (int)Key.S:
                    saveLevel(null, null);
                    break;
                case (int)Key.C:
                    clearLevel(null, null);
                    break;
                case 1:
                    currentlySelectedColor = 0;
                    break;
                case 2:
                    currentlySelectedColor = 1;
                    break;
                case 3:
                    currentlySelectedColor = 2;
                    break;
                case 4:
                    currentlySelectedColor = 3;
                    break;
                case 5:
                    currentlySelectedColor = 4;
                    break;
                case 6:
                    currentlySelectedColor = 5;
                    break;
            }
        }

        public void clearLevel(object sender, RoutedEventArgs f)
        {
            for (int i = 0; i < MAX_ROWS; i++)
            {
                for (int j = 0; j < MAX_COLS; j++)
                {
                    buttonsInGrid[i][j].Background = new SolidColorBrush(Colors.Black);
                }
            }
        }
    }
}
