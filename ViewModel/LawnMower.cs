using LawnRobot.Model;
using System;
using System.ComponentModel;

namespace LawnRobot.ViewModel
{
    public class LawnMower : IMower, INotifyPropertyChanged
    {
        public ILawnNode Location
        {
            get => _CurrentNode;
            set
            {
                if (_CurrentNode != value)
                {
                    _CurrentNode = (LawnNode)value;
                    Notify("Location");
                    Notify("LocationX");
                    Notify("LocationY");
                }
            }
        }
        public MowerDirection Direction
        { 
            get => _CurrentDirection;
            private set
            {
                if (_CurrentDirection != value)
                {
                    _CurrentDirection = value;
                    Notify("Direction");
                }
            }
        }
        public int LocationX { get => _CurrentNode.X; }
        public int LocationY { get => _CurrentNode.Y; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private LawnNode _CurrentNode;
        private MowerDirection _CurrentDirection = MowerDirection.Up;

        public LawnMower(LawnNode startNode)
        {
            _CurrentNode = startNode;
        }

        public bool Go()
        {
            throw new NotImplementedException();
        }

        public void Mow()
        {
            _CurrentNode.Mow();
        }

        public void TurnRight()
        {
            // There's a cleaner way to do this where each of the enumerations
            // are assigned properties, and you can do extension classes on the enumeration
            // and do something along the lines of MowerDirection.RotateToNext().
            // In the interest of time I'll do this a simpler way.
            Direction =
                Direction == MowerDirection.Up    ? MowerDirection.Right :
                Direction == MowerDirection.Right ? MowerDirection.Down :
                Direction == MowerDirection.Down  ? MowerDirection.Left :
                                                    MowerDirection.Up;
        }

        private void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
