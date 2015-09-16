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
            
                String path = @"C:\development\Images\sample\";
                String img = "sample4.jpg";

                Stream fs = new FileStream(path + img, FileMode.Open, FileAccess.Read);
                ManucaptureImageLib.ShapeDetectorBuilder sdBuilder = new ManucaptureImageLib.ShapeDetectorBuilder(fs);
                //sdBuilder.SetEdgeDetectionThreshold(45, 20, 50, 5);
                //sdBuilder.SetMinBlobSize(20);
                //sdBuilder.SetCircleThreshold(0.1f, 0.05f);
                //sdBuilder.SetPathFinderThreshold(2, 0.2, 1, true, 51);
                List<ManucaptureImageLib.classes.Shape> shapeList = sdBuilder.build().ResultShapeList;
                int a = 0;
            
        }
    }
}
