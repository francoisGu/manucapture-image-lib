using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManucaptureImageLib.tools
{
    static class Utils
    {
        //canny edge detection threshold parameters
        public static readonly float DEFAULT_TH = 45;

        public static readonly float DEFAULT_TL = 20;

        public static readonly float DEFAULT_SIGMA = 50;

        public static readonly int DEFAULT_MASK_SIZE = 5;

        public static readonly int DEFAULT_BLOB_FRAME_SIZE = 10;

        public static readonly float DEFAULT_MIN_ACCEPTABLE_DISTORTION = 0.5f;

        public static readonly float DEFAULT_RELATIVE_DISTORTION_LIMIT = 0.1f;
    }
}
