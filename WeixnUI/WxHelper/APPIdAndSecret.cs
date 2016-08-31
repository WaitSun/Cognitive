using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace DDClassLib.Core.test
{
    /// <summary>
    /// 开发者中心ID和秘钥
    /// </summary>
    public class APPIdAndSecret
    {
        public string AppID = WebConfigurationManager.AppSettings["AppID"];// "wxd3e0ae2c80611a65";
        public string AppSecret = WebConfigurationManager.AppSettings["AppSecret"];//"db3fec5e0a5eee268ae77823e48baac3";

    }

}
