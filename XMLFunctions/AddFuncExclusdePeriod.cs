using Npgsql;
using System.Xml;
using NpgsqlTypes;


namespace FssRedact.XMLFunctions
{
    public class AddFuncExclusdePeriod
    {
        public static async Task AddExcludePeriodAsync(int docId, int? newType, DateTime? newBegin, DateTime? newEnd)
        {
            string connectionString = "Host=localhost;Port=5432;Database=Practice;Username=postgres;Password=nikitos";

            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                // Получаем XML по docId
                string selectQuery = "SELECT dataxml FROM proactive_documents WHERE id = @docId";
                string xmlContent = null;

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
                    return; 

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlContent);

                var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("ns3", "urn:ru:fss:integration:types:proactive:benefit1:v01");

                // Находим excludePeriodList
                var excludePeriodListNode = xmlDoc.SelectSingleNode("//ns3:excludePeriodList", nsmgr);
                if (excludePeriodListNode == null)
                    return;

                var excludePeriod = xmlDoc.CreateElement("ns3:excludePeriod", nsmgr.LookupNamespace("ns3"));

                var typeNode = xmlDoc.CreateElement("ns3:type", nsmgr.LookupNamespace("ns3"));
                typeNode.InnerText = newType?.ToString() ?? "";
                excludePeriod.AppendChild(typeNode);

                var periodNode = xmlDoc.CreateElement("ns3:period", nsmgr.LookupNamespace("ns3"));

                var beginNode = xmlDoc.CreateElement("ns3:begin", nsmgr.LookupNamespace("ns3"));
                beginNode.InnerText = newBegin?.ToString("yyyy-MM-dd") ?? "";
                periodNode.AppendChild(beginNode);

                var endNode = xmlDoc.CreateElement("ns3:end", nsmgr.LookupNamespace("ns3"));
                endNode.InnerText = newEnd?.ToString("yyyy-MM-dd") ?? "";
                periodNode.AppendChild(endNode);

                excludePeriod.AppendChild(periodNode);

                // Добавляем excludePeriod в excludePeriodList
                excludePeriodListNode.AppendChild(excludePeriod);

                // Обновляем базу
                string updateQuery = "UPDATE proactive_documents SET dataxml = @newXml WHERE id = @docId";
                using (var cmd = new NpgsqlCommand(updateQuery, connection))
                {
                    cmd.Parameters.AddWithValue("newXml", NpgsqlDbType.Xml, xmlDoc.OuterXml); // <--- здесь указываем Xml
                    cmd.Parameters.AddWithValue("docId", docId);
                    await cmd.ExecuteNonQueryAsync();
                }

            }
        }
    }
}