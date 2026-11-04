using ClosedXML.Excel;
using galaxy_web.Models;
using MvcPaging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace galaxy_web.Areas.newweb.Controllers
{
    public class NwBillsController : _BaseController
    {
        //
        // GET: /newweb/NwReport/



        //public ActionResult StayList_ajax(string kw, int? page, int? start_date, int? end_date)
        //{

        //    int currentPageIndex = page.HasValue ? page.Value - 1 : 0;

        //    var company_data = this._DB.company_data.Where(m => m.visible).OrderBy(m => m.company_id).ToList();

        //    if (!string.IsNullOrEmpty(kw))
        //    {
        //        company_data = company_data.Where(m => m.name.Contains(kw)).ToList();
        //    }

        //    List<data_list> Linq = new List<data_list> { };

        //    foreach (var com in company_data)
        //    {
        //        List<data_list> data2 = getBills(com);
        //        Linq.AddRange(data2);

        //    }

        //    if (start_date.HasValue && start_date != 0)
        //    {
        //        Linq = Linq.Where(m => m.hits >= start_date).ToList();
        //    }
        //    if (end_date.HasValue && end_date != 0)
        //    {
        //        Linq = Linq.Where(m => m.hits <= end_date).ToList();
        //    }


        //    ViewBag.start_date = start_date;
        //    ViewBag.end_date = end_date;
        //    ViewBag.kw = kw;

        //    IPagedList data = Linq.Where(m => m.cost != 0 && m.status_flag == 0).OrderByDescending(m => m.hits).ToPagedList(currentPageIndex, 12);

        //    return PartialView(RouteData.Values["action"].ToString(), data);
        //}



        ////未用
        //[HttpPost]
        //public ActionResult StayEditSave(order_data order_data, int index)
        //{
        //    var data = this._DB.order_data.Find(order_data.order_id);

        //    switch (index)
        //    {
        //        case 0:    //金流資料
        //            data.pay_id = order_data.pay_id;
        //            data.pay_visible = order_data.pay_visible;
        //            data.pay_note = order_data.pay_note;
        //            if (order_data.pay_visible) { if (data.pay_datetime == null) { data.pay_datetime = DateTime.Now; } } else { data.pay_datetime = null; }
        //            data.flag = order_data.pay_visible ? 1 : 0;
        //            this._DB.SaveChanges();
        //            TempData["jgrowl"] = "金流資訊儲存成功!!";
        //            break;
        //        case 1:    //物流資料

        //            if (data.pay_visible)
        //            {
        //                data.freight_name = order_data.freight_name;
        //                data.freight_no = order_data.freight_no;
        //                data.freight_datetime = order_data.freight_datetime;
        //                data.goods_datetime = order_data.freight_datetime;
        //                data.goods_info = "WEB填入";
        //                if (!string.IsNullOrEmpty(data.freight_name)) { data.flag = 2; }
        //                this._DB.SaveChanges();


        //                DateTime dt = DateTime.Now;
        //                foreach (var item in data.order_sub)
        //                {
        //                    var data2 = new order_goods { };
        //                    data2.order_id = data.order_id;
        //                    data2.sub_id = item.sub_id;
        //                    data2.name = data.freight_name;
        //                    data2.sn = data.freight_no;
        //                    data2.num = item.num;
        //                    data2.goods_datetime = data.goods_datetime;
        //                    data2.create_user = User.Identity.Name;
        //                    data2.create_datetime = dt;

        //                    _DB.order_goods.Add(data2);
        //                    this._DB.SaveChanges();

        //                }

        //                TempData["jgrowl"] = "物流資訊儲存成功!!";
        //            }
        //            else
        //            {
        //                TempData["jgrowl"] = "請先確定付款完成，在處理物流資料";
        //            }

        //            break;
        //        case 2: //發票資料
        //            if (data.pay_visible && data.flag == 2)
        //            {
        //                data.invoice_datetime = order_data.invoice_datetime;
        //                data.invoice_no = order_data.invoice_no;
        //                //data.invoice_note = order_data.invoice_note;
        //                if (!string.IsNullOrEmpty(data.invoice_no)) { data.flag = 5; }
        //                this._DB.SaveChanges();
        //                TempData["jgrowl"] = "發票資訊儲存成功!!";
        //            }
        //            else
        //            {
        //                TempData["jgrowl"] = "請先確定付款完成或出貨完成，在處理發票資料";
        //            }

        //            break;
        //    }

        //    return RedirectToAction("Edit", new { @id = order_data.order_id });
        //}

        //public ActionResult StayAdd(int id, int yymm, int bills_flag, DateTime sD, DateTime eD)
        //{
        //    order_bills data = new order_bills();
        //    data.bills_datetime = DateTime.Now;
        //    data.invoice_datetime = DateTime.Now;
        //    data.company_id = id;

        //    //sys_admin_data admin = NW.admin(User.Identity.Name);
        //    var data2 = this._DB.order_data.Where(m => m.visible && m.company_id == id && m.flag >= 2 && m.bills_id == 0 && m.back_flag == null && m.goods_datetime >= sD && m.goods_datetime < eD && m.isdelay == false && m.order_id != 7526 && m.order_id != 7527).OrderBy(m => m.goods_datetime).ToList();
        //    ViewData["data2"] = data2;

        //    TempData["sD"] = sD;
        //    TempData["eD"] = eD;
        //    TempData["yymm"] = yymm;
        //    TempData["bills_flag"] = bills_flag;

        //    var company = this._DB.company_data.Where(m => m.company_id == id).FirstOrDefault();
        //    ViewData["company"] = company;

        //    return View(data);
        //}



        #region 顯示用model

        public class data_list
        {
            public int data_id { get; set; }

            public int group_id { get; set; }

            public string title { get; set; }

            public string cont { get; set; }

            public int inx { get; set; }

            public int hits { get; set; }

            public int status_flag { get; set; }

            public DateTime? start_date { get; set; }

            public DateTime? end_date { get; set; }

            public Boolean visible { get; set; }

            public string post_user { get; set; }

            public DateTime? post_datetime { get; set; }

            public string create_user { get; set; }

            public DateTime? create_datetime { get; set; }

            public string change_user { get; set; }

            public DateTime? change_datetime { get; set; }

            public string g_name { get; set; }

            public int counts { get; set; }

            public string url { get; set; }

            public string info { get; set; }

            public string item_no { get; set; }

            public int sprice { get; set; }

            public double cost { get; set; }

            public string gross { get; set; }

            public string s_img { get; set; }

            public string name { get; set; }

            public string tel { get; set; }

            public string mail { get; set; }

            public Boolean index_visible { get; set; }

            public string company_id { get; set; }

            public string company { get; set; }

            public int order_style { get; set; }

            public string item_no2 { get; set; }

            public int companyid { get; set; }

            public int gmid { get; set; }

            public DateTime? search_sd { get; set; }

            public DateTime? search_ed { get; set; }

            public string file_name { get; set; }

        }

        #endregion

        #region 廠商新增請款資料

        [HttpPost, ValidateInput(false)]
        public ActionResult StayAddSave()
        {
            //接收資料
            var year = Request.Form["year2"];
            var month = Request.Form["month2"];
            var y = 0;
            var mon = 0;
            var nation = Request.Form["nation"];

            try
            {
                y = Convert.ToInt32(year);
                mon = Convert.ToInt32(month);
            }
            catch
            {
                TempData["jgrowl"] = "選擇日期解析錯誤，請重新選擇。";
                return RedirectToAction("Month", "NwReport");
            }


            //請款區間類型
            //先判斷該廠商是月結還是半月結
            var aData = NW.admin(NW.DESDeCode(User.Identity.Name));
            var company = this._DB.sys_admin_bills.Where(m => m.admin_id == aData.admin_id && m.visible).FirstOrDefault();
            if (company == null)
            {
                TempData["jgrowl"] = "請先至基本資料填寫匯款資料。";
                return RedirectToAction("Month", "NwReport");
            }

            ////判斷是否於指定期間內請款
            ////0月結 先設定請款區間的隔月1號到10號
            ////1半月結 date 0是16號到25號   1是隔月1號到10號
            ////預設月結
            ////隔月1號
            //var billable_start_date = new DateTime(y, mon, 1).AddMonths(1);
            ////隔月10號
            //var billable_end_date = new DateTime(y, mon, 1).AddMonths(1).AddDays(10);
            ////設定日期
            //var set_date = DateTime.Now;

            //switch (company.bills_flag)
            //{
            //    case 0:
            //        //月結 1-10號
            //        if (set_date >= billable_start_date && set_date < billable_end_date)
            //        {
            //            break;
            //        }
            //        else
            //        {
            //            TempData["jgrowl"] = year + "年" + month + "月請款資料須於隔月1號至10號內建立。";
            //            return RedirectToAction("Month", "NwReport");
            //        }
            //    case 1:
            //        //半月結
            //        //取Request.Form["date"]判斷
            //        switch (Request.Form["date"])
            //        {
            //            case "0":
            //                //16-25號
            //                billable_start_date = new DateTime(y, mon, 16);
            //                billable_end_date = new DateTime(y, mon, 1).AddMonths(1);
            //                if (set_date >= billable_start_date && set_date < billable_end_date)
            //                {
            //                    break;
            //                }
            //                else
            //                {
            //                    TempData["jgrowl"] = year + "年" + month + "月上半月請款資料須於當月16號至月底內建立。";
            //                    return RedirectToAction("Month", "NwReport");
            //                }
            //            case "1":
            //                //隔月1-10號
            //                //billable_start_date = new DateTime(y, mon, 1).AddMonths(1);
            //                //billable_end_date = new DateTime(y, mon, 1).AddMonths(1).AddDays(10);
            //                if (set_date >= billable_start_date && set_date < billable_end_date)
            //                {
            //                    break;
            //                }
            //                else
            //                {
            //                    TempData["jgrowl"] = year + "年" + month + "月下半月請款資料須於隔月1號至10號內建立。";
            //                    return RedirectToAction("Month", "NwReport");
            //                }
            //        }

            //        break;
            //}

            ////檢查是否請款過  判斷條件 廠商id 結算類型 無作廢
            //var check_bills = this._DB.order_bills.Where(m => m.company_id == aData.admin_id && m.bills_flag == company.bills_flag && m.visible).ToList();
            //if (check_bills.FirstOrDefault() != null)
            //{
            //    //月結只有一筆 半月結兩筆
            //    switch (company.bills_flag)
            //    {
            //        case 0:
            //            //月結
            //            var check = check_bills.Where(m => m.sdate == new DateTime(y, mon, 1) && m.edate == new DateTime(y, mon, DateTime.DaysInMonth(y, mon))).FirstOrDefault();
            //            if (check != null)
            //            {
            //                TempData["jgrowl"] = year + "年" + month + "月請款資料已存在，請至待請款列表確認。";
            //                return RedirectToAction("Month", "NwReport");
            //            }
            //            break;
            //        case 1:
            //            //半月結
            //            //取Request.Form["date"]判斷
            //            switch (Request.Form["date"])
            //            {
            //                case "0":
            //                    //1-15號
            //                    check = check_bills.Where(m => m.sdate == new DateTime(y, mon, 1) && m.edate == new DateTime(y, mon, 15)).FirstOrDefault();
            //                    if (check != null)
            //                    {
            //                        TempData["jgrowl"] = year + "年" + month + "月上半月請款資料已存在，請至待請款列表確認。";
            //                        return RedirectToAction("Month", "NwReport");
            //                    }
            //                    break;
            //                case "1":
            //                    //16-月底
            //                    check = check_bills.Where(m => m.sdate == new DateTime(y, mon, 16) && m.edate == new DateTime(y, mon, DateTime.DaysInMonth(y, mon))).FirstOrDefault();
            //                    if (check != null)
            //                    {
            //                        TempData["jgrowl"] = year + "年" + month + "月下半月請款資料已存在，請至待請款列表確認。";
            //                        return RedirectToAction("Month", "NwReport");
            //                    }
            //                    break;
            //            }

            //            break;
            //    }
            //}

            //從報表按完 先儲存基本項 導到編輯頁 讓他新增請款發票資訊
            var bills = new order_bills { };

            bills.company_id = company.admin_id;
            bills.create_user = User.Identity.Name;
            bills.create_datetime = DateTime.Now;
            //請款狀態 
            //0未請款 1處理中 2請款完成
            bills.flag = 0;
            bills.bills_datetime = DateTime.Now;
            bills.yymm = year + month.PadLeft(2, '0');
            bills.bills_flag = company.bills_flag;

            bills.nation = nation;

            //switch (company.bills_flag)
            //{
            //    default:
            //    case 0:
            //        //月結
            //        bills.sdate = new DateTime(y, mon, 1);
            //        bills.edate = new DateTime(y, mon, DateTime.DaysInMonth(y, mon));
            //        break;
            //    case 1:
            //        //半月結
            //        //取Request.Form["date"]判斷
            //        switch (Request.Form["date"])
            //        {
            //            case "0":
            //                //1-15號
            //                bills.sdate = new DateTime(y, mon, 1);
            //                bills.edate = new DateTime(y, mon, 15);
            //                break;
            //            case "1":
            //                //16-月底
            //                bills.sdate = new DateTime(y, mon, 16);
            //                bills.edate = new DateTime(y, mon, DateTime.DaysInMonth(y, mon));
            //                break;
            //        }

            //        break;
            //}


            //當天請款 日期從當月1日到月底
            bills.sdate = new DateTime(y, mon, 1);
            bills.edate = new DateTime(y, mon, DateTime.DaysInMonth(y, mon));



            //儲存請款金額總額(有國別)
            var order = (from m in this._DB.order_data.Where(m => m.nation == nation)
                         join n in this._DB.order_sub on m.order_id equals n.order_id
                         join o in this._DB.order_status on m.order_id equals o.order_id into mo
                         where m.status != 999 && m.company_id == company.admin_id
                         select new
                         {
                             order_id = m.order_id,
                             goods_datetime = mo.Where(k => k.status == 2).Select(k => k.create_datetime).FirstOrDefault(),
                             backed_datetime = mo.Where(k => k.status == 4).Select(k => k.create_datetime).FirstOrDefault(),
                             cost = n.cost,
                             num = n.num,
                             freight = n.freight
                         });
            var edate = bills.edate.Value.AddDays(1);
            var mm = order.Where(m => m.goods_datetime >= bills.sdate && m.goods_datetime < edate && m.backed_datetime == null).ToList();

            if (mm.FirstOrDefault() == null)
            {
                TempData["jgrowl"] = "需請款的訂單資料數目為0，請另外選擇請款時間。";
                return RedirectToAction("Month", "NwReport");
            }

            var new_order_arr = mm.Select(m => m.order_id.ToString()).ToArray();

            //判斷當月單號是否請款過

            //請過的所有單號(有國別)
            var billed_order_arr = new List<string> { };
            var check_bills = this._DB.order_bills.Where(m => m.company_id == aData.admin_id && m.bills_flag == company.bills_flag && m.visible && m.yymm == bills.yymm && m.nation == nation).Select(m => m.order_ids).ToArray();
            foreach (var item in check_bills)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    billed_order_arr.AddRange(item.Split(',').ToList());
                }
            }

            //已經請過的 和新請款的單號交集
            var billed = billed_order_arr.Intersect(new_order_arr).ToArray();

            if (billed.Length > 0)
            {
                //排除請過的
                var ex_billed = new_order_arr.Except(billed).ToArray();

                if (ex_billed.Length <= 0)
                {
                    TempData["jgrowl"] = "需請款的訂單資料皆已申請過。";
                    return RedirectToAction("Month", "NwReport");
                }

                //把過濾過的放進新請款紀錄
                new_order_arr = ex_billed;

                //資料過濾
                mm = mm.Where(m => new_order_arr.Contains(m.order_id.ToString())).ToList();
            }

            //請款單號
            bills.order_ids = String.Join(",", new_order_arr);

            //請款總額
            var total = mm.Select(m => (m.num * m.cost) + m.freight).DefaultIfEmpty(0).Sum();
            bills.total = total;
            bills.visible = true;

            _DB.order_bills.Add(bills);
            this._DB.SaveChanges();

            return RedirectToAction("StayEdit", new { id = bills.bills_id, cid = bills.company_id });
        }

        public ActionResult StayEdit(int id, int cid)
        {


            //登入權限
            var aData = NW.admin(NW.DESDeCode(User.Identity.Name));
            if (aData.admin_id != cid && aData.power_id == 3)
            {
                TempData["jgrowl"] = "並未找到該請款資料。";
                return RedirectToAction("List", "NwBills");
            }

            var data = this._DB.order_bills.Where(m => m.bills_id == id && m.company_id == cid).FirstOrDefault();
            if (data == null)
            {
                TempData["jgrowl"] = "並未找到該請款資料，請至月報表新增。";
                return RedirectToAction("Month", "NwReport");
            }
            //所屬廠商
            ViewBag.company_name = this._DB.sys_admin_data.Where(m => m.admin_id == cid).FirstOrDefault().name_full;

            //請款帳戶資訊
            var sys_bills = this._DB.sys_admin_bills.Where(m => m.admin_id == cid).FirstOrDefault() ?? new sys_admin_bills { };
            ViewData["sys_bills"] = sys_bills;

            //請款狀態

            List<ListItem> ListItems = new List<ListItem> { };
            ListItems = new List<ListItem> { };
            ListItems.Add(new ListItem("0", "儲存資訊", data.flag == 0));
            ListItems.Add(new ListItem("1", "送出請款", data.flag == 1));
            ViewData["flag"] = ListItems;

            var edate = data.edate.Value.AddDays(1);

            //抓取請款資料區間所有訂單
            //var order_status = this._DB.order_status.OrderByDescending(m => m.status_id).Select(m => new { m.status, m.order_id, m.create_datetime }).OrderByDescending(m => m.create_datetime).ToList();
            //var orders = (from a in this._DB.order_data.Where(m => m.status != 999 && m.company_id == cid).Select(m => new { m.order_id, m.company_id, m.send_name, m.nation, m.platform_sn }).ToList()
            //              select new order_data()
            //              {
            //                  order_id = a.order_id,
            //                  company_id = a.company_id,
            //                  send_name = a.send_name,

            //                  order_datetime = order_status.Where(m => m.order_id == a.order_id && m.status == 1).Select(m => m.create_datetime).FirstOrDefault(),
            //                  goods_datetime = order_status.Where(m => m.order_id == a.order_id && m.status == 2).Select(m => m.create_datetime).FirstOrDefault(),
            //                  backed_datetime = order_status.Where(m => m.order_id == a.order_id && m.status == 4).Select(m => m.create_datetime).FirstOrDefault(),

            //                  nation = a.nation,
            //                  platform_sn = a.platform_sn

            //              }).Where(k => !k.backed_datetime.HasValue && k.goods_datetime >= data.sdate && k.goods_datetime < edate).ToList();
            try
            {
                //以該紀錄的單號作依據
                var order_id_arr = data.order_ids.Split(',');
                var order_status = this._DB.order_status.OrderByDescending(m => m.status_id).Select(m => new { m.status, m.order_id, m.create_datetime }).OrderByDescending(m => m.create_datetime).ToList();
                var orders = (from a in this._DB.order_data.Where(m => order_id_arr.Contains(m.order_id.ToString())).ToList()
                              select new order_data()
                              {
                                  order_id = a.order_id,
                                  order_sn = a.order_sn,
                                  company_id = a.company_id,
                                  send_name = a.send_name,

                                  order_datetime = order_status.Where(m => m.order_id == a.order_id && m.status == 1).Select(m => m.create_datetime).FirstOrDefault(),
                                  goods_datetime = order_status.Where(m => m.order_id == a.order_id && m.status == 2).Select(m => m.create_datetime).FirstOrDefault(),
                                  backed_datetime = order_status.Where(m => m.order_id == a.order_id && m.status == 4).Select(m => m.create_datetime).FirstOrDefault(),

                                  nation = a.nation,
                                  platform_sn = a.platform_sn

                              }).OrderBy(m => m.order_id).ToList();

                ViewData["orders"] = orders;

                var orders_arr = orders.Select(m => m.order_id).ToArray();
                ViewBag.orders_all = this._DB.order_sub.Where(m => orders_arr.Contains(m.order_id)).Select(m => (m.num * m.cost) + m.freight).DefaultIfEmpty(0).Sum();
            }
            catch
            {
                ViewData["orders"] = new List<order_data> { };
                ViewBag.orders_all = 0;
            }

            ViewBag.jgrowl = TempData["jgrowl"];

            return View(data);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult StayEditSave(order_bills order_bills)
        {
            //該筆資料
            var bills = this._DB.order_bills.Where(m => m.bills_id == order_bills.bills_id).FirstOrDefault();
            if (bills == null)
            {
                TempData["jgrowl"] = "並未找到該請款資料。";
                return RedirectToAction("List", "NwBills");
            }

            //判斷是否為自己的請款資料  管理者跳過
            var aData = NW.admin(NW.DESDeCode(User.Identity.Name));
            if (aData.admin_id != bills.company_id && aData.power_id == 3)
            {
                TempData["jgrowl"] = "並未找到該請款資料。";
                return RedirectToAction("List", "NwBills");
            }

            //點送出請款卻沒填的話導回
            if (order_bills.flag == 1 && (order_bills.invoice_datetime == null || string.IsNullOrEmpty(order_bills.invoice_no)))
            {
                TempData["jgrowl"] = "請填寫發票資訊。";
                return RedirectToAction("StayEdit", "NwBills", new { id = order_bills.bills_id, cid = bills.company_id });
            }

            bills.invoice_datetime = order_bills.invoice_datetime;
            bills.invoice_no = order_bills.invoice_no;
            bills.flag = order_bills.flag;
            bills.change_user = User.Identity.Name;
            bills.change_datetime = DateTime.Now;

            this._DB.SaveChanges();

            return RedirectToAction("StayEdit", "NwBills", new { id = order_bills.bills_id, cid = bills.company_id });
        }

        //請款列表 廠商看的
        public ActionResult StayList(string yymm, int page = 0, string nation = "TW")
        {
            int currentPageIndex = page != 0 ? page - 1 : 0;

            int[] viewable_flag = { 0, 1, 2, 3 };
            var Linq = this._DB.order_bills.Where(m => viewable_flag.Contains(m.flag) && m.nation == nation);

            //登入權限
            var aData = NW.admin(NW.DESDeCode(User.Identity.Name));
            switch (aData.power_id)
            {
                case 1:
                case 2:
                    //管理者 可看到全部
                    break;
                case 3:
                    //廠商 只能看到自己的
                    Linq = Linq.Where(m => m.company_id == aData.admin_id && m.visible);
                    break;
                default:
                    TempData["jgrowl"] = "您的權限不足。";
                    return RedirectToAction("Page", "NwAbout");
            }

            //列表選擇請款月份
            ViewBag.yymm_list = Linq.Select(m => m.yymm).Distinct().OrderByDescending(m => m).ToArray();
            ViewBag.yymm = yymm;

            //國別
            ViewBag.nation = nation;

            //過濾
            if (!string.IsNullOrEmpty(yymm))
            {
                Linq = Linq.Where(m => m.yymm == yymm);
            }

            ////過濾
            //if (start_date.HasValue)
            //{
            //    Linq = Linq.Where(m => m.bills_datetime >= start_date);
            //}

            //if (end_date.HasValue)
            //{
            //    var edate = end_date.Value.AddDays(1);
            //    //Linq = Linq.Where();
            //}

            Linq = Linq.OrderByDescending(m => m.create_datetime);

            IPagedList data = Linq.ToPagedList(currentPageIndex, 20);



            ViewBag.jgrowl = TempData["jgrowl"];

            if (page != 0)
            {
                return PartialView(RouteData.Values["action"].ToString() + "_ajax", data);
            }
            else
            {
                return View(data);
            }
        }

        //作廢用
        [HttpPost]
        public ActionResult StayAdd(int cb_flag, string cb_id)
        {

            string sql = "";
            switch (cb_flag)
            {
                case 1: //作廢
                    sql = "Update order_bills set visible = 0 Where bills_id in (" + cb_id + ")";
                    break;
            }

            if (sql != "")
            {
                _DB.Database.ExecuteSqlCommand(sql);
            }
            TempData["jgrowl"] = "請款單作廢成功。";

            return RedirectToAction("StayList");

        }

        #endregion

        #region 請款中

        //請款中列表
        public ActionResult List(string yymm, string kw, int page = 0, int flag = 1, string nation = "TW")
        {

            int currentPageIndex = page != 0 ? page - 1 : 0;

            //列表上面選單顯示(有國別)   名稱,數量
            var pills = new Dictionary<string, KeyValuePair<int, int>> { };
            for (int i = 0; i < 4; i++)
            {
                pills.Add(NW.bills_flag(i), new KeyValuePair<int, int>(i, this._DB.order_bills.Where(m => m.flag == i && m.nation == nation).Count()));
            }

            ViewData["pills"] = pills;

            //國別
            ViewBag.nation = nation;

            var viewable_flag = new int[] { 1 };
            switch (flag)
            {
                case 0:
                    //待請款
                    viewable_flag = new int[] { 0 };
                    break;
                case 1:
                    //請款中
                    break;
                case 2:
                    //確認請款
                    viewable_flag = new int[] { 2 };
                    break;
                case 3:
                    //請款完成
                    viewable_flag = new int[] { 3 };
                    break;

            }
            var Linq = this._DB.order_bills.Where(m => viewable_flag.Contains(m.flag) && m.nation == nation);

            //登入權限 只有管理者能看到
            var aData = NW.admin(NW.DESDeCode(User.Identity.Name));
            switch (aData.power_id)
            {
                case 2:
                    //管理者 可看到全部
                    break;
                default:
                    TempData["jgrowl"] = "您的權限不足。";
                    return RedirectToAction("Page", "NwAbout");
            }

            //列表選擇請款月份
            ViewBag.yymm_list = Linq.Select(m => m.yymm).Distinct().OrderByDescending(m => m).ToArray();
            ViewBag.yymm = yymm;


            //過濾
            if (!string.IsNullOrEmpty(yymm))
            {
                Linq = Linq.Where(m => m.yymm == yymm);
            }


            if (!string.IsNullOrEmpty(kw))
            {
                try
                {
                    int[] kw_arr = this._DB.sys_admin_data.Where(m => m.name_full.Contains(kw) || m.name.Contains(kw)).Select(m => m.admin_id).ToArray();
                    Linq = Linq.Where(m => kw_arr.Contains(m.company_id));

                    ViewBag.kw = kw;
                }
                catch { }
            }

            Linq = Linq.OrderByDescending(m => m.create_datetime);

            IPagedList data = Linq.ToPagedList(currentPageIndex, 20);

            ViewBag.jgrowl = TempData["jgrowl"];
            ViewBag.flag = flag;

            if (page != 0)
            {
                return PartialView(RouteData.Values["action"].ToString() + "_ajax", data);
            }
            else
            {
                return View(data);
            }
        }

        public ActionResult Edit(int id, int cid)
        {

            //登入權限 只限管理者觀看
            var aData = NW.admin(NW.DESDeCode(User.Identity.Name));
            if (aData.power_id != 2)
            {
                TempData["jgrowl"] = "並未找到該請款資料。";
                return RedirectToAction("List", "NwBills");
            }

            var data = this._DB.order_bills.Where(m => m.bills_id == id && m.company_id == cid).FirstOrDefault();
            if (data == null)
            {
                TempData["jgrowl"] = "並未找到該請款資料，請至月報表新增。";
                return RedirectToAction("Month", "NwReport");
            }
            //所屬廠商
            ViewBag.company_name = this._DB.sys_admin_data.Where(m => m.admin_id == cid).FirstOrDefault().name_full;

            //請款帳戶資訊
            var sys_bills = this._DB.sys_admin_bills.Where(m => m.admin_id == cid).FirstOrDefault() ?? new sys_admin_bills { };
            ViewData["sys_bills"] = sys_bills;

            //請款狀態

            List<ListItem> ListItems = new List<ListItem> { };
            ListItems = new List<ListItem> { };
            //以當前資料狀態判斷
            switch (data.flag)
            {
                case 1:
                    ListItems.Add(new ListItem("1", "儲存資訊", data.flag == 1));
                    ListItems.Add(new ListItem("2", "確認請款", data.flag == 2));
                    break;
                case 2:
                    ListItems.Add(new ListItem("2", "儲存資訊", data.flag == 2));
                    ListItems.Add(new ListItem("3", "匯款完成", data.flag == 3));
                    break;
            }
            
            ViewData["flag"] = ListItems;

            var edate = data.edate.Value.AddDays(1);

            //抓取請款資料區間所有訂單
            //var order_status = this._DB.order_status.OrderByDescending(m => m.status_id).Select(m => new { m.status, m.order_id, m.create_datetime }).OrderByDescending(m => m.create_datetime).ToList();
            //var orders = (from a in this._DB.order_data.Where(m => m.status != 999 && m.company_id == cid).Select(m => new { m.order_id, m.company_id, m.send_name, m.nation, m.platform_sn }).ToList()
            //              select new order_data()
            //              {
            //                  order_id = a.order_id,
            //                  company_id = a.company_id,
            //                  send_name = a.send_name,

            //                  order_datetime = order_status.Where(m => m.order_id == a.order_id && m.status == 1).Select(m => m.create_datetime).FirstOrDefault(),
            //                  goods_datetime = order_status.Where(m => m.order_id == a.order_id && m.status == 2).Select(m => m.create_datetime).FirstOrDefault(),
            //                  backed_datetime = order_status.Where(m => m.order_id == a.order_id && m.status == 4).Select(m => m.create_datetime).FirstOrDefault(),

            //                  nation = a.nation,
            //                  platform_sn = a.platform_sn

            //              }).Where(k => !k.backed_datetime.HasValue && k.goods_datetime >= data.sdate && k.goods_datetime < edate).ToList();
            try
            {
                var order_id_arr = data.order_ids.Split(',');
                var order_status = this._DB.order_status.OrderByDescending(m => m.status_id).Select(m => new { m.status, m.order_id, m.create_datetime }).OrderByDescending(m => m.create_datetime).ToList();
                var orders = (from a in this._DB.order_data.Where(m => order_id_arr.Contains(m.order_id.ToString())).ToList()
                              select new order_data()
                              {
                                  order_id = a.order_id,
                                  order_sn = a.order_sn,
                                  company_id = a.company_id,
                                  send_name = a.send_name,

                                  order_datetime = order_status.Where(m => m.order_id == a.order_id && m.status == 1).Select(m => m.create_datetime).FirstOrDefault(),
                                  goods_datetime = order_status.Where(m => m.order_id == a.order_id && m.status == 2).Select(m => m.create_datetime).FirstOrDefault(),
                                  backed_datetime = order_status.Where(m => m.order_id == a.order_id && m.status == 4).Select(m => m.create_datetime).FirstOrDefault(),

                                  nation = a.nation,
                                  platform_sn = a.platform_sn

                              }).OrderBy(m => m.order_id).ToList();

                ViewData["orders"] = orders;

                var orders_arr = orders.Select(m => m.order_id).ToArray();
                ViewBag.orders_all = this._DB.order_sub.Where(m => orders_arr.Contains(m.order_id)).Select(m => (m.num * m.cost) + m.freight).DefaultIfEmpty(0).Sum();
            }
            catch
            {
                ViewData["orders"] = new List<order_data> { };
                ViewBag.orders_all = 0;
            }

            ViewBag.jgrowl = TempData["jgrowl"];

            return View(data);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EditSave(order_bills order_bills)
        {
            //該筆資料
            var bills = this._DB.order_bills.Where(m => m.bills_id == order_bills.bills_id).FirstOrDefault();
            if (bills == null)
            {
                TempData["jgrowl"] = "並未找到該請款資料。";
                return RedirectToAction("List", "NwBills");
            }

            //判斷是否為自己的請款資料  管理者跳過
            var aData = NW.admin(NW.DESDeCode(User.Identity.Name));
            if (aData.power_id != 2)
            {
                TempData["jgrowl"] = "無此權限。";
                return RedirectToAction("List", "NwBills");
            }

            bills.flag = order_bills.flag;
            bills.change_user = User.Identity.Name;
            bills.change_datetime = DateTime.Now;
            if (order_bills.flag > 1)
            {
                //完成請款
                bills.finish_datetime = order_bills.finish_datetime;
            }

            this._DB.SaveChanges();

            return RedirectToAction("Edit", "NwBills", new { id = order_bills.bills_id, cid = bills.company_id });
        }

        #endregion

        #region Export

        public ActionResult BillsExport(int id)
        {

            //判斷是否為自己的請款資料  管理者會跳過
            var aData = NW.admin(NW.DESDeCode(User.Identity.Name));

            //該筆請款單
            var bills = this._DB.order_bills.Where(m => m.bills_id == id).FirstOrDefault();

            if (aData.admin_id != bills.company_id && aData.power_id == 3)
            {
                TempData["jgrowl"] = "並未找到該請款資料。";
                return RedirectToAction("StayList", "NwBills");
            }

            //該廠商
            var company = this._DB.sys_admin_data.Where(m => m.admin_id == bills.company_id).FirstOrDefault();

            XLWorkbook workbook = new XLWorkbook();

            var arr = bills.order_ids.Split(',');

            //var data = (from m in this._DB.order_data
            //            join n in this._DB.order_sub on m.order_id equals n.order_id
            //            join o in this._DB.order_status on m.order_id equals o.order_id into mo
            //            join p in this._DB.order_goods on m.goods_id equals p.goods_id into mp
            //            from p in mp.DefaultIfEmpty()
            //            where arr.Contains(m.order_id.ToString())
            //            select new
            //            {
            //                order_sn = m.order_sn,
            //                platform = m.platform_id,
            //                order_datetime = mo.Where(s => s.status == 1 && s.status_old == 0).Select(s => s.create_datetime).FirstOrDefault(),
            //                item_name = n.item_name,
            //                item_no = n.item_no,
            //                style = n.style,
            //                num = n.num,
            //                cost = n.cost,
            //                freight = n.freight,
            //                total = n.cost * n.num,
            //                goods_datetime = mo.Where(g => g.status == 2).Select(s => s.create_datetime).FirstOrDefault(),
            //                goods_name = mp.FirstOrDefault() == null ? "" : mp.FirstOrDefault().name,
            //                goods_info = mo.Where(g => g.status == 2).Select(s => s.info).FirstOrDefault(),
            //                info = mo.Where(s => s.status == 1 && s.status_old == 0).Select(s => s.info).FirstOrDefault()
            //            });
            var data = (from m in this._DB.order_data
                        join n in this._DB.order_sub on m.order_id equals n.order_id
                        join o in this._DB.order_status on m.order_id equals o.order_id into mo
                        //join p in this._DB.order_goods on m.goods_id equals p.goods_id into mp
                        //from p in mp.DefaultIfEmpty()
                        join q in this._DB.order_invoice on m.order_id equals q.order_id into mq
                        from q in mq.DefaultIfEmpty()
                        where arr.Contains(m.order_id.ToString())
                        select new
                        {

                            platform = m.platform_id,
                            order_datetime = mo.Where(s => s.status == 1 && s.status_old == 0).Select(s => s.create_datetime).FirstOrDefault(),
                            order_sn = m.order_sn,
                            order_name = m.order_name,
                            item_name = n.item_name,
                            item_no = n.item_no,
                            style = n.style,
                            num = n.num,
                            cost = n.cost,
                            total = (n.cost * n.num),
                            freight = n.freight,
                            freight_datetime = mo.Where(s => s.status == 2 && s.status_old == 1).Select(s => s.create_datetime).FirstOrDefault(),
                            invoice_datetime = mq.FirstOrDefault() == null ? null : mq.FirstOrDefault().create_datetime,
                            invoice_no = mq.FirstOrDefault() == null ? "" : mq.FirstOrDefault().no

                        });

            var name = bills.sdate.Value.ToString("MMdd") + "_" + bills.edate.Value.ToString("MMdd") + company.name + "請款訂單匯出";

            var sheet = workbook.Worksheets.Add(name);

            int colIdx = 1;
            int rowIdx = 2;

            var cols = "訂單編號,平台,購買日期,商品名稱,商品貨號,樣式,數量,單價,運費,總計,出貨日期,物流公司,物流單號,出貨備註".Split(',');
            cols = "平台,購買日期,出貨日期,訂單編號,訂購人,商品名稱,商品貨號,樣式,數量,單價,請款金額,發票日期,發票號碼".Split(',');
            //if (aData.power_id == 2)
            //{
            //    cols = "平台,購買日期,訂單編號,訂購人,商品名稱,商品貨號,樣式,數量,單價,請款金額,平台售出金額,利潤,發票日期,發票號碼".Split(',');
            //}

            foreach (string colName in cols)
            {
                sheet.Cell(1, colIdx).Style.Fill.BackgroundColor = XLColor.GreenYellow;
                sheet.Cell(1, colIdx++).Value = colName;
            }

            //防重複
            var recorded = new List<string> { };

            foreach (var item in data)
            {
                if (recorded.Contains(item.order_sn))
                {
                    continue;
                }
                //紀錄
                recorded.Add(item.order_sn);

                sheet.Cell(rowIdx, 1).Value = NW.platform(item.platform);
                try
                {
                    sheet.Cell(rowIdx, 2).Value = item.order_datetime.Value.ToString("yyyy/MM/dd");
                }
                catch { }
                try
                {
                    sheet.Cell(rowIdx, 3).Value = item.freight_datetime.Value.ToString("yyyy/MM/dd");
                }
                catch { }
                sheet.Cell(rowIdx, 4).Value = item.order_sn;
                sheet.Cell(rowIdx, 5).Value = item.order_name;
                sheet.Cell(rowIdx, 6).Value = item.item_name;
                sheet.Cell(rowIdx, 7).Value = item.item_no;
                sheet.Cell(rowIdx, 8).Value = item.style;
                sheet.Cell(rowIdx, 9).Value = item.num;
                sheet.Cell(rowIdx, 10).Value = item.cost;
                sheet.Cell(rowIdx, 11).Value = item.total + item.freight;
                //if (aData.power_id == 3)
                //{
                try
                {
                    sheet.Cell(rowIdx, 12).Value = item.invoice_datetime.Value.ToString("yyyy/MM/dd");
                }
                catch { }
                sheet.Cell(rowIdx, 13).Value = item.invoice_no;
                //}
                //else if (aData.power_id == 2)
                //{
                //    try
                //    {
                //        sheet.Cell(rowIdx, 11).Value = item.invoice_datetime.Value.ToString("yyyy/MM/dd");
                //    }
                //    catch { }
                //    sheet.Cell(rowIdx, 12).Value = item.invoice_no;
                //}

                rowIdx++;
            }

            sheet.Cell(rowIdx, 10).Style.Fill.BackgroundColor = XLColor.IndianYellow;
            sheet.Cell(rowIdx, 10).Value = "總額：" + bills.total;

            for (int i = 1; i <= cols.Length; i++)
            {
                sheet.Column(i).AdjustToContents();
            }


            MemoryStream ms = new MemoryStream();
            workbook.SaveAs(ms);
            workbook.Dispose();

            return File(ms.ToArray(), "application/vnd.ms-excel", name + ".xlsx");
        }

        #endregion

    }
}

