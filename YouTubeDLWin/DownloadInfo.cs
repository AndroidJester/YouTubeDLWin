using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeDLWin
{
    public class DownloadInfo
    {
        public string Output { get; set; }  = "";
        public float DownloadProgress { get; set; }  = 0f;
        public string DownloadSpeed { get; set; } = "";
        public bool IsComplete { get; set; } = false;
    }
}
