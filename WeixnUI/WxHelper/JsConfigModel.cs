using System;

namespace SrpAPI.WxInterface.Models
{
    public class JsConfigModel
    {
        public string appId { get; set; }
        public int timestamp { get; set; }
        public string nonceStr { get; set; }
        public string signature { get; set; }
    }
}
