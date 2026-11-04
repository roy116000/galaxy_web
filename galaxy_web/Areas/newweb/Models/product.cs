using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace galaxy_web.Models
{

    public class product_data
    {
        [Key]
        [DisplayName("product_id")]
        public int product_id { get; set; }

        [UIHint("SelectList")]
        [DisplayName("廠商")]
        public int company_id { get; set; }

        [UIHint("SelectList")]
        [DisplayName("分類")]
        public int sub_id { get; set; }

        [UIHint("SelectList_chained_Tags")]
        [DisplayName("分類")]
        public string group_ids { get; set; }

        [DisplayName("商品品牌")]
        public string web_name { get; set; }

        [DisplayName("商品名稱")]
        public string item_name { get; set; }

        [DisplayName("發票商品名稱")]
        public string item_invoice_name { get; set; }

        [DisplayName("商品貨號")]
        public string item_no { get; set; }

        [DisplayName("ERP貨號")]
        public string erp_no { get; set; }

        [DisplayName("成本")]
        public decimal cost { get; set; }

        [DisplayName("運費")]
        public decimal freight { get; set; }

        [DisplayName("商品定價")]
        public decimal sprice { get; set; }

        [DisplayName("商品售價")]
        public decimal price { get; set; }

        [DisplayName("平台成本")]
        public decimal erp_cost { get; set; }

        [UIHint("RadioBoxList")]
        [DisplayName("狀態")]
        public int status { get; set; }

        [UIHint("Info")]
        [DisplayName("簡介")]
        public string info { get; set; }

        [UIHint("html")]
        [DisplayName("商品介紹")]
        public string introduce { get; set; }

        [DisplayName("產地")]
        public string place { get; set; }

        [DisplayName("單位")]
        public string unit { get; set; }

        [DisplayName("規格")]
        public string specification { get; set; }

        [UIHint("Style_Tags")]
        [DisplayName("商品樣式")]
        public string style { get; set; }

        [UIHint("Select_Tags")]
        [DisplayName("同步平台")]
        public string platform_ids { get; set; }

        [DisplayName("國籍")]
        public string nation { get; set; }

        [DisplayName("排序")]
        public int inx { get; set; }

        [DisplayName("建立人")]
        public string create_user { get; set; }

        [DisplayName("建立時間")]
        public DateTime? create_datetime { get; set; }

        [DisplayName("修改人")]
        public string change_user { get; set; }

        [DisplayName("修改時間")]
        public DateTime? change_datetime { get; set; }

        [DisplayName("審核人")]
        public string check_user { get; set; }

        [DisplayName("審核時間")]
        public DateTime? check_datetime { get; set; }

        [DisplayName("同步用")]
        public int sync_flag { get; set; }

        //預購
        [DisplayName("是否為預購商品")]
        public Boolean preorder_visible { get; set; }

        [NotMapped]
        [UIHint("Imgx")]
        [DisplayName("正面;背面;其他;商品外包裝")]
        public List<string> Img { get; set; }

        //[NotMapped]
        //[UIHint("Imgx")]
        //[DisplayName("商品外包裝(盒/袋)平面圖")]
        //public List<string> other_Img { get; set; }

        [NotMapped]
        [UIHint("Info")]
        [DisplayName("備註")]
        public string status_info { get; set; }

        [NotMapped]
        public int Stock { get; set; }


        //檔期
        [DisplayName("檔期起始日期")]
        public DateTime? start_date { get; set; }

        [DisplayName("檔期結束日期")]
        public DateTime? end_date { get; set; }
    }

    public class product_group
    {
        [Key]
        [DisplayName("編號")]
        public int group_id { get; set; }

        [UIHint("SelectList")]
        [DisplayName("分區")]
        public int parent_id { get; set; }

        [DisplayName("中文名稱")]
        public string name { get; set; }

        [DisplayName("英文名稱")]
        public string name_en { get; set; }

        [DisplayName("圖片")]
        public string img { get; set; }

        [DisplayName("排序")]
        public int inx { get; set; }

        [DisplayName("啟用")]
        public Boolean visible { get; set; }

        [DisplayName("create_user")]
        public string create_user { get; set; }

        [DisplayName("create_datetime")]
        public DateTime? create_datetime { get; set; }

        [DisplayName("change_user")]
        public string change_user { get; set; }

        [DisplayName("change_datetime")]
        public DateTime? change_datetime { get; set; }

        [NotMapped]
        [DisplayName("數量")]
        public int count { get; set; }
    }

    public class product_image
    {
        [Key]
        [DisplayName("img_id")]
        public int img_id { get; set; }

        [DisplayName("group_id")]
        public int group_id { get; set; }

        [DisplayName("product_id")]
        public int product_id { get; set; }

        [DisplayName("img")]
        public string img { get; set; }

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

    public class product_status
    {
        [Key]
        [DisplayName("編號")]
        public int status_id { get; set; }

        [DisplayName("商品編號")]
        public int product_id { get; set; }

        [DisplayName("狀態")]
        public int status { get; set; }

        [DisplayName("狀態-舊")]
        public int status_old { get; set; }

        [DisplayName("備註")]
        public string info { get; set; }

        [DisplayName("create_user")]
        public string create_user { get; set; }

        [DisplayName("建立時間")]
        public DateTime? create_datetime { get; set; }
    }

    public class product_status_flag
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

        [NotMapped]
        public int count { get; set; }
    }

    public class product_stock
    {
        [Key]
        [DisplayName("stock_id")]
        public int stock_id { get; set; }

        [DisplayName("商品編號")]
        public int product_id { get; set; }

        [DisplayName("數量")]
        public int num { get; set; }

        [DisplayName("數量-舊")]
        public int num_old { get; set; }

        [UIHint("Info")]
        [DisplayName("備註")]
        public string info { get; set; }

        [DisplayName("啟用")]
        public Boolean visible { get; set; }

        [DisplayName("create_user")]
        public string create_user { get; set; }

        [DisplayName("建立時間")]
        public DateTime? create_datetime { get; set; }

        [DisplayName("change_user")]
        public string change_user { get; set; }

        [DisplayName("change_datetime")]
        public DateTime? change_datetime { get; set; }
    }

    public class product_erp_group
    {
        [Key]
        [DisplayName("group_id")]
        public int group_id { get; set; }

        [UIHint("SelectList")]
        [DisplayName("國籍")]
        public string nation { get; set; }

        [DisplayName("名稱")]
        public string name { get; set; }

        [DisplayName("代號")]
        public string pre { get; set; }

        [DisplayName("排序")]
        public int inx { get; set; }

        [DisplayName("啟用")]
        public Boolean visible { get; set; }

        [DisplayName("create_user")]
        public string create_user { get; set; }

        [DisplayName("create_datetime")]
        public DateTime? create_datetime { get; set; }

        [DisplayName("change_user")]
        public string change_user { get; set; }

        [DisplayName("change_datetime")]
        public DateTime? change_datetime { get; set; }

        [NotMapped]
        [DisplayName("編號")]
        public string Sub_sn { get; set; }

        [NotMapped]
        [DisplayName("是否啟用")]
        public bool Sub_visible { get; set; }

    }

    public class product_erp_sub
    {
        [Key]
        [DisplayName("sub_id")]
        public int sub_id { get; set; }

        [DisplayName("供應商編號")]
        public int admin_id { get; set; }

        [DisplayName("分類編號")]
        public int group_id { get; set; }

        [DisplayName("編號")]
        public string auto_sn { get; set; }

        [DisplayName("啟用")]
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

}