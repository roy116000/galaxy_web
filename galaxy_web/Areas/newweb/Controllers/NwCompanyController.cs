using galaxy_web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcPaging;
using System.IO;
using System.Data;

namespace galaxy_web.Areas.newweb.Controllers
{
    public class NwCompanyController : _BaseController
    {
        //
        // GET: /newweb/NwCompany/

        public ActionResult List(int? page, string kw, DateTime? sD, DateTime? eD, int type = 0, int sort = 0, int visible = 0)
        {
            ViewBag.jgrow = TempData[NW.ssjgrowl];

            var linq = this._DB.sys_admin_data.Where(m => m.admin_id > 0 && m.power_id == 3);

            #region"條件"

            #region"關鍵字"

            if (!string.IsNullOrEmpty(kw))
            {
                ViewBag.kw = kw;
                kw = kw.ToLower();

                linq = linq.Where(m => m.name.ToLower().Contains(kw) || m.name_full.ToLower().Contains(kw) || m.erp_sn.ToLower().Contains(kw) || m.sn.ToLower().Contains(kw));
            }

            #endregion

            #region"時間"

            //eD = eD.HasValue ? eD : NW.Today;
            //sD = sD.HasValue ? sD : new DateTime(NW.Today.Year, NW.Today.Month, 1);
            //ViewBag.eD = NW.getDate(eD);
            //eD = eD.Value.AddDays(1);

            //linq = linq.Where(m => (m.create_datetime >= sD && m.create_datetime < eD) || (m.change_datetime >= sD && m.change_datetime < eD));


            if (sD.HasValue)
            {
                ViewBag.sD = NW.getDate(sD);

                linq = linq.Where(m => m.create_datetime >= sD || m.change_datetime >= sD);
            }

            if (eD.HasValue)
            {
                ViewBag.eD = NW.getDate(eD);
                eD = eD.Value.AddDays(1);

                linq = linq.Where(m => m.create_datetime < eD || m.change_datetime < eD);
            }


            #endregion

            #region"啟用狀態"

            switch (visible)
            {
                case 1:
                    linq = linq.Where(m => m.visible);
                    break;
                case 2:
                    linq = linq.Where(m => !m.visible);
                    break;
            }

            ViewBag.visible = visible;

            #endregion

            #endregion

            if (type == 1)
            {
                return Export(linq);
            }

            int currentPageIndex = page.HasValue && page.Value > 0 ? page.Value - 1 : 0;

            //排序
            switch (sort)
            { 
                case 0:
                    linq = linq.OrderBy(m => m.erp_sn);
                    break;
                case 1:
                    linq = linq.OrderByDescending(m => m.erp_sn);
                    break;
            }

            ViewBag.sort = sort;
            IPagedList data = linq.ToPagedList(currentPageIndex, 20);

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
            ViewBag.jgrowl = TempData["jgrowl"];
            string msg = "";

            var data = this._DB.sys_admin_data.Where(m => m.admin_id == id && m.power_id == 3).SingleOrDefault();
            data = data != null ? data : new sys_admin_data() { };

            //請款資料
            var bills = this._DB.sys_admin_bills.Where(m => m.admin_id == id && m.visible).FirstOrDefault();
            ViewData["bills"] = bills ?? new sys_admin_bills { };

            //權限
            var aData = NW.admin(NW.DESDeCode(User.Identity.Name));
            ViewBag.aPower = aData.power_id;

            #region"暫存置換"

            var temp = TempData["temp"] as sys_admin_data;
            if (temp != null)
            {
                data = temp;
            }

            TempData.Remove("temp");

            #endregion

            while (true)
            {
                //新增
                if (data.admin_id == 0)
                {
                    if (id != 0) { msg = "查無資訊"; break; }
                    else
                    {
                        data.visible = true;
                        data.start_date = NW.Now;
                        data.end_date = NW.Now.AddYears(10).AddDays(-1);
                    }
                }

                break;
            }

            #region"選單"

            List<ListItem> ListItems = new List<ListItem>() { };

            //等級
            string[] G = this._DB.sys_admin_group.Where(m => m.visible).OrderBy(m => m.inx).Select(m => m.group_id + "," + m.name + " - ( " + m.limit + "筆 )").ToArray();
            ListItems = new List<ListItem>() { };
            ListItems.AddRange(Gs.sList(G, data.group_id.ToString(), "請選擇"));
            ViewData["group_id"] = ListItems;

            //郵遞區號
            var Zip = Gs.getZip(data.zip.ToString());
            ViewData["zip"] = Zip.Key;
            ViewData["zip_chained"] = Zip.Value;

            //郵遞區號2
            var Zip2 = Gs.getZip(data.zip2.ToString());
            ViewData["zip2"] = Zip2.Key;
            ViewData["zip2_chained"] = Zip2.Value;

            //銀行
            var bank = this._DB2.sys_bank_data.Where(m => m.visible).ToList();
//
            ViewData["bank"] = bank.Where(m => m.parent_id == 0).Select(m => m.name + "," + m.bank_id).Distinct().OrderBy(m => m.Substring(0, 1)).ToList();

            ViewData["code"] = bank.Where(m => m.parent_id != 0).Select(m => m.name + "," + m.parent_id + "," + m.code).Distinct().ToList();
            //該廠商銀行的資料
            var bank_data = this._DB.sys_admin_bills.Where(m => m.admin_id == id).Select(m => m.receiving_code).FirstOrDefault();
            if (bank_data != null)
            {
                var b = bank.Where(m => m.code == bank_data).FirstOrDefault();
                if (b != null)
                {
                    ViewBag.model_bank = b.parent_id + "," + b.code + "," + b.name;
                }
            }

            //該廠商等級
            var level = this._DB.sys_admin_group.Where(m => m.group_id == data.group_id).FirstOrDefault();
            if (level != null)
            {
                ViewBag.level = String.Format("{0}(商品上傳限制:{1}筆)", level.name, level.limit);
            }
            else {
                ViewBag.level = "無";
            }
            #endregion

            var Controller = RouteData.Values["controller"].ToString().ToLower().Replace("nw", "");

            #region"圖片"


            data.Img = new List<string>() { };

            var Imgx = this._DB.sys_admin_image.Where(m => m.admin_id == data.admin_id).OrderByDescending(m => m.admin_id);

            foreach (var item in Enumerable.Range(1, 6))
            {
                var Img = Imgx.Where(m => m.group_id == item).SingleOrDefault();
                Img = Img != null ? Img : new sys_admin_image() { };

                string ImgPath = Gs.Path_Img(Img.img, Controller);

                data.Img.Add(ImgPath);
            }

            #endregion

            #region"PDF"

            string tmpPath = NW.WEB_URL + "Content/pdf/" + Controller + "/" + data.admin_id; //存放路徑

            var PDF = this._DB.sys_admin_image.Where(m => m.group_id == 99 && m.admin_id == data.admin_id)
                .OrderBy(m => m.inx).ThenByDescending(m => m.img_id).ToDictionary(m => tmpPath + "/" + m.img, m => m.img);
            ViewData["PDF"] = PDF;

            #endregion

            TempData["jgrowl"] = msg;
            if (!string.IsNullOrEmpty(msg))
            {

                return RedirectToAction(RouteData.Values["action"].ToString().Replace("Add", "List").Replace("Edit", "List"));
            }
            else
            {
                return View(data);
            }
        }

        public ActionResult Edit(int id)
        {
            return Add(id);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EditSave(sys_admin_data Get, sys_admin_bills sys_admin_bills, HttpPostedFileBase[] Img, HttpPostedFileBase[] PDF)
        {

            TempData["temp"] = Get;

            string msg = "";

            while (true)
            {
                if (Get.admin_id == 0)
                {
                    if (string.IsNullOrEmpty(Get.pwdx) || string.IsNullOrEmpty(Get.pwdy)) { msg = "密碼不可為空值"; break; }
                    if (Get.pwdx.Length < 4 || Get.pwdy.Length < 4) { msg = "密碼需大於4碼"; break; }
                    if (Get.pwdx != Get.pwdy) { msg = "密碼驗證錯誤"; break; }

                    var isUid = this._DB.sys_admin_data.Where(m => m.uid == Get.sn).SingleOrDefault();
                    if (isUid != null) { msg = "此帳號已被註冊，請洽詢管理員"; break; }
                }

                var data = this._DB.sys_admin_data.Where(m => m.admin_id == Get.admin_id).SingleOrDefault();
                if (data == null && Get.admin_id != 0) { msg = "查無相關資訊"; break; }

                #region"AOE"

                data = data != null ? data : new sys_admin_data() { };

                //權限判斷
                var aData = NW.admin(NW.DESDeCode(User.Identity.Name));
                if (aData.power_id < 3)
                {
                    data.group_id = Get.group_id;
                    data.sn = Get.sn;

                    if (string.IsNullOrEmpty(data.erp_sn))
                    {
                        data.erp_sn = getNg("TU");
                        if (data.erp_sn == "超出限制") { msg = data.erp_sn; break; };
                    }

                    data.uid = Get.sn; //統編等於帳號
                    data.pwd = Get.pwdx;
                    data.visible = Get.visible;
                    data.note_visible = Get.note_visible;
                    data.start_date = Get.start_date;
                    data.end_date = Get.end_date;
                    data.introduce_uid = Get.introduce_uid;
                    data.company_uid = Get.company_uid;
                    data.name = Get.name;
                    data.name_full = Get.name_full;
                }



                data.power_id = 3;

                data.info = Get.info;
                data.zip = Get.zip;
                data.address = Get.address;
                data.zip2 = Get.zip2;
                data.address2 = Get.address2;
                data.tel = Get.tel;
                data.tel2 = Get.tel2;
                data.fax = Get.fax;
                data.mail = Get.mail;
                data.boss = Get.boss;
                data.boss_tel = Get.boss_tel;
                data.contact = Get.contact;
                data.contact_tel = Get.contact_tel;
                data.contact2 = Get.contact2;
                data.contact2_tel = Get.contact2_tel;
                data.contact3 = Get.contact3;
                data.contact3_tel = Get.contact3_tel;
                data.last_ip = Gs.getIPAddress;
                data.last_datetime = NW.Now;

                if (data.admin_id == 0)
                {
                    data.create_user = User.Identity.Name;
                    data.create_datetime = NW.Now;

                    this._DB.sys_admin_data.Add(data);

                    msg = "新增完成";
                }
                else
                {
                    data.change_user = User.Identity.Name;
                    data.change_datetime = NW.Now;

                    msg = "修改成功";
                }

                this._DB.SaveChanges();

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

                            var ImgO = this._DB.sys_admin_image.Where(m => m.admin_id == data.admin_id && m.visible && m.group_id == num).OrderByDescending(m => m.img_id).FirstOrDefault();
                            if (ImgO != null)
                            {

                                string ImgO_tmpPath = Gs.Path_Img(ImgO.img, Controller);
                                ImgO_tmpPath = Server.MapPath(ImgO_tmpPath);

                                //找到圖檔
                                if (System.IO.File.Exists(ImgO_tmpPath))
                                {
                                    I_name = ImgO.img;

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
                                var Rebmp = new Gs._Rebmp { Img = item, tmpPath = tmpPath, resizedWidth = 350, name = I_name };    //內頁主圖大小 
                                mm = Gs.Rebmp(Rebmp);
                            }
                            catch
                            {
                                msg += "第" + num + "張圖片上傳失敗，請洽詢管理員";
                                break;
                            }

                            //如果有找到舊資料
                            if (!string.IsNullOrEmpty(I_name))
                            {
                                ImgO.change_user = User.Identity.Name;
                                ImgO.change_datetime = NW.Now;
                            }
                            else
                            {
                                var img_data = new sys_admin_image()
                                {
                                    img = mm,
                                    group_id = num,
                                    admin_id = data.admin_id,
                                    visible = true,
                                    create_user = User.Identity.Name,
                                    create_datetime = NW.Now
                                };

                                this._DB.sys_admin_image.Add(img_data);
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

                #region"PDF"

                foreach (var item in PDF)
                {
                    int num = Array.IndexOf(PDF, item) + 1;

                    if (item != null && item.ContentLength > 0 && NW.getFileType(item, 2)) //有無上傳
                    {

                        var Controller = RouteData.Values["controller"].ToString().ToLower().Replace("nw", "");

                        string tmpPath = Server.MapPath("~/Content/pdf/") + Controller + "/" + data.admin_id; //存放路徑
                        if (!Directory.Exists(tmpPath)) { Directory.CreateDirectory(tmpPath); } //建資料夾

                        string p_name = tmpPath + "/" + item.FileName;
                        item.SaveAs(p_name);

                        var pdf = this._DB.sys_admin_image.Where(m => m.img == item.FileName).FirstOrDefault();
                        if (pdf != null)
                        {
                            pdf.change_user = User.Identity.Name;
                            pdf.change_datetime = NW.Now;
                        }
                        else
                        {
                            var sys_admin_image = new sys_admin_image
                            {
                                img = item.FileName,
                                group_id = 99,
                                inx = num,
                                admin_id = data.admin_id,
                                visible = true,
                                create_user = User.Identity.Name,
                                create_datetime = NW.Now
                            };

                            this._DB.sys_admin_image.Add(sys_admin_image);
                        }

                        this._DB.SaveChanges();
                    }
                }

                #endregion

                #region 請款資料

                //以接收的receiving_code抓取其他銀行資料
                var bank = this._DB2.sys_bank_data.Where(m => m.parent_id != 0 && m.code == sys_admin_bills.receiving_code).FirstOrDefault();
                if (bank == null)
                {
                    msg = "查無此銀行分行"; break;
                }

                sys_admin_bills.bank = this._DB2.sys_bank_data.Where(m => m.bank_id == bank.parent_id).Select(m => m.name).FirstOrDefault();
                sys_admin_bills.bank2 = bank.name;

                var admin_bill = this._DB.sys_admin_bills.Where(m => m.admin_id == data.admin_id && m.visible).FirstOrDefault();

                if (admin_bill != null)
                {
                    //修改
                    //admin_bill.visible = true;
                    //admin_bill.admin_id = data.admin_id;
                    admin_bill.bills_flag = sys_admin_bills.bills_flag;
                    admin_bill.receiving_name = sys_admin_bills.receiving_name;
                    admin_bill.receiving_account = sys_admin_bills.receiving_account;
                    admin_bill.receiving_code = sys_admin_bills.receiving_code;
                    admin_bill.bank = sys_admin_bills.bank;
                    admin_bill.bank2 = sys_admin_bills.bank2;
                    admin_bill.change_user = User.Identity.Name;
                    admin_bill.change_datetime = NW.Now;
                }
                else {
                    //新增
                    var new_bill = new sys_admin_bills { };

                    new_bill.visible = true;
                    new_bill.admin_id = data.admin_id;
                    new_bill.bills_flag = sys_admin_bills.bills_flag;
                    new_bill.receiving_name = sys_admin_bills.receiving_name;
                    new_bill.receiving_account = sys_admin_bills.receiving_account;
                    new_bill.receiving_code = sys_admin_bills.receiving_code;
                    new_bill.bank = sys_admin_bills.bank;
                    new_bill.bank2 = sys_admin_bills.bank2;
                    new_bill.create_user = User.Identity.Name;
                    new_bill.create_datetime = NW.Now;

                    this._DB.sys_admin_bills.Add(new_bill);
                }

                this._DB.SaveChanges();

                #endregion

                Get.admin_id = data.admin_id;

                TempData.Remove("temp");

                break;
            }

            TempData["jgrowl"] = msg;
            string action = RouteData.Values["action"].ToString().Replace("Save", "");

            return RedirectToAction(action, new { id = Get.admin_id });


        }

        //傳抬頭
        public string getNg(string Get)
        {
            string r = string.Empty;

            var M = this._DB.sys_admin_data.Where(m => m.erp_sn.StartsWith(Get)).Select(m => m.erp_sn).ToArray();

            int Max = M.Length > 0 ? M.Select(m => int.Parse(m.Split(new string[] { Get }, StringSplitOptions.None)[1])).Max() : 0;

            if (Max >= 9999 || Max < 0)
            {
                r = "超出限制";
            }
            else
            {
                r = Get + (Max + 1).ToString().PadLeft(5, '0');
            }

            return r;
        }



        #region"匯出"

        public ActionResult Export(IEnumerable<object> objects)
        {

            var data = (from a in objects as IEnumerable<sys_admin_data>
                        join b in this._DB.zip_data on a.zip equals b.zip into mm
                        join c in this._DB.zip_data on a.zip2 equals c.zip into mm2
                        select new
                        {

                            廠商代號 = a.erp_sn,
                            簡稱 = a.name,
                            公司全名 = a.name_full,
                            統一編號 = a.sn,
                            TEL一 = a.tel,
                            TEL二 = a.tel2,
                            FAXNO = a.fax,
                            E_MAIL = a.mail,
                            負責人 = a.boss,
                            聯絡人一 = a.contact,
                            聯絡人二 = a.contact2,
                            聯絡人三 = a.introduce_uid,
                            廠商騰云編號 = a.company_uid,
                            備註 = a.info,
                            郵遞區號一 = a.zip,
                            聯絡地址 = mm.Select(m => m.city + m.canton).FirstOrDefault() + a.address,
                            聯絡地址二 = mm2.Select(m => m.city + m.canton).FirstOrDefault() + a.address2,
                            郵遞區號二 = a.zip2,
                            帳單地址 = "",
                            帳單地址二 = ""

                        });



            Dictionary<string, DataTable> dt = new Dictionary<string, DataTable>() { { "廠商資訊", Gs.ConvertObjectsToDataTable(data) } };
            var r = Gs.ExportExcelFromDataTable(dt);

            string fileName = string.Format("{0}{1}.xlsx", NW.Now.ToString("yyyyMMddHHmmss"), NW.GetUniqueKey());
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

    }
}
