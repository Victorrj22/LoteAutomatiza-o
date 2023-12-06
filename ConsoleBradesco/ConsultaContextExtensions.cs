using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

public static class ConsultaContextExtensions
{
    public static void AdicionarTokenEConsulta(this ConsultaContext context)
    {
        // Crie um novo TokenAcesso
        context.Database.ExecuteSqlRaw("INSERT INTO public.token_acesso (cliente_id, token, acesso_total, incluido_em, incluido_por, name, key_type) " +
                                       "VALUES (1631, translate(uuid_generate_v4()::TEXT, '{}-', ''), true, now(), 0, 'Bradesco RH Lote 2023-12-05 (23)', 1)");
        
        var novoTokenAcessoBradesco = context.TokenAcesso.FromSqlRaw($"SELECT TOKEN_ACESSO_ID FROM public.token_acesso " +
                                                                     "WHERE cliente_id = 1631 AND NAME ILIKE '%Bradesco RH Lote 2023-12-05 (23)%' ").FirstOrDefault();
        
        context.TokenAcesso.Add(novoTokenAcessoBradesco);
        context.SaveChanges();
    }

    public static void AdicionarConsultaPorSQL(this ConsultaContext context, string nomeTabela)
    {
        using (var transaction = context.Database.BeginTransaction())
        {
            try
            {
                var sqlQuery = @"
    INSERT INTO public.consulta
    (consulta_tipo_id,
    token_acesso_id,
    consulta_resultado_tipo_id,
    origem_id,
    uid_base36,
    ip_remoto,
    acesso_negado,
    inicio,
    chave,
    document,
    entrada,
    faturavel,
    armazenar_pdf,
    custo_cred,
    custo_cred_pdf_geracao,
    custo_cred_pdf_armaz,
    custo_cred_total_inc_subcons,
    retorno_original_apagado,
    retorno_desatualizado,
    options,
    hostname,
    input_params,
    remote_ip,
    async,
    async_run_persistent,
    async_attempts,
    async_child)
    SELECT @consultaTipoId as consulta_tipo_id,
        ta.token_acesso_id,
        @consultaResultadoTipoId as consulta_resultado_tipo_id,
        @origemId as origem_id,
        doc_normalizado +
            'B' +
            CONVERT(VARCHAR(10), GETDATE(), 12) +
            CONVERT(VARCHAR(6), FLOOR((RAND() * 2000000000) + 1)) uid_base36,
        2130706433 ip_remoto,
        0 acesso_negado,
        GETDATE() AT TIME ZONE 'America/Fortaleza' inicio,
        doc_normalizado chave,
        doc_normalizado AS [document],
        CASE
            WHEN nascimento IS NULL THEN 'Cpf:' + doc_normalizado
            ELSE 'Cpf:' + doc_normalizado + ';Nascimento:' + CONVERT(VARCHAR, nascimento, 23)
        END entrada,
        0 faturavel,
        0 armazenar_pdf,
        0 custo_cred,
        0 custo_cred_pdf_geracao,
        0 custo_cred_pdf_armaz,
        0 custo_cred_total_inc_subcons,
        0 retorno_original_apagado,
        0 retorno_desatualizado,
        '{
            ""ExecutionMode"": 1,
            ""Async"": true,
            ""AsyncRunPersistent"": true,
            ""PdfGenerationMode"": 1,
            ""PdfGenerationSync"": true,
            ""StorePdf"": false,
            ""Timeout"": ""48:00:00"",
            ""CacheMaxAge"": ""60.00:00:00"",
            ""CacheIncludeNotFound"": null
        }'::jsonb AS options,
        'ROG-LEANDRO' hostname,
        '{
            ""cpf"": { ""numero"": ' + CONVERT(VARCHAR, doc_normalizado) + ' }, 
            ""full_name"": ' + COALESCE(REPLACE(nome_completo, '', '')) + ',
            ""emails"": ' + 
                CASE
                    WHEN emails IS NULL THEN 'null'
                    ELSE '[""' + REPLACE(emails, ',', '"",""') + '""]'
                END + ',
            ""customer_external_id"": ' + COALESCE(REPLACE(c.external_id, '', '')) + ', 
            ""credit_analysis"": true, 
            ""birth_date"": ' +
                CASE
                    WHEN nascimento IS NULL THEN 'null'
                    ELSE '{ ""date"": ""' + CONVERT(VARCHAR, nascimento, 23) + '"" }'
                END + ',
            ""position_title"": """"
        }'::jsonb AS input_params,
        2130706433 AS remote_ip,
        1 AS async,
        1 AS async_run_persistent,
        0 AS async_attempts,
        0 AS async_child 
    FROM (
        SELECT
            RIGHT('00000000000' + CONVERT(VARCHAR, doc), 11) doc_normalizado
        FROM (
            SELECT DISTINCT
                ON (cpf) linha,
                NULLIF(LTRIM(REGEXP_REPLACE(LTRIM(cpf), '[^0-9]', '', 'g'), '0'), '') doc,
                CASE
                    WHEN TRIM(TRANSLATE(TRANSLATE(nascimento, E' \r\n', ''), '|.-', '///')) ~* '^(\d{2}\/\d{2}\/\d{4})$'
                        THEN CONVERT(DATE, TRIM(TRANSLATE(TRANSLATE(nascimento, E' \r\n', ''), '|.-', '///')), 101)
                END nascimento,
                TRIM(REPLACE(TRANSLATE(nome, E'\r\n', ''), ' ', '')) nome_completo,
                TRIM(REPLACE(TRANSLATE(email, E'\r\n', ''), ' ', '')) emails,
                NULL email,
                " + @context.TokenAcesso + @" as token_acesso_id
            FROM " + nomeTabela + @"
            WHERE LEN(NULLIF(LTRIM(REGEXP_REPLACE(LTRIM(cpf), '[^0-9]', '', 'g'), '0'), '')) <= 14
        ) t1
        WHERE COALESCE(doc, '0') != '0'
    ) docs
    INNER JOIN public.token_acesso ta ON docs.token_acesso_id = ta.token_acesso_id
    INNER JOIN public.cliente c ON c.cliente_id = ta.cliente_id
    INNER JOIN public.consulta_tipo ct ON ct.consulta_tipo_id = 30200
    WHERE NOT EXISTS (
        SELECT 1
        FROM public.consulta c
        INNER JOIN public.token_acesso t ON c.token_acesso_id = t.token_acesso_id
        WHERE c.document = doc_normalizado
        AND t.cliente_id = ta.cliente_id
        AND t.token_acesso_id = ta.token_acesso_id
        AND c.consulta_tipo_id = ct.consulta_tipo_id
        AND c.consulta_tipo_id = ct.consulta_tipo_id
        AND c.consulta_master_id IS NULL
        AND (c.fim IS NULL OR
            (c.fim IS NOT NULL AND c.consulta_resultado_tipo_id IN (1, 2, 3, 5, 6, 7)))
        AND inicio >= (GETDATE() - 7)
        AND (c.consulta_resultado_tipo_id NOT IN (1, 2) OR pdf_resultado_compact IS NOT NULL)
        AND 1 = 1)
    ORDER BY linha DESC
    LIMIT 5000;";
                context.Database.ExecuteSqlRaw(sqlQuery);

                transaction.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao adicionar consulta por SQL: {ex.Message}");
                transaction.Rollback();
            }
        }
    }
}