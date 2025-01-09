using LawnRobot.Model;
using LawnRobot.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System.Collections.Generic;
using System.ComponentModel;

namespace LawnRobot
{
    public sealed partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ICollectionView? LawnSquaresView { get => _LawnNodesSource?.View; }

        private Lawn? _CurrentLawn;
        private CollectionViewSource? _LawnNodesSource;

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void RefreshLawn()
        {
            _CurrentLawn = LawnGenerator.BuildLawn();

            List<LawnNode> grassNodes = _CurrentLawn.GetGrassNodes();
            var nodeViewModels = new List<LawnNodeViewModel>();
            foreach (LawnNode node in grassNodes)
            {
                nodeViewModels.Add(new LawnNodeViewModel(node));
            }

            _LawnNodesSource = new CollectionViewSource() { Source = nodeViewModels }; ;
            Notify("LawnSquaresView");
        }

        private void OnRandomizeClicked(object sender, RoutedEventArgs args)
        {
            // EnsureExecutionPaused();
            RefreshLawn();
        }

        private void OnExecuteClicked(object sender, RoutedEventArgs args)
        {

        }

        private void OnTileLoaded(object sender, RoutedEventArgs e)
        {
            if ((sender is Image image) &&
                (VisualTreeHelper.GetParent(image) is ContentPresenter presenter) &&
                (image.DataContext is LawnNodeViewModel viewModel))
            {
                var converter = (IValueConverter)RootGrid.Resources["GridToCanvas"];

                Binding topBinding = new Binding
                {
                    Path = new PropertyPath("Y"),
                    Source = viewModel,
                    Converter = converter,
                };
                BindingOperations.SetBinding(presenter, Canvas.TopProperty, topBinding);

                Binding leftBinding = new Binding
                {
                    Path = new PropertyPath("X"),
                    Source = viewModel,
                    Converter = converter,
                };
                BindingOperations.SetBinding(presenter, Canvas.LeftProperty, leftBinding);
            }
        }

        private void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnRootGridLoaded(object sender, RoutedEventArgs e)
        {
            //Window.Current.AppWindow.Resize(new SizeInt32(1000, 750));
            RefreshLawn();
        }
    }
}
