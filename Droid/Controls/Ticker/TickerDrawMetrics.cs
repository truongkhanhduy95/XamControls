using System;
using System.Collections.Generic;
using Android.Graphics;

namespace XamControls.Droid.Controls
{
    public class TickerDrawMetrics
    {
        private readonly Paint textPaint;
        private Dictionary<char, float> charWidths = new Dictionary<char, float>(256);

        private float charHeight;
        public float CharHeight => charHeight;

        private float charBaseline;
        public float CharBaseline => charBaseline;

        public TickerDrawMetrics(Paint paint)
        {
            textPaint = paint;
            invalidate();
        }

        public float GetCharWidth(char character)
        {
            if (character == TickerUtils.EMPTY_CHAR) return 0;

            // This method will lazily init char
            if (charWidths.TryGetValue(character, out float value))
            {
                return value;
            }
            else
            {
                var width = textPaint.MeasureText(character.ToString());
                charWidths.Add(character, width);
                return width;
            }
        }

        public void invalidate()
        {
            charWidths.Clear();
            Paint.FontMetrics fm = textPaint.GetFontMetrics();
            charHeight = fm.Bottom - fm.Top;
            charBaseline = -fm.Top;
        }

        public float GetCharBaseLine()
        {
            return charBaseline;
        }
        public float GetCharHeight()
        {
            return charHeight;
        }
    }
}
