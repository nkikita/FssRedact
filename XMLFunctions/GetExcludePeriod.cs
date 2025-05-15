using FssRedact.Models;
using Npgsql;
using System.Xml;
using System.Xml.XPath;
namespace FssRedact.XMLFunctions
{
    public class GetExcludePeriod
    {
         public static async Task<List<ExcludePeriod>> GetExcludePeriodsAsync(int docId)
        {
            var results = new List<ExcludePeriod>();
            string connectionString = "Host=localhost;Port=5432;Database=Practice;Username=postgres;Password=nikitos";

            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT dataxml FROM proactive_documents WHERE id = @docId;";

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

                            XmlNodeList excludePeriods = xmlDoc.SelectNodes("//ns3:excludePeriod", nsmgr);

                            foreach (XmlNode exclude in excludePeriods)
                            {
                                var typeNode = exclude.SelectSingleNode("ns3:type", nsmgr);
                                var beginNode = exclude.SelectSingleNode("ns3:period/ns3:begin", nsmgr);
                                var endNode = exclude.SelectSingleNode("ns3:period/ns3:end", nsmgr);

                                results.Add(new ExcludePeriod
                                {
                                    Period_type = int.TryParse(typeNode?.InnerText, out int t) ? t : (int?)null,
                                    Begin_date = DateTime.TryParse(beginNode?.InnerText, out DateTime b) ? b : (DateTime?)null,
                                    End_date = DateTime.TryParse(endNode?.InnerText, out DateTime e) ? e : (DateTime?)null
                                });
                            }
                        }
                    }
                }
            }

            return results;
        }
    }
}