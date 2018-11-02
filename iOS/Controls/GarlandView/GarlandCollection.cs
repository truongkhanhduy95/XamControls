using System;
using Foundation;
using UIKit;

namespace XamControls.iOS.Controls
{
    public class GarlandCollection : UICollectionView
    {
        public GarlandCollection(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        //public GarlandCollection(string x)
        //{
        //    Initialize();
        //}

        private void Initialize()
        {
            var config = GarlandConfig.Instance;
            nfloat sideInset = (UIScreen.MainScreen.Bounds.Width - config.CardSize.Width) / 2;

            var layout = new UICollectionViewFlowLayout();
            layout.SectionInset = new UIEdgeInsets(0, sideInset, 0, sideInset);
            layout.ItemSize = config.CardSize;
            layout.MinimumLineSpacing = config.CardSpacing;
            layout.ScrollDirection = UICollectionViewScrollDirection.Vertical;

            this.Frame = CoreGraphics.CGRect.Empty;
            this.CollectionViewLayout = layout;

            Setup();
        }

        private void Setup()
        {
            ShowsVerticalScrollIndicator = false;
            ShowsHorizontalScrollIndicator = false;
            DelaysContentTouches = true;
            ClipsToBounds = true;
            BackgroundColor = UIColor.Clear;
        }
    }
}
