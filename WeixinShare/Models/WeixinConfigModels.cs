
namespace WeixinShare.Models
{
    public class WeixinSignatureConfig
    {
        public string appId { get; set; }
        public string nonceStr { get; set; }
        public string timestamp { get; set; }
        public string signature { get; set; }
    }
}