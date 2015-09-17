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
    public class ShapeDetectorBuilder : IShapeDetectorBuilder
    {
        //required parameter
        public readonly Stream ImageStream;

        public List<Shape> ResultShapeList = null;

        //public ShapeDetector ShapeDetector;

        //optional parameters
        public float TH = Utils.DEFAULT_TH;

        public float TL = Utils.DEFAULT_TL;

        public float Sigma = Utils.DEFAULT_SIGMA;

        public int MaskSize = Utils.DEFAULT_MASK_SIZE;

        public int BlobFrameSize = Utils.DEFAULT_BLOB_FRAME_SIZE;

        public float MinAcceptableDistortion = Utils.DEFAULT_MIN_ACCEPTABLE_DISTORTION;

        public float RelativeDistortionLimit = Utils.DEFAULT_RELATIVE_DISTORTION_LIMIT;

        public int IgnoreArea = Utils.DEFAULT_IGNORE_AREA;

        public double Tolerance = Utils.DEFAULT_TOLERANCE;

        public double CornerThreshold = Utils.DEFAULT_CORNER_THRESHOLD;

        public bool Optimizing = Utils.DEFAULT_OPTIMIZING;

        public int VectorisationThreshold = Utils.DEFAULT_VECTORISATION_THRESHOLD;

        public ShapeDetectorBuilder(Stream ImageStream)
        {
            //this.ShapeDetector = new ShapeDetector();
            this.ImageStream = ImageStream;
        }

        //optional method for setting canny edge detection threshold
        public ShapeDetectorBuilder SetEdgeDetectionThreshold(float TH, float TL, float Sigma, int MaskSize)
        {
            this.TH = TH;
            this.TL = TL;
            this.Sigma = Sigma;
            this.MaskSize = MaskSize;
            return this;
        }
        public ShapeDetectorBuilder SetMinBlobSize(int BlobFrameSize)
        {
            this.BlobFrameSize = BlobFrameSize;
            return this;
        }
        public ShapeDetectorBuilder SetCircleThreshold(float MinAcceptableDistortion, float RelativeDistortionLimit)
        {
            this.MinAcceptableDistortion = MinAcceptableDistortion;
            this.RelativeDistortionLimit = RelativeDistortionLimit;
            return this;
        }
        public ShapeDetectorBuilder SetPathFinderThreshold(int IgnoreArea, double Tolerance, double CornerThreshold, 
                                                                        bool Optimizing, int VectorisationThreshold)
        {
            this.IgnoreArea = IgnoreArea;
            this.Tolerance = Tolerance;
            this.CornerThreshold = CornerThreshold;
            this.Optimizing = Optimizing;
            this.VectorisationThreshold = VectorisationThreshold;
            return this;
        }
        public ShapeDetector build()
        {
            return new ShapeDetector(this.ImageStream, this.TH, this.TL, this.Sigma, this.MaskSize,
                this.BlobFrameSize, this.MinAcceptableDistortion, this.RelativeDistortionLimit, this.IgnoreArea,
                this.Tolerance, this.CornerThreshold, this.Optimizing, this.VectorisationThreshold);
        }

        public List<Shape> GetShapes()
        {
            return this.ResultShapeList;
        }

    }
}
