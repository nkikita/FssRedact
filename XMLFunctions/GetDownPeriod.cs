using FssRedact.Models;
using Npgsql;
using System.Xml;
using System.Xml.XPath;

namespace FssRedact.XMLFunctions
{
    public class GetDownPeriod
    {
         public static async Task<List<PeriodDate>> GetDownPeriodsAsync(int docId)
        {
            var results = new List<PeriodDate>();

            string connectionString = "Host=localhost;Port=5432;Database=Practice;Username=postgres;Password=nikitos";

            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = @"
                    SELECT dataxml
                    FROM proactive_documents
                    WHERE id = @docId;
                ";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("docId", docId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string xmlContent = reader.GetString(0);

                            XmlDocument xmlDoc = new XmlDocument();
                            xmlDoc.LoadXml(xmlContent);

                            var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                            nsmgr.AddNamespace("ns3", "urn:ru:fss:integration:types:proactive:benefit1:v01");

                            XmlNodeList downPeriods = xmlDoc.SelectNodes("//ns3:downPeriod", nsmgr);

                            foreach (XmlNode downPeriod in downPeriods)
                            {
                                var beginNode = downPeriod.SelectSingleNode("ns3:period/ns3:begin", nsmgr);
                                var endNode = downPeriod.SelectSingleNode("ns3:period/ns3:end", nsmgr);
                                var idleAvgNode = downPeriod.SelectSingleNode("ns3:idleAverage", nsmgr);

                                if (DateTime.TryParse(beginNode?.InnerText, out var beginDate) &&
                                    DateTime.TryParse(endNode?.InnerText, out var endDate) &&
                                    int.TryParse(idleAvgNode?.InnerText, out var idleAverage))
                                {
                                    results.Add(new PeriodDate
                                    {
                                        Period_begin = beginDate,
                                        Period_end = endDate,
                                        Idle_average = idleAverage
                                    });
                                }
                            }
                        }
                    }
                }
            }

            return results;
        }
    }
}