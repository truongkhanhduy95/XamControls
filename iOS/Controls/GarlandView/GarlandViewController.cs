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
                throw new NotImplementedException("Not ported yet!!!");
                //IsPresenting = true;
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
            var config = GarlandConfig.Instance;
            BackgroundHeader.Frame = new CGRect(CGPoint.Empty, new CGSize(UIScreen.MainScreen.Bounds.Width,
                                                                         UIScreen.MainScreen.Bounds.Height));
            BackgroundHeader.BackgroundColor = config.BackgroundHeaderColor;
            View.InsertSubview(BackgroundHeader, 0);
        }

        private void SetupFakeHeaders()
        {
            var config = GarlandConfig.Instance;
            var size = new CGSize(config.HeaderSize.Width / 1.6f, config.HeaderSize.Height / 1.6f);
            var vertialPosition =  Collection.Frame.Y + (config.HeaderSize.Height - size.Height)/ 2;

            RightFakeHeader.Frame = new CGRect(new CGPoint(UIScreen.MainScreen.Bounds.Width - RightFakeHeader.Frame.Width / 14,
                                                           vertialPosition), size);
            RightFakeHeader.BackgroundColor = config.FakeHeaderColor;
            RightFakeHeader.Layer.CornerRadius = config.CardRadius;
            View.AddSubview(RightFakeHeader);

            LeftFakeHeader.Frame = new CGRect(new CGPoint(-LeftFakeHeader.Frame.Width + LeftFakeHeader.Frame.Width / 14,
                                                           vertialPosition), size);
            LeftFakeHeader.Layer.CornerRadius = config.CardRadius;
            LeftFakeHeader.BackgroundColor = config.FakeHeaderColor;
            View.AddSubview(LeftFakeHeader);
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
