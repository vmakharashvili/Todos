using Todos.Core.Dtos.Users;
using Todos.Core.Models;

namespace Todos.Core.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(this User user) => new UserDto()
    {
        Id = user.Id,
        Username = user.Username,
        IsActive = user.IsActive
    };

    public static IEnumerable<UserDto> ToDtos(this IEnumerable<User> users) =>
        users.Select(x => x.ToDto());
}