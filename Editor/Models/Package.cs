namespace AwtterSDK.Editor.Models
{
    public class Package
    {
        public int ViewID { get; set; }
        public bool Install { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string IconLink { get; set; }
        public string Version { get; set; }
        public bool AutoDetect { get; set; } = true;
        public string DownloadLink { get; set; }
    }
}
