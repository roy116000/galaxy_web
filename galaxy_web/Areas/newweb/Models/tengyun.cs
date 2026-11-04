using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace galaxy_web.Models2
{
    //此為簡式表
    [DisplayName("管理員")]
    public class sys_admin_data
    {
        [Key]
        [DisplayName("admin_id")]
        public int admin_id { get; set; }

        [UIHint("SelectList")]
        [DisplayName("權限名稱")]
        public int? power_id { get; set; }

        [DisplayName("會員帳號")]
        public string uid { get; set; }

        [DisplayName("啟用")]
        public Boolean visible { get; set; }

        [DisplayName("註冊日期")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? start_date { get; set; }

        [DisplayName("結束日期")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? end_date { get; set; }

        [UIHint("SelectList")]
        [DisplayName("申請狀態")]
        public int status { get; set; }

        [DisplayName("付款水單")]
        public string account_img { get; set; }
        [DisplayName("推薦人編號")]
        public string introduce_uid { get; set; }
        [DisplayName("安置編號")]
        public string parent_uid { get; set; }

        [UIHint("SelectList")]
        [DisplayName("位置")]
        public int position { get; set; }

        [DisplayName("組織")]
        public string org { get; set; }
        [DisplayName("直推")]
        public string org_introduce { get; set; }
        [DisplayName("國別")]
        public string nation { get; set; }

        [UIHint("SelectList")]
        [DisplayName("購買配套")]
        public int group_id { get; set; }
        [UIHint("SelectList")]
        [DisplayName("個人/公司")]
        public int id_flag { get; set; }
        [DisplayName("姓名/公司名稱")]
        public string name { get; set; }
        [DisplayName("負責人")]
        public string boss { get; set; }

        [UIHint("SelectList")]
        [DisplayName("性別")]
        public string sex { get; set; }
        [DisplayName("身份證字號/公司統編")]
        public string idno { get; set; }
        [DisplayName("生日/公司創立時間")]
        public string birthday { get; set; }
        [DisplayName("戶籍地址")]
        public string address { get; set; }
        [DisplayName("通訊地址")]
        public string address2 { get; set; }

        [DisplayName("住家電話(如無市話請填入手機號碼)")]
        public string tel { get; set; }
        [DisplayName("公司電話")]
        public string tel2 { get; set; }
        [DisplayName("行動電話")]
        public string phone { get; set; }
        [DisplayName("傳真")]
        public string fax { get; set; }
        [DisplayName("電子郵件")]
        public string mail { get; set; }


    



        [DisplayName("階級")]
        public int level_id { get; set; }



        public virtual agent_group agent_group { get; set; }

        public virtual agent_level agent_level { get; set; }



    }

    public class agent_group
    {
        [Key]
        [DisplayName("group_id")]
        public int group_id { get; set; }

        [DisplayName("name")]
        public string name { get; set; }

        [DisplayName("parent_id")]
        public int parent_id { get; set; }

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

        [DisplayName("price")]
        public int price { get; set; }

        [DisplayName("static")]
        public int staticc { get; set; }

        [DisplayName("dynamic")]
        public int dynamicc { get; set; }

        [DisplayName("introduce")]
        public int introduce { get; set; }

        [DisplayName("parent")]
        public int parent { get; set; }

        [DisplayName("era")]
        public int era { get; set; }

        [DisplayName("touch")]
        public int touch { get; set; }

        [DisplayName("team")]
        public int team { get; set; }

        [DisplayName("day_limit")]
        public int day_limit { get; set; }

        [DisplayName("pv")]
        public int pv { get; set; }

        [DisplayName("touch2")]
        public int touch2 { get; set; }

        [DisplayName("team2")]
        public int team2 { get; set; }


        [DisplayName("img")]
        public string img { get; set; }


    }

    public class agent_level
    {

        [Key]
        [DisplayName("level_id")]
        public int level_id { get; set; }

        [DisplayName("聘階")]
        public string name { get; set; }

        [DisplayName("lead")]
        public decimal lead { get; set; }

        [DisplayName("toppv")]
        public int toppv { get; set; }

        [DisplayName("linemaxpv")]
        public int linemaxpv { get; set; }

        [DisplayName("other")]
        public int other { get; set; }

        [DisplayName("create_datetime")]
        public DateTime? create_datetime { get; set; }

        [DisplayName("create_user")]
        public string create_user { get; set; }

        [DisplayName("change_datetime")]
        public DateTime? change_datetime { get; set; }

        [DisplayName("change_user")]
        public string change_user { get; set; }



    }

    public class sys_bank_data
    {
        [Key]
        [DisplayName("bank_id")]
        public int bank_id { get; set; }

        [DisplayName("group_id")]
        public int group_id { get; set; }

        [DisplayName("parent_id")]
        public int parent_id { get; set; }

        [DisplayName("name")]
        public string name { get; set; }

        [DisplayName("code")]
        public string code { get; set; }

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

    public class sys_bank_group
    {
        [Key]
        [DisplayName("group_id")]
        public int group_id { get; set; }

        [DisplayName("name")]
        public string name { get; set; }

        [DisplayName("國家")]
        public string nation { get; set; }

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