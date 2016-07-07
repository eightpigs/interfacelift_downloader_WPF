using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace interfacelift_downloader
{
    /// <summary>
    /// 屏幕尺寸
    /// </summary>
    class ScreenSize
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string Intro { get; set; }
        /// <summary>
        /// 对应的分辨率
        /// </summary>
        public List<Resolution> Resolutions { get; set; }

        public ScreenSize() { }
        public ScreenSize(string Name , string Intro , List<Resolution> Resolutions )
        {
            this.Name = Name;
            this.Intro = Intro;
            this.Resolutions = Resolutions;
        }
    }
}
