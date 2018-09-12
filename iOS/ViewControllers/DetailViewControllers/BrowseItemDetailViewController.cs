using System;
using Cirrious.FluentLayouts.Touch;
using UIKit;
using XamControls.iOS.Controls;

namespace XamControls.iOS
{
    public partial class BrowseItemDetailViewController : UIViewController
    {
        public ItemDetailViewModel ViewModel { get; set; }
        public BrowseItemDetailViewController(IntPtr handle) : base(handle) { }

        private Slider slider;

        private UITextField input;
        private UIButton btnAdd;
        private TagListView tagsView;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = ViewModel.Title;
            ItemNameLabel.Text = ViewModel.Item.Text;
            ItemDescriptionLabel.Text = ViewModel.Item.Description;

            //Sample FluidSlider
            SetupSlider();

            //Sample TagView
            SetupTagView();
        }

        private void SetupSlider()
        {
            slider = new Slider();
            slider.ValueViewColor = UIColor.White;
            slider.ContentViewColor = UIColor.FromRGB(52, 152, 219);
            slider.Fraction = 0.5f;
            slider.From = 5;
            slider.To = 25;
            slider.Frame = new CoreGraphics.CGRect(20, 220, this.View.Bounds.Width - 40, 40);
            slider.ContentViewCornerRadius = 8;

            this.View.AddSubview(slider);
        }

        private void SetupTagView()
        {
            this.input = new UITextField { BorderStyle = UITextBorderStyle.RoundedRect };
            this.btnAdd = new UIButton();
            this.btnAdd.SetTitle("Add", UIControlState.Normal);
            this.btnAdd.SetTitleColor(UIColor.Blue, UIControlState.Normal);
            this.btnAdd.TouchUpInside += this.BtnAdd_TouchUpInside;
            this.tagsView = new TagListView(true)
            {
                PaddingY = 4f,
                TextFont = UIFont.SystemFontOfSize(20f)
            };
            this.tagsView.TagButtonTapped += (sender, e) =>
            {
                this.tagsView.RemoveTag(e);
            };
            this.tagsView.TagSelected += (sender, e) =>
            {
                var alert = UIAlertController.Create("TagListView Sample", $"Selected item source: {e.ToString()}", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, (obj) => this.DismissViewController(true, null)));
                this.PresentViewController(alert, true, null);
            };

            //Props
            this.tagsView.Alignment = TagsAlignment.Left;
            this.tagsView.CornerRadius = 17f;
            //this.tagsView.PaddingX = 5f;
            this.tagsView.PaddingX = 8f;
            this.tagsView.ControlsDistance = 4f;
            this.tagsView.TagBackgroundColor = UIColor.FromRGB(52, 152, 219);

            this.View.AddSubviews(this.input, this.btnAdd, this.tagsView);
            this.View.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();

            this.View.AddConstraints(
                this.input.AtTopOf(this.View, 300f),
                this.input.AtLeftOf(this.View),

                this.btnAdd.AtRightOf(this.View),
                this.btnAdd.ToRightOf(this.input),
                this.btnAdd.WithSameCenterY(this.input),

                this.tagsView.Below(this.input, 20f),
                this.tagsView.AtLeftOf(this.View, 5f),
                this.tagsView.AtRightOf(this.View, 5f)
            );
        }

        private void BtnAdd_TouchUpInside(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(this.input.Text))
            {
                tagsView.AddTag(this.input.Text);
                this.input.Text = string.Empty;
            }
        }
    }
}
