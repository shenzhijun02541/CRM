using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [DBUtility.KeyID(ID = "crno")]
    public class Crole
    {
        //角色管理表
        //crno(主键)
        public int? crno { get; set; }

        //角色名称
        public string crne { get; set; }

    }
}

