using galaxy_web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace galaxy_web.Areas.newweb.Controllers
{
    public class ApiOrderController : _BaseApiController
    {
        private _dbContext _db_;

        protected _dbContext _DB
        {
            get
            {
                if (_db_ == null)
                {
                    _db_ = new _dbContext();
                }
                return _db_;
            }
        }

        public class _getOrder
        {
            public int status { get; set; }
            public string msg { get; set; }

            public _data data { get; set; }
            public class _data
            {
                public string sn { get; set; }
                public int status { get; set; } //1,2,3,4,5
            }

        }

        public class _getO
        {
            public int id { get; set; }
            public string sn { get; set; }
        }

        [HttpPost]
        public HttpResponseMessage Get([FromBody] _getO Get)
        {
            var result = new _getOrder { data = new _getOrder._data { } };
            string msg = "";

            while (true)
            {
                if (Get.id < 1) { msg = "請填寫平台編號"; break; }
                if (string.IsNullOrEmpty(Get.sn)) { msg = "請填寫訂單單號"; break; }

                var linq = this._DB.order_data.Where(m => m.order_id > 0);

                linq = linq.Where(m => m.platform_sn == Get.sn);

                linq = linq.Where(m => m.platform_id == Get.id);

                var data = linq.FirstOrDefault();
                if (data == null) { msg = "查無此訂單"; break; }

                result.data.sn = data.platform_sn;
                result.data.status = data.status;

                break;
            }

            if (string.IsNullOrEmpty(msg))
            {
                result.status = 1;
            }
            else
            {
                result.msg = msg;
            }

            return Request.CreateResponse(HttpStatusCode.OK, result);

        }

    }
}
