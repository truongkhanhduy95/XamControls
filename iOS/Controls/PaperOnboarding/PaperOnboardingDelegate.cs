using System;
namespace XamControls.iOS.Controls
{
    public class PaperOnboardingDelegate
    {
        public virtual bool EnableTapOnPageControl => false;

        public PaperOnboardingDelegate()
        {
        }

        public virtual void OnboardingWillTransitonToIndex(int index)
        {
        }

        public virtual void OnboardingWillTransitonToLeaving()
        {
        }

        public virtual void OnboardingDidTransitonToIndex(int index)
        {
        }

        public virtual void OnboardingConfigurationItem(OnboardingContentViewItem item, int index)
        {
            
        }
    }
}
