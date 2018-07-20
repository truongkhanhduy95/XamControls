using System;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace XamControls.iOS.Controls
{
    public partial class FillAnimationView : UIView
    {
        //Constant
        public static string Path = "path";
        public static string Circle = "circle";
    }

    // MARK : public

    public partial class FillAnimationView 
    {
        public static  FillAnimationView AnimationViewOnView(UIView view, UIColor color)
        {
            var animationView = new FillAnimationView();
            animationView.Frame = CGRect.Empty;
            animationView.BackgroundColor = color;
            animationView.TranslatesAutoresizingMaskIntoConstraints = false;

            view.AddSubview(animationView);

            //Add constraints
            var attrs = new NSLayoutAttribute[] { NSLayoutAttribute.Left, NSLayoutAttribute.Right, NSLayoutAttribute.Top, NSLayoutAttribute.Bottom };
            foreach (var attr in attrs)
                (view, animationView).ConstraintOps((ConstraintInfo obj) => obj.Attribute = attr);

            return animationView;
        }

        public void FillAnimation(UIColor color, CGPoint centerPosition, double duration)
        {
            var radius = Math.Max(Bounds.Size.Width, Bounds.Size.Height) * 1.5;
            var circle = CreateCircleLayer(centerPosition, color);

            var animation = AnimationToRadius((float)radius, centerPosition, duration);
            animation.SetValueForKey(circle, (Foundation.NSString)Circle);
            circle.AddAnimation(animation, null);
        }

        private CAShapeLayer CreateCircleLayer(CGPoint position, UIColor color)
        {
            var path = UIBezierPath.FromArc(position, 1, 0, (System.nfloat)(2 * Math.PI), true);
            var layer = new CAShapeLayer();
            layer.Path = path.CGPath;
            layer.FillColor = color.CGColor;
            layer.ShouldRasterize = true;

            this.Layer.AddSublayer(layer);
            return layer;
        }
    }

    // MARK: animation

    public partial class FillAnimationView //: ICAAnimationDelegate
    {
        private CABasicAnimation AnimationToRadius(float radius, CGPoint center, double duration)
        {
            var path = UIBezierPath.FromArc(center, radius, 0, (System.nfloat)(2 * Math.PI), true);
            var animation = new CABasicAnimation();
            animation.KeyPath = Path;
            animation.Duration = duration;
            animation.SetTo(path.CGPath);
            animation.RemovedOnCompletion = false;
            animation.FillMode = CAFillMode.Forwards;
            animation.AnimationStopped += OnAnimationStopped;
            animation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseIn);

            return animation;
        }

        private void OnAnimationStopped(object sender, CAAnimationStateEventArgs e)
        {
            var circleLayer = (sender as CAAnimation).ValueForKey((Foundation.NSString)Circle) as CAShapeLayer;
            if (circleLayer == null) return;

            Layer.BackgroundColor = circleLayer.FillColor;
            circleLayer.RemoveFromSuperLayer();
        }
    }
}
