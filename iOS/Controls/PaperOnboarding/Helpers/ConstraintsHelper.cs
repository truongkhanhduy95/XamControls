using System;
using UIKit;

namespace XamControls.iOS.Controls
{
    public class ConstraintInfo 
    {
        public NSLayoutAttribute Attribute = NSLayoutAttribute.Left;
        public NSLayoutAttribute SecondAttribute = NSLayoutAttribute.NoAttribute;
        public nfloat Constant = 0;
        public string Identifer = String.Empty;
        public NSLayoutRelation Relation = NSLayoutRelation.Equal;
    }

    public static class ConstraintsHelper
    {
        public static NSLayoutConstraint ConstraintOps(this(UIView,UIView) tuple,Action<ConstraintInfo> action)
        {
            var info = new ConstraintInfo();
            action?.Invoke(info);
            info.SecondAttribute = info.SecondAttribute == NSLayoutAttribute.NoAttribute ?
                info.Attribute : info.SecondAttribute;

            var constraint = NSLayoutConstraint.Create(
                tuple.Item2,
                info.Attribute,
                info.Relation,
                tuple.Item1,
                info.SecondAttribute,
                1,
                info.Constant
            );
            constraint.SetIdentifier(info.Identifer);
            tuple.Item1.AddConstraint(constraint);

            return constraint;
        }

        public static NSLayoutConstraint ConstraintOps(this UIView view, Action<ConstraintInfo> action)
        {
            var info = new ConstraintInfo();
            action?.Invoke(info);
            
            var constraint = NSLayoutConstraint.Create(
                view,
                info.Attribute,
                info.Relation,
                1,
                info.Constant
            );
            constraint.SetIdentifier(info.Identifer);
            view.AddConstraint(constraint);

            return constraint;
        }

        public static NSLayoutConstraint ConstraintOps(this (UIView, UIView, UIView) tuple, Action<ConstraintInfo> action)
        {
            var info = new ConstraintInfo();
            action?.Invoke(info);
            info.SecondAttribute = info.SecondAttribute == NSLayoutAttribute.NoAttribute ?
                info.Attribute : info.SecondAttribute;

            var constraint = NSLayoutConstraint.Create(
                tuple.Item2,
                info.Attribute,
                info.Relation,
                tuple.Item3,
                info.SecondAttribute,
                1,
                info.Constant
            );
            constraint.SetIdentifier(info.Identifer);
            tuple.Item1.AddConstraint(constraint);

            return constraint;
        }
    }
}
