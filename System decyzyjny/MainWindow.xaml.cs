using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace System_decyzyjny
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string wybranyPlik;
        private int stepbystep = 0;
        //Uri = new Uri("C\es.txt");
        public MainWindow()
        {
            InitializeComponent();
        }
        private void wybierz_click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Pliki tekstowe (*.txt)|*.txt";
            bool? result = ofd.ShowDialog();

            if (result == true)
            {
                wybranyPlik = ofd.FileName;
                sciezka.Text = wybranyPlik;
            }

        }
        private void wyznacz_click(Object sender, RoutedEventArgs e)
        {
            if(sciezka.Text == "Nie wybrano pliku")
            {
                MessageBox.Show("Proszę wybrać plik z danymi.", "Nie wybrano pliku", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if(!(bool)kontrola.IsChecked && !(bool)automatycznie.IsChecked)
            {
                MessageBox.Show("Proszę wybrać tryb prezentacji.", "Nie wybrano trybu", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if((bool)kontrola.IsChecked)
            {
                stepbystep = 1;
                //uruchomAlgorytm(wybranyPlik, stepbystep);
            }
            else if ((bool)automatycznie.IsChecked)
            {
                stepbystep = 0;
                //uruchomAlgorytm(wybranyPlik, stepbystep);
            }
        }
    }
}
