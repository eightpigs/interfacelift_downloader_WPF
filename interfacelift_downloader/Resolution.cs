using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace interfacelift_downloader
{
    /// <summary>
    /// 屏幕分辨率
    /// </summary>
    class Resolution
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
        /// 搜索值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 分辨率
        /// </summary>
        public string Px { get; set; }

        public Resolution() { }
        public Resolution(string Name , string Intro, string Value, string Px )
        {
            this.Name = Name;
            this.Intro = Intro;
            this.Value = Value;
            this.Px = Px;
        }
    }
}
