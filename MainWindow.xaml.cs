using LawnRobot.Model;
using LawnRobot.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System.Collections.Generic;

namespace LawnRobot
{
    public sealed partial class MainWindow : Window
    {
        public ICollectionView LawnSquaresView { get => _LawnNodesSource.View; }

        private Lawn _CurrentLawn;
        private List<LawnNode> _RawLawnNodes;
        private CollectionViewSource _LawnNodesSource;

        public MainWindow()
        {
            InitializeComponent();
            //Window.Current.AppWindow.Resize(new SizeInt32(1000, 750));

            _CurrentLawn = LawnGenerator.BuildLawn();
            List<LawnNode> grassNodes = _CurrentLawn.GetGrassNodes();
            List<LawnNodeViewModel> nodeViewModels = new List<LawnNodeViewModel>();
            foreach (LawnNode node in grassNodes)
            {
                nodeViewModels.Add(new LawnNodeViewModel(node));
            }
            _LawnNodesSource = new CollectionViewSource() { Source = nodeViewModels };
            }

        private void OnRandomizeClicked(object sender, RoutedEventArgs args)
        {
            
        }

        private void OnExecuteClicked(object sender, RoutedEventArgs args)
        {

        }
    }
}
