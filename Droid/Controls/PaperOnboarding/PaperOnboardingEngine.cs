using System;
using System.Collections.Generic;
using Android.Animation;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Java.Lang;

namespace XamControls.Droid.Controls
{
    public class PaperOnboardingEngine : PaperOnboardingEngineDefaults
    {
        // scale factor for converting dp to px
        private readonly float dpToPixelsScaleFactor;

        // main layout parts
        private readonly RelativeLayout mRootLayout;
        private readonly FrameLayout mContentTextContainer;
        private readonly FrameLayout mContentIconContainer;
        private readonly FrameLayout mBackgroundContainer;
        private readonly LinearLayout mPagerIconsContainer;

        private readonly RelativeLayout mContentRootLayout;
        private readonly LinearLayout mContentCenteredContainer;

        // application context
        private readonly Context mAppContext;

        // state variables
        private List<PaperOnboardingPage> mElements = new List<PaperOnboardingPage>();
        private int mActiveElementIndex = 0;

        // params for Pager position calculations, virtually final, but initializes in onGlobalLayoutListener
        private int mPagerElementActiveSize;
        private int mPagerElementNormalSize;
        private int mPagerElementLeftMargin;
        private int mPagerElementRightMargin;

        // Listeners
        private PaperOnboardingOnChangeListener mOnChangeListener;
        private PaperOnboardingOnRightOutListener mOnRightOutListener;
        private PaperOnboardingOnLeftOutListener mOnLeftOutListener;


        public PaperOnboardingEngine(View rootLayout, List<PaperOnboardingPage> contentElements, Context appContext)
        {
            if (contentElements == null || contentElements.Count == 0)
                throw new IllegalArgumentException("No content elements provided");

            this.mElements.AddRange(contentElements);
            this.mAppContext = appContext.ApplicationContext;

            mRootLayout = (RelativeLayout)rootLayout;
            mContentTextContainer = (FrameLayout)rootLayout.FindViewById(Resource.Id.onboardingContentTextContainer);
            mContentIconContainer = (FrameLayout)rootLayout.FindViewById(Resource.Id.onboardingContentIconContainer);
            mBackgroundContainer = (FrameLayout)rootLayout.FindViewById(Resource.Id.onboardingBackgroundContainer);
            mPagerIconsContainer = (LinearLayout)rootLayout.FindViewById(Resource.Id.onboardingPagerIconsContainer);

            mContentRootLayout = (RelativeLayout)mRootLayout.GetChildAt(1);
            mContentCenteredContainer = (LinearLayout)mContentRootLayout.GetChildAt(0);

            this.dpToPixelsScaleFactor = this.mAppContext.Resources.DisplayMetrics.Density;

            InitializeStartingState();
          
            var swipeListener = new OnSwipeListener(mAppContext);
            swipeListener.OnSwipeLeft += () => ToggleContent(false);
            swipeListener.OnSwipeLeft += () => ToggleContent(true);
            mRootLayout.SetOnTouchListener(swipeListener);

            var customGlobalLayout = new CustomGlobalLayoutListener();
            customGlobalLayout.OnGlobalLayoutChanged += () => OnGlobalLayoutChanged(customGlobalLayout);
            mRootLayout.ViewTreeObserver.AddOnGlobalLayoutListener(customGlobalLayout);
        }

        private void OnGlobalLayoutChanged(ViewTreeObserver.IOnGlobalLayoutListener listener)
        {
            mRootLayout.ViewTreeObserver.RemoveOnGlobalLayoutListener(listener);

            mPagerElementActiveSize = mPagerIconsContainer.Height;
            mPagerElementNormalSize = System.Math.Min(mPagerIconsContainer.GetChildAt(0).Height,
                                                      mPagerIconsContainer.GetChildAt(mPagerIconsContainer.ChildCount - 1).Height);

            ViewGroup.MarginLayoutParams layoutParams = (ViewGroup.MarginLayoutParams)mPagerIconsContainer.GetChildAt(0).LayoutParameters;
            mPagerElementLeftMargin = layoutParams.LeftMargin;
            mPagerElementRightMargin = layoutParams.RightMargin;

            mPagerIconsContainer.SetX(CalculateNewPagerPosition(0));
            mContentCenteredContainer.SetY((mContentRootLayout.Height - mContentCenteredContainer.Height / 2));
        }

        protected int CalculateNewPagerPosition(int newActiveElement)
        {
            newActiveElement++;
            if (newActiveElement <= 0)
                newActiveElement = 1;
            int pagerActiveElemCenterPosX = mPagerElementActiveSize / 2
                    + newActiveElement * mPagerElementLeftMargin
                    + (newActiveElement - 1) * (mPagerElementNormalSize + mPagerElementRightMargin);
            return mRootLayout.Width / 2 - pagerActiveElemCenterPosX;
        }

        protected void InitializeStartingState()
        {
            // Create bottom bar icons for all elements with big first icon
            for (int i = 0; i < mElements.Count; i++)
            {
                PaperOnboardingPage PaperOnboardingPage = mElements[i];
                ViewGroup bottomBarIconElement = createPagerIconElement(PaperOnboardingPage.BottomBarIconRes, i == 0);
                mPagerIconsContainer.AddView(bottomBarIconElement);
            }
            // Initialize first element on screen
            PaperOnboardingPage activeElement = getActiveElement();
            // initial content texts
            ViewGroup initialContentText = createContentTextView(activeElement);
            mContentTextContainer.AddView(initialContentText);
            // initial content icons
            ImageView initContentIcon = createContentIconView(activeElement);
            mContentIconContainer.AddView(initContentIcon);
            // initial bg color
            mRootLayout.SetBackgroundColor(activeElement.BackgroundColor);
        }

        protected int[] calculateCurrentCenterCoordinatesOfPagerElement(int activeElementIndex)
        {
            int y = (int)(mPagerIconsContainer.GetY() + mPagerIconsContainer.Height / 2);

            if (activeElementIndex >= mPagerIconsContainer.ChildCount)
                return new int[] { mRootLayout.Width / 2, y };

            View pagerElem = mPagerIconsContainer.GetChildAt(activeElementIndex);
            int x = (int)(mPagerIconsContainer.GetX() + pagerElem.GetX() + pagerElem.Width / 2);
            return new int[] { x, y };
        }

        protected void ToggleContent(bool prev)
        { 
        }

        public void setOnChangeListener(PaperOnboardingOnChangeListener onChangeListener)
        {
            this.mOnChangeListener = onChangeListener;
        }

        public void setOnRightOutListener(PaperOnboardingOnRightOutListener onRightOutListener)
        {
            this.mOnRightOutListener = onRightOutListener;
        }

        public void setOnLeftOutListener(PaperOnboardingOnLeftOutListener onLeftOutListener)
        {
            this.mOnLeftOutListener = onLeftOutListener;
        }

        protected AnimatorSet createBGAnimatorSet(Android.Graphics.Color color)
        {
            View bgColorView = new ImageView(mAppContext);
            bgColorView.LayoutParameters = new RelativeLayout.LayoutParams(mRootLayout.Width, mRootLayout.Height);
            bgColorView.SetBackgroundColor(color);
            mBackgroundContainer.AddView(bgColorView);

            int[] pos = calculateCurrentCenterCoordinatesOfPagerElement(mActiveElementIndex);

            float finalRadius = mRootLayout.Width > mRootLayout.Height ? mRootLayout.Width : mRootLayout.Height;

            AnimatorSet bgAnimSet = new AnimatorSet();
            Animator fadeIn = ObjectAnimator.OfFloat(bgColorView, "alpha", 0, 1);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Animator circularReveal = ViewAnimationUtils.CreateCircularReveal(bgColorView, pos[0], pos[1], 0, finalRadius);
                circularReveal.SetInterpolator(new AccelerateInterpolator());
                bgAnimSet.PlayTogether(circularReveal, fadeIn);
            }
            else
            {
                bgAnimSet.PlayTogether(fadeIn);
            }

            bgAnimSet.SetDuration(ANIM_BACKGROUND_TIME);
            var endListener = new AnimatorEndListener();
            endListener.OnEndAnimation += () => {
                mRootLayout.SetBackgroundColor(color);
                bgColorView.Visibility = ViewStates.Gone;
                mBackgroundContainer.RemoveView(bgColorView);
            };
            bgAnimSet.AddListener(endListener);
            return bgAnimSet;
        }

        private AnimatorSet createContentTextShowAnimation(View currentContentText, View newContentText)
        {
            int positionDeltaPx = dpToPixels(CONTENT_TEXT_POS_DELTA_Y_DP);
            AnimatorSet animations = new AnimatorSet();
            Animator currentContentMoveUp = ObjectAnimator.OfFloat(currentContentText, "y", 0, -positionDeltaPx);
            currentContentMoveUp.SetDuration(ANIM_CONTENT_TEXT_HIDE_TIME);
            var endListener = new AnimatorEndListener();
            endListener.OnEndAnimation += () => {
                mContentTextContainer.RemoveView(currentContentText);
            };
            currentContentMoveUp.AddListener(endListener);
           
            Animator currentContentFadeOut = ObjectAnimator.OfFloat(currentContentText, "alpha", 1, 0);
            currentContentFadeOut.SetDuration(ANIM_CONTENT_TEXT_HIDE_TIME);

            animations.PlayTogether(currentContentMoveUp, currentContentFadeOut);

            Animator newContentMoveUp = ObjectAnimator.OfFloat(newContentText, "y", positionDeltaPx, 0);
            newContentMoveUp.SetDuration(ANIM_CONTENT_TEXT_SHOW_TIME);

            Animator newContentFadeIn = ObjectAnimator.OfFloat(newContentText, "alpha", 0, 1);
            newContentFadeIn.SetDuration(ANIM_CONTENT_TEXT_SHOW_TIME);

            animations.PlayTogether(newContentMoveUp, newContentFadeIn);

            animations.SetInterpolator(new DecelerateInterpolator());

            return animations;
        }

        protected Animator createContentCenteringVerticalAnimation(View newContentText, View newContentIcon)
        {
            newContentText.Measure(View.MeasureSpec.MakeMeasureSpec(mContentCenteredContainer.Width, MeasureSpecMode.AtMost), -2);
            int measuredContentTextHeight = newContentText.MeasuredHeight;
            newContentIcon.Measure(-2, -2);
            int measuredContentIconHeight = newContentIcon.MeasuredWidth;

            int newHeightOfContent = measuredContentIconHeight + measuredContentTextHeight + ((ViewGroup.MarginLayoutParams)mContentTextContainer.LayoutParameters).TopMargin;
            Animator centerContentAnimation = ObjectAnimator.OfFloat(mContentCenteredContainer, "y", mContentCenteredContainer.GetY(),
                    (mContentRootLayout.Height - newHeightOfContent) / 2);
            centerContentAnimation.SetDuration(ANIM_CONTENT_CENTERING_TIME);
            centerContentAnimation.SetInterpolator(new DecelerateInterpolator());
            return centerContentAnimation;
        }

        protected AnimatorSet createPagerIconAnimation(int oldIndex, int newIndex)
        {
            AnimatorSet animations = new AnimatorSet();
            animations.SetDuration(ANIM_PAGER_ICON_TIME);

            // scale down whole old element
            ViewGroup oldActiveItem = (ViewGroup)mPagerIconsContainer.GetChildAt(oldIndex);
            LinearLayout.LayoutParams oldActiveItemParams = (LinearLayout.LayoutParams)oldActiveItem.LayoutParameters;
            ValueAnimator oldItemScaleDown = ValueAnimator.OfInt(mPagerElementActiveSize, mPagerElementNormalSize);

            var updateListener = new CustomAnimatorUpdateListener();
            updateListener.OnUpdateAnimaion += (valueAnimator) =>
            {
                oldActiveItemParams.Height = (int)valueAnimator.AnimatedValue;
                oldActiveItemParams.Width = (int)valueAnimator.AnimatedValue;
                oldActiveItem.RequestLayout();
            };

            oldItemScaleDown.AddUpdateListener(updateListener);

            // fade out old new element icon
            View oldActiveIcon = oldActiveItem.GetChildAt(1);
            Animator oldActiveIconFadeOut = ObjectAnimator.OfFloat(oldActiveIcon, "alpha", 1, 0);

            // fade in old element shape
            ImageView oldActiveShape = (ImageView)oldActiveItem.GetChildAt(0);
            oldActiveShape.SetImageResource(oldIndex - newIndex > 0 ? Resource.Drawable.onboarding_pager_circle_icon : Resource.Drawable.onboarding_pager_round_icon);
            Animator oldActiveShapeFadeIn = ObjectAnimator.OfFloat(oldActiveShape, "alpha", 0, PAGER_ICON_SHAPE_ALPHA);
            // add animations
            animations.PlayTogether(oldItemScaleDown, oldActiveIconFadeOut, oldActiveShapeFadeIn);

            // scale up whole new element
            ViewGroup newActiveItem = (ViewGroup)mPagerIconsContainer.GetChildAt(newIndex);
            LinearLayout.LayoutParams newActiveItemParams = (LinearLayout.LayoutParams)newActiveItem.LayoutParameters;
            ValueAnimator newItemScaleUp = ValueAnimator.OfInt(mPagerElementNormalSize, mPagerElementActiveSize);

            var updateItemListener = new CustomAnimatorUpdateListener();
            updateItemListener.OnUpdateAnimaion += (valueAnimator) =>
            {
                newActiveItemParams.Height = (int)valueAnimator.AnimatedValue;
                newActiveItemParams.Width = (int)valueAnimator.AnimatedValue;
                newActiveItem.RequestLayout();
            };
            newItemScaleUp.AddUpdateListener(updateItemListener);

            // fade in new element icon
            View newActiveIcon = newActiveItem.GetChildAt(1);
            Animator newActiveIconFadeIn = ObjectAnimator.OfFloat(newActiveIcon, "alpha", 0, 1);

            // fade out new element shape
            ImageView newActiveShape = (ImageView) newActiveItem.GetChildAt(0);
            Animator newActiveShapeFadeOut = ObjectAnimator.OfFloat(newActiveShape, "alpha", PAGER_ICON_SHAPE_ALPHA, 0);

            // add animations
            animations.PlayTogether(newItemScaleUp, newActiveShapeFadeOut, newActiveIconFadeIn);

            animations.SetInterpolator(new DecelerateInterpolator());
            return animations;
        }

        protected ViewGroup createPagerIconElement(int iconDrawableRes, bool isActive)
        {
            LayoutInflater vi = LayoutInflater.From(mAppContext);
            FrameLayout bottomBarElement = (FrameLayout)vi.Inflate(Resource.Layout.onboarding_pager_layout, mPagerIconsContainer, false);
            ImageView elementShape = (ImageView)bottomBarElement.GetChildAt(0);
            ImageView elementIcon = (ImageView)bottomBarElement.GetChildAt(1);
            elementIcon.SetImageResource(iconDrawableRes);
            if (isActive)
            {
                LinearLayout.LayoutParams layoutParams = (LinearLayout.LayoutParams)bottomBarElement.LayoutParameters;
                layoutParams.Width = mPagerIconsContainer.LayoutParameters.Height;
                layoutParams.Height = mPagerIconsContainer.LayoutParameters.Height;
                elementShape.Alpha = (0f);
                elementIcon.Alpha = (1f);
            }
            else
            {
                elementShape.Alpha = (PAGER_ICON_SHAPE_ALPHA);
                elementIcon.Alpha = (0f);
            }
            return bottomBarElement;
        }

        protected ViewGroup createContentTextView(PaperOnboardingPage PaperOnboardingPage)
        {
            LayoutInflater vi = LayoutInflater.From(mAppContext);
            ViewGroup contentTextView = (ViewGroup)vi.Inflate(Resource.Layout.onboarding_text_content_layout, mContentTextContainer, false);
            TextView contentTitle = (TextView)contentTextView.GetChildAt(0);
            contentTitle.Text = PaperOnboardingPage.TitleText;
            TextView contentText = (TextView)contentTextView.GetChildAt(1);
            contentText.Text = PaperOnboardingPage.DescriptionText;
            return contentTextView;
        }

        protected ImageView createContentIconView(PaperOnboardingPage PaperOnboardingPage)
        {
            ImageView contentIcon = new ImageView(mAppContext);
            contentIcon.SetImageResource(PaperOnboardingPage.ContentIconRes);
            FrameLayout.LayoutParams iconLP = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            iconLP.Gravity = GravityFlags.Center;
            contentIcon.LayoutParameters = (iconLP);
            return contentIcon;
        }

        public int getActiveElementIndex()
        {
            return mActiveElementIndex;
        }

        protected PaperOnboardingPage getActiveElement()
        {
            return mElements.Count > mActiveElementIndex ? mElements[mActiveElementIndex] : null;
        }

        protected PaperOnboardingPage toggleToPreviousElement()
        {
            if (mActiveElementIndex - 1 >= 0)
            {
                mActiveElementIndex--;
                return mElements.Count > mActiveElementIndex ? mElements[mActiveElementIndex] : null;
            }
            else
                return null;
        }

        protected PaperOnboardingPage toggleToNextElement()
        {
            if (mActiveElementIndex + 1 < mElements.Count)
            {
                mActiveElementIndex++;
                return mElements.Count > mActiveElementIndex ? mElements[mActiveElementIndex] : null;
            }
            else
                return null;
        }

        protected int dpToPixels(int dpValue)
        {
            return (int)(dpValue * dpToPixelsScaleFactor + 0.5f);
        }
    }

    public class CustomGlobalLayoutListener : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
    {
        public Action OnGlobalLayoutChanged;

        public void OnGlobalLayout()
        {
            this.OnGlobalLayoutChanged?.Invoke();
        }
    }

    public class CustomAnimatorUpdateListener : Java.Lang.Object, ValueAnimator.IAnimatorUpdateListener
    {
        public Action<ValueAnimator> OnUpdateAnimaion;

        public void OnAnimationUpdate(ValueAnimator animation)
        {
            OnUpdateAnimaion?.Invoke(animation);
        }
    }
}
