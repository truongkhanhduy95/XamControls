using System;
using UIKit;
using XamControls.iOS.Controls;

namespace XamControls.iOS
{
    public partial class BrowseItemDetailViewController : UIViewController
    {
        public ItemDetailViewModel ViewModel { get; set; }
        public BrowseItemDetailViewController(IntPtr handle) : base(handle) { }

        private Slider slider;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = ViewModel.Title;
            ItemNameLabel.Text = ViewModel.Item.Text;
            ItemDescriptionLabel.Text = ViewModel.Item.Description;

            slider = new Slider();
            slider.ValueViewColor = UIColor.White;
            slider.ContentViewColor = UIColor.FromRGB(52 , 152 , 219 );
            slider.Fraction = 0.5f;
			slider.From = 5;
			slider.To = 25;
            slider.Frame = new CoreGraphics.CGRect(20, 220, this.View.Bounds.Width - 40, 40);
            slider.ContentViewCornerRadius = 8;
            //slider.BackgroundColor = UIColor.Gray;

            this.View.AddSubview(slider);

        }
    }
}
