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
    public class mc_device
    {
        [Key]
        public string device_id
        {
            get;
            set;
        }

        public string device_code
        {
            get;
            set;
        }

        public string device_alias
        {
            get;
            set;
        }

        public string org_id
        {
            get;
            set;
        }

        public string is_active
        {
            get;
            set;
        }
    }

    [Export("ds_deviceMapping")]
    public class mappingds_device : MappingBase<ds_device>
    { }

    public class ds_device 
    {
        [Key]
        public string device_id
        {
            get;
            set;
        }

        public string device_code
        {
            get;
            set;
        }

        public string device_alias
        {
            get;
            set;
        }

        public string org_id
        {
            get;
            set;
        }

        public string is_active
        {
            get;
            set;
        }   
    }

    [Export("mc_deviceMapping")]
    public class mappingmc_device : MappingBase<mc_device>
    { }
}
