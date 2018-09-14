using System;
namespace XamControls.Modules
{
    public interface IConnectionManager
    {
        bool IsWifiConnected();

        bool Is3GConnected();

        void NavigateToConnectionSettings();
    }
}
