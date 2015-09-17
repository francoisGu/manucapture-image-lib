using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace manucapture_image_lib
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("This is Manucapture Image Library");
            /*
                String path = @"C:\development\Images\sample\";
                String img = "sample5.jpg";

                Stream fs = new FileStream(path + img, FileMode.Open, FileAccess.Read);
                ManucaptureImageLib.ShapeDetectorBuilder sdBuilder = new ManucaptureImageLib.ShapeDetectorBuilder(fs);
                sdBuilder.SetEdgeDetectionThreshold(145, 120, 150, 15);
                sdBuilder.SetMinBlobSize(40);
                sdBuilder.SetCircleThreshold(0.4f, 0.2f);
                List<ManucaptureImageLib.classes.Shape> shapeList = sdBuilder.build().ResultShapeList;
                int a = 0;
            */
        }
    }
}
