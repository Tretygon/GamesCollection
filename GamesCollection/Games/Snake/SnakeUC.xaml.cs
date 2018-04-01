using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Snake
{
    public partial class SnakeUC : UserControl
    {
        const int rows = 12, columns = 12;
        System.Timers.Timer timer = new System.Timers.Timer() { Enabled = false, Interval = 100};
        LinkedList<Cell> snake;
        Cell current;
        Random rng = new Random();
        enum Directions { left, right, up, down};
        enum State {Empty,Fruit,Snake};
        Directions currentDirection;
        Directions nextDirection = Directions.left;
        Rectangle[,] field;
        State[,] StateField;
        Brush BaseBrush = Brushes.WhiteSmoke;
        Brush SnakeBrush = Brushes.Green;
        Window wnd;
        public SnakeUC(Window wnd)
        {
            InitializeComponent();
            Environment.CurrentDirectory = GameCollection.App.BasePath + @"/Games/Snake";
            this.wnd = wnd;
            wnd.KeyDown += Key_Down;

            Griddy.Background = BaseBrush;//Brushes.Blue;
            timer.Elapsed += (a, b) => Move();
            field = GridInitialize(Griddy, columns, rows);
            StateField = new State[columns, rows];

            for (int x = 0; x < columns; x++)
                for (int y = 0; y < rows; y++)
                    StateField[x, y] = State.Empty;

            GenerateFruit();
            NewGame(new Cell(rng.Next(0,columns), rng.Next(0, rows)));
        }
        

        void NewGame(Cell start)
        {
            snake = new LinkedList<Cell>();
            snake.AddFirst(start);
            current = start;
            Dispatcher.Invoke(() => field[current.X, current.Y].Fill = SnakeBrush);
            timer.Start();
        }
        void Restart()
        {
            timer.Stop();
            var last = snake.Last.Value;

            foreach (var item in snake)
            {
                Dispatcher.Invoke(() => field[item.X, item.Y].Fill = BaseBrush);
                StateField[item.X, item.Y] = State.Empty;
            }
            NewGame(last);
        }
        void Move()
        {
            currentDirection = nextDirection;

            Cell next = new Cell(current.X, current.Y);
            
            switch (currentDirection)
            {
                case Directions.left:
                    next.X--;
                    break;
                case Directions.right:
                    next.X++;
                    break;
                case Directions.up:
                    next.Y--;
                    break;
                case Directions.down:
                    next.Y++;
                    break;
                default:
                    break;
            }
           
            switch (StateField[next.X, next.Y])
            {
                case State.Empty:
                    var first = snake.First.Value;
                    snake.RemoveFirst();
                    Dispatcher.Invoke(() => field[first.X,first.Y].Fill = BaseBrush);
                    
                    StateField[first.X, first.Y] = State.Empty;
                    break;
                case State.Fruit:
                    GenerateFruit();
                    break;
                case State.Snake:
                    Restart();
                    return;
                default:
                    break;
            }
            Dispatcher.Invoke(() => field[next.X, next.Y].Fill = SnakeBrush);
            StateField[next.X, next.Y] = State.Snake;
            snake.AddLast(next);
            current = next;
        }

        void GenerateFruit()
        {
            List<Cell> Free = new List<Cell>();
            for (int x = 0; x < columns; x++)
                for (int y = 0; y < rows; y++)
                    if (StateField[x, y] == State.Empty)
                        Free.Add(new Cell(x, y));
            Cell fruit = Free[rng.Next(0, Free.Count)];
            Dispatcher.Invoke(() => field[fruit.X, fruit.Y].Fill = Brushes.Red);
            StateField[fruit.X, fruit.Y] = State.Fruit;
        }


        Rectangle[,] GridInitialize(Grid grid, int columns, int rows)
        {
            Rectangle[,] field = new Rectangle[columns, rows];

            for (int i = 0; i < rows; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }
            for (int i = 0; i < columns; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    Rectangle rectangle = new Rectangle
                    {
                        Margin = new Thickness(0.5),
                        Stretch = Stretch.Fill,
                        Fill = BaseBrush
                    };

                    field[x, y] = rectangle;
                    Grid.SetColumn(rectangle, x);
                    Grid.SetRow(rectangle, y);
                    grid.Children.Add(rectangle);
                }
            }
            return field;
        }

        public void Key_Down(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Space:
                    timer.Enabled = !timer.Enabled;
                    if (timer.Enabled)
                        GameCollection.App.MusicPlayer.Play();
                    else
                        GameCollection.App.MusicPlayer.Pause();
                    break;
                case Key.Left:
                    if (currentDirection!=Directions.right)
                        nextDirection = Directions.left;
                    break;
                case Key.Right:
                    if (currentDirection != Directions.left)
                        nextDirection = Directions.right;
                    break;
                case Key.Down:
                    if (currentDirection != Directions.up)
                        nextDirection = Directions.down;
                    break;
                case Key.Up:
                    if (currentDirection != Directions.down)
                        nextDirection = Directions.up;
                    break;
                default:
                    break;
            }
        }

        

        class Cell
        {
            private int x, y;
            public int X
            {
                get => x;
                set
                {
                    if (value < 0)
                    {
                        x = value + columns;
                    }
                    else if(value >= columns)
                    {
                        x = value % columns;
                    }
                    else
                    {
                        x = value;
                    }
                }
            }
            public int Y
            {
                get => y;
                set
                {
                    if (value < 0)
                    {
                        y = value + rows;
                    }
                    else if (value >= rows)
                    {
                        y = value % rows;
                    }
                    else
                    {
                        y = value;
                    }
                }
            }
            public Cell(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GameCollection.App.MusicPlayer.Play();
        }
        
        ~SnakeUC()
        {
            wnd.KeyDown -= Key_Down;
        }
    }
}
