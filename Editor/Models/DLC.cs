using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwtterSDK.Editor.Models
{
    internal class DLC
    {
        public int ViewID { get; set; }
        public bool Install { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string IconLink { get; set; }
        public string DownloadLink { get; set; }
        public string[] Dependencies { get; set; } = new string[0];
    }
}
