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
        /// 值
        /// </summary>
        public string Value { get; set; }

        public Resolution() { }
        public Resolution(string Name , string Intro, string Value )
        {
            this.Name = Name;
            this.Intro = Intro;
            this.Value = Value;
        }
    }
}
