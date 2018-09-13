﻿using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Util;
using Android.Content.Res;
using Android.Graphics.Drawables;
using static Android.Widget.TextView;
using Android.Graphics;

namespace XamControls.Droid.Controls
{
    public class TagView : RelativeLayout
    {
        public List<Tag> mTags = new List<Tag>();

        private Context mContext { get; set; }
        private LayoutInflater mInflater { get; set; }
        private ViewTreeObserver mViewTreeObserber { get; set; }

        /**
         * listeners
         */
        private IOnTagClickListener mClickListener { get; set; }
        private IOnTagDeleteListener mDeleteListener { get; set; }
        private IOnTagLongClickListener mTagLongClickListener { get; set; }

        /**
         * view size param
         */
        private int mWidth { get; set; }

        /**
         * layout initialize flag
         */
        private Boolean mInitialized = false;

        /**
         * custom layout param
         */
        private int lineMargin { get; set; }
        private int tagMargin { get; set; }
        private int textPaddingLeft { get; set; }
        private int textPaddingRight { get; set; }
        private int textPaddingTop { get; set; }
        private int textPaddingBottom { get; set; }

        public TagView(Context ctx) : base(ctx)
        {
            Initialize(ctx, null, 0);
        }

        public TagView(Context ctx, IAttributeSet attrs) : base(ctx, attrs)
        {
            Initialize(ctx, attrs, 0);
        }

        public TagView(Context ctx, IAttributeSet attrs, int defStyle) : base(ctx, attrs, defStyle)
        {
            Initialize(ctx, attrs, defStyle);
        }

        /**
         * onSizeChanged
         *
         * @param w
         * @param h
         * @param oldw
         * @param oldh
         */
        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
            mWidth = w;
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            int width = MeasuredWidth;
            if (width <= 0)
                return;
            mWidth = MeasuredWidth;
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            DrawTags();
        }

        private void Initialize(Context ctx, IAttributeSet attrs, int defStyle)
        {
            mInflater = (LayoutInflater)ctx.GetSystemService(Context.LayoutInflaterService);

            mViewTreeObserber = ViewTreeObserver;

            mViewTreeObserber.GlobalLayout += delegate
            {
                if (!mInitialized)
                {
                    mInitialized = true;
                    DrawTags();
                }
            };

            this.lineMargin = (int) Utils.DipToPx(this.Context, Constants.DEFAULT_LINE_MARGIN);
            this.tagMargin = (int)Utils.DipToPx(this.Context, Constants.DEFAULT_TAG_MARGIN);
            this.textPaddingLeft = (int)Utils.DipToPx(this.Context, Constants.DEFAULT_TAG_TEXT_PADDING_LEFT);
            this.textPaddingRight = (int)Utils.DipToPx(this.Context, Constants.DEFAULT_TAG_TEXT_PADDING_RIGHT);
            this.textPaddingTop = (int)Utils.DipToPx(this.Context, Constants.DEFAULT_TAG_TEXT_PADDING_TOP);
            this.textPaddingBottom = (int)Utils.DipToPx(this.Context, Constants.DEFAULT_TAG_TEXT_PADDING_BOTTOM);
        }

        private Drawable GetSelector(Tag tag)
        {
            if (tag.Background != null)
                return tag.Background;

            StateListDrawable states = new StateListDrawable();
            GradientDrawable gdNormal = new GradientDrawable();
            gdNormal.SetColor(tag.LayoutColor);
            gdNormal.SetCornerRadius(tag.Radius);
            if (tag.LayoutBorderSize > 0)
            {
                gdNormal.SetStroke(Utils.DipToPx(Application.Context, tag.LayoutBorderSize), ColorStateList.ValueOf(new Color(tag.LayoutBorderColor)));
            }
            GradientDrawable gdPress = new GradientDrawable();
            gdPress.SetColor(tag.LayoutColorPress);
            gdPress.SetCornerRadius(tag.Radius);
            states.AddState(new int[] { Android.Resource.Attribute.StatePressed }, gdPress);
            //must add state_pressed first，or state_pressed will not take effect
            states.AddState(new int[] { }, gdNormal);
            return states;
        }

        private void DrawTags()
        {

            if (!mInitialized)
            {
                return;
            }

            // clear all tag
            RemoveAllViews();

            // layout padding left & layout padding right
            float total = PaddingLeft + PaddingRight;

            int listIndex = 1;// List Index
            int indexBottom = 1;// The Tag to add below
            int indexHeader = 1;// The header tag of this line
            Tag tagPre = null;
            foreach (Tag item in mTags)
            {
                int position = listIndex - 1;
                Tag tag = item;

                LinearLayout tagLayout = new LinearLayout(mContext);
                tagLayout.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                tagLayout.SetGravity(GravityFlags.CenterVertical);
                tagLayout.Orientation = Android.Widget.Orientation.Horizontal;
                tagLayout.Id = listIndex;

                TextView tagView = new TextView(mContext);
                tagView.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                tagView.Gravity = GravityFlags.Center;
                tagView.Ellipsize = Android.Text.TextUtils.TruncateAt.End;
                tagView.SetSingleLine(true);


                TextView deletableView = new TextView(mContext);
                deletableView.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                deletableView.Gravity = GravityFlags.Center;
                deletableView.SetSingleLine(true);
                deletableView.Visibility = ViewStates.Gone;

                tagLayout.AddView(tagView);
                tagLayout.AddView(deletableView);

                if (Build.VERSION.SdkInt < BuildVersionCodes.JellyBean)
                {
                    tagLayout.Background = GetSelector(tag); //.SetBackgroundDrawable(getSelector(tag));
                }
                else
                {
                    tagLayout.Background = GetSelector(tag); //.SetBackground(getSelector(tag));
                }

                // tag text
                tagView.SetText(tag.Text, BufferType.Normal);
                LinearLayout.LayoutParams parametros = (LinearLayout.LayoutParams)tagView.LayoutParameters;

                parametros.SetMargins(textPaddingLeft, textPaddingTop, textPaddingRight, textPaddingBottom);
                tagView.LayoutParameters = parametros;
                tagView.SetTextColor(ColorStateList.ValueOf(new Color(tag.TagTextColor)));
                tagView.SetTextSize(ComplexUnitType.Sp, tag.TagTextSize);

                tagLayout.Click += delegate
                {
                    if (mClickListener != null)
                        mClickListener.OnTagClick(tag, position);
                };

                tagLayout.LongClick += delegate
                {
                    if (mTagLongClickListener != null)
                        mTagLongClickListener.OnTagLongClick(tag, position);
                };


                // calculate　of tag layout width
                float tagWidth = tagView.Paint.MeasureText(tag.Text) + textPaddingLeft + textPaddingRight;
                // tagView padding (left & right)

                // deletable text=
                if (tag.IsDeletable)
                {
                    deletableView.Visibility = ViewStates.Visible;
                    deletableView.SetText(tag.DeleteIcon, BufferType.Normal);
                    int offset = Utils.DipToPx(Application.Context, 2f);
                    deletableView.SetPadding(offset, textPaddingTop, textPaddingRight + offset, textPaddingBottom);
                    deletableView.SetTextColor(ColorStateList.ValueOf(new Color(tag.DeleteIndicatorColor)));
                    deletableView.SetTextSize(ComplexUnitType.Sp, tag.DeleteIndicatorSize);
                    deletableView.Click += delegate
                    {
                        if (mDeleteListener != null)
                            mDeleteListener.OnTagDeleted(this, tag, position);
                    };

                    tagWidth += deletableView.Paint.MeasureText(tag.DeleteIcon) + textPaddingLeft + textPaddingRight;
                    // deletableView Padding (left & right)
                }
                else
                {
                    deletableView.Visibility = ViewStates.Gone;
                }

                LayoutParams tagParams = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

                //add margin of each line
                tagParams.BottomMargin = lineMargin;

                if (mWidth <= total + tagWidth + Utils.DipToPx(Application.Context, Constants.LAYOUT_WIDTH_OFFSET))
                {
                    //need to add in new line
                    if (tagPre != null) tagParams.AddRule(LayoutRules.Below, indexBottom);
                    // initialize total param (layout padding left & layout padding right)
                    total = PaddingLeft + PaddingRight;
                    indexBottom = listIndex;
                    indexHeader = listIndex;
                }
                else
                {
                    //no need to new line
                    tagParams.AddRule(LayoutRules.AlignTop, indexHeader);
                    //not header of the line
                    if (listIndex != indexHeader)
                    {
                        tagParams.AddRule(LayoutRules.RightOf, listIndex - 1);
                        tagParams.LeftMargin = tagMargin;
                        total += tagMargin;
                        if (tagPre.TagTextSize < tag.TagTextSize)
                        {
                            indexBottom = listIndex;
                        }
                    }


                }
                total += tagWidth;
                AddView(tagLayout, tagParams);
                tagPre = tag;
                listIndex++;

            }

        }

        //public methods
        //----------------- separator  -----------------//

        /**
         * @param tag
         */
        public void AddTag(Tag tag)
        {
            mTags.Add(tag);
            DrawTags();
        }

        public void AddTags(List<Tag> tags)
        {
            if (tags == null) return;
            mTags = new List<Tag>();
            if (tags.Count() == 0)
                DrawTags();
            foreach (Tag item in tags)
            {
                AddTag(item);
            }
        }

        public void AddTags(String[] tags)
        {
            if (tags == null) return;
            foreach (String item in tags)
            {
                Tag tag = new Tag(item);
                AddTag(tag);
            }
        }


        /**
         * get tag list
         *
         * @return mTags TagObject List
         */
        public List<Tag> GetTags()
        {
            return mTags;
        }

        /**
         * remove tag
         *
         * @param position
         */
        public void Remove(int position)
        {
            if (position < mTags.Count())
            {
                mTags.RemoveAt(position);
                DrawTags();
            }
        }

        /**
         * remove all views
         */
        public void RemoveAll()
        {
            mTags.Clear(); //clear all of tags
            RemoveAllViews();
        }

        public int GetLineMargin()
        {
            return lineMargin;
        }

        public void SetLineMargin(float lineMargin)
        {
            this.lineMargin = Utils.DipToPx(Application.Context, lineMargin);
        }

        public int GetTagMargin()
        {
            return tagMargin;
        }

        public void SetTagMargin(float tagMargin)
        {
            this.tagMargin = Utils.DipToPx(Application.Context, tagMargin);
        }

        public int GetTextPaddingLeft()
        {
            return textPaddingLeft;
        }

        public void SetTextPaddingLeft(float textPaddingLeft)
        {
            this.textPaddingLeft = Utils.DipToPx(Application.Context, textPaddingLeft);
        }

        public int GetTextPaddingRight()
        {
            return textPaddingRight;
        }

        public void SetTextPaddingRight(float textPaddingRight)
        {
            this.textPaddingRight = Utils.DipToPx(Application.Context, textPaddingRight);
        }

        public int GetTextPaddingTop()
        {
            return textPaddingTop;
        }

        public void SetTextPaddingTop(float textPaddingTop)
        {
            this.textPaddingTop = Utils.DipToPx(Application.Context, textPaddingTop);
        }

        public int GettextPaddingBottom()
        {
            return textPaddingBottom;
        }

        public void SettextPaddingBottom(float textPaddingBottom)
        {
            this.textPaddingBottom = Utils.DipToPx(Application.Context, textPaddingBottom);
        }

        /**
         * setter for OnTagLongClickListener
         *
         * @param longClickListener
         */
        public void SetOnTagLongClickListener(IOnTagLongClickListener longClickListener)
        {
            mTagLongClickListener = longClickListener;
        }

        /**
         * setter for OnTagSelectListener
         *
         * @param clickListener
         */
        public void SetOnTagClickListener(IOnTagClickListener clickListener)
        {
            mClickListener = clickListener;
        }

        /**
         * setter for OnTagDeleteListener
         *
         * @param deleteListener
         */
        public void SetOnTagDeleteListener(IOnTagDeleteListener deleteListener)
        {
            mDeleteListener = deleteListener;
        }

        /**
         * Listeners
         */
        public interface IOnTagDeleteListener
        {
            void OnTagDeleted(TagView view, Tag tag, int position);
        }

        public interface IOnTagClickListener
        {
            void OnTagClick(Tag tag, int position);
        }

        public interface IOnTagLongClickListener
        {
            void OnTagLongClick(Tag tag, int position);
        }

    }
}
