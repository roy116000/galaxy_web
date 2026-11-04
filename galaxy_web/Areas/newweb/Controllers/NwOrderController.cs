using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using galaxy_web.Models;
using MvcPaging;
using ClosedXML.Excel;
using System.Data;
using System.Threading;
using System.Globalization;


namespace galaxy_web.Areas.newweb.Controllers
{
    public class NwOrderController : _BaseController
    {
        //
        // GET: /newweb/NwOrder/

        public class _order
        {
            public order_data order_data { get; set; }
            public order_sub order_sub { get; set; }
            public List<order_status> order_status { get; set; }
            public List<order_invoice> order_invoice { get; set; }
            public sys_admin_data company { get; set; }
            public sys_admin_data platform { get; set; }
        }
        
        public ActionResult List(int? page, string kw, DateTime? sD, DateTime? eD, int? status, string goods_sn, int? cid, int type = 0, string nation = "TW", int sort = 0, int inv_type = 0)
        {
            ViewBag.jgrowl = TempData[NW.ssjgrowl];

            List<ListItem> ListItems = new List<ListItem>() { };

            //待開發票列表用
            ViewBag.active = false;
            ViewBag.inv_type = inv_type;
            //發票
            var inv_list = this._DB.order_invoice.AsQueryable();
            var order_list = this._DB.order_data.AsQueryable();
            //如果選擇待開發票則取沒有開立消費者的
            if (inv_type == 1)
            {
                //選出有開立的order_id
                var exist_cus_inv = inv_list.Where(m => m.type == 1).Select(m => m.order_id).ToArray();
                //不包含開消費者發票的order_id
                order_list = order_list.Where(m => !exist_cus_inv.Contains(m.order_id));

                ViewBag.active = true;
            }

            var linq = (from a in order_list
                        select new
                        {
                            order_data = a,
                            order_sub = this._DB.order_sub.Where(m => m.order_id == a.order_id).OrderByDescending(m => m.sub_id).FirstOrDefault(),
                            order_status = this._DB.order_status.Where(m => m.order_id == a.order_id).OrderByDescending(m => m.order_id).ToList(),
                            order_invoice = inv_list.Where(m => m.order_id == a.order_id).OrderByDescending(m => m.invoice_id).ToList(),
                            company = this._DB.sys_admin_data.Where(m => m.admin_id == a.company_id).FirstOrDefault(),
                            platform = this._DB.sys_admin_data.Where(m => m.admin_id == a.platform_id).FirstOrDefault(),

                        }).ToList().Select(m => new _order()
                        {
                            order_data = m.order_data,
                            order_status = m.order_status ?? null,
                            order_sub = m.order_sub ?? new order_sub() { },
                            order_invoice = m.order_invoice ?? new List<order_invoice>() { },
                            company = m.company ?? new sys_admin_data() { },
                            platform = m.platform ?? new sys_admin_data() { },
                        });

            //權限
            var aData = NW.admin(NW.DESDeCode(User.Identity.Name));
            if (aData.power_id > 2)
            {
                linq = linq.Where(m => m.order_data.company_id == aData.admin_id);

                ViewBag.isC = true;
            }

            #region"國籍"

            ViewBag.nation = nation;
            nation = nation.ToLower();
            linq = linq.Where(m => m.order_data.nation.ToLower() == nation);

            #endregion

            #region"廠商"

            ListItems = new List<ListItem>() { };

            var Company = linq.Select(m => m.company.admin_id + "," + m.company.name).Distinct().ToArray();

            string c_n = cid.HasValue ? cid.ToString() : null;
            ListItems.AddRange(Gs.sList(Company, c_n, "全部"));
            ViewData["Company"] = ListItems;

            #endregion

            if (cid.HasValue)
            {
                ViewBag.cid = cid;
                linq = linq.Where(m => m.order_data.company_id == cid);
            }

            foreach (var item in linq)
            {
                if (item.order_status != null)
                {
                    //通知出貨時間
                    item.order_data.order_datetime = item.order_status.Where(m => m.status == 1).Select(m => m.create_datetime).SingleOrDefault() ?? null;
                    //出貨時間-(不是從退貨申請導回來的)
                    item.order_data.goods_datetime = item.order_status.Where(m => m.status == 2 && m.status_old != 3).Select(m => m.create_datetime).FirstOrDefault() ?? null;
                    //申請退貨時間
                    item.order_data.back_datetime = item.order_status.Where(m => m.status == 3).Select(m => m.create_datetime).FirstOrDefault() ?? null;
                    //退貨完成
                    item.order_data.backed_datetime = item.order_status.Where
                        (m =>
                            m.status == 4
                            || (m.status == 2 && m.status_old == 3)
                        ).Select(m => m.create_datetime).FirstOrDefault() ?? null;
                    //發票開立
                    item.order_data.invoice_datetime = item.order_status.Where(m => m.status == 5).Select(m => m.create_datetime).FirstOrDefault() ?? null;
                }
            }

            #region"時間"

            //eD = eD.HasValue ? eD : NW.Today;
            //sD = sD.HasValue ? sD : new DateTime(NW.Today.Year, NW.Today.Month, 1);
            if (sD.HasValue)
            {
                ViewBag.sD = NW.getDate(sD);

                linq = linq.Where(m =>
                    m.order_data.create_datetime >= sD
                    || m.order_data.order_datetime >= sD
                    || m.order_data.goods_datetime >= sD
                    || m.order_data.back_datetime >= sD
                    || m.order_data.backed_datetime >= sD
                    );
            }

            if (eD.HasValue)
            {
                ViewBag.eD = NW.getDate(eD);
                eD = eD.Value.AddDays(1);

                linq = linq.Where(m =>
                   m.order_data.create_datetime < eD
                   || m.order_data.order_datetime < eD
                   || m.order_data.goods_datetime < eD
                   || m.order_data.back_datetime < eD
                   || m.order_data.backed_datetime < eD
                   );

            }

            #endregion

            #region"關鍵字"

            if (!string.IsNullOrEmpty(kw))
            {
                ViewBag.kw = kw;
                kw = kw.ToLower();

                linq = linq.Where(m =>
                    (m.order_data.order_name == null ? false : m.order_data.order_name.ToLower().Contains(kw))
                    || (m.order_data.order_tel == null ? false : m.order_data.order_tel.Contains(kw))
                    || (m.order_data.send_name == null ? false : m.order_data.send_name.ToLower().Contains(kw))
                    || (m.order_data.send_tel == null ? false : m.order_data.send_tel.Contains(kw))
                    || (m.order_data.order_sn == null ? false : m.order_data.order_sn.Contains(kw))
                    || (m.order_data.platform_sn == null ? false : m.order_data.platform_sn.Contains(kw))
                    || (m.company.name == null ? false : m.company.name.ToLower().Contains(kw))
                    || (m.company.name_full == null ? false : m.company.name_full.ToLower().Contains(kw))
                    || (m.order_sub.item_name == null ? false : m.order_sub.item_name.ToLower().Contains(kw))
                    || (m.order_sub.item_no == null ? false : m.order_sub.item_no.ToLower().Contains(kw))
                    );
            }

            #endregion

            #region"狀態"

            ListItems = new List<ListItem>() { };
            ListItems.Add(new ListItem("", "全部 - " + linq.Count(), !status.HasValue));

            //扣除發票狀態
            var status_group = this._DB.order_status_flag.Where(m => m.visible).OrderBy(m => m.inx).ToList();
            ////再加回發票的 分廠商開立 和開立消費者發票
            //var inv_model = new order_status_flag { 
            //    flag_id = 5,
            //    name = "廠商開立發票",
            //    visible = true,
            //    inx = 5,
            //    create_user = "1"
            //};
            //status_group.Add(inv_model);
            //inv_model = new order_status_flag
            //{
            //    flag_id = 5,
            //    name = "開立消費者發票",
            //    visible = true,
            //    inx = 5,
            //    create_user = "2"
            //};
            //status_group.Add(inv_model);
            ////再次排序
            //status_group = status_group.OrderBy(m => m.inx).ToList();

            var G = (from a in status_group
                     join b in linq on a.flag_id equals b.order_data.status into mm
                     select new ListItem
                         (
                            a.flag_id.ToString(),
                            a.name + " - " + mm.Count(),
                            status.HasValue ? (a.flag_id == status.Value && a.flag_id != 5 ? true : (a.flag_id == status.Value && a.flag_id == 5 && inv_type == 0 ? true : false)) : false
                         )).ToList();


            ListItems.AddRange(G);
            ViewData["G"] = ListItems;

            if (status.HasValue)
            {
                ViewBag.status = status;

                linq = linq.Where(m => m.order_data.status == status);
            }

            #endregion

            //匯出
            switch (type)
            {
                case 1:

                    return Export(linq);
                case 2:

                    return Export(linq, true);

                case 999:
                    return LiExport(linq);
            }

            //if (type == 1)
            //{
            //    return Export(linq);             
            //}

            int currentPageIndex = page.HasValue && page.Value > 0 ? page.Value - 1 : 0;

            //排序
            switch (sort)
            {
                case 1:
                    //廠商名中文排序
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("zh-TW");
                    var company_sorting = this._DB.sys_admin_data.Where(m => m.power_id == 3).Select(m => new { m.admin_id, m.name }).AsEnumerable().OrderBy(m => m.name.Substring(0, 1)).ToArray();
                    var linq2 = new List<_order> { };
                    foreach (var item in company_sorting)
                    {
                        linq2.AddRange(linq.Where(m => m.company.admin_id == item.admin_id).ToList());
                    }
                    linq = linq2.AsQueryable();

                    break;
                case 2:
                    //收件人中文排序
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("zh-TW");
                    linq = linq.OrderBy(m => m.order_data.send_name.Substring(0,1));
                    break;
                case 3:
                    //訂購日遞減
                    linq = linq.OrderByDescending(m => m.order_data.order_datetime);
                    break;
                case 4:
                    //出貨日遞減
                    linq = linq.OrderByDescending(m => m.order_data.goods_datetime);
                    break;
                default:
                    linq = linq.OrderByDescending(m => m.order_data.order_datetime);
                    break;
            }
            ViewBag.sort = sort;


            var data = linq.ToPagedList(currentPageIndex, 20);

            ViewBag.jgrowl = TempData["jgrowl"];
            if (page.HasValue)
            {
                return PartialView(RouteData.Values["action"].ToString() + "_ajax", data);
            }
            else
            {
                return View(data);
            }

        }

        public ActionResult CNList(int? page, string kw, DateTime? sD, DateTime? eD, int? status, string goods_sn, int? cid, int type = 0)
        {
            return List(page, kw, sD, eD, status, goods_sn, cid, type, "CN");
        }

        public ActionResult SampleList(int? page, string kw, DateTime? sD, DateTime? eD, int? status, string goods_sn, int? cid, int type = 0)
        {
            return List(page, kw, sD, eD, status, goods_sn, cid, type, "TS");
        }

        public ActionResult Edit(int id)
        {
            ViewBag.jgrowl = TempData["jgrowl"];

            var data = (from a in this._DB.order_data.Where(m => m.order_id == id)
                        select new
                        {
                            order_data = a,
                            order_sub = this._DB.order_sub.Where(m => m.order_id == a.order_id).OrderByDescending(m => m.sub_id).FirstOrDefault(),
                            order_status = this._DB.order_status.Where(m => m.order_id == a.order_id).OrderByDescending(m => m.order_id).ToList(),
                            order_invoice = this._DB.order_invoice.Where(m => m.order_id == a.order_id).OrderByDescending(m => m.invoice_id).ToList(),
                            company = this._DB.sys_admin_data.Where(m => m.admin_id == a.company_id).FirstOrDefault(),
                            platform = this._DB.sys_admin_data.Where(m => m.admin_id == a.platform_id).FirstOrDefault()
                        }).ToList().Select(m => new _order()
                        {
                            order_data = m.order_data,
                            order_sub = m.order_sub ?? new order_sub() { },
                            order_status = m.order_status ?? null,
                            order_invoice = m.order_invoice ?? new List<order_invoice>() { },
                            company = m.company ?? new sys_admin_data() { },
                            platform = m.platform ?? new sys_admin_data() { },
                        }).SingleOrDefault();

            if (data == null && id != 0) { TempData["jgrowl"] = "查無相關資訊"; return RedirectToAction(RouteData.Values["action"].ToString().Replace("Eidt", "List").Replace("Save", "")); }

            var aData = NW.admin(NW.DESDeCode(User.Identity.Name));
            if (aData.power_id > 2 && data.order_data.company_id != aData.admin_id) { TempData[NW.ssjgrowl] = "解析錯誤"; return RedirectToAction(RouteData.Values["action"].ToString().Replace("Eidt", "List")); }

            ViewBag.isC = aData.power_id > 2;


            //通知出貨時間
            data.order_data.order_datetime = data.order_status.Where(m => m.status == 1).Select(m => m.create_datetime).FirstOrDefault() ?? null;
            //出貨時間-(不是從退貨申請導回來的)
            data.order_data.goods_datetime = data.order_status.Where(m => m.status == 2 && m.status_old != 3).Select(m => m.create_datetime).FirstOrDefault() ?? null;
            //申請退貨時間
            data.order_data.back_datetime = data.order_status.Where(m => m.status == 3).Select(m => m.create_datetime).FirstOrDefault() ?? null;
            //退貨完成
            data.order_data.backed_datetime = data.order_status.Where
                (m =>
                    m.status == 4
                    || (m.status == 2 && m.status_old == 3)
                ).Select(m => m.create_datetime).FirstOrDefault() ?? null;
            //發票開立
            data.order_data.invoice_datetime = data.order_status.Where(m => m.status == 5).Select(m => m.create_datetime).FirstOrDefault() ?? null;

            //消費者發票
            var cus_inv = this._DB.order_invoice.Where(m => m.order_id == data.order_data.order_id && m.type == 1).OrderByDescending(m => m.invoice_id).FirstOrDefault() ?? new order_invoice { };
            ViewData["cus_inv"] = cus_inv;

            #region"前台比對"

            var Status_F = this._DB.order_status_flag.ToDictionary(m => m.flag_id, m => m.name);
            ViewData["Status_F"] = Status_F;

            #endregion

            #region"選單"

            List<ListItem> ListItems = new List<ListItem>() { };

            #region"物流公司"

            var Goods = this._DB.order_goods.Where(m => m.visible).OrderBy(m => m.inx).Select(m => m.goods_id + "," + m.name).ToArray();
            ListItems = new List<ListItem>() { };
            ListItems.AddRange(Gs.sList(Goods, data.order_data.goods_id.ToString(), "請選擇"));
            ViewData["Goods"] = ListItems;

            #endregion

            #region"發票-類型"

            var Inv = NW.Invoice_Name.Select(m => m.Key + "," + m.Value).ToArray();
            ListItems = new List<ListItem>() { };
            ListItems.AddRange(Gs.sList(Inv, data.order_invoice.Where(m => m.type == 0).FirstOrDefault() == null ? "0" : data.order_invoice.Where(m => m.type == 0).FirstOrDefault().flag.ToString()));
            ViewData["Inv"] = ListItems;

            #endregion

            #region"發票-歸屬"

            string[] InvAtt = NW.Invoice_Attr.OrderBy(m => m.Key).Select(m => m.Key + "," + m.Value).ToArray();
            ListItems = new List<ListItem>() { };
            ListItems.AddRange(Gs.sList(InvAtt, data.order_invoice.Where(m => m.type == 0).FirstOrDefault() == null ? "0" : data.order_invoice.Where(m => m.type == 0).FirstOrDefault().flag.ToString()));
            ViewData["InvAttr"] = ListItems;

            #endregion

            #endregion

            return View(data);
        }

        [ValidateInput(false), HttpPost]
        public ActionResult EditSave(order_data Get, int? index, string goods_sn, order_invoice order_invoice, bool isback = false, int backed_cancel = 0)
        {
            string msg = "", info = "";
            int status = 0;

            DateTime? sNow = DateTime.Now;

            var data = this._DB.order_data.Where(m => m.order_id == Get.order_id).SingleOrDefault();
            if (data == null && Get.order_id != 0) { TempData["jqrowl"] = "查無相關資訊"; return RedirectToAction("List"); }

            //找出各個時段
            var Status = this._DB.order_status.Where(m => m.order_id == Get.order_id).OrderByDescending(m => m.status_id)
                .Select(m => new { m.status, m.create_datetime }).ToList();

            //出貨時間
            data.goods_datetime = Status.Where(m => m.status == 2).Select(m => m.create_datetime).FirstOrDefault() ?? null;
            //申請退貨時間
            data.back_datetime = Status.Where(m => m.status == 3).Select(m => m.create_datetime).FirstOrDefault() ?? null;

            //最後一個狀態
            data.status = Status.Select(m => m.status).FirstOrDefault();

            while (true)
            {

                switch (index)
                {

                    #region"出貨"

                    case 2:


                        if (!Get.goods_datetime.HasValue) { msg = "請填寫處理時間"; break; }
                        if (data.status != 1) { msg = "商品無出貨通知"; break; }
                        if (string.IsNullOrEmpty(goods_sn)) { msg = "請填寫出貨單號"; break; }

                        sNow = Get.goods_datetime;

                        ////出貨時間不能填過去時間
                        //if (sNow < DateTime.Today)
                        //{
                        //    msg = "出貨時間只能選擇今天或以後。";
                        //    break;
                        //}

                        data.goods_id = Get.goods_id;

                        info = goods_sn;
                        status = 2;

                        //明細
                        var Sub = this._DB.order_sub.Where(m => m.order_id == data.order_id).OrderByDescending(m => m.sub_id).FirstOrDefault();
                        if (Sub == null) { msg = "查無明細，故無法出貨"; break; }

                        #region"銷貨庫存"

                        var L_Stock = this._DB.product_stock.Where(m => m.product_id == Sub.product_id).OrderByDescending(m => m.stock_id).FirstOrDefault();
                        int Num_old = L_Stock != null ? L_Stock.num : 0;
                        var Stock = new product_stock()
                        {
                            product_id = Sub.product_id,
                            num = -Sub.num,
                            num_old = Num_old,
                            visible = true,
                            info = "訂單單號：" + data.order_sn + "，出貨單號：" + goods_sn,
                            create_user = User.Identity.Name,
                            create_datetime = sNow
                        };

                        this._DB.product_stock.Add(Stock);
                        this._DB.SaveChanges();

                        #endregion

                        //判斷商品庫存0時下架
                        var stock_total = this._DB.product_stock.Where(m => m.visible && m.product_id == Sub.product_id).Select(m => m.num).DefaultIfEmpty(0).Sum();
                        if (stock_total <= 0)
                        {

                            //商品資料狀態
                            var this_product = this._DB.product_data.Where(m => m.product_id == Sub.product_id).FirstOrDefault();
                            var temp_status = this_product.status;
                            this_product.status = 0;

                            //狀態資料列表
                            var r = new product_status()
                            {
                                product_id = Sub.product_id,
                                status = 0,
                                status_old = temp_status,
                                info = "庫存小於0自動退回草稿",
                                create_user = User.Identity.Name,
                                create_datetime = NW.Now
                            };

                            this._DB.product_status.Add(r);
                            this._DB.SaveChanges();
                        }

                        break;

                    #endregion


                    #region"退貨"

                    case 4:

                        if (!Get.backed_datetime.HasValue) { msg = "請填寫處理時間"; break; }
                        if (data.status != 3 && !data.back_datetime.HasValue) { msg = "此訂單不須退貨"; break; }

                        sNow = Get.backed_datetime;

                        info = isback ? "退貨完成" : "取消退貨";
                        status = isback ? 4 : 2;

                        break;

                    #endregion

                    case 5:

                        if (!order_invoice.create_datetime.HasValue) { msg = "請填寫處理時間"; break; }
                        if (data.status != 2 && !data.goods_datetime.HasValue) { msg = "商品無法開立發票"; break; }
                        if (data.goods_datetime > NW.Today.AddDays(10)) { msg = "商品出貨是為" + data.goods_datetime + "，尚無法開立發票"; break; }
                        if (order_invoice.flag == 2 && string.IsNullOrEmpty(order_invoice.title)) { msg = "您選擇的是三聯式發票，需填寫發票臺頭"; break; }
                        if (string.IsNullOrEmpty(order_invoice.no)) { msg = "請填寫發票號碼"; break; }

                        sNow = order_invoice.create_datetime;

                        #region"新增發票資訊"

                        var I = new order_invoice() { };
                        I.order_id = data.order_id;
                        //I.attribution_id = order_invoice.attribution;
                        I.attribution = order_invoice.attribution;
                        I.flag = order_invoice.flag;
                        I.title = order_invoice.title;
                        I.no = order_invoice.no;
                        I.create_user = User.Identity.Name;
                        I.create_datetime = sNow;
                        I.type = 0;
                        this._DB.order_invoice.Add(I);

                        #endregion

                        info = "開立發票：" + (!string.IsNullOrEmpty(I.title) ? I.title : "無臺頭") + "," + I.no;
                        status = 5;

                        break;
                    case 6:

                        if (!order_invoice.create_datetime.HasValue) { msg = "請填寫處理時間"; break; }
                        if (data.status != 2 && !data.goods_datetime.HasValue) { msg = "商品無法開立發票"; break; }
                        if (data.goods_datetime > NW.Today.AddDays(10)) { msg = "商品出貨是為" + data.goods_datetime + "，尚無法開立發票"; break; }
                        if (order_invoice.flag == 2 && string.IsNullOrEmpty(order_invoice.title)) { msg = "您選擇的是三聯式發票，需填寫發票臺頭"; break; }
                        if (string.IsNullOrEmpty(order_invoice.no)) { msg = "請填寫發票號碼"; break; }

                        sNow = order_invoice.create_datetime;

                        #region"新增發票資訊"
                        I = new order_invoice() { };
                        I.order_id = data.order_id;
                        //I.attribution_id = order_invoice.attribution;
                        I.attribution = order_invoice.attribution;
                        I.flag = order_invoice.flag;
                        I.title = order_invoice.title;
                        I.no = order_invoice.no;
                        I.create_user = User.Identity.Name;
                        I.create_datetime = sNow;
                        I.type = 1;
                        this._DB.order_invoice.Add(I);

                        #endregion

                        info = "開立發票：" + (!string.IsNullOrEmpty(I.title) ? I.title : "無臺頭") + "," + I.no;
                        status = 5;

                        break;
                }

                //沒有任何錯誤訊息
                if (string.IsNullOrEmpty(msg))
                {
                    #region"新增紀錄"

                    var S = new order_status() { };
                    //D.status_id = order_status.status_id;
                    S.order_id = data.order_id;
                    S.status = status;
                    S.status_old = data.status;
                    S.info = info;
                    S.create_user = User.Identity.Name;
                    S.create_datetime = sNow;

                    this._DB.order_status.Add(S);

                    #endregion

                    data.status = S.status;

                    this._DB.SaveChanges();


                    msg = "修改成功";
                }


                break;
            }


            TempData["jgrowl"] = msg;
            return RedirectToAction(RouteData.Values["action"].ToString().Replace("Save", "").Replace("Add", "Edit"), new { id = Get.order_id });
        }

        [HttpPost]
        public ActionResult ListDel(int cb_flag, string cb_id)
        {
            Dictionary<int, string> jgrowl_msg = new Dictionary<int, string>() { { 0, "退回草稿區" }, { 3, "上架" }, { 4, "下架" }, { 999, "作廢" } };
            //string[] jgrowl_msg = new string[] { "退回草稿區", "上架", "下架", "作廢" };
            //int[] strArray = Array.ConvertAll<string, int>(NW.Split(cb_id, ","), int.Parse);

            string sql = "";
            switch (cb_flag)
            {

                case 999: //停用
                    sql = "Update order_data set status = 999 Where order_id in (" + cb_id + ")";

                    break;

            }

            if (sql != "")
            {
                _DB.Database.ExecuteSqlCommand(sql);
            }

            foreach (var item in cb_id.Split(','))
            {
                int id = int.Parse(item);
                var R = new order_status()
                {
                    order_id = int.Parse(item),
                    status = cb_flag,
                    status_old = this._DB.order_status.Where(m => m.order_id == id).OrderByDescending(m => m.status_id).Select(m => m.status).FirstOrDefault(),
                    create_user = User.Identity.Name,
                    create_datetime = NW.Now
                };

                this._DB.order_status.Add(R);
            }
            this._DB.SaveChanges();

            TempData["jgrowl"] = jgrowl_msg[cb_flag] + "功能，設定成功";
            return RedirectToAction(RouteData.Values["action"].ToString().Replace("Del", ""), new { status = cb_flag });
        }



        #region"匯出"

        public ActionResult Export(IEnumerable<object> objects, bool reduce_col = false)
        {

            Dictionary<string, DataTable> dt = new Dictionary<string, DataTable> { };

            var data = (from a in objects as IEnumerable<_order>
                        select new
                        {
                            平台單號 = a.order_data.platform_sn,
                            ERP單號 = a.order_data.order_sn,

                            訂購方姓名 = a.order_data.order_name,
                            訂購方電話 = a.order_data.order_tel,
                            訂購方郵遞區號 = a.order_data.order_zip,
                            訂購方地址 = Gs.getAddress(a.order_data.order_address, a.order_data.order_zip),

                            寄送方姓名 = a.order_data.send_name,
                            寄送方電話 = a.order_data.send_tel,
                            寄送方郵遞區號 = a.order_data.send_zip,
                            寄送方地址 = Gs.getAddress(a.order_data.send_address, a.order_data.send_zip),

                            訂購時間 = a.order_data.order_datetime,
                            出貨時間 = a.order_data.goods_datetime,
                            退貨時間 = a.order_data.back_datetime,
                            退貨完成時間 = a.order_data.backed_datetime,
                            商品貨號 = a.order_sub.item_no,
                            //商品數量 = a.order_sub.num,
                            物流單號 = a.order_status.Where(m => m.status == 2).Select(m => m.info).FirstOrDefault(),
                            物流廠商 = this._DB.order_goods.Where(m => m.goods_id == a.order_data.goods_id).Select(m => m.name).FirstOrDefault(),
                            發票日期 = a.order_invoice.Where(m => m.type == 0).Select(m => m.create_datetime).FirstOrDefault(),
                            發票號碼 = a.order_invoice.Where(m => m.type == 0).Select(m => m.no).FirstOrDefault(),
                            國籍 = a.order_data.nation,
                            //ERP_單號 = a.order_data.order_sn,
                            商品名稱 = a.order_sub.item_name,
                            備註 = a.order_sub.info,
                            購買數量 = a.order_sub.num,
                            成本 = a.order_sub.cost,
                            小計 = a.order_sub.cost * a.order_sub.num,
                            消費者發票日期 = a.order_invoice.Where(m => m.type == 1).Select(m => m.create_datetime).FirstOrDefault(),
                            消費者發票號碼 = a.order_invoice.Where(m => m.type == 1).Select(m => m.no).FirstOrDefault(),
                        });



            if (reduce_col)
            {
                var data1 = (from a in data
                             select new
                             {
                                 平台單號 = a.平台單號,
                                 ERP單號 = a.ERP單號,

                                 訂購方姓名 = a.訂購方姓名,
                                 寄送方姓名 = a.寄送方姓名,

                                 訂購時間 = a.訂購時間,
                                 出貨時間 = a.出貨時間,

                                 商品貨號 = a.商品貨號,

                                 物流單號 = a.物流單號,
                                 物流廠商 = a.物流廠商,
                                 發票日期 = a.發票日期,
                                 發票號碼 = a.發票號碼,
                                 國籍 = a.國籍,

                                 商品名稱 = a.商品名稱,
                                 備註 = a.備註,
                                 購買數量 = a.購買數量,
                                 成本 = a.成本,
                                 小計 = a.小計,
                                 消費者發票日期 = a.消費者發票日期,
                                 消費者發票號碼 = a.消費者發票號碼
                             });

                dt = new Dictionary<string, DataTable>() 
            { 
                { "訂單資訊", Gs.ConvertObjectsToDataTable(data1) }, 
                //{ "訂單明細", Gs.ConvertObjectsToDataTable(sub) }
            };
            }
            else {
                dt = new Dictionary<string, DataTable>() 
            { 
                { "訂單資訊", Gs.ConvertObjectsToDataTable(data) }, 
                //{ "訂單明細", Gs.ConvertObjectsToDataTable(sub) }
            };
            }

            //var sub = from a in objects as IEnumerable<_order>
            //          select new
            //          {
            //              ERP_單號 = a.order_data.order_sn,
            //              商品貨號 = a.order_sub.item_no,
            //              商品名稱 = a.order_sub.item_name,
            //              備註 = a.order_sub.info,
            //              購買數量 = a.order_sub.num,
            //              成本 = a.order_sub.cost,
            //              小計 = a.order_sub.cost * a.order_sub.num
            //          };

          
            string fileName = string.Format("{0}{1}.xlsx", NW.Now.ToString("yyyyMMddHHmmss"), NW.GetUniqueKey());

            string Controller = RouteData.Values["controller"].ToString().ToLower().Replace("nw", "");
            string tmpPath = Server.MapPath("~/Content/files/") + Controller; //存放路徑

            var r = Gs.ExportExcelFromDataTable(dt);

            if (!string.IsNullOrEmpty(r.Key))
            {
                TempData[NW.ssjgrowl] = r.Key;
                return RedirectToAction("List");

            }
            else
            {
                return File(r.Value.ToArray(), "application/vnd.ms-excel", fileName);
            }
        }

        #endregion

        #region"匯出"

        public ActionResult LiExport(IEnumerable<object> objects)
        {
            var Pay = this._DB.order_pay.ToDictionary(m => m.pay_id, m => m.pay_name);
            var Company = this._DB.sys_admin_data.ToDictionary(m => m.admin_id, m => m.name);
            var Status = this._DB.order_status_flag.ToDictionary(m => m.flag_id, m => m.name);
            Status.Add(0, "未定義");

            var data = (from a in objects as IEnumerable<_order>
                        select new
                        {
                            狀態 = Status[a.order_data.status],

                            平台單號 = a.order_data.platform_sn,
                            訂單編號 = a.order_data.order_sn,
                            商品名稱 = a.order_sub.item_name,
                            商品貨號 = a.order_sub.item_no,
                            商品數量 = a.order_sub.num,
                            廠商 = Company[a.order_data.company_id],
                            購買數量 = a.order_sub.num,
                            售價 = a.order_sub.price,

                            成本 = a.order_sub.cost,

                            紅利 = "",
                            付款方式 = Pay[a.order_sub.pay_id],


                            訂購方姓名 = a.order_data.order_name,
                            訂購方電話 = a.order_data.order_tel,
                            電子郵件 = "",
                            訂購方郵遞區號 = a.order_data.order_zip,
                            訂購方地址 = Gs.getAddress(a.order_data.order_address, a.order_data.order_zip),
                            生日 = "",

                            收件方姓名 = a.order_data.send_name,
                            收件方電話 = a.order_data.send_tel,
                            收件方郵遞區號 = a.order_data.send_zip,
                            收件方地址 = Gs.getAddress(a.order_data.send_address, a.order_data.send_zip),


                            發票號碼 = a.order_invoice.Where(m => m.type == 0).Select(m => m.no).FirstOrDefault(),

                            送貨時段及時間 = "",
                            備註 = a.order_sub.info,
                            國籍 = a.order_data.nation,

                            訂購時間 = a.order_data.order_datetime,
                            出貨時間 = a.order_data.goods_datetime,
                            退貨時間 = a.order_data.back_datetime,
                            退貨完成時間 = a.order_data.backed_datetime,
                            發票日期 = a.order_invoice.Where(m => m.type == 0).Select(m => m.create_datetime).FirstOrDefault()

                        });

            //var sub = from a in objects as IEnumerable<_order>
            //          select new
            //          {
            //              ERP_單號 = a.order_data.order_sn,
            //              商品貨號 = a.order_sub.item_no,
            //              商品名稱 = a.order_sub.item_name,
            //              備註 = a.order_sub.info,
            //              購買數量 = a.order_sub.num,
            //              成本 = a.order_sub.cost,
            //              小計 = a.order_sub.cost * a.order_sub.num
            //          };

            Dictionary<string, DataTable> dt = new Dictionary<string, DataTable>() 
            { 
                { "訂單資訊", Gs.ConvertObjectsToDataTable(data) }, 
                //{ "訂單明細", Gs.ConvertObjectsToDataTable(sub) }
            };

            string fileName = string.Format("{0}{1}.xlsx", NW.Now.ToString("yyyyMMddHHmmss"), NW.GetUniqueKey());

            string Controller = RouteData.Values["controller"].ToString().ToLower().Replace("nw", "");
            string tmpPath = Server.MapPath("~/Content/files/") + Controller; //存放路徑

            var r = Gs.ExportExcelFromDataTable(dt);

            if (!string.IsNullOrEmpty(r.Key))
            {
                TempData[NW.ssjgrowl] = r.Key;
                return RedirectToAction("List");

            }
            else
            {
                return File(r.Value.ToArray(), "application/vnd.ms-excel", fileName);
            }
        }

        #endregion


        public string getSn()
        {
            string Sn = "";

            string Pre = (NW.Today.Year - 1911 - 100).ToString().PadLeft(2, '0') + NW.Today.Month.ToString().PadLeft(2, '0') + NW.Today.Day.ToString().PadLeft(2, '0');

            var data = this._DB.order_data.Where(m => m.order_sn.StartsWith(Pre)).Max(m => m.order_sn);
            if (data != null)
            {
                try
                {
                    string Max = (int.Parse(data.Split(new string[] { Pre }, StringSplitOptions.None)[1]) + 1).ToString().PadLeft(5, '0');
                    Sn = Pre + Max;
                }
                catch
                {
                    Sn = null;
                }
            }
            else
            {
                Sn = Pre + "1".PadLeft(5, '0');
            }

            return Sn;
        }

        #region 醫美專區

        public ActionResult NoteList(int? page, string kw, DateTime? sD, DateTime? eD, int? cid)
        {
            int currentPageIndex = page.HasValue && page.Value > 0 ? page.Value - 1 : 0;

            var data = this._DB.order_data.Where(m => m.status > 0);

            //權限
            var aData = NW.admin(NW.DESDeCode(User.Identity.Name));
            if (aData.power_id > 2)
            {
                data = data.Where(m => m.company_id == aData.admin_id);
            }

            ViewBag.power_id = aData.power_id;

            //非這兩種全限的
            if (!new int[] { 2, 3 }.Contains(aData.power_id))
            {
                TempData["jgrowl"] = "您無此權限。";
                return RedirectToAction("List");
            }

            //沒有權限的廠商
            if (aData.power_id > 2 && !aData.note_visible)
            {
                TempData["jgrowl"] = "您無此權限。";
                return RedirectToAction("List");
            }

            //開放使用note功能的廠商
            var Comp = this._DB.sys_admin_data.Where(m => m.visible && m.power_id == 3 && m.note_visible);
            var c_arr = Comp.Select(m => m.admin_id).ToArray();
            ViewData["com"] = Comp.ToList();

            //管理員下拉選擇廠商
            if (aData.power_id == 2)
            {
                //顯示權限資料裡有的廠商供選擇
                var ListItems = new List<ListItem>() { };
                var Company = Comp.Select(m => m.admin_id + "," + m.name).Distinct().ToArray();

                string c_n = cid.HasValue ? cid.ToString() : null;
                ListItems.AddRange(Gs.sList(Company, c_n, "全部"));
                ViewData["Company"] = ListItems;
            }

            //訂單過濾
            data = data.Where(m => c_arr.Contains(m.company_id));

            if (cid.HasValue)
            {
                data = data.Where(m => m.company_id == cid);
                ViewBag.cid = cid;
            }

            //日期
            if (sD.HasValue)
            {
                data = data.Where(m => m.create_datetime >= sD.Value);
                ViewBag.sD = sD;
            }
            if (eD.HasValue)
            {
                var eed = eD.Value.AddDays(1);
                data = data.Where(m => m.create_datetime < eed);
                ViewBag.eD = eD;
            }

            //關鍵字
            if (!string.IsNullOrEmpty(kw))
            {
                var kw2 = kw.ToLower();
                var data2 = data.Select(m => m.order_id).ToArray();
                var subb = this._DB.order_sub.Where(m => data2.Contains(m.order_id) && m.item_name.Contains(kw2)).Select(m => m.order_id).ToArray();

                data = data.Where(m => m.order_name.Contains(kw2) || m.order_tel.Contains(kw2) || m.send_name.Contains(kw2) || m.send_tel.Contains(kw2) || subb.Contains(m.order_id));

                ViewBag.kw = kw;
            }


            if (aData.power_id == 3)
            {
                data = data.Where(m => m.company_id == aData.admin_id);
            }

            ViewData["note"] = this._DB.order_note.ToList();
            ViewData["status"] = this._DB.order_status.AsQueryable();
            ViewData["sub"] = this._DB.order_sub.AsQueryable();

            var result = data.OrderByDescending(m => m.create_datetime).ToPagedList(currentPageIndex, 20);

            ViewBag.jgrowl = TempData["jgrowl"];


            if (page.HasValue)
            {
                return PartialView(RouteData.Values["action"].ToString() + "_ajax", result);
            }
            else
            {
                return View(result);
            }
        }

        public ActionResult NoteEdit(int id = 0)
        {
            //判斷是不是自己的
            var order = this._DB.order_data.Where(m => m.order_id == id).FirstOrDefault();
            if (order == null)
            {
                ViewBag.jgrowl = "查無此筆。";
                return RedirectToAction("NoteList");
            }
            //權限
            var aData = NW.admin(NW.DESDeCode(User.Identity.Name));
            if (aData.power_id > 2)
            {
                if (order.company_id != aData.admin_id)
                {
                    ViewBag.jgrowl = "查無此筆訂單。";
                    return RedirectToAction("NoteList");
                }
            }

            //該筆訂單的紀錄資料
            var note = this._DB.order_note.Where(m => m.order_id == id).FirstOrDefault() ?? new order_note { };
            ViewData["note"] = note;
            var sub = this._DB.order_sub.Where(m => m.order_id == id).FirstOrDefault() ?? new order_sub { };
            ViewData["sub"] = sub;

            ViewBag.jgrowl = TempData["jgrowl"];
            return View(order);
        }

        public ActionResult NoteEditSave(order_note order_note, string hour = "00", string minute = "00")
        {
            if (!order_note.note_time.HasValue)
            {
                TempData["jgrowl"] = "未選擇預約日期";
                return RedirectToAction(RouteData.Values["action"].ToString().Replace("Save", "").Replace("Save", ""), new { id = order_note.order_id });
            }

            var pre_datetime = new DateTime(order_note.note_time.Value.Year, order_note.note_time.Value.Month, order_note.note_time.Value.Day, Convert.ToInt32(hour), Convert.ToInt32(minute), 0);

            var data = this._DB.order_note.Where(m => m.note_id == order_note.note_id).FirstOrDefault();

            if (data == null)
            {
                //新增
                var new_note = new order_note { };
                new_note.company_id = order_note.company_id;
                new_note.create_datetime = DateTime.Now;
                new_note.create_user = User.Identity.Name;
                new_note.note_time = pre_datetime;
                new_note.order_id = order_note.order_id;
                new_note.visible = true;

                this._DB.order_note.Add(new_note);
            }
            else {
                data.note_time = pre_datetime;
            }

            this._DB.SaveChanges();

            TempData["jgrowl"] = "修改完成";
            return RedirectToAction(RouteData.Values["action"].ToString().Replace("Save", "").Replace("Save", ""), new { id = order_note.order_id });
        }

        #endregion



    }
}
