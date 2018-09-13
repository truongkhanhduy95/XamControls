using System;
using Android.Util;

namespace XamControls.Droid.Controls
{
    public static class Utils
    {
        public static int DipToPx(Android.Content.Context c, float dipValue)
        {
            DisplayMetrics metrics = c.Resources.DisplayMetrics;
            return (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, dipValue, metrics);
        }
    }
}
