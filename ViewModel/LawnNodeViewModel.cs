using LawnRobot.Model;

namespace LawnRobot.ViewModel
{
    internal class LawnNodeViewModel
    {
        public LawnNode        Node           { get; }
        public int             X              { get => Node.X; }
        public int             Y              { get => Node.Y; }
        public LawnDisplayType DisplayType    { get => Node.DisplayType; }
        public bool            ShowUpFence    { get => ShowUpFenceInner(); }
        public bool            ShowLeftFence  { get => ShowLeftFenceInner(); }
        public bool            ShowRightFence { get => ShowRightFenceInner(); }
        public bool            ShowDownFence  { get => ShowDownFenceInner(); }

        public LawnNodeViewModel(LawnNode node)
        {
            Node = node;
        }

        private bool ShowUpFenceInner()
        {
            LawnDisplayType displayType = DisplayType;
            return
                (displayType == LawnDisplayType.FenceVertical) ||
                (displayType == LawnDisplayType.FenceEndUp) ||
                (displayType == LawnDisplayType.FenceCornerUpLeft) ||
                (displayType == LawnDisplayType.FenceCornerUpRight) ||
                (displayType == LawnDisplayType.FenceTNoLeft) ||
                (displayType == LawnDisplayType.FenceTNoRight) ||
                (displayType == LawnDisplayType.FenceTNoDown);
        }

        private bool ShowLeftFenceInner()
        {
            LawnDisplayType displayType = DisplayType;
            return
                (displayType == LawnDisplayType.FenceHorizontal) ||
                (displayType == LawnDisplayType.FenceEndLeft) ||
                (displayType == LawnDisplayType.FenceCornerUpLeft) ||
                (displayType == LawnDisplayType.FenceCornerDownLeft) ||
                (displayType == LawnDisplayType.FenceTNoUp) ||
                (displayType == LawnDisplayType.FenceTNoRight) ||
                (displayType == LawnDisplayType.FenceTNoDown);
        }

        private bool ShowRightFenceInner()
        {
            LawnDisplayType displayType = DisplayType;
            return
                (displayType == LawnDisplayType.FenceHorizontal) ||
                (displayType == LawnDisplayType.FenceEndRight) ||
                (displayType == LawnDisplayType.FenceCornerUpRight) ||
                (displayType == LawnDisplayType.FenceCornerDownRight) ||
                (displayType == LawnDisplayType.FenceTNoUp) ||
                (displayType == LawnDisplayType.FenceTNoLeft) ||
                (displayType == LawnDisplayType.FenceTNoDown);
        }

        private bool ShowDownFenceInner()
        {
            LawnDisplayType displayType = DisplayType;
            return
                (displayType == LawnDisplayType.FenceVertical) ||
                (displayType == LawnDisplayType.FenceEndDown) ||
                (displayType == LawnDisplayType.FenceCornerDownLeft) ||
                (displayType == LawnDisplayType.FenceCornerDownRight) ||
                (displayType == LawnDisplayType.FenceTNoUp) ||
                (displayType == LawnDisplayType.FenceTNoLeft) ||
                (displayType == LawnDisplayType.FenceTNoRight);
        }
    }
}
