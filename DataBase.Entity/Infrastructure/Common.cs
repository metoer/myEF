using Npgsql;
using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Reflection;

namespace DataBase.Entity.Infrastructure
{
    public static class Common
    {
        /// <summary>
        /// 判断数据是否连接成功
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="useName"></param>
        /// <param name="password"></param>
        /// <param name="dataBaseName"></param>
        /// <returns></returns>
        public static bool CheckDataBaseExist()
        {
            try
            {
                NpgsqlConnection con = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["PostgreSQL-1"].ToString());

                con.Open();

                con.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static object GetSqlValue(IStoredProcedure procedure, PropertyInfo info)
        {
            object obj = info.GetValue(procedure, new object[] { });

            if (obj == null || obj.ToString() == "null")
            {
                return DBNull.Value;
            }
            else
            {
                // 临时做法，解决录音高级查询问题
                if (procedure.GetType().Name == "proc_condition_calllog")
                {
                    if (info.Name == "s_calllingno_g" || info.Name == "s_callingname" || info.Name == "s_calledno" || info.Name == "s_calledname")
                    {
                        return obj;
                    }
                }

                if (obj.GetType() == typeof(string))
                {
                    return obj.ToString().Replace("'", "''");
                }
                return obj;
            }
        }
        /// <summary>
        /// 格式化参数
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static object FormatParameter(DbParameter param)
        {
            if (param == null || param.Value == null || string.IsNullOrEmpty(param.Value.ToString()))
                return "NULL";
            else
            {
                if (IsNeedComma(param.DbType))
                    return "'" + param.Value + "'";
                else
                    return param.Value;
            }
        }
        /// <summary>
        /// 参数是否需要单引号括起来
        /// </summary>
        /// <param name="datetype"></param>
        /// <returns></returns>
        public static bool IsNeedComma(DbType datatype)
        {
            switch (datatype)
            {
                case DbType.String:
                    return true;
                case DbType.DateTime:
                    return true;
                default:
                    return false;
            }
        }
    }
}
