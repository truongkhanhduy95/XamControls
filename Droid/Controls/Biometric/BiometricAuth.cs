using System;
using System.Threading.Tasks;

namespace XamControls.Droid.Controls.Biometric
{
    public interface IBiometricAuth
    {
        void StopAuthenicate();

        Task<AuthenResult> AuthenticateAsync();

        Task<bool> CheckAvailableAsync();
    }

    public class BiometricAuth : IBiometricAuth
    {
        private readonly IBiometricMethod _biometricMethod;
        private TaskCompletionSource<AuthenResult> _taskCompletionSource;

        public BiometricAuth(IBiometricMethod biometricMethod)
        {
            _biometricMethod = biometricMethod;
            _taskCompletionSource = new TaskCompletionSource<AuthenResult>();
        }

        public Task<bool> CheckAvailableAsync()
        {
            return _biometricMethod.CheckHardwareSupport();
        }

        public async Task<AuthenResult> AuthenticateAsync()
        {
            if (await _biometricMethod.CheckHardwareSupport())
            {
                _biometricMethod.StartScanning();
                _biometricMethod.OnScanned = SetResultSafe;

                //Run in background
                return await _taskCompletionSource.Task;
            }
            else
                throw new NotSupportedException("Device doesn't support this method");
        }

        public void StopAuthenicate()
        {
            _biometricMethod.StopScanning();
        }

        private void SetResultSafe(AuthenResult result)
        {
            if (!(_taskCompletionSource.Task.IsCanceled || _taskCompletionSource.Task.IsCompleted || _taskCompletionSource.Task.IsFaulted))
            {
                _taskCompletionSource.SetResult(result);
            }
        }
    }
}
