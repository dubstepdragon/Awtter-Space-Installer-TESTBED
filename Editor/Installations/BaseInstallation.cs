using AwtterSDK.Editor.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwtterSDK.Editor.Installations
{
    internal class BaseInstallation : ICheckInstallStatus
    {
        bool _isInstalled;
        string _installedVersion;
        string _typeInstalled;
        public bool IsInstalled => _isInstalled;
        public string InstalledVersion => _installedVersion;
        public string TypeInstalled => _typeInstalled;

        public void Check()
        {
            _isInstalled = Directory.Exists("Assets/1) Model (fbx)");
            if (_isInstalled)
            {
                foreach (var file in new DirectoryInfo("Assets/1) Model (fbx)").GetFiles("*.fbx"))
                {
                    var sp = file.Name.Split(' ');
                    _typeInstalled = file.Name.ToLower().Contains("awtter") ? "awtter" : "awdeer";
                    _installedVersion = sp.FirstOrDefault(p => p.Contains("."));
                    break;
                }
            }
            else
                _installedVersion = null;
        }
    }
}
