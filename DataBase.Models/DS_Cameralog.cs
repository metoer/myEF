using DataBase.Entity.ModelBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Models
{ /// <summary>
    /// 执法记录仪操作日志
    /// </summary>
    public class mc_cameralog
    {
        [Key]
        public string log_id
        {
            get;
            set;
        }
        public string device_id
        {
            get;
            set;
        }

        public string operator_guid
        {
            get;
            set;
        }
        public string operator_id
        {
            get;
            set;
        }
        public string operator_name
        {
            get;
            set;
        }
        public string operator_code
        {
            get;
            set;
        }
        public string operator_org_id
        {
            get;
            set;
        }
        public string operator_org_id_code
        {
            get;
            set;
        }
        public string operator_org_name
        {
            get;
            set;
        }
        public string op_time
        {
            get;
            set;
        }
        public string op_type
        {
            get;
            set;
        }

        public string op_description
        {
            get;
            set;
        }

        public string operated_guid
        {
            get;
            set;
        }
        public string operated_id
        {
            get;
            set;
        }
        public string operated_name
        {
            get;
            set;
        }
        public string operated_code
        {
            get;
            set;
        }
        public string operated_org_id
        {
            get;
            set;
        }
        public string operated_org_id_code
        {
            get;
            set;
        }
        public string operated_org_name
        {
            get;
            set;
        }

        public string station_code
        {
            get;
            set;
        }

        public string file_name
        {
            get;
            set;
        }

        public string collect_time
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 执法记录仪操作日志
    /// </summary>
    public class ds_cameralog 
    {
        [Key]
        public string log_id
        {
            get;
            set;
        }
        public string device_id
        {
            get;
            set;
        }

        public string operator_guid
        {
            get;
            set;
        }
        public string operator_id
        {
            get;
            set;
        }
        public string operator_name
        {
            get;
            set;
        }
        public string operator_code
        {
            get;
            set;
        }
        public string operator_org_id
        {
            get;
            set;
        }
        public string operator_org_id_code
        {
            get;
            set;
        }
        public string operator_org_name
        {
            get;
            set;
        }
        public string op_time
        {
            get;
            set;
        }
        public string op_type
        {
            get;
            set;
        }

        public string op_description
        {
            get;
            set;
        }

        public string operated_guid
        {
            get;
            set;
        }
        public string operated_id
        {
            get;
            set;
        }
        public string operated_name
        {
            get;
            set;
        }
        public string operated_code
        {
            get;
            set;
        }
        public string operated_org_id
        {
            get;
            set;
        }
        public string operated_org_id_code
        {
            get;
            set;
        }
        public string operated_org_name
        {
            get;
            set;
        }

        public string station_code
        {
            get;
            set;
        }

        public string file_name
        {
            get;
            set;
        }

        public string collect_time
        {
            get;
            set;
        }

        public int upload_process
        {
            get;
            set;
        }
    }

    [Export("ds_cameralogMapping")]
    public class mappingds_cameralog : MappingBase<ds_cameralog>
    { }
    [Export("mc_cameralogMapping")]
    public class mappingmc_cameralog : MappingBase<mc_cameralog>
    { }
}
