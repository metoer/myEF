using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using DataBase.Entity.ModelBase;
using System.Data.Entity.Infrastructure;

namespace DataBase.Entity.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public System.Data.StateChangeEventHandler ConnectionStateChange;
        public AppDbContext(string configKey)
            : base(configKey)
        {

            var catalog = new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory);
            var container = new CompositionContainer(catalog);
            m_Mappings = container.GetExportedValues<IMapping>();
        }

        [ImportMany]
        IEnumerable<IMapping> m_Mappings = null;

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (m_Mappings != null)
            {
                //这里是关键
                foreach (var mapping in m_Mappings)
                {
                    mapping.RegistTo(modelBuilder.Configurations);
                }
            }
            base.OnModelCreating(modelBuilder);
        }
    }
}
