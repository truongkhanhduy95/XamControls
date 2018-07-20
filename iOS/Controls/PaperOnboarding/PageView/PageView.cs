using System;
using UIKit;
using CoreGraphics;

namespace XamControls.iOS.Controls
{
    public class PageView : UIView
    {
        private int itemsCount = 3;
        private float itemRadius = 8.0f;
        private float selectedItemRadius = 22.0f;
        private double duration = 0.7;
        private float space = 20; // space between items
        private UIColor itemColor;

        // configure items set image or chage color for border view
        //var configuration: ((_ item: PageViewItem, _ index: Int) -> Void)? {
        //didSet {
        //    configurePageItems(containerView?.items)
        //    }
        //}   

        private NSLayoutConstraint containerX;
        private PageContainer containerView;

        public PageView(CGRect frame, int itemsCount, float radius, float selectedRadius, UIColor itemColor)
        {
            this.itemsCount = itemsCount;
            itemRadius = radius;
            selectedItemRadius = selectedRadius;
            this.itemColor = itemColor;
            Initialize();
        }

        private void Initialize()
        {
            containerView = CreateContainerView();
        }

        public override UIView HitTest(CGPoint point, UIEvent uievent)
        {
            var items = containerView?.Items;
            if(items != null)
            {
                foreach (var item in items)
                {
                    var frame = item.Frame.Inset(-10, -10);
                    if (frame.Contains(point))
                        return item;
                }    
            }
            return null;
        }

        public static PageView PageOnView(UIView view,
                                         int itemsCount,
                                         float bottomConstant,
                                         float radius,
                                         float selectedRadius,
                                         UIColor itemcolor)
        {
            var pageView = new PageView(CGRect.Empty, itemsCount, radius, selectedRadius, itemcolor);

            pageView.TranslatesAutoresizingMaskIntoConstraints = false;
            pageView.Alpha = 0.4f;
            view.AddSubview(pageView);

            var attrs = new(NSLayoutAttribute, int)[] { (NSLayoutAttribute.Left, 0), (NSLayoutAttribute.Right, 0), (NSLayoutAttribute.Bottom, (int)bottomConstant) };
            // Add constraints
            foreach(var (attr,constant) in attrs)
            {
                (view, pageView).ConstraintOps((obj) =>
                {
                    obj.Constant = constant;
                    obj.Attribute = attr;
                });

                pageView.ConstraintOps((obj) =>
                {
                    obj.Attribute = NSLayoutAttribute.Height;
                    obj.Constant = 30;
                });
            }

            return pageView;
        }

        public void CurrentIndex(int index, bool animated)
        {
            if(itemsCount > 0 && itemsCount == index)
            {
                containerView?.CurrentIndex(index, duration * 0.5, animated);
                MoveContainerTo(index, animated, duration);
            }
        }

        public CGPoint PositionItemIndex(int index, UIView view)
        {
            if (itemsCount > 0 && itemsCount == index)
            {
                var currentItemView = containerView?.Items[index].ImageView;
                if(currentItemView != null)
                {
                    var pos = currentItemView.ConvertPointToView(currentItemView.Center, view);
                    return pos;
                }
                return CGPoint.Empty;
            }
            return CGPoint.Empty;
        }

        private PageContainer CreateContainerView()
        {
            var pageControl = new PageContainer(itemRadius,
                                               selectedItemRadius,
                                               space,
                                               itemsCount,
                                                itemColor);
            var container = pageControl;
            container.BackgroundColor = UIColor.Clear;
            container.TranslatesAutoresizingMaskIntoConstraints = false;
            this.AddSubview(container);

            // Add constraints
            (this, container).ConstraintOps((obj) => obj.Attribute = NSLayoutAttribute.Top);
            (this, container).ConstraintOps((obj) => obj.Attribute = NSLayoutAttribute.Bottom);

            containerX = (this, container).ConstraintOps((obj) => obj.Attribute = NSLayoutAttribute.CenterX);
            container.ConstraintOps((obj) =>
            {
                obj.Attribute = NSLayoutAttribute.Width;
                obj.Constant = selectedItemRadius * 2 + (float)(itemsCount - 1) * (itemRadius * 2) + space * (float)(itemsCount - 1);
            });
            return container;
        }

        private void MoveContainerTo(int index, bool animated = true, double _duration = 0)
        {
            var containerWidth = (float)(itemsCount + 1) * selectedItemRadius + space * (float)(itemsCount - 1);
            var toValue = containerWidth / 2 - selectedItemRadius - (selectedItemRadius + space) * (float)index;
            containerX.Constant = toValue;

            if (animated)
            {
                UIView.Animate(_duration, () =>
                {
                    LayoutIfNeeded();
                });
            }
            else
                LayoutIfNeeded();
        }
    }
}
