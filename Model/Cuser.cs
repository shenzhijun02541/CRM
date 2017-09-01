using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [DBUtility.KeyID(ID = "cuno")]
    public class Cuser
    {
        //主键
        public int? cuno { get; set; }

        //用户名
        public string cuname { get; set; }

        //密码
        public string cpassword { get; set; }
    }
}
