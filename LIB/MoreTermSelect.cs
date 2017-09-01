using DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LIB
{
    public class MoreTermSelect
    {
        public class MoreTermModel
        {
            public string datafields { get; set; }
            public string databases { get; set; }
            public string where { get; set; }
        }
        /// <summary>
        /// 组合条件查询函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">条件集合可以在每个value项前加sql的逻辑运算符如：>=等，在每个值加逗号则使用组合or查询，在前后加%则是模糊字符串查询，前两个日期之间加@则为在这两个日期中间的值是左到右的关系，在前面加|则为上个条件的or组合</param>
        /// <param name="tableName">表名</param>
        /// <param name="dataFields">字段名（可使用as重命名）</param>
        /// <param name="isand">主框架是否为and</param>
        /// <returns>返回T所代表的类型IList</returns>
        public static IList<T> MoreTerm<T>(IList<KeyValuePair<string, object>> model, string tableName, ref PageModel PageModel, string dataFields = "", bool isand = true, string strConn = "conn", bool isAllFields = true)
        {
            MoreTermModel objMoreTerm = ComposeSQL(model, tableName, dataFields, isand, isAllFields);

            string sql = "select " + objMoreTerm.datafields + " from " + objMoreTerm.databases;
            if (objMoreTerm.where != null)
            {
                sql += " where " + objMoreTerm.where;
            }
            if(PageModel != null&& !string.IsNullOrWhiteSpace(PageModel.fldName)&& PageModel.PageCount!=0&& PageModel.PageSize!=0)
            {
                PageModel.TotalCount = DisposeSqlHelp.ReaderToList<T>(SqlHelp.SelectReader(sql, strConn)).Count;
                int firstPage = (PageModel.PageCount - 1) * PageModel.PageSize + 1;
                sql = "select top("+ PageModel.PageSize + ") *  from (SELECT  ROW_NUMBER() OVER(ORDER BY "+ PageModel.fldName + ") RowNo,t.* from ("+sql+") t) w where  RowNo BETWEEN "+ firstPage + " and "+ PageModel.PageCount* PageModel.PageSize + " ";
            }
            return DisposeSqlHelp.ReaderToList<T>(SqlHelp.SelectReader(sql, strConn));
        }
        #region 组合SQL语句
        /// <summary>
        /// 组合SQL语句
        /// </summary>
        /// <param name="model">条件键值对</param>
        /// <param name="tableName">表名</param>
        /// <param name="dataFields">字段名</param>
        /// <param name="isand">主框架是否是and</param>
        /// <returns>MoreTermModel</returns>
        public static MoreTermModel ComposeSQL(IList<KeyValuePair<string, object>> model, string tableName, string dataFields = "", bool isand = true, bool isAllFields = true)
        {
            string where = "";
            Regex reg = new Regex(@"([><!=]+)|^([2|]{0,2}\s*is.+)|^(not in)|^(in)|^(not like)");
            for (int i = 0; i < model.Count; i++)
            {
                #region 运算标示符
                if (reg.IsMatch(model[i].Value.ToString()))
                {
                    Match regValue = reg.Match(model[i].Value.ToString());
                    where += startOR(model, i, isand);
                    where += model[i].Key + " " + model[i].Value.ToString().Replace("2|", "").Replace("|", "");
                    where += endOR(model, i);
                }
                #endregion
                #region like
                else if (model[i].Value.ToString().Contains("%"))
                {
                    if (i == model.Count - 1 || model[i].Value.ToString().Replace("%", "") != "" || model[i + 1].Value.ToString().Contains("|")) //不能删除后边的，为了添加or
                    {
                        where += startOR(model, i, isand);
                        where += model[i].Key + " like '" + model[i].Value.ToString().Replace("2|", "").Replace("|", "") + "'";
                        where += endOR(model, i);
                    }
                }
                #endregion
                #region in
                else if (model[i].Value.ToString().Contains(","))
                {
                    string values = model[i].Value.ToString().Replace("2|", "").Replace("|", "");
                    where += startOR(model, i, isand);
                    string[] value = values.Split(',');
                    int tryint = 0;
                    if (int.TryParse(value[0], out tryint))
                    {
                        where += model[i].Key + " in (" + values.Trim(',') + ")";
                    }
                    else
                    {
                        string InWhere = "";
                        foreach (string val in value)
                        {
                            InWhere += "\'" + val + "\' ,";
                        }
                        where += model[i].Key + " in (" + InWhere.Trim(',') + ")";
                    }
                    where += endOR(model, i);
                }
                #endregion

                #region 时间
                else if (model[i].Value.ToString().Contains("@"))
                {
                    string[] times = model[i].Value.ToString().Split('@');
                    where += startOR(model, i, isand);
                    if (times[0].Trim() != "")                         //如果-前没有内容则直接跳过
                    {
                        where += model[i].Key + ">='" + times[0].Replace("2|", "").Replace("|", "") + "'";   //由于第一个可能有|则必须删除此字符，第二个就不用删除了
                    }
                    if (times[1].Trim() != "")                         //如果-后没有内容则直接跳过
                    {
                        if (times[0].Trim() != "")
                        {
                            where += " and ";
                        }
                        where += model[i].Key + "<='" + times[1] + "'";
                    }
                    where += endOR(model, i);
                }
                #endregion


                #region =
                else
                {
                    where += startOR(model, i, isand);
                    where += model[i].Key + "='" + model[i].Value.ToString().Replace("2|", "").Replace("|", "") + "'";
                    where += endOR(model, i);
                }
                #endregion
            }
            #region tableNames datafields
            string[] tableNames = tableName.Split(',');
            string datebases = tableNames[0].Split('.')[0];
            string datafields = "";


            for (int i = 1; i < tableNames.Length; i = i + 2)  //拼数据库连接字段
            {
                //增加Right Join标识 ">" 
                //截取每个tablename的第一个字符
                string fstFlag = tableNames[i].Substring(0, 1);
                if (fstFlag == ">")
                {
                    tableNames[i] = tableNames[i].Substring(1);
                    datebases += " right  join " + tableNames[i].Split('.')[0] + " on " + tableNames[i] + "=" + tableNames[i - 1];
                }
                else
                {
                    datebases += " left join " + tableNames[i].Split('.')[0] + " on " + tableNames[i] + "=" + tableNames[i - 1];
                }
            }
            if (dataFields != "")
            {
                if (isAllFields)
                {
                    List<string> dataField = dataFields.Split(',').ToList();
                    List<string> NoRepeatNames = new List<string>();
                    for (int i = 0; i < tableNames.Length; i++)
                    {
                        if (!NoRepeatNames.Contains(tableNames[i].Split('.')[0]))
                        {
                            NoRepeatNames.Add(tableNames[i].Split('.')[0]);
                        }
                    }
                    for (int i = 0; i < NoRepeatNames.Count; i++)     //拼显示字段名称
                    {
                        int dataFieldCount = dataField.Count(a => a.IndexOf(NoRepeatNames[i].Split('.')[0] + ".") > -1);   //找出表名称是否在重命名字段
                        if (dataFieldCount > 0)
                        {
                            string sqlcol = "select * from syscolumns where id = object_id('" + NoRepeatNames[i].Split('.')[0] + "')";
                            DataSet ds = SqlHelp.SelectDataSet(sqlcol);
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                datafields += NoRepeatNames[i].Split('.')[0] + "." + dr["name"].ToString();
                                for (int j = 0; j < dataField.Count; j++)
                                {
                                    string[] fieldName = Regex.Split(dataField[j], @" as ");
                                    if (fieldName[0].Split('.')[1].ToLower() == dr["name"].ToString().ToLower() && fieldName[0].Split('.')[0].ToLower() == NoRepeatNames[i].Split('.')[0].ToLower())  //判断有没有自定义名称
                                    {
                                        datafields += " as " + fieldName[1];
                                    }
                                }
                                datafields += ",";
                            }
                        }
                        else
                        {
                            datafields += NoRepeatNames[i].Split('.')[0] + ".*,";    //没有重命名的表格直接写星号
                        }
                    }
                }
                else
                {
                    datafields = dataFields;
                }

            }
            else
            {
                datafields = "*";
            }
            datafields = datafields.TrimEnd(',');
            #endregion

            MoreTermModel objMoreTerm = new MoreTermModel();
            objMoreTerm.datafields = datafields;
            objMoreTerm.databases = datebases;

            if (where != "")
            {
                if (isand)
                    objMoreTerm.where = "(" + " 1=1 " + where + ")";
                else
                    objMoreTerm.where = "(" + " 1<>1 " + where + ")";
            }
            return objMoreTerm;

            // return "select " + datafields + " from " + datebases + " where 1=1" + where;
        }
        #endregion

        #region 判断是否下一个是or并添加左括号
        static string startOR(IList<KeyValuePair<string, object>> model, int i, bool isand)    //判断是否下一个是or并添加左括号
        {
            string where = "";
            if (!isand)
            {
                if (!model[i].Value.ToString().Contains("|") || model[i].Value.ToString().Contains("2|"))
                {
                    where += " or ";
                }
                else
                {
                    where += " and ";
                }
            }
            else
            {
                if (!model[i].Value.ToString().Contains("|") || model[i].Value.ToString().Contains("2|"))
                {
                    where += " and ";
                }
                else
                {
                    where += " or ";
                }
            }

            if ((i + 1) < model.Count)
            {
                if (model[i + 1].Value.ToString().Contains("|") && !model[i].Value.ToString().Contains("|"))
                {
                    where += "(";
                }
                if (model[i + 1].Value.ToString().Contains("2|") && !model[i].Value.ToString().Contains("2|"))
                {
                    where += "(";
                }
            }
            return where;
        }
        static string endOR(IList<KeyValuePair<string, object>> model, int i)
        {
            string where = "";
            if ((i + 1) < model.Count)
            {
                if (!model[i + 1].Value.ToString().Contains("|") && model[i].Value.ToString().Contains("|"))
                {
                    where += ")";
                }
                if (!model[i + 1].Value.ToString().Contains("2|") && model[i].Value.ToString().Contains("2|"))
                {
                    where += ")";
                }
            }
            else
            {
                if (model[i].Value.ToString().Contains("|"))
                {
                    where += ")";
                }
                if (model[i].Value.ToString().Contains("2|"))
                {
                    where += ")";
                }
            }
            return where;
        }
        #endregion
    }
}
