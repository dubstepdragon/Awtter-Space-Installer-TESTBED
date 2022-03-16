using AwtterSDK.Editor.Enums;
using System.Collections.Generic;

namespace AwtterSDK.Editor.Models
{
    internal class DownloadableContent
    {
        public List<BaseModel> Models { get; set; } = new List<BaseModel>();
        public List<Package> Packages { get; set; } = new List<Package>();
    }
}
