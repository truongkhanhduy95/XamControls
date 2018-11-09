using System;
using System.Threading.Tasks;
using Android;
using Android.Content;
using Android.Support.V4.Content;
using Android.Support.V4.Hardware.Fingerprint;

namespace XamControls.Droid.Controls.Biometric
{
    public class FingerprintMethod : IBiometricMethod
    {
        private Context _context;
        private FingerprintManagerCompat fingerprintManager;

        public FingerprintMethod(Context context)
        {
            _context = context;
            fingerprintManager = FingerprintManagerCompat.From(_context);
        }

        public async Task<bool> CheckHardwareSupport()
        {
            if (fingerprintManager.IsHardwareDetected)
            {
                if (await CheckPermissionIfNeeded())
                    return true;
            }
            return false;
        }

        private async Task<bool> CheckPermissionIfNeeded()
        {
            Android.Content.PM.Permission permissionResult = ContextCompat.CheckSelfPermission(_context, Manifest.Permission.UseFingerprint);
            if (permissionResult == Android.Content.PM.Permission.Granted)
            {
                // Permission granted - go ahead and start the fingerprint scanner.
                return true;
            }
            else
            {
                await Task.Delay(3000);//Simulate
                return true;
                // No permission. Go and ask for permissions and don't start the scanner. See
                // http://developer.android.com/training/permissions/requesting.html
            }

        }

        public AuthenResult StartAuthenticate()
        {
            const int flags = 0; /* always zero (0) */

            CryptoObjectHelper cryptoHelper = new CryptoObjectHelper();

            // cancellationSignal can be used to manually stop the fingerprint scanner. 
            // On: cancellationSignal.Cancel();
            var cancellationSignal = new Android.Support.V4.OS.CancellationSignal();

            // Start the fingerprint scanner.
            fingerprintManager.Authenticate(cryptoHelper.BuildCryptoObject(), flags, cancellationSignal, RegisterAuthCallback(), null);
            return null;
        }

        private FingerprintManagerCompat.AuthenticationCallback RegisterAuthCallback()
        {
            var callback = new FingerprintAuthCallback();


            return callback;
        }
    }
}
