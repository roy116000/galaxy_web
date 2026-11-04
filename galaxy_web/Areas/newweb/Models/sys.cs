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
    public class sys_function_data
    {
        [Key]
        [DisplayName("function_id")]
        public int function_id { get; set; }

        [DisplayName("菜單名稱")]
        [Required(ErrorMessage = "文字，必需填寫")]
        public string label { get; set; }

        [DisplayName("描述")]
        public string info { get; set; }

        [DisplayName("controll")]
        public string controll { get; set; }

        //[DisplayName("init_action")]
        //public string init_action { get; set; }

        [DisplayName("icon")]
        public string icon { get; set; }

        //[DisplayName("icon2")]
        //public string icon2 { get; set; }

        [DisplayName("排序")]
        public int inx { get; set; }

        [DisplayName("啟用")]
        public Boolean visible { get; set; }

        public virtual ICollection<sys_function_sub> sys_function_sub { get; set; }
    }

    public class sys_function_sub
    {
        [Key]
        [DisplayName("function_sub_id")]
        public int function_sub_id { get; set; }

        [UIHint("SelectList")]
        [DisplayName("菜單名稱")]
        public int function_id { get; set; }

        [DisplayName("子菜單名稱")]
        public string label { get; set; }

        [DisplayName("描述")]
        public string info { get; set; }

        [DisplayName("action")]
        public string action { get; set; }

        [DisplayName("排序")]
        public int inx { get; set; }

        [DisplayName("顯示於功能表")]
        public Boolean show_visible { get; set; }

        [DisplayName("啟用")]
        public Boolean visible { get; set; }

        public virtual sys_function_data sys_function_data { get; set; }
    }

    public class sys_record_data
    {
        [Key]
        [DisplayName("record_id")]
        public int record_id { get; set; }

        [DisplayName("sql_name")]
        public string sql_name { get; set; }

        [DisplayName("id")]
        public int id { get; set; }

        [DisplayName("flag")]
        public int flag { get; set; }

        [DisplayName("record")]
        public string record { get; set; }

        [DisplayName("create_user")]
        public string create_user { get; set; }

        [DisplayName("create_datetime")]
        public DateTime? create_datetime { get; set; }
    }

    #region "權限"

    [DisplayName("權限設定")]
    public class sys_power_data
    {
        [Key]
        [DisplayName("power_id")]
        public int power_id { get; set; }

        [DisplayName("權限名稱")]
        public string label { get; set; }

        [UIHint("Info")]
        [DisplayName("描述")]
        public string info { get; set; }

        [DisplayName("啟用")]
        public Boolean visible { get; set; }

        [DisplayName("Controller")]
        public string controller { get; set; }

        [DisplayName("Action")]
        public string action { get; set; }

        [NotMapped]
        [UIHint("SelectList_chained")]
        [DisplayName("啟動頁")]
        public int start_id { get; set; }

    }

    public class sys_power_sub
    {

        [Key]
        [DisplayName("power_sub_id")]
        public int power_sub_id { get; set; }

        [DisplayName("power_id")]
        public int power_id { get; set; }

        [DisplayName("function_sub_id")]
        public int function_sub_id { get; set; }
    }

    [DisplayName("權限檢視")]
    public class sys_power_list
    {
        [Key]
        [DisplayName("power_sub_id")]
        public int power_sub_id { get; set; }

        [DisplayName("power_id")]
        public int power_id { get; set; }

        [DisplayName("function_id")]
        public int function_id { get; set; }

        [DisplayName("label")]
        public string label { get; set; }

        [DisplayName("info")]
        public string info { get; set; }

        [DisplayName("controll")]
        public string controll { get; set; }

        [DisplayName("show_visible")]
        public Boolean show_visible { get; set; }

        [DisplayName("sub_label")]
        public string sub_label { get; set; }

        [DisplayName("sub_info")]
        public string sub_info { get; set; }

        [DisplayName("action")]
        public string action { get; set; }

        [DisplayName("icon")]
        public string icon { get; set; }

        //[DisplayName("init_action")]
        //public string init_action { get; set; }

        [DisplayName("inx")]
        public int inx { get; set; }

        [DisplayName("sub_inx")]
        public int sub_inx { get; set; }

    }

    #endregion

    public class zip_data 
    {
        [Key]
        public int zip { get; set; }

        [DisplayName("縣市")]
        public string city { get; set; }

        [DisplayName("鄉鎮區")]
        public string canton { get; set; }

        [DisplayName("city_id")]
        public int city_id { get; set; }

        [DisplayName("visible")]
        public Boolean visible { get; set; }
    }

}