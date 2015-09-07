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
    class ShapeDetector
    {
        public float TH { get; set; }
        public float TL { get; set; }
        public float Sigma { get; set; }
        public int MaskSize { get; set; }
        public int BlobFrameSize { get; set; }
        public float MinAcceptableDistortion { get; set; }
        public float RelativeDistortionLimit { get; set; }

        public ShapeDetector()
        {
            this.TH = Utils.DEFAULT_TH;
            this.TL = Utils.DEFAULT_TL;
            this.Sigma = Utils.DEFAULT_SIGMA;
            this.MaskSize = Utils.DEFAULT_MASK_SIZE;
            this.BlobFrameSize = Utils.DEFAULT_BLOB_FRAME_SIZE;
            this.MinAcceptableDistortion = Utils.DEFAULT_MIN_ACCEPTABLE_DISTORTION;
            this.RelativeDistortionLimit = Utils.DEFAULT_RELATIVE_DISTORTION_LIMIT;
        }

        

        #region canny edge filter
        public bool applyCannyEdgeFilter(Bitmap image)
        {
            if (image != null)
            {
                try
                {
                    float TH, TL, Sigma;
                    int MaskSize;
                    byte[] imageSize = Helper.ImageToByte2(image);

                    //canny edge filter threshold filters
                    TH = (float)Convert.ToDouble(45);
                    TL = (float)Convert.ToDouble(20);
                    MaskSize = Convert.ToInt32(5);
                    Sigma = (float)Convert.ToDouble(50);

                    System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(new MemoryStream(imageSize));
                    Canny CannyData = new Canny((System.Drawing.Bitmap)bmp, TH, TL, MaskSize, Sigma);

                    image = CannyData.DisplayImage(CannyData.EdgeMap);
                    //currentImage saved to a temporary tempCanny file, but it is just for testing not used in the project
                    image.Save("C:\\development\\Images\\sample\\tempCanny.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                catch (Exception exc)
                {
                    Console.WriteLine("ERROR - 2 :-> " + exc.ToString());
                }
            }
            return false;
        }
        #endregion 

        #region recognise objects
        public List<Shape> recogniseObjects(Bitmap image)
        {

            //int blobMinFrameSize = 10;

            List<Shape> resultShapeList = new List<Shape>();

            if (image != null)
            {
                try
                {
                    Bitmap resImage = new Bitmap(image.Width, image.Height);
                    BitmapData bmData = image.LockBits(
                        new Rectangle(0, 0, image.Width, image.Height),
                        ImageLockMode.ReadWrite, image.PixelFormat);

                    BlobCounter blobCounter = new BlobCounter();

                    blobCounter.FilterBlobs = true;
                    blobCounter.MinHeight = this.BlobFrameSize;
                    blobCounter.MinWidth = this.BlobFrameSize;
                    blobCounter.ProcessImage(bmData);
                    Blob[] blobs = blobCounter.GetObjectsInformation();
                    Pen pen = new Pen(Color.Black);
                    image.UnlockBits(bmData);

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
                            resultShapeList.Add(new Circle(x, y, radius));
                        }
                        else if (shapeChecker.IsTriangle(edgePoints, out corners) 
                                        || shapeChecker.IsQuadrilateral(edgePoints, out corners))
                        {
                            //g.DrawPolygon(pen, Helper.ToPointsArray(corners));
                            ManucaptureImageLib.classes.Polygon polygon = new ManucaptureImageLib.classes.Polygon();
                            polygon.Points = Helper.ToPointsArray(corners);
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
