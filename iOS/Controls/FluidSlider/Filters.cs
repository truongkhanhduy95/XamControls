using System;
using CoreImage;
using UIKit;

namespace XamControls.iOS.Controls
{
    public class MetaballFilter : CIFilter
    {
        private CIImage inputImage;

        private float blurRadius = 12;
        private float threshold = 0.75f;
        private UIColor backgroundColor;
        private float antialiasingRadius = (float)(UIScreen.MainScreen.Scale / 2);

        public override CIImage OutputImage
        {
            //var inputImage = inputImage != null ? inputImage : null;
            return inputImage;

        }
    }
}
