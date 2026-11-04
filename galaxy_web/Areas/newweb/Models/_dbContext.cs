using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace galaxy_web.Models
{
    public class _dbContext : DbContext
    {
        public _dbContext() : base("netwebConnectionString") { }

        #region"News"

        public DbSet<news_data> news_data { get; set; }

        public DbSet<news_group> news_group { get; set; }

        #endregion

        #region"Order"

        public DbSet<order_data> order_data { get; set; }

        public DbSet<order_sub> order_sub { get; set; }

        public DbSet<order_status> order_status { get; set; }

        public DbSet<order_status_flag> order_status_flag { get; set; }

        public DbSet<order_goods> order_goods { get; set; }

        public DbSet<order_invoice> order_invoice { get; set; }

        public DbSet<order_pay> order_pay { get; set; }

        public DbSet<order_bills> order_bills { get; set; }

        public DbSet<order_note> order_note { get; set; }

        #endregion

        #region"Proudct"

        public DbSet<product_data> product_data { get; set; }

        public DbSet<product_group> product_group { get; set; }

        public DbSet<product_image> product_image { get; set; }

        public DbSet<product_status> product_status { get; set; }

        public DbSet<product_status_flag> product_status_flag { get; set; }

        public DbSet<product_stock> product_stock { get; set; }

        public DbSet<product_erp_group> product_erp_group { get; set; }

        public DbSet<product_erp_sub> product_erp_sub { get; set; }

        #endregion

        #region"Port"

        public DbSet<port_data> port_data { get; set; }

        #endregion

        #region"Sys"

        public DbSet<sys_admin_data> sys_admin_data { get; set; }

        public DbSet<sys_admin_image> sys_admin_image { get; set; }

        public DbSet<sys_admin_group> sys_admin_group { get; set; }

        public DbSet<sys_function_data> sys_function_data { get; set; }

        public DbSet<sys_function_sub> sys_function_sub { get; set; }

        public DbSet<sys_power_data> sys_power_data { get; set; }

        public DbSet<sys_power_sub> sys_power_sub { get; set; }

        public DbSet<sys_power_list> sys_power_list { get; set; }

        public DbSet<sys_record_data> sys_record_data { get; set; }

        public DbSet<sys_admin_login> sys_admin_login { get; set; }

        public DbSet<zip_data> zip_data { get; set; }

        public DbSet<sys_admin_bills> sys_admin_bills { get; set; }

        #endregion
    }
}