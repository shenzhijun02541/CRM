using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace CRM.Controllers
{
    public class ActivityController : Controller
    {
        // GET: Activity
        public ActionResult Index(int? PageCount=1,int? PageSize=5,string acName="",int? acResult=-1,int? acRole=-1)
        {
            IList<Model.Activity> lmam = new List<Model.Activity>();
            IList<KeyValuePair<string, object>> condition = new List<KeyValuePair<string, object>>();

            //查询条件
            if (!string.IsNullOrWhiteSpace(acName))
            {
                condition.Add(new KeyValuePair<string, object>("acName",acName));
            }
            if (acResult != null && acResult != -1)
            {
                condition.Add(new KeyValuePair<string, object>("acResult", acResult));
            }
            if (acRole != null && acRole != -1)
            {
                condition.Add(new KeyValuePair<string, object>("acRole", acRole));
            }

            LIB.PageModel lpm = new LIB.PageModel();
            lpm.fldName = "acId";
            lpm.PageCount = PageCount.Value;
            lpm.PageSize = PageSize.Value;
            lmam = LIB.MoreTermSelect.MoreTerm<Model.Activity>(condition,"Activity.acRole,Crole.crno",ref lpm);
            condition.Clear();

            PagedList<Model.Activity> pagems = new PagedList<Model.Activity>(lmam, PageCount.Value, PageSize.Value, lpm.TotalCount);
            ViewData["pagems"] = pagems;

            //获取角色
            IList<Model.Crole> lmr = new List<Model.Crole>();
            LIB.PageModel lpmRole = new LIB.PageModel();
            lmr = LIB.MoreTermSelect.MoreTerm<Model.Crole>(condition, "Crole", ref lpmRole);
            ViewData["lmr"] = lmr;


            //搜索字段在页面中显示
            ViewBag.acName = acName;
            ViewBag.acResult = acResult;
            ViewBag.acRole = acRole;

            return View();
        }

        //添加活动信息
        [HttpPost] //这是一个方法
        public ActionResult Add(Model.Activity ac)
        {
            bool issuccess = BLL.CommonBLL.add<Model.Activity>(ac);
            var ret = new
            {
                messagecode = issuccess ? 1 : 0,
            };
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        //删除活动
        [HttpPost]
        public ActionResult Delete(Model.Activity ac)
        {
            bool issuccess = BLL.CommonBLL.Delete<Model.Activity>(ac);
            var ret = new
            {
                messagecode = issuccess ? 1 : 0,
            };
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

      //编辑活动信息
      [HttpPost]
        public ActionResult Edit(Model.Activity ac)
        {
            bool issuccess = BLL.CommonBLL.Update<Model.Activity>(ac);
            var ret = new
            {
                messagecode = issuccess ? 1 : 0,
            };
            return Json(ret, JsonRequestBehavior.AllowGet);
        }











    }
}