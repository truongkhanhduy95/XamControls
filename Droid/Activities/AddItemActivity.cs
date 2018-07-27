using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Widget;
using System.Collections.Generic;
using XamControls.Droid.Controls;

namespace XamControls.Droid
{
	[Activity(Label = "AddItemActivity")]
	public class AddItemActivity : Activity
	{
		private FloatingActionButton saveButton;
		private EditText title, description;

		public ItemsViewModel ViewModel { get; set; }

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			ViewModel = BrowseFragment.ViewModel;

			// Create your application here
			SetContentView(Resource.Layout.onboarding_main_layout);

			PaperOnboardingPage scr1 = new PaperOnboardingPage("Hotels",
										"All hotels and hostels are sorted by hospitality rating", Color.White, Color.White,
															   Color.ParseColor("#678FB4"), Resource.Drawable.hotels, Resource.Drawable.key);
			PaperOnboardingPage scr2 = new PaperOnboardingPage("Banks",
				"We carefully verify all banks before add them into the app", Color.White, Color.White,
															   Color.ParseColor("#65B0B4"), Resource.Drawable.banks, Resource.Drawable.wallet);
			PaperOnboardingPage scr3 = new PaperOnboardingPage("Stores",
				"All local stores are categorized for your convenience", Color.White, Color.White,
															   Color.ParseColor("#9B90BC"), Resource.Drawable.stores, Resource.Drawable.shopping_cart);

			List<PaperOnboardingPage> elements = new List<PaperOnboardingPage>();
			elements.Add(scr1);
			elements.Add(scr2);
			elements.Add(scr3);

			PaperOnboardingEngine engine = new PaperOnboardingEngine(FindViewById(Resource.Id.onboardingRootView), elements, ApplicationContext);
		}
	}
}