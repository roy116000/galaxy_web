using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace galaxy_web.Models2
{
    public class _dbContext2 : DbContext
    {
        public _dbContext2() : base("netwebConnectionString2") { }

        public DbSet<sys_admin_data> sys_admin_data { get; set; }

        public DbSet<sys_bank_data> sys_bank_data { get; set; }

        public DbSet<sys_bank_group> sys_bank_group { get; set; }
    }
}