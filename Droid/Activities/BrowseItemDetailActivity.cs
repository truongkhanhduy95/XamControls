﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace XamControls.Droid
{
    [Activity(Label = "Details", ParentActivity = typeof(MainActivity))]
    [MetaData("android.support.PARENT_ACTIVITY", Value = ".MainActivity")]
    public class BrowseItemDetailActivity : BaseActivity
    {
        /// <summary>
        /// Specify the layout to inflace
        /// </summary>
        protected override int LayoutResource => Resource.Layout.activity_item_details;
        private Controls.FluidSlider slider;

        ItemDetailViewModel viewModel;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var data = Intent.GetStringExtra("data");

            var item = Newtonsoft.Json.JsonConvert.DeserializeObject<Item>(data);
            viewModel = new ItemDetailViewModel(item);

            FindViewById<TextView>(Resource.Id.description).Text = item.Description;

            SupportActionBar.Title = item.Text;

            slider = FindViewById<Controls.FluidSlider>(Resource.Id.fluidSlider);

            slider.From = 0;
            slider.To = 500;
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnStop()
        {
            base.OnStop();
        }
    }
}
