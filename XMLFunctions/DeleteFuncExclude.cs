using Npgsql;
namespace FssRedact.XMLFunctions
{
    public class DeleteFuncExclude
    {
        public static async Task DeleteExcludePeriod(int docId, string targetBegin, string targetEnd)
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
                connection.Open();
                
                // SQL запрос с параметрами
                string query = @"
                    UPDATE proactive_documents
                    SET dataxml = XMLPARSE(DOCUMENT 
                        REGEXP_REPLACE(
                            XMLSERIALIZE(CONTENT dataxml AS TEXT),
                            '<ns3:excludePeriod>\s*<ns3:type>\d+</ns3:type>\s*<ns3:period>\s*' ||
                            '<ns3:begin>' || @targetBegin || '</ns3:begin>\s*' ||
                            '<ns3:end>' || @targetEnd || '</ns3:end>\s*</ns3:period>\s*</ns3:excludePeriod>',
                            '',
                            'g'
                        )
                    )
                    WHERE id = @docId;
                ";

                // Создание команды с параметрами
                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    // Добавление параметров
                    cmd.Parameters.AddWithValue("targetBegin", formattedBeginDate);
                    cmd.Parameters.AddWithValue("targetEnd", formattedEndDate);
                    cmd.Parameters.AddWithValue("docId", docId);

                    // Выполнение команды
                    int affectedRows = cmd.ExecuteNonQuery();
                    Console.WriteLine($"{affectedRows} row(s) updated.");
                }
            }
        }
    }
}