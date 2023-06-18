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

            silnikR.Evaluate("Sys.setenv(PATH = paste(\"D:/Programy/R-4.3.0/bin/x64\", Sys.getenv(\"PATH\"), sep=\";\"))");
            
            silnikR.Evaluate("install.packages('remotes')");
            silnikR.Evaluate("remotes::install_github('janusza/RoughSets')");  // Instalacja pakietu RoughSets z repozytorium GitHub https://github.com/janusza/RoughSets
            silnikR.Evaluate("library(RoughSets)");
            
            silnikR.Evaluate($"tablica_decyzyjna=SF.read.DecisionTable(\"{dane}\",header=TRUE,sep=\",\")");
            silnikR.Evaluate($"data <- read.table(\"{dane}\", header = TRUE)");
            DataFrame data = silnikR.GetSymbol("data").AsDataFrame();
            wynik.Text = (string)data[0, 0];
            //DataTable dataTable = new DataTable();


            //SymbolicExpression rules = silnikR.GetSymbol("data");
            silnikR.Evaluate("positiveRegion <-  BC.positive.reg.RST(data, BC.LU.approximation.RST(dt,ind)");
            

            
            
            
            
            
            
            this.Show();
        }
        public void OdczytajPlik()
        {
            silnikR.Evaluate("negativeRegion <- NegativeRegion(data)");
            silnikR.Evaluate("decisionRules <- GenerateRules(positiveRegion, negativeRegion)");
        }
        public void WyznaczObszarPozytywny()
        {
            //wylicz dolna aproksymacje
            //zapisz dane tu
            //if(stepbystep == 1)
            //wypisz progres algorytmu
            Wynik wynik = new Wynik(this);
        }
        public void WyznaczGornaAproksymacje()
        {
            //wyznacz gorna aproksymacje
        }
        public void WyznaczObszarNegatywny()
        {
            //Wyznacz uniwersum pomniejszone o gorna aproksymacje
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
