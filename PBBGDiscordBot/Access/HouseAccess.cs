using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using GoTCBot.Access.Abstractions;
using GoTCBot.Exceptions;
using GoTCBot.Models;

namespace GoTCBot.Access
{
    public class HouseAccess : IHouseAccess
    {
        private static string connectionString = "Server=tcp:gotcdiscordbot.database.windows.net,1433;Initial Catalog=discord;Persist Security Info=False;User ID=dhudy;Password=HNWXfmw1yy2AsyU3U67PP0a7ofiz0nYC;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public HouseAccess()
        {
        }

        public async Task<House> CreateHouse(string name, string identifier, string leaderIdentifier)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                User leader = await db.QueryFirstOrDefaultAsync<User>($"SELECT * FROM Player WHERE Identifier = {leaderIdentifier}");
                if(leader == null)
                {
                    throw new UserNotFoundException("The user tagged as leader does not exist in this server!");
                }
                House check = await GetHouse(identifier);
                if(check != null)
                {
                    throw new Exception("This House already exists!");
                }
                return await db.QueryFirstAsync<House>($"INSERT INTO House (Name, Identifier, Owner_FK, CreatedAt) VALUES ('{name}', '{identifier}', {leader.Player_ID}, GETDATE()); " +
                    "SELECT * FROM House WHERE House_ID = SCOPE_IDENTITY()");
            }
        }

        public async Task<House> GetHouse(string identifier)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return await db.QueryFirstOrDefaultAsync<House>($"SELECT * FROM House WHERE Identifier = {identifier}");
            }
        }
    }
}
