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
using System.Windows.Shapes;
using System.ComponentModel;
namespace Tetris
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class PopUpWindow : Window
    {
        public PopUpWindow()
        {
            InitializeComponent();
            SizeToContent = SizeToContent.WidthAndHeight;
            KeyDown += (obj, keyargs) =>
            {
                if (keyargs.Key == Key.Escape)
                    Close();
            };
            SizeChanged += (a, b) =>
            {
                Left = (SystemParameters.PrimaryScreenWidth - ActualWidth) / 2;
                Top = (SystemParameters.PrimaryScreenHeight - ActualHeight) / 2;
            };
        }
    }
}
