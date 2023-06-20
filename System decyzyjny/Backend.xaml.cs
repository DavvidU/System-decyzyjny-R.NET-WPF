using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
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
using RDotNet;

namespace System_decyzyjny
{
    /// <summary>
    /// Logika interakcji dla klasy Backend.xaml
    /// </summary>
    /// 
    public partial class Backend : Window
    {
        //Load sample data
        //Load sample data
        REngine silnikR;
        string dane;
        int krokpokroku;

        public DataTable tablica_decyzyjna;

        public Backend(string sciezka, int stepbystep)
        {
            InitializeComponent();
            dane = sciezka.Replace("\\", "\\\\");

            krokpokroku = stepbystep;
            silnikR = REngine.GetInstance();

            //Obejscie problemu dla nowszych wersji srodowiska R
            //W przypadku innej sciezki do katalogu ze srodowiskiem R zastapic odpowiednio D:/Programy

            silnikR.Evaluate("Sys.setenv(PATH = paste(\"D:/Programy/R-4.3.0/bin/x64\", Sys.getenv(\"PATH\"), sep=\";\"))");
            
            silnikR.Evaluate("install.packages('remotes')");
            silnikR.Evaluate("remotes::install_github('janusza/RoughSets')");  // Instalacja pakietu RoughSets z repozytorium GitHub https://github.com/janusza/RoughSets
            silnikR.Evaluate("library(RoughSets)");

            // Odczytanie danych z pliku do data
            
            silnikR.Evaluate($"data <- read.table(\"{dane}\", header = TRUE)");

            /*
             * Wyswietlenie tablicy decyzyjnej
             */ 

            DataFrame data = silnikR.GetSymbol("data").AsDataFrame();
            WyswietlTabeleDanych wysTabDanych = new WyswietlTabeleDanych(data, this, "Wprowadzone dane");

            
        }
        public void StworzTabliceDecyzyjna(string ilosc_atrybutow)
        {
            silnikR.Evaluate($"tablica_decyzyjna=SF.read.DecisionTable(\"{dane}\",header=TRUE,sep=\",\",decision.attr={ilosc_atrybutow},NULL)");



            WyznaczRelacjeNieodroznialnosci();
        }
        public void WyznaczRelacjeNieodroznialnosci()
        {
            silnikR.Evaluate("relacja_nieodroznialnosci=BC.IND.relation.RST(tablica_decyzyjna,NULL)");

            /*
             * Wyswietlenie klas nieodroznialnosci
             */

            GenericVector relacja_nieodroznialnosci;
            GenericVector lista_klas_nieodroznialnosci;
            int[][] klasy_nieodroznialnosci = null;
            if (krokpokroku == 1)
            {
                relacja_nieodroznialnosci = silnikR.GetSymbol("relacja_nieodroznialnosci").AsList();
                lista_klas_nieodroznialnosci = relacja_nieodroznialnosci["IND.relation"].AsList();
                int i;
                for (i = 0; i < lista_klas_nieodroznialnosci.Length; ++i) { }
                klasy_nieodroznialnosci = new int[i][];
                for (i = 0; i < lista_klas_nieodroznialnosci.Length; ++i)
                {
                    klasy_nieodroznialnosci[i] = lista_klas_nieodroznialnosci[i].AsInteger().ToArray();
                }
            }

            WyswietlTabeleDanych wysKlasyNieod;
            if (krokpokroku == 1)
                wysKlasyNieod = new WyswietlTabeleDanych(klasy_nieodroznialnosci, this, "Klasy nieodroznialnosci");
            if (krokpokroku == 0)
                WyznaczAproksymacje();
        }
        public void WyznaczAproksymacje()
        {
            silnikR.Evaluate("aproksymacje=BC.LU.approximation.RST(tablica_decyzyjna,relacja_nieodroznialnosci)");

            /*
            * Wyswietlenie dolnych
            * i gornych aproksymacji
            */

            GenericVector lista_aproksymacji;
            GenericVector lista_dolnych_aproksymacji;
            GenericVector lista_gornych_aproksymacji;
            int[][] dolne_aproksymacje = null;
            int[][] gorne_aproksymacje = null;
            string[] nazwy_klas_aproksymacji = null;
            if (krokpokroku == 1)
            {
                lista_aproksymacji = silnikR.GetSymbol("aproksymacje").AsList();
                lista_dolnych_aproksymacji = lista_aproksymacji["lower.approximation"].AsList();
                lista_gornych_aproksymacji = lista_aproksymacji["upper.approximation"].AsList();
                nazwy_klas_aproksymacji = lista_dolnych_aproksymacji.Names;
                int i;
                // Pobieranie dolnych aproksymacji
                for (i = 0; i < lista_dolnych_aproksymacji.Length; ++i) { }
                dolne_aproksymacje = new int[i][];
                for (i = 0; i < lista_dolnych_aproksymacji.Length; ++i)
                {
                    dolne_aproksymacje[i] = lista_dolnych_aproksymacji[i].AsInteger().ToArray();
                }
                // Pobieranie gornych aproksymacji
                for (i = 0; i < lista_gornych_aproksymacji.Length; ++i) { }
                gorne_aproksymacje = new int[i][];
                for (i = 0; i < lista_gornych_aproksymacji.Length; ++i)
                {
                    gorne_aproksymacje[i] = lista_gornych_aproksymacji[i].AsInteger().ToArray();
                }

                
                //WyswietlTabeleDanych wysGorneAproks = new WyswietlTabeleDanych(gorneAproksymacjeList, this, "Gorne aproksymacje");
            }
            WyswietlTabeleDanych wysDolneAproks;
            if (krokpokroku == 1)
                wysDolneAproks = new WyswietlTabeleDanych(nazwy_klas_aproksymacji, dolne_aproksymacje, gorne_aproksymacje, this, "Dolne aproksymacje");
            if (krokpokroku == 0)
                WyznaczObszarPozytywny();
        }
        public void WyznaczObszarPozytywny()
        {
            // Wyznacz sume dolnych aproksymacji
            silnikR.Evaluate("obszar_pozytywny=BC.positive.reg.RST(tablica_decyzyjna,aproksymacje)");

            /*
            * Wyswietlenie obszaru pozytywnego
            */
            GenericVector obszar_pozytywny_lista;
            int[] elementy = null;
            obszar_pozytywny_lista = silnikR.GetSymbol("obszar_pozytywny").AsList();
            elementy = obszar_pozytywny_lista["positive.reg"].AsInteger().ToArray();
            WyswietlTabeleDanych wysObszPoz;
            if (krokpokroku == 1)
                wysObszPoz = new WyswietlTabeleDanych(elementy, this, "Obszar pozytywny");
            if (krokpokroku == 0)
                WyznaczObszarNegatywny(elementy);
        }
        public void WyznaczObszarNegatywny(int[] obszar_pozytywny)
        {
            //Wyznacz uniwersum pomniejszone o gorne aproksymacje
            silnikR.Evaluate("obszar_negatywny=BC.negative.reg.RST(tablica_decyzyjna,aproksymacje)");

            /*
            * Wyswietlenie obszaru negatywnego
            */
            GenericVector obszar_negatywny_lista;
            int[] elementy = null;
            obszar_negatywny_lista = silnikR.GetSymbol("obszar_negatywny").AsList();
            elementy = obszar_negatywny_lista["negative.reg"].AsInteger().ToArray();
            WyswietlTabeleDanych wysObszNeg;
            if (krokpokroku == 1)
                wysObszNeg = new WyswietlTabeleDanych(elementy, this, "Obszar negatywny");
            if (krokpokroku == 0)
                WyznaczZbiorWskazujacychRegulDecyzyjnych(obszar_pozytywny, elementy);
        }
        public void WyznaczZbiorWskazujacychRegulDecyzyjnych(int[] obszar_pozytywny, int[] obszar_negatywny)
        {
            //po jednej regule dla kazdzego obiektu z obszaru pozytywnego technika dropping conditions
            
            DataRow[] wiersze_obszaru_pozytywnego;
            
            // Z tablicy decyzyjnej wybiesz te wiersze, ktore naleza do obszar_pozytywny
            
            string filterExpression = $"Element IN ({string.Join(",", obszar_pozytywny)})";
            wiersze_obszaru_pozytywnego = tablica_decyzyjna.Select(filterExpression);
            
            // Nowa tabela na obszar pozytywny
            
            DataTable obszarPozyt = tablica_decyzyjna.Clone();
            foreach (DataRow row in wiersze_obszaru_pozytywnego)
            {
                obszarPozyt.ImportRow(row);
            }

            // Dla kazdego elementu wyznacz regule wskazujaca

            List<string> regulyWskazujace = new List<string>();
            int i;
            foreach (DataRow row in obszarPozyt.Rows)
            {
                string[] wartosci = row.ItemArray.Select(x => x.ToString()).ToArray();
                string klasa = wartosci[wartosci.Length - 1];

                List<string> warunki = new List<string>();

                for (i = 0; i < wartosci.Length - 1; i++)
                {
                    string nazwaKolumny = obszarPozyt.Columns[i].ColumnName;
                    string warunek = $"({nazwaKolumny},{wartosci[i]})";
                    warunki.Add(warunek);
                }

                string nazwaOstatniejKolumny = obszarPozyt.Columns[obszarPozyt.Columns.Count - 1].ColumnName;
                string regula = $"{string.Join(" ^ ", warunki)} -> ({nazwaOstatniejKolumny},{klasa})";
                regulyWskazujace.Add(regula);

                // Opuszczanie kolejnych warunków
                for (int j = 0; j < warunki.Count - 1; j++)
                {
                    List<string> warunkiOpuszczone = new List<string>(warunki);
                    warunkiOpuszczone.RemoveAt(j);

                    bool pokrywaInneKlasy = false;

                    foreach (DataRow otherRow in obszarPozyt.Rows)
                    {
                        string[] otherWartosci = otherRow.ItemArray.Select(x => x.ToString()).ToArray();
                        string otherKlasa = otherWartosci[otherWartosci.Length - 1];

                        if (otherKlasa != klasa)
                        {
                            bool pokrycie = true;

                            for (int k = 0; k < warunkiOpuszczone.Count; k++)
                            {
                                if (wartosci[k] != otherWartosci[k])
                                {
                                    pokrycie = false;
                                    break;
                                }
                            }

                            if (pokrycie)
                            {
                                pokrywaInneKlasy = true;
                                break;
                            }
                        }
                    }

                    if (!pokrywaInneKlasy)
                    {
                        string regulaOpuszczona = $"{string.Join(" ^ ", warunkiOpuszczone)} -> ({nazwaOstatniejKolumny},{klasa})";
                        regulyWskazujace.Add(regulaOpuszczona);
                    }
                }
            }

            
            tabela.ItemsSource = obszarPozyt.DefaultView;
            lista.ItemsSource = regulyWskazujace;
            this.Show();
            
        }
        public void WyznaczZbiorWykluczajacychRegulDecyzyjnych()
        {
            //po jednej regule dla kazdzego obiektu z obszaru negatywnegoo technika dropping conditions
        }
        public void UsunPowtarzajaceSieReguly()
        {
            //po prostu
        }
        public void UsunRegulyBedaceUszczegolowieniemInnychRegul()
        {
            //np regula a ^ b => c jest uczegolowieniem a => c
        }
        // za pomoca regul wykluczajacych wyeliminowac obiekty niepasujace? niepowiazane? (napewno nie nalezace?)
        
        // z tak otrzymanego zbioru potencjalnie pasujacych zawezic za pomoca regul wskazujacych
    }
}
