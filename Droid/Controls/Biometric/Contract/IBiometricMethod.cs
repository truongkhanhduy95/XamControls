using System;
using System.Threading.Tasks;

namespace XamControls.Droid.Controls.Biometric
{
    public interface IBiometricMethod
    {
        Task<bool> CheckHardwareSupport();

        void StartScanning();

        void StopScanning();

        Action<AuthenResult> OnScanned { get; set; }
    }


    public class AuthenResult
    {
        public bool IsSuccess => Status == AuthenticationStatus.Succeeded;

        public string ErrorMessage { get; set; }

        public AuthenticationStatus Status { get; set; }

    }

    public enum AuthenticationStatus
    {
        Succeeded,
        Help,
        Error,
        Failure
    }
}
