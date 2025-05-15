using FssRedact.Models;
using Npgsql;
using System.Xml;

namespace FssRedact.XMLFunctions
{
    public class GetUserAddress
    {
        public static async Task<UserAdress?> GetFlatGuidAsync(int docId)
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

                            var flatNode = xmlDoc.SelectSingleNode("//*[local-name()='flat']");
                            var guidNode = xmlDoc.SelectSingleNode("//*[local-name()='guid']");

                            return new UserAdress
                            {
                                Flat = flatNode?.InnerText ?? "",
                                Guid = guidNode?.InnerText ?? ""
                            };
                        }
                    }
                }
            }

            return null;
        }
    }
}