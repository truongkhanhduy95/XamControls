using System;
using Android.Content;
using Android.Util;
using Android.Views;

namespace XamControls.Droid.Controls
{
    public class OnSwipeListener : Java.Lang.Object, View.IOnTouchListener
    {
        private GestureDetector gestureDetector;

        public Action OnSwipeRight;
        public Action OnSwipeLeft;
        public Action OnSwipeTop;
        public Action OnSwipeBottom;

        public OnSwipeListener(Context context)
        {
            gestureDetector = new GestureDetector(context, new GestureListener(this));
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            return gestureDetector.OnTouchEvent(e);
        }

       

        public class GestureListener : GestureDetector.SimpleOnGestureListener
        {

            private static int SWIPE_THRESHOLD = 100;
            private static int SWIPE_VELOCITY_THRESHOLD = 100;

            public override bool OnDown(MotionEvent e)
            {
                return true;
            }

            private OnSwipeListener _parent;
            internal GestureListener(OnSwipeListener parent) { _parent = parent; }

            public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
            {
                bool result = false;
                try
                {
                    float diffY = e2.GetY() - e1.GetY();
                    float diffX = e2.GetX() - e1.GetX();
                    if (Math.Abs(diffX) > Math.Abs(diffY))
                    {
                        if (Math.Abs(diffX) > SWIPE_THRESHOLD && Math.Abs(velocityX) > SWIPE_VELOCITY_THRESHOLD)
                        {
                            if (diffX > 0)
                            {
                                _parent.OnSwipeRight?.Invoke();
                            }
                            else
                            {
                                _parent.OnSwipeLeft?.Invoke();
                            }
                        }
                        result = true;
                    }
                    else if (Math.Abs(diffY) > SWIPE_THRESHOLD && Math.Abs(velocityY) > SWIPE_VELOCITY_THRESHOLD)
                    {
                        if (diffY > 0)
                        {
                            _parent.OnSwipeBottom?.Invoke();
                        }
                        else
                        {
                            _parent.OnSwipeTop?.Invoke();
                        }
                    }
                    result = true;

                }
                catch (Exception exception)
                {
                    Log.Error("OnSwipe", exception.Message);
                }
                return result;
            }
        }
    }
}
