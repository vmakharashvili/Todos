using Todos.Core.Dtos.Users;
using Todos.Core.Models;

namespace Todos.Core.Services.Abstract;

public interface IAuthService
{
    void SetUser(int userId);
    int GetUserId();
    string GetUserToken(UserDto user);
}