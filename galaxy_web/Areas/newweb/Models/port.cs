using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace galaxy_web.Models
{

    public class port_data
    {
        [Key]
        [DisplayName("port_id")]
        public int port_id { get; set; }

        [DisplayName("操作資訊")]
        public string name { get; set; }

        [DisplayName("操作類型")]
        public string type { get; set; }

        [DisplayName("總筆數")]
        public int num { get; set; }

        [DisplayName("成功筆數")]
        public int success_num { get; set; }

        [DisplayName("失敗筆數")]
        public int error_num { get; set; }

        [DisplayName("失敗原因")]
        public string error_msg { get; set; }

        [DisplayName("visible")]
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

}