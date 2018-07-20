using System;
using System.Collections.Generic;
using UIKit;
using XamControls.iOS.Controls;

namespace XamControls.iOS
{
    public partial class AboutViewController : UIViewController
    {
        public AboutViewModel ViewModel { get; set; }
        public AboutViewController(IntPtr handle) : base(handle)
        {
            ViewModel = new AboutViewModel();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = ViewModel.Title;

            SetUpOnboardingView();
        }

        private void SetUpOnboardingView()
        {
            var dataSource = new OnboardingDataSource(CreateItems());

            var onboarding = new PaperOnboarding();
            onboarding.DataSource = dataSource;
            onboarding.TranslatesAutoresizingMaskIntoConstraints = false;

            View.AddSubview(onboarding);

            var attrs = new NSLayoutAttribute[] { NSLayoutAttribute.Left, NSLayoutAttribute.Right, NSLayoutAttribute.Top, NSLayoutAttribute.Bottom };
            foreach(var attr in attrs)
            {
                var constraint = NSLayoutConstraint.Create(
                    onboarding,
                    attr,
                    NSLayoutRelation.Equal,
                    View,
                    attr,
                    1,
                    0
                );
                View.AddConstraint(constraint);
            }
        }

        private List<OnboardingItemInfo> CreateItems()
        {
            var items = new List<OnboardingItemInfo>();
            items.Add(new OnboardingItemInfo(
                UIImage.FromBundle("Hotels"),
                "Hotels",
                "All hotels and hostels are sorted by hospitality rating",
                UIImage.FromBundle("Key"),
                UIColor.FromRGB(0.4f, 0.56f, 0.71f),
                UIColor.White,
                UIColor.White,
                null, null)
                     );
            items.Add(new OnboardingItemInfo(
               UIImage.FromBundle("Banks"),
                "Banks",
                "We carefully verify all banks before add them into the app",
               UIImage.FromBundle("Wallet"),
               UIColor.FromRGB(0.4f, 0.69f, 0.71f),
               UIColor.White,
               UIColor.White,
               null, null)
                    );
            items.Add(new OnboardingItemInfo(
                UIImage.FromBundle("Stores"),
                "Stores",
                "All local stores are categorized for your convenience",
               UIImage.FromBundle("ShoppingCard"),
               UIColor.FromRGB(0.61f, 0.56f, 0.74f),
               UIColor.White,
               UIColor.White,
               null, null)
                    );

            return items;
        }

    }

    public class OnboardingDataSource : PaperOnboardingDataSource
    {
        public List<OnboardingItemInfo> Items { get; private set; }

        public OnboardingDataSource(List<OnboardingItemInfo> items)
        {
            Items = items;
        }


        public override OnboardingItemInfo OnboardingItem(int index)
        {
            return Items[index];
        }

        public override int OnboardingItemsCount()
        {
            return Items.Count;
        }
    }
}
