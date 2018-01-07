using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mazesolver
{
    public class Maze
    {
        public int width;
        public int height;
        public bool[,] path;
        public bool[,] visited;
        public List<Node> result;
        public int count;
        public Node Start;
        public Node End;
        public Stopwatch s;

        public Maze(string inputfile)
        {
            s = new Stopwatch();
            s.Start();
            Bitmap img = new Bitmap(Image.FromFile(Path.GetFullPath(inputfile)));
            s.Stop();
            Console.WriteLine(s.Elapsed.TotalMilliseconds + " ms to load image");
            s.Reset();
            s.Start();
            BitmapData bData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
            width = img.Width;
            height = img.Height;
            path = new bool[width, height];
            visited = new bool[width, height];

            //Converting the image to a boolean array

            /*the size of the image in bytes */
            int size = bData.Stride * bData.Height;

            /*Allocate buffer for image*/
            byte[] data = new byte[size];

            /*This overload copies data of /size/ into /data/ from location specified (/Scan0/)*/
            System.Runtime.InteropServices.Marshal.Copy(bData.Scan0, data, 0, size);

            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    path[x, y] = data[4 * ((y * img.Width) + x)] == 255 ? true : false;
                }
            }
            img.UnlockBits(bData);
            s.Stop();
            Console.WriteLine(s.Elapsed.TotalMilliseconds + " ms to convert to bool array");
            s.Reset();


            // make nodes
            s.Start();
            Node[] topnodes = new Node[width];
            int count = 0;
            for (int x = 1; x < width - 1; x++)
            {
                if (path[x, 0])
                {
                    Start = new Node(x, 0);
                    topnodes[x] = Start;
                    count++;
                }
            }


            bool prev;
            bool curr;
            bool next;
            int above;
            int below;
            Node leftnode = new Node(0, 0);
            for (int y = 1; y < height - 1; y++)
            {
                prev = false;
                curr = false;
                next = path[1, y];

                above = y - 1;
                below = y + 1;
                for (int x = 1; x < width - 1; x++)
                {
                    prev = curr;
                    curr = next;
                    next = path[x + 1, y];
                    Node n = new Node(0, 0);
                    bool nset = false;
                    if (!curr)
                    {
                        // on wall. do nothing.
                    }
                    else
                    {
                        if (prev)
                        {
                            if (next)
                            {
                                // Path Path Path
                                if (path[x, above] || path[x, below])
                                {
                                    n = new Node(x, y);
                                    leftnode.neighbors[1] = n;
                                    n.neighbors[3] = leftnode;
                                    leftnode = n;
                                    nset = true;
                                }
                            }
                            else
                            {
                                // Path Path Wall
                                // Create Path at end of corridor
                                n = new Node(x, y);
                                leftnode.neighbors[1] = n;
                                n.neighbors[3] = leftnode;
                                leftnode = new Node(0, 0);
                                nset = true;
                            }

                        }
                        else
                        {
                            if (next)
                            {
                                // Wall Path Path
                                // Create path at start of corridor
                                n = new Node(x, y);
                                leftnode = n;
                                nset = true;
                            }
                            else
                            {
                                // Wall Path Wall
                                // Create node only if dead end
                                if (!path[x, above] || !path[x, below])
                                {
                                    n = new Node(x, y);
                                    nset = true;
                                }
                            }
                        }
                        if (nset)
                        {
                            if (path[x, above])
                            {
                                Node t = topnodes[x];
                                t.neighbors[2] = n;
                                n.neighbors[0] = t;
                            }
                            if (path[x, below])
                            {
                                topnodes[x] = n;
                            }
                            else
                            {
                                topnodes[x] = new Node(0, 0);
                            }
                            count++;
                        }
                    }
                }
            }
            for (int x = 1; x < width - 1; x++)
            {
                if (path[x, height - 1])
                {
                    End = new Node(x, height - 1);
                    Node t = topnodes[x];
                    t.neighbors[2] = End;
                    End.neighbors[0] = t;
                    count++;
                }
            }
            s.Stop();
            Console.WriteLine(s.Elapsed.TotalMilliseconds + " ms to create " + count + " nodes");
            s.Reset();
            img.Dispose();
        }

        public void printMaze()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (path[x, y])
                    {
                        Console.Write(visited[x, y] ? "▓▓" : "  ");
                    }
                    else
                    {
                        Console.Write("██");
                    }
                }
                Console.WriteLine();
            }
            Console.Read();
        }

        public void solve()
        {
            int total = width * height;
            Node[,] prev = new Node[width, height];
            int infinity = int.MaxValue - 1;
            int[,] distances = new int[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    distances[x, y] = infinity;
                }
            }
            FibHeap unvisited = new FibHeap();
            HeapNode[,] nodeindex = new HeapNode[width, height];
            distances[Start.X, Start.Y] = 0;
            HeapNode startnode = new HeapNode(0, Start);
            nodeindex[Start.X, Start.Y] = startnode;
            unvisited.insert(startnode);
            count = 0;
            while (unvisited.count > 0)
            {
                count++;
                HeapNode n = unvisited.minimum();
                unvisited.removeminimum();
                Node u = n.value;
                if (distances[u.X, u.Y] == infinity)
                {
                    break;
                }
                if (u.X == End.X && u.Y == End.Y)
                {
                    break;
                }
                foreach (Node v in u.neighbors)
                {
                    if (v.X != 0)
                    {
                        if (!visited[v.X, v.Y])
                        {
                            int d = MathExt.abs(v.X - u.X) + MathExt.abs(v.Y - u.Y);
                            int newdistance = distances[u.X, u.Y] + d;
                            int remaining = MathExt.abs(v.X - End.X) + MathExt.abs(v.Y - End.Y);
                            if (newdistance < distances[v.X, v.Y])
                            {
                                HeapNode vnode = nodeindex[v.X, v.Y];
                                if (vnode == null)
                                {
                                    vnode = new HeapNode(newdistance + remaining, v);
                                    unvisited.insert(vnode);
                                    nodeindex[v.X, v.Y] = vnode;
                                    distances[v.X, v.Y] = newdistance;
                                    prev[v.X, v.Y] = u;
                                }
                                else
                                {
                                    unvisited.decreasekey(vnode, newdistance + remaining);
                                    distances[v.X, v.Y] = newdistance;
                                    prev[v.X, v.Y] = u;
                                }
                            }
                        }
                    }
                }
                visited[u.X, u.Y] = true;
            }
            result = new List<Node>();
            Node current = End;
            while (current.X != 0)
            {
                result.Insert(0, current);
                current = prev[current.X, current.Y];
            }
        }
    }
}
