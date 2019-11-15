using System;
using System.Threading.Tasks;
using GoTCBot.Models;

namespace GoTCBot.Access.Abstractions
{
    public interface IHouseAccess
    {
        Task<House> GetHouse(string identifier);

        Task<House> CreateHouse(string name, string identifier, string leaderIdentifier);
    }
}
