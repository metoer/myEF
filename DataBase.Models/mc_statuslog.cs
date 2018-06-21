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
    public class mc_statuslog
    {
        [Key]
        public string status_id { get; set; } //-- 状态ID，主键
        public string device_id { get; set; } // -- 设备ID
        public string device_alias { get; set; } // -- 设备别名
        public string device_ip { get; set; } // -- 设备IP
        public int device_type { get; set; } // -- 设备类型...
        public int status_module { get; set; } // -- 模块
        public int status_type { get; set; } // -- 状态类型...
        public string status_message { get; set; } // -- 状态内容
        public string status_time { get; set; } //-- 状态上报时间，格式：YYYY-MM-DD HH:mm:ss
    }

    [Export("mc_statuslogMapping")]
    public class mappingmc_statuslog : MappingBase<mc_statuslog>
    { }
}
