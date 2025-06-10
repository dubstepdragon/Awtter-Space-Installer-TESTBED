using AwtterSDK.Editor.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace AwtterSDK.Editor.Installations
{
    public class VRCFuryInstallation : ICheckInstallStatus
    {
        bool _isInstalled;
        Version _version;

        public bool IsInstalled => _isInstalled;

        public Version InstalledVersion => _version;

        public void Check()
        {
            _isInstalled = Utils.CheckVPMPackage("com.vrcfury.vrcfury", out var infoMinimal);

            if (_isInstalled)
            {
                Version.TryParse(infoMinimal.version, out _version);
            }
        }
    }
}
