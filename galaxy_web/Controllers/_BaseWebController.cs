using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace galaxy_web.Controllers
{
    public class _BaseWebController : Controller
    {
        ////找不到ACTION
        //protected override void HandleUnknownAction(string actionName)
        //{
        //    var R = RouteData.Values.ToDictionary(m => m.Key, m => m.Value.ToString());

        //    ViewData["R"] = R;

        //    this.View(actionName).ExecuteResult(this.ControllerContext);

        //}

    }
}
