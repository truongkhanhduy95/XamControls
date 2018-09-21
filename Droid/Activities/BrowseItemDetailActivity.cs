using Android.App;
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
        private Controls.TagView tagView;

        ItemDetailViewModel viewModel;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var data = Intent.GetStringExtra("data");

            var item = Newtonsoft.Json.JsonConvert.DeserializeObject<Item>(data);
            viewModel = new ItemDetailViewModel(item);

            FindViewById<TextView>(Resource.Id.description).Text = item.Description;

            SupportActionBar.Title = item.Text;

            SetUpSlider();
            SetUpTagView();
        }

        private void SetUpSlider()
        {
            slider = FindViewById<Controls.FluidSlider>(Resource.Id.fluidSlider);

            slider.From = 100;
            slider.To = 400;
            slider.SelectedValue = 250;
            slider.ColorBar = this.BaseContext.Resources.GetColor(Resource.Color.primary);
            //slider.ColorBubble = this.BaseContext.Resources.GetColor(Resource.Color.primary);

            slider.OnPositionChanged += (float value) =>
            {
                System.Diagnostics.Debug.WriteLine("Selected: " + value);
            };
        }

        private void SetUpTagView()
        {
            tagView = FindViewById<Controls.TagView>(Resource.Id.tagView);
            for (int i = 0; i < 5; i++)
            {
                var tag = new Controls.Tag("Item " + i);
                tag.IsDeletable = true;
                tagView.AddTag(tag);
            }
        }
    }
}
