# XamControls
Xamarin ported controls

# FluidSlider:
<img src="https://github.com/Ramotion/fluid-slider/blob/master/fluid-slider.gif" width="600" height="450" />
<br><br/>

# Paper Onboaring: 
<img src="https://github.com/Ramotion/paper-onboarding/blob/master/paper-onboarding.gif" width="600" height="450" />
<br><br/>
All credit goes to: https://github.com/Ramotion/

# Tag View: 
<img src="https://github.com/truongkhanhduy95/XamControls/blob/master/img/ios_tag_view.png" width="338" height="600" />
<br><br/>

## iOS

### Usage

1) Create new `TagListView`

``` c#
var tagsView = new TagListView(true)
  {
    PaddingY = 4f,
    TextFont = UIFont.SystemFontOfSize(20f)
  };
```

2) Setup control properties

``` c#
//Samples
tagsView.Alignment = TagsAlignment.Left;
tagsView.CornerRadius = 17f;
tagsView.PaddingX = 5f;
tagsView.PaddingX = 8f;
tagsView.ControlsDistance = 4f;
tagsView.TagBackgroundColor = UIColor.FromRGB(52, 152, 219);
```

3) Add new tag

``` c#
private void BtnAdd_TouchUpInside(object sender, EventArgs e)
{
  if(!string.IsNullOrEmpty(this.input.Text))
  {
    tagsView.AddTag(this.input.Text);
    this.input.Text = string.Empty;
  }
}
```

4) Subscribe event listeners

``` c#
tagsView.TagButtonTapped += (sender, e) =>
{
   tagsView.RemoveTag(e);
};

tagsView.TagSelected += (sender, e) =>
{
   //Do something...                
};
```

## Android

### Usage

1) Create TagView in XML layout file

``` xml
  <XamControls.Droid.Controls.TagView
      android:id="@+id/tagView"
      android:layout_width="match_parent"
      android:layout_height="wrap_content"
      android:layout_marginEnd="16dp"
      android:layout_marginStart="16dp"/>
```

``` c#
var tagView = FindViewById<TagView>(Resource.Id.tagView);
```
2) Add tags
``` c#
var tag = new Controls.Tag("This is tag name");
tag.IsDeletable = true; //Show "x" button
tagView.AddTag(tag);
```
3) Setup TagView Listener
``` c#
tagView.SetOnTagClickListener(new MyOnTagClickListener());
tagView.SetOnTagDeleteListener(new MyOnTagDeleteListener());
tagView.SetOnTagLongClickListener(new MySetOnTagLongClickListenery());
```
# Notification Handler:

## iOS

### Usage

1) Configure Firebase in AppDelegate

``` c#
public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
{
    //...
    Firebase.Core.App.Configure();
            
    return true;
} ```
