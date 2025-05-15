using Npgsql;
using System.Xml;
using NpgsqlTypes;
namespace FssRedact.XMLFunctions
{
    public class AddFubcDownPeriod
    {
        public static async Task<bool> AddDownPeriodAsync(int docId, DateTime newBegin, DateTime newEnd, decimal newIdleAverage)
        {
            string connectionString = "Host=localhost;Port=5432;Database=Practice;Username=postgres;Password=nikitos";

            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                // Получаем XML по id
                string selectQuery = "SELECT dataxml FROM proactive_documents WHERE id = @docId";
                string? xmlContent = null;

                using (var cmd = new NpgsqlCommand(selectQuery, connection))
                {
                    cmd.Parameters.AddWithValue("docId", docId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            xmlContent = reader.GetString(0);
                        }
                    }
                }

                if (xmlContent == null)
                    return false;

                // Загружаем XML
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlContent);

                var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("ns3", "urn:ru:fss:integration:types:proactive:benefit1:v01");

                XmlNode? downPeriodList = xmlDoc.SelectSingleNode("//ns3:downPeriodList", nsmgr);
                if (downPeriodList == null)
                    return false;

                // Создаём новый элемент <ns3:downPeriod>
                XmlElement newDownPeriod = xmlDoc.CreateElement("ns3", "downPeriod", nsmgr.LookupNamespace("ns3")!);

                XmlElement period = xmlDoc.CreateElement("ns3", "period", nsmgr.LookupNamespace("ns3")!);
                XmlElement begin = xmlDoc.CreateElement("ns3", "begin", nsmgr.LookupNamespace("ns3")!);
                begin.InnerText = newBegin.ToString("yyyy-MM-dd");
                XmlElement end = xmlDoc.CreateElement("ns3", "end", nsmgr.LookupNamespace("ns3")!);
                end.InnerText = newEnd.ToString("yyyy-MM-dd");

                period.AppendChild(begin);
                period.AppendChild(end);
                newDownPeriod.AppendChild(period);

                XmlElement idleAverage = xmlDoc.CreateElement("ns3", "idleAverage", nsmgr.LookupNamespace("ns3")!);
                idleAverage.InnerText = newIdleAverage.ToString(System.Globalization.CultureInfo.InvariantCulture);
                newDownPeriod.AppendChild(idleAverage);

                // Добавляем в конец списка
                downPeriodList.AppendChild(newDownPeriod);

                // Обновляем в БД
                string updateQuery = "UPDATE proactive_documents SET dataxml = @dataxml WHERE id = @docId";

               using (var updateCmd = new NpgsqlCommand(updateQuery, connection))
                {
                    updateCmd.Parameters.AddWithValue("docId", docId);
                    updateCmd.Parameters.Add("dataxml", NpgsqlDbType.Xml).Value = xmlDoc.OuterXml;

                    int rowsAffected = await updateCmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }

            }
        }
    }
}