using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace KTGKMD.EntityFrameworkCore;

public static class KTGKMDDbContextConfigurer
{
    public static void Configure(DbContextOptionsBuilder<KTGKMDDbContext> builder, string connectionString)
    {
        builder.UseSqlServer(connectionString)
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
    }

    public static void Configure(DbContextOptionsBuilder<KTGKMDDbContext> builder, DbConnection connection)
    {
        builder.UseSqlServer(connection)
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
    }
}
