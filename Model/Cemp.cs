using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [DBUtility.KeyID(ID = "ceno")]
    public class Cemp
    {
        //人员管理表的字段
        //(人员编号）主键
        public int? ceno { get; set; }

        //人员名称
        public string cene { get; set; }

        //职位
        public string cjob { get; set; }

        //工资
        public decimal csal { get; set; }

        //奖金
        public decimal comm { get; set; }

        //入职时间
        public DateTime hiredate { get; set; }

    }
}