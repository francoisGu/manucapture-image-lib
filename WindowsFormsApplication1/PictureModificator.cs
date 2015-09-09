using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Reflection;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;


namespace WindowsFormsApplication1
{
    class PictureModificator
    {
        private Bitmap currentImage;
        public int tri = 0;
        public int rect = 0;
        public int cir = 0;
        public float rad = 0;
        private FlatAnglesOptimizer shapeOptimizer = new FlatAnglesOptimizer(160);

        private float minAcceptableDistortion = 0.75f;
        private float relativeDistortionLimit = 0.05f;

        private float angleError = 7;
        private float lengthError = 0.1f;

        public PictureModificator(Bitmap currentImage)
        {
            this.currentImage = currentImage;
        }

        public PictureModificator()
        {
            this.currentImage = null;
        }

        public bool applySobelEdgeFilter()
        {
            if (currentImage != null)
            {
                try
                {
                    // create filter
                    SobelEdgeDetector filter = new SobelEdgeDetector();
                   // CannyEdgeDetector filter = new CannyEdgeDetector();
                  //  HomogenityEdgeDetector filter = new HomogenityEdgeDetector();
                    // apply the filter
                 
                    filter.ApplyInPlace(currentImage);
                    return true;
                }
                catch (Exception e)
                {

                }
            }
            return false;
        }
        
        public bool IsCircle(List<IntPoint> edgePoints, out AForge.IntPoint center, out float radius)
        {
            // make sure we have at least 8 points for curcle shape
            if (edgePoints.Count < 10)
            {
                center = new IntPoint(0, 0);
                radius = 0;
                return false;
            }

            // get bounding rectangle of the points list
            IntPoint minXY, maxXY;
            PointsCloud.GetBoundingRectangle(edgePoints, out minXY, out maxXY);
            // get cloud's size
            IntPoint cloudSize = maxXY - minXY;
            // calculate center point
            center = minXY + (IntPoint)cloudSize / 2 ;

            radius = ((float)cloudSize.X + cloudSize.Y) / 4;

            // calculate mean distance between provided edge points and estimated circle’s edge
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
        public bool applyGrayscale()
        {
            if (currentImage != null)
            {
                try
                {
                    // create grayscale filter (BT709)
                   // Grayscale filter = new Grayscale(0.2125, 0.7154, 0.0721);
                    // apply the filter
                   // Grayscale filter = new Grayscale(0.1125, 0.6154, 0.0621);
                   Grayscale filter = new Grayscale(0.0125, 0.5154, 0.0521);
                    currentImage = filter.Apply(currentImage);
                    return true;
                }
                catch (Exception e)
                { }
            }
            return false;
        }

        public bool markKnownForms()
        {
            ShapePolygon SP = ShapePolygon.Nothing;
            if (currentImage != null)
            {
                try
                {
                    Bitmap image = new Bitmap(this.currentImage);
                    // lock image
                    BitmapData bmData = image.LockBits(
                        new Rectangle(0, 0, image.Width, image.Height),
                        ImageLockMode.ReadWrite, image.PixelFormat);

                    // turn background to black
                    ColorFiltering cFilter = new ColorFiltering();
                   cFilter.Red = new IntRange(0, 80);
                   cFilter.Green = new IntRange(0, 80);
                   cFilter.Blue = new IntRange(0, 80);
                   cFilter.FillOutsideRange = false;
                   
                    cFilter.ApplyInPlace(bmData);

                    ShapePolygon spp = new ShapePolygon();
                    // locate objects
                    BlobCounter bCounter = new BlobCounter();

                    bCounter.FilterBlobs = true;
                    bCounter.MinHeight = 20;
                    bCounter.MinWidth = 20;

                    bCounter.ProcessImage(bmData);
                    Blob[] baBlobs = bCounter.GetObjectsInformation();
                    
                    image.UnlockBits(bmData);

                    // coloring objects
                    SimpleShapeChecker shapeChecker = new SimpleShapeChecker();

                    Graphics g = Graphics.FromImage(image);
                    Pen yellowPen = new Pen(Color.Yellow, 2); // circles
                    Pen redPen = new Pen(Color.Red, 5);       // quadrilateral
                    Pen brownPen = new Pen(Color.Brown, 5);   // quadrilateral with known sub-type
                    Pen greenPen = new Pen(Color.Green, 5);   // known triangle
                    Pen bluePen = new Pen(Color.Blue, 5);     // triangle
                    Pen orangePen = new Pen(Color.Orange, 5); // my triangle
                    Dictionary<PolygonSubType, Color> colors =
    new Dictionary<PolygonSubType, Color>();

                    colors.Add(PolygonSubType.Unknown, Color.White);
                    colors.Add(PolygonSubType.Trapezoid, Color.Orange);
                    colors.Add(PolygonSubType.Parallelogram, Color.Red);
                    colors.Add(PolygonSubType.Rectangle, Color.Green);
                    colors.Add(PolygonSubType.Square, Color.Blue);
                    colors.Add(PolygonSubType.Rhombus, Color.Gray);
                    //colors.Add(ShapePolygon.Pentagon, Color.Chocolate);

                    for (int i = 0, n = baBlobs.Length; i < n; i++)
                    {
                        List<IntPoint> edgePoints = bCounter.GetBlobsEdgePoints(baBlobs[i]);

                        //AForge.Point center;
                        DoublePoint center;
                        double radius;
                        List<IntPoint> corners;
                        IntPoint center2 = new IntPoint();
                        float rad2;
                        //  float _radius;

                        /*
                        // is circle ?
                        if (shapeChecker.IsCircle(edgePoints, out center, out radius))
                        {
                            g.DrawEllipse(yellowPen,
                                (float)(center.X - radius), (float)(center.Y - radius),
                                (float)(radius * 2), (float)(radius * 2));
                            cir += 1;
                        }
                         */

                        if (shapeChecker.IsCircle(edgePoints, out center, out radius))
                        {
                            Pen _pen1 = new Pen(Color.Blue);
                            string _shapeString = "" + shapeChecker.CheckShapeType(edgePoints);
                            System.Drawing.Font _font = new System.Drawing.Font("Segoe UI", 16);
                            System.Drawing.SolidBrush _brush = new System.Drawing.SolidBrush(System.Drawing.Color.Chocolate);
                            Pen _pen = new Pen(Color.GreenYellow);
                            int x = (int)center.X;
                            int y = (int)center.Y;
                            g.DrawString(_shapeString, _font, _brush, x, y);
                            g.DrawEllipse(_pen, (float)(center.X - radius),
                                                 (float)(center.Y - radius),
                                                 (float)(radius * 2),
                                                 (float)(radius * 2));
                          //  rad = (float)radius;
                            Console.WriteLine(x + " circle x , y " + y);

                        }
                        

                       else if (IsCircle(edgePoints, out center2, out rad2))
                        {
                            string _shapeString = "" + shapeChecker.CheckShapeType(edgePoints);
                            System.Drawing.Font _font = new System.Drawing.Font("Segoe UI", 16);
                            System.Drawing.SolidBrush _brush = new System.Drawing.SolidBrush(System.Drawing.Color.Aqua);
                            Pen _pen = new Pen(Color.GreenYellow);
                            int x = (int)center.X;
                            int y = (int)center.Y;
                            g.DrawString(_shapeString, _font, _brush, x, y);
                            g.DrawEllipse(_pen, (float)(center.X - rad2),
                                                 (float)(center.Y - rad2),
                                                 (float)(rad2 * 2),
                                                 (float)(rad2 * 2));
                            Console.WriteLine(x + " circle y , x " + y);
                           // rad = (float)rad;

                        }

                        /*  else if (shapeChecker.IsTriangle(edgePoints, out corners))
                          {
                              g.DrawPolygon(redPen, ToPointsArray(corners));
                              tri += 1;
                            
                          }
                          else
                          {
                             // List<IntPoint> corners;

                              // is triangle or quadrilateral
                              if (shapeChecker.IsConvexPolygon(edgePoints, out corners))
                              {
                                  PolygonSubType subType = shapeChecker.CheckPolygonSubType(corners);
                                  Pen pen;
                                  if (subType == PolygonSubType.Unknown)
                                  {
                                      pen = (corners.Count == 4) ? orangePen : bluePen;
                                    
                                      rect += 1;
                                  }
                                  else
                                  {
                                   
                                      pen = (corners.Count == 4) ? brownPen : orangePen;
                                    
                                  }

                                  g.DrawPolygon(pen, ToPointsArray(corners));
                              }
                         */
                        if (shapeChecker.IsQuadrilateral(edgePoints, out corners))
                        {
                            Pen _pen1 = new Pen(Color.Blue);
                            //MessageBox.Show(""+_shapeChecker.CheckShapeType(_edgePoint));
                            System.Drawing.Font _font = new System.Drawing.Font("Segoe UI", 16);
                            System.Drawing.SolidBrush _brush = new System.Drawing.SolidBrush(System.Drawing.Color.PaleGreen);
                            System.Drawing.Point[] _coordinates = ToPointsArray(corners);
                            if (_coordinates.Length == 4)
                            {
                                int _x = _coordinates[0].X;
                                int _y = _coordinates[0].Y;
                                Pen _pen = new Pen(Color.Brown);
                                string _shapeString = "" + shapeChecker.CheckShapeType(edgePoints);
                                g.DrawString(_shapeString, _font, _brush, _x, _y);
                                g.DrawPolygon(_pen, ToPointsArray(corners));
                            }
                            int count = 0;
                            int side1_x = 0;
                            int side1_y = 0;
                            int side2_x = 0;
                            int side2_y = 0;
                            int side3_x = 0;
                            int side3_y = 0;
                            int side4_x = 0;
                            int side4_y = 0;
                            int Side1_length = 0;
                            int Side2_length = 0;
                            int Side3_length = 0;
                            int Side4_length = 0;
                            foreach (IntPoint corner in corners)
                            {
                                count++;
                                if (count == 1)
                                {
                                    //Console.WriteLine("count is " + count);
                                    Console.WriteLine("Point "+ count +" " + corner);
                                    side1_x = corner.X;
                                    side1_y = corner.Y;
                                    
                                    

                                }
                                else if (count == 2)
                                {
                                    Console.WriteLine("Point " + count + " " + corner);
                                    side2_x = corner.X;
                                    side2_y = corner.Y;


                                }
                                    
                                else if (count == 3)
                                {
                                    Console.WriteLine("Point " + count + " " + corner);
                                    side3_x = corner.X;
                                    side3_y = corner.Y;
                                }
                                else if (count == 4)
                                {
                                    Console.WriteLine("Point " + count + " " + corner);
                                    side4_x = corner.X;
                                    side4_y = corner.Y;
                                }
                                //g.DrawRectangle(_pen1, corner.X - 1, corner.Y - 1, 3, 3);
                                
                             
                                //Console.WriteLine("length of the sides");
                               
                            }


                            Side1_length = side2_x - side1_x;
                            Side2_length = side3_y - side2_y;
                            Side3_length = side3_x - side4_x;
                            Side4_length = side4_y - side1_y;
                            Console.WriteLine("Length of side 1 " + Side1_length);
                            Console.WriteLine("Length of side 2 " + Side2_length);
                            Console.WriteLine("Length of side 3 " + Side3_length);
                            Console.WriteLine("Length of side 4 " + Side4_length);
                           
                            
                          
                            
                        }

                        if (shapeChecker.IsConvexPolygon(edgePoints, out corners))
                        {
                           
                           // MessageBox.Show(""+_shapeChecker.CheckShapeType(_edgePoint));
                            System.Drawing.Font _font = new System.Drawing.Font("Segoe UI", 16);
                            System.Drawing.SolidBrush _brush = new System.Drawing.SolidBrush(System.Drawing.Color.PaleGreen);
                            System.Drawing.Point[] _coordinates = ToPointsArray(corners);
                            if (_coordinates.Length == 5)
                            {
                                int _x = _coordinates[0].X;
                                int _y = _coordinates[0].Y;
                                Pen _pen = new Pen(Color.Brown);
                                string _shapeString = "" + shapeChecker.CheckShapeType(edgePoints);
                               // MessageBox.Show("pentagon");
                                g.DrawString(_shapeString, _font, _brush, _x, _y);
                                g.DrawPolygon(_pen, ToPointsArray(corners));
                            }
                            
                        }
                      
                        if (shapeChecker.IsTriangle(edgePoints, out corners))
                        {
                            string _shapeString = "" + shapeChecker.CheckShapeType(edgePoints);
                            System.Drawing.Font _font = new System.Drawing.Font("Segoe UI", 16);
                            System.Drawing.SolidBrush _brush = new System.Drawing.SolidBrush(System.Drawing.Color.Brown);
                            Pen _pen = new Pen(Color.GreenYellow);
                            Pen _pen1 = new Pen(Color.Blue);
                           // MessageBox.Show("corners values" + corners.Count);
                            int x = (int)center.X;
                            int y = (int)center.Y;
                            g.DrawString(_shapeString, _font, _brush, x, y);
                            g.DrawPolygon(_pen, ToPointsArray(corners));
                            foreach (IntPoint corner in corners)
                            {
                                g.DrawRectangle(_pen1, corner.X - 1, corner.Y - 1, 3, 3);
                                
                                Console.WriteLine(corner);
                            
                            }
                       
                            
                        }
                       

                    }
                    
                    

                    yellowPen.Dispose();
                    redPen.Dispose();
                    greenPen.Dispose();
                    bluePen.Dispose();
                    brownPen.Dispose();
                    g.Dispose();
                    this.currentImage = image;
                    return true;
                }
               
                catch (Exception e)
                {

                }
            }
            return false;
        }
       

        private System.Drawing.Point[] ToPointsArray(List<IntPoint> points)
        {
            System.Drawing.Point[] array = new System.Drawing.Point[points.Count];

            for (int i = 0, n = points.Count; i < n; i++)
            {
                array[i] = new System.Drawing.Point(points[i].X, points[i].Y);
            }

            return array;
        }

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