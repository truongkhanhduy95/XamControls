using System;
namespace XamControls.Droid.Controls.Biometric
{
    public interface IBiometricAuth
    {

    }

    public class BiometricAuth : IBiometricAuth
    {
        private readonly IBiometricMethod _biometricMethod;

        public BiometricAuth(IBiometricMethod biometricMethod)
        {
            _biometricMethod = biometricMethod;
        }
    }
}
