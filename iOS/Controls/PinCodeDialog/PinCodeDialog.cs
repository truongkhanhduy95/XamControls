using System;
using Cirrious.FluentLayouts.Touch;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace XamControls.iOS.Controls
{
	public partial class PinCodeDialog : UIView
	{
		public Action<string> OnVerifyClicked;
        public Action OnCancel;

        private UIViewController _viewController;
		private UIView _parenView;

		private string _hintFormat = "-";
		public string HintFormat
		{
			get { return _hintFormat; }
			set
			{
				_hintFormat = value;
                pinCodeView.Hint = _hintFormat;
			}
		}

		private string _title;
		public string Title
		{
			set
			{
				_title = value;
				lblTitle.Text = _title;
			}
		}

		private string _content;
		public string Content
		{
			set
			{
				_content = value;
				txtContent.Text = _content;
			}
		}

        private string _error;
        public string Error
        {
            set
            {
                _error = value;
				lblError.Text = _error;
				lblError.Hidden = false;
				pinCodeView.Reset();
            }
        }

		public PinCodeDialog(UIViewController vc) : base()
		{
            _viewController = vc;
			_parenView = vc.View;
			Initialize();
		}

		public PinCodeDialog(IntPtr handle) : base(handle)
		{
			Initialize();
		}

		private void Initialize()
		{
			LoadNib();
			DecorateView();
			InitEvents();
		}

		private void LoadNib()
		{
			var arr = NSBundle.MainBundle.LoadNib("PinCodeDialog", this, null);
			var v = Runtime.GetNSObject(arr.ValueAt(0)) as UIView;
			v.TranslatesAutoresizingMaskIntoConstraints = false;
			AddSubview(v);
			this.AddConstraints(
				v.AtTopOf(this),
				v.AtLeftOf(this),
				v.AtRightOf(this),
				v.AtBottomOf(this));
		}



		private void DecorateView()
		{
			this.TranslatesAutoresizingMaskIntoConstraints = false;

			//Title = AppResources.PinDialogTitle;
   //         Content = AppResources.PinDialogContent;
			//btnVerify.SetTitle(AppResources.PinDialogButtonVerify.ToUpper(), UIControlState.Normal);
			//btnCancel.SetTitle(AppResources.CancelButton.ToUpper(), UIControlState.Normal);

            HintFormat = "-";
		}

		private void InitEvents()
		{
			btnVerify.TouchUpInside += OnButtonVerifyClick;
            btnCancel.TouchUpInside += OnCancelClicked;

            var tap = new UITapGestureRecognizer(()=>OnCancel?.Invoke());

			tap.NumberOfTapsRequired = 1;
			overlayView.UserInteractionEnabled = true;
			overlayView.AddGestureRecognizer(tap);
		}

		private void OnButtonVerifyClick(object sender, EventArgs e)
		{
			OnVerifyClicked?.Invoke(pinCodeView.Value);
			//HideDialog();
		}

        private void OnCancelClicked(object sender, EventArgs e)
        {
            OnCancel?.Invoke();
        }

        public void ShowDialog()
		{
			_parenView.AddSubview(this);

			_parenView.AddConstraints(
				this.AtTopOf(_parenView),
				this.AtLeftOf(_parenView),
				this.AtRightOf(_parenView),
				this.AtBottomOf(_parenView)
			);
			_parenView.BringSubviewToFront(this);
			pinCodeView.Reset();
			pinCodeView.BecomeFirstResponder();
		}

		public void HideDialog()
		{
			this.RemoveFromSuperview();
		}

	}
}
