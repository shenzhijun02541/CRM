using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Check(Model.Cuser mcu)
        {
            IList<Model.Cuser> lmcu = new List<Model.Cuser>();
            IList<KeyValuePair<string, object>> condition = new List<KeyValuePair<string, object>>();
            condition.Add(new KeyValuePair<string, object>("cuname", mcu.cuname));
            condition.Add(new KeyValuePair<string, object>("cpassword", LIB.MD5.GetMD5(mcu.cpassword)));
            condition.Clear();
            LIB.PageModel lpm = new LIB.PageModel();
            lmcu = LIB.MoreTermSelect.MoreTerm<Model.Cuser>(condition, "Cuser", ref lpm);
            if (lmcu.Count > 0)
            {
                Session["UserInfo"] = lmcu[0];
                return RedirectToAction("Index", "Home");
            }
               
            else
            {
                return RedirectToAction("Index", "Login");
            }
                
        }
    
    }
}