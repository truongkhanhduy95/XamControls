using System;
using UIKit;

namespace XamControls.iOS.Controls
{
    public interface IOnboardingContentViewDelegate
    {
        OnboardingItemInfo OnboardingItemAtIndex(int index);
        void OnboardingConfiguration(OnboardingContentViewItem item, int index);
    }

    public abstract class OnboardingContentViewDelegate : IOnboardingContentViewDelegate
    {
        public abstract void OnboardingConfiguration(OnboardingContentViewItem item, int index);

        public abstract OnboardingItemInfo OnboardingItemAtIndex(int index);
    }

    public class OnboardingContentView : UIView
    {
        //Constants
        readonly nfloat DyOffsetAnimation = 110;
        readonly double ShowDuration = 0.8;
        readonly double HideDuration = 0.2;

        private OnboardingContentViewItem currentItem;
        private IOnboardingContentViewDelegate _delegate;

        public OnboardingContentView(int itemCounts, IOnboardingContentViewDelegate onboardDelegate)
        {
            _delegate = onboardDelegate;
            this.Frame = CoreGraphics.CGRect.Empty;
            Initialize();
        }

        public static OnboardingContentView ContentViewOnView(UIView view, IOnboardingContentViewDelegate delegateView, int itemCounts, nfloat bottomConstraints)
        {
            var contentView = new OnboardingContentView(itemCounts, delegateView);
            contentView.BackgroundColor = UIColor.Clear;
            contentView.TranslatesAutoresizingMaskIntoConstraints = false;

            view.AddSubview(contentView);

            //Add constraints
            (view, contentView).ConstraintOps((obj) => obj.Attribute = NSLayoutAttribute.Left);
            (view, contentView).ConstraintOps((obj) => obj.Attribute = NSLayoutAttribute.Right);
            (view, contentView).ConstraintOps((obj) => obj.Attribute = NSLayoutAttribute.Top);
            (view, contentView).ConstraintOps((obj) =>
            {
                obj.Attribute = NSLayoutAttribute.Bottom;
                obj.Constant = bottomConstraints;
            });

            return contentView;
        }

        public void CurrentItem(int index, bool animated)
        {
            var showItem = CreatItem(index);
            ShowItemView(showItem, ShowDuration);
            HideItemView(currentItem, HideDuration);
            currentItem = showItem;
        }

        private void Initialize()
        {
            currentItem = CreatItem(0);
        }

        private OnboardingContentViewItem CreatItem(int index)
        {
            var info = _delegate?.OnboardingItemAtIndex(index);
            if (info == null)
                return OnboardingContentViewItem.ItemOnView(this);

            var item = OnboardingContentViewItem.ItemOnView(this);
            item.ImageView.Image = info.InformationImage;
            item.TitleLabel.Text = info.Title;
            item.TitleLabel.Font = info.TitleFont;
            item.TitleLabel.TextColor = info.TitleColor;
            item.DescriptionLabel.Text = info.Description;
            item.DescriptionLabel.Font = info.DescriptionFont;
            item.DescriptionLabel.TextColor = info.DescriptionColor;

            _delegate?.OnboardingConfiguration(item, index);
            return item;
        }

        private void HideItemView(OnboardingContentViewItem item, double duration)
        {
            if (item == null) return;

            item.DescriptionBottomConstraint.Constant -= DyOffsetAnimation;
            item.TitleCenterConstraint.Constant *= 1.3f;

            UIView.Animate(duration, 0, UIViewAnimationOptions.CurveEaseOut,
                  () =>
                  {
                      item.Alpha = 0;
                      this.LayoutIfNeeded();
                  }, () =>
                  {
                      item.RemoveFromSuperview();
                  });
        }

        private void ShowItemView(OnboardingContentViewItem item, double duration)
        {
            item.DescriptionBottomConstraint.Constant = DyOffsetAnimation;
            item.TitleCenterConstraint.Constant /= 2;
            item.Alpha = 0;
            LayoutIfNeeded();

            item.DescriptionBottomConstraint.Constant = 0;
            item.TitleCenterConstraint.Constant *= 2;

            UIView.Animate(duration, 0, UIViewAnimationOptions.CurveEaseOut,
                  () =>
                  {
                      item.Alpha = 0;
                      item.Alpha = 1;
                      this.LayoutIfNeeded();
                  }, () =>
                  {
                  });
        }

    }
}
