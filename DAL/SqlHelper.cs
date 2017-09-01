using System;
using System.Configuration;//
using System.Data;//
using System.Data.SqlClient;//

public class SqlHelper
{
    private SqlConnection conn = null;
    private SqlCommand cmd = null;
    private SqlDataReader sdr = null;
    public static readonly string ConnectionStringLocalTransaction = ConfigurationManager.ConnectionStrings["connstring"].ConnectionString;
    public SqlHelper()
    {
        /*
         * 先引用组件：System.configuration
         * 再using System.Configuration
         * 	修改节点：<connectionStrings><add name="connStr" connectionString="数据库连接字符串"/></connectionStrings>
         */
        
        conn = new SqlConnection(ConnectionStringLocalTransaction);
    }
    public bool ss()
    {
        if (conn.State == ConnectionState.Open)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //打开conn
    private SqlConnection GetConn()
    {
        if (conn.State == ConnectionState.Closed)
        {
            conn.Open();
        }
        return conn;
    }

    /// <summary>
    /// 该方法执行传入增删改SQL语句，返回受影响行数
    /// </summary>
    /// <param name="sql">增删改SQL语句</param>
    /// <returns>返回受影响行数</returns>
    public int ExecuteNonQuery(String sql)
    {
        int res;
        try
        {
            SqlCommand cmd = new SqlCommand(sql, GetConn());
            res = cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {

            throw ex;
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
        return res;
    }

    /// <summary>
    ///  该方法执行传入增删改SQL语句，返回受影响行数(+1)
    /// </summary>
    /// <param name="cmdText">增删改SQL语句</param>
    /// <returns></returns>
    public int ExecuteNonQuery(string cmdText, SqlParameter[] paras)
    {
        int res;
        using (cmd = new SqlCommand(cmdText, GetConn()))
        {
            cmd.Parameters.AddRange(paras);
            res = cmd.ExecuteNonQuery();
        }
        return res;
    }
    /// <summary>
    /// 该方法执行传入的Sql查询语句
    /// </summary>
    /// <param name="sql">要执行的Sql查询语句</param>
    /// <returns>返回DataTable</returns>
    public DataTable ExecuteQuery(string sql)
    {
        DataTable dt = new DataTable();
        cmd = new SqlCommand(sql, GetConn());
        using (sdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
        {
            dt.Load(sdr);
        }
        return dt;
    }

}
