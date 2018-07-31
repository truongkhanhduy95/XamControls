using System;
using UIKit;

namespace XamControls.iOS.Controls
{
    public class OnboardingContentViewItem : UIView
    {
        public NSLayoutConstraint DescriptionBottomConstraint;
        public NSLayoutConstraint TitleCenterConstraint;
        public NSLayoutConstraint InformationImageWidthConstraint;
        public NSLayoutConstraint InformationImageHeightConstraint;

        public UIImageView ImageView;
        public UILabel TitleLabel;
        public UILabel DescriptionLabel;

        public OnboardingContentViewItem(CoreGraphics.CGRect gRect)
        {
            this.Frame = gRect;
            Initialize();
        }

        public static OnboardingContentViewItem ItemOnView(UIView view)
        {
            var item = new OnboardingContentViewItem(CoreGraphics.CGRect.Empty);
            item.BackgroundColor = UIColor.Clear;
            item.TranslatesAutoresizingMaskIntoConstraints = false;

            view.AddSubview(item);

            //Add constraints
            item.ConstraintOps((obj) =>
            {
                obj.Attribute = NSLayoutAttribute.Height;
                obj.Constant = 10000;
                obj.Relation = NSLayoutRelation.LessThanOrEqual;
            });

            (view, item).ConstraintOps((obj) => obj.Attribute = NSLayoutAttribute.Leading);
            (view, item).ConstraintOps((obj) => obj.Attribute = NSLayoutAttribute.Trailing);
            (view, item).ConstraintOps((obj) => obj.Attribute = NSLayoutAttribute.CenterX);
            (view, item).ConstraintOps((obj) => obj.Attribute = NSLayoutAttribute.CenterY);

            return item;
        }

        private void Initialize()
        {
            var titleLabel = CreateTitleLabel(this);
            var descriptionLabel = CreateDescriptionLabel(this);
            var imageView = CreateImage(this);

            //Add constraints
            TitleCenterConstraint = (this,titleLabel, imageView).ConstraintOps((obj) =>
            {
                obj.Attribute = NSLayoutAttribute.Top;
                obj.SecondAttribute = NSLayoutAttribute.Bottom;
                obj.Constant = 50;
            });

            (this, descriptionLabel, titleLabel).ConstraintOps((obj) =>
            {
                obj.Attribute = NSLayoutAttribute.Top;
                obj.SecondAttribute = NSLayoutAttribute.Bottom;
                obj.Constant = 10;
            });

            TitleLabel = titleLabel;
            DescriptionLabel = descriptionLabel;
            ImageView = imageView;
        }

        private UILabel CreateTitleLabel(UIView view)
        {
            var label = CreateLabel();
            label.Font = UIFont.BoldSystemFontOfSize(22.0f);

            view.AddSubview(label);

            //Add constraints
            label.ConstraintOps((ConstraintInfo obj) =>
            {
                obj.Attribute = NSLayoutAttribute.Height;
                obj.Constant = 10000;
                obj.Relation = NSLayoutRelation.LessThanOrEqual;
            });

            var attrs = new NSLayoutAttribute[] { NSLayoutAttribute.CenterX, NSLayoutAttribute.Leading, NSLayoutAttribute.Trailing };
            foreach(var attr in attrs)
            {
                (view, label).ConstraintOps((obj) => obj.Attribute = attr);
            }

            return label;
        }

        private UILabel CreateDescriptionLabel(UIView view)
        {
            var label = CreateLabel();
            //label.Font = 
            label.Lines = 0;

            view.AddSubview(label);

            //Add constraints
            label.ConstraintOps((obj) =>
            {
                obj.Attribute = NSLayoutAttribute.Height;
                obj.Constant = 10000;
                obj.Relation = NSLayoutRelation.LessThanOrEqual;
            });

            var attrs = new(NSLayoutAttribute, int)[] { (NSLayoutAttribute.Leading, 30), (NSLayoutAttribute.Trailing, -30) };
            foreach (var (attr, constant) in attrs)
            {
                (view, label).ConstraintOps((obj) =>
                {
                    obj.Attribute = attr;
                    obj.Constant = constant;
                });
            }

            (view, label).ConstraintOps((ConstraintInfo obj) => obj.Attribute = NSLayoutAttribute.CenterX);
            DescriptionBottomConstraint = (view, label).ConstraintOps((obj) => obj.Attribute = NSLayoutAttribute.Bottom);

            return label;
        }

        private UILabel CreateLabel()
        {
            var label = new UILabel();
            label.Frame = CoreGraphics.CGRect.Empty;
            label.BackgroundColor = UIColor.Clear;
            label.TranslatesAutoresizingMaskIntoConstraints = false;
            label.TextAlignment = UITextAlignment.Center;
            label.TextColor = UIColor.White;
            return label;
        }

        private UIImageView CreateImage(UIView view)
        {
            var imageView = new UIImageView();
            imageView.Frame = CoreGraphics.CGRect.Empty;
            imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            imageView.TranslatesAutoresizingMaskIntoConstraints = false;

            view.AddSubview(imageView);

            //Add constraints
            InformationImageWidthConstraint = imageView.ConstraintOps((obj) =>
            {
                obj.Attribute = NSLayoutAttribute.Width;
                obj.Constant = 188;
            });

            InformationImageHeightConstraint = imageView.ConstraintOps((obj) =>
            {
                obj.Attribute = NSLayoutAttribute.Height;
                obj.Constant = 188;
            });

            (view, imageView).ConstraintOps((obj) => obj.Attribute = NSLayoutAttribute.CenterX);
            (view, imageView).ConstraintOps((obj) => obj.Attribute = NSLayoutAttribute.Top);

            return imageView;
        }
    }
}
