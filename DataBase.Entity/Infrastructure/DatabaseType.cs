using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataBase.Entity.Infrastructure
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DatabaseType
    {
        /// <summary>
        /// SQLServer
        /// </summary>
        SQLServer = 1,
        /// <summary>
        /// MySQL
        /// </summary>
        MySQL = 2,
        /// <summary>
        /// PostgreSQL
        /// </summary>
        PostgreSQL = 3
    }
}
