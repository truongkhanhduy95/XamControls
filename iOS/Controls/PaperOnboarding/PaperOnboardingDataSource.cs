using System;
using UIKit;

namespace XamControls.iOS.Controls
{
    public abstract class PaperOnboardingDataSource 
    {
        public PaperOnboardingDataSource()
        {
        }

        public abstract int OnboardingItemsCount();

        public abstract OnboardingItemInfo OnboardingItem(int index);

        public virtual UIColor OnboardingPageItemColor(int index) { return UIColor.White; }

        public virtual float OnboardingPageItemRadius() { return 8; }

        public virtual float OnboardingPageItemSelectedRadius() { return 22; }

    }
}
