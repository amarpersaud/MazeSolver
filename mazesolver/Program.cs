using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace mazesolver
{
    class Program
    {
        public static string outputfile;
        public static string inputfile;
        public static Maze m;
        public static void Main(string[] args)
        {
            
            if (args.Length == 1)
            {
                outputfile = "output.png";
                inputfile = args[0];
                if (!File.Exists(inputfile))
                {
                    Console.WriteLine($"Input file not found: {inputfile}");
                    Environment.Exit(1);
                }

                if (File.Exists(outputfile))
                {
                    Console.WriteLine($"Output file already exists. Overwrite?");
                    while (true)
                    {
                        ConsoleKeyInfo cki = Console.ReadKey(false);
                        if (cki.Key == ConsoleKey.Y)
                        {
                            break;
                        }
                        if (cki.Key == ConsoleKey.N)
                        {
                            Environment.Exit(0);
                        }

                    }
                }
            }
            else if(args.Length > 1)
            {
                outputfile = args[1];
                inputfile = args[0];
                if (!File.Exists(inputfile))
                {
                    Console.WriteLine($"Input file not found: {inputfile}");
                    Environment.Exit(1);
                }

                if (File.Exists(outputfile))
                {
                    Console.WriteLine($"Output file already exists. Overwrite?");
                    while (true)
                    {
                        ConsoleKeyInfo cki = Console.ReadKey(false);
                        if(cki.Key == ConsoleKey.Y)
                        {
                            break;
                        }
                        if(cki.Key == ConsoleKey.N)
                        {
                            Environment.Exit(0);
                        }

                    }
                }
            }
            else
            {
                Console.WriteLine("Usage: mazesolver [inputfile] [outputfile](optional)");
                return;
            }
            
            m = new Maze(inputfile);

            m.s.Reset();
            m.s.Start();

            m.solve();

            m.s.Stop();

            Console.WriteLine("Finished solving maze in " + m.s.Elapsed.TotalMilliseconds + " ms. Traversed " + m.count + " nodes.");

            SaveMaze(m);
        }
        
        public static int distance(Node a, Node b)
        {
            return MathExt.abs(a.X - b.X) + MathExt.abs(a.Y - b.Y);
        }

        public static void SaveMaze(Maze m)
        {
            int maxdist = distance(m.Start, m.End);
            Bitmap bs = new Bitmap(Image.FromFile(Path.GetFullPath(inputfile)));

            Color prevcol = MathExt.getGradient(maxdist, maxdist);
            int c = 0;
            Node[] arr = m.result.ToArray();
            using (Graphics g = Graphics.FromImage(bs))
            {
                g.DrawLine(new Pen(new LinearGradientBrush(new Point(arr[0].X, arr[0].Y), new Point(arr[1].X, arr[1].Y), Color.Red, Color.Blue)), new Point(arr[0].X, arr[0].Y), new Point(arr[1].X, arr[1].Y));

                for (int i = 1; i < arr.Length; i++)
                {
                    int dist = distance(arr[i], arr[arr.Length - 1]);
                    Color scol = MathExt.getGradient(maxdist, dist);
                    //bs.SetPixel(arr[i].X, arr[i].Y, scol);
                    Point s = new Point(arr[i - 1].X, arr[i - 1].Y);
                    Point e = new Point(arr[i].X, arr[i].Y);
                    g.DrawLine(new Pen(new LinearGradientBrush(s, e, prevcol, scol)), s, e);

                    c++;

                    prevcol = scol;
                }
            }
            int scale = 1;
            Bitmap n = new Bitmap(bs.Width * scale, bs.Height * scale);
            using (Graphics g = Graphics.FromImage(n))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                //g.CompositingQuality = CompositingQuality.AssumeLinear;
                g.SmoothingMode = SmoothingMode.None;
                g.DrawImage(bs, new Rectangle(0, 0, bs.Width * scale, bs.Height * scale));
            }
            n.Save(outputfile);
        }

    }

    
    
    
}
