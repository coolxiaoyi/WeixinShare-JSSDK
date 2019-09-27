using System.Web.Http;
using WeixinShare.Models;

namespace AudioCloud.Controllers.API
{
    public class WeixinController : ApiController
    {
        /// <summary>
        /// 获取access_token
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public string GetAccess_token()
        {
            string access_token = Tencent.WeixinConfig.GetAccess_token();
            return access_token;
        }

        /// <summary>
        /// 获取jsapi_ticket
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public string GetJsapi_Ticket()
        {
            string jsapi_ticket = Tencent.WeixinConfig.GetJsapi_Ticket();
            return jsapi_ticket;
        }

        /// <summary>
        /// 获取签名信息
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public WeixinSignatureConfig GetSignature(string url)
        {
            //微信配置
            string[] weixin = Tencent.WeixinConfig.GetSignature(url);
            WeixinSignatureConfig weixinConfig = new WeixinSignatureConfig
            {
                appId = weixin[0],
                nonceStr = weixin[1],
                timestamp = weixin[2],
                signature = weixin[3]
            };
            return weixinConfig;
        }
    }
}
