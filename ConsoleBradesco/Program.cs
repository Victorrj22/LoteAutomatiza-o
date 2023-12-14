using System;
using System.Linq;

class Program
{
    static void Main()
    {
        using (var context = new ConsultaContext())
        {
            context.AdicionarTokenEConsulta();
            context.AdicionarConsultaPorSQL();
        }
    }
}