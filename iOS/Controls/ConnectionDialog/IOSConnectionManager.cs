using System;
using XamControls.Modules;

namespace XamControls.iOS.Controls
{
    public class IOSConnectionManager : IConnectionManager
    {
        public IOSConnectionManager()
        {
        }

        public bool Is3GConnected()
        {
            return false;
        }

        public bool IsWifiConnected()
        {
            return false;
        }

        public void NavigateToConnectionSettings()
        {
        }
    }
}
