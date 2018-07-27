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
                if( value != null)
                {
                    var str = value.MutableCopy() as NSMutableAttributedString;
                    NSRange outRange;
                    var paragraph = str.GetAttribute(UIStringAttributeKey.ParagraphStyle,0, out outRange) as NSParagraphStyle ??
                                       new NSParagraphStyle().MutableCopy() as NSMutableParagraphStyle;
                    paragraph.Alignment = UITextAlignment.Center;
                    str.AddAttribute(UIStringAttributeKey.ParagraphStyle, paragraph, new NSRange(0, str.Length));
					TextLabel.AttributedText = str;
					TextLabel.SetNeedsDisplay();
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
            var topY = -ShapeView.Bounds.Height + 1;

            var transform = CATransform3D.Identity;
            transform = transform.Translate(0,topY, 0);

            UIView.AnimateNotify(0.22, 0.0, springWithDampingRatio: 0.33f, initialSpringVelocity: 0.0f, options: UIViewAnimationOptions.CurveEaseIn,
            animations: () =>
            {
                ShapeView.Layer.Transform = transform;
                AnimationFrame?.Invoke();
            }, completion: null);

        }

        public void AnimateTrackingEnd()
        {
            var transform = CATransform3D.Identity;
            UIView.Animate(0.22, () =>
             {
                ShapeView.Layer.Transform = transform;
                 AnimationFrame?.Invoke();
             });
        }
    }
}
