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
        private Color[] colors = { Colors.Blue, Colors.Red, Colors.Yellow, Colors.Gray, Colors.Black, Colors.LawnGreen };

        // Edit this collection and it will effect the view as well
        public List<List<Button>> buttonsInGrid;

        public MainWindow()
        {
            InitializeComponent();

            buttonsInGrid = new List<List<Button>>();
        }

        private UIElement getButton() { return buttonGrid.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == currentlyFocusedRow && Grid.GetColumn(e) == currentlyFocusedCol); }
        private UIElement getButton(int row, int col) { return buttonGrid.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == col); }

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
            getButton().Focus();
        }

        private void GridClick(object sender, RoutedEventArgs e)
        {
            (sender as Button).Background = new SolidColorBrush(colors[currentlySelectedColor]);
        }

        private void exportLevel(object sender, RoutedEventArgs e)
        {
            FileDialog dialog = new FileDialog(this, "Export");
            dialog.Show();
        }

        public void exportFile(String file)
        {
            // Write the file to database
        }

        private void importLevel(object sender, RoutedEventArgs e)
        {
            FileDialog dialog = new FileDialog(this, "Import");
            dialog.Show();
        }

        public void importFile(String file)
        {
            // Read a file from the database
            var db = new _320Hack.DbModel();
            List<_320Hack.Room> rooms = (from r in db.Rooms select r).ToList();
            
        }

        private void keyPressed(object sender, KeyEventArgs e)
        {
            int code = (int) e.Key;

            switch (code) {
                case (int)Key.Up:
                    currentlyFocusedRow =  currentlyFocusedRow < 0 ? currentlyFocusedRow = 0 : currentlyFocusedRow--;
                    getButton().Focus();
                    break;
                case (int)Key.Down:
                    currentlyFocusedRow = currentlyFocusedRow == MAX_ROWS - 1 ? currentlyFocusedRow = MAX_ROWS - 1 : currentlyFocusedRow++;
                    getButton().Focus();
                    break;
                case (int)Key.Left:
                    currentlyFocusedCol = currentlyFocusedCol < 0 ? currentlyFocusedCol = 0 : currentlyFocusedCol--;
                    getButton().Focus();
                    break;
                case (int)Key.Right:
                    currentlyFocusedCol = currentlyFocusedCol == MAX_COLS - 1 ? currentlyFocusedCol = MAX_COLS - 1 : currentlyFocusedCol++;
                    getButton().Focus();
                    break;
                case (int)Key.I:
                    importLevel(null, null);
                    break;
                case (int)Key.E:
                    exportLevel(null, null);
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
