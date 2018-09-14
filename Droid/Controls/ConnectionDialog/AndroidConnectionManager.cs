using System;
using XamControls.Modules;

namespace XamControls.Droid.Controls
{
    public class AndroidConnectionManager : IConnectionManager
    {
        public AndroidConnectionManager()
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
