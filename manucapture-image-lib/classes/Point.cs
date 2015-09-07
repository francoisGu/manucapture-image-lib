using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManucaptureImageLib.classes
{
    public class Point
    {

        public Point(float X, float Y)
        {
            this.X = X;

            this.Y = Y;
        }

        public float X { get; set; }

        public float Y { get; set; }

    }
}
