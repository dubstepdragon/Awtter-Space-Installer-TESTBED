using AwtterSDK.Editor.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwtterSDK.Editor.Installations
{
    internal class PoyomiInstallation : ICheckInstallStatus
    {
        bool _isInstalled;
        string _installedVersion;
        public bool IsInstalled => _isInstalled;
        public string InstalledVersion => _installedVersion;
        public string TypeInstalled => null;

        public void Check()
        {
            _isInstalled = Directory.Exists("Assets/_PoiyomiShaders");
            if (_isInstalled)
            {
                if (File.Exists("Assets/_PoiyomiShaders/Shaders/Pro/Shaders/S_Poiyomi_Toon.shader"))
                {
                    var content = File.ReadAllLines("Assets/_PoiyomiShaders/Shaders/Pro/Shaders/S_Poiyomi_Toon.shader");
                    var versionLine = content.FirstOrDefault(p => p.Contains("shader_master_label"));
                    if (versionLine != null)
                    {
                        var line = versionLine.Split('(', ')')[1];
                        var sLine = line.Split(' ').FirstOrDefault(p => p.Contains("V"));
                        _installedVersion = sLine.Substring(1, sLine.IndexOf('<') - 1);
                    }
                }
            }
            else
                _installedVersion = null;
        }
    }
}
