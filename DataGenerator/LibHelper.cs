using DataBase.Entity.Infrastructure;
using DataBase.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace DataGenerator
{
    public delegate void Progess(decimal value, int index);
    public class LibHelper
    {

        public static void UpdateConfig(string ip, string port, string useName, string password, string dataBaseName)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.ConnectionStrings.ConnectionStrings.Remove("PostgreSQL-1");
            ConnectionStringSettings setting = new ConnectionStringSettings();
            setting.Name = "PostgreSQL-1";
            setting.ConnectionString = string.Format("Server={0};Port={1};Database={2};User Id={3};Password={4};", ip, port, dataBaseName, useName, password);
            setting.ProviderName = "Npgsql";
            config.ConnectionStrings.ConnectionStrings.Add(setting);
            config.Save();
            FieldInfo fieldInfo = typeof(ConfigurationManager).GetField("s_initState", BindingFlags.NonPublic | BindingFlags.Static);
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(null, 0);
            }
        }
        #region 获取用户信息
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        public static mc_user GetUserInfoByPoliceCodeMC(string userCode)
        {
            RepositoryBase<mc_user> repositoryBase = new RepositoryBase<mc_user>();

            if (repositoryBase.Model == null)
            {
                return null;
            }

            mc_user userInfo = repositoryBase.Get(p => p.user_code.Equals(userCode) && p.is_active.Equals("1"));

            return userInfo;
        }

        public static ds_user GetUserInfoByPoliceCode(string userCode)
        {
            RepositoryBase<ds_user> repositoryBase = new RepositoryBase<ds_user>();

            if (repositoryBase.Model == null)
            {
                return null;
            }

            ds_user userInfo = repositoryBase.Get(p => p.user_code.Equals(userCode) && p.is_active.Equals("1"));

            return userInfo;
        }
        #endregion

        #region 获取所有组织信息
        /// <summary>
        /// 获取所有组织信息
        /// </summary>
        /// <returns></returns>
        public static List<ds_organization> GetOrgInfos()
        {
            RepositoryBase<ds_organization> repositoryBase = new RepositoryBase<ds_organization>();

            if (repositoryBase.Model == null)
            {
                return null;
            }

            List<ds_organization> orgInfos = repositoryBase.GetAll();

            return orgInfos;
        }

        public static List<mc_organization> GetOrgInfosMC()
        {
            RepositoryBase<mc_organization> repositoryBase = new RepositoryBase<mc_organization>();

            if (repositoryBase.Model == null)
            {
                return null;
            }

            List<mc_organization> orgInfos = repositoryBase.GetAll();

            return orgInfos;
        }
        #endregion

        #region 获取所有执法记录仪信息
        /// <summary>
        /// 获取所有执法记录仪信息
        /// </summary>
        /// <returns></returns>
        public static List<ds_device> GetDeviceInfos(string deviceCode)
        {
            RepositoryBase<ds_device> repositoryBase = new RepositoryBase<ds_device>();

            if (repositoryBase.Model == null)
            {
                return null;
            }
            List<ds_device> deviceInfos = new List<ds_device>();
            if (string.IsNullOrEmpty(deviceCode))
            {
                deviceInfos = repositoryBase.GetAll();
            }
            else
            {
                ds_device deviceInfo = repositoryBase.Get(p => p.device_code.Equals(deviceCode));
                if (deviceInfo != null)
                {
                    deviceInfos.Add(deviceInfo);
                }
            }

            return deviceInfos;
        }

        public static List<mc_device> GetDeviceInfosMC(string deviceCode)
        {
            RepositoryBase<mc_device> repositoryBase = new RepositoryBase<mc_device>();

            if (repositoryBase.Model == null)
            {
                return null;
            }
            List<mc_device> deviceInfos = new List<mc_device>();
            if (string.IsNullOrEmpty(deviceCode))
            {
                deviceInfos = repositoryBase.GetAll();
            }
            else
            {
                mc_device deviceInfo = repositoryBase.Get(p => p.device_code.Equals(deviceCode));
                if (deviceInfo != null)
                {
                    deviceInfos.Add(deviceInfo);
                }
            }
            return deviceInfos;
        }
        #endregion

        #region 执法记录仪操作记录
        public static int AddCameraLog(DateTime startTime,
                                         DateTime endTime,
                                         int dayNum,
                                         string stationCode,
                                         string opTypes,
                                         ds_user userInfo,
                                         List<ds_organization> orgList,
                                         ds_organization userOrgInfo,
                                        List<ds_device> deviceInfos,
                                         int index,
                                         int OneNum,
                                         int OneSleep,
                                        Progess progess
                                        )
        {
            int count = 0;
            if (File.Exists(opTypes))
            {
                opTypes = File.ReadAllText(opTypes);
            }

            string[] types = opTypes.Split(',');

            DataBaseTable dataBaseTable = DataBaseTable.DataBaseInstance;
            DateTime tableTime = startTime;

            // 创建月份表
            while (tableTime < endTime)
            {
                dataBaseTable.CreateDSCameralog(tableTime);
                tableTime = tableTime.AddMonths(1);
            }
            dataBaseTable.CreateDSCameralog(endTime);

            Random rand = new Random();

            int days = (endTime - startTime).Days + 1;

            int spaceValue = 24 * 60 * 60 / dayNum;

            long allCount = dayNum * days;

            List<object> datas = new List<object>();

            for (int i = 0; i < days; i++)
            {
                ds_device currentDeviceInfo = deviceInfos[rand.Next(0, deviceInfos.Count)];

                ds_organization currentDeviceOrgInfo = orgList.Find(p => p.org_id.Equals(currentDeviceInfo.org_id));

                for (int j = 0; j < dayNum; j++)
                {
                    DateTime time = startTime.AddSeconds(spaceValue * j);

                    count++;

                    // 每创建100条换个被操作对象
                    if (count % 100 == 0)
                    {
                        currentDeviceInfo = deviceInfos[rand.Next(0, deviceInfos.Count)];
                        currentDeviceOrgInfo = orgList.Find(p => p.org_id.Equals(currentDeviceInfo.org_id));
                    }

                    if (OneNum > 0 && OneSleep > 0 && count % OneNum == 0)
                    {
                        if (datas.Count > 0)
                        {
                            dataBaseTable.InsertDataByTable("ds_cameralog" + startTime.ToString("yyyyMM"), typeof(ds_cameralog), datas);
                            datas.Clear();
                        }
                        Thread.Sleep(OneSleep);
                    }

                    ds_cameralog cameralog = new ds_cameralog()
                    {
                        log_id = time.ToString("yyyyMMddHHmmss") + stationCode + Guid.NewGuid().ToString("N"),
                        device_id = currentDeviceInfo.device_id,
                        operator_guid = userInfo.guid,
                        operator_id = userInfo.user_id,
                        operator_name = userInfo.user_name,
                        operator_code = userInfo.user_code,
                        operator_org_id = userInfo.org_id,
                        operator_org_id_code = userOrgInfo.org_id_code,
                        operator_org_name = userOrgInfo.org_name,
                        op_time = time.ToString("yyyy-MM-dd hh:mm:ss"),
                        op_type = types[rand.Next(0, types.Length)],
                        operated_guid = string.Empty,
                        operated_id = currentDeviceInfo.device_id,
                        operated_name = (currentDeviceInfo.device_alias ?? string.Empty).ToString(),
                        operated_code = currentDeviceInfo.device_code,
                        operated_org_id = currentDeviceInfo.org_id,
                        operated_org_id_code = currentDeviceOrgInfo.org_id_code,
                        operated_org_name = currentDeviceOrgInfo.org_name,
                        upload_process = 0,
                        collect_time = time.ToString("yyyy-MM-dd hh:mm:ss"),
                        station_code = stationCode
                    };

                    datas.Add(cameralog);

                    progess(Math.Round((decimal)count / (decimal)allCount, 4), index);
                    Thread.Sleep(1);
                }

                if (datas.Count > 0)
                {
                    dataBaseTable.InsertDataByTable("ds_cameralog" + startTime.ToString("yyyyMM"), typeof(ds_cameralog), datas);
                    datas.Clear();
                }

                startTime = startTime.AddDays(1);
            }
            return count;
        }

        public static int AddCameraLogByMc(DateTime startTime,
                                      DateTime endTime,
                                      int dayNum,
                                      string stationCode,
                                      string opTypes,
                                      mc_user userInfo,
                                      List<mc_organization> orgList,
                                      mc_organization userOrgInfo,
                                     List<mc_device> deviceInfos,
                                      int index,
                                      int OneNum,
                                      int OneSleep,
                                     Progess progess
                                     )
        {
            int count = 0;
            if (File.Exists(opTypes))
            {
                opTypes = File.ReadAllText(opTypes);
            }

            DataBaseTable dataBaseTable = DataBaseTable.DataBaseInstance;
            DateTime tableTime = startTime;

            // 创建月份表
            while (tableTime < endTime)
            {
                dataBaseTable.CreateMCCameralog(tableTime);
                tableTime = tableTime.AddMonths(1);
            }
            dataBaseTable.CreateMCCameralog(endTime);

            string[] types = opTypes.Split(',');

            Random rand = new Random();

            int days = (endTime - startTime).Days + 1;

            int spaceValue = 24 * 60 * 60 / dayNum;

            long allCount = dayNum * days;
            List<object> datas = new List<object>();
            for (int i = 0; i < days; i++)
            {

                mc_device currentDeviceInfo = deviceInfos[rand.Next(0, deviceInfos.Count)];

                mc_organization currentDeviceOrgInfo = orgList.Find(p => p.org_id.Equals(currentDeviceInfo.org_id));

                for (int j = 0; j < dayNum; j++)
                {
                    DateTime time = startTime.AddSeconds(spaceValue * j);

                    count++;

                    // 每创建100条换个被操作对象
                    if (count % 100 == 0)
                    {
                        currentDeviceInfo = deviceInfos[rand.Next(0, deviceInfos.Count)];
                        currentDeviceOrgInfo = orgList.Find(p => p.org_id.Equals(currentDeviceInfo.org_id));
                    }

                    if (OneNum > 0 && OneSleep > 0 && count % OneNum == 0)
                    {
                        if (datas.Count > 0)
                        {
                            dataBaseTable.InsertDataByTable("mc_cameralog" + startTime.ToString("yyyyMM"), typeof(mc_cameralog), datas);
                            datas.Clear();
                        }
                        Thread.Sleep(OneSleep);
                    }

                    mc_cameralog cameralog = new mc_cameralog()
                    {
                        log_id = time.ToString("yyyyMMddHHmmss") + stationCode + Guid.NewGuid().ToString("N"),
                        device_id = currentDeviceInfo.device_id,
                        operator_guid = userInfo.guid,
                        operator_id = userInfo.user_id,
                        operator_name = userInfo.user_name,
                        operator_code = userInfo.user_code,
                        operator_org_id = userInfo.org_id,
                        operator_org_id_code = userOrgInfo.org_id_code,
                        operator_org_name = userOrgInfo.org_name,
                        op_time = time.ToString("yyyy-MM-dd hh:mm:ss"),
                        op_type = types[rand.Next(0, types.Length)],
                        operated_guid = string.Empty,
                        operated_id = currentDeviceInfo.device_id,
                        operated_name = (currentDeviceInfo.device_alias ?? string.Empty).ToString(),
                        operated_code = currentDeviceInfo.device_code,
                        operated_org_id = currentDeviceInfo.org_id,
                        operated_org_id_code = currentDeviceOrgInfo.org_id_code,
                        operated_org_name = currentDeviceOrgInfo.org_name,
                        collect_time = time.ToString("yyyy-MM-dd hh:mm:ss"),
                        station_code = stationCode
                    };

                    datas.Add(cameralog);

                    progess(Math.Round((decimal)count / (decimal)allCount, 4), index);
                    Thread.Sleep(1);
                }

                if (datas.Count > 0)
                {
                    dataBaseTable.InsertDataByTable("mc_cameralog" + startTime.ToString("yyyyMM"), typeof(mc_cameralog), datas);
                    datas.Clear();
                }

                startTime = startTime.AddDays(1);
            }

            return count;
        }
        #endregion

        #region 采集站操作记录
        public static int AddStationLog(DateTime startTime,
                                      DateTime endTime,
                                      int dayNum,
                                      string stationCode,
                                      string opTypes,
                                      ds_user userInfo,
                                      ds_organization userOrgInfo,
                                      int index,
                                      int OneNum,
                                      int OneSleep,
                                      Progess progess
                                     )
        {
            int count = 0;
            if (File.Exists(opTypes))
            {
                opTypes = File.ReadAllText(opTypes);
            }

            string[] types = opTypes.Split(',');

            DataBaseTable dataBaseTable = DataBaseTable.DataBaseInstance;
            DateTime tableTime = startTime;

            // 创建月份表
            while (tableTime < endTime)
            {
                dataBaseTable.CreateDSStationLog(tableTime);
                tableTime = tableTime.AddMonths(1);
            }
            dataBaseTable.CreateDSStationLog(endTime);

            Random rand = new Random();

            int days = (endTime - startTime).Days + 1;

            int spaceValue = 24 * 60 * 60 / dayNum;

            long allCount = dayNum * days;
            List<object> datas = new List<object>();
            for (int i = 0; i < days; i++)
            {
                for (int j = 0; j < dayNum; j++)
                {
                    DateTime time = startTime.AddSeconds(spaceValue * j);

                    count++;

                    if (OneNum > 0 && OneSleep > 0 && count % OneNum == 0)
                    {
                        if (datas.Count > 0)
                        {
                            dataBaseTable.InsertDataByTable("ds_stationlog" + startTime.ToString("yyyyMM"), typeof(ds_stationlog), datas);
                            datas.Clear();
                        }

                        Thread.Sleep(OneSleep);
                    }

                    ds_stationlog stationlog = new ds_stationlog()
                    {
                        log_id = time.ToString("yyyyMMddHHmmss") + stationCode + Guid.NewGuid().ToString("N"),
                        station_code = stationCode,
                        station_ip = "127.0.0.1",
                        operator_guid = userInfo.guid,
                        operator_id = userInfo.user_id,
                        operator_name = userInfo.user_name,
                        operator_code = userInfo.user_code,
                        operator_org_id = userInfo.org_id,
                        operator_org_id_code = userOrgInfo.org_id_code,
                        operator_org_name = userOrgInfo.org_name,
                        op_time = time.ToString("yyyy-MM-dd hh:mm:ss"),
                        op_type = types[rand.Next(0, types.Length)],
                        op_description = string.Empty,
                        operated_guid = string.Empty,
                        operated_id = string.Empty,
                        operated_name = string.Empty,
                        operated_code = string.Empty,
                        operated_org_id = string.Empty,
                        operated_org_id_code = string.Empty,
                        operated_org_name = string.Empty,
                        upload_process = 0,
                        object_id = string.Empty,
                    };

                    datas.Add(stationlog);

                    progess(Math.Round((decimal)count / (decimal)allCount, 4), index);
                    Thread.Sleep(1);
                }

                if (datas.Count > 0)
                {
                    dataBaseTable.InsertDataByTable("ds_stationlog" + startTime.ToString("yyyyMM"), typeof(ds_stationlog), datas);
                    datas.Clear();
                }

                startTime = startTime.AddDays(1);
            }
            return count;
        }

        public static int AddStationLogByMC(DateTime startTime,
                                   DateTime endTime,
                                   int dayNum,
                                   string stationCode,
                                   string opTypes,
                                   mc_user userInfo,
                                   mc_organization userOrgInfo,
                                   int index,
                                   int OneNum,
                                    int OneSleep,
                                   Progess progess
                                  )
        {
            int count = 0;
            if (File.Exists(opTypes))
            {
                opTypes = File.ReadAllText(opTypes);
            }

            string[] types = opTypes.Split(',');

            DataBaseTable dataBaseTable = DataBaseTable.DataBaseInstance;
            DateTime tableTime = startTime;

            // 创建月份表
            while (tableTime < endTime)
            {
                dataBaseTable.CreateMCStationLog(tableTime);
                tableTime = tableTime.AddMonths(1);
            }
            dataBaseTable.CreateMCStationLog(endTime);

            Random rand = new Random();

            int days = (endTime - startTime).Days + 1;

            int spaceValue = 24 * 60 * 60 / dayNum;

            long allCount = dayNum * days;
            List<object> datas = new List<object>();
            for (int i = 0; i < days; i++)
            {

                for (int j = 0; j < dayNum; j++)
                {
                    DateTime time = startTime.AddSeconds(spaceValue * j);

                    count++;

                    if (OneNum > 0 && OneSleep > 0 && count % OneNum == 0)
                    {
                        if (datas.Count > 0)
                        {
                            dataBaseTable.InsertDataByTable("mc_stationlog" + startTime.ToString("yyyyMM"), typeof(mc_stationlog), datas);
                            datas.Clear();
                        }

                        Thread.Sleep(OneSleep);
                    }

                    mc_stationlog stationlog = new mc_stationlog()
                    {
                        log_id = time.ToString("yyyyMMddHHmmss") + stationCode + Guid.NewGuid().ToString("N"),
                        station_code = stationCode,
                        station_ip = "127.0.0.1",
                        operator_guid = userInfo.guid,
                        operator_id = userInfo.user_id,
                        operator_name = userInfo.user_name,
                        operator_code = userInfo.user_code,
                        operator_org_id = userInfo.org_id,
                        operator_org_id_code = userOrgInfo.org_id_code,
                        operator_org_name = userOrgInfo.org_name,
                        op_time = time.ToString("yyyy-MM-dd hh:mm:ss"),
                        op_type = types[rand.Next(0, types.Length)],
                        op_description = string.Empty,
                        operated_guid = string.Empty,
                        operated_id = string.Empty,
                        operated_name = string.Empty,
                        operated_code = string.Empty,
                        operated_org_id = string.Empty,
                        operated_org_id_code = string.Empty,
                        operated_org_name = string.Empty,
                        object_id = string.Empty,
                    };

                    datas.Add(stationlog);

                    progess(Math.Round((decimal)count / (decimal)allCount, 4), index);
                    Thread.Sleep(1);
                }

                if (datas.Count > 0)
                {
                    dataBaseTable.InsertDataByTable("mc_stationlog" + startTime.ToString("yyyyMM"), typeof(mc_stationlog), datas);
                    datas.Clear();
                }

                startTime = startTime.AddDays(1);
            }

            return count;
        }
        #endregion

        #region 媒体记录
        public static int AddMediaLog(DateTime startTime,
                                   DateTime endTime,
                                   int dayNum,
                                   string stationCode,
                                   ds_user userInfo,
                                   ds_organization userOrgInfo,
                                   List<ds_device> deviceInfos,
                                   List<ds_organization> orgList,
                                   string vedioFile,
                                   string audioFile,
                                   string imageFile,
                                   string collectDir,
                                   int index,
             int OneNum,
                                    int OneSleep,
                                   Progess progess
                                  )
        {
            int count = 0;

            DataBaseTable dataBaseTable = DataBaseTable.DataBaseInstance;
            DateTime tableTime = startTime;

            // 创建月份表
            while (tableTime < endTime)
            {
                dataBaseTable.CreateDSMediaLog(tableTime);
                tableTime = tableTime.AddMonths(1);
            }
            dataBaseTable.CreateDSMediaLog(endTime);

            List<int> fileTypes = new List<int>();
            long vedioLength = 0;
            if (File.Exists(vedioFile))
            {
                fileTypes.Add(1);
                vedioLength = new FileInfo(vedioFile).Length;
            }
            long audioLength = 0;
            if (File.Exists(audioFile))
            {
                fileTypes.Add(2);
                audioLength = new FileInfo(audioFile).Length;
            }
            long imageLength = 0;
            if (File.Exists(imageFile))
            {
                fileTypes.Add(3);
                imageLength = new FileInfo(imageFile).Length;
            }

            Random rand = new Random();

            int days = (endTime - startTime).Days + 1;

            int spaceValue = 24 * 60 * 60 / dayNum;

            List<object> datas = new List<object>();

            long allCount = dayNum * days;
            string lastTable = string.Empty;

            string thumString = GetBase64StringForImage();
            for (int i = 0; i < days; i++)
            {
                ds_device currentDeviceInfo = deviceInfos[rand.Next(0, deviceInfos.Count)];

                ds_organization currentDeviceOrgInfo = orgList.Find(p => p.org_id.Equals(currentDeviceInfo.org_id));

                string path = collectDir + (collectDir.EndsWith("\\") ? string.Empty : "\\") + currentDeviceInfo.device_code + "\\";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                for (int j = 0; j < dayNum; j++)
                {
                    DateTime time = startTime.AddSeconds(spaceValue * j);

                    count++;

                    if (OneNum > 0 && OneSleep > 0 && count % OneNum == 0)
                    {
                        if (datas.Count > 0)
                        {
                            dataBaseTable.InsertDataByTable("ds_medialog" + startTime.ToString("yyyyMM"), typeof(ds_medialog), datas);
                            datas.Clear();
                        }
                        Thread.Sleep(OneSleep);
                    }

                    if (count % 100 == 0)
                    {
                        currentDeviceInfo = deviceInfos[rand.Next(0, deviceInfos.Count)];
                        currentDeviceOrgInfo = orgList.Find(p => p.org_id.Equals(currentDeviceInfo.org_id));
                        path = collectDir + (collectDir.EndsWith("\\") ? string.Empty : "\\") + currentDeviceInfo.device_code + "\\";
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                    }

                    int fileTypeValue = fileTypes[rand.Next(0, fileTypes.Count)];
                    long currentFileLength = 0;
                    string sourceFile = string.Empty;
                    string fileName = string.Empty;
                    switch (fileTypeValue)
                    {
                        case 1:
                            fileName = path + time.ToString("yyyyMMddHHmmss") + Guid.NewGuid().ToString("N") + Path.GetExtension(vedioFile);
                            sourceFile = vedioFile;
                            currentFileLength = vedioLength;
                            break;
                        case 2:
                            fileName = path + time.ToString("yyyyMMddHHmmss") + Guid.NewGuid().ToString("N") + Path.GetExtension(audioFile);
                            sourceFile = audioFile;
                            currentFileLength = audioLength;
                            break;
                        default:
                            fileName = path + time.ToString("yyyyMMddHHmmss") + Guid.NewGuid().ToString("N") + Path.GetExtension(imageFile);
                            sourceFile = imageFile;
                            currentFileLength = imageLength;
                            break;
                    }
                    FileCopy.Add(sourceFile, fileName);
                    ds_medialog medialog = new ds_medialog()
                    {
                        record_id = time.ToString("yyyyMMddHHmmss") + stationCode + Guid.NewGuid().ToString("N"),
                        user_guid = userInfo.guid,
                        user_id = userInfo.user_id,
                        user_name = userInfo.user_name,
                        user_code = userInfo.user_code,
                        org_id = userOrgInfo.org_id,
                        org_id_code = userOrgInfo.org_id_code,
                        org_name = userOrgInfo.org_name,
                        device_id = currentDeviceInfo.device_id,
                        device_code = currentDeviceInfo.device_code,
                        record_time = time.ToString("yyyy-MM-dd hh:mm:ss"),
                        collect_time = time.ToString("yyyy-MM-dd hh:mm:ss"),
                        collect_process = 100,
                        collect_ip = "127.0.0.1",
                        media_type = fileTypeValue,
                        file_code = string.Empty,
                        file_path = path,
                        file_name = Path.GetFileName(fileName),
                        file_size = currentFileLength,
                        duration = 1800,
                        resolution_width = 800,
                        resolution_height = 600,
                        encrypt_type = 0,
                        camera_imp = 0,
                        user_imp = 0,
                        station_code = stationCode,
                        upload_process = 0,
                        file_upload_process = 0,
                        thumbnail = thumString
                    };
                    datas.Add(medialog);

                    progess(Math.Round((decimal)count / (decimal)allCount, 4), index);
                    Thread.Sleep(1);
                }

                if (datas.Count > 0)
                {
                    dataBaseTable.InsertDataByTable("ds_medialog" + startTime.ToString("yyyyMM"), typeof(ds_medialog), datas);
                    datas.Clear();
                }
                startTime = startTime.AddDays(1);
            }

            if (datas.Count > 0)
            {
                dataBaseTable.InsertDataByTable("ds_medialog" + startTime.ToString("yyyyMM"), typeof(ds_medialog), datas);
                datas.Clear();
            }

            return count;
        }

        public static int AddMediaLogMC(DateTime startTime,
                                  DateTime endTime,
                                  int dayNum,
                                  string stationCode,
                                  mc_user userInfo,
                                  mc_organization userOrgInfo,
                                  List<mc_device> deviceInfos,
                                  List<mc_organization> orgList,
                                  string vedioFile,
                                  string audioFile,
                                  string imageFile,
                                  string collectDir,
                                  int index,
                                  int OneNum,
                                  int OneSleep,
                                  Progess progess
                                 )
        {
            int count = 0;

            DataBaseTable dataBaseTable = DataBaseTable.DataBaseInstance;
            DateTime tableTime = startTime;

            // 创建月份表
            while (tableTime < endTime)
            {
                dataBaseTable.CreateMCMediaLog(tableTime);
                tableTime = tableTime.AddMonths(1);
            }
            dataBaseTable.CreateMCMediaLog(endTime);

            List<int> fileTypes = new List<int>();
            long vedioLength = 0;
            if (File.Exists(vedioFile))
            {
                fileTypes.Add(1);
                vedioLength = new FileInfo(vedioFile).Length;
            }
            long audioLength = 0;
            if (File.Exists(audioFile))
            {
                fileTypes.Add(2);
                audioLength = new FileInfo(audioFile).Length;
            }
            long imageLength = 0;
            if (File.Exists(imageFile))
            {
                fileTypes.Add(3);
                imageLength = new FileInfo(imageFile).Length;
            }

            Random rand = new Random();

            int days = (endTime - startTime).Days + 1;

            int spaceValue = 24 * 60 * 60 / dayNum;

            List<object> datas = new List<object>();

            long allCount = dayNum * days;

            string thumString = GetBase64StringForImage();
            for (int i = 0; i < days; i++)
            {
                mc_device currentDeviceInfo = deviceInfos[rand.Next(0, deviceInfos.Count)];

                mc_organization currentDeviceOrgInfo = orgList.Find(p => p.org_id.Equals(currentDeviceInfo.org_id));

                string path = collectDir + (collectDir.EndsWith("\\") ? string.Empty : "\\") + currentDeviceInfo.device_code + "\\";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                for (int j = 0; j < dayNum; j++)
                {
                    DateTime time = startTime.AddSeconds(spaceValue * j);

                    count++;

                    if (count % 100 == 0)
                    {
                        currentDeviceInfo = deviceInfos[rand.Next(0, deviceInfos.Count)];
                        currentDeviceOrgInfo = orgList.Find(p => p.org_id.Equals(currentDeviceInfo.org_id));
                        path = collectDir + (collectDir.EndsWith("\\") ? string.Empty : "\\") + currentDeviceInfo.device_code + "\\";
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                    }

                    if (OneNum > 0 && OneSleep > 0 && count % OneNum == 0)
                    {
                        if (datas.Count > 0)
                        {
                            dataBaseTable.InsertDataByTable("mc_medialog" + startTime.ToString("yyyyMM"), typeof(mc_medialog), datas);
                            datas.Clear();
                        }
                        Thread.Sleep(OneSleep);
                    }

                    int fileTypeValue = fileTypes[rand.Next(0, fileTypes.Count)];
                    long currentFileLength = 0;
                    string fileName = string.Empty;
                    string sourceFile = string.Empty;
                    switch (fileTypeValue)
                    {
                        case 1:
                            fileName = path + time.ToString("yyyyMMddHHmmss") + Guid.NewGuid().ToString("N") + Path.GetExtension(vedioFile);
                            sourceFile = vedioFile;
                            currentFileLength = vedioLength;
                            break;
                        case 2:
                            fileName = path + time.ToString("yyyyMMddHHmmss") + Guid.NewGuid().ToString("N") + Path.GetExtension(audioFile);
                            sourceFile = audioFile;
                            currentFileLength = audioLength;
                            break;
                        default:
                            fileName = path + time.ToString("yyyyMMddHHmmss") + Guid.NewGuid().ToString("N") + Path.GetExtension(imageFile);
                            sourceFile = imageFile;
                            currentFileLength = imageLength;
                            break;
                    }

                    FileCopy.Add(sourceFile, fileName);

                    mc_medialog medialog = new mc_medialog()
                    {
                        record_id = time.ToString("yyyyMMddHHmmss") + stationCode + Guid.NewGuid().ToString("N"),
                        user_guid = userInfo.guid,
                        user_id = userInfo.user_id,
                        user_name = userInfo.user_name,
                        user_code = userInfo.user_code,
                        org_id = userOrgInfo.org_id,
                        org_id_code = userOrgInfo.org_id_code,
                        org_name = userOrgInfo.org_name,
                        device_id = currentDeviceInfo.device_id,
                        device_code = currentDeviceInfo.device_code,
                        record_time = time.ToString("yyyy-MM-dd hh:mm:ss"),
                        collect_time = time.ToString("yyyy-MM-dd hh:mm:ss"),
                        collect_process = 100,
                        collect_ip = "127.0.0.1",
                        media_type = fileTypeValue,
                        file_code = string.Empty,
                        file_path = path,
                        file_name = Path.GetFileName(fileName),
                        file_size = currentFileLength,
                        duration = 1800,
                        resolution_width = 800,
                        resolution_height = 600,
                        encrypt_type = 0,
                        camera_imp = 0,
                        user_imp = 0,
                        station_code = stationCode,
                        upload_process = 0,
                        thumbnail = thumString
                    };

                    datas.Add(medialog);

                    progess(Math.Round((decimal)count / (decimal)allCount, 4), index);
                    Thread.Sleep(1);
                }

                if (datas.Count > 0)
                {
                    dataBaseTable.InsertDataByTable("mc_medialog" + startTime.ToString("yyyyMM"), typeof(mc_medialog), datas);
                    datas.Clear();
                }
                startTime = startTime.AddDays(1);
            }
            return count;
        }
        #endregion

        #region 告警信息添加
        public static int AddAlarmlog(DateTime startTime,
                                 DateTime endTime,
                                 int dayNum,
                                 string moduleFile,
                                 int index,
             int OneNum,
                                    int OneSleep,
                                 Progess progess
                                )
        {
            int count = 0;

            List<AlarmInfo> alarmInfoList = GetAlarmList(moduleFile);

            Random rand = new Random();

            int days = (endTime - startTime).Days + 1;

            int spaceValue = 24 * 60 * 60 / dayNum;

            RepositoryBase<mc_alarmlog> repositoryBase = new RepositoryBase<mc_alarmlog>();

            long allCount = dayNum * days;
            for (int i = 0; i < days; i++)
            {
                for (int j = 0; j < dayNum; j++)
                {
                    DateTime time = startTime.AddSeconds(spaceValue * j);

                    count++;

                    if (OneNum > 0 && OneSleep > 0 && count % OneNum == 0)
                    {
                        Thread.Sleep(OneSleep);
                    }

                    AlarmInfo alarmInfo = alarmInfoList[rand.Next(0, alarmInfoList.Count)];

                    mc_alarmlog medialog = new mc_alarmlog()
                    {
                        alarm_id = time.ToString("yyyyMMddHHmmss") + Guid.NewGuid().ToString("N"),
                        alarm_code = alarmInfo.AlarmCode,
                        alarm_level = alarmInfo.AlarmLevel,
                        module_id = alarmInfo.ModuleID,
                        module_name = alarmInfo.ModuleName,
                        alarm_status = rand.Next(1, 4).ToString(),
                        alarm_message = alarmInfo.AlarmMark,
                        alarm_time = time.ToString("yyyy-MM-dd hh:mm:ss"),
                        device_ip = "127.0.0.1",
                        last_alarm_time = time.ToString("yyyy-MM-dd hh:mm:ss")
                    };

                    repositoryBase.Add(medialog);
                    if (count % 1000 == 0)
                    {
                        repositoryBase.Commit();
                    }

                    progess(Math.Round((decimal)count / (decimal)allCount, 4), index);
                    Thread.Sleep(1);
                }
                startTime = startTime.AddDays(1);
            }

            repositoryBase.Commit();
            return count;
        }

        private static List<AlarmInfo> GetAlarmList(string fileName)
        {
            List<AlarmInfo> alarmList = new List<AlarmInfo>();
            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.Load(fileName);
            XmlNode node = xmlDoc.SelectSingleNode("Alarmlog");
            foreach (XmlElement item in node.ChildNodes)
            {
                AlarmInfo alarmInfo = new AlarmInfo()
                {
                    ModuleID = item.GetAttribute("ID").ToString(),
                    ModuleAlias = item.GetAttribute("moduleAlias").ToString(),
                    ModuleName = item.GetAttribute("moduleName").ToString(),
                    AlarmCode = item.GetAttribute("alarmCode").ToString(),
                    AlarmLevel = item.GetAttribute("alarmLevel").ToString(),
                    AlarmMark = item.GetAttribute("alarmMark").ToString()
                };
                alarmList.Add(alarmInfo);
            }


            return alarmList;
        }
        #endregion

        #region 操作日志中心
        public static int AddManageCenterlog(DateTime startTime,
                                 DateTime endTime,
                                 int dayNum,
                                 string stationCode,
                                 mc_user userInfo,
                                 mc_organization userOrgInfo,
                                 string manageFile,
                                 int index,
             int OneNum,
                                    int OneSleep,
                                 Progess progess
                                )
        {
            int count = 0;

            DataBaseTable dataBaseTable = DataBaseTable.DataBaseInstance;
            DateTime tableTime = startTime;

            // 创建月份表
            while (tableTime < endTime)
            {
                dataBaseTable.CreateMCManagementCenterLog(tableTime);
                tableTime = tableTime.AddMonths(1);
            }
            dataBaseTable.CreateMCManagementCenterLog(endTime);

            Dictionary<string, string> manageList = GetManageCenterInfo(manageFile);
            List<string> types = manageList.Keys.ToList();

            Random rand = new Random();

            int days = (endTime - startTime).Days + 1;

            int spaceValue = 24 * 60 * 60 / dayNum;

            List<object> datas = new List<object>();

            long allCount = dayNum * days;
            for (int i = 0; i < days; i++)
            {
                for (int j = 0; j < dayNum; j++)
                {
                    DateTime time = startTime.AddSeconds(spaceValue * j);

                    count++;

                    if (OneNum > 0 && OneSleep > 0 && count % OneNum == 0)
                    {
                        if (datas.Count > 0)
                        {
                            dataBaseTable.InsertDataByTable("mc_managementcenterlog" + startTime.ToString("yyyyMM"), typeof(mc_managementcenterlog), datas);
                            datas.Clear();
                        }
                        Thread.Sleep(OneSleep);
                    }

                    mc_managementcenterlog mc_managementcenterlog = new mc_managementcenterlog()
                    {
                        log_id = time.ToString("yyyyMMddHHmmss") + stationCode + Guid.NewGuid().ToString("N"),
                        station_code = stationCode,
                        station_ip = "127.0.0.1",
                        operator_guid = userInfo.guid,
                        operator_id = userInfo.user_id,
                        operator_name = userInfo.user_name,
                        operator_code = userInfo.user_code,
                        operator_org_id = userInfo.org_id,
                        operator_org_id_code = userOrgInfo.org_id_code,
                        operator_org_name = userOrgInfo.org_name,
                        op_time = time.ToString("yyyy-MM-dd hh:mm:ss"),
                        op_type = types[rand.Next(0, types.Count)]
                    };

                    datas.Add(mc_managementcenterlog);

                    progess(Math.Round((decimal)count / (decimal)allCount, 4), index);
                    Thread.Sleep(1);
                }

                if (datas.Count > 0)
                {
                    dataBaseTable.InsertDataByTable("mc_managementcenterlog" + startTime.ToString("yyyyMM"), typeof(mc_managementcenterlog), datas);
                    datas.Clear();
                }
                startTime = startTime.AddDays(1);
            }
            return count;
        }

        private static Dictionary<string, string> GetManageCenterInfo(string fileName)
        {
            Dictionary<string, string> manageList = new Dictionary<string, string>();
            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.Load(fileName);
            XmlNode node = xmlDoc.SelectSingleNode("webServer");
            foreach (XmlElement item in node.ChildNodes)
            {
                string id = item.GetAttribute("ID").ToString();
                string value = item.GetAttribute("Value").ToString();


                manageList.Add(id, value);
            }

            return manageList;
        }
        #endregion

        #region 状态日志表
        public static int AddStatuslog(DateTime startTime,
                               DateTime endTime,
                               int dayNum,
                               int index,
                               int OneNum,
                               int OneSleep,
                               Progess progess
                              )
        {
            int count = 0;

            Random rand = new Random();

            int days = (endTime - startTime).Days + 1;

            int spaceValue = 24 * 60 * 60 / dayNum;

            RepositoryBase<mc_statuslog> repositoryBase = new RepositoryBase<mc_statuslog>();

            long allCount = dayNum * days;
            for (int i = 0; i < days; i++)
            {
                for (int j = 0; j < dayNum; j++)
                {
                    DateTime time = startTime.AddSeconds(spaceValue * j);

                    count++;

                    if (OneNum > 0 && OneSleep > 0 && count % OneNum == 0)
                    {
                        Thread.Sleep(OneSleep);
                    }

                    mc_statuslog medialog = new mc_statuslog()
                    {
                        status_id = time.ToString("yyyyMMddHHmmss") + Guid.NewGuid().ToString("N"),
                        device_type = rand.Next(1, 4),
                        status_module = rand.Next(11, 30),
                        status_message = "测试数据",
                        status_time = time.ToString("yyyy-MM-dd hh:mm:ss"),
                        status_type = rand.Next(1, 4)
                    };

                    repositoryBase.Add(medialog);
                    if (count % 1000 == 0)
                    {
                        repositoryBase.Commit();
                    }

                    progess(Math.Round((decimal)count / (decimal)allCount, 4), index);
                    Thread.Sleep(1);
                }
                startTime = startTime.AddDays(1);
            }

            repositoryBase.Commit();
            return count;
        }

        #endregion

        #region 图片base64编码
        public static string GetBase64StringForImage()
        {
            string file = AppDomain.CurrentDomain.BaseDirectory + "Codes\\thum.jpg";
            if (!File.Exists(file))
            {
                return string.Empty;
            }

            using (System.IO.MemoryStream m = new System.IO.MemoryStream())
            {
                System.Drawing.Bitmap bit = new System.Drawing.Bitmap(file);

                bit.Save(m, System.Drawing.Imaging.ImageFormat.Gif);

                bit.Dispose();

                byte[] b = m.GetBuffer();

                string base64string = Convert.ToBase64String(b);

                return base64string;
            }
        }
        #endregion
    }
}
