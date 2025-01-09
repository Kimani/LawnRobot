using LawnRobot.Model;
using LawnRobot.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Metrics;

namespace LawnRobot
{
    public sealed partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ICollectionView? LawnSquaresView { get => _LawnNodesSource?.View; }
        public LawnMower? Mower
        {
            get => _Mower;
            set
            {
                if (_Mower != value)
                {
                    _Mower = value;
                    Notify("Mower");
                }
            }
        }

        private Lawn? _CurrentLawn;
        private CollectionViewSource? _LawnNodesSource;
        private LawnSolver? _Solver;
        private LawnMower? _Mower;

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void RefreshLawn()
        {
            _CurrentLawn = LawnGenerator.BuildLawn();

            List<LawnNode> grassNodes = _CurrentLawn.GetLawnNodes();
            var nodeViewModels = new List<LawnNodeViewModel>();
            foreach (LawnNode node in grassNodes)
            {
                nodeViewModels.Add(new LawnNodeViewModel(node));
            }

            _LawnNodesSource = new CollectionViewSource() { Source = nodeViewModels }; ;
            Notify("LawnSquaresView");

            // Setup the Mower and the Solver.
            LawnNode startNode = _CurrentLawn.GetLawnStartNode();
            Mower = new LawnMower(startNode);
            MowerElement.DataContext = Mower;
            _Solver = new LawnSolver(Mower);
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
            if ((sender is FrameworkElement element) &&
                (VisualTreeHelper.GetParent(element) is ContentPresenter presenter) &&
                (element.DataContext is LawnNodeViewModel viewModel))
            {
                Binding topBinding = new Binding
                {
                    Path = new PropertyPath("Y"),
                    Source = viewModel,
                    Converter = (IValueConverter)RootGrid.Resources["GridToCanvasY"],
                };
                BindingOperations.SetBinding(presenter, Canvas.TopProperty, topBinding);

                Binding leftBinding = new Binding
                {
                    Path = new PropertyPath("X"),
                    Source = viewModel,
                    Converter = (IValueConverter)RootGrid.Resources["GridToCanvasX"],
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
            // Window.Current.AppWindow.Resize(new SizeInt32(850, 725));
            RefreshLawn();
        }
    }
}
