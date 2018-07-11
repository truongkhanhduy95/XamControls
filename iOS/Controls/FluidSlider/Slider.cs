using System;
using CoreGraphics;
using CoreImage;
using Foundation;
using UIKit;

namespace XamControls.iOS.Controls
{
    public class Slider : UIControl
    {
        private float kBlurRadiusDefault = 12;
        private float kBlurRadiusIphonePlus = 18;

        private bool IsAnimationAllowed()
        {
            //Check if simulator
            //Check if is under highload
            return true;
        }

        private UIView contentView = new UIView();
        private ValueView valueView = new ValueView();

        private UILabel minimumLabel = new UILabel();
        private UILabel maximumLabel = new UILabel();

        private UIImageView filterView = new UIImageView();
        private MetaballFilter filter = new MetaballFilter();
        private UIImage filterViewMask = new UIImage();
        private CIContext context = new CIContext(new CIContextOptions());

        private bool isSliderTracking = false;

        public Action<Slider> DidBeginTracking;
        public Action<Slider> DidEndTracking;

        private UIColor contentViewColor;
        public UIColor ContentViewColor
        {
            get { return contentViewColor; }
            set 
            {
                contentViewColor = value;
                UpdateValueViewColor();
                SetNeedsLayout();
            }
        }

        private UIColor valueViewColor;
        public UIColor ValueViewColor
        {
            get { return contentViewColor; }
            set
            {
                valueViewColor = value;
                UpdateValueViewColor();
                SetNeedsLayout();
            }
        }


        private float fraction;
        public float Fraction
        {
            get { return fraction; }
            set
            {
                fraction = value;
                UpdateValueViewText();
                SetNeedsLayout();
            }
        }

        private bool showFractionOnlyWhileTracking;
        public bool ShowFractionOnlyWhileTracking
        {
            get { return showFractionOnlyWhileTracking; }
            set 
            {
                showFractionOnlyWhileTracking = value;
                UpdateValueViewText();
            }
        }

        
        public Slider()
        {
            Initialize();
        }

        public Slider(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        private void Initialize()
        {
            contentView.Frame = Bounds;
            contentView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            contentView.UserInteractionEnabled = false;
            AddSubview(contentView);

            contentView.AddSubview(minimumLabel);
            contentView.AddSubview(maximumLabel);
            contentView.AddSubview(valueView);
            valueView.AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleRightMargin;
            valueView.UserInteractionEnabled = false;
            valueView.AnimationFrame = RedrawFilterView;

            SetMinimumLabelAttributedText(new NSAttributedString("0"));
            SetMaximumLabelAttributedText(new NSAttributedString("1"));

            UpdateValueViewColor();
            UpdateValueViewText();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            filterViewMask = null;

            if(filterView != null) //TODO: check
                filterView.MaskView.Frame = filterView.Bounds;

            //layoutBackgroundImage()

            //layoutImageViews()
            //layoutLabelsText()
            //layoutValueView()
        }

        private void UpdateValueViewColor()
        {
            valueView.OuterFillColor = contentViewColor;
            valueView.InnerFillColor = valueViewColor;
        }

        private void UpdateValueViewText()
        { 
            if(!showFractionOnlyWhileTracking || isSliderTracking)
            {
                var text = AttributedTextForFraction(fraction);
                valueView.AttributeText = text;
            }
            else {
                valueView.AttributeText = null;
            }
        }

        public void SetMinimumLabelAttributedText(NSAttributedString attr)
        {
            minimumLabel.AttributedText = attr;
            SetNeedsLayout();
        }

        public void SetMaximumLabelAttributedText(NSAttributedString attr)
        {
            maximumLabel.AttributedText = attr;
            SetNeedsLayout();
        }

        public NSAttributedString AttributedTextForFraction(float frag)
        {
            var formatter = new NSNumberFormatter();
            formatter.MaximumFractionDigits = 2;
            formatter.MaximumIntegerDigits = 0;
            var str = formatter.StringFromNumber((NSNumber)frag) ?? "";
            return new NSAttributedString(str);
        }

        private void RedrawFilterView()
        {
            var isAnimationEnabled = IsAnimationAllowed();
            if (!isAnimationEnabled) return;

            var scale = UIScreen.MainScreen.Scale;
            var radius = (float)(UIScreen.MainScreen.Bounds.Width >= 414 ? kBlurRadiusIphonePlus : kBlurRadiusDefault);
            var bottomMargin = 10f;
            var offsetY = -contentView.Bounds.Height / 2;
            var bounds = new CGRect(valueView.Frame.X, offsetY, valueView.Frame.Size.Width, -offsetY + bottomMargin).Inset(-radius, 0);

            var inputImage = new UIGraphicsImageRenderer(Bounds.Size).CreateImage(
                (obj) => contentView.Layer.RenderInContext(obj.CGContext)
            );

            filter.BlurRadius = radius;
            filter.Threshold = 0.49f;
            filter.BackgroundColor = contentViewColor;
            filter.AntialiasingRadius = (float)(scale / 2);
            filter.InputImage = new CIImage(inputImage.CGImage);

            var outputImage = filter.OutputImage.ImageByCroppingToRect(
                new CGRect(0, 0, inputImage.Size.Width * scale, inputImage.Size.Height * scale));
            var cgImage = context.CreateCGImage(outputImage, outputImage.Extent);

            filterView.Image = UIImage.FromImage(cgImage, scale, UIImageOrientation.Up);
            filterView.Frame = bounds;

            if(filterViewMask == null)
            {
                var renderer = new UIGraphicsImageRenderer(new CGRect(new CGPoint(),Bounds.Size).Size);
                filterViewMask = renderer.CreateImage((ctx) =>
                {
                    UIColor.White.SetFill();
                    ctx.FillRect(new CGRect(new CGPoint(), bounds.Size));
                    ctx.CGContext.ClearRect(new CGRect(0, bounds.Size.Height - bottomMargin, radius, bottomMargin));
                    ctx.CGContext.ClearRect(new CGRect(bounds.Size.Width - radius, bounds.Size.Height - bottomMargin, radius, bottomMargin));
                });
                ((UIImageView)filterView.MaskView).Image = filterViewMask;
            }
        }
    }
}
