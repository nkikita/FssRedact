using Npgsql;
using System.Xml;
using FssRedact.Models;

namespace FssRedact.XMLFunctions
{
    public class GetUserInfo
    {
          public static async Task<InsuredPerson?> GetPersonInfoAsync(int docId)
        {
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
                            nsmgr.AddNamespace("ns4", "http://www.fss.ru/integration/types/person/v02");

                            string GetNodeText(string xpath) =>
                                xmlDoc.SelectSingleNode(xpath, nsmgr)?.InnerText ?? "";

                            return new InsuredPerson
                            {
                                Last_Name = GetNodeText("//ns3:fullName/ns4:lastName"),
                                First_Name = GetNodeText("//ns3:fullName/ns4:firstName"),
                                Middle_Name = GetNodeText("//ns3:fullName/ns4:middleName"),
                                Snils = GetNodeText("//ns3:snils")
                            };
                        }
                    }
                }
            }

            return null;
        }
    }
}