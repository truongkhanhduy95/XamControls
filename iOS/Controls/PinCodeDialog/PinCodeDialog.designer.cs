// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace XamControls.iOS.Controls
{
    [Register ("PinCodeDialog")]
    partial class PinCodeDialog
    {
        [Outlet]
        UIKit.UIButton btnCancel { get; set; }


        [Outlet]
        UIKit.UIButton btnVerify { get; set; }


        [Outlet]
        UIKit.UIView dialogContent { get; set; }


        [Outlet]
        UIKit.UILabel lblError { get; set; }


        [Outlet]
        UIKit.UILabel lblTitle { get; set; }


        [Outlet]
        UIKit.UIView overlayView { get; set; }


        [Outlet]
        XamControls.iOS.Controls.PinCodeView pinCodeView { get; set; }


        [Outlet]
        UIKit.UILabel txtContent { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnCancel != null) {
                btnCancel.Dispose ();
                btnCancel = null;
            }

            if (btnVerify != null) {
                btnVerify.Dispose ();
                btnVerify = null;
            }

            if (dialogContent != null) {
                dialogContent.Dispose ();
                dialogContent = null;
            }

            if (lblError != null) {
                lblError.Dispose ();
                lblError = null;
            }

            if (lblTitle != null) {
                lblTitle.Dispose ();
                lblTitle = null;
            }

            if (overlayView != null) {
                overlayView.Dispose ();
                overlayView = null;
            }

            if (pinCodeView != null) {
                pinCodeView.Dispose ();
                pinCodeView = null;
            }

            if (txtContent != null) {
                txtContent.Dispose ();
                txtContent = null;
            }
        }
    }
}