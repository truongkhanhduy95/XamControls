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

        private float labelsMargin = 10f;
        public float LabelsMargin
        {
            get { return labelsMargin; }
            set {
                labelsMargin = value;
                LayoutLabelsText();
            }
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

        private float valueViewMargin = ValueView.kLayoutMarginInset;
        public float ValueViewMargin
        {
            get { return valueViewMargin; }
            set 
            {
                valueViewMargin = Math.Max(value, ValueView.kLayoutMarginInset);
                LayoutValueView();
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

            //LayoutImageViews();
            LayoutLabelsText();
            LayoutValueView();
        }

        private void LayoutLabelsText()
        {
            minimumLabel.SizeToFit();
            minimumLabel.Frame = new CGRect(labelsMargin,
                                            Bounds.GetMidY() - minimumLabel.Bounds.GetMidY(), 
                                            minimumLabel.Bounds.Width,
                                            minimumLabel.Bounds.Height);

            maximumLabel.SizeToFit();
            maximumLabel.Frame = new CGRect(Bounds.GetMaxX() - labelsMargin - maximumLabel.Bounds.Width,
                                            Bounds.GetMidY() - maximumLabel.Bounds.GetMidY(),
                                           maximumLabel.Bounds.Width,
                                            maximumLabel.Bounds.Height);
        }

        private void LayoutImageViews()
        {
            var imageInset = ValueView.kLayoutMarginInset * 2;
            var imageSize = new CGSize(Bounds.Height - imageInset * 2, Bounds.Height - imageInset * 2);

            throw new NotImplementedException();
        }


        private void LayoutValueView()
        {
            var rect = new CGRect(contentView.Bounds.Location, contentView.Bounds.Size);
            var bounds = new UIEdgeInsets(0, valueViewMargin, 0, valueViewMargin).InsetRect(rect);
            var centerX = fraction * bounds.Size.Width + bounds.GetMinX();

            SetValueViewPositionX(centerX);
        }

        private void SetValueViewPositionX(nfloat x)
        {
            var centerBounds = BoundsForValueViewCenter();
            var clampedX = x < centerBounds.GetMinX() ? centerBounds.GetMinX() : (centerBounds.GetMaxX() < x ? centerBounds.GetMaxX() : x);
            valueView.Frame = ValueViewFrame(clampedX);
        }


        private CGRect ValueViewFrame(nfloat centerX)
        {
            return new CGRect(centerX - Bounds.Height /2, Bounds.GetMinY(), Bounds.Height, Bounds.Height);
        }

        private CGRect BoundsForValueViewCenter()
        {
            return new UIEdgeInsets(0,
                                    valueViewMargin - ValueView.kLayoutMarginInset + valueView.Bounds.GetMidX(),
                                    0,
                                    valueViewMargin - ValueView.kLayoutMarginInset + valueView.Bounds.GetMidX())
                .InsetRect(Bounds);
        }

        #region Events

        public override bool BeginTracking(UITouch uitouch, UIEvent uievent)
        {
            var result =  base.BeginTracking(uitouch, uievent);

            var x = uitouch.LocationInView(this).X;
            isSliderTracking = true;

            fraction = FractionForPositionX(x);
            valueView.AnimateTrackingBegin();

            //Send action changed; SendAction(this.);
            DidBeginTracking?.Invoke(this);

            return result;
        }

        public override bool ContinueTracking(UITouch uitouch, UIEvent uievent)
        {
            var result =  base.ContinueTracking(uitouch, uievent);
            var x = uitouch.LocationInView(this).X;
            isSliderTracking = true;

            fraction = FractionForPositionX(x);

            filterView.Center = new CGPoint(valueView.Center.X, filterView.Center.X);
            return result;
        }

        public override void EndTracking(UITouch uitouch, UIEvent uievent)
        {
            base.EndTracking(uitouch, uievent);
            isSliderTracking = false;
            valueView.AnimateTrackingEnd();
            UpdateValueViewText();
            DidEndTracking(this);
        }

        public override void CancelTracking(UIEvent uievent)
        {
            base.CancelTracking(uievent);
            isSliderTracking = false;
            valueView.AnimateTrackingEnd();
            UpdateValueViewText();
            DidEndTracking(this);
        }

        #endregion

        private float FractionForPositionX(nfloat x)
        {
            var centerBounds = BoundsForValueViewCenter();
            var clampedX = x < centerBounds.GetMinX() ? centerBounds.GetMinX() : (centerBounds.GetMaxX() < x ? centerBounds.GetMaxX() : x);
            return (float)((clampedX - centerBounds.GetMinX()) / (centerBounds.GetMaxX() - centerBounds.GetMinX()));
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
