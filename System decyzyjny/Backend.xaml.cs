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

            // Wyswietlenie wprowadzonych danych if(krokpokroku == 1)if(krokpokroku == 1)if(krokpokroku == 1)

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

            GenericVector lista_aproksymacji;
            GenericVector lista_dolnych_aproksymacji;
            GenericVector lista_gornych_aproksymacji;
            int[][] dolne_aproksymacje = null;
            int[][] gorne_aproksymacje = null;
            if (krokpokroku == 1)
            {
                lista_aproksymacji = silnikR.GetSymbol("aproksymacje").AsList();
                lista_dolnych_aproksymacji = lista_aproksymacji["lower.approximation"].AsList();
                lista_gornych_aproksymacji = lista_aproksymacji["upper.approximation"].AsList();

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
                wysDolneAproks = new WyswietlTabeleDanych(dolne_aproksymacje, gorne_aproksymacje, this, "Dolne aproksymacje");
            if (krokpokroku == 0)
                WyznaczObszarPozytywny();
        }
        public void WyznaczObszarPozytywny()
        {
            silnikR.Evaluate("obszar_pozytywny=BC.positive.reg.RST(tablica_decyzyjna,aproksymacje)");
            GenericVector obszar_pozytywny_lista = silnikR.GetSymbol("obszar_pozytywny").AsList();
            int[] indeksy = obszar_pozytywny_lista["positive.reg"].AsInteger().ToArray();
            for (int i = 0; i < indeksy.Length; ++i)
            {
                pozytywny.Text += indeksy[i].ToString();
            }


            WyznaczObszarNegatywny();
        }
        public void WyznaczObszarNegatywny()
        {
            //Wyznacz uniwersum pomniejszone o gorna aproksymacje
            silnikR.Evaluate("obszar_negatywny=BC.negative.reg.RST(tablica_decyzyjna,aproksymacje)");

            GenericVector obszar_pozytywny_lista = silnikR.GetSymbol("obszar_negatywny").AsList();
            int[] indeksy = obszar_pozytywny_lista["negative.reg"].AsInteger().ToArray();
            for (int i = 0; i < indeksy.Length; ++i)
            {
                negatywny.Text += indeksy[i].ToString();
            }
            this.Show();
        }
        public void WyznaczZbiorWskazujacychRegulDecyzyjnych()
        {
            //po jednej regule dla kazdzego obiektu z obszaru pozytywnego technika dropping conditions
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
