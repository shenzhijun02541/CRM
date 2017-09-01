using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class CommonDAL
    {
        /// <summary>
        /// 新增实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Model"></param>
        /// <returns></returns>
        public bool add<T>(T Model)
        {
            bool flag = false;
            int res = DBUtility.SqlHelp.ExecuteSingleSql(DBUtility.DisposeSqlHelp.ReturnSql<T>(Model));
            if (res > 0)
            {
                flag = true;
            }
            return flag;
        }
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public bool Update<T>(T Model)
        {
            bool flag = false;
            int res = DBUtility.SqlHelp.ExecuteSingleSql(DBUtility.DisposeSqlHelp.ReturnUpdateSql<T>(Model));
            if (res > 0)
            {
                flag = true;
            }
            return flag;
        }
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Model"></param>
        /// <returns></returns>
        public bool Delete<T>(T Model)
        {
            bool flag = false;
            int res = DBUtility.SqlHelp.ExecuteSingleSql(DBUtility.DisposeSqlHelp.ReturnDeleteSql<T>(Model));
            if (res > 0)
            {
                flag = true;
            }
            return flag;
        }
    }
}
