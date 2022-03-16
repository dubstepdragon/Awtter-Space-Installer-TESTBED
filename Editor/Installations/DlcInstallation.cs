using AwtterSDK.Editor.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwtterSDK.Editor.Installations
{
    internal class DlcInstallation : ICheckInstallStatus
    {
        bool _isInstalled;
        string _installedVersion;
        string _typeInstalled;
        string _path;
        public bool IsInstalled => _isInstalled;
        public string InstalledVersion => _installedVersion;
        public string TypeInstalled => _typeInstalled;

        public DlcInstallation(string path)
        {
            _path = path;
        }

        public void Check()
        {
            _isInstalled = Directory.Exists(_path);
            if (_isInstalled)
            {
                foreach (var file in new DirectoryInfo(_path).GetFiles("*.fbx"))
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
