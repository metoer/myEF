using DataBase.Entity.ModelBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Models
{
    public class mc_alarmlog
    {
        [Key]
        public string alarm_id { get; set; }
        public string device_id { get; set; }// -- 告警设备ID
        public string device_alias { get; set; }// -- 告警设备别名
        public string device_ip { get; set; }// -- 告警设备IP
        public string device_type { get; set; }// -- 设备类型...
        public string module_id { get; set; }//, -- 模块ID， 和文档中的模块编码对应
        public string module_name { get; set; }//, -- 模块名，和文档中的模块英文名对应
        public string alarm_message { get; set; }//, -- 告警内容
        public string alarm_time { get; set; }// -- 告警时间，格式：YYYY-MM-DD HH:mm:ss
        public string last_alarm_time { get; set; }//, -- 最后告警时间，格式：YYYY-MM-DD HH:mm:ss
        public string alarm_code { get; set; }// -- 告警代码
        public string alarm_level { get; set; }//, -- 告警级别...
        public string alarm_status { get; set; }//, -- 告警状态：...
        public string solved_time { get; set; }//, -- 解决时间，格式：YYYY-MM-DD HH:mm:ss
    }

    [Export("mc_alarmlogMapping")]
    public class mappingmc_alarmlog : MappingBase<mc_alarmlog>
    { }


    public class AlarmInfo
    {
        public string ModuleID
        {
            get;
            set;
        }

        public string ModuleName
        {
            get;
            set;
        }

        public string ModuleAlias
        {
            get;
            set;
        }

        public string AlarmCode
        {
            get;
            set;
        }

        public string AlarmLevel
        {
            get;
            set;
        }

        public string AlarmMark
        {
            get;
            set;
        }
    }
}
