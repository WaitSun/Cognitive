using System;
using SrpAPI.Common;
using Newtonsoft.Json;

namespace SrpAPI.WxInterface.Models
{
    internal class JsapiTicket
    {
        public string ticket;
        public int expires_in;
        private DateTime createdate;
        public JsapiTicket()
        {
            createdate = DateTime.Now;
        }

        public bool IsExpires
        {
            get
            {
                return (DateTime.Now - createdate).TotalSeconds >= this.expires_in;
            }
        }

        public JsapiTicket GetTicket(string access_token)
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi", access_token);
            return JsonConvert.DeserializeObject<JsapiTicket>(HttpHelper.Instance.GetSend(url));
        }
    }
}
