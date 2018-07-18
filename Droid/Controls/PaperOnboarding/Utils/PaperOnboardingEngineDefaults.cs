using System;
namespace XamControls.Droid.Controls
{
    public abstract class PaperOnboardingEngineDefaults
    {
        protected string TAG = "POB";

        // animation and view settings
        protected int ANIM_PAGER_BAR_MOVE_TIME = 700;

        protected int ANIM_PAGER_ICON_TIME = 350;

        protected int ANIM_BACKGROUND_TIME = 450;

        protected int CONTENT_TEXT_POS_DELTA_Y_DP = 50;
        protected int ANIM_CONTENT_TEXT_SHOW_TIME = 800;
        protected int ANIM_CONTENT_TEXT_HIDE_TIME = 200;

        protected int CONTENT_ICON_POS_DELTA_Y_DP = 50;
        protected int ANIM_CONTENT_ICON_SHOW_TIME = 800;
        protected int ANIM_CONTENT_ICON_HIDE_TIME = 200;

        protected float PAGER_ICON_SHAPE_ALPHA = 0.5f;

        protected int ANIM_CONTENT_CENTERING_TIME = 800;
    }
}
