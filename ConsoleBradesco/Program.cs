using System;
using System.Linq;

class Program
{
    static void Main()
    {
        using (var context = new ConsultaContext())
        {
            context.AdicionarTokenEConsulta();
            context.AdicionarConsultaPorSQL("customer_bradesco_rh_jobs.lote_20231205");
        }
    }
}