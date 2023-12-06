using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

public class TokenAcesso
{
    [Column("cliente_id")]
    public int ClienteId { get; set; }
    
    public string Token { get; set; }
    public bool AcessoTotal { get; set; }
    public DateTime IncluidoEm { get; set; }
    public int IncluidoPor { get; set; }
    public string Name { get; set; }
    public int KeyType { get; set; }
    [Key]
    public int TokenAcessoId { get; set; }
}

public class ConsultaContext : DbContext
{
    public DbSet<TokenAcesso> TokenAcesso { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=intelligencedb.directdigital.com.br;Database=directdigital_intelligence;User=direct_master;Password=uyjPjesR4YDFQrdlbPCg;");
        }
    }
}