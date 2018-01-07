using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mazesolver
{
    public struct Node
    {
        public int X;
        public int Y;
        public Node[] neighbors;
        public Node(int X, int Y)
        {
            this.neighbors = new Node[4];
            this.X = X;
            this.Y = Y;
        }
    }
}
