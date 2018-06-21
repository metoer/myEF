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
    public class mc_organization
    {
        [Key]
        public string org_id
        {
            get;
            set;
        }

        public string org_id_code
        {
            get;
            set;
        }

        public string org_name
        {
            get;
            set;
        }

        public string org_code
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
    [Export("ds_organizationMapping")]
    public class mappingds_organization : MappingBase<ds_organization>
    { }

    public class ds_organization
    {
        [Key]
        public string org_id
        {
            get;
            set;
        }

        public string org_id_code
        {
            get;
            set;
        }

        public string org_name
        {
            get;
            set;
        }

        public string org_code
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
    [Export("mc_organizationMapping")]
    public class mappingmc_organization : MappingBase<mc_organization>
    { }
}
