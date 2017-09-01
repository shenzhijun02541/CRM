using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections;
using System.Data;

namespace DBUtility
{
    public class SqlHelp
    {
        #region 执行简单SQL语句
        /// <summary>
        /// 执行单条增删改SQL语句，返回影响记录数
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>返回影响记录数</returns>
        public static int ExecuteSingleSql(string sql, string strConn = "conn")
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[strConn].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    try
                    {
                        conn.Open();
                        int rows = cmd.ExecuteNonQuery();
                        conn.Close();
                        return rows;
                    }
                    catch (SqlException e)
                    {
                        conn.Close();
                        throw new Exception(e.Message);
                    }
                }
            }
        }
        /// <summary>
        /// 返回查询结果集的第一行的第一列（object）
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>返回第一行的第一列</returns>
        public static object ExecuteSqlScalar(string sql, string strConn = "conn")
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[strConn].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    try
                    {
                        conn.Open();
                        object value = cmd.ExecuteScalar();
                        cmd.Connection.Close();
                        conn.Close();
                        if (value == null || Object.Equals(value, DBNull.Value))
                        {
                            return null;
                        }
                        else
                        {
                            return value;
                        }
                    }
                    catch (SqlException e)
                    {
                        conn.Close();
                        throw new Exception(e.Message);
                    }
                }
            }
        }


        /// <summary>
        /// 执行单条增删改SQL语句，返回影响记录数
        /// （很少用到）
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="times">设置命令过期时间</param>
        /// <returns>返回影响记录数</returns>
        public static int ExecuteSqlByTime(string sql, int times, string strConn = "conn")
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[strConn].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.CommandTimeout = times;
                        int rows = cmd.ExecuteNonQuery();
                        conn.Close();
                        return rows;
                    }
                    catch (SqlException e)
                    {
                        conn.Close();
                        throw new Exception(e.Message);
                    }
                }
            }
        }


        /// <summary>
        /// 执行多条SQL语句，实现数据库事务
        /// </summary>
        /// <param name="sqls">SQL集合</param>
        /// <returns>返回影响的记录数，理论上应达到每一条SQL命令影响行数不为零，否则Rollback</returns>
        public static int ExecuteMoreSql(List<string> sqls, string strConn = "conn")
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[strConn].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                string sql = String.Empty;
                try
                {
                    int count = 0;
                    string strsql = "SET XACT_ABORT ON;begin TRAN;";
                    for (int n = 0; n < sqls.Count; n++)
                    {
                        strsql += ";" + sqls[n];

                    }
                    strsql += ";commit TRAN";
                    sql = strsql;
                    cmd.CommandText = strsql;
                    count = cmd.ExecuteNonQuery();
                    return count;
                }
                catch (SqlException e)
                {
                    conn.Close();
                    throw new Exception(e.Message + sql);
                    return 0;
                }
            }
        }



        /// <summary>
        /// 执行多条SQL语句，实现数据库事务
        /// </summary>
        /// <param name="sqls">SQL集合</param>
        /// <returns>返回影响的记录数，理论上应达到每一条SQL命令影响行数不为零，否则Rollback</returns>
        public static int ExecuteMoreSqlForBigData(List<string> sqls, string strConn = "conn")
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[strConn].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                int count = 0;
                try
                {
                    int tames = sqls.Count / 50 + 1;
                    for (int i = 0; i < tames; i++)
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandTimeout = 0;
                        cmd.Connection = conn;
                        StringBuilder sbSql = new StringBuilder();
                        string sql = string.Empty;
                        sbSql.Append("SET XACT_ABORT ON;begin TRAN;");
                        if (i != tames - 1)
                        {
                            for (int n = 0; n < 50; n++)
                            {
                                sbSql.Append(";" + sqls[i * 50 + n]);
                            }
                        }
                        else
                        {
                            for (int n = 0, max = sqls.Count; n < max % 50; n++)
                            {
                                sbSql.Append(";" + sqls[i * 50 + n]);
                            }
                        }
                        sbSql.Append(";commit TRAN");
                        cmd.CommandText = sbSql.ToString();
                        count += cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    return count;
                }
                catch (SqlException e)
                {
                    conn.Close();
                    return 0;
                    //throw new Exception(e.Message + sql);
                }
            }
        }




        /// <summary>
        /// 执行多条SQL语句，实现数据库事务
        /// </summary>
        /// <param name="sqls">SQL集合</param>
        /// <returns>返回影响的记录数，理论上应达到每一条SQL命令影响行数不为零，否则Rollback</returns>
        public static int ExecuteMoreSql(List<string> sqls, out string error, string strConn = "conn")
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[strConn].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                try
                {
                    int count = 0;
                    string strsql = "SET XACT_ABORT ON;  BEGIN TRY begin TRAN;";
                    for (int n = 0; n < sqls.Count; n++)
                    {
                        strsql += ";" + sqls[n];
                    }
                    strsql += ";commit TRAN END TRY";
                    StringBuilder catchSqlBlock = new StringBuilder();
                    catchSqlBlock.Append(" BEGIN CATCH ");
                    catchSqlBlock.Append(" DECLARE @ErrorMessage NVARCHAR(4000); ");
                    catchSqlBlock.Append(" DECLARE @ErrorSeverity INT; ");
                    catchSqlBlock.Append(" DECLARE @ErrorState INT; ");
                    catchSqlBlock.Append(" SELECT  @ErrorMessage = ERROR_MESSAGE() , ");
                    catchSqlBlock.Append(" @ErrorSeverity = ERROR_SEVERITY() , ");
                    catchSqlBlock.Append(" @ErrorState = ERROR_STATE(); ");

                    catchSqlBlock.Append(" RAISERROR (@ErrorMessage,  ");
                    catchSqlBlock.Append(" @ErrorSeverity,  ");
                    catchSqlBlock.Append(" @ErrorState  ");
                    catchSqlBlock.Append(" ); ");
                    catchSqlBlock.Append(" IF ( @@TRANCOUNT > 0 ) ");
                    catchSqlBlock.Append(" BEGIN ");
                    catchSqlBlock.Append(" ROLLBACK TRANSACTION ");
                    catchSqlBlock.Append(" END ");
                    catchSqlBlock.Append(" END CATCH ");
                    strsql += catchSqlBlock.ToString();
                    cmd.CommandText = strsql;
                    count = cmd.ExecuteNonQuery();
                    error = string.Empty;
                    return count;
                }
                catch (Exception ex)
                {
                    error = ex.Message.Replace("\'", "").Replace("\r\n", "");
                    return 0;
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务
        /// </summary>
        /// <param name="sqls">SQL集合</param>
        /// <returns>返回影响的记录数，理论上应达到每一条SQL命令影响行数不为零，否则Rollback</returns>
        public static List<string> ExecuteMoreSqlGetList(List<string> sqls, string strConn = "conn")
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[strConn].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                try
                {
                    //int count = 0;
                    string strsql = "SET XACT_ABORT ON;begin TRAN";
                    for (int n = 0; n < sqls.Count; n++)
                    {
                        strsql += ";" + sqls[n];
                    }
                    strsql += ";commit TRAN";
                    cmd.CommandText = strsql;
                    //SqlDataReader objReder = new SqlDataReader;
                    SqlDataReader objReder = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    List<string> readerList = new List<string>();
                    int i = 0;
                    while (objReder.Read())
                    {
                        readerList.Add(objReder[0].ToString());
                        objReder.NextResult();
                        i++;
                    }
                    objReder.Close();
                    readerList.Insert(0, i.ToString());
                    return readerList;
                }
                catch
                {
                    return null;
                }
            }
        }


        /// <summary>
        /// 执行多条SQL语句，实现数据库事务
        /// </summary>
        /// <param name="sqls">SQL集合</param>
        /// <returns>返回影响的记录数，理论上应达到每一条SQL命令影响行数不为零，否则Rollback</returns>
        public static int ExecuteMoreSqlWithOutTimeOut(List<string> sqls, int timeOut, out string error, string strConn = "conn")
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[strConn].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                try
                {
                    int count = 0;
                    string strsql = "SET XACT_ABORT ON;  BEGIN TRY begin TRAN;";
                    for (int n = 0; n < sqls.Count; n++)
                    {
                        strsql += ";" + sqls[n];
                    }
                    strsql += ";commit TRAN END TRY";
                    StringBuilder catchSqlBlock = new StringBuilder();
                    catchSqlBlock.Append(" BEGIN CATCH ");
                    catchSqlBlock.Append(" DECLARE @ErrorMessage NVARCHAR(4000); ");
                    catchSqlBlock.Append(" DECLARE @ErrorSeverity INT; ");
                    catchSqlBlock.Append(" DECLARE @ErrorState INT; ");
                    catchSqlBlock.Append(" SELECT  @ErrorMessage = ERROR_MESSAGE() , ");
                    catchSqlBlock.Append(" @ErrorSeverity = ERROR_SEVERITY() , ");
                    catchSqlBlock.Append(" @ErrorState = ERROR_STATE(); ");

                    catchSqlBlock.Append(" RAISERROR (@ErrorMessage,  ");
                    catchSqlBlock.Append(" @ErrorSeverity,  ");
                    catchSqlBlock.Append(" @ErrorState  ");
                    catchSqlBlock.Append(" ); ");
                    catchSqlBlock.Append(" IF ( @@TRANCOUNT > 0 ) ");
                    catchSqlBlock.Append(" BEGIN ");
                    catchSqlBlock.Append(" ROLLBACK TRANSACTION ");
                    catchSqlBlock.Append(" END ");
                    catchSqlBlock.Append(" END CATCH ");
                    strsql += catchSqlBlock.ToString();
                    cmd.CommandText = strsql;

                    cmd.CommandTimeout = timeOut;

                    count = cmd.ExecuteNonQuery();
                    error = string.Empty;
                    return count;
                }
                catch (Exception ex)
                {
                    error = ex.Message.Replace("\'", "").Replace("\r\n", "");
                    return 0;
                }
            }
        }


        /// <summary>
        /// 向数据库里添加图像格式的数据
        /// 只适用于数据表里只有一个字段为IMAGE格式时，该方法可用ExecuteSingleSql代替。
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="fs">图像字节，数据库里字段类型为Image的情况</param>
        /// <returns>返回受影响的记录</returns>
        public static int ExecuteSqlInsertImg(string sql, byte[] fs, string strConn = "conn")
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[strConn].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlParameter para = new SqlParameter("@fs", SqlDbType.Image);
                para.Value = fs;
                cmd.Parameters.Add(para);
                try
                {
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    conn.Close();
                    return rows;
                }
                catch (SqlException e)
                {
                    throw new Exception(e.Message);
                }
                finally
                {
                    cmd.Dispose();
                    conn.Close();
                }
            }
        }
        /// <summary>
        /// 执行查询语句，返回SqlDataReader查询结果集
        /// 处理完Dr后，一定要关闭SqlDataReader
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>返回SqlDataReader查询结果集</returns>
        public static SqlDataReader SelectReader(string sql, string strConn = "conn")
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[strConn].ConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(sql, conn);
            try
            {
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return dr;
            }
            catch (SqlException e)
            {
                conn.Close();
                throw new Exception(e.Message.Replace("\'", "").Replace("\r\n", ""));
            }
        }

        /// <summary>
        /// 执行查询语句，返回SqlDataReader查询结果集 （防超时）
        /// 处理完Dr后，一定要关闭SqlDataReader
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>返回SqlDataReader查询结果集</returns>
        public static SqlDataReader SelectReaderWithOutTimeOut(string sql, int time, string strConn = "conn")
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[strConn].ConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(sql, conn);
            try
            {
                conn.Open();

                cmd.CommandTimeout = time;

                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return dr;
            }
            catch (SqlException e)
            {
                conn.Close();
                throw new Exception(e.Message.Replace("\'", "").Replace("\r\n", ""));
            }
        }

        /// <summary>
        /// 返回DataSet数据集
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <returns>DataSet数据集</returns>
        public static DataSet SelectDataSet(string sql, string strConn = "conn")
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[strConn].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    conn.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(sql, conn);
                    sda.Fill(ds);
                    conn.Close();
                    return ds;
                }
                catch (SqlException e)
                {

                    throw new Exception(e.Message);
                }
            }
        }


        /// <summary>
        /// 返回DataSet数据集_数据访问延时
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <param name="time">延时时间(秒)</param>
        /// <returns>DataSet数据集</returns>
        public static DataSet SelectDataSetLongTime(string sql, int time, string strConn = "conn")
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[strConn].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.CommandTimeout = time;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    conn.Close();
                    return ds;
                }
                catch (SqlException e)
                {
                    throw new Exception(e.Message);
                }
            }

        }
        #endregion

        #region 执行带参数（SqlParameter）的SQL语句

        /// <summary>
        /// 预处理SqlCommand命令,数据库连接/事务/命令类型/参数
        /// </summary>
        /// <param name="cmd">SqlCommand命令</param>
        /// <param name="conn">SqlConnection会话</param>
        /// <param name="tran">一个有效的事务或Null值</param>
        /// <param name="cmdText">T-SQL命令文本</param>
        /// <param name="cmdParas">分配给命令的SqlParamter参数数组</param>
        public static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction tran, string cmdText, SqlParameter[] cmdParas, string strConn = "conn")
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[strConn].ConnectionString;
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (tran != null)
                cmd.Transaction = tran;
            cmd.CommandType = CommandType.Text;
            if (cmdParas != null)
            {
                foreach (SqlParameter para in cmdParas)
                {
                    if ((para.Direction == ParameterDirection.InputOutput || para.Direction == ParameterDirection.Input) && para.Value == null)
                        para.Value = DBNull.Value;
                    cmd.Parameters.Add(para);
                }
            }
        }
        /// <summary>
        /// 执行带参数的增/删/改SQL语句，返回影响记录数
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="cmdParas">分配给命令的SqlParamter参数数组</param>
        /// <returns>影像记录数</returns>
        public static int ExecuteParaSql(string sql, SqlParameter[] cmdParas, string strConn = "conn")
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[strConn].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, conn, null, sql, cmdParas);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        conn.Close();
                        return rows;
                    }
                    catch (SqlException e)
                    {
                        conn.Close();
                        throw new Exception(e.Message);
                    }

                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务
        /// 传递Hashtable，KEY为SQL语句，value为该SQL语句的SqlParameter[]
        /// SQL语句可有/无 参数
        /// </summary>
        /// <param name="ht">SQL语句的哈希表(KEY为SQL语句，value为该SQL语句的SqlParameter[])</param>
        /// <returns>返回影响的记录数，理论上应达到每一条SQL命令影响行数不为零，否则Rollback</returns>
        public static int ExecuteMoreParaSql(Hashtable ht, string strConn = "conn")
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[strConn].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                SqlTransaction st = conn.BeginTransaction();
                cmd.Transaction = st;
                try
                {
                    int rows = 0;
                    foreach (DictionaryEntry de in ht)
                    {
                        string sql = de.Key.ToString();
                        if (sql.Contains("|||"))
                        {
                            sql = sql.Split(new string[1] { "|||" }, StringSplitOptions.None)[1];
                        }
                        SqlParameter[] paras = null;//参数数组
                        if (de.Value != null)
                        {
                            if (!de.Value.Equals(null))
                            {
                                paras = (SqlParameter[])de.Value;
                            }
                        }
                        PrepareCommand(cmd, conn, st, sql, paras);
                        int currentResult = cmd.ExecuteNonQuery();
                        if (currentResult > 0)
                        {
                            rows += currentResult;
                        }
                        else//当受影响行数为0时，回滚事务。防止执行多条命令语句时，其中一条命令因并发问题而导致数据未更新
                        {
                            st.Rollback();
                            conn.Close();
                            return 0;
                        }
                        cmd.Parameters.Clear();
                    }
                    st.Commit();
                    conn.Close();
                    return rows;
                }
                catch (SqlException e)
                {
                    st.Rollback();
                    conn.Close();
                    throw new Exception(e.Message);
                }
            }
        }

        /// <summary>
        /// 返回查询(有SqlParameter参数)结果集的第一行的第一列（object）
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="cmdParas">分配给命令的SqlParamter参数数组</param>
        /// <returns>返回第一行的第一列</returns>
        public static object ExecuteParaSqlScalar(string sql, SqlParameter[] cmdParas, string strConn = "conn")
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[strConn].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, conn, null, sql, cmdParas);
                        object objvalue = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        conn.Close();
                        if (Object.Equals(objvalue, null) || Object.Equals(objvalue, DBNull.Value))
                        {
                            return null;
                        }
                        else
                        {
                            return objvalue;
                        }
                    }
                    catch (SqlException e)
                    {

                        throw new Exception(e.Message);
                    }

                }
            }
        }

        /// <summary>
        /// 执行查询(有SqlParameter参数)语句，返回SqlDataReader查询结果集
        /// 注意！使用后一定要对SqlDataReader进行Close
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="cmdParas">分配给命令的SqlParamter参数数组</param>
        /// <returns>SqlDataReader</returns>
        public static SqlDataReader SelectParaReader(string sql, SqlParameter[] cmdParas, string strConn = "conn")
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[strConn].ConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, conn, null, sql, cmdParas);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return dr;
            }
            catch (SqlException e)
            {
                conn.Close();
                throw new Exception(e.Message);
            }

        }

        /// <summary>
        /// 执行查询语句(有SqlParameter参数)，返回DataSet数据集
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <param name="cmdParas">分配给命令的SqlParamter参数数组</param>
        /// <returns>DataSet</returns>
        public static DataSet SelectParaDataSet(string sql, SqlParameter[] cmdParas, string strConn = "conn")
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[strConn].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    PrepareCommand(cmd, conn, null, sql, cmdParas);
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        sda.Fill(ds);
                        cmd.Parameters.Clear();
                        conn.Close();
                        return ds;
                    }
                }
                catch (SqlException e)
                {
                    conn.Close();
                    throw new Exception(e.Message);
                }
            }
        }


        #endregion



        #region 执行存储过程


        /// <summary>
        /// 预处理SqlCommand命令,数据库连接/事务/命令类型/参数
        /// </summary>
        /// <param name="conn">SqlConnection会话</param>
        /// <param name="st">一个有效的事务或Null值</param>
        /// <param name="cmdText">存储过程名称</param>
        /// <param name="cmdParas">分配给命令的SqlParamter参数数组</param>
        /// <returns>SqlCommand</returns>
        public static SqlCommand BuildCommand(SqlConnection conn, SqlTransaction st, string cmdText, SqlParameter[] cmdParas, string strConn = "conn")
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[strConn].ConnectionString;
            SqlCommand cmd = new SqlCommand();
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = cmdText;
            if (st != null)
                cmd.Transaction = st;
            if (cmdParas != null)
            {
                foreach (SqlParameter para in cmdParas)
                {
                    if ((para.Direction == ParameterDirection.InputOutput || para.Direction == ParameterDirection.Input) && para.Value == null)
                        para.Value = DBNull.Value;
                    cmd.Parameters.Add(para);
                }
            }
            return cmd;
        }
        /// <summary>
        /// 预处理SqlCommand命令,基于BuildCommand方法，添加返回值returnValue.
        /// </summary>
        /// <returns>SqlCommand</returns>
        public static SqlCommand BuildIntCommand(SqlConnection conn, SqlTransaction st, string cmdText, SqlParameter[] cmdParas, string strConn = "conn")
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[strConn].ConnectionString;
            SqlCommand cmd = BuildCommand(conn, st, cmdText, cmdParas);
            cmd.Parameters.Add(new SqlParameter("@return", SqlDbType.Int, 4, ParameterDirection.ReturnValue, false, 0, 0, string.Empty, DataRowVersion.Default, null));
            return cmd;
        }

        /// <summary>
        /// 执行查询存储过程，返回SqlDataReader
        /// 注意！使用后一定要对SqlDataReader进行Close
        /// </summary>
        /// <param name="storeProcName">存储过程名</param>
        /// <param name="cmdParas">存储过程参数</param>
        /// <returns>SqlDataReader</returns>
        public static SqlDataReader SelectProcReader(string storeProcName, SqlParameter[] cmdParas, string strConn = "conn")
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[strConn].ConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = BuildCommand(conn, null, storeProcName, cmdParas);
            try
            {
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return dr;
            }
            catch (SqlException e)
            {
                conn.Close();
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// 执行查询存储过程，返回DataSet数据集
        /// </summary>
        /// <param name="storeProcName">存储过程名</param>
        /// <param name="cmdParas">存储过程参数</param>
        /// <returns>DataSet</returns>
        public static DataSet SelectProcDataSet(string storeProcName, SqlParameter[] cmdParas, string strConn = "conn")
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[strConn].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                //sql输出
                conn.InfoMessage += new SqlInfoMessageEventHandler(OnInfoMessage);
                SqlCommand cmd = BuildCommand(conn, null, storeProcName, cmdParas);
                try
                {
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    sda.Fill(ds);
                    cmd.Parameters.Clear();
                    conn.Close();
                    return ds;
                }
                catch (SqlException e)
                {

                    throw new Exception(e.Message + aaa);
                }
            }
        }

        /// <summary>
        /// 执行查询存储过程，返回DataSet数据集（延时）
        /// </summary>
        /// <param name="storeProcName">存储过程名</param>
        /// <param name="cmdParas">存储过程参数</param>
        /// <returns>DataSet</returns>
        public static DataSet SelectProcDataSetWithOutTimeOut(string storeProcName, SqlParameter[] cmdParas, int timeOut, string strConn = "conn")
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[strConn].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                //sql输出
                conn.InfoMessage += new SqlInfoMessageEventHandler(OnInfoMessage);
                SqlCommand cmd = BuildCommand(conn, null, storeProcName, cmdParas);
                try
                {

                    cmd.CommandTimeout = timeOut;

                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    sda.Fill(ds);
                    cmd.Parameters.Clear();
                    conn.Close();
                    return ds;
                }
                catch (SqlException e)
                {

                    throw new Exception(e.Message + aaa);
                }
            }
        }

        /// <summary>
        /// 执行存储过程，返回影响行数（或返回值）
        /// </summary>
        /// <param name="storeProcName">存储过程名</param>
        /// <param name="cmdParas">存储过程参数</param>
        /// <returns>影响行数（或返回值）</returns>
        public static int ExecuteProc(string storeProcName, SqlParameter[] cmdParas, string strConn = "conn")
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[strConn].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlTransaction st = conn.BeginTransaction();
                    SqlCommand cmd = BuildIntCommand(conn, st, storeProcName, cmdParas);
                    int rows = cmd.ExecuteNonQuery();//作用条数
                    st.Commit();
                    int returnvalue = (int)cmd.Parameters["@return"].Value;//返回值
                    conn.Close();
                    return returnvalue;

                }
                catch (SqlException e)
                {

                    throw new Exception(e.Message);
                }
            }

        }



        /// <summary>
        /// (执行多条存储过程)预处理SqlCommand命令,数据库连接/事务/命令类型/参数
        /// </summary>
        /// <param name="cmd">SqlCommand命令</param>
        /// <param name="conn">SqlConnection会话</param>
        /// <param name="tran">一个有效的事务或Null值</param>
        /// <param name="cmdText">T-SQL命令文本</param>
        /// <param name="cmdParas">分配给命令的SqlParamter参数数组</param>
        public static void PrepareProcCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction tran, string cmdText, SqlParameter[] cmdParas, string strConn = "conn")
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[strConn].ConnectionString;
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (tran != null)
                cmd.Transaction = tran;
            cmd.CommandType = CommandType.StoredProcedure;
            if (cmdParas != null)
            {
                foreach (SqlParameter para in cmdParas)
                {
                    if ((para.Direction == ParameterDirection.InputOutput || para.Direction == ParameterDirection.Input) && para.Value == null)
                        para.Value = DBNull.Value;
                    cmd.Parameters.Add(para);
                }
            }
            conn.Close();
        }

        /// <summary>
        /// 执行多条存储过程
        /// 传递Hashtable，KEY为存储过程名，value为该SQL语句的SqlParameter[]
        /// 存储过程可有/无 参数
        /// </summary>
        /// <param name="ht">SQL语句的哈希表(KEY为SQL语句，value为该SQL语句的SqlParameter[])</param>
        /// <returns>返回影响的记录数，理论上应达到每一条命令影响行数不为零，否则Rollback</returns>
        public static int ExecuteProcMore(Hashtable ht, string strConn = "conn")
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[strConn].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlTransaction st = conn.BeginTransaction())
                {
                    SqlCommand cmd = new SqlCommand();
                    int rows = 0;
                    try
                    {
                        foreach (DictionaryEntry de in ht)
                        {
                            string storeName = de.Key.ToString();
                            SqlParameter[] cmdParas = null;
                            if (!de.Value.Equals(null))
                            {
                                cmdParas = (SqlParameter[])de.Value;
                            }
                            PrepareProcCommand(cmd, conn, st, storeName, cmdParas);
                            int currentResult = cmd.ExecuteNonQuery();
                            if (currentResult > 0)
                            {
                                rows += currentResult;
                            }
                            else//当受影响行数为0时，回滚事务。防止执行多条命令语句时，其中一条命令因并发问题而导致数据未更新
                            {
                                st.Rollback();
                                conn.Close();
                                return 0;
                            }
                            cmd.Parameters.Clear();
                        }
                        st.Commit();
                        conn.Close();
                        return rows;
                    }
                    catch (SqlException e)
                    {
                        st.Rollback();
                        conn.Close();
                        throw new Exception(e.Message);
                    }
                }
            }
        }

        #endregion

        #region 输出字符串
        private static void OnInfoMessage(object sender, System.Data.SqlClient.SqlInfoMessageEventArgs args)
        {
            System.Data.SqlClient.SqlError sqlEvent = null;
            foreach (System.Data.SqlClient.SqlError sqlEvent_loopVariable in args.Errors)
            {
                sqlEvent = sqlEvent_loopVariable;
                aaa = sqlEvent.Message;
            }
            //return aaa;
        }
        static string aaa;
        #endregion
    }
}
