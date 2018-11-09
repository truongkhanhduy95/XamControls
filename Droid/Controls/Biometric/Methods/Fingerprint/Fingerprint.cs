using System;
using Android;
using Android.Content;
using Android.Support.V4.Content;
using Android.Support.V4.Hardware.Fingerprint;

namespace XamControls.Droid.Controls.Biometric
{
    public interface IBiometricMethod
    {
        bool CheckHardwareSupport();
    }

    public class Fingerprint : IBiometricMethod
    {
        private Context _context;
        private FingerprintManagerCompat fingerprintManager;

        public Fingerprint(Context context)
        {
            _context = context;
            fingerprintManager = FingerprintManagerCompat.From(_context);
        }

        public bool CheckHardwareSupport()
        {
            return fingerprintManager.IsHardwareDetected;
        }

        public void CheckPermission()
        {
            // The context is typically a reference to the current activity.
            Android.Content.PM.Permission permissionResult = ContextCompat.CheckSelfPermission(_context, Manifest.Permission.UseFingerprint);
            if (permissionResult == Android.Content.PM.Permission.Granted)
            {
                // Permission granted - go ahead and start the fingerprint scanner.
            }
            else
            {
                // No permission. Go and ask for permissions and don't start the scanner. See
                // http://developer.android.com/training/permissions/requesting.html
            }
        }

        public void Scanning()
        {
            const int flags = 0; /* always zero (0) */

            // CryptoObjectHelper is described in the previous section.
            CryptoObjectHelper cryptoHelper = new CryptoObjectHelper();

            // cancellationSignal can be used to manually stop the fingerprint scanner. 
            var cancellationSignal = new Android.Support.V4.OS.CancellationSignal();

            FingerprintManagerCompat fingerPrintManager = FingerprintManagerCompat.From(_context);

            // AuthenticationCallback is a base class that will be covered later on in this guide.
            FingerprintManagerCompat.AuthenticationCallback authenticationCallback = new MyAuthCallbackSample();

            // Start the fingerprint scanner.
            fingerprintManager.Authenticate(cryptoHelper.BuildCryptoObject(), flags, cancellationSignal, authenticationCallback, null);
        }

    }
}
