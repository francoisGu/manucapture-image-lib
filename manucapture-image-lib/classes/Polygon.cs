using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ManucaptureImageLib.classes
{
    public class Polygon : Shape
    {
        public IList<Point> Points { get; set; }

        public Polygon(IList<Point> Points)
        {
            this.Points = Points;
        }

    }
}
