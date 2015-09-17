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


        public List<Shape> ResultShapeList { get; set; }

        private Bitmap Image { get; set; }

        public ShapeDetector(Stream ImageStream, float TH, float TL, float Sigma, int MaskSize, 
            int BlobFrameSize, float MinAcceptableDistortion, float RelativeDistortionLimit)
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

            List<Shape> ResultShapeList = null;
            this.Image = SetImage(this.ImageStream);
            //this.Image = SdBuilder.Image;
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
                    //this.Image.Save("C:\\development\\Images\\sample\\tempCanny.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
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
                            //g.DrawEllipse(pen, (x - radius), (y - radius), (radius * 2), (radius * 2));
                            resultShapeList.Add(new ManucaptureImageLib.classes.Circle(x, y, radius));
                        }
                        if (shapeChecker.IsTriangle(edgePoints, out corners) 
                                        || shapeChecker.IsQuadrilateral(edgePoints, out corners))
                        {
                            //g.DrawPolygon(pen, Helper.ToPointsArray(corners));
                            
                            List<ManucaptureImageLib.classes.Point> points = Helper.ToPointsArray(corners);
                            ManucaptureImageLib.classes.Polygon polygon = new ManucaptureImageLib.classes.Polygon(points);
                            resultShapeList.Add(polygon);
                        }
                        /*else if (shapeChecker.IsQuadrilateral(edgePoints, out corners))
                        {
                            System.Drawing.Point[] _coordinates = Helper.ToPointsArray(corners);
                            if (_coordinates.Length == 4)
                            {
                                int _x = _coordinates[0].X;
                                int _y = _coordinates[0].Y;
                                g.DrawPolygon(pen, Helper.ToPointsArray(corners));
                            }
                        }*/
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
