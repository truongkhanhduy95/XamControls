using System;
using System.Collections.Generic;
using CoreAnimation;
using UIKit;

namespace XamControls.iOS.Controls
{
    public class OnboardingItemInfo
    {
        public UIImage InformationImage { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public UIImage PageIcon { get; private set; }
        public UIColor Color { get; private set; }
        public UIColor TitleColor { get; private set; }
        public UIColor DescriptionColor { get; private set; }
        public UIFont TitleFont { get; private set; }
        public UIFont DescriptionFont { get; private set; }

        public OnboardingItemInfo(UIImage inforImage,
                                 string title,
                                 string description,
                                 UIImage pageIcon,
                                 UIColor color,
                                 UIColor titleColor,
                                 UIColor descriptionColor,
                                 UIFont titleFont,
                                 UIFont descriptionFont)
        {
            InformationImage = inforImage;
            Title = title;
            Description = description;
            PageIcon = pageIcon;
            Color = color;
            TitleColor = titleColor;
            DescriptionColor = descriptionColor;
            TitleFont = titleFont;
            DescriptionFont = descriptionFont;
        }
    }

    public partial class PaperOnboarding : UIView
    {
        private PaperOnboardingDataSource _dataSource;
        public PaperOnboardingDataSource DataSource
        {
            get { return _dataSource; }
            set 
            {
                _dataSource = value;
                Initialize();
            }
        }

        private PaperOnboardingDelegate _delegate;
        public PaperOnboardingDelegate Delegate
        {
            get { return _delegate; }
            set
            {
                _delegate = value;
            }
        }

        public int CurrentIndex { get; private set; }
        public int ItemsCounts { get; private set; }

        private List<OnboardingItemInfo> itemsInfo;

        private float pageViewBottomConstant;
        private float pageViewSelectedRadius = 22;
        private float pageViewRadius = 8;

        private FillAnimationView fillAnimationView;
        private PageView pageView;
        private GestureControl gestureControl;
        private OnboardingContentView contentView;

        public PaperOnboarding(float pageViewBottomConstant = 32)
        {
            this.pageViewBottomConstant = pageViewBottomConstant;
            this.pageViewSelectedRadius = 32;
            this.pageViewRadius = 0;
        }

        public void SetCurrentIndex(int index, bool animated)
        {
            if(ItemsCounts > 0 && ItemsCounts > index && index >= 0)
            {
                _delegate?.OnboardingWillTransitonToIndex(index);
                CurrentIndex = index;
                CATransaction.Begin();

                CATransaction.CompletionBlock = () => _delegate?.OnboardingDidTransitonToIndex(index);

                var position = pageView?.PositionItemIndex(index, this);
                if (position != null)
                {
                    fillAnimationView?.FillAnimation(GetItemColor(CurrentIndex), position.Value, 0.5);
                }
                pageView?.CurrentIndex(index, animated);
                contentView?.CurrentItem(index, animated);

                CATransaction.Commit();
            }
            else if(index >= ItemsCounts)
            {
                _delegate?.OnboardingWillTransitonToLeaving();   
            }
        }

        private void Initialize()
        {
            if (_dataSource != null)
            {
                ItemsCounts = _dataSource.OnboardingItemsCount();
                pageViewRadius = _dataSource.OnboardingPageItemRadius();
                pageViewSelectedRadius = _dataSource.OnboardingPageItemSelectedRadius();
            }

            itemsInfo = CreateItemsInfo();
            this.TranslatesAutoresizingMaskIntoConstraints = false;
            fillAnimationView = FillAnimationView.AnimationViewOnView(this, GetItemColor(CurrentIndex));
            contentView = OnboardingContentView.ContentViewOnView(this,
                                                                  this,
                                                                 ItemsCounts,
                                                                  pageViewBottomConstant * -1 - pageViewSelectedRadius);

            pageView = CreatePageView();
            gestureControl = new GestureControl(this, this);

            var tapGesture = new UITapGestureRecognizer(HandleAction);
            this.AddGestureRecognizer(tapGesture);
        }

        private void HandleAction(UIGestureRecognizer sender)
        {
        }

        private PageView CreatePageView()
        {
            var page = PageView.PageOnView(this, ItemsCounts, -pageViewBottomConstant,
                                           pageViewRadius, pageViewSelectedRadius, _dataSource.OnboardingPageItemColor(0));
            
            page.Configuration += (PageViewItem item, int index) => 
            {
                item.ImageView.Image = this.itemsInfo[index].PageIcon;
            };
            page.CurrentIndex(0, false);
            return page;
        }

        private List<OnboardingItemInfo> CreateItemsInfo()
        {
            if (_dataSource == null)
                throw new Exception("Data Source not set!!");

            var items = new List<OnboardingItemInfo>();
            for (var i = 0; i < ItemsCounts; i++)
            {
                var info = _dataSource.OnboardingItem(i);
                items.Add(info);
            }
            return items;
        }

        private UIColor GetItemColor(int index)
        {
            if (itemsInfo == null || itemsInfo.Count == 0) 
                return UIColor.Black;
            else
            {
                var color = itemsInfo[index].Color;
                return color;    
            }
        }
    }

    // Implement interface
    public partial class PaperOnboarding : IOnboardingContentViewDelegate
    {
        public void OnboardingConfiguration(OnboardingContentViewItem item, int index)
        {
            _delegate?.OnboardingConfigurationItem(item, index);
        }

        public OnboardingItemInfo OnboardingItemAtIndex(int index)
        {
            return itemsInfo[index];
        }
    }

    public partial class PaperOnboarding : IGestureControleDelegate
    {
        public void GestureControlDidSwipe(UISwipeGestureRecognizerDirection direction)
        {
            switch(direction)
            {
                case (UISwipeGestureRecognizerDirection.Right):
                    SetCurrentIndex(CurrentIndex - 1, true); break;
                case (UISwipeGestureRecognizerDirection.Left):
                    SetCurrentIndex(CurrentIndex + 1, true); break;
            }
        }
    }
}
