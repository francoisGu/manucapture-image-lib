using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManucaptureImageLib.classes
{
    public class Circle : Shape
    {
        public float CentreX { get; set; }

        public float CentreY { get; set; }

        public float Radius { get; set; }

        public Circle(float CentreX, float CentreY, float Radius)
        {
            this.CentreX = CentreX;
            this.CentreY = CentreY;
            this.Radius = Radius;
        }
    }
}
