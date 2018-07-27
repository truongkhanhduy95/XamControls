using Android.Graphics;
using Java.IO;

namespace XamControls.Droid.Controls
{
	public class PaperOnboardingPage : Java.Lang.Object, ISerializable
	{
		#region PROPS

		private string titleText;

		public string TitleText
		{
			get { return titleText; }
			set
			{
				titleText = value;
			}
		}

		private string descriptionText;

		public string DescriptionText
		{
			get { return descriptionText; }
			set
			{
				descriptionText = value;
			}
		}

		private Color titleColor;

		public Color TitleColor
		{
			get { return titleColor; }
			set
			{
				titleColor = value;
			}
		}

		private Color descriptionColor;

		public Color DescriptionColor
		{
			get { return descriptionColor; }
			set
			{
				DescriptionColor = value;
			}
		}

		private Color bgColor;

		public Color BackgroundColor
		{
			get { return bgColor; }
			set
			{
				bgColor = value;
			}
		}

		private int contentIconRes;

		public int ContentIconRes
		{
			get { return contentIconRes; }
			set
			{
				contentIconRes = value;
			}
		}

		private int bottomBarIconRes;

		public int BottomBarIconRes
		{
			get { return bottomBarIconRes; }
			set
			{
				bottomBarIconRes = value;
			}
		}

		#endregion PROPS

		public PaperOnboardingPage()
		{
		}

		public PaperOnboardingPage(string titleText, string descriptionText, Color titleColor, Color descriptionColor, Color bgColor, int contentIconRes, int bottomBarIconRes)
		{
			this.bgColor = bgColor;
			this.contentIconRes = contentIconRes;
			this.bottomBarIconRes = bottomBarIconRes;
			this.descriptionText = descriptionText;
			this.titleText = titleText;
			this.descriptionColor = descriptionColor;
			this.titleColor = titleColor;
		}

		public override bool Equals(Java.Lang.Object obj)
		{
			if (this == obj) return true;
			if (obj == null) return false;

			PaperOnboardingPage that = (PaperOnboardingPage)obj;

			if (bgColor != that.bgColor) return false;
			if (titleColor != that.titleColor) return false;
			if (descriptionColor != that.descriptionColor) return false;
			if (contentIconRes != that.contentIconRes) return false;
			if (bottomBarIconRes != that.bottomBarIconRes) return false;
			if (titleText != null ? !titleText.Equals(that.titleText) : that.titleText != null)
				return false;
			return descriptionText != null ? descriptionText.Equals(that.descriptionText) : that.descriptionText == null;
		}

		public override int GetHashCode()
		{
			int result = titleText != null ? titleText.GetHashCode() : 0;
			result = 31 * result + (descriptionText != null ? descriptionText.GetHashCode() : 0);
			result = 31 * result + titleColor;
			result = 31 * result + descriptionColor;
			result = 31 * result + bgColor;
			result = 31 * result + contentIconRes;
			result = 31 * result + bottomBarIconRes;
			return result;
		}

		public override string ToString()
		{
			return "PaperOnboardingPage{" +
				"titleText='" + titleText + '\'' +
				", descriptionText='" + descriptionText + '\'' +
				", titleColor=" + titleColor +
				", descriptionColor=" + descriptionColor +
				", bgColor=" + bgColor +
				", contentIconRes=" + contentIconRes +
				", bottomBarIconRes=" + bottomBarIconRes +
				'}';
		}
	}
}