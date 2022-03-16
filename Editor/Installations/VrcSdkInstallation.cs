using AwtterSDK.Editor.Interfaces;
using AwtterSDK.Editor.Models;
using System;
using System.IO;

namespace AwtterSDK.Editor.Installations
{
    internal class VrcSdkInstallation : ICheckInstallStatus
    {
        bool _isInstalled;
        string _installedVersion;
        public bool IsInstalled => _isInstalled;
        public string InstalledVersion => _installedVersion;
        public string TypeInstalled => null;

        public void Check()
        {
            _isInstalled = Directory.Exists("Assets/VRCSDK");
            if (_isInstalled)
            {
                if (File.Exists("Assets/VRCSDK/version.txt"))
                    _installedVersion = File.ReadAllText("Assets/VRCSDK/version.txt");
            }
            else
                _installedVersion = null;
        }
    }
}
