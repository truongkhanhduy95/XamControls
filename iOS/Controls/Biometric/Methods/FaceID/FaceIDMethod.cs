using System;
using System.Threading.Tasks;

namespace XamControls.iOS.Controls.Biometric
{
    public class FaceIDMethod : IBiometricMethod
    {
        public Action<AuthenResult> OnScanned { get; set; }

     
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
    }
}
