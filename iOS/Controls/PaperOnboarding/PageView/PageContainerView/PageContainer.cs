using System;
using System.Collections.Generic;
using UIKit;

namespace XamControls.iOS.Controls
{
    public class PageContainer : UIView
    {
        public List<PageViewItem> Items;
        private float space; //space between items
        private int currentIndex = 0;

        private float itemRadius;
        private float selectedItemRadius;
        private int itemsCount;
        private string animationKey = "animationKey";

        public PageContainer(float radius,
                            float selectedRadius,
                            float space,
                            int itemsCount,
                            UIColor itemColor)
        {
            this.itemsCount = itemsCount;
            this.space = space;
            itemRadius = radius;
            selectedItemRadius = selectedRadius;

            Items = CreateItems(itemsCount, radius, selectedRadius, itemColor);
        }

        public void CurrentIndex(int index, double duration, bool animated)
        {
            var items = this.Items;
            if (index == currentIndex) return; //TODO: CHECK HERE

            AnimationItem(items[index], true, duration);


            var fillColor = index > currentIndex ? true : false;
            AnimationItem(items[currentIndex], false, duration, fillColor);

            currentIndex = index;
        }

        protected void AnimationItem(PageViewItem item, bool selected, double duration, bool fillColor = false)
        {
            var toValue = selected ? selectedItemRadius * 2: itemRadius * 2;
            foreach(var constraint in item.Constraints)
            {
                if (constraint.GetIdentifier() == animationKey)
                    constraint.Constant = toValue;
            }

            UIView.Animate(duration, 0, UIViewAnimationOptions.CurveEaseOut, () =>
            {
                this.LayoutIfNeeded();
            }, () => { });

            item.AnimationSelected(selected, duration, fillColor);
        }

        private List<PageViewItem> CreateItems(int count, float radius, float selectedRadius, UIColor itemColor)
        {
            var _items = new List<PageViewItem>();

            // Create first item
            var tag = 1;
            var item = CreateItem(radius, selectedRadius, itemColor, true);//itemColor(tag - 1));
            item.Tag = tag;
            AddConstraintsToView(item, selectedRadius);
            _items.Add(item);

            for (var i = 1; i < count; i++)
            {
                tag += 1;
                var nextItem = CreateItem(radius, selectedRadius, itemColor, false);//itemColor(tag - 1));
                AddConstraintsToView(nextItem, item, radius, isEnd: i == count -1);
                _items.Add(nextItem);
                item = nextItem;
                item.Tag = tag;
            }

            return _items;
        }

        private PageViewItem CreateItem(float radius, float selectedRadius, UIColor itemColor, bool isSelect = false)
        {
            var item = new PageViewItem(radius, itemColor, selectedRadius, isSelect);
            item.TranslatesAutoresizingMaskIntoConstraints = false;
            item.BackgroundColor = UIColor.Clear;

            this.AddSubview(item);

            return item;
        }

        private void AddConstraintsToView(UIView item, float radius)
        {
            (this, item).ConstraintOps((obj) => obj.Attribute = NSLayoutAttribute.Left);
            (this, item).ConstraintOps((obj) => obj.Attribute = NSLayoutAttribute.CenterY);

            var attrs = new NSLayoutAttribute[] { NSLayoutAttribute.Width, NSLayoutAttribute.Height };

            foreach(var att in attrs)
                item.ConstraintOps((obj) =>
                {
                    obj.Attribute = att;
                    obj.Constant = (System.nfloat)(radius * 2.0);
                    obj.Identifer = animationKey;
                });
        }

        private void AddConstraintsToView(UIView item, UIView leftView, float radius, bool isEnd = false)
        {
            if(isEnd)
                (this, item).ConstraintOps((obj) => obj.Attribute = NSLayoutAttribute.Right);

            (this, item).ConstraintOps((obj) => obj.Attribute = NSLayoutAttribute.CenterY);

            (this, item, leftView).ConstraintOps((obj) => 
            {
                obj.Attribute = NSLayoutAttribute.Leading;
                obj.SecondAttribute = NSLayoutAttribute.Trailing;
                obj.Constant = space;
            });


            var attrs = new NSLayoutAttribute[] { NSLayoutAttribute.Width, NSLayoutAttribute.Height };

            foreach (var att in attrs)
                item.ConstraintOps((obj) =>
                {
                    obj.Attribute = att;
                    obj.Constant = (System.nfloat)(radius * 2.0);
                    obj.Identifer = animationKey;
                });
        }
    }
}
