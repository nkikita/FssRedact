using Npgsql;
using System.Xml;

namespace FssRedact.XMLFunctions
{
    public class EditFuncExclude
    {
        public static async Task UpdateExcludePeriodAsync(int docId, int oldType, DateTime oldBegin, DateTime oldEnd,
                                                          int newType, DateTime newBegin, DateTime newEnd)
        {
            string connectionString = "Host=localhost;Port=5432;Database=Practice;Username=postgres;Password=nikitos";

            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            string selectQuery = "SELECT dataxml FROM proactive_documents WHERE id = @docId";
            string? xmlContent = null;

            using (var selectCmd = new NpgsqlCommand(selectQuery, connection))
            {
                selectCmd.Parameters.AddWithValue("docId", docId);
                using var reader = await selectCmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    xmlContent = reader.GetString(0);
                }
            }

            if (xmlContent == null)
                return;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlContent);

            var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("ns3", "urn:ru:fss:integration:types:proactive:benefit1:v01");

            var excludePeriods = xmlDoc.SelectNodes("//ns3:excludePeriod", nsmgr);

            XmlNode? targetNode = null;

            foreach (XmlNode node in excludePeriods!)
            {
                var typeNode = node.SelectSingleNode("ns3:type", nsmgr);
                var beginNode = node.SelectSingleNode("ns3:period/ns3:begin", nsmgr);
                var endNode = node.SelectSingleNode("ns3:period/ns3:end", nsmgr);

                if (typeNode?.InnerText == oldType.ToString() &&
                    beginNode?.InnerText == oldBegin.ToString("yyyy-MM-dd") &&
                    endNode?.InnerText == oldEnd.ToString("yyyy-MM-dd"))
                {
                    targetNode = node;
                    break;
                }
            }

            if (targetNode != null)
            {
                var typeNode = targetNode.SelectSingleNode("ns3:type", nsmgr);
                var beginNode = targetNode.SelectSingleNode("ns3:period/ns3:begin", nsmgr);
                var endNode = targetNode.SelectSingleNode("ns3:period/ns3:end", nsmgr);

                if (typeNode != null) typeNode.InnerText = newType.ToString();
                if (beginNode != null) beginNode.InnerText = newBegin.ToString("yyyy-MM-dd");
                if (endNode != null) endNode.InnerText = newEnd.ToString("yyyy-MM-dd");

                string updatedXml = xmlDoc.OuterXml;

                string updateQuery = "UPDATE proactive_documents SET dataxml = @dataxml::xml WHERE id = @docId";
                using var updateCmd = new NpgsqlCommand(updateQuery, connection);
                updateCmd.Parameters.AddWithValue("dataxml", updatedXml);
                updateCmd.Parameters.AddWithValue("docId", docId);
                await updateCmd.ExecuteNonQueryAsync();
            }
        }
    }
}