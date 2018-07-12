using System;
using CoreImage;
using UIKit;

namespace XamControls.iOS.Controls
{
    public class MetaballFilter : CIFilter
    {
        public CIImage InputImage;

        public float BlurRadius = 12;
        public float Threshold = 0.75f;
        public UIColor BackgroundColor;
        public float AntialiasingRadius = (float)(UIScreen.MainScreen.Scale / 2);



        //public override CIImage OutputImage
        //{
        //    //var inputImage = inputImage != null ? inputImage : null;
        //    return null; //not allow image right now

        //}

        public override CIImage OutputImage 
        {
            get   
            {
                CIImage image;
                if (InputImage == null) return null;
                using(var filter = CIFilter.FromName("CIColorControls"))
                {
                    filter.Image = InputImage;
                    image = filter.OutputImage;

                    image = image?.CreateByApplyingGaussianBlur(BlurRadius);

                    if(BackgroundColor!=null)
                    {
                        
                    }
                }

                return image;
            }
        }
    }

    public class ThresholdFilter : CIFilter
    {
        public float Threshold = 0.75f;

    }
}
