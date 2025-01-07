using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace LawnRobot
{
    public sealed partial class MainWindow : Window
    {
        public Lawn CurrentLawn { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            //Window.Current.AppWindow.Resize(new SizeInt32(1000, 750));

            CurrentLawn = LawnGenerator.BuildLawn();
        }

        private void OnRandomizeClicked(object sender, RoutedEventArgs args)
        {
            
        }

        private void OnExecuteClicked(object sender, RoutedEventArgs args)
        {

        }
    }
}
