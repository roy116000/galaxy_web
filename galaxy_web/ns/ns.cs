using galaxy_web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail; //mail
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization; //JavaScriptSerializer
using System.Web.Security;
//Encoding

public class GetParams
{
    public List<GetParam> data { get; set; }
}

public class GetParam
{
    public string PageName { get; set; }
    public int B_Img_W { get; set; }
    public int B_Img_H { get; set; }
    public int S_Img_W { get; set; }
    public int S_Img_H { get; set; }
    public int Pages { get; set; }
    public int G_Img_W { get; set; }
    public int G_Img_H { get; set; }
    public string Img_Info { get; set; }
}

public class NW
{
    public static string ssjgrowl = "jgrowl";
    public static string ssTitle = "Galaxy_web";

    public static string point = "N0";
    public static string WEB_URL = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + "/";
    public static string IMG_URL = "http://www.galaxylife.cc/";// 
    public static string ssLoginer = "Galaxy_web";
    public static string ssNophoto = "/images/nophoto/no_photo.jpg";

    public static string[] nation_list = new string[] { "TW", "CN" };

    //編輯器專用
    public static string controller = "";
    public static string id = "";

    //ERP成本比率
    public static decimal erp_sp = decimal.Parse("0.4");

    //取唯一值秒+8馬
    public static string GetUniqueKey()
    {
        int maxSize = 8;
        //int minSize = 5;
        char[] chars = new char[62];
        string a;
        a = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        chars = a.ToCharArray();
        int size = maxSize;
        byte[] data = new byte[1];
        RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
        crypto.GetNonZeroBytes(data);
        size = maxSize;
        data = new byte[size];
        crypto.GetNonZeroBytes(data);
        StringBuilder result = new StringBuilder(size);
        foreach (byte b in data)
        {
            result.Append(chars[b % (chars.Length - 1)]);
        }
        return result.ToString();
    }

    //取得請款明細的物流資訊
    public static string goods_info(int? order_id)
    {
        var db = new _dbContext();
        string name = string.Empty;
        string info = string.Empty;
        string date = string.Empty;
        string result = " , , ";
        try
        {
            if (order_id.HasValue)
            {
                //訂單資料
                var order = db.order_data.Where(m => m.order_id == order_id).FirstOrDefault();

                if (order != null)
                {
                    //貨運公司
                    name = db.order_goods.Where(m => m.goods_id == order.goods_id).Select(m => m.name).FirstOrDefault() ?? "無";

                    var goods = db.order_status.Where(m => m.order_id == order.order_id && m.status == 2 && m.status_old != 3).OrderByDescending(m => m.status_id);

                    //單號
                    info = goods.Select(m => m.info).FirstOrDefault() ?? "無";

                    ////出貨日
                    //date = goods.Select(m => m.create_datetime.Value.ToString("yyyy/MM/dd")).FirstOrDefault() ?? "無";

                    result = name + "," + info;
                }
            }
        }
        catch { }

        return result;
    }

    //取得請款明細的訂單金額
    public static order_sub bills_sub(int? order_id)
    {
        var db = new _dbContext();
        var order = new order_sub { };
        try
        {
            if (order_id.HasValue)
            {
                //訂單明細資料
                order = db.order_sub.Where(m => m.order_id == order_id).FirstOrDefault();
                
            }
        }
        catch { }

        return order;
    }
    //取得請款明細的商品廠商編號
    public static product_data bills_product(int? product_id)
    {
        var db = new _dbContext();
        var product = new product_data { };
        try
        {
            if (product_id.HasValue)
            {
                //訂單明細資料
                product = db.product_data.Where(m => m.product_id == product_id).FirstOrDefault();

            }
        }
        catch { }

        return product;
    }

    public static string bills_flag(int flag = 0)
    {
        string r = string.Empty;
        try
        {
            switch (flag)
            {
                case 0:
                    r = "未請款";
                    break;
                case 1:
                    r = "請款中";
                    break;
                case 2:
                    r = "確認請款";
                    break;
                case 3:
                    r = "匯款完成";
                    break;
            }
        }
        catch { }

        return r;
    }
    public static string[] platforms = new string[] { "44,翻購", "54,健康資訊讚", "64,94cheap" };
    public static string platform(int platform_id = 0)
    {
        string r = "";
        try
        {
            switch (platform_id)
            {
                case 44:
                    r = "翻購";
                    break;
                case 54:
                    r = "健康資訊讚";
                    break;
                case 64:
                    r = "94cheap";
                    break;
            }
        }
        catch { }

        return r;
    }
    public static string convert_platform(string s)
    {
        string r = "";

        if (!string.IsNullOrEmpty(s))
        {
            var sr = s.Split(',');
            foreach (var item in sr.OrderBy(m => m))
            {
                if (!string.IsNullOrEmpty(item))
                {
                    string n = platform(Convert.ToInt32(item));
                    r = r + n + ",";
                }
            }
            r = r.TrimEnd(',');
        }

        return r;
    }


    #region "狀態顯示"

    //送貨時段
    public static Dictionary<int, string> Send_time1 = new Dictionary<int, string>() 
    { 
        { 0, "不限" }, 
        { 1, "上班日" }, 
        { 2, "非上班日" }, 
    };

    //送貨時間
    public static Dictionary<int, string> Send_time2 = new Dictionary<int, string>() 
    { 
        { 0, "不限" }, 
        { 1, "早上(09:00-12:00)" }, 
        { 2, "下午(14:00-17:00)" }, 
        { 3, "晚上(17:00-19:00)" }
    };

    //國家
    public static Dictionary<string, string> Nation = new Dictionary<string, string>() { { "", "請選擇：國籍" }, { "TW", "TW" }, { "CN", "CN" } };

    //商品狀態
    public static Dictionary<int, string> Status_Arr = new Dictionary<int, string>() { { 0, "建檔中" }, { 1, "審核中" }, { 2, "審核完成" }, { 3, "上架" }, { 4, "下架" }, { 5, "作廢" } };

    //訂單狀態
    //public static Dictionary<int, string> Flag_Arr = new Dictionary<int, string>() { { 0, "訂單成立" }, { 1, "付款完成" }, { 2, "準備出貨" }, { 3, "出貨完成" }, { 4, "退貨申請" }, { 5, "退款申請" }, { 6, "退貨退款完成" }, { 7, "訂單完成" }, { 99, "訂單作廢" } };
    public static Dictionary<int, string> Flag_Arr = new Dictionary<int, string>() { { 0, "訂單成立" }, { 1, "請出貨" }, { 2, "出貨完成" }, { 3, "申請退貨" }, { 4, "退貨完成" }, { 5, "開立發票" }, { 6, "訂單完成" }, { 999, "訂單作廢" } };
    public static Dictionary<int, string> Flag_ArrS = new Dictionary<int, string>() { { 0, "訂單成立" }, { 1, "請出貨(等待聯絡)" }, { 2, "出貨完成(療程結束)" }, { 3, "申請退貨(退單)" }, { 4, "退貨(退單)完成" }, { 5, "開立發票" }, { 6, "訂單完成" }, { 999, "訂單作廢" } };


    //付款方式
    public static Dictionary<int, string> Pay_Flag = new Dictionary<int, string>() { { 1, "購物金" }, { 2, "線上刷卡" } };

    //性別
    public static Dictionary<int, string> Sex = new Dictionary<int, string>() { { 1, "先生" }, { 0, "小姐" } };



    #region"物流公司名稱"

    public static Dictionary<int, string> Goods_Name = new Dictionary<int, string>() 
    { 
        { 0, "未出貨" }, 
        { 1, "中華郵政" }, 
        { 2, "黑貓宅急便" },
        { 3, "台灣宅配通" }, 
        { 4, "新竹物流" }, 
        { 5, "嘉里大榮物流(大榮貨運)" },
        { 6, "便利帶" }, 
        { 7, "日通快遞" }, 
        { 8, "全速配" },
        { 9, "同同宅配" }, 
        { 10, "國陽郵遞" }, 
        { 11, "第一郵控" },
        { 12, "統群物流" }, 
        { 13, "通盈貨運" }, 
        { 14, "速遞家" },
        { 15, "超峰快遞" }, 
        { 16, "錸乾物流" }, 
        { 17, "尚發運通" },
        { 18, "雅仕快遞" }, 
        { 19, "驥騰物流" }, 
        { 20, "巨航快遞" },
        { 21, "台快快遞" },
        { 22, "亞風速遞" },
        { 23, "中連貨運" },
        { 24, "祥億貨運" },
        { 25, "順豐速運" },
        { 26, "永騰物流" },
        { 27, "速利貨運" },
        { 28, "咏高交通" },
        { 29, "速遞物流" },
        { 30, "揚績國際有限公司(揚績物流)" },
        { 31, "聯新快遞" },
        { 32, "飛際快遞" },
        { 33, "日鎰快遞" },
        { 34, "信速通運" },
        { 35, "大誠/新航線快遞" },
        { 36, "聯盟物流(駿通企業)" },
        { 37, "優速快遞" },
        { 38, "新航快遞" },
        { 39, "來來快遞" },
        { 40, "丞琦貨運" },
        { 41, "棟樑物流" },
        { 42, "明箭快遞" },
        { 43, "神行(中國)快遞" },
        { 44, "協合展業" },
        { 45, "強訊/佑訊郵通(上大郵通)" },
        { 46, "進南貨運有限公司" },
        { 47, "迪比翼快遞DPEX" },
        { 48, "CTC全統物流" },
        { 49, "詹姆士國際快遞" },
        { 50, "天成快遞" },
        { 51, "全球快遞" },
        { 52, "自行配送" },
        { 53, "其他" }
    };

    #endregion

    #region"發票"

    //發票-類型
    public static Dictionary<int, string> Invoice_Name = new Dictionary<int, string>() { { 1, "二聯式" }, { 2, "三聯式" } };

    //發票-歸屬
    public static Dictionary<int, string> Invoice_Attr = new Dictionary<int, string>() { { 0, "自取" }, { 1, "捐贈" } };

    #endregion

    #endregion

    #region"時間專區"

    //當下
    public static DateTime Now
    {
        get
        {
            DateTime sD = DateTime.Now;
            //測試
            //sD = sD.AddDays(1);
            return sD;
        }
    }

    //當天
    public static DateTime Today
    {
        get
        {
            DateTime sD = Now.AddHours(0);
            //測試
            //sD = sD.AddDays(1);
            return new DateTime(sD.Year, sD.Month, sD.Day, 0, 0, 0);
        }
    }

    //明天
    public static DateTime Tomorrow
    {
        get
        {
            DateTime sD = Today.AddDays(1);
            //測試
            //sD = sD.AddDays(1);
            return new DateTime(sD.Year, sD.Month, sD.Day, 0, 0, 0);
        }
    }

    #endregion

    #region "取得登入者資訊"

    public static sys_admin_data admin(string id)
    {

        int admin_id = 0;
        if (!string.IsNullOrEmpty(id))
        {
            try
            {
                admin_id = int.Parse(id);
            }
            catch { }
        }
        _dbContext db = new _dbContext();
        var data = (from m in db.sys_admin_data where m.admin_id == admin_id && m.visible == true select m).FirstOrDefault();

        if (data != null)
        {
            return data;
        }
        else
        {
            return new sys_admin_data();
        }


    }
    public static sys_admin_data admin2(string id)
    {

        int admin_id = 0;
        if (!string.IsNullOrEmpty(id))
        {
            try
            {
                admin_id = int.Parse(id);
            }
            catch { }
        }
        _dbContext db = new _dbContext();
        var data = (from m in db.sys_admin_data where m.admin_id == admin_id select m).FirstOrDefault();

        if (data != null)
        {
            return data;
        }
        else
        {
            return new sys_admin_data();
        }


    }
    #endregion

    #region "取得流水號"
    public static string getImgWater()
    {
        Random Rnd = new Random();
        string str = DateTime.Now.ToString("yyyyMMddHHmmssffff") + Rnd.Next(10, 99);

        return str;
    }
    #endregion

    #region "圖片類型"
    public static Boolean getFileType(HttpPostedFileBase FileData, int sys)
    {
        Boolean Type = false;
        switch (sys)
        {
            case 1: //圖片
                Type = Regex.IsMatch(Path.GetExtension(FileData.FileName), "^[.](jpg|png|jpeg|gif|bmp)$", RegexOptions.IgnoreCase);
                break;
            case 2: //檔案
                Type = Regex.IsMatch(Path.GetExtension(FileData.FileName), "^[.](7z|zip|txt|doc|xls|ppt|wmv|avi|mpeg|pdf|zip|rar|docx|xlsx)$", RegexOptions.IgnoreCase);
                break;
            case 3: //execl檔
                Type = Regex.IsMatch(Path.GetExtension(FileData.FileName), "^[.](xls|xlsx)$", RegexOptions.IgnoreCase);
                break;

        }
        return Type;
    }
    #endregion

    #region "權限List_ajax的修正"
    public static string getActionID(string action)
    {
        action = action.Replace("_ajax", "");
        switch (action)
        {
            case "ConfigEdit":
                action += "/0";
                break;
            case "InfoEdit":
                action += "/0";
                break;
        }
        return action;
    }
    #endregion

    #region "設定"

    public static GetParam GetParam(string name)
    {
        string jsonx = "";

        jsonx = "{";
        jsonx += " data:[ ";
        jsonx += "      {'PageName':'about','B_Img_W':300,'B_Img_H':300,'S_Img_W':40,'S_Img_H':40,'Pages':20,'G_Img_W':0,'G_Img_H':0},";
        jsonx += "      {'PageName':'news','B_Img_W':300,'B_Img_H':300,'S_Img_W':195,'S_Img_H':195,'Pages':20,'G_Img_W':0,'G_Img_H':0},";
        jsonx += "      {'PageName':'news2','B_Img_W':300,'B_Img_H':300,'S_Img_W':195,'S_Img_H':195,'Pages':20,'G_Img_W':0,'G_Img_H':0},";
        jsonx += "      {'PageName':'news3','B_Img_W':300,'B_Img_H':300,'S_Img_W':195,'S_Img_H':195,'Pages':20,'G_Img_W':0,'G_Img_H':0},";
        jsonx += "      {'PageName':'news4','B_Img_W':300,'B_Img_H':300,'S_Img_W':195,'S_Img_H':195,'Pages':20,'G_Img_W':0,'G_Img_H':0},";
        jsonx += "      {'PageName':'order','B_Img_W':300,'B_Img_H':300,'S_Img_W':176,'S_Img_H':176,'Pages':20,'G_Img_W':0,'G_Img_H':0},";
        jsonx += "      {'PageName':'purse','B_Img_W':300,'B_Img_H':300,'S_Img_W':176,'S_Img_H':176,'Pages':20,'G_Img_W':0,'G_Img_H':0},";
        jsonx += "      {'PageName':'bills','B_Img_W':300,'B_Img_H':300,'S_Img_W':176,'S_Img_H':176,'Pages':20,'G_Img_W':0,'G_Img_H':0},";
        jsonx += "      {'PageName':'product','B_Img_W':500,'B_Img_H':500,'S_Img_W':222,'S_Img_H':222,'Pages':20,'G_Img_W':500,'G_Img_H':100},";
        jsonx += "      {'PageName':'member','B_Img_W':350,'B_Img_H':350,'S_Img_W':100,'S_Img_H':100,'Pages':20,'G_Img_W':0,'G_Img_H':0},";
        jsonx += "      {'PageName':'adv','B_Img_W':150,'B_Img_H':150,'S_Img_W':0,'S_Img_H':0,'Pages':20,'G_Img_W':0,'G_Img_H':0},";
        jsonx += "      {'PageName':'driver','B_Img_W':500,'B_Img_H':500,'S_Img_W':150,'S_Img_H':150,'Pages':20,'G_Img_W':0,'G_Img_H':0},";
        jsonx += "      {'PageName':'question','B_Img_W':300,'B_Img_H':300,'S_Img_W':100,'S_Img_H':100,'Pages':20,'G_Img_W':0,'G_Img_H':0},";
        jsonx += "      {'PageName':'contact','B_Img_W':300,'B_Img_H':300,'S_Img_W':100,'S_Img_H':100,'Pages':20,'G_Img_W':0,'G_Img_H':0},";
        jsonx += "      {'PageName':'album','B_Img_W':500,'B_Img_H':500,'S_Img_W':130,'S_Img_H':130,'Pages':20,'G_Img_W':0,'G_Img_H':0},";
        jsonx += "      {'PageName':'edm','B_Img_W':500,'B_Img_H':500,'S_Img_W':130,'S_Img_H':130,'Pages':20,'G_Img_W':0,'G_Img_H':0},";
        jsonx += "      {'PageName':'onlinevote','B_Img_W':300,'B_Img_H':300,'S_Img_W':195,'S_Img_H':195,'Pages':20,'G_Img_W':0,'G_Img_H':0},";
        jsonx += "      {'PageName':'activity','B_Img_W':300,'B_Img_H':300,'S_Img_W':195,'S_Img_H':195,'Pages':20,'G_Img_W':0,'G_Img_H':0}";
        jsonx += "  ]";
        jsonx += "}";

        //反序列JSON
        GetParams itemx = new JavaScriptSerializer().Deserialize<GetParams>(jsonx);

        //宣告變數
        GetParam item = new GetParam();

        foreach (var json in itemx.data)
        {
            if (json.PageName == name)
            {
                item.B_Img_W = json.B_Img_W;
                item.B_Img_H = json.B_Img_H;
                item.S_Img_W = json.S_Img_W;
                item.S_Img_H = json.S_Img_H;
                item.Pages = json.Pages;
                item.G_Img_W = json.G_Img_W;
                item.G_Img_H = json.G_Img_H;
                item.Img_Info = " *圖片建議大小：" + json.B_Img_W + " x " + json.B_Img_H + " px";
            }
        }

        return item;
    }

    //是為否
    public static string echoBool(bool b, string xx1, string xx2)
    {
        string echoBool = null;

        if (b == true)
        {
            echoBool = xx1;
        }
        else
        {
            echoBool = xx2;
        }

        return echoBool;

    }

    //時間
    public static object spliteDate(object str, int i)
    {
        string spl = "";
        switch (i)
        {
            case 1:
                spl = string.Format("{0:yyyy/MM/dd}", str);
                break;
            case 2:
                spl = string.Format("{0:yyMMdd}", str);
                break;
        }
        return spl;

    }

    //取代
    public static string ReplaceAt(string input, string newChar)
    {
        if (input == null)
        {
            throw new System.ArgumentNullException();
        }

        char[] chars = input.ToCharArray();

        if (chars.Length > 1)
        {
            switch (chars.Length)
            {
                case 1:
                    chars[0] = Convert.ToChar(input);
                    break;
                case 2:
                    chars[1] = Convert.ToChar(newChar);
                    break;
                default:
                    for (var i = 1; i < chars.Length - 1; i++)
                    {
                        chars[i] = Convert.ToChar(newChar);
                    };
                    chars[0] = chars[0];
                    chars[chars.Length - 1] = chars[chars.Length - 1];
                    break;
            }
        }
        return new string(chars);
    }

    //去除Html
    public static string splitHtml(string html)
    {
        string split = html;

        if (split == null) { return split; }

        Regex regex = new System.Text.RegularExpressions.Regex("<[^>]*>");
        split = regex.Replace(split, "");

        return split;
    }

    //縮字
    public static string word(string key, int count, string mode)
    {
        string split = splitHtml(key);
        string[] input = { " ", "　", "\r", "\n", "\r\n", "&nbsp;", "&#12288;", "&hellip;" };

        if (split == null) { return split; }

        foreach (string xx in input)
        {
            split = split.Replace(xx, "");
        }

        Encoding big5 = Encoding.GetEncoding("big5");
        byte[] b = big5.GetBytes(split);

        count *= 2;

        if (b.Length <= count)
        {
            return split;
        }
        else
        {
            string res = big5.GetString(b, 0, count);
            if (!big5.GetString(b).StartsWith(res))
            {
                res = big5.GetString(b, 0, count - 1);
            }
            if (string.IsNullOrEmpty(split))
            {
                mode = "";
            }
            return res += mode;
        }
    }

    //個資隱藏
    public static string hideword(string word, int y, int total = 0)
    {
        if (string.IsNullOrEmpty(word)) { word = ""; }
        int number = word.Length;
        string first = word;
        if (number > y)
        {
            first = word.Substring(0, y);
            int num = (number - y);
            if (total > 0 && total < num) { num = total; }
            for (int i = 0; i < (num); i++)
            {
                first += "O";
            }
        }
        return first;
    }

    //亂數
    public static string myCode(int textLength)
    {
        const string Chars = "ABCDEFGHIJKLMNPOQRSTUVWXYZ0123456789";

        Random random = new Random();

        var result = new string(
            Enumerable.Repeat(Chars, textLength)
            .Select(x => x[random.Next(x.Length)]).ToArray()
            );

        return result;

    }

    #endregion

    #region 字串分類

    public static string[] Split(string Text, string separator)
    {
        string[] sp = new string[] { separator };
        return Text.Split(sp, StringSplitOptions.RemoveEmptyEntries);
    }

    public static string[] Split(string Text, char separator)
    {
        char[] sp = new char[] { separator };
        return Text.Split(sp, StringSplitOptions.RemoveEmptyEntries);
    }

    #endregion

    #region "Log紀錄"

    public static void Log(string message)
    {
        string dd = DateTime.Now.ToString("yyyyMMdd");
        string folder = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\" + "nwlog\\";
        string filename = folder + dd + ".txt";
        try
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string swLine = message;
            var sw = new System.IO.StreamWriter(filename, true);
            sw.WriteLine(swLine);
            sw.Close();
        }
        catch
        {

        }

    }

    #endregion

    #region "寫錯誤nwlog檔"
    public static void nwLog(System.Web.Mvc.ExceptionContext filterContext)
    {
        string dd = DateTime.Now.ToString("yyyyMMdd");
        string folder = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\" + "nwlog\\";
        string filename = folder + dd + ".txt";
        try
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            //string ip = filterContext.HttpContext.Request.UserHostAddress;
            string ip = filterContext.HttpContext.Request.ServerVariables["REMOTE_ADDR"].ToString();
            string actionName = filterContext.RouteData.Values["action"].ToString();
            string controllerName = filterContext.RouteData.Values["controller"].ToString();
            string id = (filterContext.RouteData.Values["id"] == null ? "" : filterContext.RouteData.Values["id"].ToString());
            string err = filterContext.Exception.Message;
            string e = filterContext.Exception.ToString().Replace("\r\n", "||");
            string swLine = DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "\t" + ip + "\t" + actionName + "\t" + controllerName + "\t" + id + "\t" + err + "\t" + e;


            var sw = new System.IO.StreamWriter(filename, true);
            sw.WriteLine(swLine);
            sw.Close();

        }
        catch
        {
        }
    }
    #endregion

    #region"信箱過濾字元"

    public static string mailFilter(string address)
    {
        string sR = address != null ? address : "";
        string[] arrStr = { " ", "、", "(", ")" };

        foreach (string s in arrStr)
        {
            sR = sR.Replace(s, "");
        }

        return sR;
    }

    #endregion

    #region "過濾前台內頁顯示字元"

    public static string showTitleHtml(string str)
    {
        System.Text.StringBuilder sR = new StringBuilder();

        if (str != null)
        {
            sR.Append(str);
        }
        //sR.Append(str != null ? str : "");
        //System.Text.StringBuilder sR = str != null ? new StringBuilder(str) : new StringBuilder();

        string[] arrStr = { " ", "%", "-", "/", "\\", "?", "@", "~", "*", "+", "&", ":", "\"" };

        foreach (string s in arrStr)
        {
            sR = sR.Replace(s, "");
        }

        return sR.ToString();
    }

    #endregion

    #region "取得client IP Mobile"
    public static string GetIP(HttpRequestBase request)
    {
        string ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        if (ip == null || ip.Trim() == string.Empty)
        {
            ip = request.ServerVariables["REMOTE_ADDR"];
        }
        return ip;
    }
    public static bool isMobile(HttpRequestBase request)
    {
        string u = request.ServerVariables["HTTP_USER_AGENT"];
        Regex b = new Regex(@"android.+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        Regex v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(di|rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        if ((b.IsMatch(u) || v.IsMatch(u.Substring(0, 4))))
        {
            return true;
        }
        else
            return false;
    }

    public static bool isMobileNew()
    {
        bool r = false;

        string u = HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"];
        Regex b = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        Regex v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        if ((b.IsMatch(u) || v.IsMatch(u.Substring(0, 4))))
        {
            r = true;
        }

        return r;

    }

    #endregion

    #region "定義正則表達式函數"
    public static Boolean isRegex(string str, String pattern)
    {
        Regex Regex1 = new Regex(pattern, RegexOptions.IgnoreCase);
        return Regex1.IsMatch(str);
    }
    #endregion

    #region "取得虛擬帳號"
    //取得台新虛擬帳號
    public static string getBankSN(int o, int m)
    {

        //台新銀行為14碼 9 6 7 5 7 0 0 0 1 3 2 2 0 3

        string approvecode = "";
        string sn = "6885"; //帳號
        string sDate = DateTime.Today.AddDays(2).ToString("MMdd"); //匯款截止日期

        //測試
        //sDate = "0114";   //時間 月日
        //string sn = "8108"; //帳號

        int cwww = o; //訂單單號
        string power = "4567891234567";
        string money_power = "87654321";

        int x1 = 0, x2 = 0, x3 = 0;
        string c1 = sn + sDate + cwww.ToString("00000"), c2 = m.ToString("00000000");

        for (int i = 1; i <= c1.Length; i++)
        {
            int j = Convert.ToInt32(c1.Substring(i - 1, 1));
            int k = Convert.ToInt32(power.Substring(i - 1, 1));
            x1 += (j * k);
        }
        x1 = 10 - Convert.ToInt32(x1.ToString().Right(1));
        x1 = Convert.ToInt32(x1.ToString().Right(1));

        for (int i = 1; i <= 8; i++)
        {
            int l = Convert.ToInt32(c2.Substring(i - 1, 1));
            int n = Convert.ToInt32(money_power.Substring(i - 1, 1));
            x2 += (l * n);
        }
        x2 = 10 - Convert.ToInt32(x2.ToString().Right(1));
        x2 = Convert.ToInt32(x2.ToString().Right(1));

        x3 = Convert.ToInt32((x1 + x2).ToString().Right(1));

        approvecode = c1 + x3.ToString();
        return approvecode;
    }

    #endregion

    #region "寫虛擬帳號回傳錯誤log檔"
    public static void VirtualLog(string msg)
    {
        string dd = DateTime.Now.ToString("yyyyMMdd");
        string folder = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\" + "nwlog\\";
        string filename = folder + dd + ".txt";
        try
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }



            string swLine = DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "\t" + msg + "\t";


            var sw = new System.IO.StreamWriter(filename, true);
            sw.WriteLine(swLine);
            sw.Close();

        }
        catch
        {
        }
    }
    #endregion

    public static string getDate(DateTime? d, string style = "yyyy-MM-dd")
    {
        string s = "";
        if (d.HasValue)
        {
            s = d.Value.ToString(style);
        }
        return s;
    }

    #region "寄信"


    public enum MailFlag : int { 消費者_購物通知信 = 1, 管理者_購物通知信, 消費者_付款成功通知信, 管理者_付款成功_通知出貨, 消費者_付款失敗通知信, 管理者_付款失敗通知信, 管理者_廠商新增成功通知信, 廠商_付款成功_通知出貨 };


    //public static bool sendMail(string mailTo, string Subject, string Body)
    //{
    //    //return sendMail("tfseshop@sinon.com.tw","台灣楓康購-系統通知信", mailTo, Subject,Body);
    //    return sendMail("shop@netsourcer.com.tw", "健康購-系統通知信", mailTo, Subject, Body);
    //}

    public static bool sendMail(string mailFrom, string mailFromName, string mailTo, string Subject, string Body)
    {
        bool isSuccess = false;

        try
        {
            //if (mailTo.Contains("lisalinallan520520@yahoo.com.tw"))
            //{
            //    mailTo = "hijieva@gmail. com";
            //}

            SmtpClient smtp = new SmtpClient();
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(mailFrom, mailFromName);  // 寄件者
            int i = 0;
            foreach (var ss in mailTo.Split(';'))
            {
                var m = ss.Replace("\r\n", "");
                if (i == 0)
                {
                    mail.To.Add(new MailAddress(m));    //收件者
                }
                else
                {
                    mail.CC.Add(new MailAddress(m));
                }
                i += 1;
            }
            mail.Subject = Subject; // 主旨
            mail.Body = Body; // 內文
            mail.IsBodyHtml = true;
            mail.SubjectEncoding = UTF8Encoding.UTF8;
            mail.BodyEncoding = UTF8Encoding.UTF8;

            smtp.Send(mail);
            isSuccess = true;


        }
        catch
        {
            isSuccess = false;
        }

        return isSuccess;
    }

    //mail發送
    public static string xsendMail(string mailto, string aFrom, string subject, string body)
    {
        //收、寄、主旨、內容
        try
        {
            string smtp = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["mailsmtp"].ToString();

            string[] arr = Split(smtp, ",");

            SmtpClient SmtpServer = new SmtpClient(arr[0].ToString());
            SmtpServer.Port = 25;
            SmtpServer.Credentials = new System.Net.NetworkCredential(arr[3].ToString(), arr[4].ToString());
            SmtpServer.EnableSsl = true;


            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(aFrom);
            mail.To.Add(mailto);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            mail.SubjectEncoding = UTF8Encoding.UTF8;
            mail.BodyEncoding = UTF8Encoding.UTF8;

            SmtpServer.Send(mail);
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return "";
    }


    //public static void Othsend(int mail_id, int order_id)
    //{

    //    _dbContext db = new _dbContext();

    //    var mailData = db.mail_group.Where(m => m.group_id == mail_id).FirstOrDefault();

    //    var order = db.order_data.Where(m => m.order_id == order_id).FirstOrDefault();
    //    var member = db.member_data.Where(m => m.member_id == order.member_id).FirstOrDefault();
    //    var company = db.company_data.Where(m => m.company_id == order.company_id).FirstOrDefault();

    //    string subjcet = mailData.subject;
    //    string body = mailData.body;

    //    switch (mail_id)
    //    {
    //        //退貨通知信
    //        case 11:
    //            body = body.Replace("{company_name}", company.name);
    //            body = body.Replace("{order_id}", order_id.ToString());
    //            body = body.Replace("{back_datetime}", order.back_datetime.Value.ToString("yyyy/MM/dd"));
    //            sendMail(company.mail + ";" + mailData.cc, subjcet, body);
    //            break;
    //        //取消訂單通知信
    //        case 12:
    //            body = body.Replace("{company_name}", company.name);
    //            body = body.Replace("{order_id}", order_id.ToString());
    //            body = body.Replace("{back_datetime}", order.back_datetime.Value.ToString("yyyy/MM/dd"));
    //            sendMail(company.mail + ";" + mailData.cc, subjcet, body);
    //            break;
    //        //退貨完成通知信
    //        case 13:
    //            body = body.Replace("{name}", member.name);
    //            body = body.Replace("{order_id}", order.order_sn);
    //            body = body.Replace("{back_datetime}", order.back_datetime.Value.ToString("yyyy/MM/dd"));
    //            body = body.Replace("{backok_datetime}", order.backok_datetime != null ? order.backok_datetime.Value.ToString("yyyy/MM/dd") : DateTime.Today.ToString("yyyy/MM/dd"));
    //            sendMail(member.mail, subjcet, body);
    //            break;
    //        //退刷完成
    //        case 14:
    //            body = body.Replace("{name}", member.name);
    //            body = body.Replace("{order_id}", order.order_sn);
    //            body = body.Replace("{back_datetime}", order.back_datetime.Value.ToString("yyyy/MM/dd"));
    //            body = body.Replace("{backok_datetime}", DateTime.Today.ToString("yyyy/MM/dd"));
    //            sendMail(member.mail, subjcet, body);
    //            break;
    //        //退刷申請
    //        case 17:
    //            body = body.Replace("{order_id}", order_id.ToString());
    //            sendMail(mailData.cc, subjcet, body);
    //            break;

    //    }
    //}

    #endregion


    //轉成陣列
    public static string[] dotToArrString(string s, int num = 3)
    {
        string[] arrRN = new string[num]; ;
        if (!string.IsNullOrEmpty(s))
        {
            string[] arr = s.Split(',');
            for (int i = 0; i < arr.Length; i++)
            {
                string[] arr2 = arr[i].Split(':');
                switch (arr2[0])
                {
                    case "0":
                    case "常溫運費":
                        arrRN[0] = arr2[1];
                        break;
                    case "1":
                    case "冷藏運費":
                        arrRN[1] = arr2[1];
                        break;
                    case "2":
                    case "冷凍運費":
                        arrRN[2] = arr2[1];
                        break;
                }
            }
        }


        return arrRN;
    }

    public static int[] dotToArrInt(string s, int num = 3)
    {
        int[] arrRN = new int[num]; ;
        if (!string.IsNullOrEmpty(s))
        {
            string[] arr = s.Split(',');
            for (int i = 0; i < arr.Length; i++)
            {
                string[] arr2 = arr[i].Split(':');
                switch (arr2[0])
                {
                    case "0":
                    case "常溫運費":
                        arrRN[0] = Convert.ToInt32(arr2[1]);
                        break;
                    case "1":
                    case "冷藏運費":
                        arrRN[1] = Convert.ToInt32(arr2[1]);
                        break;
                    case "2":
                    case "冷凍運費":
                        arrRN[2] = Convert.ToInt32(arr2[1]);
                        break;
                }
            }
        }


        return arrRN;
    }

    //交易失敗log檔
    public static void PayErrnwLog(string err)
    {
        string dd = DateTime.Now.ToString("yyyyMMdd");
        string folder = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\" + "nwlog\\";
        string filename = folder + dd + ".txt";

        var sw = new System.IO.StreamWriter(filename, true);
        sw.WriteLine(err);
        sw.Close();


    }

    #region"MD5"

    public static string MD5_Key = "netsourcer";

    public static string MD5(string input)
    {
        // step 1, calculate MD5 hash from input
        System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        byte[] hash = md5.ComputeHash(inputBytes);

        // step 2, convert byte array to hex string
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < hash.Length; i++)
        {
            sb.Append(hash[i].ToString("X2"));
        }
        return sb.ToString();
    }

    #endregion

    #region"DES"

    private static string DES_Key = "kgBMf5c6";

    #region DESEnCode DES 加密
    public static string DESEnCode(string pToEncrypt)
    {
        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        byte[] inputByteArray = Encoding.GetEncoding("UTF-8").GetBytes(pToEncrypt);

        des.Key = ASCIIEncoding.ASCII.GetBytes(DES_Key);
        des.IV = ASCIIEncoding.ASCII.GetBytes(DES_Key);
        MemoryStream ms = new MemoryStream();
        CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);

        cs.Write(inputByteArray, 0, inputByteArray.Length);
        cs.FlushFinalBlock();

        StringBuilder ret = new StringBuilder(); foreach (byte b in ms.ToArray())
        {
            ret.AppendFormat("{0:X2}", b);
        }

        ret.ToString();
        return ret.ToString();
    }
    #endregion

    #region DESDeCode DES 解密
    public static string DESDeCode(string pToDecrypt)
    {
        string r = "";

        if (!string.IsNullOrEmpty(pToDecrypt))
        {

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            byte[] inputByteArray = new byte[pToDecrypt.Length / 2]; for (int x = 0; x < pToDecrypt.Length / 2; x++)
            {
                int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16)); inputByteArray[x] = (byte)i;
            }

            des.Key = ASCIIEncoding.ASCII.GetBytes(DES_Key); des.IV = ASCIIEncoding.ASCII.GetBytes(DES_Key);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length); cs.FlushFinalBlock();

            StringBuilder ret = new StringBuilder();

            r = System.Text.Encoding.UTF8.GetString(ms.ToArray());
        }


        return r;
    }
    #endregion

    #endregion

    //信箱確認
    public static bool checkMail(string mail)
    {
        bool isMail = false;

        string pattern = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4})$";

        Regex r = new Regex(pattern);
        Match m = r.Match(mail);

        isMail = m.Success;

        return isMail;
    }

}

public static class MyExtensions
{
    /// <summary>
    /// Returns the last few characters of the string with a length
    /// specified by the given parameter. If the string's length is less than the
    /// given length the complete string is returned. If length is zero or
    /// less an empty string is returned
    /// </summary>
    /// <param name="s">the string to process/// <param name="length">Number of characters to return/// <returns></returns>
    public static string Right(this string s, int length)
    {
        length = Math.Max(length, 0);

        if (s.Length > length)
        {
            return s.Substring(s.Length - length, length);
        }
        else
        {
            return s;
        }
    }
}
