using ManucaptureImageLib.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManucaptureImageLib.classes
{
    public class Path : Shape
    {
        public String pathString { get; set; }

        public Path(String pathString)
        {
            this.pathString = pathString;
        }
    }
}
