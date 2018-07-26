using System;
using System.Collections.Generic;
using CoreAnimation;
using CoreGraphics;
using UIKit;

namespace XamControls.iOS.Controls
{
    public enum TransitionDirection
    {
        Left,
        Right
    }

    public class GarlandPresentAnimationController : UIViewControllerAnimatedTransitioning 
    {
        private TransitionDirection transitionDirection = TransitionDirection.Right;

        private nfloat finalFromXFrame
        {
            get {
                return transitionDirection == TransitionDirection.Right
                                                                 ? UIScreen.MainScreen.Bounds.Width : 0;
            }
        }

        private CGRect CreateRect(CGRect origin, CGSize size)
        {
            var center = new CGPoint(origin.GetMidX(), origin.GetMidY());
            var rect = new CGRect(center.X -size.Width/2, center.Y - size.Height /2,size.Width,size.Height);
            return rect;
        }

        public override void AnimateTransition(IUIViewControllerContextTransitioning transitionContext)
        {
            var fromVC = (GarlandViewController) transitionContext.GetViewControllerForKey(UITransitionContext.FromViewControllerKey);
            var toVC = (GarlandViewController) transitionContext.GetViewControllerForKey(UITransitionContext.ToViewControllerKey);

            var fromHeaderSnapshot = fromVC.HeaderView.SnapshotView(true);
            if (fromHeaderSnapshot == null)
            {
                transitionContext.CompleteTransition(false); return;
            }

            var fromCollection = fromVC.Collection;
            var toCollection = toVC.Collection;

            var containerView = transitionContext.ContainerView;

            containerView.Frame = fromVC.View.Frame;
            containerView.AddSubview(toVC.View);
            toVC.View.Frame = UIScreen.MainScreen.Bounds;
            toVC.View.LayoutSubviews();

            var toHeaderSnapshot = toVC.HeaderView.SnapshotView(true);
            var rightFakeHeaderSnapshot = toVC.RightFakeHeader.SnapshotView(true);
            var leftFakeHeaderSnapshot = toVC.LeftFakeHeader.SnapshotView(true);

            if (toHeaderSnapshot == null || rightFakeHeaderSnapshot == null || leftFakeHeaderSnapshot == null)
            {
                transitionContext.CompleteTransition(false); return;
            }

            var headerStartFrame = fromVC.View.ConvertRectToView(fromVC.HeaderView.Frame, containerView);
            var headerFinalFrame = CreateRect(headerStartFrame, toHeaderSnapshot.Frame.Size);

            //generate & configure cells transition views
            var visibleFromSnapshots = new List<UIView>();
            var overlappedCells = new List<UIView>();
            var convertedFromCellCoords = new List<CGPoint>();
            var cellSize = new List<CGSize>();

            foreach(var cell in fromCollection.VisibleCells)
            {
                var cellSnap = cell.SnapshotView(true);
                if (cellSnap == null) continue;

                var convertedCoord = fromCollection.ConvertPointToView(new CGPoint(cell.Frame.X, cell.Frame.Y), null); //NULL HERE
                cellSnap.Frame = new CGRect(convertedCoord.X, convertedCoord.Y, cell.Frame.Width, cell.Frame.Height);
                cellSize.Add(new CGSize(cell.Frame.Width, cell.Frame.Height));
                if(convertedCoord.Y < headerStartFrame.GetMinY())
                {
                    cellSnap.Alpha = 0;
                }
                else if(convertedCoord.Y < headerStartFrame.GetMaxY())
                {
                    overlappedCells.Add(cellSnap);
                }
            }
        }

        public override double TransitionDuration(IUIViewControllerContextTransitioning transitionContext)
        {
            return GarlandConfig.Instance.AnimationDuration;
        }
    }

    public class AnimationHelper
    {
        public static CATransform3D YRotation(double angle)
        {
            return CATransform3D.MakeRotation((nfloat)angle, 0.0f, 1.0f, 0.0f);
        }

        public static void PerspectiveTransformForContainerView(UIView containerView)
        {
            var transform = CATransform3D.Identity;
            transform.m34 = -0.002f;
            containerView.Layer.SublayerTransform = transform;
        }
    }
}
