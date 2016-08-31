using System;
using SrpAPI.Common;
using SrpAPI.Caching;
using Newtonsoft.Json;
using SrpAPI.WxInterface.Models;

namespace SrpAPI.WxInterface
{
    public class TokenMannger
    {
        public static TokenMannger Instance { get { return new TokenMannger(); } }

        /// <summary>
        /// 获取并刷新Token
        /// </summary>
        public string GetToken(string AppName, string AppId, string AppSecret)
        {
            string CACHE_KEY = "_GlobalToken-" + AppName;
            var devInfo = CacheManager.Instance.Get<dynamic>(CACHE_KEY);
            if (devInfo == null)
            {
                string result = HttpHelper.Instance.GetSend("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + AppId + "&secret=" + AppSecret);
                devInfo = JsonConvert.DeserializeObject<dynamic>(result);
                if (devInfo.access_token == null) return null;
                CacheManager.Instance.Insert<dynamic>(CACHE_KEY, devInfo, DateTime.Now.AddMinutes(117));
            }
            return devInfo.access_token.ToString();
        }
    }
}
