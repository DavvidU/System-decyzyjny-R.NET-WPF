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

namespace System_decyzyjny
{
    /// <summary>
    /// Logika interakcji dla klasy Backend.xaml
    /// </summary>
    public partial class Backend : Window
    {
        public Backend(string sciezka, int stepbystep)
        {
            InitializeComponent();
            Metoda();
        }
        public  void Metoda()
        {
            this.Show();
        }
    }
}
