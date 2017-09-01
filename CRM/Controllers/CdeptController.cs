using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM.Controllers
{
    public class CdeptController : Controller
    {
        //GET : Cdept
        public ActionResult Index()
        {
            IList<Model.Cdept> lmd = new List<Model.Cdept>();
            IList<KeyValuePair<string, object>> condition = new List<KeyValuePair<string, object>>();
            LIB.PageModel lpm = new LIB.PageModel();
            lmd = LIB.MoreTermSelect.MoreTerm<Model.Cdept>(condition, "Cdept", ref lpm);
            ViewData["lmd"] = lmd;
            return View();
        }

        //添加人员
        [HttpPost]
        public ActionResult Add(Model.Cdept md)
        {
            bool issuccess = BLL.CommonBLL.add<Model.Cdept>(md);
            var ret = new
            {
                messagecode = issuccess ? 1 : 0,
            };
            return Json(ret, JsonRequestBehavior.AllowGet);

        }

        //删除人员

        [HttpPost]
        public ActionResult Delete(Model.Cdept md)
        {
            bool issuccess = BLL.CommonBLL.Delete<Model.Cdept>(md);
            var ret = new
            {
                messagecode = issuccess ? 1 : 0,
            };
            return Json(ret, JsonRequestBehavior.AllowGet);

        }

        //编辑人员
        [HttpPost]
        public ActionResult Update(Model.Cdept md)
        {
            bool issuccess = BLL.CommonBLL.Update<Model.Cdept>(md);
            var ret = new
            {
                messagecode = issuccess ? 1 : 0,
            };
            return Json(ret, JsonRequestBehavior.AllowGet);

        }
    }
}