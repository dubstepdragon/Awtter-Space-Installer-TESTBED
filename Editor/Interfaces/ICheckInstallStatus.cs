using AwtterSDK.Editor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwtterSDK.Editor.Interfaces
{
    internal interface ICheckInstallStatus
    {
        bool IsInstalled { get; }
        string InstalledVersion { get; }
        string TypeInstalled { get; }
        void Check();
    }
}
