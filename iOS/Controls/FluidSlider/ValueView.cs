using System;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace XamControls.iOS.Controls
{
    public class ValueView : UIView
    {
        public static float kLayoutMarginInset = 4;

        private CAShapeLayer outerShapeLayer;
        private CAShapeLayer innerShapeLayer;

        public UILabel TextLabel;
        public UIView ShapeView;

        public Action AnimationFrame;

        public UIColor OuterFillColor 
        {
            //get { return outerShapeLayer.FillColor; }
            set
            {
                if (value != null)
                {
                    outerShapeLayer.FillColor = value.CGColor;
                    outerShapeLayer.RemoveAllAnimations();    
                }
            }
        }

        public UIColor InnerFillColor
        {
            //get { return innerShapeLayer.FillColor; }
            set
            {
                if(value != null)
                {
                    innerShapeLayer.FillColor = value.CGColor;
                    innerShapeLayer.RemoveAllAnimations();    
                }
            }
        }

        public NSAttributedString AttributeText
        {
            get { return TextLabel.AttributedText; }
            set
            {
                if( value != TextLabel.AttributedText)
                {
                    var str = value.MutableCopy() as NSMutableAttributedString;
                    //var paragraph = str.GetAttribute(str.)
                    //throw new NotImplementedException(); //TODO
                }
                else
                {
                    TextLabel.AttributedText = null;
                }
            }
        }

        public ValueView()
        {
            Initialize();
        }

        public ValueView(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        private void Initialize()
        {
            TextLabel = new UILabel();
            ShapeView = new UIView();

            outerShapeLayer = new CAShapeLayer();
            innerShapeLayer = new CAShapeLayer();

            ShapeView.Frame = Bounds;
            ShapeView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            ShapeView.Layer.AddSublayer(outerShapeLayer);
            ShapeView.Layer.AddSublayer(innerShapeLayer);
            this.AddSubview(ShapeView);

            ShapeView.AddSubview(TextLabel);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            TextLabel.Frame = this.Bounds;

            outerShapeLayer.Path = UIBezierPath.FromOval(ShapeView.Bounds).CGPath;
            innerShapeLayer.Path = UIBezierPath.FromOval(ShapeView.Bounds.Inset(dx: kLayoutMarginInset, dy: kLayoutMarginInset)).CGPath;
            Layer.RemoveAllAnimations();
        }

        public void AnimateTrackingBegin()
        {
            UIView.AnimateAsync(0.22, () =>
             {
                var topY = -ShapeView.Bounds.Height - 4;

                 var animation = CASpringAnimation.FromKeyPath(@"translation.y");
                 animation.SetTo(NSNumber.FromFloat((float)(topY + ShapeView.Bounds.GetMidY())));
                 animation.RemovedOnCompletion = true;
                 animation.Duration = 0.22;
                 animation.AnimationStarted += (sender, e) =>
                 {
                     AnimationFrame?.Invoke();
                     System.Diagnostics.Debug.WriteLine("Runnn");
                 };

                 ShapeView.Layer.AddAnimation(animation, "bounce");
             });
        }

        public void AnimateTrackingEnd()
        {
            UIView.AnimateAsync(0.22, () =>
            {
                var animation = CABasicAnimation.FromKeyPath(@"translation.y");
                animation.SetTo(NSNumber.FromFloat((float)(ShapeView.Bounds.GetMidY())));
                animation.RemovedOnCompletion = true;
                animation.Duration = 0.22;
                animation.AnimationStarted += (sender, e) =>
                {
                    AnimationFrame?.Invoke();
                };

                ShapeView.Layer.AddAnimation(animation, "bounce");;
            });
        }
    }
}
