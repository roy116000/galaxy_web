using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace galaxy_web
{
    // 注意: 如需啟用 IIS6 或 IIS7 傳統模式的說明，
    // 請造訪 http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            System.Data.Entity.Database.SetInitializer<Models._dbContext>(null);
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

        }

        //protected void Application_Error(object sender, EventArgs e)
        //{
        //    Exception exception = Server.GetLastError();
        //    Response.Clear();

        //    HttpException httpException = exception as HttpException;
        //    if (httpException != null)
        //    {
        //        RouteData routeData = new RouteData();
        //        routeData.Values.Add("controller", "Home");
        //        routeData.Values.Add("action", "Error");
        //        //switch (httpException.GetHttpCode())
        //        //{
        //        //    case 404:
        //        //        // page not found
        //        //        routeData.Values.Add("action", "HttpError404");
        //        //        break;
        //        //    case 500:
        //        //        // server error
        //        //        routeData.Values.Add("action", "HttpError500");
        //        //        break;
        //        //    default:
        //        //        routeData.Values.Add("action", "General");
        //        //        break;
        //        //}
        //        routeData.Values.Add("code", httpException.GetHttpCode());
        //        routeData.Values.Add("error", exception);
        //        // clear error on server
        //        Server.ClearError();

        //        // at this point how to properly pass route data to error controller?
        //    }
        //}

        //protected void Application_Error(object sender, EventArgs e)
        //{
        //    var Url = Request;

        //    //這些副檔名不跑
        //    string[] Extension = { ".jpg", ".gif", "png" };
        //    if (!Extension.Contains(Url.CurrentExecutionFilePathExtension.ToLower()))
        //    {
        //        string msg = "系統錯誤，請洽管理員!!(-101)";
        //        var exception = Server.GetLastError();
        //        var httpException = exception as HttpException;
        //        Response.Clear();
        //        Server.ClearError();
        //        var routeData = new RouteData();
        //        routeData.Values["controller"] = "Home";
        //        routeData.Values["action"] = "Error";
        //        routeData.Values["exception"] = exception;
        //        Response.ContentType = "text/html";
        //        Response.StatusCode = 500;
        //        if (httpException != null)
        //        {
        //            Response.StatusCode = httpException.GetHttpCode();
        //            switch (Response.StatusCode)
        //            {
        //                case 404:
        //                    //routeData.Values["action"] = "PageNotFound";
        //                    msg = "糟糕，您使用的網址不存在!!(404)";
        //                    break;
        //            }
        //        }

        //        //使用者
        //        routeData.Values["msg"] = msg;

        //        NW.Log("現在時間：" + NW.Now + "，Url：" + Url + "，Code：" + Response.StatusCode + "，Message：" + exception.Message);

        //        Response.RedirectToRoute("web1", new { controller = "Home", action = "Error", msg = Response.StatusCode });
        //        Response.End();

        //        //IController errorsController = new galaxy_web.Controllers.HomeController();
        //        //var rc = new RequestContext(new HttpContextWrapper(Context), routeData);
        //        //errorsController.Execute(rc);
        //    }

        //}

    }


}