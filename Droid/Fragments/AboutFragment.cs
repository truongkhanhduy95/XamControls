using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using XamControls.Droid.Controls;

namespace XamControls.Droid
{
    public class AboutFragment : Android.Support.V4.App.Fragment, IFragmentVisible
    {
        public static AboutFragment NewInstance() =>
            new AboutFragment { Arguments = new Bundle() };

        public AboutViewModel ViewModel { get; set; }

        private TickerView tickerView;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        Button learnMoreButton;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_about, container, false);
            ViewModel = new AboutViewModel();
            learnMoreButton = view.FindViewById<Button>(Resource.Id.button_learn_more);
            tickerView = view.FindViewById<TickerView>(Resource.Id.tickerView);
            return view;
        }

        public override void OnStart()
        {
            base.OnStart();
            learnMoreButton.Click += LearnMoreButton_Click;

            tickerView.setCharacterLists(TickerUtils.ProvideNumberList().ToCharArray());
            tickerView.setText("9555");
        }

        public override void OnStop()
        {
            base.OnStop();
            learnMoreButton.Click -= LearnMoreButton_Click;
        }

        public void BecameVisible()
        {

        }

        void LearnMoreButton_Click(object sender, System.EventArgs e)
        {
            ViewModel.OpenWebCommand.Execute(null);
        }
    }
}
