using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace galaxy_web.Controllers
{
    public class HomeController : _BaseWebController
    {

        public ActionResult Index()
        {


            return Redirect("/newweb");

            //var sn = Gs.getIPAddress;

            //if (sn == "::1" || sn.IndexOf("192.168.1") > -1)
            //{
            //    return Redirect("/newweb");
            //}
            //else
            //{
            //    return Redirect("http://www.google.com");
            //}

        }

        public ActionResult Error(string msg)
        {
            try
            {
                switch (msg)
                {
                    case "404":

                        msg = "糟糕，您使用的網址不存在!!(404)";
                        break;

                    default:
                        msg = "系統錯誤，請洽管理員!!(-101)";
                        break;
                }

                ViewBag.msg = msg;

                //var data = RouteData;
                //var httpException = RouteData.Values["exception"] as HttpException;
                //ViewBag.msg = RouteData.Values["msg"].ToString();

                //寫入記錄檔
                //NW.Log("現在時間：" + NW.Now + "，Url：" + Url + "，Code：" + Response.StatusCode + "，Message：" + httpException.Message);
            }
            catch
            {

            }
            return View();
        }

    }
}
