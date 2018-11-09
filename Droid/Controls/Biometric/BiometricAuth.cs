using System;
using System.Threading.Tasks;

namespace XamControls.Droid.Controls.Biometric
{
    public interface IBiometricAuth
    {
        AuthenResult StartAuthenticate();

        Task<bool> CheckHardwareSupport();
    }

    public class BiometricAuth : IBiometricAuth
    {
        private readonly IBiometricMethod _biometricMethod;

        public BiometricAuth(IBiometricMethod biometricMethod)
        {
            _biometricMethod = biometricMethod;
        }

        public Task<bool> CheckHardwareSupport()
        {
            return _biometricMethod.CheckHardwareSupport();
        }

        public AuthenResult StartAuthenticate()
        {
            return _biometricMethod.StartAuthenticate();
        }
    }
}
