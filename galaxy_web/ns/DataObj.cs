using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Web.Configuration;
using System.Data.SqlClient;
//using System.Web.HttpContext;
using System.Configuration;
using System.Web;

namespace galaxy_web.ns
{
    public class DataObj
    {

        public enum ChangeStyle : int
        {
            ALL = 0,
            INSERT = 1,
            UPDATE = 2,
            DELETE = 4
        }

        private static SqlConnection _cn = new SqlConnection();


        public void New()
        {
            cnOpen();
        }


        public void New(string ConnectionString)
        {
            cnOpen(ConnectionString);
        }


        public SqlConnection CN
        {
            get { return _cn; }
        }


        private void cnOpen(string ConnectionString = "")
        {
            if (_cn.State == ConnectionState.Open)
                _cn.Close();
            if (string.IsNullOrEmpty(ConnectionString))
            {
                ConnectionString = WebConfigurationManager.ConnectionStrings["netwebConnectionString"].ToString();
               
            }
            else
            {
               ConnectionString = WebConfigurationManager.ConnectionStrings[ConnectionString].ToString();
            }

            _cn.ConnectionString = ConnectionString;
            _cn.Open();
        }

        public DataTable GetTable(string sql)
        {
            //Dim cmd = New SqlCommand(sql, cn)
            SqlDataAdapter da = new SqlDataAdapter(sql, _cn);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            dt.BeginLoadData();
            da.Fill(ds);
            dt.EndLoadData();
            dt = ds.Tables[0];
            da.Dispose();
            return dt;
        }

        // 連線字串
        // <add name="netwebConnectionString" connectionString="Data Source=192.168.1.16;Initial Catalog=geo;Persist Security Info=True;User ID=sa;Password=" providerName="System.Data.SqlClient" />
            //if (TableName != "") {
            //    geo.ns.DataObj db = new geo.ns.DataObj();
            //    db.New();

            //    string data_class = db.GetModel(TableName);

            //    ViewBag.data_class = data_class;

            //    string dbContext = db.getdbContext(TableName);

            //    ViewBag.dbContext = dbContext;

            //    ViewBag.TableName = TableName;
            //}



        public string getdbContext(string TableName) {
            string sR = "";

            sR+="using System.Data.Entity; \n \n";
            sR += "\\\\ System.Data.Entity.Database.SetInitializer<Models.dbContext>(null); \n \n";

            sR+="    public class dbContext : DbContext \n";
            sR+="    { \n";
            sR+="        public dbContext() : base(\"name=netwebConnectionString\") { } \n";
            sR+=" \n";
            sR += "        public DbSet<" + TableName + "> "+TableName+" { get; set; } \n";
            sR+="    } \n";

            return sR;
        }

        public string GetModel(string TableName) {
            string sR = "";

            string sql = "Select * from " + TableName;
            DataTable dt = GetTable(sql);

            int i = 0;

            sR += "using System.ComponentModel; \n";
            sR += "using System.ComponentModel.DataAnnotations; \n";
            sR += "using System.ComponentModel.DataAnnotations.Schema; \n \n \n";

            sR += "[Required(ErrorMessage=\"錯誤訊息\")] \n \n \n";

            foreach (DataColumn dc in dt.Columns)
            {
                if (i == 0) { sR += "[Key] \n"; }
                string dType = dc.DataType.Name.ToLower().Replace("int32", "int").Replace("datetime", "DateTime?").Replace("boolean", "Boolean");
                sR += "[DisplayName(\"" + dc.ColumnName + "\")] \n";
                sR += "public " + dType + " " + dc.ColumnName + " {get; set;} \n \n";
                i += 1;
            }
            return sR;
        }


        public string GetEditString(string TableName)
        {
            string sR = "";

            string sql = "Select * from " + TableName;
            DataTable dt = GetTable(sql);


            foreach (DataColumn dc in dt.Columns)
            {
                sR += "data." + dc.ColumnName + " = " + TableName + "." + dc.ColumnName + ";\n";
            }
            return sR;
        }

        public string GetViewString(string TableName)
        {
            string sR = "";

            string sql = "Select * from " + TableName;
            DataTable dt = GetTable(sql);


            foreach (DataColumn dc in dt.Columns)
            {
                //sR += "data." + dc.ColumnName + " = " + TableName + "." + dc.ColumnName + ";\n";
                sR += "@Html.EditorFor(model => model." + dc.ColumnName + ") \n\n";
            }
            return sR;
        }


        public int exeSQL(string sql)
        {
            if (_cn.State != ConnectionState.Open)
                cnOpen(_cn.ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, _cn);
            int iR = cmd.ExecuteNonQuery();
            cmd.Dispose();
            return iR;
        }

        public SqlDataReader GetDataRead(string sql)
        {
            SqlCommand cmd = new SqlCommand(sql, _cn);
            SqlDataReader dread = cmd.ExecuteReader();
            //cmd.Dispose();
           return dread;
        }
    }
}