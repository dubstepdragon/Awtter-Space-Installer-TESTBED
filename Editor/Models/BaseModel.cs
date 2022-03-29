using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwtterSDK.Editor.Models
{
    [Serializable]
    internal class BaseModel
    {
        [NonSerialized]
        public int ViewID;
        [NonSerialized]
        public bool Install;
        public string Name;
        public string ShortName;
        public string Version;
        public string IconLink;
        public string DownloadLink;
        public string[] Dependencies = new string[0];
        public List<DLC> AvaliableDLC = new List<DLC>();
    }
}
