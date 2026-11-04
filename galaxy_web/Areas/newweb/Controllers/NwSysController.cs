using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using galaxy_web.Models;
using MvcPaging;
using System.Data.Linq;


namespace galaxy_web.Areas.newweb.Controllers
{
    public class NwSysController : _BaseController
    {
        //
        // GET: /newweb/NwSys/
        #region "功能列表"

        public ActionResult List(string kw)
        {

            int currentPageIndex = 0;
            IPagedList data;
            if (String.IsNullOrEmpty(kw))
            {
                data = this._DB.sys_function_data.OrderBy(m => m.inx).ToPagedList(currentPageIndex, 999);
            }
            else
            {
                data = this._DB.sys_function_data.Where(m => m.label.Contains(kw)).OrderBy(m => m.inx).ToPagedList(currentPageIndex, 999);
            }
            ViewBag.kw = kw;
            ViewBag.jgrowl = TempData["jgrowl"];
            return View(data);
        }

        public ActionResult List_ajax(int? page, string kw)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            IPagedList data;
            if (String.IsNullOrEmpty(kw))
            {
                data = this._DB.sys_function_data.OrderBy(m => m.inx).ToPagedList(currentPageIndex, 20);
            }
            else
            {
                data = this._DB.sys_function_data.Where(m => m.label.Contains(kw)).OrderBy(m => m.inx).ToPagedList(currentPageIndex, 20);
            }
            ViewBag.kw = kw;
            return PartialView(RouteData.Values["action"].ToString(), data);
        }


        public ActionResult Edit(int id)
        {

            var data = this._DB.sys_function_data.Find(id);

            ViewBag.jgrowl = TempData["jgrowl"];

            return View(data);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EditSave(sys_function_data sys_function_data)
        {
            var data = this._DB.sys_function_data.Find(sys_function_data.function_id);
            //var data1 = this._DB.sys_function_data.Where(m => m.zz_id.Equals(sys_function_data.zz_id)).Select(m => m).SingleOrDefault();


            if (data == null)
            {
                ModelState.AddModelError("", "查無此筆資料");
            }
            else
            {
                // data.function_id = sys_function_data.function_id;
                data.label = sys_function_data.label;
                data.info = sys_function_data.info;
                data.controll = sys_function_data.controll;
                //data.init_action = sys_function_data.init_action;
                data.icon = sys_function_data.icon;
                //data.icon2 = sys_function_data.icon2;
                data.inx = sys_function_data.inx;
                data.visible = sys_function_data.visible;

                //data.create_user = sys_function_data.create_user;
                //data.create_datetime = sys_function_data.create_datetime;

                this._DB.SaveChanges();

            }

            //更新

            TempData["jgrowl"] = "資料更新成功";

            return RedirectToAction(RouteData.Values["action"].ToString().Replace("Save", ""), new { id = sys_function_data.function_id });
        }

        public ActionResult Add(int id)
        {
            sys_function_data data = new sys_function_data();

            data.visible = true;
            return View(data);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AddSave(sys_function_data sys_function_data)
        {
            //var data = this._DB.sys_function_data.Find(sys_function_data.zz_id);
            //var data1 = this._DB.sys_function_data.Where(m => m.zz_id.Equals(sys_function_data.zz_id)).Select(m => m).SingleOrDefault();
            var data = new sys_function_data { };

            if (data == null)
            {
                ModelState.AddModelError("", "查無此筆資料");
            }
            else
            {

                // data.function_id = sys_function_data.function_id;
                data.label = sys_function_data.label;
                data.info = sys_function_data.info;
                data.controll = sys_function_data.controll;
                //data.init_action = sys_function_data.init_action;
                data.icon = sys_function_data.icon;
                //data.icon2 = sys_function_data.icon2;
                data.inx = sys_function_data.inx;
                data.visible = sys_function_data.visible;


                _DB.sys_function_data.Add(data);
                this._DB.SaveChanges();

                sys_function_data.function_id = data.function_id;
            }

            //更新

            TempData["jgrowl"] = "資料新增成功!!";

            return RedirectToAction(RouteData.Values["action"].ToString().Replace("AddSave", "Edit"), new { id = sys_function_data.function_id });
        }

        [HttpPost]
        public ActionResult ListDel(int cb_flag, string cb_id)
        {
            string[] jgrowl_msg = new string[] { "刪除", "啟用", "停用" };
            int[] strArray = Array.ConvertAll<string, int>(NW.Split(cb_id, ","), int.Parse);


            var data = (from xx in _DB.sys_function_data
                        where strArray.Contains(xx.function_id)
                        select xx).ToList();

            foreach (var item in data)
            {
                sys_function_data vmD = _DB.sys_function_data.Find(item.function_id);

                switch (cb_flag)
                {
                    case 1:
                        vmD.visible = true;
                        break;
                    case 2:
                        vmD.visible = false;
                        break;
                    case 0:
                        _DB.sys_function_data.Remove(vmD);
                        break;
                }
                _DB.SaveChanges();
            }

            TempData["jgrowl"] = jgrowl_msg[cb_flag] + "功能，設定成功";

            return RedirectToAction(RouteData.Values["action"].ToString().Replace("Del", ""));
        }

        #endregion

        #region "子菜單列表"

        public ActionResult SubList(int id, string kw)
        {

            int currentPageIndex = 0;
            IPagedList data;
            if (String.IsNullOrEmpty(kw))
            {
                data = this._DB.sys_function_sub.Where(m => m.function_id == id).OrderBy(m => m.function_id).ToPagedList(currentPageIndex, 999);
            }
            else
            {
                data = this._DB.sys_function_sub.Where(m => m.function_id == id).Where(m => m.label.Contains(kw)).OrderBy(m => m.function_id).ToPagedList(currentPageIndex, 999);
            }

            List<sys_function_data> data2 = this._DB.sys_function_data.OrderBy(m => m.inx).ToList();
            ViewData["sys_function_data"] = data2;

            ViewBag.kw = kw;
            ViewBag.jgrowl = TempData["jgrowl"];
            return View(data);
        }

        public ActionResult SubList_ajax(int id, int? page, string kw)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            IPagedList data;
            if (String.IsNullOrEmpty(kw))
            {
                data = this._DB.sys_function_data.OrderBy(m => m.function_id).ToPagedList(currentPageIndex, 20);
            }
            else
            {
                data = this._DB.sys_function_data.Where(m => m.label.Contains(kw)).OrderBy(m => m.function_id).ToPagedList(currentPageIndex, 20);
            }
            ViewBag.kw = kw;
            return PartialView(RouteData.Values["action"].ToString(), data);
        }


        public ActionResult SubEdit(int id)
        {

            var data = this._DB.sys_function_sub.Find(id);

            var data2 = this._DB.sys_function_data.OrderBy(m => m.inx);

            //單選
            List<ListItem> ListItems = new List<ListItem> { };

            foreach (var item in data2)
            {
                ListItems.Add(new ListItem(item.function_id.ToString(), item.label, (item.function_id == data.function_id)));
            }

            ViewData["function_id"] = ListItems;

            ViewBag.jgrowl = TempData["jgrowl"];

            return View(data);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SubEditSave(sys_function_sub sys_function_sub)
        {
            var data = this._DB.sys_function_sub.Find(sys_function_sub.function_sub_id);
            //var data1 = this._DB.sys_function_data.Where(m => m.zz_id.Equals(sys_function_data.zz_id)).Select(m => m).SingleOrDefault();


            if (data == null)
            {
                ModelState.AddModelError("", "查無此筆資料");
            }
            else
            {
                //data.function_sub_id = sys_function_sub.function_sub_id;
                data.function_id = sys_function_sub.function_id;
                data.label = sys_function_sub.label;
                data.info = sys_function_sub.info;
                data.action = sys_function_sub.action;
                data.inx = sys_function_sub.inx;
                data.show_visible = sys_function_sub.show_visible;
                data.visible = sys_function_sub.visible;

                this._DB.SaveChanges();

            }

            //更新

            TempData["jgrowl"] = "資料更新成功";

            return RedirectToAction(RouteData.Values["action"].ToString().Replace("Save", ""), new { id = sys_function_sub.function_sub_id });
        }

        public ActionResult SubAdd(int id)
        {
            sys_function_sub data = new sys_function_sub();

            //var data2 = this._DB.sys_function_data.OrderBy(m => m.inx);

            //單選

            List<ListItem> ListItems = new List<ListItem> { };

            var F = this._DB.sys_function_data.OrderBy(m => m.inx).Select(m => m.function_id + "," + m.label).ToArray();

            ListItems.AddRange(Gs.sList(F, id.ToString()));

            //foreach (var item in data2)
            //{
            //    ListItems.Add(new ListItem(item.function_id.ToString(), item.label, (item.function_id == id)));
            //}
            ViewData["function_id"] = ListItems;

            return View(data);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SubAddSave(sys_function_sub sys_function_sub, Boolean? add_fun)
        {
            //var data = this._DB.sys_function_data.Find(sys_function_data.zz_id);
            //var data1 = this._DB.sys_function_data.Where(m => m.zz_id.Equals(sys_function_data.zz_id)).Select(m => m).SingleOrDefault();

            //var data = this._DB.sys_function_sub.Where(m => m.function_sub_id == sys_function_sub.function_sub_id).FirstOrDefault();
            //data = data != null ? data : new sys_function_sub() { };

            //if (data == null && sys_function_sub.function_sub_id != 0) { ModelState.AddModelError("", "查無此筆資料"); }
            //else 
            //{

            //}

            var data = new sys_function_sub { };

            if (data == null)
            {
                ModelState.AddModelError("", "查無此筆資料");
            }
            else
            {

                // data.function_id = sys_function_data.function_id;
                data.function_id = sys_function_sub.function_id;
                data.label = sys_function_sub.label;
                data.info = sys_function_sub.info;
                data.action = sys_function_sub.action;
                data.inx = sys_function_sub.inx;
                data.show_visible = sys_function_sub.show_visible;
                data.visible = sys_function_sub.visible;

                _DB.sys_function_sub.Add(data);
                this._DB.SaveChanges();

                sys_function_sub.function_sub_id = data.function_sub_id;

                //1	1	設定菜單	NULL	List	1	1	1
                //2	1	新增菜單	NULL	Add	0	0	0
                //3	1	新增儲存菜單	NULL	AddSave	0	0	1
                //4	1	修改菜單	NULL	Edit	0	0	1
                //5	1	修改儲存菜單	NULL	EditSave	0	0	1
                //6	1	刪除菜單	NULL	ListDel	0	0	1

                if (add_fun == true)
                {
                    string head = sys_function_sub.action.Replace("List", "");
                    data = new sys_function_sub { function_id = sys_function_sub.function_id, label = "新增" + sys_function_sub.label, info = "", action = head + "Add", inx = 0, show_visible = false, visible = true };
                    _DB.sys_function_sub.Add(data);
                    data = new sys_function_sub { function_id = sys_function_sub.function_id, label = "新增儲存" + sys_function_sub.label, info = "", action = head + "AddSave", inx = 0, show_visible = false, visible = true };
                    _DB.sys_function_sub.Add(data);
                    data = new sys_function_sub { function_id = sys_function_sub.function_id, label = "修改" + sys_function_sub.label, info = "", action = head + "Edit", inx = 0, show_visible = false, visible = true };
                    _DB.sys_function_sub.Add(data);
                    data = new sys_function_sub { function_id = sys_function_sub.function_id, label = "修改儲存" + sys_function_sub.label, info = "", action = head + "EditSave", inx = 0, show_visible = false, visible = true };
                    _DB.sys_function_sub.Add(data);
                    data = new sys_function_sub { function_id = sys_function_sub.function_id, label = "刪除" + sys_function_sub.label, info = "", action = head + "ListDel", inx = 0, show_visible = false, visible = true };
                    _DB.sys_function_sub.Add(data);
                    this._DB.SaveChanges();
                }

            }

            //更新

            TempData["jgrowl"] = "資料新增成功!!";

            return RedirectToAction(RouteData.Values["action"].ToString().Replace("AddSave", "Edit"), new { id = sys_function_sub.function_sub_id });
        }

        [HttpPost]
        public ActionResult SubListDel(int cb_flag, string cb_id)
        {
            string[] jgrowl_msg = new string[] { "刪除", "啟用", "停用" };
            int[] strArray = Array.ConvertAll<string, int>(NW.Split(cb_id, ","), int.Parse);

            int id = 0;

            var data = (from xx in _DB.sys_function_sub
                        where strArray.Contains(xx.function_sub_id)
                        select xx).ToList();

            foreach (var item in data)
            {
                sys_function_sub vmD = _DB.sys_function_sub.Find(item.function_sub_id);
                id = vmD.function_id;
                switch (cb_flag)
                {
                    case 1:
                        vmD.visible = true;
                        break;
                    case 2:
                        vmD.visible = false;
                        break;
                    case 0:
                        _DB.sys_function_sub.Remove(vmD);
                        break;
                }
                _DB.SaveChanges();
            }

            TempData["jgrowl"] = jgrowl_msg[cb_flag] + "功能，設定成功";

            return RedirectToAction(RouteData.Values["action"].ToString().Replace("Del", ""), new { id = id });
        }

        #endregion

        #region "權限列表"

        public ActionResult PowerList(string kw)
        {

            int currentPageIndex = 0;
            IPagedList data;
            if (String.IsNullOrEmpty(kw))
            {
                data = this._DB.sys_power_data.OrderBy(m => m.power_id).ToPagedList(currentPageIndex, 20);
            }
            else
            {
                data = this._DB.sys_power_data.Where(m => m.label.Contains(kw)).OrderBy(m => m.power_id).ToPagedList(currentPageIndex, 20);
            }
            ViewBag.kw = kw;
            ViewBag.jgrowl = TempData["jgrowl"];
            return View(data);
        }

        public ActionResult PowerList_ajax(int? page, string kw)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            IPagedList data;
            if (String.IsNullOrEmpty(kw))
            {
                data = this._DB.sys_power_data.OrderBy(m => m.power_id).ToPagedList(currentPageIndex, 20);
            }
            else
            {
                data = this._DB.sys_power_data.Where(m => m.label.Contains(kw)).OrderBy(m => m.power_id).ToPagedList(currentPageIndex, 20);
            }
            ViewBag.kw = kw;
            return PartialView(RouteData.Values["action"].ToString(), data);
        }


        public ActionResult PowerEdit(int id)
        {

            var data = this._DB.sys_power_data.Find(id);
            if (data != null)
            {
                var controller = this._DB.sys_function_data.Where(m => m.visible).Select(m => m.function_id + "," + m.controll).ToArray();
                var action = this._DB.sys_function_sub.Where(m => m.visible && m.show_visible).Select(m => m.function_sub_id + "," + m.action + "," + m.function_id).ToArray();

                List<ListItem> ListItems = new List<ListItem>() { };

                var b = this._DB.sys_function_data.Where(m => m.controll == data.controller).FirstOrDefault();
                b = b != null ? b : new sys_function_data() { };
                ListItems = new List<ListItem>() { };
                ListItems.AddRange(Gs.sList(controller, b.function_id.ToString(), "請選擇"));
                ViewData["start_id_chained"] = ListItems;

                var a = this._DB.sys_function_sub.Where(m => m.action == data.action && m.function_id == b.function_id).FirstOrDefault();
                a = a != null ? a : new sys_function_sub() { };
                ListItems = new List<ListItem>() { };
                ListItems.AddRange(Gs.sList(action, a.function_sub_id.ToString(), "請選擇"));
                ViewData["start_id"] = ListItems;



            }

            ViewBag.jgrowl = TempData["jgrowl"];

            return View(data);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PowerEditSave(sys_power_data sys_power_data, int[] block_id, int start_id = 0)
        {
            var data = this._DB.sys_power_data.Find(sys_power_data.power_id);
            //var data1 = this._DB.sys_power_data.Where(m => m.zz_id.Equals(sys_power_data.zz_id)).Select(m => m).SingleOrDefault();


            if (data == null)
            {
                ModelState.AddModelError("", "查無此筆資料");
            }
            else
            {
                string controller = "", action = "";
                if (start_id > 0)
                {
                    var a = this._DB.sys_function_sub.Where(m => m.function_sub_id == start_id).FirstOrDefault();
                    a = a != null ? a : new sys_function_sub() { };
                    action = a.action;

                    var b = this._DB.sys_function_data.Where(m => m.function_id == a.function_id).FirstOrDefault();
                    b = b != null ? b : new sys_function_data() { };
                    controller = b.controll;
                }

                //data.power_id = sys_power_data.power_id;
                data.label = sys_power_data.label;
                data.info = sys_power_data.info;
                data.visible = sys_power_data.visible;
                data.controller = controller;
                data.action = action;

                //刪除
                int sc = _DB.Database.ExecuteSqlCommand("DELETE FROM sys_power_sub WHERE power_id = {0}", sys_power_data.power_id);

                //新增
                if (block_id != null)
                {
                    foreach (int function_sub_id in block_id)
                    {
                        var data2 = new sys_power_sub { power_id = sys_power_data.power_id, function_sub_id = function_sub_id };
                        this._DB.sys_power_sub.Add(data2);
                    }

                }

                this._DB.SaveChanges();

            }

            //更新

            TempData["jgrowl"] = "資料更新成功";

            return RedirectToAction(RouteData.Values["action"].ToString().Replace("Save", ""), new { id = sys_power_data.power_id });
        }

        public ActionResult PowerAdd(int id)
        {
            sys_power_data data = new sys_power_data();

            data.visible = true;
            return View(data);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PowerAddSave(sys_power_data sys_power_data, int[] block_id)
        {
            var data = new sys_power_data { };

            if (data == null)
            {
                ModelState.AddModelError("", "查無此筆資料");
            }
            else
            {

                //data.power_id = sys_power_data.power_id;
                data.label = sys_power_data.label;
                data.info = sys_power_data.info;
                data.visible = sys_power_data.visible;
                data.controller = sys_power_data.controller;
                data.action = sys_power_data.action;

                _DB.sys_power_data.Add(data);
                this._DB.SaveChanges();

                sys_power_data.power_id = data.power_id;

                //新增
                foreach (int function_sub_id in block_id)
                {
                    var data2 = new sys_power_sub { power_id = sys_power_data.power_id, function_sub_id = function_sub_id };
                    this._DB.sys_power_sub.Add(data2);
                }
                this._DB.SaveChanges();

            }

            //更新

            TempData["jgrowl"] = "資料新增成功!!";

            return RedirectToAction(RouteData.Values["action"].ToString().Replace("AddSave", "Edit"), new { id = sys_power_data.power_id });
        }

        [HttpPost]
        public ActionResult PowerListDel(int cb_flag, string cb_id)
        {
            string[] jgrowl_msg = new string[] { "刪除", "啟用", "停用" };
            int[] strArray = Array.ConvertAll<string, int>(NW.Split(cb_id, ","), int.Parse);

            int power_id = 0;

            var data = (from xx in _DB.sys_power_data
                        where strArray.Contains(xx.power_id)
                        select xx).ToList();

            foreach (var item in data)
            {
                sys_power_data vmD = _DB.sys_power_data.Find(item.power_id);

                power_id = item.power_id;

                switch (cb_flag)
                {
                    case 1:
                        vmD.visible = true;
                        break;
                    case 2:
                        vmD.visible = false;
                        break;
                    case 0:
                        _DB.sys_power_data.Remove(vmD);



                        break;
                }
                _DB.SaveChanges();
            }

            if (cb_flag == 0)
            {
                //刪除權限設定
                int sc = _DB.Database.ExecuteSqlCommand("DELETE FROM sys_power_sub WHERE power_id = {0}", power_id);
            }

            TempData["jgrowl"] = jgrowl_msg[cb_flag] + "功能，設定成功";

            return RedirectToAction(RouteData.Values["action"].ToString().Replace("Del", ""));
        }

        #endregion

    }
}
