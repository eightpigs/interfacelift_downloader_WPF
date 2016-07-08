using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace interfacelift_downloader
{
    class HttpUtils
    {
        private static readonly string DefaultUserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
        /// <summary>
        /// 发送GET请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static HttpWebResponse SendGet(string url)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.UserAgent = DefaultUserAgent;

            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// 发送POST请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="parameters">随同请求POST的参数名称及参数值字典</param>
        /// <param name="requestEncoding">发送HTTP请求时所用的编码</param>
        /// <returns></returns>
        public static HttpWebResponse SendPost(string url, Dictionary<string, string> parameters, Encoding requestEncoding)
        {
            HttpWebRequest request = null;

            //如果是发送HTTPS请求，证书验证总是返回true
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }

            request.Method = "POST";
            request.UserAgent = DefaultUserAgent;
            request.ContentType = "application/x-www-form-urlencoded";

            //写post数据到请求体
            StringBuilder buffer = new StringBuilder();
            int i = 0;
            foreach (string key in parameters.Keys)
            {
                if (i > 0)
                {
                    buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                }
                else
                {
                    buffer.AppendFormat("{0}={1}", key, parameters[key]);
                }
                i++;
            }
            byte[] data = requestEncoding.GetBytes(buffer.ToString());
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// 获取Response中的文本内容
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static string getContent(HttpWebResponse response)
        {
            string data = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                data = readStream.ReadToEnd();
                
                response.Close();
                readStream.Close();
            }
            return data;
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="url">文本Url</param>
        /// <param name="filePath">保存路径</param>
        /// <returns>1 : 下载新文件完成    2:该文件已存在,跳过</returns>
        public static int downFile(string url, string filePath)
        {
            try
            {
                Console.WriteLine(url);
                WebClient client = new WebClient();
                client.Headers.Add("user-agent", DefaultUserAgent);
                string fileName = url.Substring(url.IndexOf("_") + 1);
                fileName = fileName.Substring(0, fileName.IndexOf("_")) + url.Substring(url.LastIndexOf("."));

                FileInfo file = new FileInfo(filePath + fileName);
                if (!file.Exists)
                {
                    client.DownloadFile(url, filePath + fileName);
                    return 1;
                }
                else
                    return 2;
            }catch
            {
                return 0;
            }
        }
    }
}
