using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ManucaptureImageLib.classes;

namespace ManucaptureImageLib
{
    public interface IShapeDetectorBuilder
    {
        ShapeDetectorBuilder SetEdgeDetectionThreshold(float TH, float TL, float Sigma, int MaskSize);
        ShapeDetectorBuilder SetMinBlobSize(int BlobFrameSize);
        ShapeDetectorBuilder SetCircleThreshold(float MinAcceptableDistortion, float RelativeDistortionLimit);
        List<Shape> GetShapes();

    }
}
