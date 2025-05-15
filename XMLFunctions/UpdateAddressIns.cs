using Npgsql;


namespace FssRedact.XMLFunctions
{
    public class UpdateAddressIns
    {
         public static async Task UpdateFlatGuidAsync(int docId, string newFlat, string newGuid)
        {
            string connectionString = "Host=localhost;Port=5432;Database=Practice;Username=postgres;Password=nikitos";

            string sql = @"
                UPDATE proactive_documents
                SET dataxml = XMLPARSE(DOCUMENT 
                    REGEXP_REPLACE(
                        REGEXP_REPLACE(
                            XMLSERIALIZE(CONTENT dataxml AS TEXT),
                            '<flat>.*?</flat>', 
                            '<flat>' || @newFlat || '</flat>',
                            'g'
                        ),
                        '<guid>.*?</guid>', 
                        '<guid>' || @newGuid || '</guid>',
                        'g'
                    )
                )
                WHERE id = @docId;
            ";

            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            await using var cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("newFlat", newFlat);
            cmd.Parameters.AddWithValue("newGuid", newGuid);
            cmd.Parameters.AddWithValue("docId", docId);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}