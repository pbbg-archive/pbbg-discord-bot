using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using GoTCBot.Access.Abstractions;
using GoTCBot.Models;

namespace GoTCBot.Access
{
    public class PlayerAccess : IPlayerAccess
    {
        private static string connectionString = "Server=tcp:gotcdiscordbot.database.windows.net,1433;Initial Catalog=discord;Persist Security Info=False;User ID=dhudy;Password=HNWXfmw1yy2AsyU3U67PP0a7ofiz0nYC;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public async Task<User> GetUser(string identifier)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return await db.QueryFirstOrDefaultAsync<User>($"Select * From Player WHERE Identifier = '{identifier}'");
            }
        }

        public async Task<User> GetUserById(int userId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return await db.QueryFirstOrDefaultAsync<User>($"Select * From Player WHERE Player_ID = '{userId}'");
            }
        }

        public async Task<User> Create(string identifier, string name)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return await db.QueryFirstAsync<User>($"INSERT INTO Player (Name, Identifier, CreatedAt) VALUES ('{name}', '{identifier}', GETDATE()); " +
                    "SELECT * FROM Player WHERE Player_ID = SCOPE_IDENTITY()");
            }
        }

        public async Task<int> AssignToHouse(int houseId, int roleId, int userId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return await db.ExecuteAsync($"UPDATE Player SET House_FK = {houseId}, Role_FK = {roleId} WHERE Player_ID = {userId}");
            }
        }

        public async Task<int> RemoveFromHouse(int userId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return await db.ExecuteAsync($"UPDATE Player SET House_FK = NULL, Role_FK = NULL WHERE Player_ID = {userId}");
            }
        }
    }
}
