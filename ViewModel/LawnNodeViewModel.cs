using LawnRobot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawnRobot.ViewModel
{
    internal class LawnNodeViewModel
    {
        public LawnNode Node { get; }
        public int      X    { get => Node.X; }
        public int      Y    { get => Node.Y; }

        public LawnNodeViewModel(LawnNode node)
        {
            Node = node;
        }
    }
}
