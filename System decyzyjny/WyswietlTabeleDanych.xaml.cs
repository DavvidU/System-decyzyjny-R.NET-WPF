using RDotNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection.Emit;
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
    /// Logika interakcji dla klasy WyswietlTabeleDanych.xaml
    /// </summary>
    public partial class WyswietlTabeleDanych : Window
    {
        Backend backend;

        DataFrame otrzymana_tabela;
        string ilosc_atrybutow;
        string co_wywolano;
        DataTable przekonwertowana_tabela;
        DataTable klasy_abstrakcji;
        int[][] gorne_aproks;
        string[] nazwy_wszystkich_klas_aproksymacji;
        int[] globalny_obszar_pozytywny;
        int[] globalny_obszar_negatywny;

        //Konstruktor dla wyswietlania tablicy decyzyjnej
        public WyswietlTabeleDanych(DataFrame arg, Backend backend, string tytul)
        {
            InitializeComponent();
            Title = tytul;
            co_wywolano = tytul;
            otrzymana_tabela = arg;
            this.backend = backend;
            przekonwertowana_tabela = new DataTable();

            //Przepisz kolumny

            string[] nazwy_kolumn = otrzymana_tabela.ColumnNames[0].Split('.');
            ilosc_atrybutow = nazwy_kolumn.Length.ToString();
            przekonwertowana_tabela.Columns.Add("Element");
            foreach (var kolumna in nazwy_kolumn)
            {
                przekonwertowana_tabela.Columns.Add(kolumna);
            }

            //Przepisz wiersze

            for(int i = 0; i < otrzymana_tabela.RowCount; ++i)
            {
                DataRow dr = przekonwertowana_tabela.NewRow();
                string wiersz = otrzymana_tabela.GetRow(i)[0].ToString();
                string[] zawartosc_komorek = wiersz.Split(',');
                for (int j = 0; j < zawartosc_komorek.Length + 1; ++j)
                {
                    if (j == 0)
                        dr[j] = i + 1;
                    if (j > 0)
                        dr[j] = zawartosc_komorek[j - 1];
                }
                przekonwertowana_tabela.Rows.Add(dr);
            }
            backend.tablica_decyzyjna = przekonwertowana_tabela;
            tabela.ItemsSource = przekonwertowana_tabela.DefaultView;
            this.Show();
        }
        //Konstruktor dla wyswietlania klas nieodroznialnosci
        public WyswietlTabeleDanych(int[][] arg, Backend backend, string tytul)
        {
            InitializeComponent();
            Title = tytul;
            this.backend = backend;
            co_wywolano = tytul;

            klasy_abstrakcji = new DataTable();

            // Dodaj kolumnę dla nazwy wiersza jako pierwszą kolumnę
            DataColumn nazwaWierszaColumn = klasy_abstrakcji.Columns.Add("Klasa nieodroznialnosci");
            nazwaWierszaColumn.SetOrdinal(0);

            // Dodaj pozostałe kolumny do DataTable
            int maxColumnCount = arg.Max(row => row.Length);
            for (int i = 0; i < maxColumnCount; i++)
            {
                klasy_abstrakcji.Columns.Add($"Element {i+1}");
            }

            // Dodaj wiersze do DataTable
            for (int i = 0; i < arg.Length; i++)
            {
                DataRow dataRow = klasy_abstrakcji.Rows.Add();
                for (int j = 0; j < arg[i].Length; j++)
                {
                    dataRow[j + 1] = arg[i][j]; // Przesunięcie o 1, aby uwzględnić pierwszą kolumnę "NazwaWiersza"
                }

                // Ustaw nazwę wiersza
                if (arg[i].Length > 0)
                {
                    klasy_abstrakcji.Rows[i]["Klasa nieodroznialnosci"] = $"[{arg[i][0]}]B";
                }
                else
                {
                    klasy_abstrakcji.Rows[i]["Klasa nieodroznialnosci"] = "[Empty]B";
                }
            }

            tabela.HeadersVisibility = DataGridHeadersVisibility.All;
            tabela.ItemsSource = klasy_abstrakcji.DefaultView;
            this.Show();
        }
        //Konstruktor dla wyswietlania dolnych aproksymacji
        public WyswietlTabeleDanych(string[] nazwy_klas_aproksymacji, int[][] arg1, int[][] arg2, Backend backend, string tytul)
        {
            InitializeComponent();
            Title = tytul;
            this.backend = backend;
            co_wywolano = tytul;
            gorne_aproks = arg2;
            nazwy_wszystkich_klas_aproksymacji = nazwy_klas_aproksymacji;

            DataTable dolne_aproksymacje = new DataTable();

            // Dodaj kolumnę dla nazwy wiersza jako pierwszą kolumnę
            DataColumn nazwaWierszaColumn = dolne_aproksymacje.Columns.Add("Klasa");
            nazwaWierszaColumn.SetOrdinal(0);

            // Dodaj pozostałe kolumny do DataTable
            int maxColumnCount = arg1.Max(row => row.Length);
            for (int i = 0; i < maxColumnCount; i++)
            {
                dolne_aproksymacje.Columns.Add($"Element {i + 1}");
            }

            // Dodaj wiersze do DataTable
            for (int i = 0; i < arg1.Length; i++)
            {
                DataRow dataRow = dolne_aproksymacje.Rows.Add();
                for (int j = 0; j < arg1[i].Length; j++)
                {
                    dataRow[j + 1] = arg1[i][j]; // Przesunięcie o 1, aby uwzględnić pierwszą kolumnę "NazwaWiersza"
                }

                // Ustaw nazwę wiersza
                if (arg1[i].Length > 0)
                {
                    dolne_aproksymacje.Rows[i]["Klasa"] = $"{nazwy_klas_aproksymacji[i]}";
                }
                else
                {
                    dolne_aproksymacje.Rows[i]["Klasa"] = "[Empty]B";
                }
            }

            tabela.HeadersVisibility = DataGridHeadersVisibility.All;
            tabela.ItemsSource = dolne_aproksymacje.DefaultView;
            this.Show();
        }
        //Konstruktor dla wyswietlania gornych aproksymacji
        public WyswietlTabeleDanych(string[] nazwy_klas_aproksymacji, int[][] arg, Backend backend, string tytul, int x)
        {
            InitializeComponent();
            Title = tytul;
            this.backend = backend;
            co_wywolano = tytul;

            DataTable gorne_aproksymacje = new DataTable();

            // Dodaj kolumnę dla nazwy wiersza jako pierwszą kolumnę
            DataColumn nazwaWierszaColumn = gorne_aproksymacje.Columns.Add("Klasa");
            nazwaWierszaColumn.SetOrdinal(0);

            // Dodaj pozostałe kolumny do DataTable
            int maxColumnCount = arg.Max(row => row.Length);
            for (int i = 0; i < maxColumnCount; i++)
            {
                gorne_aproksymacje.Columns.Add($"Element {i + 1}");
            }

            // Dodaj wiersze do DataTable
            for (int i = 0; i < arg.Length; i++)
            {
                DataRow dataRow = gorne_aproksymacje.Rows.Add();
                for (int j = 0; j < arg[i].Length; j++)
                {
                    dataRow[j + 1] = arg[i][j]; // Przesunięcie o 1, aby uwzględnić pierwszą kolumnę "NazwaWiersza"
                }

                // Ustaw nazwę wiersza
                if (arg[i].Length > 0)
                {
                    gorne_aproksymacje.Rows[i]["Klasa"] = $"{nazwy_klas_aproksymacji[i]}";
                }
                else
                {
                    gorne_aproksymacje.Rows[i]["Klasa"] = "[Empty]";
                }
            }

            tabela.HeadersVisibility = DataGridHeadersVisibility.All;
            tabela.ItemsSource = gorne_aproksymacje.DefaultView;
            this.Show();
        }
        //Konstruktor dla wyswietlania obszaru pozytywnego i negatywnego
        public WyswietlTabeleDanych(int[] arg, Backend backend, string tytul)
        {
            InitializeComponent();
            Title = tytul;
            this.backend = backend;
            co_wywolano = tytul;
            if (tytul == "Obszar pozytywny")
                globalny_obszar_pozytywny = arg;
            if (tytul == "Obszar negatywny")
                globalny_obszar_negatywny = arg;

            DataTable obszar = new DataTable();

            // Dodaj pozostałe kolumny do DataTable
            for (int i = 0; i < arg.Length; ++i)
            {
                obszar.Columns.Add($"Element {i + 1}");
            }
            if(arg.Length == 0)
                obszar.Columns.Add($"Obszar pusty");

            // Dodaj wiersze do DataTable
            DataRow dataRow = obszar.Rows.Add();
            for(int i = 0; i < arg.Length; ++i)
            {
                dataRow[i] = arg[i];
            }
            if (arg.Length == 0)
                dataRow[0] = "Brak elementow";

            
            tabela.ItemsSource = obszar.DefaultView;
            this.Show();
        }
        public void next_click(object sender, EventArgs e)
        {
            if (co_wywolano == "Wprowadzone dane")
            {
                nextPrzycisk.IsEnabled = false;
                backend.StworzTabliceDecyzyjna(ilosc_atrybutow);
            }
            if (co_wywolano == "Klasy nieodroznialnosci")
            {
                nextPrzycisk.IsEnabled = false;
                backend.WyznaczAproksymacje();
            }
            if (co_wywolano == "Dolne aproksymacje")
            {
                nextPrzycisk.IsEnabled = false;
                WyswietlTabeleDanych wysGorneAproks = new WyswietlTabeleDanych(nazwy_wszystkich_klas_aproksymacji, gorne_aproks, backend, "Gorne aproksymacje", 1);
            }
            if (co_wywolano == "Gorne aproksymacje")
            {
                nextPrzycisk.IsEnabled = false;
                backend.WyznaczObszarPozytywny();
            }
            if (co_wywolano == "Obszar pozytywny")
            {
                nextPrzycisk.IsEnabled = false;
                backend.WyznaczObszarNegatywny(globalny_obszar_pozytywny);
            }
            if (co_wywolano == "Obszar negatywny")
            {
                nextPrzycisk.IsEnabled = false;
                backend.WyznaczZbiorWskazujacychRegulDecyzyjnych(globalny_obszar_pozytywny, globalny_obszar_negatywny);
            }
        }
        public void save_click(object sender, EventArgs e)
        {

        }
    }
}
