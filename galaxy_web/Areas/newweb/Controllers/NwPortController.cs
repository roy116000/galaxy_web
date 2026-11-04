using ClosedXML.Excel;
using galaxy_web.Models;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace galaxy_web.Areas.newweb.Controllers
{
    public class NwPortController : _BaseController
    {
        public ActionResult List()
        {
            ViewBag.jgrowl = TempData[NW.ssjgrowl];

            var data = this._DB.port_data.Where(m => m.visible).OrderByDescending(m => m.create_datetime).Take(10).ToList();

            //權限
            var aData = NW.admin(NW.DESDeCode(User.Identity.Name));
            ViewBag.power = aData.power_id;

            return View(data);
        }

        [HttpPost]
        public ActionResult Edit(string name, HttpPostedFileBase Im)
        {
            string msg = "";
            if (string.IsNullOrWhiteSpace(name))
            {
                msg = "解析失敗";
                goto Error;
            }

            if (Im == null)
            {
                msg = "請選擇檔案";
                goto Error;
            }

            switch (name)
            {
                case "匯入訂單":

                    Order(Im);

                    break;

                case "匯入消費者發票":

                    Inv_cus_import(Im);

                    break;
            }


            return RedirectToAction("List");

        Error:

            TempData[NW.ssjgrowl] = msg;
            return RedirectToAction("List");
        }

        #region 匯入消費者發票用

        public void Inv_cus_import(HttpPostedFileBase Iminv)
        {
            if (Iminv != null && Iminv.ContentLength > 0 && NW.getFileType(Iminv, 3))
            {
                //辯別是2007還是2003
                //IWorkbook excel = WorkbookFactory.Create(Iminv.InputStream);

                //NPOI.SS.UserModel.ISheet sheet = excel.GetSheetAt(0);

                //資料
                var order_inv_cus = this._DB.order_invoice.Where(m => m.type == 1).ToList();
                var order_data = this._DB.order_data.Where(m => m.status >= 2 && m.status != 999).ToList();


                var result = new Dictionary<int, string> { };
                var workbook = new XLWorkbook(Iminv.InputStream);
                var ws1 = workbook.Worksheet(1);

                //取得所有的標題
                Dictionary<int, string> Heads = ws1.Row(1).CellsUsed().ToDictionary(m => m.Address.ColumnNumber - 1, m => m.Value.ToString());
                var heads = new string[] { "ERP單號", "消費者發票日期", "消費者發票號碼" };
                //回傳需要抓的col位置 與名稱
                foreach (var item in Heads)
                {
                    var this_item = heads.Where(m => m == item.Value).FirstOrDefault();
                    if (this_item != null)
                    {
                        result.Add(item.Key + 1, this_item);
                    }
                }

                //for (int row = 1; row <= ws1.RangeUsed().Rows().Skip(1).Count(); row++) // 使用For 走訪所有的資料列
                foreach (var item in ws1.RangeUsed().Rows().Skip(1))
                {
                    try
                    {
                        var sn = item.Cell(result.Where(m => m.Value == "ERP單號").Select(m => m.Key).FirstOrDefault()).Value.ToString();
                        //該筆sn
                        //var sn = sheet.GetRow(row).GetCell(result.Where(m => m.Value == "ERP單號").Select(m => m.Key).FirstOrDefault()).ToString();
                       

                        //判斷是不是已經有消費者發票紀錄
                        var oid = order_data.Where(m => m.order_sn == sn).Select(m => m.order_id).ToArray();
                        var inv_cus_data = order_inv_cus.Where(m => oid.Contains(m.order_id)).FirstOrDefault();
                        if (inv_cus_data == null && oid.Length > 0)
                        {
                            //沒填會因無法轉換而跳catch

                            var new_inv_cus = new order_invoice { };
                            new_inv_cus.order_id = oid.FirstOrDefault();
                            new_inv_cus.attribution_id = 0;
                            new_inv_cus.attribution = 0;
                            new_inv_cus.flag = 0;
                            //new_inv_cus.no = sheet.GetRow(row).GetCell(result.Where(m => m.Value == "消費者發票號碼").Select(m => m.Key).FirstOrDefault()).ToString();
                            new_inv_cus.no = item.Cell(result.Where(m => m.Value == "消費者發票號碼").Select(m => m.Key).FirstOrDefault()).Value.ToString();
                            //new_inv_cus.create_datetime = Convert.ToDateTime(sheet.GetRow(row).GetCell(result.Where(m => m.Value == "消費者發票日期").Select(m => m.Key).FirstOrDefault()));
                            new_inv_cus.create_datetime = Convert.ToDateTime(item.Cell(result.Where(m => m.Value == "消費者發票日期").Select(m => m.Key).FirstOrDefault()).Value);
                            new_inv_cus.create_user = User.Identity.Name;
                            new_inv_cus.type = 1;

                            this._DB.order_invoice.Add(new_inv_cus);
                        }
                    }
                    catch { 
                        
                    }

                }

                this._DB.SaveChanges();
            }

        }

        #endregion

        public enum Title
        {
            平台編號 = 1, 平台單號, ERP單號, 訂購方姓名, 訂購方電話, 訂購方郵遞區號, 訂購方地址, 寄送方姓名, 寄送方電話, 寄送方郵遞區號,
            寄送方地址, 訂購時間, 出貨時間, 物流單號, 物流廠商, 退貨時間, 退貨完成時間, 商品貨號, 商品數量, 發票日期, 發票號碼, 備註, 收件時段, 國籍, 付款方式
        }

        public ActionResult Order(HttpPostedFileBase Im)
        {
            List<Dictionary<string, string>> dy = new List<Dictionary<string, string>>() { };

            int num = 0, success_num = 0, error_num = 0;
            string msg = null;
            string br = "<br />";

            //本次需要的欄位
            string[] TT = Enum.GetNames(typeof(Title));
            KeyValuePair<string, List<Dictionary<string, string>>> getRows = Gs.getRows(Im, TT);

            //本次總比數
            num = getRows.Value.Count();

            if (!string.IsNullOrEmpty(getRows.Key))
            {
                //Excel欄位不完全
                msg = getRows.Key;

                dy.AddRange(getRows.Value);
            }
            else
            {

                #region"寫入資料庫"

                //這些為非必填
                string[] noCheck = { 
                            Title.ERP單號.ToString(), Title.退貨時間.ToString(), Title.出貨時間.ToString(),  Title.退貨完成時間.ToString(), 
                            Title.物流單號.ToString(), Title.物流廠商.ToString(), Title.發票日期.ToString(),  Title.發票號碼.ToString(), 
                            Title.訂購方地址.ToString(), Title.備註.ToString() };


                List<Dictionary<string, string>> Rows = getRows.Value;
                foreach (var item in Rows)
                {
                    bool r = false;

                    while (true)
                    {

                        //本次是Excel的第幾列
                        int i = Array.IndexOf(Rows.ToArray(), item) + 2;

                        //尋找商品
                        string Erp_no = item.Where(m => m.Key == Title.商品貨號.ToString()).Select(m => m.Value).FirstOrDefault();
                        var Product = this._DB.product_data.Where(m => m.erp_no == Erp_no).SingleOrDefault();
                        if (Product == null) { msg += br + "第" + i + "列：" + "查無此商品"; break; };

                        #region"訂單部分"

                        //平台單號(不可以重複)
                        string platform_sn = item.Where(m => m.Key == Title.平台單號.ToString()).Select(m => m.Value).FirstOrDefault();
                        var data = this._DB.order_data.Where(m => m.platform_sn == platform_sn).SingleOrDefault() ?? new order_data() { };

                        #region"組裝對應"

                        Dictionary<string, string> Correspond = new Dictionary<string, string>()
                        { 
                            { Title.平台編號.ToString(), "platform_id"},
                            { Title.平台單號.ToString(), "platform_sn" }, 
                            { Title.訂購方姓名.ToString(), "order_name" },
                            { Title.訂購方電話.ToString(), "order_tel" },
                            { Title.訂購方郵遞區號.ToString(), "order_zip" },
                            { Title.訂購方地址.ToString(), "order_address" },
                            { Title.寄送方姓名.ToString(), "send_name" },
                            { Title.寄送方電話.ToString(), "send_tel" },
                            { Title.寄送方郵遞區號.ToString(), "send_zip" },
                            { Title.寄送方地址.ToString(), "send_address" },
                            { Title.訂購時間.ToString(), "create_datetime" },
                            { Title.國籍.ToString(), "nation"}
                        };

                        KeyValuePair<string, object> GetTable = Gs.getTable(data, Correspond, item, noCheck);
                        if (!string.IsNullOrWhiteSpace(GetTable.Key)) { msg += br + "第" + i + "列：" + GetTable.Key; break; }

                        #endregion

                        data = GetTable.Value as order_data;

                        //Erp_sn
                        if (string.IsNullOrEmpty(data.order_sn))
                        {
                            string Sn = getSn();
                            if (string.IsNullOrEmpty(Sn)) { msg += br + "第" + i + "列：" + "無法產出Erp_sn"; break; }

                            data.order_sn = Sn;
                        }

                        data.company_id = Product.company_id;
                        data.create_user = User.Identity.Name;
                        data.create_datetime = data.create_datetime ?? NW.Now;
                        data.order_address = Gs.getAddress(data.order_address, data.order_zip); //訂購人地址
                        data.send_address = Gs.getAddress(data.send_address, data.send_zip); //寄送人地址

                        if (data.order_id == 0)
                        {
                            this._DB.order_data.Add(data);
                        }

                        this._DB.SaveChanges();

                        #endregion

                        #region"明細部分"

                        var Sub = this._DB.order_sub.Where(m => m.order_id == data.order_id).FirstOrDefault();
                        Sub = Sub != null ? Sub : new order_sub() { };

                        #region"組裝對應"

                        Correspond = new Dictionary<string, string>()
                        { 
                            { Title.商品貨號.ToString(), "item_no" }, 
                            { Title.商品數量.ToString(), "num" },             
                            { Title.備註.ToString(), "info" },
                            { Title.付款方式.ToString(), "pay_id"}
                        };

                        GetTable = Gs.getTable(Sub, Correspond, item);

                        if (!string.IsNullOrWhiteSpace(GetTable.Key)) { msg += br + "第" + i + "列：" + GetTable.Key; break; }

                        #endregion

                        Sub = GetTable.Value as order_sub;
                        Sub.order_id = data.order_id;
                        Sub.cost = Product.cost;
                        Sub.price = Product.erp_cost;
                        Sub.freight = Product.freight;
                        Sub.item_name = Product.item_name;
                        Sub.product_id = Product.product_id;


                        if (Sub.sub_id == 0)
                        {
                            this._DB.order_sub.Add(Sub);
                        }

                        this._DB.SaveChanges();

                        #endregion

                        #region"時間部分"

                        Dictionary<int, KeyValuePair<string, string>> Date = new Dictionary<int, KeyValuePair<string, string>>() { };
                        Date.Add(1, new KeyValuePair<string, string>(Title.訂購時間.ToString(), item[Title.訂購時間.ToString()]));
                        //Date.Add(2, new KeyValuePair<string, string>(Title.出貨時間.ToString(), item[Title.出貨時間.ToString()]));
                        Date.Add(3, new KeyValuePair<string, string>(Title.退貨時間.ToString(), item[Title.退貨時間.ToString()]));
                        Date.Add(4, new KeyValuePair<string, string>(Title.退貨完成時間.ToString(), item[Title.退貨完成時間.ToString()]));
                        //Date.Add(5, new KeyValuePair<string, string>(Title.發票日期.ToString(), item[Title.發票日期.ToString()]));

                        foreach (var item2 in Date)
                        {
                            string Da = item2.Value.Value;
                            if (!string.IsNullOrEmpty(Da))
                            {
                                //當次狀態
                                int Status = item2.Key;
                                try
                                {
                                    DateTime D = Convert.ToDateTime(Da);
                                    var L = this._DB.order_status.Where(m => m.order_id == data.order_id && m.status == Status).FirstOrDefault() ?? new order_status() { };
                                    L.order_id = data.order_id;
                                    L.status = Status;
                                    L.create_user = User.Identity.Name;
                                    L.create_datetime = D;

                                    switch (Status)
                                    {
                                        case 1:
                                            L.info = (item[Title.收件時段.ToString()] + "," + item[Title.備註.ToString()]).Trim(',');
                                            break;
                                        case 2:
                                            L.info = (item[Title.物流廠商.ToString()] + "," + item[Title.物流單號.ToString()]).Trim(',');
                                            break;
                                        case 5:
                                            L.info = item[Title.發票號碼.ToString()];

                                            #region"新增發票"

                                            var I = this._DB.order_invoice.Where(m => m.order_id == data.order_id).FirstOrDefault() ?? new order_invoice() { };
                                            I.order_id = data.order_id;
                                            I.flag = 1;
                                            I.no = item[Title.發票號碼.ToString()];
                                            I.create_user = User.Identity.Name;
                                            I.create_datetime = D;

                                            if (I.invoice_id == 0)
                                            {
                                                this._DB.order_invoice.Add(I);
                                            }

                                            #endregion

                                            break;
                                    }

                                    if (L.status_id == 0)
                                    {
                                        L.status_old = this._DB.order_status.Where(m => m.order_id == data.order_id).OrderByDescending(m => m.create_datetime).Select(m => m.status).FirstOrDefault();
                                        this._DB.order_status.Add(L);
                                    }
                                    this._DB.SaveChanges();
                                }
                                catch
                                {
                                    msg += br + "第" + i + "列：" + item2.Value.Key + "，內容有誤";
                                }

                            }
                        }

                        //訂單的最後狀態
                        data.status = this._DB.order_status.Where(m => m.order_id == data.order_id).OrderByDescending(m => m.status).Select(m => m.status).FirstOrDefault();
                        this._DB.SaveChanges();

                        #endregion


                        #region"退貨"

                        ////最後的狀態
                        //var Last_S = this._DB.order_status.Where(m => m.order_id == data.order_id).OrderByDescending(m => m.status_id).FirstOrDefault();
                        //Last_S = Last_S != null ? Last_S : new order_status() { };


                        ////申請退貨
                        //var backdatetime = item.Where(m => m.Key == Title.退貨時間.ToString()).Select(m => m.Value).FirstOrDefault();
                        //if (!string.IsNullOrEmpty(backdatetime) && Last_S.status < 3)
                        //{


                        //    //是否完成退貨了
                        //    var backdedatetime = item.Where(m => m.Key == Title.退貨完成時間.ToString()).Select(m => m.Value).FirstOrDefault();
                        //    if (string.IsNullOrEmpty(backdedatetime))
                        //    {
                        //        try
                        //        {
                        //            DateTime back = DateTime.Parse(backdatetime);
                        //            data.status = 3;
                        //        }
                        //        catch
                        //        {
                        //            msg += br + "第" + i + "列：" + "退貨時間格式有誤，故先無法退貨"; break;
                        //        }
                        //    }
                        //}
                        ////else
                        ////{
                        ////    data.status = Last_S.status == 0 ? 1 : Last_S.status;
                        ////}

                        //if (Last_S.status == 0 || Last_S.status < data.status)
                        //{
                        //    Last_S.status = data.status;
                        //    Last_S.order_id = data.order_id;
                        //    Last_S.create_user = User.Identity.Name;
                        //    Last_S.create_datetime = NW.Now;

                        //    if (Last_S.status_id == 0)
                        //    {
                        //        this._DB.order_status.Add(Last_S);
                        //    }
                        //    else
                        //    {

                        //        var B = this._DB.order_status.Where(m => m.order_id == data.order_id).OrderByDescending(m => m.status_id).FirstOrDefault();
                        //        if (B != null)
                        //        {
                        //            Last_S.status_old = B.status;
                        //        }
                        //    }
                        //}

                        //this._DB.SaveChanges();

                        #endregion

                        //成功數量統計
                        r = true;
                        success_num += 1;


                        break;
                    }

                    if (!r)
                    {
                        dy.Add(item);
                    }
                }

                #endregion

            }

            #region"建立本次匯入紀錄"

            if (!string.IsNullOrEmpty(msg))
            {
                msg = msg.Trim(new char[] { '<', 'b', 'r', ' ', '/', '>' });
            }

            port_data port_data = new port_data()
            {
                name = "訂單資訊",
                type = "匯入",
                num = num,
                success_num = success_num,
                error_num = (num - success_num),
                error_msg = msg,
                visible = true,
                create_user = User.Identity.Name,
                create_datetime = NW.Now
            };

            this._DB.port_data.Add(port_data);
            this._DB.SaveChanges();

            #endregion

            return RedirectToAction("List");

            //if (dy.Count() > 0)
            //{
            //    return getError(dy);
            //}
            //else
            //{
            //    return RedirectToAction("List");
            //}


        }

        //將錯誤的再轉成一次Excel
        private ActionResult getError(List<Dictionary<string, string>> dy)
        {
            DataTable Table = new DataTable() { };

            Dictionary<string, string> haeders = dy.ElementAt(0);
            foreach (var colHead in haeders)
            {
                Table.Columns.Add(colHead.Key);
            }

            foreach (var row in dy)
            {
                DataRow dataRow = Table.NewRow();
                foreach (var col in row)
                {

                    dataRow[col.Key] = col.Value;
                }
                Table.Rows.Add(dataRow);
            }

            //IEnumerable<object> data = ;

            Dictionary<string, DataTable> dt = new Dictionary<string, DataTable>() { { "錯誤資訊", Table } };
            string fileName = string.Format("{0}{1}.xlsx", NW.Now.ToString("yyyyMMddHHmmss"), NW.GetUniqueKey());

            var r = Gs.ExportExcelFromDataTable(dt);
            return File(r.Value.ToArray(), "application/vnd.ms-excel", fileName);


        }

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
    }
}

