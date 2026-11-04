using ClosedXML.Excel;
using galaxy_web.Models;
using galaxy_web.Models2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

public class Gs
{
    private static _dbContext _db_;
    protected static _dbContext _DB
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

    private static _dbContext2 _db2_;
    protected static _dbContext2 _DB2
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


    public static List<ListItem> sList(string[] str, string str_data = null, string addAll = null)
    {
        int num = 0;

        List<ListItem> ListItems = new List<ListItem>();

        if (!string.IsNullOrEmpty(addAll))
        {
            //沒東西 就第一個等於啟用
            ListItems.Add(new ListItem("", addAll, string.IsNullOrEmpty(str_data) ? true : false));
        }


        foreach (var item in str)
        {
            //搜尋條件
            string ischeck = "," + (!string.IsNullOrEmpty(str_data) ? str_data : "") + ",";

            //拆解
            List<string> s = item.Split(',').ToList();
            string id = s.Count() > 1 ? s[0] : num.ToString();
            string className = s.Count() > 2 ? s[2] : null;
            bool isChecked = ("," + s[0] + ",").Contains(ischeck);

            ListItem L = new ListItem(id, s[1], isChecked, className);

            ListItems.Add(L);

            num++;
        }

        return ListItems;
    }

    public class _Rebmp
    {
        public HttpPostedFileBase Img { get; set; }
        public string tmpPath { get; set; }
        public int? resizedWidth { get; set; }
        public int? resizedHeight { get; set; }
        public string[] strFilePath3 { get; set; }
        public string name { get; set; }
    }

    //轉圖 欲轉檔的圖檔、存放位置、寬度、長度
    public static string Rebmp(_Rebmp Get)
    {
        HttpPostedFileBase Img = Get.Img;
        string tmpPath = Get.tmpPath;
        int? resizedWidth = Get.resizedWidth, resizedHeight = Get.resizedHeight;
        string[] strFilePath3 = Get.strFilePath3;

        tmpPath = !string.IsNullOrEmpty(tmpPath) ? tmpPath : "~/Content/images/temp"; //存放位置
        if (!Directory.Exists(tmpPath)) { Directory.CreateDirectory(tmpPath); } //建立資料夾

        string water = !string.IsNullOrEmpty(Get.name) ? Get.name : (NW.getImgWater() + Path.GetExtension(Img.FileName));
        string strFileName = water; //新的檔案名稱
        string strFilePath = tmpPath + "/" + Img.FileName; //存放檔案路徑
        string strFilePath2 = tmpPath + "/" + strFileName; //存放檔案路徑

        List<string> allFilePath = new List<string>() { strFilePath2 };

        if (strFilePath3 != null) //如果有其他指定路徑
        {
            allFilePath.AddRange(strFilePath3);
        }

        foreach (var item in allFilePath)
        {
            while (true)
            {
                //如果是gif, 直接存檔新圖，不縮圖
                if (strFilePath.Contains(".gif")) { Img.SaveAs(item); break; }

                //一定要先存一張預設的
                Img.SaveAs(strFilePath);
                System.Drawing.Image image = System.Drawing.Image.FromFile(strFilePath);
                //必須使用絕對路徑
                ImageFormat thisFormat = image.RawFormat;

                //取得影像的格式
                int fixWidth = 0, fixHeight = 0;

                if (image.Width > resizedWidth || image.Height > resizedHeight)
                //如果圖片的寬大於最大值或高大於最大值就往下執行
                {

                    #region"寬高設定"

                    if (resizedWidth.HasValue && resizedHeight.HasValue)
                    {
                        fixWidth = (int)resizedWidth;
                        fixHeight = (int)resizedHeight;
                    }
                    else if (resizedWidth.HasValue)
                    {
                        fixWidth = (int)resizedWidth;
                        //設定修改後的圖寬
                        fixHeight = Convert.ToInt32((Convert.ToDouble(fixWidth) / Convert.ToDouble(image.Width)) * Convert.ToDouble(image.Height));
                        //設定修改後的圖高
                    }
                    else if (resizedHeight.HasValue)
                    {
                        fixHeight = (int)resizedHeight;
                        //設定修改後的圖高
                        fixWidth = Convert.ToInt32((Convert.ToDouble(fixHeight) / Convert.ToDouble(image.Height)) * Convert.ToDouble(image.Width));
                        //設定修改後的圖寬
                    }
                    else
                    {
                        fixWidth = image.Width;
                        fixHeight = image.Height;
                    }

                    #endregion

                }
                else //圖片沒有超過設定值，不執行縮圖          
                {
                    fixHeight = image.Height;
                    fixWidth = image.Width;
                }

                // 產生縮圖 
                using (var bmp = new Bitmap(fixWidth, fixHeight))
                {
                    using (var gr = Graphics.FromImage(bmp))
                    {
                        ImageCodecInfo ici = GetEncoder(ImageFormat.Jpeg);

                        var ep = new System.Drawing.Imaging.EncoderParameters();
                        ep.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

                        gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        gr.DrawImage(image, new Rectangle(0, 0, fixWidth, fixHeight), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
                        bmp.Save(strFilePath2, ici, ep);
                    }
                }

                //Bitmap bmp = new Bitmap(fixWidth, fixHeight);

                ////將bimap轉為graphic來操作
                //System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(bmp);

                ////設定縮圖的品質
                //gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                //gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                ////以原始的image為來源，繪製成rectDestination的大小(0,0,resizedWidth,resizedHeight為新的大小座標)
                //System.Drawing.Rectangle rectDestination = new System.Drawing.Rectangle(0, 0, fixWidth, fixHeight);
                ////gr.DrawImage(image, rectDestination);
                //gr.DrawImage(image, new Rectangle(0, 0, fixWidth, fixHeight), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);

                ////存檔
                //image.Save(strFilePath2);
                //gr.Dispose();

                ////將修改過的圖存於設定的位子
                //bmp.Dispose();
                ////釋放記憶體
                image.Dispose();
                ////釋放掉圖檔 

                break;
            }

            //刪除原檔
            System.IO.File.Delete(strFilePath);
        }

        return strFileName.ToString();
    }

    private static ImageCodecInfo GetEncoder(ImageFormat format)
    {

        ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

        foreach (ImageCodecInfo codec in codecs)
        {
            if (codec.FormatID == format.Guid)
            {
                return codec;
            }
        }
        return null;
    }

    #region"圖片路徑"

    //圖片、放在哪裡、前置網址
    public static string Path_Img(string img, string con = null, string url = null)
    {

        string path = (con == null ? "Content/images/product/" : "/Content/images/" + con + "/"); //圖片路徑

        string img_s = "";

        if (!string.IsNullOrEmpty(img))
        {

            if (img.IndexOf("http://") != -1)
            {
                img_s = img;
            }
            else
            {
                img_s = img.IndexOf("images") != -1 ? img : path + img;

                //有無前置網址
                img_s = (!string.IsNullOrEmpty(url)) ? url + img_s : img_s;
            }

            if (string.IsNullOrEmpty(url))
            {
                string tmpPath = HttpContext.Current.Server.MapPath(img_s);
                //找看看路徑有沒有圖片
                if (!System.IO.File.Exists(tmpPath))
                {
                    img_s = NW.ssNophoto;
                }
            }

        }
        else
        {
            img_s = NW.ssNophoto;
        }


        img_s = img_s.Replace("//C", "/C");

        return img_s;
    }

    #endregion

    #region"驗證專區"

    public static bool isPhone(string phone)
    {
        bool isphone = false;

        string pattern = @"^09[\d]{8}$";
        Regex r = new Regex(pattern);
        Match m = r.Match(phone);

        isphone = m.Success;


        return isphone;

    }

    public static bool isMail(string mail)
    {
        bool r = false;

        if (!string.IsNullOrEmpty(mail))
        {
            try
            {
                System.Net.Mail.MailAddress mail_S = new System.Net.Mail.MailAddress(mail);
                r = true;
            }
            catch
            {
                r = false;
            }

        }

        return r;
    }

    #endregion

    #region"取得連線的IP"

    public static string getIPAddress
    {
        get
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;

            string sIPAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(sIPAddress))
            {

                return context.Request.ServerVariables["REMOTE_ADDR"];

            }

            else
            {

                string[] ipArray = sIPAddress.Split(new Char[] { ',' });

                return ipArray[0];

            }
        }
    }

    #endregion

    #region"連線的裝置"

    public static string getDevice
    {
        get
        {
            string device = "";

            device = HttpContext.Current.Request.UserAgent;

            return device;
        }

    }

    #endregion

    #region"MAC_Address"

    public static string getMac()
    {
        NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

        string mac = "";

        foreach (var nic in nics)
        {
            // 因為電腦中可能有很多的網卡(包含虛擬的網卡)，
            // 我只需要 Ethernet 網卡的 MAC
            if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
            {
                mac = nic.GetPhysicalAddress().ToString();
            }
        }

        return mac;
    }

    #endregion

    #region "出貨單號"

    //取得16碼出貨單號
    public static string getOrderSn(int order_id)
    {

        string tmp = "DJ" + DateTime.Now.ToString("yyMMdd") + order_id.ToString("000000");
        return tmp;
    }

    #endregion

    #region"共用時間"

    public static DateTime Now
    {
        get { return DateTime.Now; }
    }

    public static DateTime T = DateTime.Today;

    public static DateTime Today
    {
        get { return T; }
    }

    public static DateTime Tomorrow
    {
        get { return T.AddDays(1); }
    }

    #endregion

    #region"郵遞區號"

    public static KeyValuePair<List<ListItem>, List<ListItem>> getZip(string myZip = null)
    {

        #region"郵遞區號"

        var zip_All = _DB.zip_data.Where(m => m.visible).Select(m => new { m.zip, m.city_id, m.city, m.canton }).ToArray();

        List<ListItem> ListItems = new List<ListItem>() { };

        var zip = zip_All.Select(m => m.zip + "," + m.canton + "," + m.city_id).ToArray();
        ListItems.AddRange(Gs.sList(zip, myZip, "請選擇"));
        //ViewData["zip"] = ListItems;


        var a = ListItems.Where(m => m.IsChecked == true).OrderByDescending(m => m.Value).FirstOrDefault();

        string className = a != null ? a.className : null;
        var zip_g = zip_All.Select(m => m.city_id + "," + m.city).Distinct().ToArray();
        var ListItems_chained = new List<ListItem> { };
        ListItems_chained.AddRange(Gs.sList(zip_g, className, "請選擇"));
        //ViewData["zip_chained"] = ListItems;

        return new KeyValuePair<List<ListItem>, List<ListItem>>(ListItems, ListItems_chained);

        #endregion
    }

    #endregion

    #region"EXCEL"

    public enum isStatus : int { 非必填 = 1, 數字格式 = 2, 幣別格式 = 3, 時間格式 = 4 };

    public static KeyValuePair<string, List<Dictionary<string, string>>> getRows(HttpPostedFileBase Im, string[] Title)
    {
        string msg = "";
        var Rows = new List<Dictionary<string, string>>() { };

        var workbook = new XLWorkbook(Im.InputStream);
        var ws1 = workbook.Worksheet(1);

        //取得所有的標題
        Dictionary<int, string> isHeads = ws1.Row(1).CellsUsed().ToDictionary(m => m.Address.ColumnNumber, m => m.Value.ToString());

        #region"比對欄位是否都正確"

        //直接先判斷是否符合全部的欄位，方便之後的比對
        string[] isField = Title;

        //bool a = isHeads.Select(m => m.Value).Except(isField).Any();
        string[] b = isField.Except(isHeads.Select(m => m.Value)).ToArray();

        if (b.Any()) { msg = "請補上缺少的欄位：" + string.Join(",", b); }

        #endregion

        //取得本次資料
        Rows = ws1.RangeUsed().Rows().Skip(1).Select(m => m.Cells().ToDictionary(x => isHeads[x.Address.ColumnNumber], x => x.Value.ToString())).ToList();

        return new KeyValuePair<string, List<Dictionary<string, string>>>(msg, Rows);
    }

    //寫入資料表
    public static KeyValuePair<string, object> getTable(object Table, Dictionary<string, string> Correspond, Dictionary<string, string> Content, string[] noCheck = null)
    {
        string msg = "";

        while (true)
        {
            #region"判斷欄位必填"

            if (noCheck != null)
            {
                string[] i = Content.Select(m => m.Key).Except(noCheck).ToArray();
                string[] isCheck = Content.Where(m => i.Contains(m.Key) && string.IsNullOrEmpty(m.Value)).Select(m => m.Key).ToArray();

                //有該填的沒填就跳出
                if (isCheck.Any()) { msg = "欄位必填：" + string.Join(",", isCheck); break; }
            }


            #endregion

            #region"寫入本次的資訊"

            //本次填入的欄位
            foreach (var item in Correspond)
            {
                string Key = item.Key; //對應的欄位

                //是否為必填欄位

                string title = item.Value; //欄位名稱
                //當次的值
                string value = Content.Where(m => m.Key == Key).Select(m => m.Value).FirstOrDefault();

                Type model_type = Table.GetType();
                PropertyInfo model_property = model_type.GetProperty(item.Value);

                #region"判斷型態"

                //取得為該名稱的欄位型態
                if (model_property != null)
                {
                    var type_name = model_property.GetMethod.ReturnType.Name;
                    try
                    {
                        switch (type_name)
                        {
                            case "String":

                                string string_value = value;
                                model_property.SetValue(Table, string_value);

                                break;

                            case "Int32":

                                int int_value = Convert.ToInt32(value);
                                model_property.SetValue(Table, int_value);

                                break;

                            case "Decimal":

                                decimal decimal_value = Convert.ToDecimal(value);
                                model_property.SetValue(Table, decimal_value);

                                break;

                            case "Double":

                                Double double_value = Convert.ToDouble(value);
                                model_property.SetValue(Table, double_value);

                                break;

                            case "Single":
                                Single single_value = Convert.ToSingle(value);
                                model_property.SetValue(Table, single_value);

                                break;

                            case "Datetime":

                                DateTime datetime_value = Convert.ToDateTime(value);
                                model_property.SetValue(Table, datetime_value);

                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        msg += title + ",";
                        //有儲存失敗的 故自動停用
                        //model.visible = false;
                    }
                }

                #endregion


            }

            //如果有錯誤
            if (!string.IsNullOrWhiteSpace(msg)) { msg = "無法存入：" + msg.Trim(','); }

            #endregion

            break;
        }

        return new KeyValuePair<string, object>(msg, Table);

    }




    public class _getImport
    {
        public HttpPostedFileBase Im { get; set; }
        public string[] Title { get; set; }
        public Dictionary<int, string[]> isCheck { get; set; }

        public Enum ss { get; set; }
    }


    public static KeyValuePair<string, List<Dictionary<string, string>>> getImport(_getImport Get)
    {
        KeyValuePair<string, List<Dictionary<string, string>>> r = new KeyValuePair<string, List<Dictionary<string, string>>>() { };

        string msg = "";
        var Rows = new List<Dictionary<string, string>>() { };

        while (true)
        {
            //判斷格式
            if (!NW.getFileType(Get.Im, 3)) { msg = "請上傳.xlsx格式"; break; }

            var workbook = new XLWorkbook(Get.Im.InputStream);
            var ws1 = workbook.Worksheet(1);

            //取得所有的標題
            Dictionary<int, string> isHeads = ws1.Row(1).CellsUsed().ToDictionary(m => m.Address.ColumnNumber, m => m.Value.ToString());

            #region"比對欄位是否都正確"

            //直接先判斷是否符合全部的欄位，方便之後的比對
            string[] isField = Get.Title;

            //bool a = isHeads.Select(m => m.Value).Except(isField).Any();
            bool b = isField.Except(isHeads.Select(m => m.Value)).Any();

            if (b) { msg = "請確認欄位格式與給的是相符的"; break; }

            #endregion

            #region"比對欄位的各種判斷"

            //定義各個判斷的狀態
            Dictionary<int, string> isStatus = new Dictionary<int, string>() { { 1, "必填、非必填" }, { 2, "int" }, { 3, "decimal" }, { 4, "datetime" } };

            //先欄位、再來需要判斷的各種條件(參照isStatus)
            Dictionary<int, string[]> isCheck = Get.isCheck;

            #region"各種判斷的迴圈"

            foreach (var item in isStatus)
            {
                //條件
                int key = item.Key;

                #region"需要判斷的"

                string[] i = isCheck.Where(m => m.Key == key).Select(m => m.Value).FirstOrDefault() ?? new string[] { };

                //特殊的
                switch (key)
                {
                    case 1: //必填

                        i = isHeads.Select(m => m.Value).Except(i).ToArray();
                        break;
                }

                #endregion

                #region"根據列數進行迴圈"

                foreach (var item3 in i)
                {
                    string x = isHeads.Where(m => m.Value == item3).Select(m => m.Key.ToString()).FirstOrDefault();

                    //第幾列(不包含標題的內容)
                    var Column = ws1.Columns(x).CellsUsed().Skip(1).ToDictionary(m => m.Address.RowNumber, m => m.Value);

                    #region"條件"

                    switch (key)
                    {
                        case 1: //必填與非必填

                            //總行數(不包含標題)
                            int RowsN = ws1.RangeUsed().Rows().Count() - 1;
                            if (RowsN != Column.Count()) { msg += "第" + x + "欄的欄位必填"; break; }
                            break;

                        case 2: //需為數字

                            try { int[] it = Column.Select(m => Convert.ToInt32(m.Value)).ToArray(); }
                            catch { msg += "第" + x + "欄需為數字"; }
                            break;

                        case 3:

                            try { decimal[] ty = Column.Select(m => Convert.ToDecimal(m.Value)).ToArray(); }
                            catch { msg += "第" + x + "欄需為數字"; }
                            break;

                        case 4:

                            try { DateTime[] ty = Column.Select(m => Convert.ToDateTime(m.Value)).ToArray(); }
                            catch { msg += "第" + x + "欄需為時間"; }
                            break;
                    }

                    #endregion
                }
                //出現錯誤，直接跳掉
                //if (!string.IsNullOrEmpty(msg)) { break; }

                #endregion

            }

            #endregion

            #endregion

            Rows = ws1.RangeUsed().Rows().Skip(1).Select(m => m.Cells().ToDictionary(x => isHeads[x.Address.ColumnNumber], x => x.Value.ToString())).ToList();
            ////驗證都沒有錯誤，將資料回傳
            //if (string.IsNullOrEmpty(msg))
            //{
            //    //EXCEL的內容
            //    Rows = ws1.RangeUsed().Rows().Skip(1).Select(m => m.Cells().ToDictionary(x => isHeads[x.Address.ColumnNumber], x => x.Value.ToString())).ToList();
            //}

            break;
        }

        //回傳值
        r = new KeyValuePair<string, List<Dictionary<string, string>>>(msg, Rows);
        return r;
    }

    #region"匯出"

    //轉成DATATABLE
    public static DataTable ConvertObjectsToDataTable(IEnumerable<object> objects)
    {
        DataTable dt = null;

        if (objects != null && objects.Count() > 0)
        {

            Type type = objects.First().GetType();
            dt = new DataTable(type.Name);

            //性質
            foreach (PropertyInfo property in type.GetProperties())
            {
                dt.Columns.Add(new DataColumn(property.Name));
            }

            //域
            foreach (FieldInfo field in type.GetFields())
            {

                dt.Columns.Add(new DataColumn(field.Name));
            }

            //物件
            foreach (object obj in objects)
            {
                DataRow dr = dt.NewRow();
                foreach (DataColumn column in dt.Columns)
                {
                    PropertyInfo propertyInfo = type.GetProperty(column.ColumnName);
                    if (propertyInfo != null)
                    {
                        dr[column.ColumnName] = propertyInfo.GetValue(obj, null);
                    }

                    FieldInfo fieldInfo = type.GetField(column.ColumnName);
                    if (fieldInfo != null)
                    {
                        dr[column.ColumnName] = fieldInfo.GetValue(obj);
                    }

                    //column.MaxLength = 20;
                }
                dt.Rows.Add(dr);
            }

        }



        return dt;



    }

    #endregion

    //需存入Excel、檔名(絕對路徑)
    public static KeyValuePair<string, MemoryStream> ExportExcelFromDataTable(Dictionary<string, DataTable> dt, string fileName = null)
    {
        bool r = false;
        string msg = "";
        MemoryStream s = new MemoryStream();

        try
        {
            XLWorkbook workbook = new XLWorkbook();
            var data = dt.ToList();
            foreach (var item in data)
            {
                workbook.AddWorksheet(item.Value, item.Key);
            }




            if (!string.IsNullOrEmpty(fileName))
            {
                //workbook.SaveAs(fileName);
            }
            else
            {

                workbook.SaveAs(s);
            }


            workbook.Dispose();

            r = true;


        }
        catch (Exception ex)
        {
            msg = ex.ToString();
        }

        return new KeyValuePair<string, MemoryStream>(msg, s);
    }

    #endregion

    //Table
    public static KeyValuePair<string, object> xxgetTable(object Table, Dictionary<string, string> Correspond, Dictionary<string, string> Content)
    {
        string msg = "";

        foreach (var item in Correspond)
        {
            //欄位名稱
            string title = item.Value;

            //當次的值
            string value = Content.Where(m => m.Key == item.Key).Select(m => m.Value).FirstOrDefault();

            Type model_type = Table.GetType();
            PropertyInfo model_property = model_type.GetProperty(item.Value);

            #region"判斷型態"

            //取得為該名稱的欄位型態
            if (model_property != null)
            {
                var type_name = model_property.GetMethod.ReturnType.Name;
                try
                {
                    switch (type_name)
                    {
                        case "String":

                            string string_value = value;
                            model_property.SetValue(Table, string_value);

                            break;

                        case "Int32":

                            int int_value = Convert.ToInt32(value);
                            model_property.SetValue(Table, int_value);

                            break;

                        case "Decimal":

                            decimal decimal_value = Convert.ToDecimal(value);
                            model_property.SetValue(Table, decimal_value);

                            break;

                        case "Double":

                            Double double_value = Convert.ToDouble(value);
                            model_property.SetValue(Table, double_value);

                            break;

                        case "Single":
                            Single single_value = Convert.ToSingle(value);
                            model_property.SetValue(Table, single_value);

                            break;

                        case "Datetime":

                            DateTime datetime_value = Convert.ToDateTime(value);
                            model_property.SetValue(Table, datetime_value);

                            break;
                    }
                }
                catch (Exception ex)
                {
                    msg = msg + title + "錯誤" + "<br>";
                    //有儲存失敗的 故自動停用
                    //model.visible = false;
                }
            }

            #endregion
        }


        return new KeyValuePair<string, object>(msg, Table);
    }

    #region"去除重複縣市區"

    public static string getAddress(string address, int? zip)
    {
        string r = address;
        if (zip.HasValue)
        {
            var Zip = _DB.zip_data.Where(m => m.zip == zip).FirstOrDefault();
            if (Zip != null)
            {
                r = r.Replace(Zip.city, "");
                r = r.Replace(Zip.canton, "");
                r = r.Trim(' ').Trim('-');
                r = Zip.city + Zip.canton + " - " + r;
            }
        }

        return r;
    }

    #endregion

}


