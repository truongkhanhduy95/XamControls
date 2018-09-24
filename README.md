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
