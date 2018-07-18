using System;
using System.Collections.Generic;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;

namespace XamControls.Droid.Controls
{
    public class PaperOnboardingFragment : Fragment
    {
        private static string ELEMENTS_PARAM = "elements";
        private List<PaperOnboardingPage> elements;
        public List<PaperOnboardingPage> Elements
        {
            get { return elements; }
            set 
            {
                elements = value;
            }
        }

        private PaperOnboardingOnChangeListener onChangeListener;
        private PaperOnboardingOnRightOutListener onRightOutListener;
        private PaperOnboardingOnLeftOutListener onLeftOutListener;


        public static PaperOnboardingFragment newInstance(List<PaperOnboardingPage> elements)
        {
            PaperOnboardingFragment fragment = new PaperOnboardingFragment();
            Bundle args = new Bundle();
            args.PutSerializable(ELEMENTS_PARAM, new Java.Util.ArrayList(elements));
            fragment.Arguments = args;
            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if(Arguments != null)
            {
                //CHECK HERE
                elements = new List<PaperOnboardingPage>(((Java.Util.ArrayList)Arguments.Get(ELEMENTS_PARAM)).ToArray<PaperOnboardingPage>());
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.onboarding_main_layout, container, false);

            // create engine for onboarding element
            //PaperOnboardingEngine mPaperOnboardingEngine = new PaperOnboardingEngine(view.FindViewById(Resource.Id.onboardingRootView), elements, Activity.ApplicationContext;
            //// set listeners
                                                                                     //mPaperOnboardingEngine.setOnChangeListener(onChangeListener);
                                                                                     //mPaperOnboardingEngine.setOnLeftOutListener(onRightOutListener);
                                                                                     //mPaperOnboardingEngine.setOnRightOutListener(onLeftOutListener);

            return view;
        }

        public void SetOnChangeListener(PaperOnboardingOnChangeListener onChangeListener)
        {
            this.onChangeListener = onChangeListener;
        }

        public void SetOnRightOutListener(PaperOnboardingOnRightOutListener onRightOutListener)
        {
            this.onRightOutListener = onRightOutListener;
        }

        public void SetOnLeftOutListener(PaperOnboardingOnLeftOutListener onLeftOutListener)
        {
            this.onLeftOutListener = onLeftOutListener;
        }
    }
}
