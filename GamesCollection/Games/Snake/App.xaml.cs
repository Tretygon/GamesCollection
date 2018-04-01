using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using System;


namespace Tetris
{
    public partial class App : Application
    {
        public static MainWindow wnd;
        public static GameSettings Settings { get; private set; }
        public App()
        {
#if DEBUG
            Environment.CurrentDirectory = Environment.CurrentDirectory + @"../../../";
#endif
            Settings = GameSettings.Load();
            wnd = new MainWindow();           
            wnd.Show();
            Exit += (a,b)=> Settings.Save();
        }
        public static void RestoreDefaultSettings()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Settings = new GameSettings();
                Settings.Save();
                MainWindow newWnd = new MainWindow();            //apparently there must always be a visible mainwindow, otherwise the app shuts down...
                newWnd.Show();
                Application.Current.MainWindow = newWnd;
                wnd.Close();
                wnd = newWnd;
                
            });
        }
    }
}

