using System;
using CoreAnimation;
using CoreGraphics;
using UIKit;

namespace XamControls.iOS.Controls
{
    public class PageViewItem : UIView
    {
        private float circleRadius;
        private float selectedCircleRadius;
        private float lineWidth;
        private UIColor itemColor;

        private bool select;

        private UIView centerView;
        public UIImageView ImageView;
        private CAShapeLayer circleLayer;
        private int tickIndex;

        public PageViewItem(float radius,
                           UIColor color,
                           float selectedRadius,
                            bool isSelect = false,
                           float lineWidth = 3
                           )
        {
            itemColor = color;
            this.lineWidth = lineWidth;
            circleRadius = radius;
            selectedCircleRadius = selectedRadius;
            select = isSelect;
            Initialize();
        }

        private void Initialize()
        {
            centerView = CreateBorderView();
            ImageView = CreateImageView();
        }

        public void AnimationSelected(bool selected, double duration, bool fillColor)
        {
            var toAlpha = selected ? 1 : 0;
            ImageAlphaAnimation(toAlpha, duration);

            var currentRad = selected ? selectedCircleRadius : circleRadius;
            var scaleAnim = CircleScaleAnimation(currentRad - lineWidth / 2, duration);
            var toColor = fillColor ? itemColor : UIColor.Clear;
            var colorAnim = CircleBackgroundAnimation(toColor, duration);

            circleLayer?.AddAnimation(scaleAnim, null);
            circleLayer?.AddAnimation(colorAnim, null);
        }

        private UIView CreateBorderView()
        {
            var view = new UIView(CGRect.Empty);
            view.BackgroundColor = UIColor.Blue;
            view.TranslatesAutoresizingMaskIntoConstraints = false;

            this.AddSubview(view);

            // Create circle layer
            var currentRadius = select ? selectedCircleRadius : circleRadius;
            var clLayer = CreateCircleShapeLayer(currentRadius, lineWidth);
            view.Layer.AddSublayer(clLayer);
            this.circleLayer = clLayer;

            // Add constraints
            (this, view).ConstraintOps((obj) => obj.Attribute = NSLayoutAttribute.CenterX);
            (this, view).ConstraintOps((obj) => obj.Attribute = NSLayoutAttribute.CenterY);
            view.ConstraintOps((obj) => obj.Attribute = NSLayoutAttribute.Height);
            view.ConstraintOps((obj) => obj.Attribute = NSLayoutAttribute.Width);

            return view;
        }

        private UIImageView CreateImageView()
        {
            var imgView = new UIImageView(CGRect.Empty);
            imgView.ContentMode = UIViewContentMode.ScaleAspectFit;
            imgView.TranslatesAutoresizingMaskIntoConstraints = false;
            imgView.Alpha = select ? 1 : 0;

            this.AddSubview(imgView);

            // Add constraints
            (this, imgView).ConstraintOps((obj) => obj.Attribute = NSLayoutAttribute.Left);
            (this, imgView).ConstraintOps((obj) => obj.Attribute = NSLayoutAttribute.Right);
            (this, imgView).ConstraintOps((obj) => obj.Attribute = NSLayoutAttribute.Top);
            (this, imgView).ConstraintOps((obj) => obj.Attribute = NSLayoutAttribute.Bottom);

            return imgView;
        }

        private CAShapeLayer CreateCircleShapeLayer(float radius, float lineW)
        {
            var path = UIBezierPath.FromArc(CGPoint.Empty,
                                           radius - lineW / 2,
                                           0,
                                            (nfloat)(2 * Math.PI), true);
            var layer = new CAShapeLayer();
            layer.Path = path.CGPath;
            layer.LineWidth = lineW;
            layer.StrokeColor = itemColor.CGColor;
            layer.FillColor = UIColor.Clear.CGColor;

            return layer;
        }

        private CABasicAnimation CircleScaleAnimation(float toRadius, double duration)
        {
            var path = UIBezierPath.FromArc(CGPoint.Empty,
                                           toRadius,
                                           0,
                                            (nfloat)(2 * Math.PI), true);
            var animation = CABasicAnimation.FromKeyPath("path");
            animation.Duration = duration;
            animation.SetTo(path.CGPath);
            animation.RemovedOnCompletion = false;
            animation.FillMode = CAFillMode.Forwards;
            return animation;
        }

        private CABasicAnimation CircleBackgroundAnimation(UIColor toColor, double duration)
        {
           
            var animation = CABasicAnimation.FromKeyPath("fillColor");
            animation.Duration = duration;
            animation.SetTo(toColor.CGColor);
            animation.RemovedOnCompletion = false;
            animation.FillMode = CAFillMode.Forwards;
            animation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);
            return animation;
        }

        private void ImageAlphaAnimation(float toValue, double duration)
        {
            UIView.Animate(duration, () =>
             {
                ImageView.Alpha = toValue;
             });
        }
    }
}
