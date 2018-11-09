using System;
using System.Threading.Tasks;
using Foundation;
using LocalAuthentication;
using ObjCRuntime;
using UIKit;

namespace XamControls.iOS.Controls.Biometric
{
    public class TouchIDMethod : IBiometricMethod
    {
        private LAContext _context;

        public Action<AuthenResult> OnScanned { get; set; }

        public TouchIDMethod()
        {
            CreateLaContext();
        }

        public Task<bool> CheckHardwareSupport()
        {
            throw new NotImplementedException();
        }

        public void StartScanning()
        {
        }

        public void StopScanning()
        {
        }

        private void CreateNewContext()
        {
            if (_context != null)
            {
                if (_context.RespondsToSelector(new Selector("invalidate")))
                {
                    _context.Invalidate();
                }
                _context.Dispose();
            }

            CreateLaContext();
        }

        private void CreateLaContext()
        {
            var info = new NSProcessInfo();
            if (!UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
                return;
            // Check LAContext is not available on iOS7 and below, so check LAContext after checking iOS version.
            if (Class.GetHandle(typeof(LAContext)) == IntPtr.Zero)
                return;

            _context = new LAContext();
        }
    }
}
