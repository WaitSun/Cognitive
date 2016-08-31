using System;
using SrpAPI.Common;
using SrpAPI.Caching;
using System.Web.Security;
using SrpAPI.WxInterface.Models;

namespace SrpAPI.WxInterface
{
    public class JsticketMannger
    {
        public static JsticketMannger Instance { get { return new JsticketMannger(); } }
        public dynamic GetSignature(string AppId, string AppName, string url, string access_token)
        {
            string ticket = GetTicket(AppName, access_token).ticket;
            var model = new JsConfigModel();
            model.appId = AppId;
            model.nonceStr = Guid.NewGuid().ToString();
            model.timestamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            string str = "jsapi_ticket=" + ticket + "&noncestr=" + model.nonceStr + "&timestamp=" + model.timestamp + "&url=" + url + "";
            model.signature = FormsAuthentication.HashPasswordForStoringInConfigFile(str, "SHA1").ToLower();
            return model;
        }

        /// <summary>
        /// 获取jsticket
        /// </summary>
        private JsapiTicket GetTicket(string AppName, string access_token)
        {
            string CACHE_KEY = "_JSAPI_TICKET-" + AppName;
            var jsticket = CacheManager.Instance.Get<JsapiTicket>(CACHE_KEY);
            if (jsticket == null)
            {
                jsticket = new JsapiTicket().GetTicket(access_token);
                CacheManager.Instance.Insert<JsapiTicket>(CACHE_KEY, jsticket, DateTime.Now.AddMinutes(115));
            }
            if (jsticket.IsExpires)
            {
                CacheManager.Instance.Remove(CACHE_KEY);
                jsticket = new JsapiTicket().GetTicket(access_token);
                CacheManager.Instance.Insert<JsapiTicket>(CACHE_KEY, jsticket, DateTime.Now.AddMinutes(115));
            }
            return jsticket;
        }
    }
}
