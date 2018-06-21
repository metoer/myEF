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
    public class mc_managementcenterlog
    {
        [Key]
        public string log_id { get; set; }//, -- 日志ID，主键
        public string station_id { get; set; }//, -- 采集站ID
        public string station_code { get; set; }//, -- 采集站设备序列号
        public string station_ip { get; set; }//, -- 采集站IP
        public string operator_guid { get; set; }//, -- 使用者Guid
        public string operator_id { get; set; }//, -- 使用者ID
        public string operator_name { get; set; }//, -- 使用者姓名
        public string operator_code { get; set; }//, -- 使用者编号
        public string operator_org_id { get; set; }//, -- 使用者部门ID
        public string operator_org_id_code { get; set; }//, -- 使用者部门ID 编码
        public string operator_org_name { get; set; }//, -- 使用者部门名称
        public string op_time { get; set; }//, -- 日志时间，格式：YYYY-MM-DD HH:mm:ss
        public string op_type { get; set; }//, -- 操作类型...
        public string op_description { get; set; }// -- 操作描述
        public string operated_guid { get; set; }//, -- 被操作者Guid
        public string operated_id { get; set; }//, -- 被操作者ID
        public string operated_name { get; set; }//, -- 被操作者姓名
        public string operated_code { get; set; }// -- 被操作者编号
        public string operated_org_id { get; set; }//, -- 被操作者部门ID
        public string operated_org_id_code { get; set; }//, -- 被操作者部门ID 编码
        public string operated_org_name { get; set; }//, -- 被操作者部门名称
    }

    [Export("mc_managementcenterlogMapping")]
    public class mappingmc_managementcenterlog : MappingBase<mc_managementcenterlog>
    { }
}
