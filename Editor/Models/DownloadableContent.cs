using AwtterSDK.Editor.Enums;
using System;
using System.Collections.Generic;

namespace AwtterSDK.Editor.Models
{
    [Serializable]
    internal class DownloadableContent
    {
        public List<BaseModel> Models = new List<BaseModel>();
        public List<Package> Packages = new List<Package>();
    }
}
