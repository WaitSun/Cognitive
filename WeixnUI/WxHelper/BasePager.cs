using System;
using System.Web;
using System.Web.Mvc;
using SrpAPI.WxInterface;
using SrpAPI.Common;
using DDClassLib.Core.test;

namespace DDClassLib.Core.BaseController
{
    /// <summary>
    /// js分享  filterContext.Controller.ViewBag.JsConfig
    /// </summary>
    public class BasePager : Controller
    {
        APPIdAndSecret App = new APPIdAndSecret();
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string AppName = string.Empty;
            string controller = filterContext.HttpContext.Request.Url.PathAndQuery.Substring(1); //适应带参数的返回 xxx?
            if (filterContext.HttpContext.Request.Url.AbsoluteUri.Contains("localhost"))
            {
                AppName = controller.IndexOf("/") > -1 ? controller.Substring(0, controller.IndexOf("/")) : controller;
            }
            else
            {
                AppName = controller.IndexOf("/") > -1 ? controller.Split('/')[1] : controller;
                AppName = AppName.Split('?')[0];
            }
           // ViewBag.User = (Core.Models.UserModel)Session["_User"];
            AppName = "test";

            var token = TokenMannger.Instance.GetToken(AppName, App.AppID, App.AppSecret);
            filterContext.Controller.ViewBag.JsConfig = JsticketMannger.Instance.GetSignature(App.AppID, AppName, Request.Url.AbsoluteUri, token);
        }
    }
}
