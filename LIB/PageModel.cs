using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB
{
    public class PageModel
    {
        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        public int PageCount { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public string fldName { get; set; }
        /// <summary>
        /// 共多少页
        /// </summary>
        public int TotalCount { get; set; }
    }
}
