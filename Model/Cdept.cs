using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [DBUtility.KeyID(ID = "cdno")]
    public class Cdept
    {
        //部门管理表
        //部门编号（主键）
        public int? cdno { get; set; }

        //部门名称
        public string cdne { get; set; }

    }
}
