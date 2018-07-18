using System;
using Android.Animation;

namespace XamControls.Droid.Controls
{
    public class AnimatorEndListener : Java.Lang.Object, Animator.IAnimatorListener
    {
        public Action OnEndAnimation;

        public void OnAnimationCancel(Animator animation)
        {
            
        }

        public void OnAnimationEnd(Animator animation)
        {
            OnEndAnimation?.Invoke();
        }

        public void OnAnimationRepeat(Animator animation)
        {
            
        }

        public void OnAnimationStart(Animator animation)
        {
            
        }
    }
}
