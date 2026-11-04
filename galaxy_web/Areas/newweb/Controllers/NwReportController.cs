using galaxy_web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace galaxy_web.Areas.newweb.Controllers
{
    public class NwReportController : _BaseController
    {
        //
        // GET: /newweb/NwReport/

        public class _Month
        {
            public order_data order_data { get; set; }
            public order_sub order_sub { get; set; }
            //public product_data product_data { get; set; }
            public sys_admin_data company { get; set; }
        }

        
        public ActionResult Month(int? year, int? month, string kw, int? cid, int type = 0, string nation = "TW")
        {
            ViewBag.jgrowl = TempData["jgrowl"];

            List<ListItem> ListItems = new List<ListItem>() { };

            string msg = string.Empty;

            //先決定時間
            if (!year.HasValue) { year = NW.Today.Year; }
            if (!month.HasValue) { month = NW.Today.Month; }

            while (true)
            {
                if (!year.HasValue) { msg = "請填寫年份"; break; }
                if (!month.HasValue) { msg = "請填寫月份"; break; }
                ViewBag.year = year; //年份
                ViewBag.month = month; //月份

                var order_status = this._DB.order_status.OrderByDescending(m => m.status_id).Select(m => new { m.status, m.order_id, m.create_datetime }).OrderByDescending(m => m.create_datetime).ToList();

                //沒作廢的
                var oData = (from a in this._DB.order_data.Where(m => m.status != 999).Select(m => new { m.order_id, m.company_id, m.send_name, m.nation, m.platform_sn }).ToList()
                             select new order_data()
                             {
                                 order_id = a.order_id,
                                 company_id = a.company_id,
                                 send_name = a.send_name,

                                 order_datetime = order_status.Where(m => m.order_id == a.order_id && m.status == 1).Select(m => m.create_datetime).FirstOrDefault(),
                                 goods_datetime = order_status.Where(m => m.order_id == a.order_id && m.status == 2).Select(m => m.create_datetime).FirstOrDefault(),
                                 backed_datetime = order_status.Where(m => m.order_id == a.order_id && m.status == 4).Select(m => m.create_datetime).FirstOrDefault(),

                                 nation = a.nation,
                                 platform_sn = a.platform_sn

                             }).ToList();

                var order_sub = this._DB.order_sub.ToList();
                var company_data = this._DB.sys_admin_data.ToList();

                var linq = (from a in oData
                            select new _Month()
                            {
                                order_data = a,
                                order_sub = order_sub.Where(m => m.order_id == a.order_id).FirstOrDefault(),
                                company = company_data.Where(m => m.admin_id == a.company_id).FirstOrDefault(),

                            });

                //權限
                var aData = NW.admin(NW.DESDeCode(User.Identity.Name));
                if (aData.power_id > 2)
                {
                    ViewBag.isC = true;
                    linq = linq.Where(m => m.order_data.company_id == aData.admin_id);

                    #region 請款顯示用
                   //改在addsave控制儲存 按鈕都會顯示
                    var bill = this._DB.sys_admin_bills.Where(m => m.admin_id == aData.admin_id).FirstOrDefault();
                    if (bill != null)
                    {
                        ViewBag.bills_flag = bill.bills_flag;
                    }
                    else {
                        ViewBag.bills_flag = 0;
                    }

                    #endregion
                }

                #region"國籍判斷"

                ViewBag.nation = nation;
                nation = nation.ToLower();
                linq = linq.Where(m => m.order_data.nation.ToLower() == nation);

                #endregion

                #region"供應商"

                var Company = company_data.Where(m => m.visible && m.power_id == 3).OrderBy(m => m.name.Length).Select(m => m.admin_id + "," + m.name).ToArray();

                string c_n = cid.HasValue ? cid.ToString() : null;
                ListItems.AddRange(Gs.sList(Company, c_n, "全部"));
                ViewData["Company"] = ListItems;

                if (cid.HasValue)
                {
                    ViewBag.cid = cid;
                    linq = linq.Where(m => m.company.admin_id == cid);

                }

                #endregion

                if (!string.IsNullOrEmpty(kw))
                {
                    ViewBag.kw = kw;
                    kw = kw.ToLower();

                    linq = linq.Where(m =>
                        m.order_sub.item_no.ToLower().Contains(kw)
                        || m.order_sub.item_name.ToLower().Contains(kw)
                        || m.order_data.send_name.ToLower().Contains(kw)
                        );
                }


                DateTime sD = new DateTime(year.Value, month.Value, 1);
                DateTime eD = new DateTime(sD.AddMonths(1).Year, sD.AddMonths(1).Month, 1);

                //找出選擇月份的出貨單
                var Month = linq.Where(m => m.order_data.goods_datetime >= sD && m.order_data.goods_datetime < eD).ToLookup(m => m.order_data.backed_datetime.HasValue);

                //有退貨-當月訂當月退的
                var Y = Month[true].ToList();
                ViewData["Back"] = Y;
                ViewBag.Back_num = Y.Count();

                //沒退貨
                var N = Month[false].ToList();
                ViewData["Goods"] = N;
                ViewBag.Goods_num = N.Count();

                //上個月的退貨單
                eD = sD;
                sD = sD.AddMonths(-1);

                var LMonth = linq.Where(m => m.order_data.goods_datetime >= sD && m.order_data.goods_datetime < eD && m.order_data.backed_datetime.HasValue).ToList();
                ViewData["LBack"] = LMonth;
                ViewBag.LBack_num = LMonth.Count();

                if (type == 1)
                {
                    IEnumerable<object>[] linqs = { N, LMonth, Y };
                    return Export(linqs);
                }

                break;
            }


            return View();
        }

        public ActionResult CNMonth(int? year, int? month, string kw, int? cid, int type = 0)
        {
            return Month(year, month, kw, cid, type, "CN");
        }

        public ActionResult SampleMonth(int? year, int? month, string kw, int? cid, int type = 0)
        {
            return Month(year, month, kw, cid, type, "TS");
        }

        public ActionResult Export(IEnumerable<object> objects)
        {
            string[] arr = { "當月 - 出貨", "上個月 - 退貨", "當月 - 退貨單" };
            Dictionary<string, DataTable> dt = new Dictionary<string, DataTable>() { };
            foreach (var item in objects)
            {
                int num = Array.IndexOf(objects.ToArray(), item);

                var linq = item as IEnumerable<_Month>;
                //foreach (var item2 in linq) 
                //{
                //    decimal cost = (item2.order_sub.cost + item2.order_sub.freight);
                //}

                // + m.order_sub.freight + m.order_sub.freight + m.order_sub.freight + m.order_sub.freight

                var data = linq.Select(m => new
                {

                    供應商 = m.company.name,
                    訂單單號 = m.order_data.platform_sn,
                    訂購日期 = m.order_data.order_datetime,
                    出貨日期 = m.order_data.goods_datetime,
                    寄送方 = m.order_data.send_name,
                    商品名稱 = m.order_sub.item_name,
                    商品貨號 = m.order_sub.item_no,
                    數量 = m.order_sub.num,
                    成本 = (m.order_sub.cost).ToString(NW.point),
                    總計 = ((m.order_sub.cost) * m.order_sub.num).ToString(NW.point),
                    未稅 = (decimal.Round(((m.order_sub.cost + m.order_sub.freight) * m.order_sub.num) / Convert.ToDecimal(1.05))).ToString(NW.point),
                    稅金 = (((m.order_sub.cost) * m.order_sub.num) - decimal.Round(((m.order_sub.cost) * m.order_sub.num) / Convert.ToDecimal(1.05))).ToString(NW.point)
                });

                dt.Add(arr[num], Gs.ConvertObjectsToDataTable(data));
            }

            string fileName = string.Format("{0}{1}.xlsx", NW.Now.ToString("yyyyMMddHHmmss"), NW.GetUniqueKey());


            var r = Gs.ExportExcelFromDataTable(dt);
            return File(r.Value.ToArray(), "application/vnd.ms-excel", fileName);
        }

        public class _Ranking
        {
            public int company_id { get; set; }
            public string company_name { get; set; }
            public string nation { get; set; }
            public string item_name { get; set; }
            public string item_no { get; set; }
            public int pay_id { get; set; }
            public int num { get; set; }
            public decimal cost { get; set; }
            public decimal erp_cost { get; set; }
            public decimal price { get; set; }
            public DateTime? goods_datetime { get; set; }
            public DateTime? back_datetime { get; set; }

        }

        //public ActionResult Ranking(int? year, int? month, string kw, int? cid, string nation = "TW", int type = 0)
        public ActionResult Ranking(DateTime? sD, DateTime? eD, string kw, int? cid, string nation = "TW", int type = 0)
        {
            ViewBag.jgrow = TempData["jgrow"];

            List<ListItem> ListItems = new List<ListItem>() { };

            string msg = "";

            //先決定時間
            sD = sD ?? new DateTime(NW.Today.Year, NW.Today.Month, 1);
            eD = eD ?? NW.Today;


            while (true)
            {

                var order_status = this._DB.order_status.OrderByDescending(m => m.status_id).Select(m => new { m.status, m.order_id, m.create_datetime }).ToList();

                var linq = from a in this._DB.order_data.Where(m => m.status != 999).Select(m => new { m.company_id, m.order_id, m.nation }).ToList()
                           join b in this._DB.order_sub on a.order_id equals b.order_id
                           join c in this._DB.sys_admin_data on a.company_id equals c.admin_id
                           join d in this._DB.product_data on b.product_id equals d.product_id
                           select new _Ranking()
                           {
                               company_id = a.company_id,
                               nation = a.nation,
                               item_name = b.item_name,
                               item_no = b.item_no,
                               pay_id = b.pay_id, //支付方式
                               num = b.num,
                               cost = b.cost, //訂購當時的成本 運費 + b.freight
                               company_name = c.name,
                               erp_cost = d.erp_cost,
                               price = d.price,
                               goods_datetime = order_status.Where(m => m.order_id == a.order_id && m.status == 2).Select(m => m.create_datetime).FirstOrDefault(),
                               back_datetime = order_status.Where(m => m.order_id == a.order_id && m.status == 4).Select(m => m.create_datetime).FirstOrDefault(),  
                           };

                //沒有退貨
                linq = linq.Where(m => !m.back_datetime.HasValue);



                //權限
                var aData = NW.admin(NW.DESDeCode(User.Identity.Name));
                if (aData.power_id > 2)
                {
                    ViewBag.isC = true;
                    linq = linq.Where(m => m.company_id == aData.admin_id);
                }

                #region"國籍判斷"

                ViewBag.nation = nation;
                nation = nation.ToLower();
                linq = linq.Where(m => m.nation.ToLower() == nation);

                #endregion

                #region"供應商"

                var Company = this._DB.sys_admin_data.Where(m => m.visible && m.power_id == 3).OrderBy(m => m.name.Length)
                    .Select(m => m.admin_id + "," + m.name).ToArray();

                string c_n = cid.HasValue ? cid.ToString() : null;
                ListItems.AddRange(Gs.sList(Company, c_n, "請選擇：廠商"));
                ViewData["Company"] = ListItems;

                if (cid.HasValue)
                {
                    ViewBag.cid = cid;
                    linq = linq.Where(m => m.company_id == cid);
                }


                #endregion

                #region"kw"

                if (!string.IsNullOrEmpty(kw))
                {
                    ViewBag.kw = kw;
                    kw = kw.ToLower();

                    linq = linq.Where(m => m.item_no.ToLower().Contains(kw) || m.item_name.ToLower().Contains(kw));
                }

                #endregion

                #region"sD eD"

                if (sD.HasValue && eD.HasValue)
                {
                    ViewBag.sD = NW.getDate(sD);
                    ViewBag.eD = NW.getDate(eD);

                    eD = eD.Value.AddDays(1);

                    //找出選擇月份的出貨單
                    linq = linq.Where(m => m.goods_datetime >= sD && m.goods_datetime < eD);
                }

                #endregion

                var xx = linq.OrderByDescending(m => m.num).GroupBy(m => new { m.item_no }).Select(m => new _Ranking()
                {
                    company_id = m.FirstOrDefault().company_id,
                    company_name = m.FirstOrDefault().company_name,
                    nation = m.FirstOrDefault().nation,
                    item_name = m.FirstOrDefault().item_name,
                    item_no = m.FirstOrDefault().item_no,
                    price = m.FirstOrDefault().price,
                    cost = m.FirstOrDefault().cost,
                    erp_cost = m.FirstOrDefault().erp_cost,
                    pay_id = m.FirstOrDefault().pay_id,
                    num = m.Select(x => x.num).DefaultIfEmpty(0).Sum()
                }).OrderBy(m => m.company_name.Length).ToList();

                var data = this._DB.order_pay.Where(m => m.visible).OrderBy(m => m.pay_id).ToDictionary(m => m.pay_name, m => xx.Where(x => x.pay_id == m.pay_id).ToList());

                //var data = xx.GroupBy(m => m.pay_id).OrderBy(m => m.Key).ToDictionary(m => Pay[m.Key], m => m.ToList());

                ViewData["Rank"] = data;

                if (type == 1)
                {
                    return RankExport(data);
                }

                break;
            }

            return View();
        }

        public ActionResult RankExport(Dictionary<string, List<_Ranking>> objects)
        {
            Dictionary<string, DataTable> dt = new Dictionary<string, DataTable>() { };

            foreach (var item in objects)
            {
                var linq = item.Value as IEnumerable<_Ranking>;
                var data = linq.Select(m => new
                {
                    供應商 = m.company_name,
                    商品名稱 = m.item_name,
                    商品貨號 = m.item_no,
                    數量 = m.num,
                    商城售價 = m.price.ToString(NW.point),
                    銷售總計 = (m.num * m.price).ToString(NW.point),
                    單位成本 = m.cost.ToString(NW.point),
                    成本總計 = (m.num * m.cost).ToString(NW.point),
                    翻購成本 = m.erp_cost.ToString(NW.point),
                    營收毛利 = ((m.erp_cost - m.cost) * m.num).ToString(NW.point),
                    毛利率 = (((m.price - m.cost) / m.price) * 100).ToString("F2") + "%"
                });

                dt.Add(item.Key, Gs.ConvertObjectsToDataTable(data));
            }


            string fileName = string.Format("{0}{1}.xlsx", NW.Now.ToString("yyyyMMddHHmmss"), NW.GetUniqueKey());
            var r = Gs.ExportExcelFromDataTable(dt);
            return File(r.Value.ToArray(), "application/vnd.ms-excel", fileName);
        }

    }
}

