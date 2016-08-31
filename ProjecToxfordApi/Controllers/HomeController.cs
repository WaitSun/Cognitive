using DDClassLib.Core.BaseController;
using DDClassLib.Core.test;
using Newtonsoft.Json;
using ProjecToxfordApi.Models;
using SrpAPI.Common;
using SrpAPI.WxInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ProjecToxfordApi.Controllers
{
    public class HomeController : BasePager
    {
        List<SpeciesPople> list = new List<SpeciesPople>();
        FaceController controller = new FaceController();

        public ActionResult Index()
        {
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
            int isLock = 1;
            string ImgUrl = string.Empty;
            #region
            //下载图片
            ImgUrl = dowimg(serverId);
            if (!string.IsNullOrWhiteSpace(ImgUrl))
            {
                isLock = 1;
            }
            #endregion
            return Json(new { isLock = isLock, ImgUrl = ImgUrl }, JsonRequestBehavior.AllowGet);
        }
        public string dowimg(string serverId)
        {
            try
            {

                APPIdAndSecret App = new APPIdAndSecret();
                string access_token = TokenMannger.Instance.GetToken("test", App.AppID, App.AppSecret);
                string path = "https://api.weixin.qq.com/cgi-bin/media/get?access_token=" + access_token + "&media_id=" + serverId;
                WebClient wc = new WebClient();
                string name = serverId + ".jpg";
                var photofolder = Server.MapPath("~/Images/");
                if (!Directory.Exists(photofolder))
                {
                    Directory.CreateDirectory(photofolder);
                }
                if ((System.IO.File.Exists(photofolder + name)))
                {
                    return name;
                }
                Mylog.Error(path + " : " + photofolder + "\\" + name);
                wc.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
                Byte[] pageData = wc.DownloadData(path); //从指定网站下载数据
                string pageHtml = Encoding.UTF8.GetString(pageData);  //如果获取网站页面采用的是GB2312          
                if (!pageHtml.Contains("errmsg"))
                {               
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
            // <param name="Species">种类（1.1：娃像谁 3人照片 1张；1.2：娃像谁 单人照 3张 ；2.1：夫妻相 双人照 1张；2.2夫妻相 2张）</param>
            int isLock = 0;
            string ImgMsg = "图片未完全上传!";
            string xiangsidu = string.Empty;
            Mylog.Log("Species =" + Species + " Child= " + model.Child + " Dad= " + model.Dad + " ManAndWife= " + model.ManAndWife + " ManAndWifeAndChild= " + model.ManAndWifeAndChild + " Mom= " + model.Mom);
            string fileName = string.Empty;
            List<DetectResultModels> listmodel = new List<DetectResultModels>();
            switch (Species)
            {
                case "1.1":  //Family3
                    if (!string.IsNullOrWhiteSpace(model.ManAndWifeAndChild))
                    {
                        #region MyRegion
                        // model.ManAndWifeAndChild
                        //测试相识度，返回对应信息（孩子像谁？？？？）                     
                        //下载图片 获取 faceId json                       
                        fileName = dowimg(model.ManAndWifeAndChild);
                        if (!string.IsNullOrWhiteSpace(fileName))
                        {
                            controller.Detect(model.ManAndWifeAndChild, fileName); //获取的face信息
                            model.ManAndWifeAndChild = controller.rest;
                            controller.rest = null;
                        }
                        else
                        {
                            return Json(new { isLock = 0, ImgMsg = "图片上传出错，请稍后重试；" }, JsonRequestBehavior.AllowGet);
                        }
                        
                        if (model.ManAndWifeAndChild != null)
                        {
                            //获取照片信息
                            foreach (var item in JsonConvert.DeserializeObject<dynamic>(model.ManAndWifeAndChild))
                            {
                                var ResultModels = new DetectResultModels();
                                ResultModels.faceId = item.faceId; ResultModels.FileName = item.fileName; ResultModels.Age = item.age; ResultModels.Gender = item.gender == "male" ? "男" : "女"; ResultModels.Smile = item.smile;
                                listmodel.Add(ResultModels);
                            }
                            if (listmodel.Count == 3)
                            {
                                isLock = 1;
                                #region 先找出年龄最小的（孩子）
                                if (listmodel[0].Age > listmodel[1].Age)
                                {
                                    if (listmodel[1].Age > listmodel[2].Age)
                                    {
                                        // listmodel[2];//是孩子
                                        #region MyRegion
                                        if (listmodel[0].Gender == "男")
                                        {
                                            double DadAndChild = Getconfidence(listmodel[0].faceId, listmodel[2].faceId); //孩子与爸爸对比
                                            double WifeAndChild = Getconfidence(listmodel[1].faceId, listmodel[2].faceId);//孩子与妈妈对比
                                            ImgMsg = GetConfidenceMsg(DadAndChild, WifeAndChild);
                                        }
                                        else
                                        {
                                            double DadAndChild = Getconfidence(listmodel[1].faceId, listmodel[2].faceId); //孩子与爸爸对比
                                            double WifeAndChild = Getconfidence(listmodel[0].faceId, listmodel[2].faceId);//孩子与妈妈对比
                                            ImgMsg = GetConfidenceMsg(DadAndChild, WifeAndChild);
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        //   listmodel[1]//是孩子
                                        #region MyRegion
                                        if (listmodel[0].Gender == "男")
                                        {
                                            double DadAndChild = Getconfidence(listmodel[0].faceId, listmodel[1].faceId); //孩子与爸爸对比
                                            double WifeAndChild = Getconfidence(listmodel[2].faceId, listmodel[1].faceId);//孩子与妈妈对比
                                            ImgMsg = GetConfidenceMsg(DadAndChild, WifeAndChild);
                                        }
                                        else
                                        {
                                            double DadAndChild = Getconfidence(listmodel[2].faceId, listmodel[1].faceId); //孩子与爸爸对比
                                            double WifeAndChild = Getconfidence(listmodel[0].faceId, listmodel[1].faceId);//孩子与妈妈对比
                                            ImgMsg = GetConfidenceMsg(DadAndChild, WifeAndChild);
                                        }
                                        #endregion
                                    }
                                }
                                else
                                {
                                    if (listmodel[0].Age > listmodel[2].Age)
                                    {
                                        //    listmodel[2];//是孩子
                                        #region MyRegion
                                        if (listmodel[0].Gender == "男")
                                        {
                                            double DadAndChild = Getconfidence(listmodel[0].faceId, listmodel[2].faceId); //孩子与爸爸对比
                                            double WifeAndChild = Getconfidence(listmodel[1].faceId, listmodel[2].faceId);//孩子与妈妈对比
                                            ImgMsg = GetConfidenceMsg(DadAndChild, WifeAndChild);
                                        }
                                        else
                                        {
                                            double DadAndChild = Getconfidence(listmodel[1].faceId, listmodel[2].faceId); //孩子与爸爸对比
                                            double WifeAndChild = Getconfidence(listmodel[0].faceId, listmodel[2].faceId);//孩子与妈妈对比
                                            ImgMsg = GetConfidenceMsg(DadAndChild, WifeAndChild);
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        //    listmodel[0];//是孩子
                                        #region MyRegion
                                        if (listmodel[1].Gender == "男")
                                        {
                                            double DadAndChild = Getconfidence(listmodel[1].faceId, listmodel[0].faceId); //孩子与爸爸对比
                                            double WifeAndChild = Getconfidence(listmodel[2].faceId, listmodel[0].faceId);//孩子与妈妈对比
                                            ImgMsg = GetConfidenceMsg(DadAndChild, WifeAndChild);
                                        }
                                        else
                                        {
                                            double DadAndChild = Getconfidence(listmodel[2].faceId, listmodel[0].faceId); //孩子与爸爸对比
                                            double WifeAndChild = Getconfidence(listmodel[1].faceId, listmodel[0].faceId);//孩子与妈妈对比
                                            ImgMsg = GetConfidenceMsg(DadAndChild, WifeAndChild);
                                        }
                                        #endregion
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                ImgMsg = "请上传一张清晰的一个孩子与父母的合影！";
                            }

                        }
                        else
                        {
                            ImgMsg = "图片处理失败!请重新上传清晰的图片"; //图片处理失败（获取faceId 失败）
                        }
                        #endregion
                    }
                    listmodel = null;
                    break;
                case "1.2":   //Family
                    if (!string.IsNullOrWhiteSpace(model.Child) && !string.IsNullOrWhiteSpace(model.Mom) && !string.IsNullOrWhiteSpace(model.Dad))
                    {
                        #region 下载图片 获取 faceId json 
                        fileName = dowimg(model.Mom);
                        if (!string.IsNullOrWhiteSpace(fileName))
                        {
                            controller.Detect(model.Mom, fileName); //获取的face信息
                            model.Mom = controller.rest;
                            controller.rest = null;
                        }
                        else
                        {
                            return Json(new { isLock = 0, ImgMsg = "图片上传出错，请稍后重试；" }, JsonRequestBehavior.AllowGet);
                        }
                        fileName = dowimg(model.Dad);
                        if (!string.IsNullOrWhiteSpace(fileName))
                        {
                            controller.Detect(model.Dad, fileName); //获取的face信息
                            model.Dad = controller.rest;
                            controller.rest = null;
                        }
                        else
                        {
                            return Json(new { isLock = 0, ImgMsg = "图片上传出错，请稍后重试；" }, JsonRequestBehavior.AllowGet);
                        }
                        fileName = dowimg(model.Child);
                        if (!string.IsNullOrWhiteSpace(fileName))
                        {
                            controller.Detect(model.Child, fileName); //获取的face信息
                            model.Child = controller.rest;
                            controller.rest = null;
                        }
                        else
                        {
                            return Json(new { isLock = 0, ImgMsg = "图片上传出错，请稍后重试；" }, JsonRequestBehavior.AllowGet);
                        }
                        #endregion

                        #region MyRegion
                        if (model.Child != null && model.Mom != null && model.Dad != null)
                        {
                            //获取照片信息
                            model.Child = JsonConvert.DeserializeObject<dynamic>(model.Child)[0].faceId;
                            model.Mom = JsonConvert.DeserializeObject<dynamic>(model.Mom)[0].faceId;
                            model.Dad = JsonConvert.DeserializeObject<dynamic>(model.Dad)[0].faceId;
                            isLock = 1;
                            double DadAndChild = Getconfidence(model.Dad, model.Child); //孩子与爸爸对比
                            double WifeAndChild = Getconfidence(model.Mom, model.Child);//孩子与妈妈对比
                            ImgMsg = GetConfidenceMsg(DadAndChild, WifeAndChild);
                        }
                        else
                        {
                            ImgMsg = "图片处理失败!请重新上传清晰的图片"; //图片处理失败（获取faceId 失败）
                        }
                        #endregion
                    }
                    listmodel = null;
                    break;
                case "2.1":  //Couple2
                    if (!string.IsNullOrWhiteSpace(model.ManAndWife))
                    {
                        #region MyRegion
                        fileName = dowimg(model.ManAndWife);
                        if (!string.IsNullOrWhiteSpace(fileName))
                        {
                            #region MyRegion
                            controller.Detect(model.ManAndWife, fileName); //获取的face信息
                            model.ManAndWife = controller.rest;
                            if (model.ManAndWife != null)
                            {
                                //获取照片信息
                                foreach (var item in JsonConvert.DeserializeObject<dynamic>(model.ManAndWife))
                                {
                                    var ResultModels = new DetectResultModels();
                                    ResultModels.faceId = item.faceId; ResultModels.FileName = item.fileName; ResultModels.Age = item.age; ResultModels.Gender = item.gender == "male" ? "男" : "女"; ResultModels.Smile = item.smile;
                                    listmodel.Add(ResultModels);
                                }
                                if (listmodel.Count == 2)
                                {
                                    isLock = 1;
                                    double DadAndChild = Getconfidence(listmodel[0].faceId, listmodel[1].faceId); //孩子与爸爸对比
                                    ImgMsg = "你们的夫妻相是：" + DadAndChild * 100 + "% ";
                                }
                                else
                                {
                                    ImgMsg = "请上传一张清晰的帅哥与美女的合影！";
                                }
                            }
                            else
                            {
                                ImgMsg = "图片处理失败!请重新上传清晰的图片"; //图片处理失败（获取faceId 失败）
                            }
                            #endregion
                        }
                        else
                        {
                            ImgMsg = "图片上传出错，请稍后重试；";
                        }

                        #endregion
                    }
                    listmodel = null;
                    break;
                case "2.2":  //Couple
                    if (!string.IsNullOrWhiteSpace(model.Dad) && !string.IsNullOrWhiteSpace(model.Mom))
                    {
                        #region 下载图片 获取 faceId json 
                        fileName = dowimg(model.Mom);
                        if (!string.IsNullOrWhiteSpace(fileName))
                        {
                            controller.Detect(model.Mom, fileName); //获取的face信息                                                                
                            model.Mom = controller.rest;
                            controller.rest = null;
                        }
                        else
                        {
                            return Json(new { isLock = 0, ImgMsg = "图片上传出错，请稍后重试；" }, JsonRequestBehavior.AllowGet);
                        }
                        fileName = dowimg(model.Dad);
                        if (!string.IsNullOrWhiteSpace(fileName))
                        {
                            // System.Threading.Thread.Sleep(5000);
                            controller.Detect(model.Dad, fileName); //获取的face信息

                            model.Dad = controller.rest;
                            controller.rest = null;
                        }
                        else
                        {
                            return Json(new { isLock = 0, ImgMsg = "图片上传出错，请稍后重试；" }, JsonRequestBehavior.AllowGet);
                        }
                        #endregion
                        #region MyRegion
                        if (model.Mom != null && model.Dad != null)
                        {
                            model.Mom = JsonConvert.DeserializeObject<dynamic>(model.Mom)[0].faceId;
                            model.Dad = JsonConvert.DeserializeObject<dynamic>(model.Dad)[0].faceId;
                            isLock = 1;
                            double DadAndChild = Getconfidence(model.Dad, model.Mom);
                            ImgMsg = "你们的夫妻相是：" + DadAndChild * 100 + "% ";

                        }
                        else
                        {
                            ImgMsg = "图片处理失败!请重新上传清晰的图片"; //图片处理失败（获取faceId 失败）
                        }
                        #endregion
                    }
                    listmodel = null;
                    break;
                default:
                    break;
            }
            return Json(new
            {
                isLock = isLock,
                ImgMsg = ImgMsg
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 传入孩子与爸爸和妈妈的相识度，返回提示信息
        /// </summary>
        /// <param name="DadAndChild"></param>
        /// <param name="WifeAndChild"></param>
        /// <returns></returns>
        private static string GetConfidenceMsg(double DadAndChild, double WifeAndChild)
        {
            string ImgMsg;
            if (DadAndChild > 0 && WifeAndChild > 0)
            {
                if (DadAndChild > WifeAndChild)
                {
                    ImgMsg = "孩子与爸爸更像，相识度为 ：" + DadAndChild * 100 + "% ";
                }
                else
                {
                    ImgMsg = "孩子与妈妈更像，相识度为 ：" + WifeAndChild * 100 + "% ";

                }
            }
            else
            {
                ImgMsg = "图片处理失败，请重新上传清晰的图片";
            }

            return ImgMsg;
        }     
        private double Getconfidence(string FristFaceId, string TwoFaceId)
        {
            controller.Verify(FristFaceId, TwoFaceId);
            if (controller.Confidence != null)
            {
                return JsonConvert.DeserializeObject<dynamic>(controller.Confidence.ToString()).confidence;
            }
            return 0.0;
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
