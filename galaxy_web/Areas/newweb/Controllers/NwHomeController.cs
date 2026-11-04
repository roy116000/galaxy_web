using galaxy_web.Models;
using galaxy_web.Models2;
//System.Drawing.Imaging
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Drawing;//Bitmap
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace galaxy_web.Areas.newweb.Controllers
{

    public class NwHomeController : Controller
    {
        private _dbContext _db_;
        protected _dbContext _DB
        {
            get
            {
                if (_db_ == null)
                {
                    _db_ = new _dbContext();
                }
                return _db_;
            }
        }

        private _dbContext2 _db2_;
        protected _dbContext2 _DB2
        {
            get
            {
                if (_db2_ == null)
                {
                    _db2_ = new _dbContext2();
                }
                return _db2_;
            }
        }

        public ActionResult Index()
        {
            //var msg = string.Empty;
            //var all = this._DB.product_erp_sub.ToList();
            //foreach (var item in all)
            //{
            //    var g = this._DB.product_erp_sub.Where(m => m.group_id == item.group_id && m.auto_sn == item.auto_sn).Count();
            //    if (g > 1)
            //    {
            //        msg += item.group_id + "|" + item.auto_sn + ",";
            //    }
            //}
            //var dd = msg;

            ////原本號稱可以清除所有 Cookie 的方法...
            //FormsAuthentication.SignOut();
            ////清除所有的 session
            //Session.RemoveAll();

            ViewBag.Msg = TempData["loginErr"];
            return View();
        }

        #region "登出/登入/驗證碼"

        //登入
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string uid, string pwd, string code, string ReturnUrl)
        {
            string id = "", controller = "", action = "";
            string mycode = "", msg = "";

            while (true)
            {
                //判斷登入次數 超過6次 鎖

                #region"先記錄登入資料"

                sys_admin_login L = new sys_admin_login()
                {
                    uid = uid,
                    pwd = pwd,
                    code = code,
                    ip = Gs.getIPAddress,
                    device = Gs.getDevice,
                    login_datetime = NW.Now,
                    isSuccess = false
                };

                this._DB.sys_admin_login.Add(L);
                this._DB.SaveChanges();

                #endregion

                if (Session["myCode"] != null) { mycode = Session["myCode"].ToString(); Session.Remove("myCode"); }
                if (code != mycode) { msg = "驗證碼錯誤"; break; }

                //找人
                var data = this._DB.sys_admin_data.Where(m => m.visible && m.uid == uid && m.pwd == pwd).SingleOrDefault();
                if (data == null) { msg = "帳號或密碼錯誤"; break; }

                //登入紀錄-加密
                id = NW.DESEnCode(data.admin_id.ToString());
                //FormsAuthentication.SetAuthCookie(id, true);

                FormsAuthentication.SignOut();
                FormsAuthentication.SetAuthCookie(id, false, FormsAuthentication.FormsCookiePath);

                ////改發Ticket的方式
                //FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, id, DateTime.Now, DateTime.Now.AddMinutes(30), false, id, FormsAuthentication.FormsCookiePath);
                // Encrypt the ticket.
                //string encTicket = FormsAuthentication.Encrypt(ticket);
                //// Create the cookie.
                //Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));


                //記錄人
                L.create_user = id;
                L.create_datetime = NW.Now;
                this._DB.SaveChanges();

                if (!string.IsNullOrEmpty(ReturnUrl))
                {
                    return Redirect(ReturnUrl);
                }
                else
                {
                    //權限
                    var Power = this._DB.sys_power_data.Where(m => m.power_id == data.power_id).FirstOrDefault();
                    if (Power != null)
                    {
                        controller = Power.controller;
                        action = Power.action;
                    }

                    //當天登入次數
                    int L_Today = this._DB.sys_admin_login.Where(m => m.uid == uid && m.login_datetime >= NW.Today && m.login_datetime < NW.Tomorrow).Count();
                    if (L_Today < 2)
                    {
                        TempData["Alert"] = true;
                    }

                    return RedirectToAction(action, controller);
                }


            }

            TempData["loginErr"] = msg;
            return RedirectToAction("Index", "NwHome");

        }

        //登出
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "NwHome");
        }

        //驗證碼
        public FileResult code(string myCode = "myCode")
        {
            //string[] Code ={ "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
            //               "N",  "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
            //                "1", "2", "3", "4", "5", "6", "7", "8", "9" };

            string[] Code = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

            string strRd = string.Empty;

            Random rd = new Random();

            //建立實體圖檔並設定大小
            Bitmap Bmp = new Bitmap(80, 25);
            Graphics Gpi = Graphics.FromImage(Bmp);

            Font Font1 = new Font("Verdana", 14, FontStyle.Regular);
            for (int i = 0; i < 5; i++)
            {
                strRd += Code[rd.Next(10)];
            }

            Pen PenLine = new Pen(Brushes.Red, 1);
            //背景彥色
            Gpi.Clear(Color.Black);
            Gpi.DrawLine(PenLine, 0, rd.Next(80), 90, rd.Next(25));
            Gpi.DrawString(strRd, Font1, Brushes.AntiqueWhite, 0, 0);

            //亂數產生霧點
            for (int i = 0; i <= 25; i++)
            {
                int RandPixelX = rd.Next(0, 80);
                int RandPixelY = rd.Next(0, 25);
                Bmp.SetPixel(RandPixelX, RandPixelY, Color.Pink);
            }

            //儲存驗證碼
            Session[myCode] = strRd;

            MemoryStream s = new MemoryStream();

            Bmp.Save(s, System.Drawing.Imaging.ImageFormat.Gif);

            return File(s.GetBuffer(), "image/gif");
        }



        #endregion

        #region "版面"

        //public void template_Layout() 
        //{          
        //    var route = RouteTable.Routes.GetRouteData(HttpContext);
        //    //將撈出的controller轉為字串並設給controller
        //    string controller = route.Values["controller"] as string;
        //    //將撈出的action轉為字串並設給action
        //    string action = route.Values["action"] as string;

        //    var data = this._DB.sys_power_list.Where(m => m.controll == controller && m.action == action).SingleOrDefault();
        //    if (data != null) 
        //    {
        //        ViewBag.h1 = data.sub_label;
        //        ViewBag.span = data.sub_info;
        //    }          
        //}

        #endregion

        [HttpGet]
        public ActionResult xIndex(string TableName)
        {
            //string TableName = "news";
            if (String.IsNullOrEmpty(TableName))
            {
                var data = this._DB.sys_power_list.Where(m => m.show_visible == true).Where(m => m.power_id == 1).ToList();

                return View(data);

            }
            else
            {
                galaxy_web.ns.DataObj db = new galaxy_web.ns.DataObj();
                db.New("netwebConnectionString");

                string data_class = db.GetModel(TableName);

                ViewBag.data_class = data_class;

                string dbContext = db.getdbContext(TableName);

                ViewBag.dbContext = dbContext;

                ViewBag.TableName = TableName;

                string data_editString = db.GetEditString(TableName);

                ViewBag.editString = data_editString;

                string data_viewString = db.GetViewString(TableName);

                ViewBag.viewString = data_viewString;

                return View();
            }

        }

        public ActionResult notAllow()
        {
            return View();
        }

        #region "版面"

        //左選菜單
        public ActionResult template_left(string controller, string action)
        {
            ViewBag.controller = controller;
            ViewBag.action = action;
            //ViewBag.export = TempData["export"];

            DateTime sD = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            DateTime eD = sD.AddMonths(1);

            int power_id = 0;
            //int company_id = 0;
            int[] ordertotal = { 0, 0, 0, 0 };


            if (!String.IsNullOrEmpty(User.Identity.Name))
            {
                var pdata = NW.admin(NW.DESDeCode(User.Identity.Name));
                if (pdata.power_id < 1)
                {
                    return LogOut();
                }
                power_id = (int)pdata.power_id;
                //company_id = pdata.company_id;


                //var ordernum = db.order_data.Where(m => m.visible);

                //if(power_id == 4){

                //   //ordertotal[0] = ordernum.Where(m => m.visible && m.company_id == company_id && m.order_datetime >= sD && m.order_datetime < eD && m.flag > 0).Count();
                //   ordertotal[1] = ordernum.Where(m => m.company_id == company_id && m.export_datetime == null && (m.flag == 1 || m.flag == 5)).Count();
                //   ordertotal[2] = ordernum.Where(m => m.company_id == company_id && m.export_datetime != null && m.goods_datetime == null && (m.flag == 1 || m.flag == 5)).Count();
                //   //ordertotal[3] = ordernum.Where(m => m.visible && m.company_id == company_id &&  m.flag == 5).Count();
                //}
                //else{

                //   //ordertotal[0] = ordernum.Where(m => m.visible && m.order_datetime >= sD && m.order_datetime < eD).Count();
                //   ordertotal[1] = ordernum.Where(m => m.export_datetime == null && (m.flag == 1 || m.flag == 5)).Count();
                //   ordertotal[2] = ordernum.Where(m => m.export_datetime != null && m.goods_datetime == null && (m.flag == 1 || m.flag == 5)).Count();
                //   //ordertotal[3] = ordernum.Where(m => m.visible && m.flag == 5).Count();
                //}  


            }


            //List<int> order = new List<int>();
            //foreach (int item in ordertotal)
            //{
            //    order.Add(item);
            //}
            //ViewData["order"] = order;


            var data = this._DB.sys_power_list.Where(m => m.show_visible == true && m.power_id == power_id)
                .OrderBy(m => m.inx).ThenBy(m => m.sub_inx).ToList();

            //if (power_id == 1) { data = this._DB.sys_power_list.Where(m => m.power_id == power_id).OrderBy(m => m.inx).ThenBy(m => m.sub_inx).ToList(); }

            return PartialView(data);
        }

        //sitemap
        public ActionResult template_breadcrumb(string controller, string action)
        {
            int power_id = 0;

            //公司簡介
            action = NW.getActionID(action);

            if (!String.IsNullOrEmpty(User.Identity.Name)) { power_id = int.Parse(NW.DESDeCode(User.Identity.Name)); }
            var data = this._DB.sys_power_list.Where(m => m.power_id == power_id).Where(m => m.controll == controller).Where(m => m.action == action).SingleOrDefault();
            if (data != null)
            {
                ViewBag.controllerName = data.label;
                ViewBag.actionName = data.sub_label;
            }
            ViewBag.controller = controller;
            ViewBag.action = action;
            return PartialView();
        }

        //pagetitle
        public ActionResult template_pagetitle(string controller, string action)
        {
            int power_id = 0;

            //公司簡介
            action = NW.getActionID(action);

            if (!String.IsNullOrEmpty(User.Identity.Name)) { power_id = int.Parse(NW.DESDeCode(User.Identity.Name)); }
            var data = this._DB.sys_power_list.Where(m => m.power_id == power_id).Where(m => m.controll == controller).Where(m => m.action == action).SingleOrDefault();
            if (data != null)
            {
                ViewBag.h1 = data.sub_label;
                ViewBag.span = data.sub_info;
            }
            else
            {
                ViewBag.h1 = "";
                ViewBag.span = "";
            }
            return PartialView();
        }

        #endregion

        #region "圖片上傳"

        class ImageData
        {
            public string imgSrc { get; set; }
            public string simgName { get; set; }
            public string bimgName { get; set; }
            public string info { get; set; }
            public int inx { get; set; }
            public string ID { get; set; }
            public string status { get; set; }
        }

        //[HttpPost]
        //public ActionResult UploadImage2(HttpPostedFileBase Filedata, int id, string name, VmImage vmig)
        //{

        //    ImageData imgData = new ImageData() { ID = id.ToString(), status = "0" };

        //    if (Filedata != null && Regex.IsMatch(Path.GetExtension(Filedata.FileName), "^[.](jpg|png|jpeg|gif|bmp)$", RegexOptions.IgnoreCase))
        //    {

        //        string tmpPath = Server.MapPath("~/Content/images/" + name);

        //        if (!Directory.Exists(tmpPath))
        //        {
        //            Directory.CreateDirectory(tmpPath);
        //        }
        //        int SW = 0, SH = 0, BW = 0, BH = 0;

        //        switch (name)
        //        {
        //            case "news":
        //                SW = NW.GetParam(name).S_Img_W; SH = NW.GetParam(name).B_Img_H;
        //                BW = NW.GetParam(name).B_Img_W; BH = NW.GetParam(name).B_Img_H;
        //                break;
        //            case "product":
        //                SW = NW.GetParam(name).S_Img_W; SH = NW.GetParam(name).B_Img_H;
        //                BW = NW.GetParam(name).B_Img_W; BH = NW.GetParam(name).B_Img_H;
        //                break;
        //        }

        //        //存放原圖路徑
        //        string strFilePath = Server.MapPath("~/Content/images/" + name + "/" + Filedata.FileName);
        //        //新的檔案名稱 小圖

        //        string water = NW.getImgWater();

        //       //string strFileName = Guid.NewGuid().ToString() + Path.GetExtension(Filedata.FileName);

        //        string strFileName = water + "1" + Path.GetExtension(Filedata.FileName);

        //        //存放檔案路徑
        //        string strFilePath2 = Server.MapPath("~/Content/images/" + name + "/" + strFileName);

        //        //string strFileName2 = Guid.NewGuid().ToString() + Path.GetExtension(Filedata.FileName);

        //        string strFileName2 = water + "2" + Path.GetExtension(Filedata.FileName);

        //        string strFilePath3 = Server.MapPath("~/Content/images/" + name + "/" + strFileName2);

        //        //檔案存放在Server
        //        Filedata.SaveAs(strFilePath);
        //        imgData.simgName = Images.Helps.ImageHelper.Rebmp(strFileName, strFilePath, strFilePath2, SW, SH);

        //        //檔案存放在Server
        //        Filedata.SaveAs(strFilePath);
        //        imgData.bimgName = Images.Helps.ImageHelper.Rebmp(strFileName2, strFilePath, strFilePath3, BW, BH);


        //        imgData.imgSrc = Url.Content("~/Content/images/" + name + "/" + strFileName2);
        //        //imgData.info = Filedata.FileName.Replace(Path.GetExtension(Filedata.FileName), "");
        //        imgData.info = "";
        //        imgData.status = "1";
        //        imgData.inx = 0;

        //    }
        //    //回傳Json格式
        //    return Json(imgData);
        //}

        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase Filedata, string id, string name)
        {
            var route = RouteTable.Routes.GetRouteData(HttpContext);
            string action = route.Values["action"] as string; //將撈出的action轉為字串並設給action


            ImageData imgData = new ImageData() { ID = id, status = "0" };

            if (Filedata != null && Regex.IsMatch(Path.GetExtension(Filedata.FileName), "^[.](jpg|png|jpeg|gif|bmp)$", RegexOptions.IgnoreCase))
            {

                string tmpPath = Server.MapPath("~/Content/images/" + name);

                if (!Directory.Exists(tmpPath))
                {
                    Directory.CreateDirectory(tmpPath);
                }
                int SW = 0, SH = 0, BW = 0, BH = 0;
                int block = 0;
                SW = NW.GetParam(name).S_Img_W; SH = NW.GetParam(name).S_Img_H;
                BW = NW.GetParam(name).B_Img_W; BH = NW.GetParam(name).B_Img_H;

                switch (name)
                {
                    case "member": //只需一張的設定
                        block = 1;
                        break;
                }

                string water = NW.getImgWater();

                //新的檔案名稱
                //string strFileName = Guid.NewGuid().ToString() + Path.GetExtension(Filedata.FileName);
                string strFileName = water + "1" + Path.GetExtension(Filedata.FileName);
                //存放原圖路徑
                string strFilePath = Server.MapPath("~/Content/images/" + name + "/" + Filedata.FileName);
                //string strFileName2 = Guid.NewGuid().ToString() + Path.GetExtension(Filedata.FileName);
                string strFileName2 = water + "2" + Path.GetExtension(Filedata.FileName);
                string strFilePath3 = Server.MapPath("~/Content/images/" + name + "/" + strFileName2);
                //存放檔案路徑
                string strFilePath2 = Server.MapPath("~/Content/images/" + name + "/" + strFileName);



                //檔案存放在Server
                Filedata.SaveAs(strFilePath);
                //imgData.simgName = Images.Helps.ImageHelper.Rebmp(strFileName, strFilePath, strFilePath2, SW, SH);

                var Rebmp = new Gs._Rebmp()
                {
                    Img = Filedata,
                    resizedHeight = SH,
                    resizedWidth = SW,
                    tmpPath = tmpPath
                };

                imgData.simgName = Gs.Rebmp(Rebmp);
                //imgData.simgName = Images.Helps.ImageHelper.Rebmp(Filedata, tmpPath, SW, SH);
                imgData.imgSrc = Url.Content("~/Content/images/" + name + "/" + strFileName);

                if (block == 0)
                {

                    Filedata.SaveAs(strFilePath);
                    imgData.bimgName = Images.Helps.ImageHelper.Rebmp(Filedata, tmpPath, BW, BH, strFilePath3);
                }


                //imgData.info = Filedata.FileName.Replace(Path.GetExtension(Filedata.FileName), "");
                imgData.info = "";
                imgData.status = "1";

            }
            //回傳Json格式
            return Json(imgData);
        }

        [HttpPost]
        public ActionResult UploadImageCustom(HttpPostedFileBase Filedata, string id, string name, string path)
        {

            ImageData imgData = new ImageData() { ID = id, status = "0" };

            if (Filedata != null && Regex.IsMatch(Path.GetExtension(Filedata.FileName), "^[.](jpg|png|jpeg|gif|bmp)$", RegexOptions.IgnoreCase))
            {

                string tmpPath = Server.MapPath("~/Content/images/" + path);

                if (!Directory.Exists(tmpPath))
                {
                    Directory.CreateDirectory(tmpPath);
                }
                int SW = 0, SH = 0, BW = 0, BH = 0;
                int block = 0;
                SW = NW.GetParam(name).S_Img_W; SH = NW.GetParam(name).S_Img_H;
                BW = NW.GetParam(name).B_Img_W; BH = NW.GetParam(name).B_Img_H;

                switch (name)
                {
                    case "member": //只需一張的設定
                        block = 1;
                        break;
                }

                string water = NW.getImgWater();
                //新的檔案名稱
                //string strFileName = Guid.NewGuid().ToString() + Path.GetExtension(Filedata.FileName);
                string strFileName = water + "1" + Path.GetExtension(Filedata.FileName);
                //存放原圖路徑
                string strFilePath = Server.MapPath("~/Content/images/" + path + "/" + Filedata.FileName);
                //string strFileName2 = Guid.NewGuid().ToString() + Path.GetExtension(Filedata.FileName);
                string strFileName2 = water + "2" + Path.GetExtension(Filedata.FileName);
                string strFilePath3 = Server.MapPath("~/Content/images/" + path + "/" + strFileName2);

                //存放檔案路徑
                string strFilePath2 = Server.MapPath("~/Content/images/" + path + "/" + strFileName);



                //檔案存放在Server
                Filedata.SaveAs(strFilePath);
                imgData.simgName = Images.Helps.ImageHelper.Rebmp(Filedata, tmpPath, SW, SH);
                imgData.imgSrc = Url.Content("~/Content/images/" + path + "/" + strFileName);

                if (block == 0)
                {
                    Filedata.SaveAs(strFilePath);
                    imgData.bimgName = Images.Helps.ImageHelper.Rebmp(Filedata, tmpPath, BW, BH, strFilePath3);
                }


                //imgData.info = Filedata.FileName.Replace(Path.GetExtension(Filedata.FileName), "");
                imgData.info = "";
                imgData.status = "1";

            }
            //回傳Json格式
            return Json(imgData);
        }

        [HttpPost]
        public ActionResult Del()
        {
            return View();
        }
        #endregion

        #region "匯出匯入訂單"

        //public ActionResult Export()
        //{

        //    //建立Excel

        //    XLWorkbook workbook = new XLWorkbook();

        //    var sheet = workbook.Worksheets.Add("訂單列表");

        //    int colIdx = 1;

        //    string[] cols = "轉單日期,最晚出貨日,交易序號,訂購人,收件人姓名,店家商品料號,商品名稱,商品規格,數量,金額小計,購物車備註,買家確認日,收件人地址,收件人電話(日),收件人電話(夜),收件人行動電話,發票寄送地址,付款別,物流設定,收件人郵遞區號,商品編號,訂單編號,店家出貨日,商品類型,訂單狀態,入帳日,會員編號,會員信箱".Split(',');
        //    foreach (string colName in cols)
        //   { 
        //        sheet.Cell(1, colIdx++).Value = colName;
        //   }


        //    //修改標題列Style

        //    //var header = sheet.Range("A1:" + Convert.ToString((char)(64+cols.Length-6)) + "1");

        //    //for (int i = 1; i < 27; i++) {
        //    //    sheet.Cell(1, i).Style.Fill.BackgroundColor = XLColor.LightGray;
        //    //    sheet.Cell(1, i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        //    //}

        //    //for (int i = 27; i < 33; i++)
        //    //{
        //    //    sheet.Cell(1, i).Style.Fill.BackgroundColor = XLColor.Red;
        //    //    sheet.Cell(1, i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        //    //}

        //    //header = sheet.Range("AA:AF");
        //    //header.Style.Fill.BackgroundColor = XLColor.Red;
        //    //header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        //    var data = db.order_data.Where(m => m.order_id >= 0 && m.flag == 1).ToList();

        //    if (data == null) { return Content("您輸入的條件，並無資料，請重新選擇條件，謝謝!!"); }

        //    int rowIdx = 2;

        //   //"轉單日期,最晚出貨日,交易序號,訂購人,收件人姓名,1
        //    //店家商品料號,商品名稱,商品規格,數量,金額小計,6
        //    //購物車備註,買家確認日,收件人地址,收件人電話(日),收件人電話(夜),11
        //    //收件人行動電話,發票寄送地址,付款別,物流設定,收件人郵遞區號,16
        //    //商品編號,訂單編號,店家出貨日,商品類型,訂單狀態,21
        //    //入帳日,會員編號,會員信箱";26
        //    string p = "'";
        //    foreach (var item in data) {
        //        var mData = db.member_data.Where(m => m.member_id == item.member_id).FirstOrDefault();
        //        string name= mData.name;
        //        string freight_flag_name = item.freight_flag_name;

        //        if (!string.IsNullOrEmpty(freight_flag_name)) { 
        //            foreach (string ss in freight_flag_name.Split(',')){
        //                var arr = ss.Split(':');
        //                int iFright_id = 0;
        //                string Fright_sn = "81006001";
        //                switch (arr[0]) {
        //                    case "冷藏運費": iFright_id = 1; Fright_sn = "81006005"; break;
        //                    case "冷凍運費": iFright_id = 2; Fright_sn = "81006008"; break;
        //                }

        //                var idata = new order_sub {order_id=item.order_id };
        //                idata.item_name = arr[0];
        //                idata.item_no = Fright_sn;
        //                idata.price = Convert.ToInt32(arr[1]);
        //                idata.num = 1;
        //                idata.freight_id = 9;
        //                idata.freight_visible = true;
        //                item.order_sub.Add(idata);
        //            }
        //        }

        //        item.order_sub = item.order_sub.OrderBy(m=>m.freight_id).ToList();
        //        foreach (var subitem in item.order_sub) {
        //            //var pData = db.product_data.Where(m => m.product_id == subitem.product_id).FirstOrDefault();
        //            //if (pData != null) { ffname = NW.FFName[pData.freight_id]; }
        //            string ffname = "";
        //            if (subitem.freight_id >= 0 && subitem.freight_id < 3) {ffname = NW.FFName[subitem.freight_id]; }
        //            sheet.Cell(rowIdx, 1).Value = p+item.pay_datetime.Value.ToString("yyyy/MM/dd HH:mm");
        //            sheet.Cell(rowIdx, 2).Value = p+item.pay_datetime.Value.AddDays(3).ToString("yyyy/MM/dd HH:mm");
        //            sheet.Cell(rowIdx, 3).Value = p+item.order_id.ToString()+"-" + subitem.freight_id.ToString();
        //            sheet.Cell(rowIdx, 4).Value = p+name;
        //            sheet.Cell(rowIdx, 5).Value = p+item.send_name;

        //            sheet.Cell(rowIdx, 6).Value = p+subitem.item_no;
        //            sheet.Cell(rowIdx, 7).Value = p+subitem.item_name;
        //            sheet.Cell(rowIdx, 8).Value = p+subitem.style_name;
        //            sheet.Cell(rowIdx, 9).Value = p+subitem.num;
        //            sheet.Cell(rowIdx, 10).Value = p+subitem.num*subitem.price;

        //            sheet.Cell(rowIdx, 11).Value = p;
        //            sheet.Cell(rowIdx, 12).Value = p;
        //            sheet.Cell(rowIdx, 13).Value = p+item.send_zip+item.send_city+item.send_canton+item.send_address;
        //            sheet.Cell(rowIdx, 14).Value = p;
        //            sheet.Cell(rowIdx, 15).Value = p;

        //            sheet.Cell(rowIdx, 16).Value = p+item.send_tel;
        //            sheet.Cell(rowIdx, 17).Value = p+mData.zip + mData.zip_data.city + mData.zip_data.canton + mData.address;
        //            sheet.Cell(rowIdx, 18).Value = p+NW.payName(item.pay_id);
        //            sheet.Cell(rowIdx, 19).Value = p+ffname;
        //            sheet.Cell(rowIdx, 20).Value = p+item.send_zip;

        //            sheet.Cell(rowIdx, 21).Value = p+subitem.item_no;
        //            sheet.Cell(rowIdx, 22).Value = p+item.order_id.ToString()+"-"+subitem.sub_id.ToString();
        //            sheet.Cell(rowIdx, 23).Value = p;
        //            sheet.Cell(rowIdx, 24).Value = p;
        //            sheet.Cell(rowIdx, 25).Value = p+"備貨中";

        //            sheet.Cell(rowIdx, 26).Value = p;
        //            sheet.Cell(rowIdx, 27).Value = p+mData.member_id.ToString();
        //            sheet.Cell(rowIdx, 28).Value = p+mData.mail;

        //            //sheet.Cell(rowIdx, 27).Value = p;
        //            //sheet.Cell(rowIdx, 28).Value = p;
        //            //sheet.Cell(rowIdx, 29).Value = p;
        //            //sheet.Cell(rowIdx, 30).Value = p;
        //            //sheet.Cell(rowIdx, 31).Value = p;
        //            //sheet.Cell(rowIdx, 32).Value = p;

        //            rowIdx+=1;
        //        }


        //    }


        //    //將Path放在第一欄(稍後隱藏)

        //    //sheet.Cell(rowIdx, 1).Value = "路徑";

        //    ////存入檔名或目錄名

        //    //sheet.Cell(rowIdx, 2).Value = "名稱";

        //    //sheet.Cell(rowIdx, 2).Style.Font.FontColor = XLColor.GrayAsparagus;




        //    //第一欄隱藏

        //    //sheet.Column(1).Hide();

        //    //自動伸縮欄寬

        //    for (int i = 1; i <= cols.Length; i++) {
        //        sheet.Column(i).AdjustToContents();
        //    }

        //    //sheet.Column(2).Width += 2;

        //    //sheet.Column(3).Width = 50;

        //    //寫入檔案

        //    string excelPath = Server.MapPath("~/Content/files/"+DateTime.Now.ToString("yyyyMMddHHmmss")+".xlsx");

        //    workbook.SaveAs(excelPath);



        //    return Content("下載完成");
        //}

        public ActionResult Inport()
        {

            // 建立一個工作簿 Office 2007
            XSSFWorkbook excel;

            string fileLocation = Server.MapPath("~/Content/files/20140718164133.xlsx");

            // 檔案讀取
            using (FileStream files = new FileStream(fileLocation, FileMode.Open, FileAccess.Read))
            {
                excel = new XSSFWorkbook(files); // 將剛剛的Excel 讀取進入到工作簿中
            }
            NPOI.SS.UserModel.ISheet sheet = excel.GetSheetAt(0);

            for (int row = 1; row <= sheet.LastRowNum; row++) // 使用For 走訪所有的資料列
            {
                if (sheet.GetRow(row) != null) // 驗證是不是空白列
                {
                    for (int c = 0; c <= sheet.GetRow(row).LastCellNum; c++) // 使用For 走訪資料欄
                    {
                        if (sheet.GetRow(row).GetCell(c) != null)
                        {
                            string x2 = sheet.GetRow(row).GetCell(c).ToString();

                        }
                    }
                }
            }



            return Content("匯入完成");
        }

        #endregion

        #region"圖檔刪除-配合模組(Img)"

        [HttpPost]
        public ActionResult ImgDel(string src)
        {
            string msg = "";

            if (!string.IsNullOrEmpty(src))
            {
                string ImgPath = Server.MapPath(src);


                //找到圖檔
                if (System.IO.File.Exists(ImgPath))
                {
                    string Controller = src.IndexOf("product") > -1 ? "product" : "adv";
                    //var Controller = RouteData.Values["controller"].ToString().ToLower().Replace("nw", "");

                    string tmpPath = Server.MapPath("~/Content/images/") + Controller; //存放路徑

                    //新路徑
                    string tempPath_new = tmpPath + "/Delete";
                    if (!Directory.Exists(tempPath_new)) { Directory.CreateDirectory(tempPath_new); } //建資料夾

                    //新的位置
                    tempPath_new = tempPath_new + "/" + NW.Now.ToString("yyMMddHHmmss") + "-" + src.Split(new string[] { Controller + "/" }, StringSplitOptions.None)[1];

                    //移動並更名
                    System.IO.File.Move(ImgPath, tempPath_new);

                    //var Img = this._DB.product_image.Where(m => src.Contains(m.img)).SingleOrDefault();
                    //if (Img != null)
                    //{
                    //    Img.visible = false;
                    //    this._DB.SaveChanges();

                    //    //System.IO.File.Delete(tmpPath);
                    //    msg = "圖片已刪除";
                    //}
                    msg = "圖片已刪除";
                }
            }

            return Json(new { msg = msg }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region"查詢騰雲會員"

        [HttpPost]
        public ActionResult getMember(string uid)
        {
            string msg = "查無此會員";
            var data = this._DB2.sys_admin_data.Where(m => m.uid == uid && m.visible && m.power_id == 10).Select(m => new { m.name, m.agent_group, m.start_date }).FirstOrDefault();
            if (data != null)
            {
                msg = "會員姓名：" + data.name + "，等級：" + data.agent_group.name + "，註冊時間：" + data.start_date.Value.ToShortDateString();
            }

            return Json(new { msg = msg }, JsonRequestBehavior.AllowGet);
            //return Content(msg);
        }

        #endregion


        //public ActionResult test()
        //{
        //    var P = this._DB.product_data.Select(m => new { m.product_id, m.cost, m.erp_cost }).ToList();
        //    var S = this._DB.order_sub.ToList();
        //    foreach (var item in S)
        //    {
        //        item.cost = P.Where(m => m.product_id == item.product_id).Select(m => m.cost).FirstOrDefault();
        //        item.price = P.Where(m => m.product_id == item.product_id).Select(m => m.erp_cost).FirstOrDefault();
        //        this._DB.SaveChanges();
        //    }
        //    return Content("");
        //}

        //public ActionResult test()
        //{
        //    var P = this._DB.product_data.Where(m => m.status == 3 && ((m.price - m.cost) / m.price) < 0.15m && m.price > 0 && (m.erp_cost < m.price || m.erp_cost > m.price)).ToList();

        //    foreach (var item in P)
        //    {
        //        item.erp_cost = item.price;
        //        item.sync_flag = 1;
        //    }

        //    this._DB.SaveChanges();

        //    return Content("");
        //}

    }




}
