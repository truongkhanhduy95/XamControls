using System;
using System.Drawing;
using Android.Animation;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Views.Animations;

namespace XamControls.Droid.Controls
{
    public class TickerView : View
    {
        private static  int DEFAULT_TEXT_SIZE = 12;
        //private static  Color DEFAULT_TEXT_COLOR = Color.Black;
        private static  int DEFAULT_ANIMATION_DURATION = 350;
        private static  IInterpolator DEFAULT_ANIMATION_INTERPOLATOR =
            new AccelerateDecelerateInterpolator();
        private static GravityFlags DEFAULT_GRAVITY = GravityFlags.Start;

        private Paint textPaint = new TextPaint(PaintFlags.AntiAlias);

        private TickerDrawMetrics metrics;
        private TickerColumnManager columnManager;
        private ValueAnimator animator = ValueAnimator.OfFloat(1f);

        // Minor optimizations for re-positioning the canvas for the composer.
        private  Rect viewBounds = new Rect();

        private string text;

        private int lastMeasuredDesiredWidth, lastMeasuredDesiredHeight;

        // View attributes, defaults are set in init().
        private GravityFlags gravity;
        private int textColor;
        private float textSize;
        private int textStyle;
        private long animationDelayInMillis;
        private long animationDurationInMillis;
        private IInterpolator animationInterpolator;
        private bool animateMeasurementChange;
        // pending text set from XML because we didn't have a character list initially
        private String pendingTextToSet;

        public TickerView(Context ctx) : base(ctx)
        {
            Init(ctx, null);
        }

        public TickerView(Context ctx, IAttributeSet attrs) : base(ctx, attrs)
        {
            Init(ctx, null);
        }

        public TickerView(Context ctx, IAttributeSet attrs, int defStyle) : base(ctx, attrs, defStyle)
        {
            Init(ctx, null);
        }

        protected void Init(Context context, IAttributeSet attrs)
        {
            metrics = new TickerDrawMetrics(textPaint);
            columnManager = new TickerColumnManager(metrics);

            // After we've fetched the correct values for the attributes, set them on the view
            animationInterpolator = DEFAULT_ANIMATION_INTERPOLATOR;

            this.animationDurationInMillis = DEFAULT_ANIMATION_DURATION;
            this.animateMeasurementChange = false;
            this.gravity = DEFAULT_GRAVITY;


            //setTextColor(styledAttributes.textColor);
            //setTextSize(styledAttributes.textSize);

            //int defaultCharList = 0;
            //switch (defaultCharList)
            //{
            //    case 1:
            //        setCharacterLists(TickerUtils.provideNumberList());
            //        break;
            //    case 2:
            //        setCharacterLists(TickerUtils.provideAlphabeticalList());
            //        break;
            //    default:
            //        if (isInEditMode())
            //        {
            //            setCharacterLists(TickerUtils.provideNumberList());
            //        }
            //}

            //if (isCharacterListsSet())
            //{
            //    setText(styledAttributes.text, false);
            //}
            //else
            //{
            //    this.pendingTextToSet = styledAttributes.text;
            //}


            //animator.addUpdateListener(new ValueAnimator.AnimatorUpdateListener() {
            //@Override
            //public void onAnimationUpdate(ValueAnimator animation)
            //{
            //    columnManager.setAnimationProgress(
            //            animation.getAnimatedFraction());
            //    checkForRelayout();
            //    invalidate();
            //}
            //});
            //animator.addListener(new AnimatorListenerAdapter()
            //{
            //    @Override
            //    public void onAnimationEnd(Animator animation)
            //    {
            //        columnManager.onAnimationEnd();
            //        checkForRelayout();
            //        invalidate();
            //    }
            //});
        }

        public void setCharacterLists(char[] characterLists)
        {
            columnManager.setCharacterLists(characterLists);
            if (pendingTextToSet != null)
            {
                setText(pendingTextToSet, false);
                pendingTextToSet = null;
            }
        }

        public bool isCharacterListsSet()
        {
            return columnManager.getCharacterLists() != null;
        }

        public void setText(string text)
        {
            setText(text, !string.IsNullOrEmpty(text));
        }

        public void setText(string text, bool animate)
        {
            if (string.Equals(text, this.text))
            {
                return;
            }

            this.text = text;
            char[] targetText = text == null ? new char[0] : text.ToCharArray();

            columnManager.setText(targetText);
            ContentDescription = text;

            if (animate)
            {
                // Kick off the animator that draws the transition
                if (animator.IsRunning)
                {
                    animator.Cancel();
                }

                animator.StartDelay = animationDelayInMillis;
                animator.SetDuration(animationDurationInMillis);
                animator.SetInterpolator(animationInterpolator);
                animator.Start();
            }
            else
            {
                columnManager.setAnimationProgress(1f);
                columnManager.onAnimationEnd();
                checkForRelayout();
                Invalidate();
            }
        }

        public string getText()
        {
            return text;
        }

        public int getTextColor()
        {
            return textColor;
        }

        //public void setTextColor( Color color)
        //{
        //    if (this.textColor != color)
        //    {
        //        textColor = color;
        //        textPaint.Color = (textColor);
        //        Invalidate();
        //    }
        //}

        public float getTextSize()
        {
            return textSize;
        }

        public void setTextSize(float textSize)
        {
            if (this.textSize != textSize)
            {
                this.textSize = textSize;
                textPaint.TextSize = textSize;
                onTextPaintMeasurementChanged();
            }
        }

        public long getAnimationDelay()
        {
            return animationDelayInMillis;
        }

        public void setAnimationDelay(long animationDelayInMillis)
        {
            this.animationDelayInMillis = animationDelayInMillis;
        }

        public long getAnimationDuration()
        {
            return animationDurationInMillis;
        }

        public void setAnimateMeasurementChange(bool animateMeasurementChange)
        {
            this.animateMeasurementChange = animateMeasurementChange;
        }

        public void addAnimatorListener(Animator.IAnimatorListener animatorListener)
        {
            animator.AddListener(animatorListener);
        }

        private void checkForRelayout()
        {
            bool widthChanged = lastMeasuredDesiredWidth != computeDesiredWidth();
            bool heightChanged = lastMeasuredDesiredHeight != computeDesiredHeight();

            if (widthChanged || heightChanged)
            {
                RequestLayout();
            }
        }

        private int computeDesiredWidth()
        {
            int contentWidth = (int)(animateMeasurementChange ?
                    columnManager.getCurrentWidth() : columnManager.getMinimumRequiredWidth());
            return contentWidth + PaddingLeft + PaddingRight;
        }

        private int computeDesiredHeight()
        {
            return (int)metrics.CharHeight + PaddingTop + PaddingBottom;
        }

        private void onTextPaintMeasurementChanged()
        {
            metrics.invalidate();
            checkForRelayout();
            Invalidate();
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            lastMeasuredDesiredWidth = computeDesiredWidth();
            lastMeasuredDesiredHeight = computeDesiredHeight();

            int desiredWidth = ResolveSize(lastMeasuredDesiredWidth, widthMeasureSpec);
            int desiredHeight = ResolveSize(lastMeasuredDesiredHeight, heightMeasureSpec);

            SetMeasuredDimension(desiredWidth, desiredHeight);
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
            viewBounds.Set(PaddingLeft, PaddingTop, w - PaddingRight,
                h - PaddingBottom);
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            canvas.Save();

            realignAndClipCanvasForGravity(canvas);

            // canvas.drawText writes the text on the baseline so we need to translate beforehand.
            canvas.Translate(0f, metrics.CharBaseline);

            columnManager.draw(canvas, textPaint);

            canvas.Restore();
        }

        private void realignAndClipCanvasForGravity(Canvas canvas)
        {
            float currentWidth = columnManager.getCurrentWidth();
            float currentHeight = metrics.CharHeight;
            realignAndClipCanvasForGravity(canvas, gravity, viewBounds, currentWidth, currentHeight);
        }


        static void realignAndClipCanvasForGravity(Canvas canvas, GravityFlags gravity, Rect viewBounds,
           float currentWidth, float currentHeight)
        {
            int availableWidth = viewBounds.Width();
            int availableHeight = viewBounds.Height();

            float translationX = 0;
            float translationY = 0;
            if ((gravity & GravityFlags.CenterVertical) == GravityFlags.CenterVertical)
            {
                translationY = viewBounds.Top + (availableHeight - currentHeight) / 2f;
            }
            if ((gravity & GravityFlags.CenterHorizontal) == GravityFlags.CenterHorizontal)
            {
                translationX = viewBounds.Left + (availableWidth - currentWidth) / 2f;
            }
            if ((gravity & GravityFlags.Top) == GravityFlags.Top)
            {
                translationY = 0;
            }
            if ((gravity & GravityFlags.Bottom) == GravityFlags.Bottom)
            {
                translationY = viewBounds.Top + (availableHeight - currentHeight);
            }
            if ((gravity & GravityFlags.Start) == GravityFlags.Start)
            {
                translationX = 0;
            }
            if ((gravity & GravityFlags.End) == GravityFlags.End)
            {
                translationX = viewBounds.Left + (availableWidth - currentWidth);
            }

            canvas.Translate(translationX, translationY);
            canvas.ClipRect(0f, 0f, currentWidth, currentHeight);
        }
    }
}
