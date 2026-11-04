using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using galaxy_web.Models;
using MvcPaging;
using System.IO;
using ClosedXML.Excel;
using System.Data;
using System.Threading;
using System.Globalization;

namespace galaxy_web.Areas.newweb.Controllers
{
    public class NwProductController : _BaseController
    {
        //
        // GET: /newweb/NwProduct/
        
        public ActionResult List(int? page, int? status, string kw, DateTime? sD, DateTime? eD, int? cid, int type = 0, string nation = "TW", int sort = 0, string p = "")
        {
            List<ListItem> ListItems = new List<ListItem>() { };

            //商品
            var linq = this._DB.product_data.Where(m => m.product_id > 0);

            //.Select(m => new { m.web_name, m.product_id, m.item_name, m.status, m.item_no, m.erp_no, m.company_id, m.create_datetime, m.change_datetime });

            #region"權限判斷"

            var aData = NW.admin(NW.DESDeCode(User.Identity.Name));
            if (aData.power_id > 2)
            {
                linq = linq.Where(m => m.company_id == aData.admin_id);
            }

            ViewBag.aPower = aData.power_id;

            #endregion

            #region"關鍵字"

            if (!string.IsNullOrEmpty(kw))
            {
                ViewBag.kw = kw;
                kw = kw.ToLower();

                linq = linq.Where(m =>
                    m.web_name.ToLower().Contains(kw) ||
                    m.item_name.ToLower().Contains(kw) ||
                    m.item_no.ToLower().Contains(kw) ||
                    m.erp_no.ToLower().Contains(kw)
                    );
            }

            #endregion

            #region"國籍判斷"

            ViewBag.nation = nation;
            nation = nation.ToLower();
            linq = linq.Where(m => m.nation.ToLower() == nation);

            #endregion

            #region"廠商區別"

            if (cid.HasValue)
            {
                ViewBag.cid = cid;
                linq = linq.Where(m => m.company_id == cid);
            }

            #endregion

            #region"平台過濾"


            if (!string.IsNullOrEmpty(p))
            {
                linq = linq.Where(m => m.platform_ids.IndexOf(p) >= 0);
            }

            ViewBag.p = p;

            #endregion

            #region"狀態"

            #region"狀態分類"

            ListItems.Add(new ListItem("", "全部 - " + linq.Count(), !status.HasValue));

            var G = (from a in this._DB.product_status_flag.Where(m => m.visible).OrderBy(m => m.inx).ToList()
                     join b in linq on a.flag_id equals b.status into mm
                     select new ListItem
                         (
                         a.flag_id.ToString(),
                         a.name + " - " + mm.Count(),
                         status.HasValue ? status == a.flag_id ? true : false : false
                         )).ToList();

            ListItems.AddRange(G);

            ViewData["G"] = ListItems;

            #endregion

            //var linq_all = linq.Where(m => m.status != 999);

            if (status.HasValue)
            {
                ViewBag.status = status;

                linq = linq.Where(m => m.status == status);
            }
            else
            {
                linq = linq.Where(m => m.status != 999);

            }

            #endregion

            #region"選單"

            #region"廠商"

            var company_data = this._DB.sys_admin_data.Where(m => m.visible && m.power_id == 3).ToList();

            ListItems = new List<ListItem>() { };

            var Company = company_data.Select(m => m.admin_id + "," + m.name).Distinct().OrderBy(m => m.Length).ToArray();

            string c_n = cid.HasValue ? cid.ToString() : null;
            ListItems.AddRange(Gs.sList(Company, c_n, "全部"));
            ViewData["Company"] = ListItems;

            #endregion

            #endregion

            #region"往前台丟"

            //狀態
            var F = this._DB.product_status_flag.Where(m => m.visible).OrderBy(m => m.inx).ToDictionary(m => m.flag_id, m => m.name);
            ViewData["F"] = F;

            //廠商
            var C = company_data.Where(m => m.power_id == 3).ToDictionary(m => m.admin_id, m => m.name);
            ViewData["C"] = C;

            #endregion


            if (type == 1)
            {

                return Export(linq);
            }

            int currentPageIndex = page.HasValue && page.Value > 0 ? page.Value - 1 : 0;

            //排序
            switch (sort)
            { 
                case 1:
                    //中文排序
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("zh-TW");
                    var company_sorting = company_data.Where(m => m.power_id == 3).Select(m => new { m.admin_id, m.name }).AsEnumerable().OrderBy(m => m.name.Substring(0, 1)).ToArray();
                    var linq2 = new List<product_data> { };
                    foreach (var item in company_sorting)
                    {
                        linq2.AddRange(linq.Where(m => m.company_id == item.admin_id).ToList());
                    }
                    linq = linq2.AsQueryable();

                    break;
                case 2:
                    linq = linq.OrderByDescending(m => (m.price > 0 ? ((m.price - m.cost) / m.price) : 0));
                    break;
                default:
                    linq = linq.OrderByDescending(m => m.change_datetime ?? m.create_datetime);
                    break;
            }
            ViewBag.sort = sort;

            var data = linq.ToPagedList(currentPageIndex, 20);

            #region"產品目前數量"

            var product_stock = this._DB.product_stock.AsNoTracking().Where(m => m.visible).ToList();

            foreach (var item in data)
            {
                item.Stock = product_stock.Where(m => m.product_id == item.product_id).Select(m => m.num).DefaultIfEmpty(0).Sum();
            }

            #endregion

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

        public ActionResult CNList(int? page, int? status, string kw, DateTime? sD, DateTime? eD, int? cid, int type = 0, string nation = "CN")
        {
            return List(page, status, kw, sD, eD, cid, type, nation);
        }

        public ActionResult SampleList(int? page, int? status, string kw, DateTime? sD, DateTime? eD, int? cid, int type = 0, string nation = "TS")
        {
            return List(page, status, kw, sD, eD, cid, type, nation);
        }

        #region"商品"

        public ActionResult Listx(int? page, int? status, string kw, DateTime? sD, DateTime? eD, int? cid, int type = 0)
        {
            List<ListItem> ListItems = new List<ListItem>() { };

            var linq = this._DB.product_data.Where(m => m.product_id > 0);
            var linq_a = this._DB.sys_admin_data.Where(m => m.power_id == 3);

            #region"條件"

            #region"權限判斷"

            var aData = NW.admin(NW.DESDeCode(User.Identity.Name));
            if (aData.power_id > 2)
            {
                linq = linq.Where(m => m.company_id == aData.admin_id);
                linq_a = linq_a.Where(m => m.admin_id == aData.admin_id);
            }

            ViewBag.aPower = aData.power_id;

            #endregion

            #region"廠商區別"

            if (cid.HasValue)
            {
                ViewBag.cid = cid;
                linq = linq.Where(m => m.company_id == cid);
            }

            #endregion

            #region"時間"

            var linq_s = this._DB.product_status.GroupBy(m => m.product_id).Where(m => m.OrderByDescending(x => x.status_id).Select(x => x.create_datetime >= sD && x.create_datetime < eD).FirstOrDefault()).Select(m => m.Key).ToArray();

            //eD = eD.HasValue ? eD : NW.Today;
            //sD = sD.HasValue ? sD : new DateTime(NW.Today.Year, NW.Today.Month, 1);
            if (sD.HasValue)
            {
                ViewBag.sD = NW.getDate(sD);
                linq = linq.Where(m => m.create_datetime >= sD || linq_s.Contains(m.product_id));
            }

            if (eD.HasValue)
            {
                ViewBag.eD = NW.getDate(eD);
                eD = eD.Value.AddDays(1);

                linq = linq.Where(m => m.create_datetime < eD || linq_s.Contains(m.product_id));
            }

            #endregion

            #region"關鍵字"

            if (!string.IsNullOrEmpty(kw))
            {
                ViewBag.kw = kw;
                kw = kw.ToLower();

                linq = linq.Where(m =>
                    m.web_name.ToLower().Contains(kw) ||
                    m.item_name.ToLower().Contains(kw) ||
                    m.item_no.ToLower().Contains(kw) ||
                    m.erp_no.ToLower().Contains(kw)
                    );
            }

            #endregion

            #region"狀態"

            #region"狀態分類"

            ListItems.Add(new ListItem("", "全部 - " + linq.Count(), !status.HasValue));

            var G = (from a in this._DB.product_status_flag.Where(m => m.visible).OrderBy(m => m.inx).ToList()
                     join b in linq on a.flag_id equals b.status into mm
                     select new ListItem
                         (
                         a.flag_id.ToString(),
                         a.name + " - " + mm.Count(),
                         status.HasValue ? status == a.flag_id ? true : false : false
                         )).ToList();

            ListItems.AddRange(G);

            ViewData["G"] = ListItems;

            #endregion

            //var linq_all = linq.Where(m => m.status != 999);

            if (status.HasValue)
            {
                ViewBag.status = status;

                linq = linq.Where(m => m.status == status);
            }
            else
            {
                linq = linq.Where(m => m.status != 999);

            }

            #endregion



            #endregion

            #region"選單"


            #region"廠商"

            ListItems = new List<ListItem>() { };

            var Company = linq_a.Select(m => m.admin_id + "," + m.name).Distinct().ToArray();

            string c_n = cid.HasValue ? cid.ToString() : null;
            ListItems.AddRange(Gs.sList(Company, c_n, "全部"));
            ViewData["Company"] = ListItems;

            #endregion


            #endregion

            #region"往前台丟"

            //狀態
            var F = this._DB.product_status_flag.Where(m => m.visible).OrderBy(m => m.inx).ToDictionary(m => m.flag_id, m => m.name);
            ViewData["F"] = F;

            //廠商
            var C = this._DB.sys_admin_data.Where(m => m.power_id == 3).ToDictionary(m => m.admin_id, m => m.name);
            ViewData["C"] = C;

            #endregion

            if (type == 1)
            {
                return Export(linq);
            }

            int currentPageIndex = page.HasValue && page.Value > 0 ? page.Value - 1 : 0;

            IPagedList data = linq.OrderByDescending(m => m.change_datetime ?? m.create_datetime).ToPagedList(currentPageIndex, 20);

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

        public ActionResult Add(int id, string nation = "TW")
        {
            ViewBag.jgrowl = TempData[NW.ssjgrowl];

            var aData = NW.admin(NW.DESDeCode(User.Identity.Name));

            ViewBag.aPower = aData.power_id;

            var data = this._DB.product_data.Where(m => m.product_id == id).SingleOrDefault();
            if (data == null && id != 0) { TempData[NW.ssjgrowl] = "查無相關資訊"; return RedirectToAction("List"); }

            if (data == null && aData.power_id != 3) { TempData[NW.ssjgrowl] = "無法新增產品，請更換廠商帳號"; return RedirectToAction("List"); }

            int cid = data != null ? data.company_id : aData.admin_id;
            ViewData["aData"] = this._DB.sys_admin_data.Where(m => m.admin_id == cid).SingleOrDefault();

            data = data != null ? data : new product_data() { nation = nation };

            if (data.product_id == 0)
            {
                //data.price = 1;
                //data.cost = 1;           
            }
            else
            {


                #region"修改紀錄"

                var R = this._DB.product_status.Where(m => m.status_id > 0 && m.product_id == data.product_id).OrderByDescending(m => m.status_id).ToList();
                if (R.Any())
                {
                    foreach (var item in R)
                    {
                        if (!string.IsNullOrEmpty(item.create_user))
                        {
                            item.create_user = NW.admin(NW.DESDeCode(item.create_user)).name;
                        }

                    }

                    ViewData["R"] = R;


                }

                //狀態顯示
                var Status_F = this._DB.product_status_flag.ToDictionary(m => m.flag_id, m => m.name);
                ViewData["Status_F"] = Status_F;

                #endregion

                #region"庫存紀錄"

                var S = this._DB.product_stock.Where(m => m.visible && m.product_id == data.product_id).OrderByDescending(m => m.stock_id).ToList();
                if (S.Any())
                {
                    ViewData["S"] = S;
                }

                #endregion

            }

            #region"圖片"

            var Controller = RouteData.Values["controller"].ToString().ToLower().Replace("nw", "");
            data.Img = new List<string>() { };


            var Imgx = this._DB.product_image.Where(m => m.product_id == data.product_id && m.visible).OrderByDescending(m => m.img_id);

            foreach (var item in Enumerable.Range(1, 4))
            {
                var Img = Imgx.Where(m => m.group_id == item).OrderByDescending(m => m.img_id).FirstOrDefault();
                Img = Img != null ? Img : new product_image() { };

                string ImgPath = Gs.Path_Img(Img.img, Controller);

                data.Img.Add(ImgPath);
            }

            #endregion

            #region"選單"

            List<ListItem> ListItems = new List<ListItem>() { };

            #region"分區分類"

            //分類-全
            var Group = this._DB.product_group.Where(m => m.visible).OrderBy(m => m.inx).Select(m => new { m.parent_id, m.group_id, m.name });

            //目前已有
            string[] data_G = data.group_ids != null ? data.group_ids.Split(',') : new string[] { };

            var G = Group.Where(m => m.parent_id != 0).ToLookup(m => data_G.Contains(m.group_id.ToString()));

            var Y = G[true].ToDictionary(m => m.group_id, m => m.name);
            ViewData["group_ids_s"] = Y;

            //選單列表
            var N = G[false].Select(m => m.group_id + "," + m.name + "," + m.parent_id).ToArray();
            ListItems = new List<ListItem>() { };
            ListItems.AddRange(Gs.sList(N, null, "請選擇"));
            ViewData["group_ids"] = ListItems;

            //分區
            string[] Z = Group.Where(m => m.parent_id == 0).Select(m => m.group_id + "," + m.name).ToArray();
            ListItems = new List<ListItem>() { };
            ListItems.AddRange(Gs.sList(Z, null, "請選擇"));
            ViewData["group_ids_chained"] = ListItems;

            #endregion

            #region"狀態"

            //int status = data.status > 0 ? data.status : 1;

            string[] F = this._DB.product_status_flag.Where(m => m.visible).OrderBy(m => m.inx).Select(m => m.flag_id + "," + m.name).ToArray();

            ListItems = new List<ListItem>() { };
            ListItems.AddRange(Gs.sList(F, data.status.ToString()));

            #region"權限判斷"

            if (aData.power_id > 2)
            {
                ListItems = ListItems.Take(2).ToList();
            }

            #endregion

            ViewData["status"] = ListItems;

            #endregion

            #region"廠商"

            ListItems = new List<ListItem>() { };
            var C = this._DB.sys_admin_data.Where(m => m.visible && m.power_id == 3);
            if (aData.power_id > 2)
            {
                C = C.Where(m => m.admin_id == aData.admin_id);
            }

            var CC = C.Select(m => m.admin_id + "," + m.name).ToArray();

            ListItems.AddRange(Gs.sList(CC, data.company_id.ToString()));

            ViewData["company_id"] = ListItems;

            #endregion

            var vv = NW.platforms;
            ListItems = new List<ListItem>() { };
            ListItems.AddRange(Gs.sList(vv, null, "請選擇"));
            ViewData["platform_ids"] = ListItems;

            if (!string.IsNullOrEmpty(data.platform_ids))
            {
                var f = data.platform_ids.Split(',').Where(m => !string.IsNullOrEmpty(m)).ToArray();
                ViewData["platform_ids_s"] = f.ToDictionary(m => Convert.ToInt32(m), m => NW.platform(Convert.ToInt32(m)));

                ListItems = new List<ListItem>() { };
                ListItems.AddRange(Gs.sList(vv.Where(m => !f.Contains(m.Split(',')[0])).ToArray(), null, "請選擇"));
                ViewData["platform_ids"] = ListItems;
            }

            #region"分類"

            int company_id = data.company_id != 0 ? data.company_id : aData.admin_id;

            Dictionary<int, string> gData = this._DB.product_erp_group.Where(m => (nation != "TS" ?  m.nation.ToLower() == nation.ToLower() : true)).OrderBy(m => m.pre).ToDictionary(m => m.group_id, m => m.pre + "." + m.name);

            int[] g = gData.Select(m => m.Key).ToArray();
            string[] sData = this._DB.product_erp_sub.Where(m => m.admin_id == company_id && m.visible && g.Contains(m.group_id)).OrderBy(m => m.group_id).ToList().Select(m => m.sub_id + "," + gData[m.group_id]).ToArray();

            //string[] gData = this._DB.product_erp_group.Where(m => sData.Contains(m.group_id) && m.visible).Select(m => m.group_id + "," + m.pre + "." + m.name).ToArray();

            ListItems = new List<ListItem>() { };
            ListItems.AddRange(Gs.sList(sData, data.sub_id.ToString(), "請選擇"));
            ViewData["sub_id"] = ListItems;
            ViewBag.sub = ListItems.Where(m => m.IsChecked == true).Select(m => m.DisplayText).FirstOrDefault();

            #endregion

            //檔期預設
            if (!data.start_date.HasValue)
            {
                data.start_date = DateTime.Today;
            }
            if (!data.end_date.HasValue)
            {
                data.end_date = DateTime.Today.AddDays(7);
            }

            #endregion

            return View(data);


        }

        public ActionResult Edit(int id)
        {
            return Add(id, "TW");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EditSave(product_data Get, int? stock_num, string stock_info, string group_ids_s, string platform_ids_s, HttpPostedFileBase[] Img)
        {

            string msg = string.Empty;

            var aData = NW.admin(NW.DESDeCode(User.Identity.Name));

            var data = this._DB.product_data.Where(m => m.product_id == Get.product_id).SingleOrDefault();
            if (data == null && Get.product_id != 0) { TempData["jgrowl"] = "查無相關資訊"; return RedirectToAction("List"); }

            data = data != null ? data : new product_data() { };

            //先記錄起來
            int status_old = data.status;

            //同步平台數
            var sync_num = new string[] { };

            while (true)
            {

                if (string.IsNullOrEmpty(Get.item_name) && string.IsNullOrEmpty(data.item_name)) { msg = "請填寫品名"; break; }
                //if (string.IsNullOrEmpty(Get.item_no)) { msg = "請填寫訂單單號"; break; }

                //貨號判斷
                bool isItem_no = this._DB.product_data.Any(m => m.item_no == Get.item_no && m.company_id == Get.company_id && m.product_id != Get.product_id);
                if (isItem_no) { msg = "此貨號已經使用過，無法新增，請更換"; break; }

                #region"ERP_貨號"

                //ERP貨號

                if (string.IsNullOrEmpty(data.erp_no))
                {
                    var Sub = this._DB.product_erp_sub.Where(m => m.sub_id == Get.sub_id).SingleOrDefault();
                    if (Sub == null) { msg = "分類解析錯誤，請洽詢管理員"; break; }

                    var G = this._DB.product_erp_group.Where(m => m.group_id == Sub.group_id).SingleOrDefault();
                    if (G == null) { msg = "分類解析錯誤，請洽詢管理員"; break; }

                    string Sn = G.pre + Sub.auto_sn;

                    var Max = this._DB.product_data.Where(m => m.erp_no.StartsWith(Sn)).Select(m => m.erp_no).Max();
                    int Number = Max != null ? Convert.ToInt32(Max.Split(new string[] { Sn }, StringSplitOptions.None)[1]) + 1 : 1;

                    Get.erp_no = Sn + Number.ToString().PadLeft(4, '0');
                }
                else
                {
                    Get.erp_no = data.erp_no;
                }

                #endregion


                if (aData.power_id > 2)
                {
                    #region 判斷廠商等級限制上架商品數

                    //新增才需判斷
                    if (Get.product_id == 0)
                    {
                        //取得該登入廠商的等級
                        var level = this._DB.sys_admin_group.Where(m => m.group_id == aData.group_id && m.visible).FirstOrDefault();
                        if (level == null)
                        {
                            msg = "廠商等級有誤，請先選擇其他等級"; break;
                        }
                        else
                        {
                            //該廠商所有非作廢商品
                            var pro_count = this._DB.product_data.Where(m => m.company_id == Get.company_id && m.status != 999).Count();
                            //因為接下來會新增所以要大於等於 才不會造成已經有10個過了以下判斷新增變成11個
                            if (pro_count >= level.limit)
                            {
                                msg = "該等級廠商商品已達上傳上限" + level.limit + "個，請作廢其他商品，以便新增或修改"; break;
                            }
                        }
                    }

                    #endregion

                    #region 有上傳營業登記證才可上架商品

                    //this._DB.sys_admin_image.Where(m => m.admin_id == Get.company_id && new int[] {1,}m.group_id)

                    #endregion

                    #region"已經送審-(需事先置換的)"

                    if (data.status > 0)
                    {
                        Get.sprice = data.sprice;
                        Get.price = data.price;
                        Get.cost = data.cost;
                        Get.erp_cost = data.erp_cost;
                        Get.erp_no = data.erp_no;

                        Get.item_no = data.item_no;
                        Get.item_name = data.item_name;
                        Get.web_name = data.web_name;

                        Get.company_id = data.company_id;

                        Get.status = data.status;

                    }

                    #endregion


                    //判斷是否真的是供應商自己
                    var Com = this._DB.sys_admin_data.Where(m => m.admin_id == Get.company_id).SingleOrDefault();
                    if (Com == null || Com.admin_id != aData.admin_id)
                    {
                        Get.company_id = 0;
                        msg = "請確認是否為廠商"; break;
                    }

                    //供應商無法上架與下架
                    //if (Get.status > 2) { msg = "權限不足，請與管理員聯繫"; break; }
                }
                //else
                //{
                //    data.erp_cost = Get.erp_cost;
                //    data.erp_no = Get.erp_no;
                //}

                #region"新增與修改"

                data.company_id = Get.company_id;


                //data.group_ids = Get.group_ids;
                if (data.product_id == 0 || aData.power_id < 3)
                {
                    data.web_name = Get.web_name;
                    data.item_name = Get.item_name;
                    
                    if (!string.IsNullOrEmpty(platform_ids_s))
                    {
                        platform_ids_s = platform_ids_s.TrimStart(',');
                    }

                    data.platform_ids = platform_ids_s;
                    if (!string.IsNullOrEmpty(data.platform_ids))
                    {
                        sync_num = platform_ids_s.Split(',');
                    }
                }
                data.item_invoice_name = Get.item_invoice_name;
                //data.web_name = Get.web_name;
                //data.item_name = Get.item_name;

                //if (Get.product_id == 0 || aData.power_id < 3)
                //{
                //    data.item_invoice_name = Get.item_invoice_name;
                //}
                data.item_no = !string.IsNullOrEmpty(Get.item_no) ? Get.item_no : Get.erp_no;


                if (Get.cost < 1)
                {
                    msg = "成本不得為0"; break;
                }

                data.cost = Get.cost;
                data.freight = Get.freight;
                //data.sprice = Get.sprice;
                //data.price = Get.price;
                //if (aData.power_id == 3)
                //{
                //    data.sprice = Get.sprice;
                //    data.price = data.cost * 2;
                //}
                //else if (aData.power_id < 3)
                //{
                data.sprice = Get.sprice;
                data.price = Get.price;
                //}

                ////管理者才能存
                //if (aData.power_id < 3)
                //{
                //    data.price = Get.price;
                //}

                if (Get.erp_cost > 0)
                {
                    data.erp_cost = Get.erp_cost;
                }
                else
                {
                    //decimal.Ceiling((Get.price * 0.15m) + Get.cost + Get.freight);
                    data.erp_cost = Math.Ceiling(((Get.price * 1.1m) + Get.cost) * 0.5m);
                    //data.erp_cost = decimal.Ceiling(((Get.price + Get.freight) * decimal.Parse("0.4")) + (Get.cost + Get.freight) * decimal.Parse("0.6"));
                }

                //data.erp_cost = decimal.Ceiling((Get.price - (Get.cost + Get.freight)) * NW.erp_sp + (Get.cost + Get.freight));


                data.info = Get.info;
                //預購
                data.preorder_visible = Get.preorder_visible;
                try
                {
                    data.introduce = Get.introduce.Replace("http://www.youtube.com/", "https://www.youtube.com/");
                }
                catch
                {

                }

                data.place = Get.place;
                data.unit = Get.unit;
                data.specification = Get.specification;
                data.inx = Get.inx;
                data.style = Get.style;

                data.sub_id = data.sub_id != 0 ? data.sub_id : Get.sub_id;

                //試用品才儲存檔期
                if (Get.nation == "TS")
                {
                    data.start_date = Get.start_date;
                    data.end_date = Get.end_date;
                }

                if (data.product_id == 0)
                {
                    data.erp_no = Get.erp_no;
                    data.nation = Get.nation;

                    data.create_user = User.Identity.Name;
                    data.create_datetime = DateTime.Now;

                    this._DB.product_data.Add(data);

                    msg = "新增成功";
                }
                else
                {
                    data.change_user = User.Identity.Name;
                    data.change_datetime = DateTime.Now;

                    msg = "修改完成";
                }

                //審核完成
                if (data.status == 2 && string.IsNullOrEmpty(data.check_user))
                {
                    data.check_user = User.Identity.Name;
                    data.check_datetime = DateTime.Now;
                }

                this._DB.SaveChanges();

                #endregion

                #region"銷售數量變更"

                if (stock_num.HasValue && stock_num.Value != 0)
                {
                    try
                    {
                        int num_old = this._DB.product_stock.Where(m => m.product_id == data.product_id && m.visible).Select(m => m.num).DefaultIfEmpty(0).Sum();

                        var S = new product_stock() { };
                        S.product_id = data.product_id;
                        S.num = stock_num.Value;
                        S.num_old = num_old;
                        S.info = stock_info;
                        S.visible = true;
                        S.create_user = User.Identity.Name;
                        S.create_datetime = NW.Now;

                        this._DB.product_stock.Add(S);
                        this._DB.SaveChanges();
                    }
                    catch
                    {
                        msg = "銷售紀錄錯誤，故新增失敗";
                        break;
                    }
                }

                #endregion

                #region"狀態變更"

                if (Get.status == 3 && string.IsNullOrEmpty(data.erp_no)) { msg = "尚未取得ERP的認證，故無法上架"; break; }
                else
                {
                    //狀態-另外拉出來做
                    data.status = Get.status;

                    if (Get.status != status_old)
                    {
                        try
                        {
                            //如果是從草稿變送審  順便紀錄商城售價

                            var r = new product_status()
                            {
                                product_id = data.product_id,
                                status = Get.status,
                                status_old = status_old,
                                info = Get.status_info + (status_old == 0 && Get.status == 1 ? String.Format("(商城售價紀錄:{0})", data.sprice.ToString("N1")) : ""),
                                create_user = User.Identity.Name,
                                create_datetime = NW.Now
                            };

                            this._DB.product_status.Add(r);

                        }
                        catch
                        {
                            msg = "狀態變更紀錄有誤，請聯絡管理者";
                            break;
                        }

                        this._DB.SaveChanges();

                    }
                }

                #endregion

                #region"圖片"

                foreach (var item in Img)
                {
                    var mm = "圖片格式不符，請重新上傳";
                    int num = Array.IndexOf(Img, item) + 1;

                    try
                    {

                        //判斷圖片類型

                        if (item != null && item.ContentLength > 0 && NW.getFileType(item, 1)) //有無上傳
                        {

                            var Controller = RouteData.Values["controller"].ToString().ToLower().Replace("nw", "");

                            string tmpPath = Server.MapPath("~/Content/images/") + Controller; //存放路徑
                            if (!Directory.Exists(tmpPath)) { Directory.CreateDirectory(tmpPath); } //建資料夾

                            string I_name = "";

                            #region"先尋找過去的檔案，移走，並取代他"

                            var ImgO = this._DB.product_image.Where(m => m.product_id == data.product_id && m.visible && m.group_id == num).OrderByDescending(m => m.img_id).FirstOrDefault();
                            if (ImgO != null)
                            {
                                I_name = ImgO.img;

                                string ImgO_tmpPath = Gs.Path_Img(ImgO.img, Controller);
                                ImgO_tmpPath = Server.MapPath(ImgO_tmpPath);

                                ImgO_tmpPath = ImgO_tmpPath.IndexOf("no_photo") > -1 ? "" : ImgO_tmpPath;

                                //找到圖檔
                                if (System.IO.File.Exists(ImgO_tmpPath))
                                {

                                    //新路徑
                                    string tempPath_new = tmpPath + "/Old";
                                    if (!Directory.Exists(tempPath_new)) { Directory.CreateDirectory(tempPath_new); } //建資料夾

                                    //新的位置
                                    tempPath_new = tempPath_new + "/" + NW.Now.ToString("yyMMddHHmmss") + "-" + I_name;

                                    //移動並更名
                                    System.IO.File.Move(ImgO_tmpPath, tempPath_new);
                                }
                            }

                            #endregion

                            #region"圖片轉換並儲存"

                            try
                            {
                                //圖片、存放位置、高、寬、自己想要的命名(可不填)
                                var Rebmp = new Gs._Rebmp { Img = item, tmpPath = tmpPath, resizedWidth = 500, name = I_name };    //內頁主圖大小 
                                mm = Gs.Rebmp(Rebmp);
                            }
                            catch
                            {
                                msg += "第" + num + "張圖片上傳失敗，請洽詢管理員";
                                break;
                            }

                            //如果有找到舊資料
                            if (ImgO != null)
                            {
                                ImgO.change_user = User.Identity.Name;
                                ImgO.change_datetime = NW.Now;
                            }
                            else
                            {
                                var img_data = new product_image()
                                {
                                    img = mm,
                                    group_id = num,
                                    product_id = data.product_id,
                                    visible = true,
                                    create_user = User.Identity.Name,
                                    create_datetime = NW.Now
                                };

                                this._DB.product_image.Add(img_data);
                            }

                            this._DB.SaveChanges();

                            #endregion

                        }
                    }
                    catch (Exception ex)
                    {
                        msg += "第" + num + "張圖片，" + ex;
                    }
                }


                #endregion



                break;
            }

            data.sync_flag = sync_num.Length;
            this._DB.SaveChanges();


            TempData["jgrowl"] = msg;
            return RedirectToAction(RouteData.Values["action"].ToString().Replace("Add", "Edit").Replace("Save", ""), new { id = data.product_id });
        }

        [HttpPost]
        public ActionResult ListDel(int cb_flag, string cb_id)
        {
            Dictionary<int, string> jgrowl_msg = new Dictionary<int, string>() { { 0, "退回草稿區" }, { 3, "上架" }, { 4, "下架" }, { 999, "作廢" } };
            //string[] jgrowl_msg = new string[] { "退回草稿區", "上架", "下架", "作廢" };
            //int[] strArray = Array.ConvertAll<string, int>(NW.Split(cb_id, ","), int.Parse);

            var n = new string[] { };

            var pro_data = this._DB.product_data.AsQueryable();

            //如果是設定同步功能
            if (!string.IsNullOrEmpty(NW.platform(cb_flag)) && !string.IsNullOrEmpty(cb_id))
            {
                var d = cb_id.Split(',');
                var list = this._DB.product_data.Where(m => d.Contains(m.product_id.ToString())).ToList();
                foreach (var item in list)
                {
                    if (item.platform_ids != null)
                    {
                        var f = item.platform_ids.Split(',');
                        if (!f.Contains(cb_flag.ToString()))
                        {
                            //沒有就新增
                            item.platform_ids = item.platform_ids + "," + cb_flag.ToString();
                        }
                        else
                        {
                            //有就取消
                            item.platform_ids = String.Join(",", f.Except(new string[] { cb_flag.ToString() }));
                        }

                        if (!string.IsNullOrEmpty(item.platform_ids))
                        {
                            var c = item.platform_ids.Split(',');
                            item.sync_flag = c.Length;
                        }
                    }
                    else
                    {
                        //空資料
                        item.platform_ids = cb_flag.ToString();
                        item.sync_flag = 1;
                    }
                }

                this._DB.SaveChanges();

                TempData["jgrowl"] = "平台設定功能已完成";
                //跳回上架區
                return RedirectToAction(RouteData.Values["action"].ToString().Replace("Del", ""), new { status = 3 });

            }



            string sql = "";
            switch (cb_flag)
            {
                case 0:
                    sql = "Update product_data set status = 0 Where product_id in (" + cb_id + ")";
                    break;

                case 3:
                    sql = "Update product_data set status = 3 Where product_id in (" + cb_id + ")";
                    break;

                case 4:
                    sql = "Update product_data set status = 4 Where product_id in (" + cb_id + ")";
                    break;


                case 999: //停用
                    sql = "Update product_data set status = 999 Where product_id in (" + cb_id + ")";

                    break;

            }

            if (sql != "")
            {
                _DB.Database.ExecuteSqlCommand(sql);

                //更新需同步狀態
                _DB.Database.ExecuteSqlCommand("Update product_data set sync_flag = 1 Where product_id in (" + cb_id + ")");
            }



            foreach (var item in cb_id.Split(','))
            {
                int id = int.Parse(item);

                var  p = pro_data.Where(m => m.product_id == id).FirstOrDefault();

                if (p != null)
                {
                    if (!string.IsNullOrEmpty(p.platform_ids))
                    {
                        var d = p.platform_ids.Split(',');
                        p.sync_flag = d.Length;
                    }
                }

                var R = new product_status()
                {
                    product_id = int.Parse(item),
                    status = cb_flag,
                    status_old = this._DB.product_status.Where(m => m.product_id == id).OrderByDescending(m => m.status_id).Select(m => m.status).FirstOrDefault(),
                    create_user = User.Identity.Name,
                    create_datetime = NW.Now
                };

                this._DB.product_status.Add(R);
            }
            this._DB.SaveChanges();

            TempData["jgrowl"] = jgrowl_msg[cb_flag] + "功能，設定成功";
            return RedirectToAction(RouteData.Values["action"].ToString().Replace("Del", ""), new { status = cb_flag });
        }

        #region"匯出"

        public ActionResult Export(IEnumerable<object> objects)
        {
            //string[] Field = { "品號", "品名", "規格", "貨號", "單位", "庫存數量", "單位成本", " 商品分類一", "商品分類二", "主要庫別", "庫別名稱", "採購人員"
            //                     , "主供應商", "主供應商名稱", "標準進價", "標準售價", "售價定價一", "售價定價二", "單位標準材料成本@運費", "售價定價一含稅", "售價定價二含稅" };

            var data = (from a in objects as IEnumerable<product_data>
                        join b in this._DB.sys_admin_data on a.company_id equals b.admin_id
                        join c in this._DB.product_stock on a.product_id equals c.product_id into mm
                        join d in this._DB.product_erp_sub on a.sub_id equals d.sub_id
                        join e in this._DB.product_erp_group on d.group_id equals e.group_id
                        join f in this._DB.product_image.Where(m => m.visible).OrderBy(m => m.group_id) on a.product_id equals f.product_id into mm2
                        select new
                        {
                            品號 = a.erp_no,
                            品名 = (!string.IsNullOrEmpty(a.web_name) ? "【" + a.web_name + "】" : null) + a.item_name,
                            規格 = a.specification,
                            貨號 = a.item_no,
                            單位 = a.unit,
                            庫存數量 = mm.Select(m => m.num).DefaultIfEmpty(0).Sum(),
                            單位成本 = 0,
                            商品分類一 = e.pre,
                            商品分類二 = "",
                            主要庫別 = "TU001",
                            庫別名稱 = "騰云生活庫",
                            採購人員 = "",
                            主供應商 = b.erp_sn,
                            主供應商名稱 = b.name,
                            標準進價 = a.cost.ToString(NW.point),
                            標準售價 = a.price.ToString(NW.point),
                            售價定價一 = a.sprice.ToString(NW.point),
                            售價定價二 = a.erp_cost.ToString(NW.point),
                            運費 = a.freight.ToString(NW.point),
                            售價定價一含稅 = "Y",
                            售價定價二含稅 = "",
                            圖片 = string.Join(",", (mm2.Select(m => Gs.Path_Img(m.img, "product", NW.WEB_URL)).ToArray())),
                            商品簡介 = a.info,
                            商品介紹 = a.introduce,
                            國籍 = a.nation
                        });

            XLWorkbook workbook = NewExport(data);

            MemoryStream ms = new MemoryStream();
            workbook.SaveAs(ms);
            workbook.Dispose();

            return File(ms.ToArray(), "application/vnd.ms-excel", DateTime.Today.ToString("yyyyMMdd") + "匯出商品.xlsx");


            //datatable版 字串超過excel寬度會開不起來
            Dictionary<string, DataTable> dt = new Dictionary<string, DataTable>() { { "商品資訊", Gs.ConvertObjectsToDataTable(data) } };

            string fileName = string.Format("{0}{1}.xlsx", NW.Now.ToString("yyyyMMddHHmmss"), NW.GetUniqueKey());


            var r = Gs.ExportExcelFromDataTable(dt);
            return File(r.Value.ToArray(), "application/vnd.ms-excel", fileName);

            //if (!string.IsNullOrEmpty(r.Key))
            //{
            //    TempData[NW.ssjgrowl] = r.Key;
            //    return RedirectToAction("List");

            //}
            //else
            //{
            //    return File(r.Value.ToArray(), "application/vnd.ms-excel", fileName);
            //}


        }

        public XLWorkbook NewExport(IEnumerable<object> T)
        {

            XLWorkbook workbook = new XLWorkbook();

            string[] cols = "".Split(',');

            cols = "訂單編號,品號,商品名稱,會員編號或推薦人編號,訂購人,訂購日期,出貨日期,付款方式,訂購金額,進貨成本,購買數量,銀河系成本,ERP成本,回饋紅利,回饋pv,商品狀態,商品廠商,發票號碼".Split(',');

            var sheet = workbook.Worksheets.Add(DateTime.Today.ToString("yyyyMMdd") + "商品匯出");

            int colIdx = 1;
            int rowIdx = 2;

            string[] Field = { "品號", "品名", "規格", "貨號", "單位", "庫存數量", "單位成本", " 商品分類一", "商品分類二", "主要庫別", "庫別名稱", "採購人員"
                                 , "主供應商", "主供應商名稱", "標準進價", "標準售價", "售價定價一", "售價定價二", "運費", "售價定價一含稅", "售價定價二含稅", "圖片" ,"商品簡介", "商品介紹", "國籍" };

            foreach (string colName in Field)
            {
                sheet.Cell(1, colIdx).Style.Fill.BackgroundColor = XLColor.Red;
                sheet.Cell(1, colIdx++).Value = colName;
            }


            foreach (var item in T)
            {
                try
                {
                    //映射
                    var row = item.GetType().GetProperties();
                    int count = 1;
                    foreach (var cell in row)
                    {

                        sheet.Cell(rowIdx, count).Value = cell.GetValue(item, null);
                        count += 1;

                    }

                    rowIdx++;

                }
                catch
                {

                }
            }

            for (int i = 1; i <= Field.Length; i++)
            {
                if (i != 22 && i != 23 && i != 24)
                {
                    sheet.Column(i).AdjustToContents();
                }
            }

            return workbook;
        }

        #endregion

        #region"預覽"

        public ActionResult Exhibition(int id)
        {
            string msg = "";

            var data = this._DB.product_data.Where(m => m.product_id == id).SingleOrDefault();

            while (true)
            {
                if (data == null) { msg = "查無相關資訊"; break; }


                var aData = NW.admin(NW.DESDeCode(User.Identity.Name));
                if (aData.power_id > 2) { if (aData.admin_id != data.company_id) { msg = "不是您的商品"; break; } }

                //庫存量
                var S = this._DB.product_stock.Where(m => m.product_id == data.product_id && m.visible).Select(m => m.num).DefaultIfEmpty(0).Sum();
                data.Stock = S;

                //圖片
                data.Img = new List<string>() { };
                var Img = this._DB.product_image.Where(m => m.product_id == data.product_id && m.visible).OrderBy(m => m.group_id).ToList();
                foreach (var item in Img)
                {
                    data.Img.Add(Gs.Path_Img(item.img, "product"));
                }

                break;
            }

            if (!string.IsNullOrEmpty(msg))
            {
                TempData[NW.ssjgrowl] = msg;

                return RedirectToAction("List");
            }
            else
            {
                return View(data);
            }
        }

        #endregion

        #endregion

        #region"ERP-分類"

        public ActionResult GroupList(string nation)
        {

            ViewBag.jgrowl = TempData[NW.ssjgrowl];

            List<ListItem> ListItems = new List<ListItem>() { };

            var linq = this._DB.product_erp_group.Where(m => m.group_id > 0);

            #region"國籍"

            //ListItems = new List<ListItem>() { };
            //ListItems.Add(new ListItem("", "全部：國籍", false));
            //ListItems.AddRange(NW.Nation.Select(m => new ListItem(m.Key, m.Value, m.Key == nation)));
            //ViewData["nation"] = ListItems;

            if (!string.IsNullOrEmpty(nation))
            {
                ViewBag.nation = nation;
                nation = nation.ToLower();
                linq = linq.Where(m => m.nation.ToLower() == nation);
            }



            #endregion



            var data = linq.OrderBy(m => m.inx).ToList();

            return View(data);
        }

        public ActionResult GroupAdd(int id)
        {

            ViewBag.jgrowl = TempData[NW.ssjgrowl];

            string msg = string.Empty;
            string action = RouteData.Values["action"].ToString();

            var data = this._DB.product_erp_group.Where(m => m.group_id == id).SingleOrDefault();
            if (data == null && id != 0) { msg = "查無相關資訊"; return RedirectToAction(action.Replace("Add", "List").Replace("Edit", "List")); }

            data = data != null ? data : new product_erp_group() { };

            if (data.group_id == 0)
            {
                data.inx = 999;
                data.visible = true;
            }

            List<ListItem> ListItems = new List<ListItem>() { };
            ListItems = NW.Nation.Select(m => new ListItem(m.Key, m.Value, m.Key == data.nation)).ToList();
            ViewData["nation"] = ListItems;

            return View(data);
        }

        public ActionResult GroupEdit(int id)
        {
            return GroupAdd(id);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GroupEditSave(product_erp_group Get)
        {
            string msg = string.Empty;
            string action = RouteData.Values["action"].ToString().Replace("Save", "");

            var data = this._DB.product_erp_group.Where(m => m.group_id == Get.group_id).SingleOrDefault();
            if (data == null && Get.group_id != 0)
            {
                TempData[NW.ssjgrowl] = "查無相關資訊";
                return RedirectToAction(action.Replace("Add", "List").Replace("Edit", "List"));
            }

            while (true)
            {
                data = data != null ? data : new product_erp_group() { };

                data.name = Get.name;

                data.nation = Get.nation;
                data.inx = Get.inx;
                data.visible = Get.visible;

                if (data.group_id == 0)
                {
                    //代號
                    if (string.IsNullOrEmpty(Get.pre)) { msg = "代號必填"; break; }
                    bool haveP = this._DB.product_erp_group.Any(m => m.pre == Get.pre);
                    if (haveP) { msg = "已有相關代號"; break; }
                    data.pre = Get.pre;

                    data.create_user = User.Identity.Name;
                    data.create_datetime = NW.Now;

                    this._DB.product_erp_group.Add(data);

                    msg = "新增成功";
                }
                else
                {
                    data.change_user = User.Identity.Name;
                    data.change_datetime = NW.Now;

                    msg = "修改成功";
                }

                this._DB.SaveChanges();

                break;
            }


            TempData[NW.ssjgrowl] = msg;
            return RedirectToAction(action.Replace("Add", "Edit"), new { id = data.group_id });

        }


        #endregion

        #region"ERP-分類"

        public ActionResult SubList(int id, string nation)
        {
            ViewBag.jgrowl = TempData[NW.ssjgrowl];

            var aData = this._DB.sys_admin_data.Where(m => m.admin_id == id && m.power_id == 3).SingleOrDefault();
            if (aData == null && id != 0) { TempData[NW.ssjgrowl] = "查無相關資訊"; return RedirectToAction("List", "NwCompany"); }



            var linq = this._DB.product_erp_group.Where(m => m.group_id > 0);

            #region"國籍"

            if (!string.IsNullOrEmpty(nation))
            {
                ViewBag.nation = nation;
                linq = linq.Where(m => m.nation.ToLower() == nation.ToLower());
            }

            #endregion

            var data = linq.OrderBy(m => m.pre).ToList();
            foreach (var item in data)
            {
                var S = this._DB.product_erp_sub.Where(m => m.admin_id == aData.admin_id && m.group_id == item.group_id).SingleOrDefault();
                S = S != null ? S : new product_erp_sub() { };
                item.Sub_sn = S.auto_sn;
                item.Sub_visible = S.visible;

                //最大編號
                var L = this._DB.product_erp_sub.Where(m => m.group_id == item.group_id).Select(m => m.auto_sn).Max();
                item.change_user = L;
            }



            ViewBag.c_name = aData.name;
            ViewBag.cid = aData.admin_id;
            return View(data);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SubEditSave(int gid, int cid, string sn, bool visible, string nation)
        {
            string msg = string.Empty;

            var aData = this._DB.sys_admin_data.Where(m => m.admin_id == cid && m.power_id == 3).SingleOrDefault();
            if (aData == null && cid != 0) { TempData[NW.ssjgrowl] = "查無相關資訊"; return RedirectToAction("List", "NwCompany"); }

            while (true)
            {
                //分類
                var G = this._DB.product_erp_group.Where(m => m.group_id == gid).SingleOrDefault();
                if (G == null) { msg = "查無相關分類"; break; }

                var isG = this._DB.product_erp_sub.Any(m => m.group_id == G.group_id && m.auto_sn == sn);
                if (isG) { msg = G.name + "，已有編號，請更換"; break; }


                var S = this._DB.product_erp_sub.Where(m => m.group_id == G.group_id && m.admin_id == cid).SingleOrDefault();
                if (S == null && string.IsNullOrEmpty(sn)) { msg = "請輸入所需編號"; break; }
                //if (S != null && S.auto_sn != sn) { msg = G.name +  "，已有編號，無法更改"; break; }

                S = S != null ? S : new product_erp_sub() { };

                S.group_id = G.group_id;
                S.admin_id = aData.admin_id;
                S.visible = visible;

                if (S.sub_id == 0)
                {
                    if (sn.Length != 3) { msg = "編號只能輸入3碼，請更換"; break; }
                    S.auto_sn = sn;
                    S.create_user = User.Identity.Name;
                    S.create_datetime = NW.Now;

                    this._DB.product_erp_sub.Add(S);

                    msg = "新增" + G.name + "完成";
                }
                else
                {
                    S.change_user = User.Identity.Name;
                    S.change_datetime = NW.Now;

                    msg = "修改" + G.name + "完成";
                }

                this._DB.SaveChanges();



                break;
            }

            TempData[NW.ssjgrowl] = msg;
            return RedirectToAction("SubList", new { id = aData.admin_id, nation = nation });
        }

        #endregion


        #region"匯入"

        //[HttpPost]
        //public ActionResult Import(HttpPostedFileBase Im)
        //{

        //    string msg = "";
        //    while (true)
        //    {
        //        //登入者
        //        var aData = NW.admin(NW.DESDeCode(User.Identity.Name));
        //        //if (aData.power_id != 3) { msg = "權限不足，無法新增商品"; break; }

        //        try
        //        {
        //            var go = new Gs._getImport()
        //            {
        //                Im = Im,
        //                isField = new string[] { "web_name", "item_name", "item_no", "cost", "price", "stock", "info", "introduce", "place", "unit" },
        //                isCheck = new Dictionary<int, int[]>() { { 1, new int[] { 7, 8 } }, { 2, new int[] { 6 } }, { 3, new int[] { 4, 5 } } }
        //            };

        //            //接收
        //            var Get = Gs.getImport(go);

        //            //有錯誤訊息
        //            if (!string.IsNullOrEmpty(Get.Key)) { msg = Get.Key; break; }

        //            List<product_data> Products = new List<product_data>() { };
        //            var Rows = Get.Value;
        //            foreach (var item in Rows)
        //            {
        //                //廠商資料
        //                int company_id = aData.admin_id;

        //                //商品貨號
        //                string item_no = item.Where(m => m.Key == 3).Select(m => m.Value).FirstOrDefault();
        //                if (string.IsNullOrEmpty(item_no)) { msg = "商品貨號必填"; break; };

        //                #region"新增或修改"

        //                var P = this._DB.product_data.Where(m => m.item_no == item_no && m.company_id == company_id).FirstOrDefault();
        //                P = P != null ? P : new product_data() { };

        //                P.company_id = company_id;
        //                P.web_name = item.Where(m => m.Key == 1).Select(m => m.Value).FirstOrDefault();
        //                P.item_name = item.Where(m => m.Key == 2).Select(m => m.Value).FirstOrDefault();
        //                P.item_no = item.Where(m => m.Key == 3).Select(m => m.Value).FirstOrDefault();
        //                P.cost = item.Where(m => m.Key == 4).Select(m => decimal.Parse(m.Value)).FirstOrDefault();
        //                P.price = item.Where(m => m.Key == 5).Select(m => decimal.Parse(m.Value)).FirstOrDefault();
        //                P.info = item.Where(m => m.Key == 7).Select(m => m.Value).FirstOrDefault();
        //                P.introduce = item.Where(m => m.Key == 8).Select(m => m.Value).FirstOrDefault();
        //                P.place = item.Where(m => m.Key == 9).Select(m => m.Value).FirstOrDefault();
        //                P.unit = item.Where(m => m.Key == 10).Select(m => m.Value).FirstOrDefault();

        //                P.status = 1; //送審

        //                if (P.product_id == 0)
        //                {
        //                    P.create_user = User.Identity.Name;
        //                    P.create_datetime = NW.Now;

        //                    this._DB.product_data.Add(P);
        //                }
        //                else
        //                {
        //                    P.change_user = User.Identity.Name;
        //                    P.change_datetime = NW.Now;
        //                }

        //                Products.Add(P);

        //                #endregion

        //                //確認資料是否都有填
        //                if (
        //                    string.IsNullOrEmpty(P.item_no)
        //                    || string.IsNullOrEmpty(P.web_name)
        //                    || string.IsNullOrEmpty(P.item_name)
        //                    || P.cost < 1 || P.price < 1
        //                    )
        //                { msg = "商品貨號、品牌、品名皆為必填，成本與建議售價皆須 > 1"; break; }
        //            }

        //            //有錯誤就跳掉
        //            if (!string.IsNullOrEmpty(msg)) { break; }

        //            //儲存
        //            this._DB.SaveChanges();

        //            #region"新增庫存、狀態紀錄"

        //            foreach (var item in Rows)
        //            {

        //                string item_no = item.Where(m => m.Key == 3).Select(m => m.Value).FirstOrDefault();
        //                int company_id = aData.admin_id;
        //                int product_id = Products.Where(m => m.item_no == item_no && m.company_id == company_id).FirstOrDefault().product_id;

        //                #region"庫存"

        //                int num_old = this._DB.product_stock.Where(m => m.product_id == product_id && m.visible).Select(m => m.num).DefaultIfEmpty(0).Sum();

        //                product_stock product_stock = new product_stock()
        //                {
        //                    product_id = product_id,
        //                    num = item.Where(m => m.Key == 6).Select(m => int.Parse(m.Value)).FirstOrDefault(),
        //                    num_old = num_old,
        //                    visible = true,
        //                    create_user = User.Identity.Name,
        //                    create_datetime = NW.Now
        //                };

        //                this._DB.product_stock.Add(product_stock);
        //                this._DB.SaveChanges();

        //                #endregion

        //                #region"狀態"

        //                var Status = this._DB.product_status.Where(m => m.product_id == product_id).OrderByDescending(m => m.status_id).FirstOrDefault();
        //                int flag_old = Status != null ? Status.status : 0;

        //                product_status product_status = new product_status()
        //                {
        //                    product_id = product_id,
        //                    status = 1,
        //                    status_old = flag_old,
        //                    info = flag_old > 0 ? "系統匯入-降狀態" : "",
        //                    create_user = User.Identity.Name,
        //                    create_datetime = NW.Now
        //                };

        //                this._DB.product_status.Add(product_status);
        //                this._DB.SaveChanges();

        //                #endregion

        //            }

        //            #endregion

        //            msg = "匯入成功";

        //        }
        //        catch
        //        {
        //            msg = "匯入失敗，請與管理者聯繫";
        //        }

        //        break;
        //    }

        //    TempData[NW.ssjgrowl] = msg;
        //    return RedirectToAction("List");
        //}

        #endregion


        #region"分區"

        public ActionResult xxGroupList(int? page, string kw)
        {
            ViewBag.jgrow = TempData["jgrow"];

            var linq = this._DB.product_group.Where(m => m.group_id > 0);

            linq = linq.Where(m => m.parent_id == 0);

            if (!string.IsNullOrEmpty(kw))
            {
                ViewBag.kw = kw;
                kw = kw.ToLower();

                linq = linq.Where(m => m.name.ToLower().Contains(kw) || m.name_en.ToLower().Contains(kw));
            }

            int currentPageIndex = page.HasValue && page.Value > 0 ? page.Value - 1 : 0;
            IPagedList data = linq.OrderBy(m => m.inx).ToPagedList(currentPageIndex, 20);

            if (page.HasValue)
            {
                return PartialView(RouteData.Values["action"].ToString() + "_ajax", data);
            }
            else
            {
                return View(data);
            }

        }

        public ActionResult xxGroupAdd(int id)
        {
            ViewBag.jgrow = TempData["jgrow"];

            string msg = "";
            var data = this._DB.product_group.Where(m => m.group_id == id).SingleOrDefault();
            data = data != null ? data : new product_group() { };

            while (true)
            {
                if (data.group_id == 0 && id != 0) { msg = "查無資訊"; break; }

                if (data.group_id == 0)
                {
                    data.inx = 99;
                    data.visible = true;
                }

                List<ListItem> ListItems = new List<ListItem>() { };

                //分類
                string[] G = this._DB.product_group.Where(m => m.parent_id == 0 && m.visible).Select(m => m.group_id + "," + m.name).ToArray();

                ListItems = new List<ListItem>() { };
                ListItems.AddRange(Gs.sList(G, data.parent_id.ToString()));
                ViewData["parent_id"] = ListItems;

                break;
            }

            return View(data);
        }



        [HttpPost, ValidateInput(false)]
        public ActionResult xxGroupEditSave(product_group Get)
        {

            return RedirectToAction(RouteData.Values["action"].ToString().Replace("Add", "Edit").Replace("Save", ""), new { id = Get.group_id });
        }

        #endregion

        #region"分類----"

        public ActionResult GroupSubList(int? page, string kw, int? id)
        {
            var linq = this._DB.product_group.Where(m => m.group_id > 0);

            if (id.HasValue)
            {
                linq = linq.Where(m => m.parent_id == id);
            }

            if (!string.IsNullOrEmpty(kw))
            {
                ViewBag.kw = kw;
                kw = kw.ToLower();

                linq = linq.Where(m => m.name.ToLower().Contains(kw) || m.name_en.ToLower().Contains(kw));
            }

            int currentPageIndex = page.HasValue && page.Value > 0 ? page.Value - 1 : 0;
            IPagedList data = linq.OrderBy(m => m.inx).ToPagedList(currentPageIndex, 20);

            if (page.HasValue)
            {
                return PartialView(RouteData.Values["action"].ToString() + "_ajax", data);
            }
            else
            {
                return View(data);
            }
        }

        public ActionResult GroupSubAdd(int id)
        {
            return GroupAdd(id);
        }

        public ActionResult GroupSubEdit(int id)
        {
            return GroupAdd(id);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GroupSubEditSave(product_group Get)
        {
            return xxGroupEditSave(Get);
        }

        #endregion
    }
}
