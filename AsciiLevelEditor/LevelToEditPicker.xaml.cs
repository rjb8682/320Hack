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

namespace AsciiLevelEditor
{
    /// <summary>
    /// Interaction logic for LevelToEditPicker.xaml
    /// </summary>
    public partial class LevelToEditPicker : Window
    {
        public LevelToEditPicker()
        {
            InitializeComponent();

            var db = new _320Hack.DbModel();

            foreach (_320Hack.Room room in (from r in db.Rooms select r).ToList())
            {
                MenuItem roomItem = new MenuItem();
                roomItem.Header = room.Id.ToString();
                Level.Items.Add(roomItem);
            }
        }
    }
}
