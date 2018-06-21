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
    public class mc_stationlog
    {
        [Key]
        public string log_id { get; set; }
        public string station_id { get; set; }
        public string station_code { get; set; }
        public string station_ip { get; set; }
        public string operator_guid { get; set; }
        public string operator_id { get; set; }
        public string operator_name { get; set; }
        public string operator_code { get; set; }
        public string operator_org_id { get; set; }
        public string operator_org_id_code { get; set; }
        public string operator_org_name { get; set; }
        public string op_time { get; set; }
        public string op_type { get; set; }
        public string op_description { get; set; }
        public string operated_guid { get; set; }
        public string operated_id { get; set; }
        public string operated_name { get; set; }
        public string operated_code { get; set; }
        public string operated_org_id { get; set; }
        public string operated_org_id_code { get; set; }
        public string operated_org_name { get; set; }
        public string object_id { get; set; }
        public string op_object_type { get; set; }
     
    }

    public class ds_stationlog
    {
        [Key]
        public string log_id { get; set; }
        public string station_id { get; set; }
        public string station_code { get; set; }
        public string station_ip { get; set; }
        public string operator_guid { get; set; }
        public string operator_id { get; set; }
        public string operator_name { get; set; }
        public string operator_code { get; set; }
        public string operator_org_id { get; set; }
        public string operator_org_id_code { get; set; }
        public string operator_org_name { get; set; }
        public string op_time { get; set; }
        public string op_type { get; set; }
        public string op_description { get; set; }
        public string operated_guid { get; set; }
        public string operated_id { get; set; }
        public string operated_name { get; set; }
        public string operated_code { get; set; }
        public string operated_org_id { get; set; }
        public string operated_org_id_code { get; set; }
        public string operated_org_name { get; set; }
        public string object_id { get; set; }
        public string op_object_type { get; set; }
        public int upload_process { get; set; }
    }

    [Export("ds_stationlogMapping")]
    public class mappingds_stationlog : MappingBase<ds_stationlog>
    { }
    [Export("mc_stationlogMapping")]
    public class mappingmc_stationlog : MappingBase<mc_stationlog>
    { }
}
