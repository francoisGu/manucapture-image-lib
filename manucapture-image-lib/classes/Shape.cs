using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManucaptureImageLib.classes
{
    public class Shape
    {
        public Shape()
        {

        }
        public BoundaryType BoundaryType { get; set; }

    }
    public enum BoundaryType
    {
        Exterior = 1,
        Interior = 2
    }
}
