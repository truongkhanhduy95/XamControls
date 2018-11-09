using System;
using System.Threading.Tasks;

namespace XamControls.Droid.Controls.Biometric
{
    public class AuthenResult
    {
        public byte[] SignedBytes { get; private set; }

        public bool IsSuccess { get; private set; }

        public string Message { get; private set; }

        public AuthenResult(bool isSuccess, string message, byte[] signedBytes)
        {
            SignedBytes = signedBytes;
            IsSuccess = isSuccess;
            Message = message;
        }
    }

    public interface IBiometricMethod
    {
        Task<bool> CheckHardwareSupport();

        AuthenResult StartAuthenticate();
    }
}
