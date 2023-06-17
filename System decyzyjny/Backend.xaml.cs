using System;
using System.Collections.Generic;
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
        }
        public void OdczytajPlik()
        {

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
