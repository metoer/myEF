using DataBase.Entity.Infrastructure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataGenerator
{
    public class DataBaseTable
    {
        private AppDbContext _DataContext;
        private DatabaseType _dbType = DatabaseType.PostgreSQL;
        protected IProcTools _procTool;

        private static object dataLock = new object();

        private static DataBaseTable dataBaseTable;

        public static DataBaseTable DataBaseInstance
        {
            get
            {
                if (dataBaseTable == null)
                {
                    lock (dataLock)
                    {

                        if (dataBaseTable == null)
                        {
                            dataBaseTable = new DataBaseTable();
                        }
                    }
                }

                return dataBaseTable;
            }
        }

        private DataBaseTable()
        {
            try
            {
                _procTool = DbFactory.Instance.GetProcTools(_dbType);
                _DataContext = new AppDbContext(ConfigurationManager.AppSettings["Provider"]);
                if (_DataContext.Database.Exists())
                {
                    var objectContext = (_DataContext as IObjectContextAdapter).ObjectContext;
                    objectContext.CommandTimeout = 30000;
                }
                else
                {
                    _DataContext = null;
                }
            }
            catch (Exception e) { }
        }

        #region 采集站数据库表
        public void CreateDSMediaLog(DateTime time)
        {
            try
            {
                string sql = @"Create table dbo.DS_MediaLog{0}
                        ( 
                          CONSTRAINT DS_MediaLog_pkey_{1} PRIMARY KEY (record_id)
                         ) INHERITS (dbo.DS_MediaLog)";
                string name = time.ToString("yyyyMM");
                ExecuteNonQuery(string.Format(sql, name, name));
            }
            catch
            {
            }
        }

        public void CreateDSStationLog(DateTime time)
        {
            try
            {
                string sql = @"Create table dbo.ds_stationLog{0}
                        (                         
                          CONSTRAINT DS_StationLog_pkey_{1} PRIMARY KEY (log_id)
                         ) INHERITS (dbo.ds_stationLog)";
                string name = time.ToString("yyyyMM");
                ExecuteNonQuery(string.Format(sql, name, name));
            }
            catch
            {
            }
        }

        public void CreateDSCameralog(DateTime time)
        {
            try
            {
                string sql = @"Create table dbo.ds_cameralog{0}
                        (                         
                          CONSTRAINT DS_CameraLog_pkey_{1} PRIMARY KEY (log_id)
                         )  INHERITS (dbo.ds_cameralog)";
                string name = time.ToString("yyyyMM");
                ExecuteNonQuery(string.Format(sql, name, name));
            }
            catch (Exception e)
            {
            }

        }
        #endregion


        #region 后台数据库表
        public void CreateMCMediaLog(DateTime time)
        {
            try
            {
                string sql = @"Create table dbo.MC_MediaLog{0}
                        (                          
                          CONSTRAINT MC_MediaLog_pkey_{1} PRIMARY KEY (record_id)
                         ) INHERITS (dbo.MC_MediaLog)";
                string name = time.ToString("yyyyMM");
                ExecuteNonQuery(string.Format(sql, name, name));
            }
            catch (Exception e)
            {
            }
        }

        public void CreateMCStationLog(DateTime time)
        {
            try
            {
                string sql = @"Create table dbo.MC_StationLog{0}
                        ( 
                          CONSTRAINT MC_StationLog_pkey_{1} PRIMARY KEY (log_id)
                         ) INHERITS (dbo.MC_StationLog)";
                string name = time.ToString("yyyyMM");
                ExecuteNonQuery(string.Format(sql, name, name));
            }
            catch (Exception e)
            {
            }
        }

        public void CreateMCCameralog(DateTime time)
        {
            try
            {
                string sql = @"Create table dbo.MC_CameraLog{0}
                        (                          
                          CONSTRAINT MC_CameraLog_pkey_{1} PRIMARY KEY (log_id)
                         ) INHERITS (dbo.MC_CameraLog) ";
                string name = time.ToString("yyyyMM");
                ExecuteNonQuery(string.Format(sql, name, name));
            }
            catch (Exception e)
            {
            }
        }

        public void CreateMCManagementCenterLog(DateTime time)
        {
            try
            {
                string sql = @"Create table dbo.MC_ManagementCenterLog{0}
                        (                          
                          CONSTRAINT MC_ManagementCenterLog_pkey_{1} PRIMARY KEY (log_id)
                         ) INHERITS (dbo.MC_ManagementCenterLog)";
                string name = time.ToString("yyyyMM");
                ExecuteNonQuery(string.Format(sql, name, name));
            }
            catch (Exception e)
            {
            }
        }
        #endregion

        public void InsertDataByTable(string tableName, Type type, List<object> sources)
        {

            string sql = string.Format("insert into dbo.{0} (", tableName);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            string paraName = string.Empty;
            foreach (var item in properties)
            {
                paraName += "," + item.Name;
            }

            if (!string.IsNullOrEmpty(paraName))
            {
                paraName = paraName.Substring(1);
            }

            sql += paraName + ") Values ";

            string paraValues = string.Empty;
            foreach (var item in sources)
            {
                string paraValue = string.Empty;
                foreach (var pro in properties)
                {
                    object value = pro.GetValue(item);
                    paraValue += "," + "'" + pro.GetValue(item) + "'";

                }

                if (!string.IsNullOrEmpty(paraValue))
                {
                    paraValue = paraValue.Substring(1);
                }

                paraValues += ",(" + paraValue + ")";
            }

            if (!string.IsNullOrEmpty(paraValues))
            {
                paraValues = paraValues.Substring(1);
            }

            sql += paraValues;
            ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 执行非查询T-SQL命令，Insert、Update、Delete等
        /// </summary>
        /// <param name="sql">T-SQL语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>sql命令影响的行数</returns>
        public virtual int ExecuteNonQuery(string sql, params object[] parameters)
        {

            return _DataContext.Database.ExecuteSqlCommand(sql, parameters);
        }
    }
}
