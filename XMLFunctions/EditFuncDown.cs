using System;
using System.Globalization;
using System.Threading.Tasks;
using Npgsql;

namespace FssRedact.XMLFunctions
{
    public class EditFuncDown
    {
        public static async Task UpdateDownPeriod(int docId, string targetBegin, string targetEnd, string oldIdle, string newBegin, string newEnd, string newIdle)
        {
            string connectionString = "Host=localhost;Port=5432;Database=Practice;Username=postgres;Password=nikitos";
            DateTime beginDate, endDate;
            if (!DateTime.TryParseExact(targetBegin, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out beginDate))
            {
                
                Console.WriteLine("Ошибка: Некорректный формат даты начала.");
                return;
            }

            if (!DateTime.TryParseExact(targetEnd, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out endDate))
            {
                Console.WriteLine("Ошибка: Некорректный формат даты конца.");
                return;
            }

            // Преобразование в нужный формат
            string formattedBeginDate = beginDate.ToString("yyyy-MM-dd");
            string formattedEndDate = endDate.ToString("yyyy-MM-dd");

            // Создание соединения с базой данных
            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                // SQL запрос с параметрами
                string query = @"
                    UPDATE proactive_documents
                    SET dataxml = XMLPARSE(DOCUMENT 
                        REGEXP_REPLACE(
                            XMLSERIALIZE(CONTENT dataxml AS TEXT),
                            '<ns3:downPeriod>\s*<ns3:period>\s*<ns3:begin>' || @old_begin || '</ns3:begin>\s*<ns3:end>' || @old_end || '</ns3:end>\s*</ns3:period>\s*<ns3:idleAverage>' || @old_idle || '</ns3:idleAverage>',
                            '<ns3:downPeriod>
                                <ns3:period>
                                    <ns3:begin>' || @new_begin || '</ns3:begin>
                                    <ns3:end>' || @new_end || '</ns3:end>
                                </ns3:period>
                                <ns3:idleAverage>' || @new_idle || '</ns3:idleAverage>',
                            'g'
                        )
                    )
                    WHERE id = @doc_id;
                ";

                // Создание команды с параметрами
                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    // Добавление параметров
                    cmd.Parameters.AddWithValue("old_begin", formattedBeginDate);
                    cmd.Parameters.AddWithValue("old_end", formattedEndDate);
                    cmd.Parameters.AddWithValue("old_idle", oldIdle);

                    cmd.Parameters.AddWithValue("new_begin", newBegin);
                    cmd.Parameters.AddWithValue("new_end", newEnd);
                    cmd.Parameters.AddWithValue("new_idle", newIdle);

                    cmd.Parameters.AddWithValue("doc_id", docId);

                    Console.WriteLine("Proverks" + formattedBeginDate + formattedEndDate + oldIdle + targetBegin + targetEnd + newIdle);
                   
                    // Выполнение команды
                    int affectedRows = await cmd.ExecuteNonQueryAsync();
                    Console.WriteLine($"{affectedRows} row(s) updated.");
                }
            }
        }
    }
}
