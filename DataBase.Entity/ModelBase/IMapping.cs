using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.ComponentModel.Composition;

namespace DataBase.Entity.ModelBase
{
    [InheritedExport]
    public interface IMapping
    {
        void RegistTo(ConfigurationRegistrar configurationRegistrar);
    }
}
