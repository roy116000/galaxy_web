using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using galaxy_web.Models;

namespace galaxy_web.Areas.newweb.Controllers
{
    public class _BaseApiController : ApiController
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


    }
}
