using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class CommonBLL
    {
        private static DAL.CommonDAL _dal;
        private static DAL.CommonDAL dal
        {
            get
            {
                if (_dal == null)
                {
                    _dal = new DAL.CommonDAL();
                }
                return _dal;
            }
        }
        /// <summary>
        /// 新增实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Model"></param>
        /// <returns></returns>
        public static bool add<T>(T Model)
        {
            return dal.add<T>(Model);
        }
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public static bool Update<T>(T Model)
        {
            return dal.Update<T>(Model);
        }
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Model"></param>
        /// <returns></returns>
        public static bool Delete<T>(T Model)
        {
            return dal.Delete<T>(Model);
        }
    }
}