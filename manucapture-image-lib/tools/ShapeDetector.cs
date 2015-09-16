using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ManucaptureImageLib.tools;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using AForge;
using ManucaptureImageLib.classes;
using System.Collections;
using CsPotrace;

namespace ManucaptureImageLib.tools
{
    public class ShapeDetector
    {
        public readonly Stream ImageStream;        
        public readonly float TH;
        public readonly float TL;
        public readonly float Sigma;
        public readonly int MaskSize;
        public readonly int BlobFrameSize;
        public readonly float MinAcceptableDistortion;
        public readonly float RelativeDistortionLimit;
        public readonly int IgnoreArea;
        public readonly double Tolerance;
        public readonly double CornerThreshold;
        public readonly bool Optimizing;
        public readonly int VectorisationThreshold;

        public List<Shape> ResultShapeList { get; set; }

        private Bitmap Image { get; set; }

        private static System.Globalization.CultureInfo enUsCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
        public ShapeDetector(Stream ImageStream, float TH, float TL, float Sigma, int MaskSize, 
            int BlobFrameSize, float MinAcceptableDistortion, float RelativeDistortionLimit,
            int IgnoreArea, double Tolerance, double CornerThreshold, bool Optimizing, int VectorisationThreshold)
        {
		    // Required parameters
            this.ImageStream = ImageStream;
            // optional paramaters
            this.TH = TH;
            this.TL = TL;
            this.Sigma = Sigma;
            this.MaskSize = MaskSize;
            this.BlobFrameSize = BlobFrameSize;
            this.MinAcceptableDistortion = MinAcceptableDistortion;
            this.RelativeDistortionLimit = RelativeDistortionLimit;
            this.IgnoreArea = IgnoreArea;
            this.Tolerance = Tolerance;
            this.CornerThreshold = CornerThreshold;
            this.Optimizing = Optimizing;
            this.VectorisationThreshold = VectorisationThreshold;

            List<Shape> ResultShapeList = null;
            this.Image = SetImage(this.ImageStream);
            
            if (applyCannyEdgeFilter())
            {
                ResultShapeList = recogniseObjects();
            } else
            {
                Console.WriteLine("ERROR - 1 :-> EDGE FILTERING");
            }
            this.ResultShapeList = ResultShapeList;
	    }
        
        private Bitmap SetImage(Stream ImageStream)
        {
            return Helper.BytesToBitmap(ImageStream);
        }

        #region canny edge filter
        public bool applyCannyEdgeFilter()
        {
            if (this.Image != null)
            {
                try
                {
                    byte[] imageSize = Helper.ImageToByte2(this.Image);

                    //canny edge filter threshold filters
                    
                    System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(new MemoryStream(imageSize));
                    Canny CannyData = new Canny((System.Drawing.Bitmap)bmp, this.TH, this.TL, this.MaskSize, this.Sigma);

                    this.Image = CannyData.DisplayImage(CannyData.EdgeMap);
                    //currentImage saved to a temporary tempCanny file, but it is just for testing not used in the project
                    this.Image.Save("C:\\development\\Images\\sample\\tempCanny.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                catch (Exception exc)
                {
                    Console.WriteLine("ERROR - 2 :-> " + exc.ToString());
                }
                return true;
            }
            else
            {
                return false;
            }
            
        }
        #endregion 

        #region recognise objects
        public List<Shape> recogniseObjects()
        {

            List<Shape> resultShapeList = new List<Shape>();

            if (this.Image != null)
            {
                try
                {
                    Bitmap resImage = new Bitmap(this.Image.Width, this.Image.Height);
                    BitmapData bmData = this.Image.LockBits(
                        new Rectangle(0, 0, this.Image.Width, this.Image.Height),
                        ImageLockMode.ReadWrite, this.Image.PixelFormat);

                    BlobCounter blobCounter = new BlobCounter();

                    blobCounter.FilterBlobs = true;
                    blobCounter.MinHeight = this.BlobFrameSize;
                    blobCounter.MinWidth = this.BlobFrameSize;
                    blobCounter.ProcessImage(bmData);
                    Blob[] blobs = blobCounter.GetObjectsInformation();
                    Pen pen = new Pen(Color.Black);
                    this.Image.UnlockBits(bmData);

                    SimpleShapeChecker shapeChecker = new SimpleShapeChecker();
                    Graphics g = Graphics.FromImage(resImage);

                    for (int i = 0, n = blobs.Length; i < n; i++)
                    {
                        List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blobs[i]);
                        AForge.Point center;
                        float radius;
                        List<IntPoint> corners;

                        //calls the custom IsCircle method, shapeChecker.isCircle() method can be used for normal thresholds
                        if (IsCircle(edgePoints, out center, out radius))
                        {
                            int x = (int)center.X;
                            int y = (int)center.Y;
                            resultShapeList.Add(new ManucaptureImageLib.classes.Circle(x, y, radius));
                        }
                        else if (shapeChecker.IsTriangle(edgePoints, out corners) 
                                        || shapeChecker.IsQuadrilateral(edgePoints, out corners))
                        {
                            
                            List<ManucaptureImageLib.classes.Point> points = Helper.ToPointsArray(corners);
                            ManucaptureImageLib.classes.Polygon polygon = new ManucaptureImageLib.classes.Polygon(points);
                            resultShapeList.Add(polygon);
                        }
                        else
                        {
                            String pathStr = recognisePath(edgePoints);
                            Console.WriteLine(pathStr);
                            System.IO.File.WriteAllText(@"C:\development\Images\sample\PathLines_"+i+".svg", pathStr);
                            ManucaptureImageLib.classes.Path path = new ManucaptureImageLib.classes.Path(pathStr);
                            resultShapeList.Add(path);
                        }
                    }
                    

                }
                catch (Exception exc)
                {
                    Console.WriteLine("ERROR - 3 :-> " + exc.ToString());
                }
            }
            return resultShapeList;
        }
        #endregion

        #region canny edge filter - 2
        public Bitmap applyCannyEdgeFilter2(Bitmap BmpInput)
        {
            
            try
            {
                byte[] imageSize = Helper.ImageToByte2(BmpInput);

                //canny edge filter threshold filters

                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(new MemoryStream(imageSize));
                Canny CannyData = new Canny((System.Drawing.Bitmap)bmp, this.TH, this.TL, this.MaskSize, this.Sigma);

                BmpInput = CannyData.DisplayImage(CannyData.EdgeMap);
                //currentImage saved to a temporary tempCanny file, but it is just for testing not used in the project
                BmpInput.Save("C:\\development\\Images\\sample\\tempCanny2.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (Exception exc)
            {
                Console.WriteLine("ERROR - 2 :-> " + exc.ToString());
            }
            
            return BmpInput;
            

        }
        #endregion 

        #region potrace, recognisePath
        public String recognisePath(List<IntPoint> Corners)
        {
            
            bool[,] Matrix;
            ArrayList ListOfCurveArray;
        
            Potrace.turdsize = Convert.ToInt32(IgnoreArea);

            try{
                Potrace.alphamax = Convert.ToDouble(Tolerance);
                Potrace.opttolerance = Convert.ToDouble(CornerThreshold);
            }catch(Exception e){

            }
            
            Potrace.curveoptimizing = Optimizing;
            Bitmap VectorisedImage = createBitmapFromBlob(Corners);
            ListOfCurveArray = new ArrayList();

            Matrix = Potrace.BitMapToBinary(VectorisedImage, VectorisationThreshold);
            Potrace.potrace_trace(Matrix, ListOfCurveArray);
            VectorisedImage.Save("C:\\development\\Images\\sample\\bmpManuc.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            String s = export2SVG2(ListOfCurveArray, VectorisedImage.Width, VectorisedImage.Height);
            
            return s;
        }
        #endregion

        #region export2SVG, converts only paths
        private static String export2SVG(ArrayList Fig, int Width, int Height)
        {
            StringBuilder svg = new StringBuilder();
            string head = "";
            svg.AppendLine(head);

            foreach (ArrayList Path in Fig)
            {

                bool isContour = true;
                svg.Append("<path d='");
                for (int i = 0; i < Path.Count; i++)
                {
                    Potrace.Curve[] Curves = (Potrace.Curve[])Path[i];

                    string curve_path = isContour ? GetContourPath(Curves) : GetHolePath(Curves);



                    if (i == Path.Count - 1)
                    {
                        svg.Append(curve_path);
                    }
                    else
                    {
                        svg.AppendLine(curve_path);
                    }

                    isContour = false;

                }
                svg.AppendLine("' />");

            }



            string foot = "";
            svg.AppendLine(foot);
            svg.Replace("'", "\"");


            return svg.ToString();
        }
        #endregion

        #region export to SVG method-2, includes <svg> headers, as original in portrace
        public static string export2SVG2(ArrayList Fig, int Width, int Height)
        {


            StringBuilder svg = new StringBuilder();
            string head = String.Format(@"<?xml version='1.0' standalone='no'?>
<!DOCTYPE svg PUBLIC '-//W3C//DTD SVG 20010904//EN' 
'http://www.w3.org/TR/2001/REC-SVG-20010904/DTD/svg10.dtd'>
<svg version='1.0' xmlns='http://www.w3.org/2000/svg' preserveAspectRatio='xMidYMid meet'
width='{0}' height='{1}'  viewBox='0 0 {0} {1}'>
<g>", Width, Height);
            svg.AppendLine(head);

            foreach (ArrayList Path in Fig)
            {

                bool isContour = true;
                svg.Append("<path d='");
                for (int i = 0; i < Path.Count; i++)
                {
                    Potrace.Curve[] Curves = (Potrace.Curve[])Path[i];

                    string curve_path = isContour ? GetContourPath(Curves) : GetHolePath(Curves);



                    if (i == Path.Count - 1)
                    {
                        svg.Append(curve_path);
                    }
                    else
                    {
                        svg.AppendLine(curve_path);
                    }

                    isContour = false;

                }
                svg.AppendLine("' />");

            }



            string foot = @"</g>
</svg>";
            svg.AppendLine(foot);
            svg.Replace("'", "\"");


            return svg.ToString();
        }
        #endregion

        #region create bitmap from blob
        private Bitmap createBitmapFromBlob(List<IntPoint> Corners)
        {
            int max_X = MaxPoint(Corners, true);
            int max_Y = MaxPoint(Corners, false);
            int min_X = MinPoint(Corners, true);
            int min_Y = MinPoint(Corners, false);
            Bitmap Bmp = new Bitmap(this.Image.Width, this.Image.Height);

            //this is for setting pixels exactly as in the real image, (as original)
           /* for (int i = min_X; i < max_X; i++)
            {
                for (int j = min_Y; j < max_Y; j++)
                {
                    Bmp.SetPixel(i, j, this.Image.GetPixel(i, j));
                }
            }*/

            for (int i = 0; i < Corners.Count; i++)
            {
                try
                {

                    Bmp.SetPixel(Corners[i].X, Corners[i].Y, this.Image.GetPixel(Corners[i].X, Corners[i].Y));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

            }
            Bmp.Save("C:\\development\\Images\\sample\\tempbmp.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

            return Bmp;
        }
        #endregion

        #region find max point in blob
        private int MaxPoint(List<IntPoint> Corners, bool isX)
        {
            int max = 0;
            for (int i = 0; i < Corners.Count; i++)
            {
                IntPoint ip = Corners[i];
                if (isX)
                {
                    if (max < ip.X)
                    {
                        max = ip.X;
                    }
                }
                else
                {
                    if (max < ip.Y)
                    {
                        max = ip.Y;
                    }
                }
                
                
            }
                return max;
        }
        #endregion

        #region find min point in blob
        private int MinPoint(List<IntPoint> Corners, bool isX)
        {
            int min = 0;
            if (isX)
            {
                min = this.Image.Width;
            }
            else
            {
                min = this.Image.Height;
            }
            for (int i = 0; i < Corners.Count; i++)
            {
                IntPoint ip = Corners[i];
                if (isX)
                {
                    if (min > ip.X)
                    {
                        min = ip.X;
                    }
                }
                else
                {
                    if (min > ip.Y)
                    {
                        min = ip.Y;
                    }
                }


            }
            return min;
        }
        #endregion

        #region get counter path
        private static string GetContourPath(Potrace.Curve[] Curves)
        {

            StringBuilder path = new StringBuilder();

            for (int i = 0; i < Curves.Length; i++)
            {
                Potrace.Curve Curve = Curves[i];


                if (i == 0)
                {
                    path.AppendLine("M" + Curve.A.x.ToString("0.0", enUsCulture) + " " + Curve.A.y.ToString("0.0", enUsCulture));
                }

                if (Curve.Kind == Potrace.CurveKind.Bezier)
                {

                    path.Append("C" + Curve.ControlPointA.x.ToString("0.0", enUsCulture) + " " + Curve.ControlPointA.y.ToString("0.0", enUsCulture) + " " +
                                    Curve.ControlPointB.x.ToString("0.0", enUsCulture) + " " + Curve.ControlPointB.y.ToString("0.0", enUsCulture) + " " +
                                    Curve.B.x.ToString("0.0", enUsCulture) + " " + Curve.B.y.ToString("0.0", enUsCulture));




                }
                if (Curve.Kind == Potrace.CurveKind.Line)
                {
                    path.Append("L" + Curve.B.x.ToString("0.0", enUsCulture) + " " + Curve.B.y.ToString("0.0", enUsCulture));

                }
                if (i == Curves.Length - 1)
                {
                    path.Append("Z");
                }
                else
                {
                    path.AppendLine("");
                }


            }



            return path.ToString();

        }

        #endregion

        #region get hole path
        private static string GetHolePath(Potrace.Curve[] Curves)
        {
            StringBuilder path = new StringBuilder();

            for (int i = Curves.Length - 1; i >= 0; i--)
            {
                Potrace.Curve Curve = Curves[i];


                if (i == Curves.Length - 1)
                {
                    path.AppendLine("M" + Curve.B.x.ToString("0.0", enUsCulture) + " " + Curve.B.y.ToString("0.0", enUsCulture));
                }

                if (Curve.Kind == Potrace.CurveKind.Bezier)
                {



                    path.Append("C" + Curve.ControlPointB.x.ToString("0.0", enUsCulture) + " " + Curve.ControlPointB.y.ToString("0.0", enUsCulture) + " " +
                                  Curve.ControlPointA.x.ToString("0.0", enUsCulture) + " " + Curve.ControlPointA.y.ToString("0.0", enUsCulture) + " " +
                                  Curve.A.x.ToString("0.0", enUsCulture) + " " + Curve.A.y.ToString("0.0", enUsCulture));



                }
                if (Curve.Kind == Potrace.CurveKind.Line)
                {
                    path.Append("L" + Curve.B.x.ToString("0.0", enUsCulture) + " " + Curve.B.y.ToString("0.0", enUsCulture));

                }
                if (i == 0)
                {
                    path.Append("Z");
                }
                else
                {
                    path.AppendLine("");
                }

            }



            return path.ToString();
        }
        #endregion

        #region a custom method for recognising circles
        private bool IsCircle(List<IntPoint> edgePoints, out AForge.Point center, out float radius)
        {
            //the parameters which you can arrange threshold
            float minAcceptableDistortion = this.MinAcceptableDistortion;
            float relativeDistortionLimit = this.RelativeDistortionLimit;

            // make sure we have at least 8 points for curcle shape
            if (edgePoints.Count < 8)
            {
                center = new AForge.Point(0, 0);
                radius = 0;
                return false;
            }

            IntPoint minXY, maxXY;
            PointsCloud.GetBoundingRectangle(edgePoints, out minXY, out maxXY);
            IntPoint cloudSize = maxXY - minXY;
            center = minXY + (AForge.Point)cloudSize / 2;

            radius = ((float)cloudSize.X + cloudSize.Y) / 4;

            float meanDistance = 0;

            for (int i = 0, n = edgePoints.Count; i < n; i++)
            {
                meanDistance += (float)Math.Abs(center.DistanceTo(edgePoints[i]) - radius);
            }
            meanDistance /= edgePoints.Count;

            float maxDitance = Math.Max(minAcceptableDistortion,
                ((float)cloudSize.X + cloudSize.Y) / 2 * relativeDistortionLimit);

            return (meanDistance <= maxDitance);
        }
        #endregion
    }
}
