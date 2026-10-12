using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace KTGKMD.EntityFrameworkCore;

public static class KTGKMDDbContextConfigurer
{
    public static void Configure(DbContextOptionsBuilder<KTGKMDDbContext> builder, string connectionString)
    {
        builder.UseSqlServer(connectionString);
    }

    public static void Configure(DbContextOptionsBuilder<KTGKMDDbContext> builder, DbConnection connection)
    {
        builder.UseSqlServer(connection);
    }
}
