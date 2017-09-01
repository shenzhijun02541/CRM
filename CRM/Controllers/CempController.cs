using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM.Controllers
{
    public class CempController : Controller
    {
        //GET : Cemp
        public ActionResult Index()
        {
            IList<Model.Cemp> lmd = new List<Model.Cemp>();
            IList<KeyValuePair<string, object>> condition = new List<KeyValuePair<string, object>>();
            LIB.PageModel lpm = new LIB.PageModel();
            lmd = LIB.MoreTermSelect.MoreTerm<Model.Cemp>(condition, "Cemp", ref lpm);
            ViewData["lmd"] = lmd;
            return View();
        }

        //添加人员
        [HttpPost]
        public ActionResult Add(Model.Cemp md)
        {
            bool issuccess = BLL.CommonBLL.add<Model.Cemp>(md);
            var ret = new
            {
                messagecode = issuccess ? 1 : 0,
            };
            return Json(ret, JsonRequestBehavior.AllowGet);

        }

        //删除人员

        [HttpPost]
        public ActionResult Delete(Model.Cemp md)
        {
            bool issuccess = BLL.CommonBLL.Delete<Model.Cemp>(md);
            var ret = new
            {
                messagecode = issuccess ? 1 : 0,
            };
            return Json(ret, JsonRequestBehavior.AllowGet);

        }

        //编辑人员
        [HttpPost]
        public ActionResult Update(Model.Cemp md)
        {
            bool issuccess = BLL.CommonBLL.Update<Model.Cemp>(md);
            var ret = new
            {
                messagecode = issuccess ? 1 : 0,
            };
            return Json(ret, JsonRequestBehavior.AllowGet);

        }
    }
}