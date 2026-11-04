using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace galaxy_web.Models
{

    [DisplayName("管理員")]
    public class sys_admin_data
    {
        [Key]
        [DisplayName("admin_id")]
        public int admin_id { get; set; }

        [UIHint("SelectList")]
        [DisplayName("權限名稱")]
        public int power_id { get; set; }

        [UIHint("SelectList")]
        [DisplayName("等級")]
        public int group_id { get; set; }

        [DisplayName("統一編號 / 身分證 / 會員編號")]
        public string sn { get; set; }

        [DisplayName("ERP-編號")]
        public string erp_sn { get; set; }

        [DisplayName("帳號")]
        public string uid { get; set; }

        [DisplayName("密碼")]
        public string pwd { get; set; }

        [DisplayName("支付密碼")]
        public string pwd2 { get; set; }

        [DisplayName("key")]
        public string pkey { get; set; }

        [DisplayName("啟用")]
        public bool visible { get; set; }

        [DisplayName("起始日期")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? start_date { get; set; }

        [DisplayName("結束日期")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? end_date { get; set; }

        [DisplayName("推薦人")]
        public string introduce_uid { get; set; }

        [DisplayName("廠商騰雲編號")]
        public string company_uid { get; set; }

        [DisplayName("廠商名稱")]
        public string name { get; set; }

        [DisplayName("廠商全名")]
        public string name_full { get; set; }

        [UIHint("Info")]
        [DisplayName("備註")]
        public string info { get; set; }

        [UIHint("SelectList_chained")]
        [DisplayName("郵遞區號")]
        public int zip { get; set; }

        [DisplayName("地址")]
        public string address { get; set; }

        [UIHint("SelectList_chained")]
        [DisplayName("郵遞區號2")]
        public int zip2 { get; set; }

        [DisplayName("地址2")]
        public string address2 { get; set; }

        [DisplayName("電話")]
        public string tel { get; set; }

        [DisplayName("電話2")]
        public string tel2 { get; set; }

        [DisplayName("傳真")]
        public string fax { get; set; }

        [DisplayName("信箱")]
        public string mail { get; set; }

        [DisplayName("負責人")]
        public string boss { get; set; }

        [DisplayName("負責人-電話")]
        public string boss_tel { get; set; }

        [DisplayName("聯絡人")]
        public string contact { get; set; }

        [DisplayName("聯絡人-電話")]
        public string contact_tel { get; set; }

        [DisplayName("聯絡人2")]
        public string contact2 { get; set; }

        [DisplayName("聯絡人2-電話")]
        public string contact2_tel { get; set; }

        [DisplayName("聯絡人3")]
        public string contact3 { get; set; }

        [DisplayName("聯絡人3-電話")]
        public string contact3_tel { get; set; }

        [DisplayName("IP")]
        public string last_ip { get; set; }

        [DisplayName("最後-登入時間")]
        public DateTime? last_datetime { get; set; }

        [DisplayName("create_user")]
        public string create_user { get; set; }

        [DisplayName("create_datetime")]
        public DateTime? create_datetime { get; set; }

        [DisplayName("change_user")]
        public string change_user { get; set; }

        [DisplayName("change_datetime")]
        public DateTime? change_datetime { get; set; }

        [DisplayName("是否開放醫美專區功能")]
        public Boolean note_visible { get; set; }

        [NotMapped]
        public string pwdx { get; set; }
        [NotMapped]
        public string pwdy { get; set; }

        [NotMapped]
        [UIHint("Imgx")]
        [DisplayName("營利事業登記證;食品檢驗;販賣許可證;電子商務(網路販賣證明);營利事業登記證2;營利事業登記證3")]
        public List<string> Img { get; set; }

        [NotMapped]
        [UIHint("Files")]
        [DisplayName("PDF")]
        public string PDF { get; set; }

        //請款銀行用
        [NotMapped]
        [UIHint("SelectList_chained")]
        public string Bank { get; set; }

    }

    [DisplayName("等級")]
    public class sys_admin_group
    {
        [Key]
        [DisplayName("group_id")]
        public int group_id { get; set; }

        [DisplayName("parent_id")]
        public int parent_id { get; set; }

        [DisplayName("name")]
        public string name { get; set; }

        [DisplayName("limit")]
        public int limit { get; set; }

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

    [DisplayName("帳戶銀行")]
    public class sys_admin_bank
    {
        [Key]
        [DisplayName("bank_id")]
        public int bank_id { get; set; }

        [DisplayName("admin_id")]
        public int admin_id { get; set; }

        [DisplayName("bank")]
        public string bank { get; set; }

        [DisplayName("bank2")]
        public string bank2 { get; set; }

        [DisplayName("account")]
        public string account { get; set; }

        [DisplayName("no")]
        public string no { get; set; }

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

    [DisplayName("會員請款")]
    public class sys_admin_bills
    {
        [Key]
        [DisplayName("編號")]
        public int bills_bank_id { get; set; }

        [DisplayName("廠商編號")]
        public int admin_id { get; set; }

        //0月結  1半月結
        [DisplayName("結算類型")]
        public int bills_flag { get; set; }

        [DisplayName("收款戶名")]
        public string receiving_name { get; set; }

        [DisplayName("收款帳號")]
        public string receiving_account { get; set; }

        [DisplayName("收款行代號")]
        public string receiving_code { get; set; }

        [DisplayName("銀行別")]
        public string bank { get; set; }

        [DisplayName("分行別")]
        public string bank2 { get; set; }

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

    [DisplayName("登入紀錄")]
    public class sys_admin_login
    {
        [Key]
        [DisplayName("login_id")]
        public int login_id { get; set; }

        [DisplayName("uid")]
        public string uid { get; set; }

        [DisplayName("pwd")]
        public string pwd { get; set; }

        [DisplayName("code")]
        public string code { get; set; }

        [DisplayName("pkey")]
        public string pkey { get; set; }

        [DisplayName("push_no")]
        public string push_no { get; set; }

        [DisplayName("ip")]
        public string ip { get; set; }

        [DisplayName("device")]
        public string device { get; set; }

        [DisplayName("login_datetime")]
        public DateTime? login_datetime { get; set; }

        [DisplayName("isSuccess")]
        public Boolean isSuccess { get; set; }

        [DisplayName("create_user")]
        public string create_user { get; set; }

        [DisplayName("create_datetime")]
        public DateTime? create_datetime { get; set; }
    }

    [DisplayName("會員圖片")]
    public class sys_admin_image
    {
        [Key]
        [DisplayName("編號")]
        public int img_id { get; set; }

        [DisplayName("分類編號")]
        public int group_id { get; set; }

        [DisplayName("會員編號")]
        public int admin_id { get; set; }

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

}