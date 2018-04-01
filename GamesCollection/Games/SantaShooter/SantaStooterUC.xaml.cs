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
using System.Windows.Threading;
using System.Windows.Media.Animation;

namespace SantaShooter
{
    public partial class SantaShooterUC : UserControl
    {
        public static class Images
        {
            public static readonly BitmapImage santa;
            public static readonly BitmapImage saw;
            public static readonly BitmapImage hearth;

            static Images()
            {
                santa = new BitmapImage(new Uri(Environment.CurrentDirectory + @"/Images/Santa.png"));
                saw = new BitmapImage(new Uri(Environment.CurrentDirectory + @"/Images/Saw.png"));
                hearth = new BitmapImage(new Uri(Environment.CurrentDirectory + @"/Images/Hearth.svg"));

            }
        }

        static int imageSize = 150;
        public DispatcherTimer Spawner;
        public Random rng;
        Window wnd;
        private int score = 0;
        private int hp = 3;
        int counter1 = 0;
        int counter2 = 0;
        event Action GameOver;
        private bool immortality = false;
        private bool IsGameOver = false;
        public int Score
        {
            get => score;
            set
            {
                score = value;
                ScoreLabel.Content = score;
                if (score >= 100)
                    Win();
            }
        }
        public int Hp
        {
            get => hp;
            set
            {
                if (value <= 5 && hp > 0)
                {
                    hp = value;
                    UpdateHealth();
                }
                if (value == 0 && !immortality)
                {
                    Loss();
                }
            }
        }
        MediaPlayer NoisePlayer = new MediaPlayer();
        public SantaShooterUC(Window wnd)
        {
            InitializeComponent();
            this.wnd = wnd;
            Environment.CurrentDirectory = GameCollection.App.BasePath + @"/Games/SantaShooter";
            Mouse.OverrideCursor = new Cursor(Environment.CurrentDirectory + @"/Images/Snipe.cur");
            ImmortalityCheckBox.Visibility = Visibility.Hidden;
            Spawner = new DispatcherTimer();
            rng = new Random();
            Spawner.Interval = TimeSpan.FromMilliseconds(500);
            Spawner.Tick += (a, b) => SpawnAThing();
            HP1.Source = HP2.Source = HP3.Source = HP4.Source = HP5.Source = Images.hearth;
            UpdateHealth();
        }

        public abstract class FallingStuff
        {
            public int size;
            protected SantaShooterUC UC;
            public Image img;
            public int speed;
            public PathFigure pathFigure;
            public FallingStuff(int start, BitmapImage _img, SantaShooterUC universe, int size)
            {
                UC = universe;
                pathFigure = new PathFigure();
                this.size = size;
                img = new Image
                {
                    Source = _img,
                    Width = size,
                    Height = size,
                    Margin = new Thickness(start, -size, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                };
                UC.Griddy.Children.Add(img);

            }
            public virtual void Click()
            {
                Kill();
            }
            public virtual void FallOffOfTheFlatWorld()
            {
                Kill();
            }
            public void Kill()
            {
                UC.Griddy.Children.Remove(img);
            }
            public void PlaySound(string nameAndFormat)
            {
                UC.NoisePlayer.Stop();
                UC.NoisePlayer.Open(new Uri(Environment.CurrentDirectory + $@"/Sounds/{nameAndFormat}"));
                UC.NoisePlayer.Play();
            }
        }
        public class Santa : FallingStuff
        {
            public Santa(int start, SantaShooterUC universe, int size) : base(start, Images.santa, universe, size)
            {
                Grid.SetZIndex(img, 1);
                speed = 5;
                pathFigure.StartPoint = new Point(0, -size / 2);
                PolyBezierSegment segment = new PolyBezierSegment();
                segment.Points.Add(pathFigure.StartPoint);
                for (int i = 1; i <= (UC.Score / 10) + 3; i++)
                {
                    segment.Points.Add(
                        new Point(
                            UC.rng.Next(-size / 2, (int)UC.Griddy.ActualWidth - start - size - (int)segment.Points.Last().X),
                            UC.rng.Next((int)segment.Points.Last().Y, i * (((int)UC.Griddy.ActualHeight - size) / ((UC.Score / 10) + 3)))
                            ));
                }
                PolyLineSegment polyLineSegment = new PolyLineSegment();
                polyLineSegment.Points.Add(
                    new Point(
                        UC.rng.Next(-start - size, (int)UC.Griddy.ActualWidth - start - size - (int)segment.Points.Last().X),
                        UC.Griddy.ActualHeight
                        ));
                pathFigure.Segments.Add(segment);
                pathFigure.Segments.Add(polyLineSegment);
            }
            public override void Click()
            {
                PlaySound($@"Snipe/{(++UC.counter1 % 8) + 1}.mp3");
                UC.Score++;
                Kill();
            }
            public override void FallOffOfTheFlatWorld()
            {
                if (!UC.IsGameOver)
                {
                    UC.Hp--;
                }
                Kill();
            }
        }
        public class Saw : FallingStuff
        {
            public Saw(int start, SantaShooterUC universe, int size) : base(start, Images.saw, universe, size)
            {
                Grid.SetZIndex(img, 2);
                speed = 5;
                pathFigure.StartPoint = new Point(0, -size / 2);
                PolyBezierSegment segment = new PolyBezierSegment();
                segment.Points.Add(pathFigure.StartPoint);
                for (int i = 1; i <= (UC.Score / 10) + 6; i++)
                {
                    segment.Points.Add(
                        new Point(
                            UC.rng.Next(-size / 2, (int)UC.Griddy.ActualWidth - start - size),
                            UC.rng.Next(-size / 2, (int)UC.Griddy.ActualHeight - size)
                            ));
                }
                PolyLineSegment polyLineSegment = new PolyLineSegment();
                polyLineSegment.Points.Add(
                    new Point(
                        UC.rng.Next(-start - size, (int)UC.Griddy.ActualWidth - start - size - (int)segment.Points.Last().X),
                        UC.Griddy.ActualHeight
                        ));
                pathFigure.Segments.Add(segment);
                pathFigure.Segments.Add(polyLineSegment);
            }
            public override void Click()
            {
                int i = (++UC.counter2 % 6) + 1;
                if (i > 8 || i < 1)
                {
                    throw new Exception();
                }
                PlaySound($@"Punch/{(++UC.counter2 % 6) + 1}.mp3");
                UC.Hp--;
            }
        }
        public class Hearth : FallingStuff
        {
            public Hearth(int start, SantaShooterUC universe, int size): base(start, Images.hearth, universe, size)
            {
                Grid.SetZIndex(img, 1);
                speed = 3;
                pathFigure.StartPoint = new Point(0, -size / 2);
                PolyLineSegment segment = new PolyLineSegment();
                segment.Points.Add(pathFigure.StartPoint);
                for (int i = 1; i <= (UC.Score / 10) + 2; i++)
                {
                    segment.Points.Add(
                        new Point(
                            UC.rng.Next(-start + size / 2, (int)UC.Griddy.ActualWidth - start - size),
                            UC.rng.Next((int)segment.Points.Last().Y, i * (((int)UC.Griddy.ActualHeight - size) / ((UC.Score / 10) + 2)))
                            ));
                }
                PolyLineSegment polyLineSegment = new PolyLineSegment();
                segment.Points.Add(
                    new Point(
                        UC.rng.Next(-start - size, (int)UC.Griddy.ActualWidth - start - size - (int)segment.Points.Last().X),
                        UC.Griddy.ActualHeight
                        ));
                pathFigure.Segments.Add(segment);
                pathFigure.Segments.Add(polyLineSegment);
            }
            public override void Click()
            {
                //PlaySound("Choir.mp3");
                UC.Hp++;
                Kill();
            }
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Spawner.Start();
        }
        public void Start()
        {
            
        }
        void UpdateHealth()
        {
            if (Hp >= 1)
            {
                if (HP1.Visibility != Visibility.Visible)
                    HP1.Visibility = Visibility.Visible;
            }
            else
            {
                if (HP1.Visibility != Visibility.Hidden)
                    HP1.Visibility = Visibility.Hidden;
            }
            if (Hp >= 2)
            {
                if (HP2.Visibility != Visibility.Visible)
                    HP2.Visibility = Visibility.Visible;
            }
            else
            {
                if (HP2.Visibility != Visibility.Hidden)
                    HP2.Visibility = Visibility.Hidden;
            }
            if (Hp >= 3)
            {
                if (HP3.Visibility != Visibility.Visible)
                    HP3.Visibility = Visibility.Visible;
            }
            else
            {
                if (HP3.Visibility != Visibility.Hidden)
                    HP3.Visibility = Visibility.Hidden;
            }
            if (Hp >= 4)
            {
                if (HP4.Visibility != Visibility.Visible)
                    HP4.Visibility = Visibility.Visible;
            }
            else
            {
                if (HP4.Visibility != Visibility.Hidden)
                    HP4.Visibility = Visibility.Hidden;
            }
            if (Hp >= 5)
            {
                if (HP5.Visibility != Visibility.Visible)
                    HP5.Visibility = Visibility.Visible;
            }
            else
            {
                if (HP5.Visibility != Visibility.Hidden)
                    HP5.Visibility = Visibility.Hidden;
            }
        }
        void Loss()
        {
            IsGameOver = true;
            MessageBox.Show("You Lost" + Environment.NewLine + $"Score: {Score}");
            NewGame();

        }
        void Win()
        {
            IsGameOver = true;
            MessageBox.Show("YOU WON");
            NewGame();
        }
        void NewGame()
        {
            GameOver.Invoke();
            foreach (Action item in GameOver.GetInvocationList())
            {
                GameOver -= item;
            }
            Griddy.Children.Clear();
            Score = 0;
            hp = 3;
            UpdateHealth();
            IsGameOver = false;
        }

        void SpawnAThing()
        {
            FallingStuff thing;
            int start = rng.Next(0, (int)Griddy.ActualWidth - imageSize);
            int spawnChance = rng.Next(1, 11);
            if (spawnChance < 6)
            {
                thing = new Santa(start, this, imageSize);
            }
            else if (spawnChance < 10)
            {
                thing = new Saw(start, this, imageSize);
            }
            else
            {
                thing = new Hearth(start, this, imageSize);
            }
            
            thing.img.RenderTransform =new MatrixTransform();
            PathGeometry animationPath = new PathGeometry();
            animationPath.Figures.Add(thing.pathFigure);
            animationPath.Freeze();

            MatrixAnimationUsingPath anim = new MatrixAnimationUsingPath
            {
                PathGeometry = animationPath,
                Duration = TimeSpan.FromSeconds(thing.speed),
                RepeatBehavior = new RepeatBehavior(1),
                FillBehavior = FillBehavior.Stop
            };
           
            NameScope.SetNameScope(this, new NameScope());
            RegisterName("MatrixTransform", thing.img.RenderTransform);
            Storyboard.SetTargetName(anim, "MatrixTransform");
            Storyboard.SetTargetProperty(anim, new PropertyPath( MatrixTransform.MatrixProperty));

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(anim);
            storyboard.RepeatBehavior = new RepeatBehavior(1);

            thing.img.Loaded += (a, b) => storyboard.Begin(this);
            EventHandler hop = new EventHandler((a, b) => thing.FallOffOfTheFlatWorld());
            storyboard.Completed += hop;
            GameOver += () => storyboard.Completed -= hop;
            thing.img.MouseDown += (a, b) => {
                thing.Click();
                storyboard.Pause();
                storyboard.Completed -= hop;
                GameOver -= () => storyboard.Completed -= hop;
            };
        }

        private void ImmortalityCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            immortality = true;
        }

        private void ImmortalityCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            immortality = false;
        }
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var newSize = e.NewSize;
            imageSize = (int)((newSize.Height / 2 + newSize.Width / 2) / 7);
        }
        ~SantaShooterUC()
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = null;
            });
            
        }
    }
}

