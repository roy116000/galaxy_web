using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcPaging;
using galaxy_web.Models;

namespace galaxy_web.Areas.newweb.Controllers
{
    public class NwNewsController : _BaseController
    {
        //
        // GET: /newweb/NwNews/

        public ActionResult List(int? page, string kw, int? gid, DateTime? sD, DateTime? eD)
        {
            List<ListItem> ListItems = new List<ListItem>() { };

            var linq = this._DB.news_data.Where(m => m.news_id > 0);

            #region"kw"

            if (!string.IsNullOrEmpty(kw))
            {
                ViewBag.kw = kw;
                linq = linq.Where(m => m.title.Contains(kw));
            }

            #endregion

            #region"gid"

            ListItems = new List<ListItem>() { };

            var G_name = this._DB.news_group.ToDictionary(m => m.group_id, m => m.name);
            ViewData["G_name"] = G_name;

            ListItems = this._DB.news_group.Where(m => m.visible).ToList()
                .Select(m => new ListItem(m.group_id.ToString(), m.name + "（" + linq.Where(x => x.group_id == m.group_id).Count() + "）", gid == m.group_id)).ToList();
            ListItems.Insert(0, new ListItem("", "全部（" + linq.Count() + "）", false));

            ViewData["G"] = ListItems;

            if (gid.HasValue)
            {
                ViewBag.gid = gid;
                linq = linq.Where(m => m.group_id == gid);
            }

            #endregion

            #region"datetime"

            if (sD.HasValue)
            {
                ViewBag.sD = NW.getDate(sD);
                linq = linq.Where(m => m.start_date >= sD);
            }

            if (eD.HasValue)
            {
                ViewBag.eD = NW.getDate(eD);
                eD = eD.Value.AddDays(1);
                linq = linq.Where(m => m.end_date < eD);
            }

            #endregion

            int currentPageIndex = page.HasValue && page.Value > 0 ? page.Value - 1 : 0;
            var data = linq.OrderByDescending(m => m.change_datetime ?? m.create_datetime).ToPagedList(currentPageIndex, 20);

            ViewBag.jgrowl = TempData[NW.ssjgrowl];
            if (page.HasValue)
            {
                return PartialView(RouteData.Values["action"].ToString() + "_ajax", data);
            }
            else
            {
                return View(data);
            }

        }

        public ActionResult Add(int id)
        {
            ViewBag.jgrowl = TempData[NW.ssjgrowl];

            var data = this._DB.news_data.Where(m => m.news_id == id).FirstOrDefault() ?? new news_data() { };
            if (data.news_id == 0)
            {
                data.start_date = NW.Now;
                data.end_date = data.start_date.Value.AddYears(10).AddDays(-1);
                data.visible = true;
            }

            #region"選項"

            List<ListItem> ListItems = new List<ListItem>() { };
            ListItems.AddRange(this._DB.news_group.Where(m => m.visible).ToList().Select(m => new ListItem(m.group_id.ToString(), m.name, m.group_id == data.group_id)));
            ListItems.Insert(0, new ListItem("", "請選擇", false));
            ViewData["group_id"] = ListItems;

            #endregion

            return View(data);
        }

        public ActionResult Edit(int id)
        {

            return Add(id);
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult EditSave(news_data Get)
        {
            string msg = "";

            var data = this._DB.news_data.Where(m => m.news_id == Get.news_id).FirstOrDefault() ?? new news_data() { };

            data.group_id = Get.group_id;
            data.title = Get.title;
            data.info = Get.info;
            data.cont = Get.cont;
            data.start_date = Get.start_date;
            data.end_date = Get.end_date;
            data.visible = Get.visible;
            data.inx = Get.inx;
            data.hits = Get.hits;

            if (data.news_id == 0)
            {
                data.create_user = User.Identity.Name;
                data.create_datetime = NW.Now;

                this._DB.news_data.Add(data);

                msg = "新增成功";
            }
            else
            {
                data.change_user = User.Identity.Name;
                data.change_datetime = NW.Now;

                msg = "修改成功";
            }

            this._DB.SaveChanges();

            TempData[NW.ssjgrowl] = msg;
            return RedirectToAction(RouteData.Values["action"].ToString().Replace("Save", ""), new { id = data.news_id });
        }

    }
}
