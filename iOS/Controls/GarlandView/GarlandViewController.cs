using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace XamControls.iOS.Controls
{
    public partial class GarlandViewController : UIViewController
    {
        private GarlandViewController _nextViewController;
        public GarlandViewController NextViewController 
        { 
            get {
                return _nextViewController;
            }
            private set {
                _nextViewController = value;
            }
        }

        public GarlandCollection Collection = new GarlandCollection(null); //CHECK
        public UIView HeaderView { get; private set; }
        public UIView BackgroundHeader;

        public UIView RightFakeHeader;
        public UIView LeftFakeHeader;

        //open var animationXDest: CGFloat = 0.0
        //open var selectedCardIndex: IndexPath = IndexPath()
        public bool IsPresenting = false;

        private GarlandPresentAnimationController animationController = new GarlandPresentAnimationController();

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            this.TransitioningDelegate = this;

            Collection.Frame = new CGRect(0, GarlandConfig.Instance.HeaderVerticalOffset,
                                          View.Bounds.Width, View.Bounds.Height - GarlandConfig.Instance.HeaderVerticalOffset);
            Collection.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            View.AddSubview(Collection);

            SetupBackground();
            SetupFakeHeaders();

            var panGesture = new UIPanGestureRecognizer((UIPanGestureRecognizer gesture) =>
            {
                var velocity = gesture.VelocityInView(View);
                var translation = gesture.TranslationInView(View);

                if(velocity.X > 0 && translation.X > 15)
                {
                    PerformTransition(TransitionDirection.Right);
                }
                else if(translation.X < -15)
                {
                    PerformTransition(TransitionDirection.Left);
                }

            });
            View.AddGestureRecognizer(panGesture);
        }

        private void PerformTransition(TransitionDirection direction)
        {
            if(!IsPresenting)
            {
                IsPresenting = true;
            }
        }

        public void SetUpHeader(UIView header)
        {
            this.HeaderView = header;
            Collection.ContentInset = new UIEdgeInsets(GarlandConfig.Instance.HeaderSize.Height + GarlandConfig.Instance.CardSpacing,
                                                      Collection.ContentInset.Left,
                                                      Collection.ContentInset.Bottom,
                                                       Collection.ContentInset.Right);

            HeaderView.Frame = new CGRect(new CGPoint((UIScreen.MainScreen.Bounds.Width - GarlandConfig.Instance.HeaderSize.Width) / 2,
                                                      Collection.Frame.GetMinY()), GarlandConfig.Instance.HeaderSize);
            View.AddSubview(HeaderView);
        }

        private void SetupBackground()
        {
            
        }

        private void SetupFakeHeaders()
        {
            
        }

    }

    //Implement animation transition
    public partial class GarlandViewController : IUIViewControllerTransitioningDelegate
    {
        [Export("animationControllerForPresentedController:presentingController:sourceController:")]
        public IUIViewControllerAnimatedTransitioning GetAnimationControllerForPresentedController(UIViewController presented, UIViewController presenting, UIViewController source)
        {
            return animationController;
        }

        [Export("animationControllerForDismissedController:")]
        public IUIViewControllerAnimatedTransitioning GetAnimationControllerForDismissedController(UIViewController dismissed)
        {
            return animationController;

        }
    }


}
