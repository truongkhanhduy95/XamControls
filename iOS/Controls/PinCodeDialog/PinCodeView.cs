using System;
using System.Collections.Generic;
using System.Linq;
using Cirrious.FluentLayouts.Touch;
using Foundation;
using UIKit;

namespace XamControls.iOS.Controls
{
    [Register("PinCodeView")]
    public class PinCodeView : UITextField, IUITextFieldDelegate
    {
        private const float TEXT_MARGIN = 8f;
        private int _currentPosition;
        private List<UITextField> _textFieldList;

        #region Properties

        private int _pinLength;
        public int PinLength
        {
            get { return _pinLength; }
            set
            {
                _pinLength = value;
                SetNeedsLayout();
            }
        }

        private string _hint;
        public string Hint
        {
            get { return _hint; }
            set{
                _hint = value;
                SetNeedsLayout();
            }
        }

        private string _value;
        public string Value
        {
            get { return _value; }
            set{
                _value = value;
            }
        }

        #endregion

        public PinCodeView (IntPtr handle) : base (handle)
        {
            Initialize();
        }

        private void Initialize()
        {
			KeyboardType = UIKeyboardType.NumbersAndPunctuation;
			AutocorrectionType = UITextAutocorrectionType.No;
			ReturnKeyType = UIReturnKeyType.Done;
            this.Delegate = this;

            BackgroundColor = UIColor.Clear;
            _textFieldList = new List<UITextField>();
            _pinLength = 4;
            this.BecomeFirstResponder();
        }

        public override void Draw(CoreGraphics.CGRect rect)
        {
            base.Draw(rect);
            SetupTextFields();
		}

        private void SetupTextFields()
        {
            _textFieldList.Clear();
            UITextField textField;
            for (int i = 0; i < _pinLength; i++)
            {
                textField = InitTextFieldWithTag(tag: i);
                _textFieldList.Add(textField);
                DecorTextFields(textField);
            }
        }

        private UITextField InitTextFieldWithTag(int tag)
        {
            var textField = new UITextField()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                UserInteractionEnabled = false,
                BackgroundColor = UIColor.White,
                TextAlignment = UITextAlignment.Center,
            };

            textField.Layer.BorderColor = UIColor.LightGray.CGColor;
            textField.Layer.BorderWidth = 1f;

            textField.Placeholder = _hint;
            textField.Tag = tag;

            return textField;
        }

		[Export("textFieldShouldReturn:")]
		public bool ShouldReturn(UITextField textField)
		{
			ResignFirstResponder();
			return true;
		}

        private void DecorTextFields(UITextField textField)
        {
            var tag = textField.Tag;

            var previousView = _textFieldList.FirstOrDefault((x) => x.Tag == tag - 1);
            this.AddSubview(textField);

            if (tag == 0)
            {
                this.AddConstraints(
                    textField.AtTopOf(this),
                    textField.AtLeftOf(this),
                    textField.AtBottomOf(this),
                    textField.Width().EqualTo((this.Bounds.Width - (_pinLength  - 1) * TEXT_MARGIN) / _pinLength)
                );
            }
            else
            {
				this.AddConstraints(
					textField.AtTopOf(this),
					textField.ToRightOf(previousView, TEXT_MARGIN),
					textField.AtBottomOf(this),
					textField.Width().EqualTo((this.Bounds.Width - (_pinLength - 1) * TEXT_MARGIN) / _pinLength)
				);
            }
        }

        public void Reset()
        {
            _value = string.Empty;
            _currentPosition = 0;
            _textFieldList.ForEach((s) => s.Text = string.Empty);
        }


        private void SetPINText(string text)
        {
            var pin = _textFieldList.FirstOrDefault((x) => x.Tag == _currentPosition);
            pin.Text = text;
            _value = string.Empty;
            _textFieldList.ForEach((s) => _value += s.Text);
        }

        [Export("textField:shouldChangeCharactersInRange:replacementString:")]
        public bool ShouldChangeCharacters(UITextField textField, NSRange range, string replacementString)
        {
			int number = 0;
			if (replacementString.Length == 0 || int.TryParse(replacementString, out number))
			{
				if (_currentPosition < _pinLength)
				{
					SetPINText(replacementString);
					_currentPosition++;
				}
			}
            return false;
        }

        public override void DeleteBackward()
        {
            if (_currentPosition > 0)
            {
                _currentPosition--;
                SetPINText(string.Empty);
            }
            base.DeleteBackward();
        }

        public override bool CanPerform(ObjCRuntime.Selector action, NSObject withSender)
        {
            
            _textFieldList.FirstOrDefault((x) => x.Tag == _currentPosition).Text = string.Empty;
            return false;
        }

        public override CoreGraphics.CGRect GetCaretRectForPosition(UITextPosition position)
        {
            return CoreGraphics.CGRect.Empty;
        }
      
    }
}
