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

namespace GameCollection
{

    public partial class App : Application
    {
        public static Window wnd;
        public static MediaPlayer MusicPlayer;
        public static String BasePath;
        
        public App()
        {
#if DEBUG
            Environment.CurrentDirectory = Environment.CurrentDirectory + "/../../";
#endif
            BasePath = Environment.CurrentDirectory;

            MusicPlayer = new MediaPlayer();
            MusicPlayer.MediaEnded += (a, b) => { MusicPlayer.Position = TimeSpan.Zero; MusicPlayer.Play();};
                       
            wnd = new MainWindow();
            //ChangeUserControl(new SantaShooter.SantaShooterUC(), "Santa Shooter");
            //ChangeUserControl(new Snake.SnakeUC(wnd), "Snake");
            //ChangeUserControl(new Tetris.Tetris.TetrisUC(), "Snake");
            ChangeUserControl(new CrossRoads(), "lol");
            wnd.Show();
        }
        public static void ChangeUserControl(UserControl userContol, String title)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                wnd.Content = userContol;
                MusicPlayer.Stop();
                MusicPlayer.Open(new Uri(Environment.CurrentDirectory + @"/Sound/Music.mp3"));
                MusicPlayer.Play();
                wnd.Icon = new BitmapImage(new Uri(Environment.CurrentDirectory + @"/Icon.ico"));
                wnd.Title = title;
            });
        }
    }
}
