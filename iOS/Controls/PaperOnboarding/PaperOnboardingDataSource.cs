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

        public abstract UIColor OnboardingPageItemColor(int index);

        public abstract float OnboardingPageItemRadius();

        public virtual UIColor OnboardingPageItemColor() { return UIColor.White; }

        public virtual float OnboardinPageItemRadius() { return 8; }

        public virtual float OnboardingPageItemSelectedRadius() { return 22; }

    }
}
