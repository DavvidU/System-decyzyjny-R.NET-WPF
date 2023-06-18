using RDotNet;
using System;
using System.Collections.Generic;
using System.Data;
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
        DataFrame otrzymana_tabela;
        //DynamicVector[] wektor;
        DataTable przekonwertowana_tabela;
        public WyswietlTabeleDanych(DataFrame arg, Backend backend)
        {
            InitializeComponent();
            otrzymana_tabela = arg;
            przekonwertowana_tabela = new DataTable();

            //Przepisz kolumny

            string[] nazwy_kolumn = otrzymana_tabela.ColumnNames[0].Split('.');
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
        public void next_click(object sender, EventArgs e)
        {

        }
        public void save_click(object sender, EventArgs e)
        {

        }
    }
}
