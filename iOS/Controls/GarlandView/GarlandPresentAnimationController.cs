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
            var fromVC = (GarlandViewController)transitionContext.GetViewControllerForKey(UITransitionContext.FromViewControllerKey);
            var toVC = (GarlandViewController)transitionContext.GetViewControllerForKey(UITransitionContext.ToViewControllerKey);

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

            foreach (var cell in fromCollection.VisibleCells)
            {
                var cellSnap = cell.SnapshotView(true);
                if (cellSnap == null) continue;

                var convertedCoord = fromCollection.ConvertPointToView(new CGPoint(cell.Frame.X, cell.Frame.Y), null); //NULL HERE
                cellSnap.Frame = new CGRect(convertedCoord.X, convertedCoord.Y, cell.Frame.Width, cell.Frame.Height);
                cellSize.Add(new CGSize(cell.Frame.Width, cell.Frame.Height));
                if (convertedCoord.Y < headerStartFrame.GetMinY())
                {
                    cellSnap.Alpha = 0;
                }
                else if (convertedCoord.Y < headerStartFrame.GetMaxY())
                {
                    overlappedCells.Add(cellSnap);
                }
                var config = GarlandConfig.Instance;
                cellSnap.Layer.CornerRadius = config.CardRadius;
                cellSnap.Layer.ShadowOffset = config.CardShadowOffset;
                cellSnap.Layer.ShadowColor = config.CardShadowColor.CGColor;
                cellSnap.Layer.ShadowOpacity = config.CardShadowOpacity;
                cellSnap.Layer.ShadowRadius = config.CardShadowRadius;
                visibleFromSnapshots.Add(cellSnap);
                containerView.AddSubview(cellSnap);
            }

            var visibleToSnapshots = new List<UIView>();
            foreach (var cell in toCollection.VisibleCells)
            {
                var cellSnap = cell.SnapshotView(true);
                if (cellSnap == null) continue;

                var convertedCoord = toCollection.ConvertPointToView(new CGPoint(cell.Frame.X, cell.Frame.Y), null); //.. AND HERE
                convertedFromCellCoords.Add(convertedCoord);
                cellSnap.Frame = new CGRect(new CGPoint(cellSnap.Frame.X, cellSnap.Frame.Y), new CGSize(cellSnap.Frame.Width / 3, cellSnap.Frame.Height / 2));
                var finalX = this.finalFromXFrame;
                if (finalX == UIScreen.MainScreen.Bounds.Width)
                {
                    finalX += cellSnap.Frame.Width;
                }
                var newPoint = new CGPoint(UIScreen.MainScreen.Bounds.Width - finalX, convertedCoord.Y + cellSnap.Frame.Height / 2);
                cellSnap.Frame = new CGRect(newPoint, cellSnap.Frame.Size);
                cellSnap.Alpha = 0.2f;

                var config = GarlandConfig.Instance;
                cellSnap.Layer.CornerRadius = config.CardRadius;
                cellSnap.Layer.ShadowOffset = config.CardShadowOffset;
                cellSnap.Layer.ShadowColor = config.CardShadowColor.CGColor;
                cellSnap.Layer.ShadowOpacity = config.CardShadowOpacity;
                cellSnap.Layer.ShadowRadius = config.CardShadowRadius;
                visibleToSnapshots.Add(cellSnap);
                containerView.AddSubview(cellSnap);
            }

            //configure headers
            fromHeaderSnapshot.Frame = headerStartFrame;
            var configuration = GarlandConfig.Instance;
            fromHeaderSnapshot.Layer.MasksToBounds = false;
            fromHeaderSnapshot.Layer.CornerRadius = configuration.CardRadius;
            fromHeaderSnapshot.Layer.ShadowOffset = configuration.CardShadowOffset;
            fromHeaderSnapshot.Layer.ShadowColor = configuration.CardShadowColor.CGColor;
            fromHeaderSnapshot.Layer.ShadowOpacity = configuration.CardShadowOpacity;
            fromHeaderSnapshot.Layer.ShadowRadius = configuration.CardShadowRadius;

            var toFakeHeader = finalFromXFrame == 0 ? toVC.RightFakeHeader : toVC.LeftFakeHeader;
            var fromFakeHeader = finalFromXFrame == 0 ? toVC.LeftFakeHeader : toVC.RightFakeHeader;
            var toFakeHeaderSnapshot = finalFromXFrame == 0 ? rightFakeHeaderSnapshot : leftFakeHeaderSnapshot;
            var fromFakeHeaderSnapshot = finalFromXFrame == 0 ? leftFakeHeaderSnapshot : rightFakeHeaderSnapshot;

            var headerToFrame = toVC.View.ConvertRectToView(toFakeHeader.Frame, containerView);
            var headerFromFrame = toVC.View.ConvertRectToView(fromFakeHeader.Frame, containerView);

            toFakeHeaderSnapshot.Frame = headerToFrame;
            fromFakeHeaderSnapshot.Frame = headerStartFrame;
            toHeaderSnapshot.Frame = headerToFrame;
            toHeaderSnapshot.Alpha = 0;

            containerView.AddSubview(toFakeHeaderSnapshot);
            containerView.AddSubview(fromFakeHeaderSnapshot);
            containerView.AddSubview(toHeaderSnapshot);
            containerView.AddSubview(fromHeaderSnapshot);

            //hide origin views
            //fromFakeHeaderSnapshot.alpha = 0
            toFakeHeader.Alpha = 0;
            toVC.HeaderView.Alpha = 0;
            toVC.Collection.Alpha = 0;
            fromVC.HeaderView.Alpha = 0;
            fromVC.Collection.Alpha = 0;

            AnimationHelper.PerspectiveTransformForContainerView(containerView);
            var duration = TransitionDuration(transitionContext);

            UIView.AnimateKeyframes(duration, 0, UIViewKeyframeAnimationOptions.CalculationModeLinear, () =>
            {
                UIView.AddKeyframeWithRelativeStartTime(0.0, 0.25f, () =>
                  {
                      toHeaderSnapshot.Alpha = 1;
                  });

                UIView.AddKeyframeWithRelativeStartTime(0.1, 0.3f, () =>
                {
                    overlappedCells.ForEach((obj) => obj.Alpha = 0);
                });

                UIView.AddKeyframeWithRelativeStartTime(0, 1f, () =>
                {
                    fromHeaderSnapshot.Frame = headerFromFrame;
                    fromFakeHeaderSnapshot.Frame = headerFromFrame;
                    toHeaderSnapshot.Frame = headerFinalFrame;
                    toFakeHeaderSnapshot.Frame = headerFinalFrame;


                    fromHeaderSnapshot.Alpha = 0.2f;
                    toHeaderSnapshot.Alpha = 1;


                    fromFakeHeader.Transform = CGAffineTransform.MakeTranslation(headerStartFrame.GetMidX() - headerToFrame.GetMidX(), 0);

                    for (var i = 0; i < visibleToSnapshots.Count; i++)
                    {
                        var snapshot = visibleToSnapshots[i];
                        snapshot.Frame = new CGRect(convertedFromCellCoords[i], new CGSize(cellSize[i].Width, cellSize[i].Height));
                        snapshot.Alpha = 1.0f;
                    }

                    foreach (var snapshot in visibleFromSnapshots)
                    {
                        snapshot.Frame = new CGRect(new CGPoint(snapshot.Frame.X, snapshot.Frame.Y),
                                                    new CGSize(snapshot.Frame.Width / 3, snapshot.Frame.Height / 2));
                        var finalX = this.finalFromXFrame;
                        if (finalX == UIScreen.MainScreen.Bounds.Width)
                            finalX -= snapshot.Frame.Width;
                        snapshot.Frame = new CGRect(new CGPoint(finalX, snapshot.Frame.Y + snapshot.Frame.Height / 2),
                                                    snapshot.Frame.Size);
                        snapshot.Alpha = 0;

                    }
                });

                UIView.AddKeyframeWithRelativeStartTime(0.75, 0.25f, () =>
                {
                    fromHeaderSnapshot.Alpha = 0;
                });

                UIView.AddKeyframeWithRelativeStartTime(0.9, 0.1f, () =>
                {
                    toFakeHeader.Alpha = 1;
                });

            }, (bool finished) =>
             {
                 fromFakeHeader.Transform = CGAffineTransform.MakeIdentity();
                 toVC.Collection.Alpha = 1.0f;
                 toVC.HeaderView.Alpha = 1;
                 toVC.LeftFakeHeader.Alpha = 1;
                 toVC.RightFakeHeader.Alpha = 1;
                   
                 foreach (var snap in visibleFromSnapshots)
                 {
                     snap.RemoveFromSuperview();
                 }
                 foreach (var snap in visibleToSnapshots)
                 {
                     snap.RemoveFromSuperview();
                 }
                 leftFakeHeaderSnapshot.RemoveFromSuperview();
                 rightFakeHeaderSnapshot.RemoveFromSuperview();
                 fromHeaderSnapshot.RemoveFromSuperview();
                 toHeaderSnapshot.RemoveFromSuperview();
                 fromVC.View.RemoveFromSuperview();
                 transitionContext.CompleteTransition(!transitionContext.TransitionWasCancelled);
             });
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
