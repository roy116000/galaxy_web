using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace galaxy_web.Models
{

    public class news_data
    {
        [Key]
        [DisplayName("news_id")]
        public int news_id { get; set; }

        [UIHint("SelectList")]
        [DisplayName("分類")]
        public int group_id { get; set; }

        [DisplayName("標題")]
        public string title { get; set; }

        [UIHint("Info")]
        [DisplayName("簡介")]
        public string info { get; set; }

        [UIHint("Html")]
        [DisplayName("內容")]
        public string cont { get; set; }

        [DisplayName("起始時間")]
        public DateTime? start_date { get; set; }

        [DisplayName("結束時間")]
        public DateTime? end_date { get; set; }

        [DisplayName("點擊數")]
        public int hits { get; set; }

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
    }

    public class news_group
    {
        [Key]
        [DisplayName("group_id")]
        public int group_id { get; set; }

        [DisplayName("parent_id")]
        public int parent_id { get; set; }

        [DisplayName("name")]
        public string name { get; set; }

        [DisplayName("name_en")]
        public string name_en { get; set; }

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