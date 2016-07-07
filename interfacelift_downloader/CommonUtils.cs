using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace interfacelift_downloader
{
    class CommonUtils
    {
        //如果是纯数字还可以采用ASCII码进行判断  
        /// <summary>     
        /// 判断是否是数字     
        /// </summary>     
        /// <param name="str">字符串</param>     
        /// <returns>bool</returns>     
        public static bool IsNumeric(string str)
        {
            if (str == null || str.Length == 0)
                return false;
            System.Text.ASCIIEncoding ascii = new System.Text.ASCIIEncoding();
            byte[] bytestr = ascii.GetBytes(str);
            foreach (byte c in bytestr)
            {
                if (c < 48 || c > 57)
                {
                    return false;
                }
            }
            return true;
        } 
    }
}
