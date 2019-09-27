using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using WeixinShare;

namespace Tencent
{
    public class WeixinConfig
    {
        private static string AppID = ConfigurationManager.AppSettings["WeixinAppID"];
        private static string AppSecret = ConfigurationManager.AppSettings["WeixinAppSecret"];

        /// <summary>
        /// 提交网络请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        private static string SubmitHttpWebRequest(string url, string para = "")
        {
            string retString = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Timeout = 10000;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string encoding = response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8"; //默认编码  
                }
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding)))
                {
                    retString = reader.ReadToEnd();

                    if (para != "")
                    {
                        JObject jsonObj = (JObject)JsonConvert.DeserializeObject(retString);
                        if (jsonObj[para] != null)
                        {
                            retString = jsonObj[para].ToString();
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                retString = null;
            }
            return retString;
        }

        /// <summary>
        /// 生成随机字符串
        /// </summary>
        /// <param name="codeLen"></param>
        /// <returns></returns>
        public static string CreateRandCode(int codeLen = 16)
        {
            string codeSerial = "2,3,4,5,6,7,a,c,d,e,f,h,i,j,k,m,n,p,r,s,t,A,C,D,E,F,G,H,J,K,M,N,P,Q,R,S,U,V,W,X,Y,Z";
            if (codeLen == 0)
            {
                codeLen = 16;
            }
            string[] arr = codeSerial.Split(',');
            string code = "";
            int randValue = -1;
            Random rand = new Random(unchecked((int)DateTime.Now.Ticks));
            for (int i = 0; i < codeLen; i++)
            {
                randValue = rand.Next(0, arr.Length - 1);
                code += arr[randValue];
            }
            return code;
        }

        /// <summary>
        /// SHA1加密
        /// </summary>
        /// <param name="string1"></param>
        /// <returns></returns>
        private static string GetSHA1(string string1)
        {
            SHA1 sha;
            ASCIIEncoding enc;
            string hash = "";
            sha = new SHA1CryptoServiceProvider();
            enc = new ASCIIEncoding();
            byte[] dataToHash = enc.GetBytes(string1);
            byte[] dataHashed = sha.ComputeHash(dataToHash);
            hash = BitConverter.ToString(dataHashed).Replace("-", "");
            hash = hash.ToLower();
            return hash;
        }

        /// <summary>
        /// 获取access_token
        /// </summary>
        /// <returns></returns>
        public static string GetAccess_token()
        {
            string access_token = string.Empty;
            string cacheName = "Weixin_access_token";
            object obj = CacheHelper.GetCache(cacheName);
            if (obj != null)
            {
                access_token = obj.ToString();
            }
            else
            {
                string url = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + AppID + "&secret=" + AppSecret;
                access_token = SubmitHttpWebRequest(url, "access_token");
                //设置缓存
                //7200秒内有效，不可无限次调取微信接口
                CacheHelper.SetCache(cacheName, access_token, 7200);
            }
            return access_token;
        }

        /// <summary>
        /// 获取jsapi_ticket
        /// </summary>
        /// <returns></returns>
        public static string GetJsapi_Ticket()
        {
            string jsapi_ticket = string.Empty;
            string cacheName = "Weixin_jsapi_ticket";
            object obj = CacheHelper.GetCache(cacheName);
            if (obj != null)
            {
                jsapi_ticket = obj.ToString();
            }
            else
            {
                string access_token = GetAccess_token();
                string url = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?type=jsapi&access_token=" + access_token;
                jsapi_ticket = SubmitHttpWebRequest(url, "ticket");
                //设置缓存
                //7200秒内有效，不可无限次调取微信接口
                CacheHelper.SetCache(cacheName, jsapi_ticket, 7200);
            }
            return jsapi_ticket;
        }

        /// <summary>
        /// 获取签名信息
        /// </summary>
        /// <returns></returns>
        public static string[] GetSignature(string url)
        {
            //JSAPI调用凭证
            string jsapi_ticket = GetJsapi_Ticket();

            //随机生成的字符串
            string noncestr = CreateRandCode();

            //当前时间戳            
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string timestamp = ((Int64)ts.TotalSeconds).ToString();
            //string timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();//.net framework4.6

            //url
            if (url.IndexOf("#") > 0)
            {
                url = url.Substring(0, url.IndexOf("#"));
            }

            //签名
            StringBuilder string1 = new StringBuilder();
            string1.AppendFormat("jsapi_ticket={0}&noncestr={1}&timestamp={2}&url={3}", jsapi_ticket, noncestr, timestamp, url);
            string signature = GetSHA1(string1.ToString());

            //返回相关信息
            string[] rtn = new string[] { AppID, noncestr, timestamp, signature };
            return rtn;
        }
    }
}
