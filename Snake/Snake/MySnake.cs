using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Shapes; //kształty
using System.Windows.Media;
using System.Windows.Controls;

namespace Snake
{
    class MySnake
    {
        //Tworze glowe na podstawie klasy SnakePart i pozwalam na ustawianie dla niej wartosci
        public SnakePart head { get; private set; }
        //Lista elementow z jakich sklada sie waz
        //Lista ma zawierac elementy weza wiec w nawiasach jako typ podaje klase
        public List<SnakePart> Parts { get; private set; }


        //Konstruktor - to co tworzymy podczas pierwszego uruchomienia
        public MySnake()
        {
            //umiejscowienie głowy
            head = new SnakePart(20,0);
            //kolor głowy
            head.Rect.Fill = Brushes.Red;
            //glowa weza to kwadrat o boku 20px
            head.Rect.Width = head.Rect.Height = 20;
            Parts = new List<SnakePart>();
            Parts.Add(new SnakePart(19, 0));
        }
        //Metoda Rysujaca weza
        public void Rysuj(){
            //rysuj w kolumnie na podstawie danych z rect i jego wspolrzednej X
            Grid.SetColumn(head.Rect, head.X);
            //rysuj w wierszu na podstawie danych z rect i jego wspolrzednej Y
            Grid.SetRow(head.Rect, head.Y);

            foreach (SnakePart snakePart in Parts)
            {
                //rysuj w kolumnie na podstawie danych z rect i jego wspolrzednej X
                Grid.SetColumn(snakePart.Rect, snakePart.X);
                //rysuj w wierszu na podstawie danych z rect i jego wspolrzednej Y
                Grid.SetRow(snakePart.Rect, snakePart.Y);
            }
        }

        
    }
}
