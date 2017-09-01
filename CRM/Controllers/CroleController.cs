using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM.Controllers
{
    public class CroleController : Controller
    {
        //GET : Crole
        public ActionResult Index()
        {
            IList<Model.Crole> lmd = new List<Model.Crole>();
            IList<KeyValuePair<string, object>> condition = new List<KeyValuePair<string, object>>();
            LIB.PageModel lpm = new LIB.PageModel();
            lmd = LIB.MoreTermSelect.MoreTerm<Model.Crole>(condition, "Crole", ref lpm);
            ViewData["lmd"] = lmd;
            return View();
        }

        //添加人员
        [HttpPost]
        public ActionResult Add(Model.Crole md)
        {
            bool issuccess = BLL.CommonBLL.add<Model.Crole>(md);
            var ret = new
            {
                messagecode = issuccess ? 1 : 0,
            };
            return Json(ret, JsonRequestBehavior.AllowGet);

        }

        //删除人员

        [HttpPost]
        public ActionResult Delete(Model.Crole md)
        {
            bool issuccess = BLL.CommonBLL.Delete<Model.Crole>(md);
            var ret = new
            {
                messagecode = issuccess ? 1 : 0,
            };
            return Json(ret, JsonRequestBehavior.AllowGet);

        }

        //编辑人员
        [HttpPost]
        public ActionResult Update(Model.Crole md)
        {
            bool issuccess = BLL.CommonBLL.Update<Model.Crole>(md);
            var ret = new
            {
                messagecode = issuccess ? 1 : 0,
            };
            return Json(ret, JsonRequestBehavior.AllowGet);

        }
    }
}