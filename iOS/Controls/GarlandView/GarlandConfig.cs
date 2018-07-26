using System;
using CoreGraphics;
using UIKit;

namespace XamControls.iOS.Controls
{
    public class GarlandConfig
    {
        private static GarlandConfig _instance;
        public static GarlandConfig Instance
        {
            get {
                return _instance ?? (_instance = new GarlandConfig());
            }    
        }

        private GarlandConfig(){}

        //Default config
        public float ParallaxHeaderOffset = 40f;

        public float CardRadius = 5;

        public float CardCellDetailedWidth = 300;

        public UIEdgeInsets SideInsets = new UIEdgeInsets(10, 4.5f, 10, 64);

        public double AnimationDuration = 0.7f;

        public float CardSpacing = 30;

        public CGSize CardSize = new CGSize(Math.Round(UIScreen.MainScreen.Bounds.Width * 0.8f), 128f);

        public CGSize HeaderSize = new CGSize(Math.Round(UIScreen.MainScreen.Bounds.Width * 0.8f) + 8, 128f);

        public float HeaderVerticalOffset = 60;

        public UIColor CardShadowColor = UIColor.Black;

        public CGSize CardShadowOffset = new CGSize(width: 0, height: 2);

        public float CardShadowRadius = 5;

        public float CardShadowOpacity = 0.3f;
    }
}
