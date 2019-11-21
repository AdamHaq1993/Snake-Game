using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SnakeGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    ////Simple snake game
    //features:
    //a snake that eats food and grows in size as HeaderedContentControl does
    //the snake gets faster randomly as he eats food
    //snake dies if he touches himself or the edges
    //plays audio on eating and speeding up events
    //just closes the app
    public partial class MainWindow : Window
    {
        // list of food
        private readonly List<Point> _bonusPoints = new List<Point>();

        //list for snake
        private readonly List<Point> _snakePoints = new List<Point>();

        private readonly Brush _snakeColor = Brushes.Green;
        private enum SnakeSize
        {
            Thin = 4,
            Normal = 6,
            Thick = 8
        };
        private enum Movingdirection
        {
            Upwards = 8,
            Downwards = 2,
            Toleft = 4,
            Toright = 6
        };
        public int p = 1;
        public double i = 1;
        public  double gameSpeed = 50000;
        private readonly Point _startingPoint = new Point(100, 100);
        private Point _currentPosition = new Point();

        // Movement direction initialisation
        private int _direction = 0;

        //text for score
     //   public string TextScore;

        //Placeholder for the previous movement direction
        //  The guy needs this to avoid its own body.  
        private int _previousDirection = 0;

        //Here you can change the size of the snake. 
        // Possible sizes are THIN, NORMAL and THICK
        private readonly int _headSize = (int)SnakeSize.Thick;

        private int _length = 100;
        private int _score = 0;
        private readonly Random _rnd = new Random();
        private readonly object CoreApplication;
        DispatcherTimer timer = new DispatcherTimer();
        
        public MainWindow()
        {
            InitializeComponent();
            eat.Position = TimeSpan.FromSeconds(1);
            backgroundMusic.Volume = 0.2;
            backgroundMusic.Play();
            TextScore.Text = "0";
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan((int)gameSpeed);
            timer.Start();
            
            this.KeyDown += new KeyEventHandler(OnButtonKeyDown);
            PaintSnake(_startingPoint);
            _currentPosition = _startingPoint;

            // puts food down if theres less than 10 foods
            for (var n = 0; n < 10; n++)
            {
                PaintBonus(n);
            }
        }
        private int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        private void PaintBonus(int index)
        {
            Point bonusPoint = new Point(_rnd.Next(5, 780), _rnd.Next(5, 480));

            Ellipse newEllipse = new Ellipse
            {
                Fill = Brushes.Red,
                Width = 10,
                Height = 10
            };

            Canvas.SetTop(newEllipse, bonusPoint.Y);
            Canvas.SetLeft(newEllipse, bonusPoint.X);
            PaintCanvas.Children.Insert(index, newEllipse);
            _bonusPoints.Insert(index, bonusPoint);

        }


        private void PaintSnake(Point currentposition)
        {

            //make the snake longer

            Ellipse newEllipse = new Ellipse
            {
                Fill = _snakeColor,
                Width = _headSize,
                Height = _headSize
            };

            Canvas.SetTop(newEllipse, currentposition.Y);
            Canvas.SetLeft(newEllipse, currentposition.X);

            int count = PaintCanvas.Children.Count;
            PaintCanvas.Children.Add(newEllipse);
            _snakePoints.Add(currentposition);

            // fix
            if (count > _length)
            {
                PaintCanvas.Children.RemoveAt(count - _length + 9);
                _snakePoints.RemoveAt(count - _length);
            }
        }

      

        private void timer_Tick(object sender, EventArgs e)
        {
            
            //this is being werid... fix later
            switch (_direction)
            {
                case (int)Movingdirection.Downwards:
                    _currentPosition.Y += 1;
                    PaintSnake(_currentPosition);
                    break;
                case (int)Movingdirection.Upwards:
                    _currentPosition.Y -= 1;
                    PaintSnake(_currentPosition);
                    break;
                case (int)Movingdirection.Toleft:
                    _currentPosition.X -= 1;
                    PaintSnake(_currentPosition);
                    break;
                case (int)Movingdirection.Toright:
                    _currentPosition.X += 1;
                    PaintSnake(_currentPosition);
                    break;
            }
            if ((_currentPosition.X < 5) || (_currentPosition.X > 680) ||
                (_currentPosition.Y < 5) || (_currentPosition.Y > 500))
            {
                GameOver();     
            }

            // eating makes the snake get bigger
            int n = 0;
            foreach (Point point in _bonusPoints)
            {

                if ((Math.Abs(point.X - _currentPosition.X) < _headSize) &&
                    (Math.Abs(point.Y - _currentPosition.Y) < _headSize))
                {

                    //play eating noise
                    eat.Stop();
                    eat.Position = TimeSpan.FromSeconds(1);
                    eat.Play();
                    _length += 10;
                    _score += 10;
                    TextScore.Text =  Convert.ToString(_score);
                    //make snake move faster
                    int returnValue = RandomNumber(1, 100);
                    if (returnValue < 35)
                    {
                        //play whoosh noise 
                        speedUp.Stop();
                        speedUp.Position = TimeSpan.FromSeconds(0);
                        speedUp.Play();
                        //increase game speed
                        timer.Stop();
                        if (gameSpeed > 10000)
                        {
                            gameSpeed = (gameSpeed - 100);
                        }
                        if (gameSpeed < 10000)
                        {
                            gameSpeed = 10000;
                        }
                       
                        //DispatcherTimer timer = new DispatcherTimer();
                        timer.Tick += new EventHandler(timer_Tick);
                        timer.Interval = new TimeSpan((int)gameSpeed);
                        timer.Start();

                    }
                    // erase the food
                    // from the list of bonuses as well as from the canvas
                    _bonusPoints.RemoveAt(n);
                    PaintCanvas.Children.RemoveAt(n);
                    PaintBonus(n);
                    break;

                }
                n++;
            }

            // cant hit yourself
            for (int q = 0; q < (_snakePoints.Count - _headSize * 2); q++)
            {
                Point point = new Point(_snakePoints[q].X, _snakePoints[q].Y);
                if ((Math.Abs(point.X - _currentPosition.X) < (_headSize)) &&
                     (Math.Abs(point.Y - _currentPosition.Y) < (_headSize)))
                {
                    GameOver();                   
                    break;
                }

            }
            
        }

        public void MediaFailedHandler(object sender, ExceptionRoutedEventArgs e)
        {
            // e.ErrorException contains information what went wrong when playing your mp3
        }

        private void OnButtonKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Down:
                    if (_previousDirection != (int)Movingdirection.Upwards)
                        _direction = (int)Movingdirection.Downwards;
                    break;
                case Key.Up:
                    if (_previousDirection != (int)Movingdirection.Downwards)
                        _direction = (int)Movingdirection.Upwards;
                    break;
                case Key.Left:
                    if (_previousDirection != (int)Movingdirection.Toright)
                        _direction = (int)Movingdirection.Toleft;
                    break;
                case Key.Right:
                    if (_previousDirection != (int)Movingdirection.Toleft)
                        _direction = (int)Movingdirection.Toright;
                    break;

            }
            _previousDirection = _direction;

        }

     

        private void GameOver()
        {

            //change this so you dont have to back out each time
            MessageBox.Show($@"You Lose! Your score is { _score}", "Game Over", MessageBoxButton.OK, MessageBoxImage.Hand);
            this.Close();

            //MainWindow win2 = new MainWindow();

            //if (p != 1)
            //{
            //    win2.Show();
            //    p++;
            //}
            //p = 0;
        }
    }
}
