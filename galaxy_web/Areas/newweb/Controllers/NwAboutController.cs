using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using galaxy_web.Models;
using MvcPaging;
using System.IO;

namespace galaxy_web.Areas.newweb.Controllers
{
    public class NwAboutController : _BaseController
    {

        public ActionResult Page()
        {
            ViewBag.Alert = TempData["Alert"] ?? false;

            //權限
            var aData = NW.admin(NW.DESDeCode(User.Identity.Name));

            #region"商品"

            var product_data = this._DB.product_data.Where(m => m.product_id > 0);
            //var product_image = this._DB.product_image.Where(m => m.visible).Select(m => new { m.img_id, m.product_id, m.img, m.group_id }).OrderBy(m => m.group_id);


            if (aData.power_id > 2)
            {
                product_data = product_data.Where(m => m.company_id == aData.admin_id);
            }

            var linq_p = from a in this._DB.product_status_flag.Where(m => m.visible).OrderBy(m => m.inx).ToList()
                         join b in product_data on a.flag_id equals b.status into mm
                         select new KeyValuePair<product_status_flag, List<product_data>>(a, mm.ToList()) { };

            var data_p = linq_p.ToList();

            ViewData["Product"] = data_p;

            #endregion

            #region"訂單"

            var order_data = from a in this._DB.order_data.Where(m => m.order_id > 0).ToList()
                             join b in this._DB.order_sub on a.order_id equals b.order_id
                             select new order_data
                             {
                                 company_id = a.company_id,
                                 order_id = a.order_id,
                                 order_sn = a.order_sn,
                                 send_name = a.send_name,
                                 status = a.status,
                                 create_user = b.item_name,
                                 order_name = b.item_no
                             };

            if (aData.power_id > 2)
            {
                order_data = order_data.Where(m => m.company_id == aData.admin_id);
            }

            var linq_o = from a in this._DB.order_status_flag.Where(m => m.visible).OrderBy(m => m.inx).ToList()
                         join b in order_data on a.flag_id equals b.status into mm
                         select new KeyValuePair<order_status_flag, List<order_data>>(a, mm.ToList());

            var data_o = linq_o.ToList();

            ViewData["Order"] = data_o;

            #endregion

            #region"公告"

            var linq_n = this._DB.news_data.Where(m => m.visible && m.group_id == 1);
            linq_n = linq_n.Where(m => m.start_date <= NW.Today && m.end_date > NW.Tomorrow);

            var data_n = linq_n.OrderByDescending(m => m.news_id).Take(5).ToList();

            ViewData["News"] = data_n;

            #endregion

            return View();
        }

        public ActionResult Edit()
        {
            string Aid = NW.DESDeCode(User.Identity.Name);
            sys_admin_data aData = new sys_admin_data() { };
            try { aData = NW.admin(Aid); }
            catch { }

            return RedirectToAction("Edit", "NwCompany", new { id = aData.admin_id });
        }

        public ActionResult PwdEdit()
        {
            ViewBag.jgrowl = TempData[NW.ssjgrowl];

            string Aid = NW.DESDeCode(User.Identity.Name);
            sys_admin_data aData = NW.admin(Aid);

            return View(aData);
        }

        public ActionResult PwdEditSave(sys_admin_data Get, string pwdx, string pwdy)
        {
            string msg = "";

            if (string.IsNullOrEmpty(pwdx) || string.IsNullOrEmpty(pwdy)) { msg = "請填寫密碼或確認密碼"; goto Error; }
            if (pwdx != pwdy) { msg = "密碼不相同，請重新填寫"; goto Error; }

            var data = this._DB.sys_admin_data.Where(m => m.admin_id == Get.admin_id).SingleOrDefault();
            if (data != null)
            {
                data.pwd = pwdx;
                this._DB.SaveChanges();
            }

            TempData[NW.ssjgrowl] = "密碼修改-成功";
            return RedirectToAction("Edit", "NwCompany", new { id = data.admin_id });

        Error:
            TempData[NW.ssjgrowl] = msg;
            return RedirectToAction("PwdEdit");

        }
    }
}
