public class Program
{
    static async Task Main()
    {
        var query = new ConsultaContextExtensions();
        await query.AdicionarTokenEConsulta();
        await query.AdicionarConsultaPorSQL();
    }
}