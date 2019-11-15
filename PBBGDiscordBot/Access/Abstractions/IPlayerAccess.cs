using System;
using System.Threading.Tasks;
using GoTCBot.Models;

namespace GoTCBot.Access.Abstractions
{
    public interface IPlayerAccess
    {
        Task<User> GetUser(string identifier);

        Task<User> GetUserById(int userId);

        Task<int> AssignToHouse(int houseId, int roleId, int userId);

        Task<User> Create(string identifier, string name);

        Task<int> RemoveFromHouse(int userId);
    }
}
