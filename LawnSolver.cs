namespace LawnRobot
{
    public enum LawnNodeType
    {
        Unvisited,
        Obstacle,
        TallGrass,
        ShortGrass,
    }

    public enum MowerDirection
    {
        Up,
        Right,
        Down,
        Left,
    }

    public interface ILawnNode
    {
        ILawnNode?   Up    { get; }
        ILawnNode?   Down  { get; }
        ILawnNode?   Left  { get; }
        ILawnNode?   Right { get; }
        LawnNodeType NodeType  { get; }
        int          X     { get; }
        int          Y     { get; }
    }

    public interface IMower
    {
        ILawnNode      Location  { get; }
        MowerDirection Direction { get; }

        bool Go();
        void Mow();
        void TurnRight();
    }

    public class LawnSolver
    {
        private IMower? _Mower;

        public LawnSolver(IMower? mower)
        {
            _Mower = mower;
        }

        public void Step()
        {
            // Mow where we are at if we can.
            ILawnNode currentLocation = _Mower.Location;
            if (currentLocation.NodeType == LawnNodeType.TallGrass)
            {
                _Mower.Mow();
                return;
            }
        }
    }
}
