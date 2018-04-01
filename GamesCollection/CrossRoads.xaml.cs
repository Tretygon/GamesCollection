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
    
    /// <summary>
    /// Interaction logic for CrossRoads.xaml
    /// </summary>
    public partial class CrossRoads : UserControl
    {
        public CrossRoads()
        {
            InitializeComponent();
            Environment.CurrentDirectory = App.BasePath;
        }

        private void TetrisButton_Click(object sender, RoutedEventArgs e)
        {
            App.ChangeUserControl(new Tetris.TetrisUC());
        }
        private void SnakeButton_Click(object sender, RoutedEventArgs e)
        {
            App.ChangeUserControl(new Snake.SnakeUC(App.wnd), "Snake");
        }
        private void SantaButton_Click(object sender, RoutedEventArgs e)
        {
            App.ChangeUserControl(new SantaShooter.SantaShooterUC(App.wnd), "Santa Shooter");
        }
        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
