﻿using System;
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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class HelpMenu : Window
    {
        public HelpMenu()
        {
            InitializeComponent();
        }

        private void keyPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }
}
