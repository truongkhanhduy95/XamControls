using System;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;

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
        const double TOP_SPREAD_FACTOR = 0.4f;
        const double BOTTOM_START_SPREAD_FACTOR = 0.25f;
        const double BOTTOM_END_SPREAD_FACTOR = 0.1f;
        const double METABALL_HANDLER_FACTOR = 2.4f;
        const double METABALL_MAX_DISTANCE = 15.0f;
        const double METABALL_RISE_DISTANCE = 1.1f;

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
        private RectF rectTopCircle;
        private RectF rectBottomCircle;
        private RectF rectTouch;
        private RectF rectLabel;
        private Rect rectText;
        private Path pathMetaball;

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

        private string bubleText;
        private string startText = TEXT_START;
        private string endText = TEXT_END;

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
            return new State(base.OnSaveInstanceState(), position, startText, endText, TextSize,
                             ColorBubble, ColorBar, colorBarText, colorBubbleText, duration);
        }

        protected override void OnRestoreInstanceState(IParcelable state)
        {
            base.OnRestoreInstanceState(state);
            if (state is State)
            {
                position = ((State)state).Position;
                startText = ((State)state).StartText;
                endText = ((State)state).EndText;
                TextSize = ((State)state).TextSize;
                //ColorBubble = ((State)state).ColorLabel;
                //ColorBar = ((State)state).ColorBar;
                //colorBarText = ((State)state).ColorBarText;
                //colorBubbleText = ((State)state).ColorLabelText;
                duration = ((State)state).Duaration;
            }
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
