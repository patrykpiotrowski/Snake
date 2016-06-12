using System;
using System.Collections.Generic;
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
using System.Drawing;


//do dispatcher timer
using System.Windows.Threading;

namespace Snake
{

    //interfejs do pokazywania Menu
    interface IMenu
    {
        string PokazMenu();
    }

    class Pokaz : IMenu
    {
        public string PokazMenu()
        {
            string LabelSnake = "SNAKE";
            return LabelSnake;
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //wielkość pojedynczego pola planszy do gry
        private static readonly int SIZE = 20;
        //Referencja do MySnake
        private MySnake _snake;
        //na poczatku ruch jest w prawo
        private int _directionX = 1;
        //na poczatku ruch jest w poziomie
        private int _directionY = 0;
        //Timer zmieniajacy WPF - wiec dispatcher timer
        private DispatcherTimer _timer;
        //jedzenie dla weza
        private SnakePart _food;
        //Ilosc czesci do dodania - zaleznie od poziomu trudnosci
        private int _partsToAdd;
        //zmienna przechowujaca liczbe zdobytych punktow
        private int Points;
        //zmienna przechowujaca wybrany poziom trudnosci
        public int Mode = 2;
        //Dostepne poziomy trudnosci
        public enum PoziomTrudnosci : int
        {
            //liczba punktow i czesci weza przyznawana za jedzenie
            Easy = 2, //0
            Medium = 4, //1
            Hard = 6, //2
            Extreme = 10, //3
        }

        public MainWindow()
        {
            InitializeComponent();
            Pokaz PokazLogo = new Pokaz();
            LabelSnake.Content = PokazLogo.PokazMenu();

        }

        //inicjalizacja pola do gry
        void InitBoard()
        {
            
            for (int i = 0; i < grid.Width/SIZE; i++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                columnDefinition.Width = new GridLength(SIZE);
                grid.ColumnDefinitions.Add(columnDefinition);
            }
            for (int i = 0; i < grid.Height/20 - 3; i++)
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(SIZE);
                grid.RowDefinitions.Add(rowDefinition);
            }
            //nowy waz
            _snake = new MySnake();
        }

            //inicjalizacja weza
            void InitSnake(){
                //do siatki dodaje glowe
                grid.Children.Add(_snake.head.Rect);
                foreach (SnakePart snakepart in _snake.Parts)
	            {
                    grid.Children.Add(snakepart.Rect);
                    _snake.Rysuj();
	            }
            }

            void RuchWeza()
            {
                int snakePartCount = _snake.Parts.Count;
                if (_partsToAdd > 0)
                {
                    SnakePart newPart = new SnakePart(_snake.Parts[_snake.Parts.Count - 1].X, _snake.Parts[_snake.Parts.Count - 1].Y);
                    grid.Children.Add(newPart.Rect);
                    _snake.Parts.Add(newPart);
                    _partsToAdd --;
                }
                
 
                //zaczynamy od liczby elementow pomniejszonej o 1;
                //petla konczy sie na 1 elemencie;
                //stopniowo przechodzimy przez kazdy element;
                for (int i = _snake.Parts.Count - 1; i >= 1; i--)
                {
                    _snake.Parts[i].X = _snake.Parts[i - 1].X;
                    _snake.Parts[i].Y = _snake.Parts[i - 1].Y;
                }
                //pierwszy element ciala weza przyjmuje wspolrzedne glowy w poprzedniej chwili
                _snake.Parts[0].X = _snake.head.X;
                _snake.Parts[0].Y = _snake.head.Y;
                //glowa zyskuje ustawiony kierunek
                _snake.head.X += _directionX;
                _snake.head.Y += _directionY;
                //Czy nastapila kolizja
                if (CheckCollision()){
                    KoniecGry();
                }
                
                else
                {
                    //Jeśli wąż znalazł pożywienie to rysuj kolejne jedzenie
                    if (CzyJedzenie())
                    {
                        //Dodaj ilosc punktow zgodnie z wybrany,m poziomem trudnosci
                        Points+=Mode;
                        //Wyswietl aktualna liczbe punktow
                        LabelPoints.Content = Convert.ToString(Points);
                        RysujJedzenie();
                    }
                    //Po dokonaniu zmian narysuj weza
                    _snake.Rysuj();

                }
            }

            //inicjalizacja timera
            void InitTimer()
            {
                //tworze nowy dispatcher timer
                _timer = new DispatcherTimer();
                //Częstotliwość przerwań jest obliczana na podstawie wybranego poziomu trudności
                //y = -18x + 200
                _timer.Interval = new TimeSpan(0,0,0,0,(-18*Mode)+200);
                //dodanie metody Tick do Dispatcher timer
                _timer.Tick += new EventHandler(_timer_Tick);
                _timer.Start();
            }
            
            //kod wykonywany poprzerwaniu
            void _timer_Tick(object sender, EventArgs e)
            {
                RuchWeza();
            }

            private void Window_KeyDown(object sender, KeyEventArgs e)
            {
                if ((e.Key == Key.F1 || e.Key == Key.F2 || e.Key == Key.F3 || e.Key == Key.F12) && Menu.Visibility == Visibility.Visible)
                {
                    LabelMode.Content = Convert.ToString(PoziomTrudnosci.Easy);
                    //domyslnie ustawiony jest poziom latwy wiec nastepne instrukcje 
                    //sprawdzaja czy ma byc ustawiony inny poziom korzystajac z enum
                    if (e.Key == Key.F2)
                    {
                        Mode = (int)PoziomTrudnosci.Medium;
                        LabelMode.Content = Convert.ToString(PoziomTrudnosci.Medium);

                    }
                    else if (e.Key == Key.F3)
                    {
                        Mode = (int)PoziomTrudnosci.Hard;
                        LabelMode.Content = Convert.ToString(PoziomTrudnosci.Hard);
                    }
                    else if (e.Key == Key.F12)
                    {
                        Mode = (int)PoziomTrudnosci.Extreme;
                        LabelMode.Content = Convert.ToString(PoziomTrudnosci.Extreme);
                    }
                    //ukryj napis Snake
                    LabelSnake.Visibility = Visibility.Hidden;
                    //ukryj menu
                    Menu.Visibility = Visibility.Hidden;
                    InitBoard();
                    InitSnake();
                    InitTimer();
                    InitFood();
                }
                //Jesli wcisniety klawisz Lewy i waz nie porusza sie w prawo to:
                if (e.Key == Key.Left && _directionX != 1 && _directionY != 0)
                {
                    _directionX = -1;
                    _directionY = 0;
                }
                else if (e.Key == Key.Right && _directionX != -1 && _directionY != 0)
                {
                    _directionX = 1;
                    _directionY = 0;
                }
                else if (e.Key == Key.Down && _directionX != 0 && _directionY != -1) 
                {
                    _directionX = 0;
                    _directionY = 1;
                }
                else if (e.Key == Key.Up && _directionX != 0 && _directionY != 1)
                {
                    _directionX = 0;
                    _directionY = -1;
                }

            }
            void InitFood()
            {
                //poczatkowa lokalizacja jedzenia
                _food = new SnakePart(20, 10);
                //kolor jedzenia
                _food.Rect.Fill = Brushes.Blue;
                //rozmiar jedzenia
                _food.Rect.Height = _food.Rect.Width = 20;
                //dodaj jedzenie (prostokat ktory do reprezentuje) do okna
                grid.Children.Add(_food.Rect);
                //dodaj jedzenie do okna (konkretna kolumna i wiersz)
                Grid.SetColumn(_food.Rect, _food.X);
                Grid.SetRow(_food.Rect, _food.Y);
            }

            
            bool CzyJedzenie()
            {
                //Sprawdzenie czy glowa znalazla sie na jedzeniu
                if (_snake.head.X == _food.X && _snake.head.Y == _food.Y)
                {
                    //dodanie kolejnej czesci weza
                    _partsToAdd += Mode; //++
                    //Tworze nowa losowa zmienna
                    Random rand = new Random();
                    //KOLORY
                    Random r = new Random();
                    Color randomColor = new Color();
                    randomColor.A = 255; //alpha channel of the color
                    randomColor.R = (byte)r.Next(150, 255); //red channel
                    randomColor.G = (byte)r.Next(150, 255); //green channel
                    randomColor.B = (byte)r.Next(150, 255); //blue channel

                    MyRectangle.Fill = new SolidColorBrush(randomColor);
                    //grid.Background = new SolidColorBrush(randomColor);

                    //Jesli poprzednia instrukcja warunkowa nie została spełniona to 
                    //20 razy sprawdzam czy pole jest puste
                    for (int i = 0; i < 20; i++)
                    {
                        int x = rand.Next(0, (int)(grid.Width / SIZE));
                        int y = rand.Next(0, (int)(grid.Height / SIZE - 3));

                        if (CzyPolePuste(x, y))
                        {
                            _food.X = x;
                            _food.Y = y;
                            return true;
                        }
                    }
                    KoniecGry();
                }
                return false;
            }
            //Sprawdzenie czy pole jest puste
            bool CzyPolePuste(int x, int y)
            {
                if (_snake.head.X == x && _snake.head.Y == y)
                {
                    return false;
                }
                foreach (SnakePart snakePart in _snake.Parts)
                {
                    if (snakePart.X == x && snakePart.Y == y)
                    {
                        return false;
                    }
                }
                return true;
            }

            void KoniecGry()
            {
                _timer.Stop();
                //jesli uzytkownik nie zdobyl zadnego punktu nalezy obsluzyc wyjatek dzielenia przez zero
                int percent;
                try
                {
                    //obliczenie ilosci zajetego miejsca przez weza
                    percent = 1200 / Points;
                    percent = 100 / percent; 
                }
                catch
                {
                    percent = 0;
                }
                DrukujWynik(Points,percent);
            }

            private void RysujJedzenie()
            {
                Grid.SetColumn(_food.Rect, _food.X);
                Grid.SetRow(_food.Rect, _food.Y);
            }
        //KOLIZJE
            //sprawdz kolizje
            bool CheckCollision()
            {
                //Czy waz wyszedl poza plansze lub Czy glowa weza weszla na ogon
                if (CheckBoardCollision() || CheckItselfCollision())
                {
                    return true;
                }
                return false;
            }

            //czy glowa weza znalazla sie poza polem planszy
            bool CheckBoardCollision()
            {
                if ((_snake.head.X < 0 || _snake.head.X > grid.Width / SIZE) || (_snake.head.Y < 0 || _snake.head.Y > grid.Height / SIZE - 3))
                {
                    return true;
                }
                return false;
            }
            bool CheckItselfCollision()
            {
                //dla kazdej czesci sprawdzam czy znalazla sie w miejscu glowy
                foreach (SnakePart snakePart in _snake.Parts)
                {
                    if (_snake.head.X == snakePart.X && _snake.head.Y == snakePart.Y)
                        return true;
                }
                return false;
            }
            // Polimorfizm statyczny
            public static void DrukujWynik(string points, string percent)
            {
                if (MessageBox.Show("Your score: " + points + " !\nYou have taken " + percent + " % of board!\nCONGRATULATIONS!", "End Game", MessageBoxButton.OK) == MessageBoxResult.OK)
                {
                    //zamkniecie aplikacji po wyswietleniu wyniku
                    Application.Current.Shutdown();
                }
            }
            public static void DrukujWynik(int points, int percent)
            {
                if (MessageBox.Show("Your score: " + points + " !\nYou have taken " + percent + " % of board!\nCONGRATULATIONS!", "End Game", MessageBoxButton.OK) == MessageBoxResult.OK)
                {
                    //zamkniecie aplikacji po wyswietleniu wyniku
                    Application.Current.Shutdown();
                }
            }

        }

        
    }

