using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CompaniesManagement.Models
{
    public class DataContext : DbContext
    {
        /// <summary>
        /// public constructor():base("database_name"){...} 
        /// </summary>
        
        public DbSet<Company> Companies { set; get; }
    }
}