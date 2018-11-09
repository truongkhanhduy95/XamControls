using System;
using System.Threading.Tasks;
using Android;
using Android.Content;
using Android.Support.V4.Content;
using Android.Support.V4.Hardware.Fingerprint;
using Android.Support.V4.OS;

namespace XamControls.Droid.Controls.Biometric
{
    public class FingerprintMethod : IBiometricMethod
    {
        private Context _context;
        private FingerprintManagerCompat fingerprintManager;
        private CancellationSignal cancellationSignal;

        public Action<AuthenResult> OnScanned { get; set; }

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

        public void StartScanning()
        {
            const int flags = 0; /* always zero (0) */

            CryptoObjectHelper cryptoHelper = new CryptoObjectHelper();

            // cancellationSignal can be used to manually stop the fingerprint scanner. 
            // On: cancellationSignal.Cancel();
            cancellationSignal = new CancellationSignal();

            // NOTE: For more security, like online transactions,etc...
            // Consider using Asymmetric keys
            // Send public key to server to identity user
            // When bio-auth succeed, send private to server to decrypted
            // https://android-developers.googleblog.com/2015/10/new-in-android-samples-authenticating.html

            // Start the fingerprint scanner.
            fingerprintManager.Authenticate(cryptoHelper.BuildCryptoObject(), flags, cancellationSignal, RegisterAuthCallback(), null);
            System.Diagnostics.Debug.WriteLine("Start scanning...");
        }

        public void StopScanning()
        {
            if(cancellationSignal != null)
            {
                cancellationSignal.Cancel();
                cancellationSignal = null;
                System.Diagnostics.Debug.WriteLine("Stop scanning...");
            }
        }


        private FingerprintManagerCompat.AuthenticationCallback RegisterAuthCallback()
        {
            var callback = new FingerprintAuthCallback();
            callback.OnAuthenticatedSuccess += () => 
            {
                OnScanned?.Invoke(GetResult(AuthenticationStatus.Succeeded, string.Empty));
            };
            callback.OnAuthenicatedFailed += () => 
            {
                OnScanned?.Invoke(GetResult(AuthenticationStatus.Failure, string.Empty));

            };
            callback.OnAuthenticatedHelp += (helpString) => 
            {
                OnScanned?.Invoke(GetResult(AuthenticationStatus.Help, helpString));

            };
            callback.OnAuthenticatedError += (errorString) => 
            {
                OnScanned?.Invoke(GetResult(AuthenticationStatus.Error, errorString));

            };
            return callback;
        }

        private AuthenResult GetResult(AuthenticationStatus status, string message)
        {
            return new AuthenResult()
            {
                Status = status,
                ErrorMessage = message
            };
        }
    }
}
