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
    /// <summary>
    /// 用户信息表
    /// </summary>
    public class mc_user
    {
        [Key]
        public string guid
        {
            get;
            set;
        }

        public string user_id
        {
            get;
            set;
        }

        public string user_name
        {
            get;
            set;
        }

        public string user_code
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

    [Export("ds_userMapping")]
    public class mappingds_user : MappingBase<ds_user>
    {      
    }

    public class ds_user
    {
        [Key]
        public string guid
        {
            get;
            set;
        }

        public string user_id
        {
            get;
            set;
        }

        public string user_name
        {
            get;
            set;
        }

        public string user_code
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

    [Export("mc_userMapping")]
    public class mappingmc_user : MappingBase<mc_user>
    { }
}
