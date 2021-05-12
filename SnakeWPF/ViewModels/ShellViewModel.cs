using Caliburn.Micro;
using SnakeWPF.Commands;
using SnakeWPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SnakeWPF.ViewModels
{
    /// <summary>
    /// ViewModel class for game view
    /// </summary>
    public class ShellViewModel : Screen
    {
        #region Fields
        private Random _rand = new Random();
        private const int _tileLength = 16;
        private bool _gameLoaded = false;

        private DispatcherTimer _timer;

        private Canvas _board;
        private FoodModel _food;
        private SnakeModel _snake;
        private ICommand _keyDown;

        private int _snakeLength;
        #endregion

        #region Getters and Setters
        public int SnakeLength
        {
            get { return _snakeLength; }
            set 
            {
                _snakeLength = value;
                NotifyOfPropertyChange(() => SnakeLength);
            }
        }

        public Canvas Board
        {
            get { return _board; }
            set
            {
                _board = value;
                NotifyOfPropertyChange(() => Board);
            }
        }

        public SnakeModel Snake
        {
            get { return _snake; }
            set
            {
                _snake = value;
                NotifyOfPropertyChange(() => Snake);
            }
        }

        public FoodModel Food
        {
            get { return _food; }
            set
            {
                _food = value;
                NotifyOfPropertyChange(() => Food);

            }
        }

        public ICommand KeyDown
        {
            get { return _keyDown; }
            set { _keyDown = value; }
        }
        #endregion

        #region Bitmap Loading
        private static string[] urls = new string[]
        {
            "https://cdn.pixabay.com/photo/2015/02/28/15/25/snake-653639_960_720.jpg",
            "https://cdn.pixabay.com/photo/2016/11/29/02/53/python-1866944_960_720.jpg",
            "https://cdn.pixabay.com/photo/2014/08/15/21/40/snake-419043_960_720.jpg",
            "https://cdn.pixabay.com/photo/2013/12/10/18/22/green-tree-python-226553_960_720.jpg",
            "https://cdn.pixabay.com/photo/2014/07/30/19/28/king-cobra-405623_960_720.jpg"
        };

        private string _selectedUrl = urls[0];

        public string SelectedUrl
        {
            get { return _selectedUrl; }
            set 
            { 
                _selectedUrl = value;
                NotifyOfPropertyChange(() => SelectedUrl);
            }
        }

        public void ChangeImage()
        {
            int[] indexes = Enumerable.Range(0, 5).Where(x => x != Array.IndexOf(urls, SelectedUrl)).ToArray();
            SelectedUrl = urls[indexes[_rand.Next(0, 4)]];
        }
        #endregion

        /// <summary>
        /// Starting new game, called by button
        /// </summary>
        public void LoadBoard()
        {
            if (_gameLoaded)
                return;

            _gameLoaded = true;

            InitializeFields();

            ChangeFoodPosition();
            Board.Children.Add(Food.Shape);

            CreateSnake();

            NotifyOfPropertyChange(() => KeyDown);
            NotifyOfPropertyChange(() => Board);

            SetTimer();
        }

        /// <summary>
        /// Preparing snake and adding it to canvas
        /// </summary>
        private void CreateSnake()
        {
            Snake.Body.Add(new BodyElementModel { Shape = new Rectangle() { Height = _tileLength, Width = _tileLength, Fill = Brushes.Green }, PosX = Snake.HeadX + _tileLength, PosY = Snake.HeadY });
            Canvas.SetTop(Snake.HeadShape, Snake.HeadY);
            Canvas.SetLeft(Snake.HeadShape, Snake.HeadX);
            Board.Children.Add(Snake.HeadShape);

            for (int i = 0; i < 4; i++)
            {
                Snake.Body.Add(new BodyElementModel { Shape = new Rectangle() { Height = _tileLength, Width = _tileLength, Fill = Brushes.Green }, PosX = Snake.Body[i].PosX + _tileLength, PosY = Snake.Body[i].PosY });
            }
            SnakeLength = Snake.Body.Count;

            foreach (var body in Snake.Body)
            {
                Canvas.SetTop(body.Shape, body.PosY);
                Canvas.SetLeft(body.Shape, body.PosX);
                Board.Children.Add(body.Shape);
            }
        }

        /// <summary>
        /// Initializing fields
        /// </summary>
        private void InitializeFields()
        {
            _board = new Canvas();

            _food = new FoodModel()
            {
                Shape = new Ellipse() { Height = _tileLength, Width = _tileLength, Fill = Brushes.Red },
                PosX = 0,
                PosY = 0
            };

            _snake = new SnakeModel()
            {
                Direction = Direction.West,
                HeadShape = new Ellipse() { Height = _tileLength, Width = _tileLength, Fill = Brushes.Green },
                HeadX = 5 * _tileLength,
                HeadY = 5 * _tileLength,
                Body = new ObservableCollection<BodyElementModel>()
            };

            _keyDown = new KeyDownCommand(ref _snake);

            _timer = new DispatcherTimer();
        }

        /// <summary>
        /// Ending the game by stopping timer and clearing canvas
        /// </summary>
        private void GameOver()
        {
            _timer.Stop();
            _gameLoaded = false;
            Board.Children.Clear();
        }

        /// <summary>
        /// Starting timer and defining its behavior
        /// </summary>
        private void SetTimer()
        {
            _timer.Interval = TimeSpan.FromSeconds(0.3);
            _timer.Tick += timer_Tick;
            _timer.Start();
        }

        /// <summary>
        /// Main game loop depending on timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            // Checking if snake hit wall of itself
            if(Snake.HeadX < 0 || Snake.HeadX >= 400 || Snake.HeadY < 0 || Snake.HeadY >= 400)
            {
                GameOver();
                return;
            }

            foreach (var body in Snake.Body)
            {
                if(body.PosX == Snake.HeadX && body.PosY == Snake.HeadY)
                {
                    GameOver();
                    return;
                }
            }

            Move();

            // Handling food eating mechanism
            if (Snake.HeadX == Food.PosX && Snake.HeadY == Food.PosY)
            {
                ChangeFoodPosition();
                ChangeImage();
                Grow();
            }

            NotifyOfPropertyChange(() => Board);
        }

        /// <summary>
        /// Adjusting snake's body coordinates and refreshing it on canvas
        /// </summary>
        private void Move()
        {
            for (int i = Snake.Body.Count - 1; i > 0; i--)
            {
                Snake.Body[i].PosX = Snake.Body[i - 1].PosX;
                Snake.Body[i].PosY = Snake.Body[i - 1].PosY;
            }
            Snake.Body[0].PosX = Snake.HeadX;
            Snake.Body[0].PosY = Snake.HeadY;

            foreach (var body in Snake.Body)
            {
                Canvas.SetTop(body.Shape, body.PosY);
                Canvas.SetLeft(body.Shape, body.PosX);
            }

            switch (Snake.Direction)
            {
                case Direction.North:
                    Snake.HeadY -= _tileLength;
                    Canvas.SetTop(Snake.HeadShape, Snake.HeadY);
                    break;
                case Direction.East:
                    Snake.HeadX += _tileLength;
                    Canvas.SetLeft(Snake.HeadShape, Snake.HeadX);
                    break;
                case Direction.South:
                    Snake.HeadY += _tileLength;
                    Canvas.SetTop(Snake.HeadShape, Snake.HeadY);
                    break;
                case Direction.West:
                    Snake.HeadX -= _tileLength;
                    Canvas.SetLeft(Snake.HeadShape, Snake.HeadX);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Adding new body part after eating food
        /// </summary>
        private void Grow()
        {
            Snake.Body.Add(new BodyElementModel { Shape = new Rectangle() { Height = _tileLength, Width = _tileLength, Fill = Brushes.Green }, PosX = Snake.Body[Snake.Body.Count - 1].PosX, PosY = Snake.Body[Snake.Body.Count - 1].PosY });
            Canvas.SetTop(Snake.Body[Snake.Body.Count - 1].Shape, Snake.Body[Snake.Body.Count - 1].PosY);
            Canvas.SetLeft(Snake.Body[Snake.Body.Count - 1].Shape, Snake.Body[Snake.Body.Count - 1].PosX);
            Board.Children.Add(Snake.Body[Snake.Body.Count - 1].Shape);

            SnakeLength = Snake.Body.Count;
        }

        /// <summary>
        /// Repositioning food after snake has eaten it
        /// </summary>
        private void ChangeFoodPosition()
        {
            Food.PosX = _rand.Next(0, 24) * _tileLength;
            Food.PosY = _rand.Next(0, 24) * _tileLength;
            Canvas.SetTop(Food.Shape, Food.PosY);
            Canvas.SetLeft(Food.Shape, Food.PosX);
        }
    }
}
