using System;
using System.Diagnostics;
using System.Web.Mvc;

namespace MVC_ActionFilter.ActionFilter
{
    public class ActionTimerAttribute : ActionFilterAttribute
    {
        //  在要計算時間的Action開頭引用        
        //  [MVC_ActionFilter.ActionFilter.ActionTimer]

        private Stopwatch swAction;
        private Stopwatch swView;

        public ActionTimerAttribute()
        {
            swAction = new Stopwatch();
            swView = new Stopwatch();
        }

        #region 計算Action執行時間

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            swAction.Start();
            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var controllerName = (string)filterContext.RouteData.Values["controller"];
            var actionName = (string)filterContext.RouteData.Values["action"];

            swAction.Stop();
            TimeSpan ts = swAction.Elapsed;
            swAction.Reset();
            Debug.WriteLine("Action執行時間: {" + ts.ToString() + "} from {" + controllerName + "}/{" + actionName + "}");
            
            base.OnActionExecuted(filterContext);
        }

        #endregion

        #region 計算View執行時間

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            swView.Start();
            base.OnResultExecuting(filterContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            var controllerName = (string)filterContext.RouteData.Values["controller"];
            var actionName = (string)filterContext.RouteData.Values["action"];

            swView.Stop();
            TimeSpan ts = swView.Elapsed;
            swView.Reset();
            Debug.WriteLine("View執行時間: {" + ts.ToString() + "} from {" + controllerName + "}/{" + actionName + "}");

            base.OnResultExecuted(filterContext);
        }

        #endregion
    }
}