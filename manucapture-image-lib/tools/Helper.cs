using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using AForge;

namespace ManucaptureImageLib.tools
{
    static class Helper
    {
        public static Bitmap BytesToBitmap(Stream imageStream)
        {
            Bitmap img = (Bitmap)System.Drawing.Image.FromStream(imageStream);
            return img;
        }

        public static byte[] ImageToByte2(System.Drawing.Image img)
        {
            byte[] byteArray = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                stream.Close();
                byteArray = stream.ToArray();
            }
            return byteArray;
        }

        public static List<ManucaptureImageLib.classes.Point> ToPointsArray(List<IntPoint> corners)
        {
            List<ManucaptureImageLib.classes.Point> pointList = new List<ManucaptureImageLib.classes.Point>();

            foreach (IntPoint p in corners)
            {
                ManucaptureImageLib.classes.Point point = new ManucaptureImageLib.classes.Point(p.X, p.Y);
                pointList.Add(point);
            }
            return pointList;
        }
    }
}
