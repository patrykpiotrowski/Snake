using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes; //kształty
using System.Windows.Media;

namespace Snake
{
    class SnakePart
    {
        //współrzedne odpowiedniej części
        public int X { get; set; }
        public int Y { get; set; }
        //
        public Rectangle Rect { get; private set; }

        //konstruktor
        public SnakePart(int x, int y)
        {
            X = x;
            Y = y;
            //nowy prostokąt
            Rect = new Rectangle();
            //kwadrat o boku 18px
            Rect.Width = Rect.Height = 18;
            //waz sklada sie z kwadratow koloru czarnego
            Rect.Fill = Brushes.Black;
        }
    }
}
