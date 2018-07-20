using System;
using UIKit;

namespace XamControls.iOS.Controls
{
    public interface IGestureControleDelegate
    {
        void GestureControlDidSwipe(UISwipeGestureRecognizerDirection direction);
    }

    public abstract class GestureControleDelegate : IGestureControleDelegate
    {
        public abstract void GestureControlDidSwipe(UISwipeGestureRecognizerDirection direction);
    }

    public partial class GestureControl : UIView
    {
        private IGestureControleDelegate _delegate;

        public GestureControl(UIView view, IGestureControleDelegate gestureDelegate)
        {
            _delegate = gestureDelegate;

            //superInit
            this.Frame = CoreGraphics.CGRect.Empty;

            var swipeLeft = new UISwipeGestureRecognizer(SwipeHandler);
            //swipeLeft.AddTarget(this, SwipeHandler);
            swipeLeft.Direction = UISwipeGestureRecognizerDirection.Left;
            AddGestureRecognizer(swipeLeft);

            var swipeRight = new UISwipeGestureRecognizer(SwipeHandler);
            //swipeLeft.AddTarget(this, SwipeHandler);
            swipeRight.Direction = UISwipeGestureRecognizerDirection.Right;
            AddGestureRecognizer(swipeRight);

            TranslatesAutoresizingMaskIntoConstraints = false;
            BackgroundColor = UIColor.Clear;

            view.AddSubview(this);

            //Add constraint
            var attrs = new NSLayoutAttribute[] { NSLayoutAttribute.Left, NSLayoutAttribute.Right, NSLayoutAttribute.Top, NSLayoutAttribute.Bottom };
            foreach (var attr in attrs)
                (view, this).ConstraintOps((ConstraintInfo obj) => obj.Attribute = attr);
        }
    }

    // MARK: actions

    public partial class GestureControl
    {
        public void SwipeHandler(UISwipeGestureRecognizer gesture)
        {
            _delegate.GestureControlDidSwipe(gesture.Direction);
        }
    }

}
