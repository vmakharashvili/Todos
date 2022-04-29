using Todos.Core.Dtos.Users;
using Todos.Core.Exceptions;
using Todos.Core.Extensions;
using Todos.Core.Repositories;
using Todos.Core.Mappers;
using Todos.Core.Models;

namespace Todos.Core.Services;

public class UserService
{
    private readonly Settings _settings;
    private readonly IUserRepository _userRepository;

    public UserService(Settings settings, IUserRepository userRepository)
    {
        _settings = settings;
        _userRepository = userRepository;
    }

    public async Task<UserDto> Create(CreateUserDto model)
    {
        if (string.IsNullOrWhiteSpace(model.Username))
        {
            throw new DomainException("Username Required");
        }

        if (string.IsNullOrWhiteSpace(model.Password))
        {
            throw new DomainException("Password Required");
        }

        var sameuser = await _userRepository.GetByName(model.Username);
        if (sameuser != null)
        {
            throw new DomainException("Such username is already taken. Use different one");
        }

        var user = new User()
        {
            Username = model.Username,
            PasswordHash = model.Password.Hash256(_settings.SecretKey),
            IsActive = true
        };
        user = await _userRepository.Create(user);
        return user.ToDto();
    }

    public async Task<UserDto> LogIn(LogInDto model)
    {
        if (string.IsNullOrWhiteSpace(model.Username))
        {
            throw new DomainException("Username Required");
        }

        if (string.IsNullOrWhiteSpace(model.Password))
        {
            throw new DomainException("Password Required");
        }

        var user = await _userRepository.GetByName(model.Username);
        if (user == null)
        {
            throw new DomainException("Such User doesn't exist");
        }

        if (!user.IsActive)
        {
            throw new DomainException("User not active");
        }

        if (user.PasswordHash == model.Password.Hash256(_settings.SecretKey))
        {
            return user.ToDto();
        }

        throw new DomainException("Password not correct");
    }

    public async Task<UserDto> GetUserByName(string userName)
    {
        var user = await _userRepository.GetByName(userName);
        return user.ToDto();
    }

    public async Task<IEnumerable<UserDto>> GetAllUsers()
    {
        var users = await _userRepository.GetAll();
        return users.ToDtos();
    }
}