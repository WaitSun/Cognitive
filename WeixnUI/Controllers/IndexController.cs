using DDClassLib.Core.BaseController;
using DDClassLib.Core.test;
using ProjecToxfordApi.Controllers;
using SrpAPI.Common;
using SrpAPI.WxInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace UpLoadImg.Controllers
{
    public class IndexController : BasePager
    {
        //private static string photofolder = System.Configuration.ConfigurationManager.AppSettings["ProjecToxfordPhotos"];
        //
        // GET: /Index/
        List<SpeciesPople> list = new List<SpeciesPople>();
        public ActionResult Index()
        {
            downtemp("A4BG2I_jAk71lfPzk0gxfQApgoeJNN0mMo0qLS7jcCjgldJ23kupHAnjljzl0Rs3");
            return View();
        }
        //2.2   2
        public ActionResult Couple()
        {
            return View();
        }
        // 2.1  1
        public ActionResult Couple2()
        {
            return View();
        }
        //1.1  1
        public ActionResult Family()
        {
            //dowimg("A4BG2I_jAk71lfPzk0gxfQApgoeJNN0mMo0qLS7jcCjgldJ23kupHAnjljzl0Rs3");
            return View();
        }
        //1.2  3
        public ActionResult Family3()
        {
            return View();
        }

        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="Who">照片上是谁（Mom  Dad  Child ManAndWife ）</param>
        /// <param name="Species">种类（1.1：娃像谁 3人照片 1张；1.2：娃像谁 单人照 3张 ；2.1：夫妻相 双人照 1张；2.2夫妻相 2张）</param>
        /// <param name="serverId">服务器id</param>
        /// <returns></returns>
        public JsonResult downtemp(string serverId)
        {
            FaceController controller = new FaceController();
            controller.Detect(serverId);
            int isLock = 1;
            string ImgUrl = string.Empty;

            ////下载图片
            //ImgUrl = dowimg(serverId);
            //Mylog.Log(ImgUrl);
            //if (!string.IsNullOrWhiteSpace(ImgUrl))
            //{
            //    isLock = 1;
            //}
            return Json(new { isLock = isLock, ImgUrl = ImgUrl }, JsonRequestBehavior.AllowGet);
        }
        public string dowimg(string mediaid)
        {
            try
            {
                APPIdAndSecret App = new APPIdAndSecret();
                string access_token = TokenMannger.Instance.GetToken("test", App.AppID, App.AppSecret);
                string path = "https://api.weixin.qq.com/cgi-bin/media/get?access_token=" + access_token + "&media_id=" + mediaid;
                WebClient wc = new WebClient();
                string name = System.DateTime.Now.ToString("yyyymmddhhmmssmmmm") + ".jpg";
                var photofolder = Server.MapPath("~/Images/");
                if (!Directory.Exists(photofolder))
                {
                    //Directory.CreateDirectory(Server.MapPath("~/Images/"));
                    Directory.CreateDirectory(photofolder);
                }
                // Mylog.Error(path + " : " + Server.MapPath("~/Images/") + name);
                Mylog.Error(path + " : " + photofolder + "\\" + name);
                wc.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
                Byte[] pageData = wc.DownloadData(path); //从指定网站下载数据
                string pageHtml = Encoding.UTF8.GetString(pageData);  //如果获取网站页面采用的是GB2312，则使用这句            
                if (!pageHtml.Contains("errmsg"))
                {
                    // wc.DownloadFile(path, Server.MapPath("~/Images/") + name);//将图片拷贝到
                    wc.DownloadFile(path, photofolder + "\\" + name);//将图片拷贝到
                    return name;
                }
                else
                {
                    Mylog.Error(pageHtml);
                    return "";
                }
            }
            catch (Exception ex)
            {
                Mylog.Error(ex.Message);
                throw;
            }

        }

        /// <summary>
        /// 照片相识度对比
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult Compare(string Species, SpeciesPople model)
        {
            /// <param name="Species">种类（1.1：娃像谁 3人照片 1张；1.2：娃像谁 单人照 3张 ；2.1：夫妻相 双人照 1张；2.2夫妻相 2张）</param>
            int isLock = 0;
            string ImgMsg = string.Empty;

            switch (Species)
            {
                case "1.1":
                    if (!string.IsNullOrWhiteSpace(model.ManAndWifeAndChild))
                    {
                        isLock = 1;
                        // model.ManAndWifeAndChild
                        //去测试相识度，返回对应信息（孩子像谁？？？？）
                    }
                    else
                    {
                        ImgMsg = "图片未完全上传！";
                    }

                    break;
                case "1.2":
                    if (!string.IsNullOrWhiteSpace(model.Child) && !string.IsNullOrWhiteSpace(model.Mom) && !string.IsNullOrWhiteSpace(model.Dad))
                    {
                        isLock = 1;
                        //model.Child
                        //model.Mom
                        //model.Dad
                    }
                    else
                    {
                        ImgMsg = "图片未完全上传！";
                    }

                    break;
                case "2.1":
                    if (!string.IsNullOrWhiteSpace(model.ManAndWife))
                    {
                        isLock = 1;
                        //model.ManAndWife

                    }
                    else
                    {
                        ImgMsg = "图片未完全上传！";
                    }
                    break;
                case "2.2":
                    if (!string.IsNullOrWhiteSpace(model.Dad) && !string.IsNullOrWhiteSpace(model.Mom))
                    {
                        isLock = 1;
                        //model.Dad
                        //model.Mom

                    }
                    else
                    {
                        ImgMsg = "图片未完全上传！";
                    }
                    break;
                default:
                    break;
            }
            return Json(new { isLock = isLock, ImgMsg = ImgMsg }, JsonRequestBehavior.AllowGet);
        }
    }

    public class SpeciesPople
    {
        public string Mom { get; set; }
        public string Dad { get; set; }
        public string Child { get; set; }
        public string ManAndWife { get; set; }
        public string ManAndWifeAndChild { get; set; }
    }
}
