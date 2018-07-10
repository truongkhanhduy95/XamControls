using System;
using System.Collections.Generic;
using Android.Animation;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Animations;

namespace XamControls.Droid.Controls
{
    public enum Size
    {
        //Value caculated in dp

        NORMAL = 56,
        SMALL = 40,
    }

    public class FluidSlider : View
    {
        #region CONST
        const double BAR_CORNER_RADIUS = 2;
        const double BAR_VERTICAL_OFFSET = 1.5f;
        const double BAR_INNER_HORIZONTAL_OFFSET = 0; // TODO: remove

        const double SLIDER_WIDTH = 4;
        const double SLIDER_HEIGHT = 1 + BAR_VERTICAL_OFFSET;

        const double TOP_CIRCLE_DIAMETER = 1;
        const double BOTTOM_CIRCLE_DIAMETER = 25.0f;
        const double TOUCH_CIRCLE_DIAMETER = 1;
        const double LABEL_CIRCLE_DIAMETER = 10;

        const double ANIMATION_DURATION = 400;
        const float TOP_SPREAD_FACTOR = 0.4f;
        const float BOTTOM_START_SPREAD_FACTOR = 0.25f;
        const float BOTTOM_END_SPREAD_FACTOR = 0.1f;
        const float METABALL_HANDLER_FACTOR = 2.4f;
        const float METABALL_MAX_DISTANCE = 15.0f;
        const float METABALL_RISE_DISTANCE = 1.1f;

        const double TEXT_SIZE = 12;
        const double TEXT_OFFSET = 8;
        const string TEXT_START = "0";
        const string TEXT_END = "100";

        const string COLOR_BAR = "#6168E7";
        Color COLOR_LABEL = Color.White;
        Color COLOR_LABEL_TEXT = Color.Black;
        Color COLOR_BAR_TEXT = Color.White;

        const double INITIAL_POSITION = 0.5f;
        #endregion

        #region Props
        private float barHeight;

        private int desiredWidth;
        private int desiredHeight;

        private float topCircleDiameter;
        private float bottomCircleDiameter;
        private float touchRectDiameter;
        private float labelRectDiameter;

        private float metaballMaxDistance;
        private float metaballRiseDistance;
        private float textOffset;

        private float barVerticalOffset;
        private float barCornerRadius;
        private float barInnerOffset;

        private RectF rectBar = new RectF();
        private RectF rectTopCircle= new RectF();
        private RectF rectBottomCircle= new RectF();
        private RectF rectTouch= new RectF();
        private RectF rectLabel= new RectF();
        private Rect rectText= new Rect();
        private Path pathMetaball = new Path();

        private Paint paintBar;
        private Paint paintLabel;
        private Paint paintText;

        private double maxMovement;
        private double touchX;
        #endregion

        Color colorBubbleText;
        Color colorBarText;

        private long duration;
        public long Duration
        {
            get { return (long)ANIMATION_DURATION; }
            set { duration = Math.Abs(value); }
        }

        private Size size = Size.SMALL;
        public Size Size
        {
            get { return size; }
            set { size = value; }
        }

        public Color ColorBar 
        {
            get { return paintBar.Color; }
            set
            {
                paintBar.Color = (value); //TODO: Handle parse color to int
            }
        }

        public Color ColorBubble
        {
            get { return paintBar.Color; }
            set
            {
                paintBar.Color = (value); //TODO: Handle parse color to int
            }
        }

        public float TextSize
        {
            get { return paintText.TextSize; }
            set
            {
                paintText.TextSize = value;
            }
        }

        private string bubbleText;
        public string StartText = TEXT_START;
        public string EndText = TEXT_END;

        public Action<float> OnPositionChanged;
        public Action OnBeginTracking;
        public Action OnEndTracking;

        private float position = (float)INITIAL_POSITION;
        public float Position
        {
            get { return position; }
            set 
            {
                var field = Math.Max(0, Math.Min(1, value));
                Invalidate();
                position = value;
                OnPositionChanged?.Invoke(field);
            }
        }

        protected Context _context;

        public FluidSlider(Context context) : base(context)
        {
            Init();
        }

        public FluidSlider(Context context, IAttributeSet attributeSet) 
            : base(context, attributeSet)
        {
            _context = context;
            Init();
        }

        public FluidSlider(Context context, IAttributeSet attributeSet, int defStyle) 
            : base(context, attributeSet, defStyle)
        {
            _context = context;
            Init();
        }

        private void Init()
        {
            paintBar = new Paint(PaintFlags.AntiAlias);
            paintBar.SetStyle(Paint.Style.Fill);

            paintLabel = new Paint(PaintFlags.AntiAlias);
            paintLabel.SetStyle(Paint.Style.Fill);

            paintText = new Paint(PaintFlags.AntiAlias);

            var density = _context.Resources.DisplayMetrics.Density;

            ColorBar = Color.ParseColor(COLOR_BAR);
            //ColorBubble = COLOR_LABEL;
            TextSize = (float)(TEXT_SIZE * density);
            barHeight = (float)(size) * density;

            desiredWidth = ((int)(barHeight * SLIDER_WIDTH));
            desiredHeight = ((int)(barHeight * SLIDER_HEIGHT));

            topCircleDiameter = (float)(barHeight * TOP_CIRCLE_DIAMETER);
            bottomCircleDiameter = (float)(barHeight * BOTTOM_CIRCLE_DIAMETER);
            touchRectDiameter = (float)(barHeight * TOUCH_CIRCLE_DIAMETER);
            labelRectDiameter = (float)(barHeight - LABEL_CIRCLE_DIAMETER * density);

            metaballMaxDistance = (float)(barHeight * METABALL_MAX_DISTANCE);
            metaballRiseDistance = (float)(barHeight * METABALL_RISE_DISTANCE);

            barVerticalOffset = (float)(barHeight * BAR_VERTICAL_OFFSET);
            barCornerRadius = (float)(BAR_CORNER_RADIUS * density);
            barInnerOffset = (float)(BAR_INNER_HORIZONTAL_OFFSET * density);
            textOffset = (float)(TEXT_OFFSET * density);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                OutlineProvider = new CustomOutlineProvider(rectBar, barCornerRadius);
            }
        }

        protected override IParcelable OnSaveInstanceState()
        {
            return new State(base.OnSaveInstanceState(), position, StartText, EndText, TextSize,
                             ColorBubble, ColorBar, colorBarText, colorBubbleText, duration);
        }

        protected override void OnRestoreInstanceState(IParcelable state)
        {
            base.OnRestoreInstanceState(state);
            if (state is State)
            {
                position = ((State)state).Position;
                StartText = ((State)state).StartText;
                EndText = ((State)state).EndText;
                TextSize = ((State)state).TextSize;
                //ColorBubble = ((State)state).ColorLabel;
                //ColorBar = ((State)state).ColorBar;
                //colorBarText = ((State)state).ColorBarText;
                //colorBubbleText = ((State)state).ColorLabelText;
                duration = ((State)state).Duaration;
            }
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            var w = ResolveSizeAndState(desiredWidth, widthMeasureSpec, 0);
            var h = ResolveSizeAndState(desiredHeight, heightMeasureSpec, 0);
            SetMeasuredDimension(w, h);
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);

            var width = w;
            rectBar.Set(0f, barVerticalOffset, width, barVerticalOffset + barHeight);
            rectTopCircle.Set(0f, barVerticalOffset, topCircleDiameter, barVerticalOffset + topCircleDiameter);
            rectBottomCircle.Set(0f, barVerticalOffset, bottomCircleDiameter, barVerticalOffset + bottomCircleDiameter);
            rectTouch.Set(0f, barVerticalOffset, touchRectDiameter, barVerticalOffset + touchRectDiameter);

            var vOffset = barVerticalOffset + (topCircleDiameter - labelRectDiameter) / 2f;
            rectLabel.Set(0f, vOffset, labelRectDiameter, vOffset + labelRectDiameter);

            maxMovement = width - touchRectDiameter - barInnerOffset * 2;
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            // Draw slider bar and text
            canvas.DrawRoundRect(rectBar, barCornerRadius, barCornerRadius, paintBar);

            if(!string.IsNullOrEmpty(StartText))
                DrawText(canvas, paintText, StartText, Paint.Align.Left, colorBarText, textOffset, rectBar, rectText);
            if (!string.IsNullOrEmpty(EndText))
                DrawText(canvas, paintText, EndText, Paint.Align.Right, colorBarText, textOffset, rectBar, rectText);

            // Draw metaball
            var x = barInnerOffset + touchRectDiameter / 2 + maxMovement * position;
            OffsetRectToPosition((float)x, new RectF[] { rectTouch, rectTopCircle, rectBottomCircle, rectLabel });

            DrawMetaball(canvas, paintBar, pathMetaball, rectBottomCircle, rectTopCircle, rectBar.Top, metaballRiseDistance, metaballMaxDistance, barCornerRadius);

            // Draw label and text
            canvas.DrawOval(rectLabel, paintLabel);

            var text = string.IsNullOrEmpty(bubbleText) ? ((int)(position * 100)).ToString() : bubbleText;
            DrawText(canvas, paintText, text, Paint.Align.Center, colorBubbleText, 0f, rectLabel, rectText);
        }

        private void OffsetRectToPosition(float pos, RectF[] rects)
        {
            foreach (var rect in rects)
            {
                rect.OffsetTo(position - rect.Width() / 2f, rect.Top);
            }
        }

        public override bool PerformClick()
        {
            base.PerformClick();
            return true;
        }

        private Tuple<float, float> GetVector(float radians, float length)
        {
            float x = (float)(Math.Cos(radians) * length);
            float y = (float)(Math.Sin(radians) * length);
            return new Tuple<float, float>(x, y);
        }
       
        private List<float> AppendToTuple(Tuple<float, float> tuple, float var1 = 0, float var2 = 0)
        {
            return new List<float>(){tuple.Item1 + var1, tuple.Item2 + var2};
        }

        private float getVectorLength(float x1, float y1, float x2, float y2)
        {
            var x = x1 - x2;
            var y = y1 - y2;
            return (float)Math.Sqrt(x * x + y * y);
        }

        private void DrawMetaball(Canvas canvas,
                                  Paint paint,
                                  Path path,
                                  RectF circle1,
                                  RectF circle2,
                                  float topBorder,
                                  float riseDistance,
                                  float maxDistance,
                                  float cornerRadius,
                                  float topSpreadFactor = TOP_SPREAD_FACTOR,
                                  float bottomStartSpreadFactor = BOTTOM_START_SPREAD_FACTOR,
                                  float bottomEndSpreadFactor = BOTTOM_END_SPREAD_FACTOR,
                                  float handleRate = METABALL_HANDLER_FACTOR)
        {
            var radius1 = circle1.Width() / 2.0f;
            var radius2 = circle2.Width() / 2.0f;

            if (radius1 == 0.0f || radius2 == 0.0f)
            {
                return;
            }

            var d = getVectorLength(circle1.CenterX(), circle1.CenterY(), circle2.CenterX(), circle2.CenterY());
            if (d > maxDistance || d <= Math.Abs(radius1 - radius2))
            {
                return;
            }

                var riseRatio = Math.Min(1f, Math.Max(0f, topBorder - circle2.Top) / riseDistance);

            float u1, u2;
            if (d < radius1 + radius2)
            { // case circles are overlapping
                u1 = (float)Math.Acos((radius1 * radius1 + d * d - radius2 * radius2) / (2 * radius1 * d));
                u2 = (float)Math.Acos((radius2 * radius2 + d * d - radius1 * radius1) / (2 * radius2 * d));
            }
            else
            {
                u1 = 0.0f;
                u2 = 0.0f;
            }

            var centerXMin = circle2.CenterX() - circle1.CenterX();
            var centerYMin = circle2.CenterY() - circle1.CenterY();

            var bottomSpreadDiff = bottomStartSpreadFactor - bottomEndSpreadFactor;
            var bottomSpreadFactor = bottomStartSpreadFactor - bottomSpreadDiff * riseRatio;

            float fPI = (float)Math.PI;
            var angle1 = Math.Atan2(centerYMin, centerXMin);
            var angle2 = Math.Acos((radius1 - radius2) / d);
            var angle1a = angle1 + u1 + (angle2 - u1) * bottomSpreadFactor;
            var angle1b = angle1 - u1 - (angle2 - u1) * bottomSpreadFactor;
            var angle2a = (angle1 + fPI - u2 - (fPI - u2 - angle2) * topSpreadFactor);
            var angle2b = (angle1 - fPI + u2 + (fPI - u2 - angle2) * topSpreadFactor);

            var p1a = AppendToTuple(GetVector((float)angle1a, radius1), circle1.CenterX(), circle1.CenterY());
            var p1b = AppendToTuple(GetVector((float)angle1b, radius1), circle1.CenterX(), circle1.CenterY());
            var p2a = AppendToTuple(GetVector((float)angle2a, radius2), circle2.CenterX(), circle2.CenterY());
            var p2b = AppendToTuple(GetVector((float)angle2b, radius2), circle2.CenterX(), circle2.CenterY());

            var totalRadius = (radius1 + radius2);
            var d2Base = Math.Min(
                    Math.Max(topSpreadFactor, bottomSpreadFactor) * handleRate,
                    getVectorLength(p1a[0], p1a[1], p2a[0], p2a[1]) / totalRadius);

            // case circles are overlapping:
            var d2 = d2Base * Math.Min(1.0f, d * 2 / (radius1 + radius2));

            var r1 = radius1 * d2;
            var r2 = radius2 * d2;

            var pi2 = fPI / 2;
            var sp1 = AppendToTuple(GetVector((float)(angle1a - pi2), r1));
            var sp2 = AppendToTuple(GetVector((float)(angle2a + pi2), r2));
            var sp3 = AppendToTuple(GetVector((float)(angle2b - pi2), r2));
            var sp4 = AppendToTuple(GetVector((float)(angle1b + pi2), r1));

            // move bottom point to bar top border
            var yOffset = (Math.Abs(topBorder - p1a[1]) * riseRatio) - 1;
            var fp1a = new List<float>() { p1a[0], p1a[1] - yOffset }; //.let { l->listOf(l[0], l[1] - yOffset) };
            var fp1b = new List<float>() { p1b[0], p1b[1] - yOffset }; //p1b.let { l->listOf(l[0], l[1] - yOffset) };

            //using(path) {
                path.Reset();
                path.MoveTo(fp1a[0], fp1a[1] + cornerRadius);
                path.LineTo(fp1a[0], fp1a[1]);
                path.CubicTo(fp1a[0] + sp1[0], fp1a[1] + sp1[1], p2a[0] + sp2[0], p2a[1] + sp2[1], p2a[0], p2a[1]);
                path.LineTo(circle2.CenterX(), circle2.CenterY());
                path.LineTo(p2b[0], p2b[1]);
                path.CubicTo(p2b[0] + sp3[0], p2b[1] + sp3[1], fp1b[0] + sp4[0], fp1b[1] + sp4[1], fp1b[0], fp1b[1]);
                path.LineTo(fp1b[0], fp1b[1] + cornerRadius);
                path.Close();
//            }

            canvas.DrawPath(path, paint);
            canvas.DrawOval(circle2, paint);
        }

        private void DrawText(Canvas canvas, Paint paint,
                              string text, Paint.Align align, Color color, float offset,
                              RectF holderRect, Rect textRect)
        {
            paint.Color = color;
            paint.TextAlign = align;
            paint.GetTextBounds(text, 0, text.Length, textRect);

            float x = 0.0f;
            if (align == Paint.Align.Left)
                x = offset;
            else if (align == Paint.Align.Center)
                x = holderRect.CenterX();
            else if (align == Paint.Align.Right)
                x = holderRect.Right - offset;
            
            var y = holderRect.CenterY() + textRect.Height() / 2f - textRect.Bottom;
            canvas.DrawText(text.ToCharArray(), 0, text.Length, x, y, paint);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            switch (e.ActionMasked)
            {
                case MotionEventActions.Down:
                    var x = e.GetX();
                    var y = e.GetY();
                    if(rectBar.Contains(x,y))
                    {
                        if(!rectTouch.Contains(x,y))
                        {
                            position = (float)Math.Max(0f, Math.Min(1f, (x - rectTouch.Width() / 2) / maxMovement));
                        }
                        touchX = x;
                        OnBeginTracking?.Invoke();
                        ShowLabel(metaballRiseDistance);
                        this.Parent.RequestDisallowInterceptTouchEvent(true);
                        return true;
                    }
                    else
                        return false;
                case MotionEventActions.Move:
                    var newPos = Math.Max(0f, Math.Min(1f, position + (e.GetX() - touchX) / maxMovement));
                    touchX = e.GetX();
                    position = (float)newPos;
                    return true;

                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                    touchX = 0f;
                    OnEndTracking?.Invoke();
                    HideLabel();
                    PerformClick();
                    this.Parent.RequestDisallowInterceptTouchEvent(false);
                    return true;
                    
                default:
                    return false;
            };
        }

        private void ShowLabel(float distance)
        {
            var top = barVerticalOffset - distance;
            var labelVOffset = (topCircleDiameter - labelRectDiameter) / 2f;

            var animation = ValueAnimator.OfFloat(rectTopCircle.Top, top);
            animation.Update += (sender, e) =>
            {
                float value = (float)e.Animation.AnimatedValue;
                rectTopCircle.OffsetTo(rectTopCircle.Left, value);
                rectLabel.OffsetTo(rectLabel.Left, value + labelVOffset);
                Invalidate();
            };
            animation.SetDuration(duration);
            animation.SetInterpolator(new OvershootInterpolator());
            animation.Start();
        }

        private void HideLabel()
        {
            var labelVOffset = (topCircleDiameter - labelRectDiameter) / 2f;
            var animation = ValueAnimator.OfFloat(rectTopCircle.Top, barVerticalOffset);
            animation.Update += (sender, e) =>
            {
                float value = (float)e.Animation.AnimatedValue;
                rectTopCircle.OffsetTo(rectTopCircle.Left, value);
                rectLabel.OffsetTo(rectLabel.Left, value + labelVOffset);
                Invalidate();
            };

            animation.SetDuration(duration);
            animation.Start();
        }
    }

    class CustomOutlineProvider : ViewOutlineProvider
    {
        private RectF _rectBar;
        private float _barCornerRadius;
        public CustomOutlineProvider(RectF rectBar, float barCornerRadius)
        {
            _rectBar = rectBar;
            _barCornerRadius = barCornerRadius;
        }

        public override void GetOutline(View view, Outline outline)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                var rect = new Rect((int)_rectBar.Left, (int)_rectBar.Top, (int)_rectBar.Right, (int)_rectBar.Bottom);
                outline?.SetRoundRect(rect, _barCornerRadius);
            }
        }
    }

    class State : View.BaseSavedState
    {
        public float Position;
        public string StartText;
        public string EndText;
        public float TextSize;
        public int ColorLabel;
        public int ColorBar;
        public int ColorBarText;
        public int ColorLabelText;
        public long Duaration;

        public State(IParcelable superState,
                     float position,
                    string startText,
                    string endText,
                    float textSize,
                    int colorLabel,
                    int colorBar,
                    int colorBarText,
                    int colorLabelText,
                     long duration) : base(superState)
        {
            Position = position;
            StartText = startText;
            EndText = endText;
            TextSize = textSize;
            ColorBar = colorBar;
            ColorLabel = colorLabel;
            ColorBarText = colorBarText;
            ColorLabelText = colorLabelText;
            Duaration = duration;
        }

        public override int DescribeContents() => 0;

        public override void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            dest.WriteFloat(Position);
            dest.WriteString(StartText);
            dest.WriteString(EndText);
            dest.WriteFloat(TextSize);
            dest.WriteInt(ColorLabel);
            dest.WriteInt(ColorBar);
            dest.WriteInt(ColorBarText);
            dest.WriteInt(ColorLabelText);
            dest.WriteLong(Duaration);
        }
    }
}
