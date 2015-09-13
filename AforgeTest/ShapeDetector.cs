using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using AForge;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using System.IO;

namespace AforgeTest
{
    class ShapeDetector
    {
        private Bitmap currentImage;
        public ShapeDetector()
        {
            this.currentImage = null;
        }
        public ShapeDetector(Bitmap currentImage)
        {
            this.currentImage = currentImage;
        }
        #region Sobel edge filter --not used in current version
        public bool applySobelEdgeFilter()
        {
            if (currentImage != null)
            {
                try
                {
                    SobelEdgeDetector filter = new SobelEdgeDetector();
                    filter.ApplyInPlace(currentImage);
                    return true;
                }
                catch (Exception e)
                {

                }
            }
            return false;
        }
        #endregion


        #region canny edge filter
        public bool applyCannyEdgeFilter()
        {
            if (currentImage != null)
            {
                try
                {
                    float TH, TL, Sigma;
                    int MaskSize;
                    byte[] imageSize = ImageToByte2(currentImage);
                    
                    //canny edge filter threshold filters
                    TH = (float)Convert.ToDouble(45);
                    TL = (float)Convert.ToDouble(20);
                    MaskSize = Convert.ToInt32(5);
                    Sigma = (float)Convert.ToDouble(50);

                    System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(new MemoryStream(imageSize));
                    Canny CannyData = new Canny((System.Drawing.Bitmap)bmp, TH, TL, MaskSize, Sigma);

                    currentImage = CannyData.DisplayImage(CannyData.EdgeMap);
                    //currentImage saved to a temporary tempCanny file, but it is just for testing not used in the project
                    currentImage.Save("C:\\development\\Images\\sample\\tempCanny.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                catch (Exception exc)
                {
                    Console.WriteLine("ERROR - 2 :-> " + exc.ToString());
                }
            }
            return false;
        }
        #endregion 
        #region applyGrayScale --not used in current version
        public bool applyGrayscale()
        {
            if (currentImage != null)
            {
                try
                {
                    Grayscale filter = new Grayscale(0.2125, 0.7154, 0.0721);
                    currentImage = filter.Apply(currentImage);
                    return true;
                }
                catch (Exception e)
                { }
            }
            return false;
        }
        #endregion

        #region recognise objects
        public bool recogniseObjects()
        {

            int blobMinFrameSize = 10;

            if (currentImage != null)
            {
                try
                {
                    Bitmap image = new Bitmap(this.currentImage);
                    Bitmap resImage = new Bitmap(image.Width, image.Height);
                    BitmapData bmData = image.LockBits(
                        new Rectangle(0, 0, image.Width, image.Height),
                        ImageLockMode.ReadWrite, image.PixelFormat);

                    BlobCounter blobCounter = new BlobCounter();

                    blobCounter.FilterBlobs = true;
                    blobCounter.MinHeight = blobMinFrameSize;
                    blobCounter.MinWidth = blobMinFrameSize;
                    blobCounter.ProcessImage(bmData);
                    Blob[] blobs = blobCounter.GetObjectsInformation();
                    Pen pen = new Pen(Color.Black);
                    image.UnlockBits(bmData);

                    SimpleShapeChecker shapeChecker = new SimpleShapeChecker();
                    Graphics g = Graphics.FromImage(resImage);

                    List<IntPoint> exteriorPoint;
                    int exteriorSize = 0;

                    for (int i = 0, n = blobs.Length; i < n; i++)
                    {
                        List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blobs[i]);
                        AForge.Point center;
                        float radius;
                        List<IntPoint> corners;


                        //Get the most up down, right, left point 
                        int maxY, minY, maxX, minX;
                        maxX = minX = edgePoints[0].X;
                        maxY = minY = edgePoints[0].Y;

                        foreach(IntPoint point in edgePoints)
                        {
                            if (point.X > maxX) maxX = point.X;
                            if (point.X < minX) minX = point.X;
                            if (point.Y > maxY) maxY = point.Y;
                            if (point.Y < minY) minY = point.Y;
                        }
                        int es = (maxX - minX) * (maxY - minY);
                        if (exteriorSize < es)
                        {
                            exteriorSize = es;
                            exteriorPoint = edgePoints;
                        }


                        //calls the custom IsCircle method, shapeChecker.isCircle() method can be used for normal thresholds
                        if (IsCircle(edgePoints, out center, out radius))
                        {
                            int x = (int)center.X;
                            int y = (int)center.Y;
                            g.DrawEllipse(pen, (x - radius), (y - radius), (radius * 2), (radius * 2));
                        }
                        if (shapeChecker.IsTriangle(edgePoints, out corners))
                        {                            
                            g.DrawPolygon(pen, ToPointsArray(corners));
                        }
                        if (shapeChecker.IsQuadrilateral(edgePoints, out corners))
                        {
                            System.Drawing.Point[] _coordinates = ToPointsArray(corners);
                            if (_coordinates.Length == 4)
                            {
                                int _x = _coordinates[0].X;
                                int _y = _coordinates[0].Y;
                                g.DrawPolygon(pen, ToPointsArray(corners));
                            }
                        }
                    }
                    #region EmguCV circle detection --not used in current version
                    //this is a part of EmguCV circle detection, but it is useless

                    /*
                    int imgWidth = 400, imgHeight = 400;
                    Image<Bgr, Byte> img = new Image<Bgr, byte>(image);
                    img.Resize(imgWidth, imgHeight, Emgu.CV.CvEnum.Inter.Linear, true);

                    //Convert the image to grayscale and filter out the noise
                    UMat uimage = new UMat();
                    CvInvoke.CvtColor(img, uimage, ColorConversion.Bgr2Gray);

                    //use image pyr to remove noise
                    UMat pyrDown = new UMat();
                    CvInvoke.PyrDown(uimage, pyrDown);
                    CvInvoke.PyrUp(pyrDown, uimage);

                    double cannyThreshold = 180.0;
                    double circleAccumulatorThreshold = 120.0;
                    CircleF[] circles = CvInvoke.HoughCircles(uimage, HoughType.Gradient, 2.0, 20.0, 
                                                                cannyThreshold, circleAccumulatorThreshold, 5);

                    Image<Bgr, Byte> circleImage = img.CopyBlank();
                    foreach (CircleF circle in circles)
                    {
                        float x = (float)circle.Center.X;
                        float y = (float)circle.Center.Y;
                        float radius = circle.Radius;
                        g.DrawEllipse(pen, (x - radius), (y - radius), (radius * 2), (radius * 2));
                    }
                        //circleImage.Draw(circle, new Bgr(Color.Brown), 2);
                    //circleImageBox.Image = circleImage.ToBitmap();
                    */
                    #endregion

                    this.currentImage = resImage;
                }
                catch (Exception exc)
                {
                    Console.WriteLine("ERROR - 3 :-> " + exc.ToString());
                }
            }
            return true;
        }
        #endregion

        #region a custom method for recognising circles
        public bool IsCircle(List<IntPoint> edgePoints, out AForge.Point center, out float radius)
        {
            //the parameters which you can arrange threshold
            float minAcceptableDistortion = 0.5f;
            float relativeDistortionLimit = 0.1f;

            // make sure we have at least 8 points for curcle shape
            if (edgePoints.Count < 8 )
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

        private System.Drawing.Point[] ToPointsArray(List<IntPoint> points)
        {
            System.Drawing.Point[] array = new System.Drawing.Point[points.Count];

            for (int i = 0, n = points.Count; i < n; i++)
            {
                array[i] = new System.Drawing.Point(points[i].X, points[i].Y);
            }

            return array;
        }
        
        #region converting image to a byte array
        public byte[] ImageToByte2(System.Drawing.Image img)
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
        #endregion

        public void setCurrentImage(Bitmap currentImage)
        {
            this.currentImage = currentImage;
        }

        public Bitmap getCurrentImage()
        {
            return currentImage;
        }

    }
}
