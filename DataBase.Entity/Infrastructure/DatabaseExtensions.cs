using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System;
using System.Data;
using System.Data.Common;
using System.Text;

namespace DataBase.Entity.Infrastructure
{
    /// <summary>
    /// 执行存储过程扩展类
    /// </summary>
    public static class DatabaseExtensions
    {
        public static IEnumerable<TResult> ExecuteStoredProcedure<TResult>(this Database database, IProcTools procTool, string procCmd, IStoredProcedure procedure)
        {
            var parameters = procTool.CreateDbParametersFromProperties(procedure);

            var format = procTool.CreateProcCommand<TResult>(procCmd, parameters);

            return database.SqlQuery<TResult>(format, parameters.ToArray());
        }
    }

    public class DbFactory
    {
        private static DbFactory _instance = null;

        public static DbFactory Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DbFactory();
                return _instance;
            }
        }

        public IProcTools GetProcTools(DatabaseType dbType)
        {
            IProcTools procTool;
            switch (dbType)
            {
                case DatabaseType.MySQL:
                    procTool = new MySQLProcTools();
                    break;
                case DatabaseType.PostgreSQL:
                    procTool = new PostgreSQLProcTools();
                    break;
                case DatabaseType.SQLServer:
                    procTool = new SQLServerProcTools();
                    break;
                default:
                    procTool = new SQLServerProcTools();
                    break;
            }

            return procTool;
        }
    }

    public abstract class IProcTools
    {
        public virtual List<DbParameter> CreateDbParametersFromProperties(IStoredProcedure procedure)
        {
            var procedureType = procedure.GetType();
            var propertiesOfProcedure = procedureType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            List<DbParameter> parameters =
                propertiesOfProcedure.Select(propertyInfo => new SqlParameter(string.Format("@{0}", (object)propertyInfo.Name),
                                                Common.GetSqlValue(procedure, propertyInfo))).AsEnumerable<DbParameter>().ToList();
            return parameters;
        }
        public virtual string CreateProcCommand<TResult>(string procName, List<DbParameter> parameters)
        {
            return null;
        }
    }

    public class SQLServerProcTools : IProcTools
    {
        public override string CreateProcCommand<TResult>(string procName, List<DbParameter> parameters)
        {
            string queryString = string.Format("exec {0}", "dbo." + procName);
            string paramString = string.Join(",", parameters.Select(p => p.ParameterName));
            return queryString + " " + paramString;
        }
    }

    public class MySQLProcTools : IProcTools
    {
        public override string CreateProcCommand<TResult>(string procName, List<DbParameter> parameters)
        {
            string queryString = string.Format("call {0}(", procName);
            parameters.ForEach(x => queryString = string.Format("{0} {1},", queryString, Common.FormatParameter(x)));

            return string.Concat(queryString.TrimEnd(','), ");");
        }
    }


    public class PostgreSQLProcTools : IProcTools
    {
        public override List<DbParameter> CreateDbParametersFromProperties(IStoredProcedure procedure)
        {
            var procedureType = procedure.GetType();
            var propertiesOfProcedure = procedureType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            List<DbParameter> parameters =
                propertiesOfProcedure.Select(propertyInfo => new Npgsql.NpgsqlParameter(string.Format("@{0}", (object)propertyInfo.Name),
                                                Common.GetSqlValue(procedure, propertyInfo))).AsEnumerable<DbParameter>().ToList();

            return parameters;
        }

        public override string CreateProcCommand<TResult>(string procName, List<DbParameter> parameters)
        {
            string paramString = string.Join(",", parameters.Select(p => p.ParameterName));

            string queryString = string.Empty;

            // 存储过程的返回值是否是集合
            bool isRecordProc = typeof(TResult).IsClass && !typeof(TResult).IsPrimitive && !typeof(TResult).IsArray && typeof(TResult) != typeof(string);
            if (isRecordProc)
            {
                var propertiesOfReturn = typeof(TResult).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                StringBuilder sBuilder = new StringBuilder();
                foreach (var propertyInfo in propertiesOfReturn)
                {
                    sBuilder.AppendFormat("{0} {1},", propertyInfo.Name, GetPostgreSQLType(propertyInfo.PropertyType));
                }

                // 返回集合的字段名和类型字符串
                string returnFieldsString = sBuilder.Remove(sBuilder.Length - 1, 1).ToString();

                queryString = string.Format("SELECT * FROM dbo.{0}({1}) t ({2});", procName, paramString, returnFieldsString);
            }
            else
            {
                queryString = string.Format("SELECT * FROM dbo.{0}({1});", procName, paramString);
            }
            return queryString;
        }

        private string GetPostgreSQLType(Type type)
        {
            string t = string.Empty;
            if (type == typeof(int) || type == typeof(Nullable<int>))
                t = "integer";
            else if (type == typeof(string))
                t = "character varying";
            else if (type == typeof(DateTime) || type == typeof(Nullable<System.DateTime>))
                t = "timestamp without time zone";
            else
                t = "character varying";

            return t;
        }
    }
}
