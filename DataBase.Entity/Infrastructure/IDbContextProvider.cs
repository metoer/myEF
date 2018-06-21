using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace DataBase.Entity.Infrastructure
{
    [InheritedExport]
    public interface IDbContextProvider
    {
        AppDbContext Get();
    }
}
