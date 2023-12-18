using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
public class ConsultaContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string connectionString = "Host=localhost;Port=5430;Database=rocadeira;Username=postgres;Password=password";

            optionsBuilder.UseNpgsql(connectionString);

            // Desativar migrações automáticas
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.LazyLoadOnDisposedContextWarning));
        }

        base.OnConfiguring(optionsBuilder);
    }
    
}

