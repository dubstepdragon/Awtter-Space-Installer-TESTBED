using System;

namespace AwtterSDK.Editor.Models
{
    [Serializable]
    public class Package
    {
        [NonSerialized]
        public int ViewID;
        [NonSerialized]
        public bool Install;
        public string Name;
        public string ShortName;
        public string IconLink;
        public string Version;
        public bool AutoDetect = true;
        public string DownloadLink;
    }
}
