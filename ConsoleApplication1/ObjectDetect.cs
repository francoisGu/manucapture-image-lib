using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace ConsoleApplication1
{
    /// <summary>
    /// This Class is to detect the object in the image
    /// Implemented by Frank Gu, 23/AUG/2015
    /// </summary>
    public class ObjectDetect
    {
        Image<Bgr, Byte> img;
        Image<Gray, Byte> gray_image;
        Image<Bgr, Byte> image_copy;

        CircleF[] circles;
        LineSegment2D[] lines;
        List<Point[]> polygons;

        double cannyThreshold = 180.0;
        double cannyThresholdLinking = 120;
        double circleAccumulatorThreshold = 120;

        //Construction
        public ObjectDetect(String fileName)
        {
            
            polygons = new List<Point[]>();
            LoadImg(fileName);

        }
        public ObjectDetect()
        {
            polygons = new List<Point[]>();
        }

        public void SaveShapeImg(String outPutFileName)
        {
            DrawShapes();
            image_copy.ToBitmap().Save(outPutFileName);
        }


        public void LoadImg(string fileName)
        {
            
            img = new Image<Bgr, byte>(fileName)
                .Resize(400, 400, Emgu.CV.CvEnum.Inter.Linear, true);
            gray_image = img.Convert<Gray, Byte>();
            image_copy = img.Copy();
            PerformShapeDetection();

            /*
            #region test
            //Convert the image to grayscale and filter out the noise
            UMat uimage = new UMat();
            CvInvoke.CvtColor(img, uimage, ColorConversion.Bgr2Gray);

            uimage.Save("gray.jpg");

            #region Canny
            //get Canny edge image
            UMat cannyEdges = new UMat();
            //cannyThreshold
            CvInvoke.Canny(uimage, cannyEdges, 50, 150);
            #endregion
            cannyEdges.Save("canny.jpg");
            #endregion
            */
        }

        private void PerformShapeDetection()
        {
            if(img != null)
            {
                //Convert the image to grayscale and filter out the noise
                UMat uimage = new UMat();
                CvInvoke.CvtColor(img, uimage, ColorConversion.Bgr2Gray);

                uimage.Save("gray.jpg");

                //use image pyr to remove noise
                UMat pyrDown = new UMat();
                CvInvoke.PyrDown(uimage, pyrDown);
                CvInvoke.PyrUp(pyrDown, uimage);

                uimage.Save("reduceNoice.jpg");

                #region circle detection
                circles = CvInvoke.HoughCircles(
                    uimage,//inputfile
                    HoughType.Gradient,
                    2.0, //Resolution of the accumulator used to detect centers of the circles.
                    20.0,
                    cannyThreshold,
                    circleAccumulatorThreshold,
                    5);
                #endregion

                #region Canny
                //get Canny edge image
                UMat cannyEdges = new UMat();
                //cannyThreshold
                CvInvoke.Canny(uimage, cannyEdges, 150, 150);
                #endregion

                cannyEdges.Save("canny.jpg");

                #region Line detection
                //detect lines
                lines = CvInvoke.HoughLinesP(
                   cannyEdges,
                   3, //Distance resolution in pixel-related units
                   Math.PI / 45.0, //Angle resolution measured in radians.
                   20, //threshold
                   30, //min Line width
                   10); //gap between lines
                #endregion

                #region Polygon Detection
                using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
                {
                    CvInvoke.FindContours(
                        cannyEdges, 
                        contours, 
                        null, 
                        RetrType.List, 
                        ChainApproxMethod.ChainApproxSimple);
                    int count = contours.Size;
                    for (int i = 0; i < count; i++)
                    {
                        //get one of the contours from the list
                        using (VectorOfPoint contour = contours[i])
                        using (VectorOfPoint approxContour = new VectorOfPoint())
                        {
                            //Approximates a polygonal curve
                            CvInvoke.ApproxPolyDP(
                                contour,
                                approxContour,
                                CvInvoke.ArcLength(contour, true) * 0.05, //calculate the perimeter
                                true);
                            //only consider contours with area greater than 250
                            if (CvInvoke.ContourArea(approxContour, false) > 250)
                            {
                                int vertices = approxContour.Size;
                                Point[] pts = new Point[vertices];
                                pts = approxContour.ToArray();
                                polygons.Add(pts);
                            }
                        }
                    }

                }
                #endregion
            }
        }


        /// <summary>
        /// Draw finding shapes
        /// </summary>
        public void DrawShapes()
        {
            #region draw circles
            //Mat circleImage = new Mat(img.Size, DepthType.Cv8U, 3);
            //circleImage.SetTo(new MCvScalar(0));
            if(circles != null)
            {
                foreach (CircleF circle in circles)
                    CvInvoke.Circle(
                        image_copy,
                        Point.Round(circle.Center),
                        (int)circle.Radius,
                        new Bgr(Color.Brown).MCvScalar,
                        2);
            }
            //circleImageBox.Image = circleImage;
            #endregion

            #region draw lines
            //Mat lineImage = new Mat(img.Size, DepthType.Cv8U, 3);
            //lineImage.SetTo(new MCvScalar(0));
            if(lines != null)
            {
                foreach (LineSegment2D line in lines)
                    CvInvoke.Line(
                        image_copy,
                        line.P1,
                        line.P2,
                        new Bgr(Color.Green).
                        MCvScalar,
                        2);
            }

            //lineImageBox.Image = lineImage;
            #endregion

            #region draw polygons
            foreach(Point[] polygon in polygons)
                CvInvoke.Polylines(image_copy,
                    polygon, 
                    true, 
                    new Bgr(Color.DarkBlue).MCvScalar, 
                    2);
            #endregion
        }

        public void SaveTextFile(String textFileName)
        {
            StreamWriter sw = new StreamWriter(textFileName);
            #region write circles
            //Mat circleImage = new Mat(img.Size, DepthType.Cv8U, 3);
            //circleImage.SetTo(new MCvScalar(0));
            if (circles != null)
            {
                foreach (CircleF circle in circles)
                {
                    string w = "<circle>\n" +
                        "\t radius: " + circle.Radius.ToString() +
                        "\n\t center: " + circle.Center.ToString()+
                        "\n"+@"<\circle>" + "\n";
                    sw.Write(w);
                }

            }
            //circleImageBox.Image = circleImage;
            #endregion

            
            #region draw lines
            //Mat lineImage = new Mat(img.Size, DepthType.Cv8U, 3);
            //lineImage.SetTo(new MCvScalar(0));
            if (lines != null)
            {
                foreach (LineSegment2D line in lines)
                {
                    string w = "<line>\n" +
                        "\t point: " + line.P1.ToString() +
                        "\n\t point: " + line.P2.ToString() +
                        "\n"+@"<\line>" + "\n";
                    sw.Write(w);
                }

            }

            //lineImageBox.Image = lineImage;
            #endregion

            #region draw polygons
            foreach (Point[] polygon in polygons)
            {
                string w = "<polygon>";
                for(int i = 0; i < polygon.Length; i++)
                {
                    w += "\n\t point: " + polygon[i].ToString();
                }

                w += "\n" + @"<\polygon>" + "\n";
                sw.Write(w);

            }
            #endregion

            sw.Close();
        }

    }
}
