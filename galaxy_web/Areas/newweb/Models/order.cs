using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace galaxy_web.Models
{

    public class order_data
    {
        [Key]
        [DisplayName("order_id")]
        public int order_id { get; set; }

        [DisplayName("platform_id")]
        public int platform_id { get; set; }

        [DisplayName("platform_sn")]
        public string platform_sn { get; set; }

        [DisplayName("company_id")]
        public int company_id { get; set; }

        [DisplayName("status")]
        public int status { get; set; }

        [DisplayName("order_sn")]
        public string order_sn { get; set; }

        [DisplayName("order_name")]
        public string order_name { get; set; }

        [DisplayName("order_tel")]
        public string order_tel { get; set; }

        [DisplayName("order_zip")]
        public int order_zip { get; set; }

        [DisplayName("order_address")]
        public string order_address { get; set; }

        [DisplayName("send_name")]
        public string send_name { get; set; }

        [DisplayName("send_tel")]
        public string send_tel { get; set; }

        [DisplayName("send_zip")]
        public int send_zip { get; set; }

        [DisplayName("send_address")]
        public string send_address { get; set; }

        [DisplayName("create_user")]
        public string create_user { get; set; }

        [DisplayName("create_datetime")]
        public DateTime? create_datetime { get; set; }

        [DisplayName("物流公司")]
        public int goods_id { get; set; }

        //public int invoice_flag { get; set; }

        //public string invoice_title { get; set; }

        //public string invoice_no { get; set; }

        [DisplayName("國籍")]
        public string nation { get; set; }

        [NotMapped]
        public DateTime? order_datetime { get; set; }
        [NotMapped]
        public DateTime? goods_datetime { get; set; }
        [NotMapped]
        public DateTime? back_datetime { get; set; }
        [NotMapped]
        public DateTime? backed_datetime { get; set; }
        [NotMapped]
        public DateTime? invoice_datetime { get; set; }

    }

    public class order_sub
    {
        [Key]
        [DisplayName("sub_id")]
        public int sub_id { get; set; }

        
        [DisplayName("order_id")]
        public int order_id { get; set; }

        [DisplayName("product_id")]
        public int product_id { get; set; }

        [DisplayName("item_name")]
        public string item_name { get; set; }

        [DisplayName("item_no")]
        public string item_no { get; set; }

        [DisplayName("price")]
        public decimal price { get; set; }

        [DisplayName("cost")]
        public decimal cost { get; set; }

        [DisplayName("freight")]
        public decimal freight { get; set; }

        [DisplayName("num")]
        public int num { get; set; }

        [DisplayName("info")]
        public string info { get; set; }

        [DisplayName("消費方式")]
        public int pay_id { get; set; }

        [DisplayName("商品樣式")]
        public string style { get; set; }
    }

    [DisplayName("訂單狀態紀錄")]
    public class order_status
    {
        [Key]
        [DisplayName("編號")]
        public int status_id { get; set; }

        [DisplayName("訂單編號")]
        public int order_id { get; set; }

        [DisplayName("狀態")]
        public int status { get; set; }

        [DisplayName("先前狀態")]
        public int status_old { get; set; }

        [DisplayName("備註")]
        public string info { get; set; }

        [DisplayName("建立者")]
        public string create_user { get; set; }

        [DisplayName("建立時間")]
        public DateTime? create_datetime { get; set; }
    }

    [DisplayName("訂單的狀態")]
    public class order_status_flag
    {
        [Key]
        [DisplayName("flag_id")]
        public int flag_id { get; set; }

        [DisplayName("name")]
        public string name { get; set; }

        [DisplayName("inx")]
        public int inx { get; set; }

        [DisplayName("visible")]
        public Boolean visible { get; set; }

        [DisplayName("create_user")]
        public string create_user { get; set; }

        [DisplayName("create_datetime")]
        public DateTime? create_datetime { get; set; }

        [DisplayName("change_user")]
        public string change_user { get; set; }

        [DisplayName("change_datetime")]
        public DateTime? change_datetime { get; set; }
    }

    [DisplayName("發票資訊")]
    public class order_invoice
    {
        [Key]
        [DisplayName("編號")]
        public int invoice_id { get; set; }

        [DisplayName("訂單編號")]
        public int order_id { get; set; }

        [DisplayName("歸屬編號")]
        public int attribution_id { get; set; }

        [DisplayName("歸屬")]
        public int attribution { get; set; }

        [DisplayName("類型")]
        public int flag { get; set; }

        [DisplayName("臺頭")]
        public string title { get; set; }

        [DisplayName("號碼")]
        public string no { get; set; }

        [DisplayName("建立者")]
        public string create_user { get; set; }

        [DisplayName("建立時間")]
        public DateTime? create_datetime { get; set; }

        //0廠商開給我們  1我們開給消費者
        [DisplayName("對象類型")]
        public int type { get; set; }

    }

    [DisplayName("物流公司")]
    public class order_goods
    {
        [Key]
        public int goods_id { get; set; }

        public string name { get; set; }

        public int inx { get; set; }

        public bool visible { get; set; }

        public string create_uesr { get; set; }

        public DateTime? create_datetime { get; set; }

    }

    public class order_pay
    {
        [Key]
        public int pay_id { get; set; }

        public string pay_name { get; set; }

        public bool visible { get; set; }

        public string create_user { get; set; }

        public DateTime? create_datetime { get; set; }

        public string change_user { get; set; }

        public DateTime? change_datetime { get; set; }
    }

    public class order_bills
    {
        [Key]
        public int bills_id { get; set; }

        [DisplayName("廠商")]
        public int company_id { get; set; }

        [UIHint("Info_Tags")]
        [DisplayName("發票號碼")]
        public string invoice_no { get; set; }

        [DisplayName("發票日期")]
        public DateTime? invoice_datetime { get; set; }

        [DisplayName("create_user")]
        public string create_user { get; set; }

        [DisplayName("create_datetime")]
        public DateTime? create_datetime { get; set; }

        [DisplayName("change_user")]
        public string change_user { get; set; }

        [DisplayName("change_datetime")]
        public DateTime? change_datetime { get; set; }

        [UIHint("RadioBoxList")]
        [DisplayName("請款狀態")]
        public int flag { get; set; }

        [DisplayName("審核人")]
        public string check_user { get; set; }

        [DisplayName("請款時間")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? bills_datetime { get; set; }

        [UIHint("Info")]
        [DisplayName("備註")]
        [Display(Prompt = "此欄位僅能填入文字")]
        public string info { get; set; }

        [DisplayName("請款年月")]
        public string yymm { get; set; }

        //0月結 1半月結上 2半月結下
        [DisplayName("請款方式")]
        public int bills_flag { get; set; }

        [DisplayName("出貨起始日")]
        public DateTime? sdate { get; set; }

        [DisplayName("出貨最後日")]
        public DateTime? edate { get; set; }

        [DisplayName("請款單號")]
        public string order_ids { get; set; }

        [DisplayName("請款總額(cost_total)")]
        public decimal total { get; set; }

        //管理員操作
        public bool visible { get; set; }

        [DisplayName("匯款完成時間")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? finish_datetime { get; set; }

        [DisplayName("訂單國別")]
        public string nation { get; set; }

    }

    public class order_note
    {
        [Key]
        public int note_id { get; set; }

        [DisplayName("廠商")]
        public int company_id { get; set; }

        [DisplayName("order_id")]
        public int order_id { get; set; }

        [DisplayName("日期紀錄1")]
        public DateTime? note_time { get; set; }

        [DisplayName("日期紀錄2")]
        public DateTime? note_time2 { get; set; }

        public bool note_visible { get; set; }

        public bool note_visible2 { get; set; }

        [DisplayName("note_cont")]
        public string note_cont { get; set; }

        [DisplayName("note_cont")]
        public string note_cont2 { get; set; }

        [DisplayName("create_user")]
        public string create_user { get; set; }

        [DisplayName("create_datetime")]
        public DateTime? create_datetime { get; set; }

        [DisplayName("change_user")]
        public string change_user { get; set; }

        [DisplayName("change_datetime")]
        public DateTime? change_datetime { get; set; }

        public bool visible { get; set; }


    }

}