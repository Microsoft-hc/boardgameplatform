using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using UnityEngine;
/*
 也可以使用using块来套用 Mysql操作类 mysql会自动关闭数据库
 using(var sql = new SqlHelper(填写对应参数))
 {
     //手动打开数据库
     sql.Open();
 }
*/

    /// <summary>
    /// Mysql添/删/改/查 操作类   切记使用该类方法 除了（Connect方法）其他方法 先调用Open() 使用完后 在Close()掉
    /// </summary>
    public class SqlHelper : IDisposable
    {
        string _server, _port, _user, _password, _datename, _format;
        string connectStr;
        MySqlConnection conn;
        /// <summary>
        /// 连接数据库构造方法
        /// </summary>
        /// <param name="host">IP地址</param>
        /// <param name="port">端口</param>
        /// <param name="user">用户名</param>
        /// <param name="passwd">密码</param>
        /// <param name="database">数据库名称</param>
        /// <param name="format">请填写字体</param>
        public SqlHelper(string format)
        {
            //Connect("1.13.174.102", "3306", "root", "123456", "user", format);
            Connect("192.168.133.1", "3306", "root", "ABCabc123", "user", format);
        }
        /// <summary>
        /// 连接数据库构造方法
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="user"></param>
        /// <param name="passwd"></param>
        /// <param name="database"></param>
        public SqlHelper()
        {
            //"127.0.0.1", "3306", "root", "root", "user"
            //Connect("1.13.174.102", "3306", "root", "123456", "user");
            Connect("192.168.133.1", "3306", "root", "ABCabc123", "user");
        }

        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <param name="host">IP地址</param>
        /// <param name="port">端口</param>
        /// <param name="user">用户名</param>
        /// <param name="passwd">密码</param>
        /// <param name="database">数据库名称</param>
        /// <param name="format">字体默认utf8</param>
        public void Connect(string host, string port, string user, string passwd, string database, string format = "utf8")
        {
            _server = host;
            _port = port; _user = user;
            _password = passwd;
            _datename = database;
            _format = format;
            connectStr = string.Format("server={0};port={1};user={2};password={3}; database={4};charset={5}", _server, _port, _user, _password, _datename, _format);//设置连接ip，端口，用户名，密码，以及编码格式    
            conn = new MySqlConnection(connectStr);//创建连接类
        }

        /// <summary>
        /// 打开数据库连接
        /// </summary>
        public void Open()
        {
            conn.Open();//正式打开连接 
        }
        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public void Close()
        {
            conn.Close();//关闭连接
        }

        /// <summary>
        /// using块回收资源时执行
        /// </summary>
        public void Dispose()
        {
            Close();
        }

        #region 查询语句
        /// <summary>
        /// 查询指定字段
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public DataSet Select(string tableName, string[] items)
        {
            string query = "SELECT " + items[0];
            for (int i = 1; i < items.Length; ++i)
            {
                query += ", " + items[i];
            }
            query += " FROM " + tableName;
            Debug.LogFormat("query: {0}", query);
            return ExecuteQuery(query);
        }
        /// <summary>
        /// 查询所有字段
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public DataSet Select(string tableName)
        {
            string query = "SELECT * FROM " + tableName;
            return ExecuteQuery(query);
        }

        #region 根据条件查询指定字段
        /// <summary>
        /// 查询指定字段数据中满足条件的
        /// DataSet内存中的数据库，DataSet是不依赖于数据库的独立数据集合,是一种不包含表头的纯数据文件
        /// 有条件的查询，查询在users这个表当中，只需要字段名为userAccount，userPwd，userName，ID这几个字段对应的数据，
        /// 满足条件为 userAccount对应的value=account， userPwd对应的value=md5Password；
        /// ("users", new string[] { "userAccount", "userPwd", "userName", "ID" }, new string[] { "userAccount", "userPwd" }, new string[] { "=", "=" }, new string[] { account, md5Password });
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="items">查询字段名</param>
        /// <param name="cols">判断字段名</param>
        /// <param name="operations">条件运算符</param>
        /// <param name="values">满足的条件值</param>
        /// <returns></returns>
        public DataSet SelectWhere(string tableName, string[] items, string[] cols, string[] operations, string[] values)
        {
            if (cols.Length != operations.Length || operations.Length != values.Length)
            {
                throw new Exception("col.Length != operation.Length != values.Length");
            }

            string query = "SELECT " + items[0];

            for (int i = 1; i < items.Length; ++i)
            {
                query += ", " + items[i];
            }

            query += " FROM " + tableName + " WHERE " + cols[0] + operations[0] + "'" + values[0] + "' ";

            for (int i = 1; i < cols.Length; ++i)
            {
                query += " AND " + cols[i] + operations[i] + "'" + values[i] + "' ";
            }
            Debug.LogFormat("query: {0}", query);
            return ExecuteQuery(query);
        }
        #endregion

        #region 根据条件查询全部字段
        /// <summary>
        /// 根据条件查询全部字段
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="cols">判断字段</param>
        /// <param name="operations">判断字符</param>
        /// <param name="values">条件成立数据</param>
        /// <returns></returns>
        public DataSet SelectWhere(string tableName, string[] cols, string[] operations, string[] values)
        {
            if (cols.Length != operations.Length || operations.Length != values.Length)
            {
                throw new Exception("col.Length != operation.Length != values.Length");
            }

            string query = "SELECT *";

            query += " FROM " + tableName + " WHERE " + cols[0] + operations[0] + "'" + values[0] + "' ";

            for (int i = 1; i < cols.Length; ++i)
            {
                query += " AND " + cols[i] + operations[i] + "'" + values[i] + "' ";
            }
            Debug.LogFormat("query: {0}", query);
            return ExecuteQuery(query);
        }
        #endregion

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public DataSet ExecuteQuery(string SQLString)
        {
            DataSet ds = new DataSet();
            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter(SQLString, conn);
                da.Fill(ds);
            }
            catch (MySqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return ds;
        }
        #endregion

        #region 更新语句
        /// <summary>
        /// 更新数据 param tableName=表名  selectkey=查找字段（主键) selectvalue=查找内容 cols=更新字段 colsvalues=更新内容
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="selectKeys">查找字段</param>
        /// <param name="selectValues">查找内容</param>
        /// <param name="cols">更新字段</param>
        /// <param name="colsValues">更新数据</param>
        /// <returns></returns>
        public int UpdateIntoSpecific(string tableName, string[] selectKeys, string[] selectValues, string[] cols, string[] colsValues)
        {
            string query = "UPDATE " + tableName + " SET " + cols[0] + " = " + "'" + colsValues[0] + "'";

            for (int i = 1; i < colsValues.Length; ++i)
            {
                query += ", " + cols[i] + " =" + "'" + colsValues[i] + "'";
            }

            query += " WHERE " + selectKeys[0] + " = " + "'" + selectValues[0] + "' ";
            for (int i = 1; i < selectKeys.Length; ++i)
            {
                query += " AND " + selectKeys[i] + " = " + "'" + selectValues[i] + "' ";
            }
            return ExecuteNonQuery(query);
        }
        /// <summary>
        /// 更新数据 param tableName=表名  selectkey=查找字段（主键) operation=判断的符号 selectvalue=查找内容 cols=更新字段 colsvalues=更新内容
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="selectKeys">查找字段</param>
        /// <param name="operation">判断符号</param>
        /// <param name="selectValues">查找内容</param>
        /// <param name="cols">更新字段</param>
        /// <param name="colsValues">更新数据</param>
        /// <returns></returns>
        public int UpdateIntoSpecific(string tableName, string[] selectKeys, string[] operation, string[] selectValues, string[] cols, string[] colsValues)
        {
            string query = "UPDATE " + tableName + " SET " + cols[0] + " = " + "'" + colsValues[0] + "'";

            for (int i = 1; i < colsValues.Length; ++i)
            {
                query += ", " + cols[i] + " =" + "'" + colsValues[i] + "'";
            }

            query += " WHERE " + selectKeys[0] + " " + operation[0] + " " + "'" + selectValues[0] + "' ";
            for (int i = 1; i < selectKeys.Length; ++i)
            {
                query += " AND " + selectKeys[i] + " " + operation[i] + " " + "'" + selectValues[i] + "' ";
            }
            return ExecuteNonQuery(query);
        }
        #endregion

        #region 插入语句
        /// <summary>
        /// 插入部分ID
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="cols">字段名</param>
        /// <param name="values">具体数值</param>
        /// <returns></returns>
        public int InsertInto(string tableName, string[] cols, string[] values)
        {
            if (cols.Length != values.Length)
            {
                throw new Exception("columns.Length != colType.Length");
            }

            string query = "INSERT INTO " + tableName + " (" + cols[0];
            for (int i = 1; i < cols.Length; ++i)
            {
                query += ", " + cols[i];
            }

            query += ") VALUES (" + "'" + values[0] + "'";
            for (int i = 1; i < values.Length; ++i)
            {
                query += ", " + "'" + values[i] + "'";
            }

            query += ")";
            Debug.LogFormat("query: {0}", query);
            return ExecuteNonQuery(query);
        }
        #endregion

        #region 删除语句

        #region 根据条件删除表
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="cols">字段</param>
        /// <param name="colsValues">字段值</param>
        /// <returns></returns>
        public int Delete(string tableName, string[] cols, string[] colsValues)
        {
            string query = "DELETE FROM " + tableName + " WHERE " + cols[0] + " = " + "'" + colsValues[0] + "'";

            for (int i = 1; i < colsValues.Length; ++i)
            {
                query += " and " + cols[i] + " = " + "'" + colsValues[i] + "'";
            }
            return ExecuteNonQuery(query);
        }
        /// <summary>
        /// 删除表
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="cols"></param>
        /// <param name="operation"></param>
        /// <param name="colsValues"></param>
        /// <returns></returns>
        public int Delete(string tableName, string[] cols, string[] operation, string[] colsValues)
        {
            string query = "DELETE FROM " + tableName + " WHERE " + cols[0] + " " + operation[0] + " " + "'" + colsValues[0] + "'";

            for (int i = 1; i < colsValues.Length; ++i)
            {
                query += " and " + cols[i] + " " + operation[i] + " " + "'" + colsValues[i] + "'";
            }
            return ExecuteNonQuery(query);
        }
        #endregion

        /// <summary> 
        /// 删除表中全部数据
        /// </summary>
        public int DeleteContents(string tableName)
        {
            string query = "DELETE FROM " + tableName;
            return ExecuteNonQuery(query);
        }
        #endregion

        #region 创建表
        /// <summary>
        /// 创建表 param name=表名 col=字段名 colType=字段类型
        /// </summary>
        public int CreateTable(string name, string[] col, string[] colType)
        {
            if (col.Length != colType.Length)
            {
                throw new Exception("columns.Length != colType.Length");
            }
            string query = "CREATE TABLE " + name + " (" + col[0] + " " + colType[0];
            for (int i = 1; i < col.Length; ++i)
            {
                query += ", " + col[i] + " " + colType[i];
            }
            query += ")";
            return ExecuteNonQuery(query);
        }
        #endregion

        /// <summary>
        /// 执行SQL语句，返回影响的记录数。用于Update、Insert和Delete
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteNonQuery(string SQLString)
        {

            using (MySqlCommand cmd = new MySqlCommand(SQLString, conn))
            {
                try
                {
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (MySqlException E)
                {
                    throw new Exception(E.Message);
                }
                finally
                {
                    cmd.Dispose();
                }
            }

        }


    }



/*
  DataSet类使用方法
  asp.NET中DataSet对象获取相应列值、行列数、列名、取出特定值这些操作的总结，具体代码如下：

  1.  DataSet.Table[0].Rows[ i ][ j ]
      其中i　代表第 i 行数, j 代表第 j 列数

  2.　DataSet.Table[0].Rows[ i ].ItemArray[ j ]
      其中i　代表第 i 行数, j 代表第 j 列数

  3.　DataSet.Tables[0].Columns.Count
      取得表的总列数

  4.　DataSet.Tables[0].Rows.Count
      取得表的总行数

  5.　DataSet.Tables[0].Columns[ i ].ToString()
      取得表的 i 列名(字段名)

  6.  ds.Tables.Count - 返回表的数量，一个 select 返回一个 table
*/


    /// <summary>
    /// mysql工具类
    /// </summary>
    public class MysqlTools
    {

        /// <summary>
        /// 获取表中数据 字典中第一个键是字段名  第二个是 对应字段值 返回字典数组
        /// </summary>
        /// <param name="ds">数据表类</param>
        /// <returns></returns>
        public static Dictionary<string, object>[] TableData(DataSet ds)
        {
            //判断是否没有数据
            if (ds.Tables.Count == 0)
            {
                //返回空
                return null;
            }
            //声明集合
            List<Dictionary<string, object>> tableList = new List<Dictionary<string, object>>();
            //遍历DataSet表条数
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                //遍历表该条字段数量
                for (int j = 0; j < ds.Tables[i].Rows.Count; j++)
                {
                    //根据该条数 并新建一块字典空间存储到该字典中
                    tableList.Add(new Dictionary<string, object>());
                    //存储该条数类对象
                    var temp = ds.Tables[i];
                    //获取该条所有字段值
                    var obj = temp.Rows[j].ItemArray;
                    //遍历所有字段数组
                    for (int k = 0; k < obj.Length; k++)
                    {
                        //获取该字段名称
                        string tableName = temp.Columns[k].ToString();
                        //根据字段名称 和对应字段值存储到字段中
                        tableList[j].Add(tableName, obj[k]);
                    }
                }
            }
            //返回字典数组
            return tableList.ToArray();
        }
        /// <summary>
        /// 获取单个数据
        /// </summary>
        /// <param name="ds">数据表类</param>
        /// <param name="Name">字段名称</param>
        /// <returns></returns>
        public static object GetValue(DataSet ds, string Name)
        {
            //判断是否没有数据
            if (ds.Tables.Count == 0)
            {
                //返回空
                return null;
            }

            //遍历DataSet表条数
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                //遍历表该条字段数量
                for (int j = 0; j < ds.Tables[i].Rows.Count; j++)
                {
                    //存储该条数类对象
                    var temp = ds.Tables[i];
                    //获取该条所有字段值
                    var obj = temp.Rows[j].ItemArray;
                    //遍历所有字段数组
                    for (int k = 0; k < obj.Length; k++)
                    {
                        //获取该字段名称
                        string tableName = temp.Columns[k].ToString();
                        //判断该字段名称是否等于Name
                        if (tableName == Name)
                        {
                            //返回数据
                            return obj[k];
                        }
                    }
                }
            }
            //返回null
            return null;
        }
        /// <summary>
        /// 获取多个数据
        /// </summary>
        /// <returns></returns>
        public static object[] GetValues(DataSet ds, string Name)
        {
            //判断是否没有数据
            if (ds.Tables.Count == 0)
            {
                //返回空
                return null;
            }
            //声明字段集合
            List<object> list = new List<object>();
            //遍历DataSet表条数
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                //遍历表该条字段数量
                for (int j = 0; j < ds.Tables[i].Rows.Count; j++)
                {
                    //存储该条数类对象
                    var temp = ds.Tables[i];
                    //获取该条所有字段值
                    var obj = temp.Rows[j].ItemArray;
                    //遍历所有字段数组
                    for (int k = 0; k < obj.Length; k++)
                    {
                        //获取该字段名称
                        string tableName = temp.Columns[k].ToString();
                        //判断该字段名称是否等于Name
                        if (tableName == Name)
                        {
                            //存储到集合中
                            list.Add(obj[k]);
                        }
                    }
                }
            }
            //返回数组
            return list.ToArray();
        }
    }


