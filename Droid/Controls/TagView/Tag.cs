using System;
using Android.Graphics.Drawables;

namespace XamControls.Droid.Controls
{
    public class Tag
    {
        public int Id;
        public String Text;
        public int TagTextColor;
        public float TagTextSize;
        public int LayoutColor;
        public int LayoutColorPress;
        public Boolean IsDeletable;
        public int DeleteIndicatorColor;
        public float DeleteIndicatorSize;
        public float Radius;
        public String DeleteIcon;
        public float LayoutBorderSize;
        public int LayoutBorderColor;
        public Drawable Background;

        public Tag(String text)
        {
            Initialize(0, text, Constants.DEFAULT_TAG_TEXT_COLOR, Constants.DEFAULT_TAG_TEXT_SIZE, Constants.DEFAULT_TAG_LAYOUT_COLOR, Constants.DEFAULT_TAG_LAYOUT_COLOR_PRESS,
                    Constants.DEFAULT_TAG_IS_DELETABLE, Constants.DEFAULT_TAG_DELETE_INDICATOR_COLOR, Constants.DEFAULT_TAG_DELETE_INDICATOR_SIZE, Constants.DEFAULT_TAG_RADIUS, Constants.DEFAULT_TAG_DELETE_ICON, Constants.DEFAULT_TAG_LAYOUT_BORDER_SIZE, Constants.DEFAULT_TAG_LAYOUT_BORDER_COLOR);
        }

        private void Initialize(int id, String text, int tagTextColor, float tagTextSize,
                          int layoutColor, int layoutColorPress, Boolean isDeletable,
                          int deleteIndicatorColor, float deleteIndicatorSize, float radius,
                          String deleteIcon, float layoutBorderSize, int layoutBorderColor)
        {
            this.Id = id;
            this.Text = text;
            this.TagTextColor = tagTextColor;
            this.TagTextSize = tagTextSize;
            this.LayoutColor = layoutColor;
            this.LayoutColorPress = layoutColorPress;
            this.IsDeletable = isDeletable;
            this.DeleteIndicatorColor = deleteIndicatorColor;
            this.DeleteIndicatorSize = deleteIndicatorSize;
            this.Radius = radius;
            this.DeleteIcon = deleteIcon;
            this.LayoutBorderSize = layoutBorderSize;
            this.LayoutBorderColor = layoutBorderColor;
        }
    }
}
