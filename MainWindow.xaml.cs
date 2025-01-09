using LawnRobot.Model;
using LawnRobot.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Threading;

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
        private int? _CurrentlyExecutingSession = null;
        int _NextSession = 0;

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
                nodeViewModels.Add(new LawnNodeViewModel(DispatcherQueue, node));
            }

            _LawnNodesSource = new CollectionViewSource() { Source = nodeViewModels }; ;
            Notify("LawnSquaresView");

            // Setup the Mower and the Solver.
            _CurrentlyExecutingSession = null;
            LawnNode startNode = _CurrentLawn.GetLawnStartNode();
            Mower = new LawnMower(startNode);
            MowerElement.DataContext = Mower;
            _Solver = new LawnSolver(Mower);
        }

        private void OnRandomizeClicked(object sender, RoutedEventArgs args)
        {
            RefreshLawn();
            ExecuteButton.IsEnabled = true;
        }

        private void OnExecuteClicked(object sender, RoutedEventArgs args)
        {
            if (_CurrentlyExecutingSession == null)
            {
                ExecuteButton.IsEnabled = false;
                _CurrentlyExecutingSession = _NextSession++;
                Thread newThread = new Thread(new ThreadStart(LawnMowerExecutionThread));
                newThread.Start();
            }
        }

        private void LawnMowerExecutionThread()
        {
            // Copy resources locally so we do not start operating on an incoming dataset
            // if our execution context is ended and we should quit out.
            int thisSession = _CurrentlyExecutingSession.Value;
            LawnMower mowerLocal = Mower;
            Lawn lawnLocal = _CurrentLawn;
            LawnSolver solverLocal = _Solver;

            // Do one step at a time, seperated by 1/3 of a second, until we find that every
            // grass node in the lawn is ShortGrass, or until we observe that we're no longer
            // the current executing simulation, in which case we should bail.
            solverLocal.Step();
            while (
                (_CurrentlyExecutingSession != null) &&
                (_CurrentlyExecutingSession.Value == thisSession) &&
                lawnLocal.HasAnyTallGrass())
            {
                Thread.Sleep(333);
                solverLocal.Step();
            }

            /// Complete this operation by clearing out our currently executing session.
            _CurrentlyExecutingSession = null;
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

                if (sender is Image image)
                {
                    Binding sourceBinding = new Binding
                    {
                        Path = new PropertyPath("DisplayType"),
                        Source = viewModel,
                        Converter = (IValueConverter)RootGrid.Resources["GrassToImage"],
                    };
                    BindingOperations.SetBinding(image, Image.SourceProperty, sourceBinding);
                }
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
