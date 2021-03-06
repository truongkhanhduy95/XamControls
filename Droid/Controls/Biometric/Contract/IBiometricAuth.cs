﻿using System;
using System.Threading.Tasks;

namespace XamControls.Droid.Controls.Biometric
{
    public interface IBiometricAuth
    {
        void StopAuthenicate();

        Task<AuthenResult> AuthenticateAsync();

        Task<bool> CheckAvailableAsync();
    }
}
