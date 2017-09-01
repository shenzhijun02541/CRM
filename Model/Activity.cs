using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public enum acResult
    {
        未评价=0,
        一般=1,
        中等=2,
        好评=3,

    }
    [DBUtility.KeyID(ID = "acId")]
    public class Activity
    {
        // 主键
        public int? acId { get; set; }

        //活动名称
        public string acName { get; set; }

        //活动负责人
        public int? acRole { get; set; }

        //活动预算
        public decimal acBudget { get; set; }

        //活动评价
        public int acResult { get; set; }

        //开始时间
        public DateTime? acBeginTime { get; set; }

        //结束时间
        public DateTime? acEndTime { get; set; }

        //角色名称
        public string RoleName { get; set; }
      
    }
}
