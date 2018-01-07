using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mazesolver
{
    public static class MathExt
    {
        public static int abs(int X)
        {
            return X > 0 ? X : -X;
        }
        public static Color getGradient(int max, int dist)
        {
            int r = clamp((int)(255 * (float)dist / (float)max), 0, 255);
            int g = 0;
            int b = clamp(255 - (int)(255 * (float)dist / (float)max), 0, 255);
            return Color.FromArgb(255, r, g, b);

        }
        public static int min(int a, int b)
        {
            return a < b ? a : b;
        }

        public static int max(int a, int b)
        {
            return a > b ? a : b;
        }
        public static int clamp(int x, int min, int max)
        {
            if (x > max) { return max; }
            if (x < min) { return min; }
            return x;
        }
    }
}
