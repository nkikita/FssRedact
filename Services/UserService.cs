using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Npgsql;
using FssRedact.Models; 
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace FssRedact.Services
{
    public class UserService
        {
            private readonly string _connectionString;
            private readonly HttpClient _httpClient;

        public UserService(IConfiguration configuration, HttpClient httpClient)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Proactive_documents?>> GetDocumentInfoAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);

            var quer =  await connection.QueryAsync<Proactive_documents>("SELECT * FROM proactive_documents");
            return quer.ToList();
        }
        public async Task<InsuredPerson?> GetUserInfoAsync(int? userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);

            var quer =  await connection.QueryAsync<InsuredPerson>("select * from get_insured_info(@UserId)", new { userId });
            return quer.FirstOrDefault();
        }

        public async Task<IEnumerable<Address>> GetAddress(string text)
        {
            
            string url = $"https://data.pbprog.ru/api/address/full-address/parse?addressText={text}&token=939358a987eb492cb4adc68ee9944fb014f184db";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string jsonResponse = await response.Content.ReadAsStringAsync();
            var addresses = JsonSerializer.Deserialize<IEnumerable<Address>>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return addresses ?? new List<Address>();
        }


         public async Task<UserAdress?> GetAdressInfoAsync(int? userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);

            var quer =  await connection.QueryAsync<UserAdress>("select * from get_insured_address(@UserId)", new { userId });
            return quer.FirstOrDefault();
        }

         public async Task<IEnumerable<PeriodDate?>> GetDownPeriodAsync(int? userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);

            var quer =  await connection.QueryAsync<PeriodDate>("select * from get_down_periods(@UserId)", new { userId });
            return quer.ToList();
        }
        public async Task<IEnumerable<ExcludePeriod>>? GetExclPeriodAsync(int? userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);

            var quer =  await connection.QueryAsync<ExcludePeriod>("select * from get_exclude_periods(@UserId)", new { userId });
            return quer.ToList();
        }
        public void UpdateAdressInfoAsync(int? userId, string? flat, string? guid)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var quer =  "select update_insured_address(@UserId,@flat,@guid)";
            connection.Execute(quer, new { userId, flat, guid } );
        }


        public void DeleteExcludePreiod(int? userId, DateTime? date1, DateTime? date2)
        {
            using var connection = new NpgsqlConnection(_connectionString);

            var query = "SELECT delete_exclude_period(@UserId, @date1::DATE, @date2::DATE)";

            connection.Execute(query, new 
            { 
                UserId = userId, 
                date1 = date1?.Date,
                date2 = date2?.Date
            });
        }

        public void DeleteDownPreiod(int? userId, DateTime? date1, DateTime? date2)
        {
            using var connection = new NpgsqlConnection(_connectionString);

            var query = "SELECT delete_down_period(@UserId, @date1::DATE, @date2::DATE)";

            connection.Execute(query, new 
            { 
                UserId = userId, 
                date1 = date1?.Date,
                date2 = date2?.Date
            });
        }

        public void AddDownPreiod(int? userId, DateTime? date1, DateTime? date2, int option)
        {
            using var connection = new NpgsqlConnection(_connectionString);

            var query = "SELECT add_down_period(@UserId, @date1::DATE, @date2::DATE, @oaption)";

            connection.Execute(query, new 
            { 
                UserId = userId, 
                date1 = date1?.Date,
                date2 = date2?.Date,
                oaption = option
            });
        }
        public void AddExcludePreiod(int? userId, DateTime? date1, DateTime? date2, int option)
        {
            using var connection = new NpgsqlConnection(_connectionString);

            var query = "SELECT add_exclude_period(@UserId, @oaption, @date1::DATE, @date2::DATE)";

            connection.Execute(query, new 
            { 
                UserId = userId, 
                oaption = option,
                date1 = date1?.Date,
                date2 = date2?.Date
            });
        }
        public void UpdateDownPeriod(int? userId, DateTime? olddate1, DateTime? olddate2, int oldoption, DateTime? newdate1, DateTime? newdate2, int newoption)
        {
            using var connection = new NpgsqlConnection(_connectionString);

            var query = "SELECT update_down_period(@UserId, @OldDate1::DATE, @OldDate2::DATE, @OldOption, @NewDate1::DATE, @NewDate2::DATE, @NewOption)";

            connection.Execute(query, new 
            { 
                UserId = userId, 
                OldDate1 = olddate1?.Date,
                OldDate2 = olddate2?.Date,
                OldOption = oldoption,
                NewDate1 = newdate1?.Date,
                NewDate2 = newdate2?.Date,
                NewOption = newoption
            });
        }
       public void UpdateExcludePeriod(int userId, int oldType, DateTime oldBegin, DateTime oldEnd, int newType, DateTime newBegin, DateTime newEnd)
        {
            using var connection = new NpgsqlConnection(_connectionString);

            var query = "SELECT update_exclude_period(@UserId, @OldType, @OldBegin::DATE, @OldEnd::DATE, @NewType, @NewBegin::DATE, @NewEnd::DATE)";

            connection.Execute(query, new 
            { 
                UserId = userId, 
                OldType = oldType,
                OldBegin = oldBegin,
                OldEnd = oldEnd,
                NewType = newType,
                NewBegin = newBegin,
                NewEnd = newEnd
            });
        }

    }
}