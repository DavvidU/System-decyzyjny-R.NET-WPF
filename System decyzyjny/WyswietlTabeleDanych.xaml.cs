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
            foreach(var kolumna in nazwy_kolumn)
            {
                przekonwertowana_tabela.Columns.Add(kolumna);
            }

            //Przepisz wiersze

            for(int i = 0; i < otrzymana_tabela.RowCount; ++i)
            {
                DataRow dr = przekonwertowana_tabela.NewRow();
                string wiersz = otrzymana_tabela.GetRow(i)[0].ToString();
                string[] zawartosc_komorek = wiersz.Split(',');
                for (int j = 0; j < zawartosc_komorek.Length; ++j)
                {
                    dr[j] = zawartosc_komorek[j];
                }
                przekonwertowana_tabela.Rows.Add(dr);
            }
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
        public WyswietlTabeleDanych(int[][] arg1, int[][] arg2, Backend backend, string tytul)
        {
            InitializeComponent();
            Title = tytul;
            this.backend = backend;
            co_wywolano = tytul;
            gorne_aproks = arg2;

            DataTable dolne_aproksymacje = new DataTable();

            // Dodaj kolumnę dla nazwy wiersza jako pierwszą kolumnę
            DataColumn nazwaWierszaColumn = dolne_aproksymacje.Columns.Add("Klasa nieodroznialnosci");
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
                    dolne_aproksymacje.Rows[i]["Klasa nieodroznialnosci"] = $"[{arg1[i][0]}]B";
                }
                else
                {
                    dolne_aproksymacje.Rows[i]["Klasa nieodroznialnosci"] = "[Empty]B";
                }
            }

            tabela.HeadersVisibility = DataGridHeadersVisibility.All;
            tabela.ItemsSource = dolne_aproksymacje.DefaultView;
            this.Show();
        }
        //Konstruktor dla wyswietlania gornych aproksymacji
        public WyswietlTabeleDanych(int[][] arg, Backend backend, string tytul, int x)
        {
            InitializeComponent();
            Title = tytul;
            this.backend = backend;
            co_wywolano = tytul;

            DataTable gorne_aproksymacje = new DataTable();

            // Dodaj kolumnę dla nazwy wiersza jako pierwszą kolumnę
            DataColumn nazwaWierszaColumn = gorne_aproksymacje.Columns.Add("Klasa nieodroznialnosci");
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
                    gorne_aproksymacje.Rows[i]["Klasa nieodroznialnosci"] = $"[{arg[i][0]}]B";
                }
                else
                {
                    gorne_aproksymacje.Rows[i]["Klasa nieodroznialnosci"] = "[Empty]B";
                }
            }

            tabela.HeadersVisibility = DataGridHeadersVisibility.All;
            tabela.ItemsSource = gorne_aproksymacje.DefaultView;
            this.Show();
        }
        public void next_click(object sender, EventArgs e)
        {
            if (co_wywolano == "Wprowadzone dane")
            {
                backend.StworzTabliceDecyzyjna(ilosc_atrybutow);
                nextPrzycisk.IsEnabled = false;
            }
            if (co_wywolano == "Klasy nieodroznialnosci")
            {
                backend.WyznaczAproksymacje();
                this.Close();
            }
            if (co_wywolano == "Dolne aproksymacje")
            {
                WyswietlTabeleDanych wysGorneAproks = new WyswietlTabeleDanych(gorne_aproks, backend, "Gorne aproksymacje", 1);
                this.Close();
            }
            if (co_wywolano == "Gorne aproksymacje")
            {
                backend.WyznaczObszarPozytywny();
                this.Close();
            }
            
        }
        public void save_click(object sender, EventArgs e)
        {

        }
    }
}
