using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using ManucaptureImageLib.tools;
using ManucaptureImageLib.classes;

namespace ManucaptureImageLib
{
    public class ImageLib
    {

        Bitmap input;

        ShapeDetector shapeDetector;

        public ImageLib()
        {
            this.shapeDetector = new ShapeDetector();
        }

        public void setImageStream(MemoryStream imageStream)
        {
            this.input = Helper.BytesToBitmap(imageStream);
        }

        //input image as a file, for testing
        public void setImageFile(Bitmap image)
        {
            this.input = image;
        }

        //optional method for setting canny edge detection threshold
        public void setCannyThreshold(float TH, float TL, float Sigma, int MaskSize)
        {
            this.shapeDetector.TH = TH;
            this.shapeDetector.TL = TL;
            this.shapeDetector.Sigma = Sigma;
            this.shapeDetector.MaskSize = MaskSize;
        }
        public void setBlobSize(int BlobFrameSize)
        {
            this.shapeDetector.BlobFrameSize = BlobFrameSize;
        }
        public void setCircleThreshold(float minAcceptableDistortion, float relativeDistortionLimit)
        {
            this.shapeDetector.MinAcceptableDistortion = minAcceptableDistortion;
            this.shapeDetector.RelativeDistortionLimit = relativeDistortionLimit;
        }

        public List<Shape> recogniseObjects()
        {
            this.shapeDetector.applyCannyEdgeFilter(this.input);
            List<Shape> resultShapeList = this.shapeDetector.recogniseObjects(this.input);
            return resultShapeList;
        }

    }
}
