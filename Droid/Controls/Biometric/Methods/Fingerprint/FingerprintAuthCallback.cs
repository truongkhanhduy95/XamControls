using System;
using Android.Support.V4.Hardware.Fingerprint;
using Android.Util;
using Java.Lang;
using Javax.Crypto;

namespace XamControls.Droid.Controls.Biometric
{
    class FingerprintAuthCallback : FingerprintManagerCompat.AuthenticationCallback
    {
        public Action OnAuthenticatedSuccess;
        public Action OnAuthenicatedFailed;
        public Action<string> OnAuthenticatedHelp;
        public Action<string> OnAuthenticatedError;

        // Can be any byte array, keep unique to application.
        static readonly byte[] SECRET_BYTES = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        // The TAG can be any string, this one is for demonstration.
        static readonly string TAG = "MyAuthCallbackSample:";

        public FingerprintAuthCallback()
        {
        }

        public override void OnAuthenticationSucceeded(FingerprintManagerCompat.AuthenticationResult result)
        {
            if (result.CryptoObject.Cipher != null)
            {
                try
                {
                    // Calling DoFinal on the Cipher ensures that the encryption worked.
                    byte[] doFinalResult = result.CryptoObject.Cipher.DoFinal(SECRET_BYTES);

                    // No errors occurred, trust the results.        
                    OnAuthenticatedSuccess?.Invoke();
                }
                catch (BadPaddingException bpe)
                {
                    // Can't really trust the results.
                    Log.Error(TAG, "Failed to encrypt the data with the generated key." + bpe);
                    OnAuthenicatedFailed?.Invoke();
                }
                catch (IllegalBlockSizeException ibse)
                {
                    // Can't really trust the results.
                    Log.Error(TAG, "Failed to encrypt the data with the generated key." + ibse);
                    OnAuthenicatedFailed?.Invoke();
                }
            }
            else
            {
                // No cipher used, assume that everything went well and trust the results.
            }
        }

        public override void OnAuthenticationError(int errMsgId, ICharSequence errString)
        {
            // Report the error to the user. Note that if the user canceled the scan,
            // this method will be called and the errMsgId will be FingerprintState.ErrorCanceled.
            OnAuthenticatedError?.Invoke(errString.ToString());
        }

        public override void OnAuthenticationFailed()
        {
            OnAuthenicatedFailed?.Invoke();
            // Tell the user that the fingerprint was not recognized.
        }

        public override void OnAuthenticationHelp(int helpMsgId, ICharSequence helpString)
        {
            // Notify the user that the scan failed and display the provided hint.
            OnAuthenticatedHelp?.Invoke(helpString.ToString());
        }
    }
}
