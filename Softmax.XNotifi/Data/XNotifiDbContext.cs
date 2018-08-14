using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Softmax.XNotifi.Data
{
    public class XNotifiDbContext : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            // optionsBuilder.UseSqlServer("server=sql5017.site4now.net;database=DB_A25F98_sbcweb;uid=DB_A25F98_sbcweb_admin;password=Kudi@8419;MultipleActiveResultSets=true");
            //optionsBuilder.UseSqlServer("server=sql5036.site4now.net;database=DB_A25F98_sbcstage;uid=DB_A25F98_sbcstage_admin;password=Kudi@8419;MultipleActiveResultSets=true");
            optionsBuilder.UseSqlServer("server=L-HQ-ITD-04\\SQLSERVER2017EXP;database=Softmax_XNotifi;uid=sa;password=Password@1;MultipleActiveResultSets=true");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
