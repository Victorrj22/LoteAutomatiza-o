using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public class TokenAcesso
{
    [Key]
    [Column("token_acesso_id")]
    public int token_acesso_id { get; set; }
}

public class ConsultaContext : DbContext
{
     public DbSet<TokenAcesso> TokenAcesso { get; set; }

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

