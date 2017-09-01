using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Reflection;
using System.Collections;
using System.Text.RegularExpressions;
using System.Data;

namespace DBUtility
{
    public class DisposeSqlHelp
    {
        /// <summary>
        /// 读取单条sqldatareader，返回单个实体类
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="dr">SqlDataReader</param>
        /// <returns></returns>
        public static T ReaderToModel<T>(SqlDataReader dr)
        {
            using (dr)
            {
                if (dr.Read())//读取一次
                {
                    Type modelType = typeof(T);
                    int count = dr.FieldCount;
                    T model = Activator.CreateInstance<T>();
                    for (int i = 0; i < count; i++)
                    {
                        if (!IsNullOrDBNull(dr[i]))
                        {
                            PropertyInfo pi = modelType.GetProperty(dr.GetName(i), BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                            if (pi != null)
                            {
                                pi.SetValue(model, HackType(dr[i], pi.PropertyType), null);
                            }
                        }
                    }
                    return model;
                }
            }
            dr.Close();
            return default(T);
        }
        /// <summary>
        /// 读取多条sqldatareader，返回实体类的泛型集合
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="dr">SqlDataReader</param>
        /// <returns></returns>
        public static IList<T> ReaderToList<T>(SqlDataReader dr)
        {
            using (dr)
            {
                List<T> list = new List<T>();
                Type modelType = typeof(T);
                int count = dr.FieldCount;
                while (dr.Read())
                {
                    T model;
                    model = Activator.CreateInstance<T>();
                    for (int i = 0; i < count; i++)
                    {
                        if (!IsNullOrDBNull(dr[i]))
                        {
                            PropertyInfo pi = modelType.GetProperty(dr.GetName(i), BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                            if (pi != null)
                            {
                                pi.SetValue(model, HackType(dr[i], pi.PropertyType), null);
                            }
                        }
                    }
                    list.Add(model);
                }
                dr.Close();
                return list;
            }
        }
        /// <summary>
        /// 这个类对空类型进行判断转换，要不然会报错  
        /// </summary>
        /// <param name="value"></param>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        private static object HackType(object value, Type conversionType)
        {
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                    return null;

                System.ComponentModel.NullableConverter nullableConverter = new System.ComponentModel.NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }
            return Convert.ChangeType(value, conversionType);
        }


        private static bool IsNullOrDBNull(object obj)
        {
            return (obj == null || (obj is DBNull)) ? true : false;
        }

        /// <summary>
        /// 返回插入SQL语句
        /// </summary>
        /// <typeparam name="T">实现Imodel实体类的类型</typeparam>
        /// <typeparam name="K">实现IDB的类的类型</typeparam>
        /// <param name="t">实现Imodel实体类</param>
        /// <param name="k">实现IDB的类</param>
        /// <returns>影响的条数</returns>
        public static string ReturnSql<T>(T t)
        {
            Type type = typeof(T);
            String tableName = type.Name;//表名
            object[] obsoletAttribut = type.GetCustomAttributes(typeof(ObsoleteAttribute), false);
            if (obsoletAttribut != null && obsoletAttribut.Length > 0)
            {
                tableName = ((ObsoleteAttribute)obsoletAttribut[0]).Message.Trim();
            }
            PropertyInfo[] info = type.GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.IgnoreCase | BindingFlags.Public);

            String sql = "Insert into " + tableName + "(";
            String sql1 = ""; //Table(........)
            String sql2 = ""; //Values(........)
            for (int i = 0; i < info.Length; i++)
            {
                if (info[i].GetValue(t, null) != null)
                {
                    PropertyInfo mycolumn = info[i];
                    if (mycolumn != null)
                    {
                        //判断当类型为datetime时，且值为空时，不进入赋值，以此来保证数据库里datetime列为空--因为datetime为值类型，值类型是不能赋值为null
                        string dt = mycolumn.GetValue(t, null).ToString();
                        DateTime tempTime = new DateTime();
                        if (mycolumn.PropertyType.Name.ToLower() != "datetime")     //判断是否为日期类型，不能使用mycolumn.PropertyType.Name来判断，因为有为空的日期类型会判断成一个null后边还带个“~”的类型
                        {
                            //元数据与数据库字段对应的名称
                            if (mycolumn.PropertyType.Name.ToLower() == "nullable`1" && mycolumn.PropertyType.FullName.ToLower().IndexOf("system.datetime") > -1)
                            {
                                tempTime = Convert.ToDateTime(dt);
                                if (tempTime != DateTime.MinValue)
                                {
                                    sql1 += mycolumn.Name + ",";
                                    sql2 += "'" + Convert.ToDateTime(mycolumn.GetValue(t, null)).ToString("yyyy-MM-dd HH:mm:ss.fff") + "',";
                                }
                            }
                            else
                            {
                                sql1 += mycolumn.Name + ",";
                                sql2 += "'" + Convert.ToString(mycolumn.GetValue(t, null)).Replace("'", "''") + "',";
                            }
                        }
                        else
                        {
                            tempTime = Convert.ToDateTime(dt);
                            //元数据与数据库字段对应的名称
                            if (tempTime != DateTime.MinValue)
                            {
                                sql1 += mycolumn.Name + ",";
                                sql2 += "'" + Convert.ToDateTime(mycolumn.GetValue(t, null)).ToString("yyyy-MM-dd HH:mm:ss.fff") + "',";
                            }
                        }
                    }
                }
            }
            sql1 = sql1.TrimEnd(',');
            sql2 = sql2.TrimEnd(',');
            sql = sql + sql1 + ") Values(" + sql2 + ") SELECT @@IDENTITY;";
            return sql;
        }
        public static string ReturnUpdateSql<T>(T model, bool isUpdateNull = false)
        {
            Type type = typeof(T);

            int i = type.GetProperties().Count();
            object[] classAttr = type.GetCustomAttributes(typeof(KeyID), false);
            System.Reflection.PropertyInfo keyAttr = null;
            if (classAttr.Length > 0)
            {
                keyAttr = type.GetProperty(((KeyID)classAttr[0]).ID, BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.IgnoreCase | BindingFlags.Public);
            }
            else
            {
                keyAttr = type.GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.IgnoreCase | BindingFlags.Public)[0];
            }
            if (keyAttr.GetValue(model, null) != null)
            {
                string sql = "update " + type.Name + " set ";
                foreach (System.Reflection.PropertyInfo property in type.GetProperties())
                {
                    if (property.Name != keyAttr.Name)
                    {
                        object value = property.GetValue(model, null);
                        if (value != null)
                        {
                            if (property.PropertyType.Name.ToLower() == "string")
                            {
                                sql += property.Name + "='" + Convert.ToString(value).Replace("'", "''") + "',";
                            }
                            else if (property.PropertyType.Name.ToLower() == "int")
                            {
                                sql += property.Name + "=" + value + ",";
                            }
                            else if (property.PropertyType.Name.ToLower() == "datetime")
                            {
                                if ((property.GetValue(model, null) as DateTime?) != DateTime.MinValue)
                                {
                                    sql += property.Name + "='" + value + "',";
                                }
                                else
                                {
                                    if (isUpdateNull)
                                    {
                                        sql += property.Name + " = null,";
                                    }
                                }
                            }
                            else if (property.PropertyType.Name.ToLower() == "nullable`1")
                            {
                                if (property.PropertyType.FullName.ToLower().IndexOf("system.datetime") > -1)
                                {
                                    if ((property.GetValue(model, null) as DateTime?) != DateTime.MinValue)
                                    {
                                        sql += property.Name + "='" + value + "',";
                                    }
                                    else
                                    {
                                        if (isUpdateNull)
                                        {
                                            sql += property.Name + " = null,";
                                        }
                                    }
                                }
                                else
                                {
                                    sql += property.Name + "='" + value + "',";
                                }
                            }
                            else
                            {
                                sql += property.Name + "='" + value + "',";
                            }
                        }
                        else
                        {
                            if (isUpdateNull)
                            {
                                sql += property.Name + "=NULL,";
                            }
                        }
                    }
                }
                sql = sql.Substring(0, sql.Length - 1) + " where " + keyAttr.Name + "='" + keyAttr.GetValue(model, null) + "'";
                return sql;
            }
            else
            {
                return "";
            }
        }
        public static string ReturnDeleteSql<T>(T model)
        {
            Type type = typeof(T);

            int i = type.GetProperties().Count();
            object[] classAttr = type.GetCustomAttributes(typeof(KeyID), false);
            System.Reflection.PropertyInfo keyAttr = null;
            if (classAttr.Length > 0)
            {
                keyAttr = type.GetProperty(((KeyID)classAttr[0]).ID, BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.IgnoreCase | BindingFlags.Public);
            }
            else
            {
                keyAttr = type.GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.IgnoreCase | BindingFlags.Public)[0];
            }
            if (keyAttr.GetValue(model, null) != null)
            {
                string sql = "delete from " + type.Name + " where " + keyAttr.Name + "='" + keyAttr.GetValue(model, null) + "'";
                return sql;
            }
            else
            {
                return "";
            }
        }
        /// <summary>
        /// 执行插入命令，By: SQL
        /// </summary>
        /// <typeparam name="T">实体类类型</typeparam>
        /// <param name="t">实例</param>
        /// <returns></returns>
        public static int ExccuteInsertBySQL<T>(T t)
        {
            string sql = ReturnSql<T>(t);
            return DBUtility.SqlHelp.ExecuteSingleSql(sql);
        }

        /// <summary>
        /// 执行插入命令,By: SqlParameter
        /// </summary>
        /// <typeparam name="T">实体类类型</typeparam>
        /// <param name="t">实例</param>
        /// <returns></returns>
        public static int ExccuteInsertByPara<T>(T t)
        {
            Type type = typeof(T);
            String tableName = type.Name;//表名
            PropertyInfo[] info = type.GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.IgnoreCase | BindingFlags.Public);

            String sql = "Insert into " + tableName + "(";
            String sqlColumn = ""; //Table(........)
            String sqlValue = ""; //Values(........)
            List<SqlParameter> listPara = new List<SqlParameter>();
            for (int i = 0; i < info.Length; i++)
            {
                if (info[i].GetValue(t, null) != null)
                {
                    PropertyInfo mycolumn = info[i];
                    if (mycolumn != null)
                    {
                        string dt = mycolumn.GetValue(t, null).ToString();
                        if (!(mycolumn.PropertyType.Name.ToLower() == "datetime" && DateTime.Parse(dt) == DateTime.Parse("0001-1-1 0:00:00")))
                        {
                            sqlColumn += mycolumn.Name + ",";
                            sqlValue += "@" + mycolumn.Name + ",";
                            SqlParameter para = new SqlParameter("@" + mycolumn.Name, mycolumn.GetValue(t, null));
                            listPara.Add(para);
                        }
                    }
                }
            }
            sqlColumn = sqlColumn.TrimEnd(',');
            sqlValue = sqlValue.TrimEnd(',');
            sql = sql + sqlColumn + ") Values(" + sqlValue + ")";
            SqlParameter[] paras = listPara.ToArray();
            return DBUtility.SqlHelp.ExecuteParaSql(sql, paras);
        }
    }
}
