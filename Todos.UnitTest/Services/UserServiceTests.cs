using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using NSubstitute;
using Todos.Core.Dtos.Users;
using Todos.Core.Exceptions;
using Todos.Core.Extensions;
using Todos.Core.Models;
using Todos.Core.Repositories;
using Todos.Core.Services;
using Xunit;

namespace Todos.UnitTest.Services;

public class UserServiceTests
{
    private readonly UserService _sut;
    private readonly Faker _faker;
    private readonly IUserRepository _userRepository;
    private readonly Settings _settings;
    private readonly User _savedUser;
    private readonly string _savedUserPassword;

    public UserServiceTests()
    {
        _faker = new Faker();
        _userRepository = Substitute.For<IUserRepository>();
        _settings = new Settings()
            { SecretKey = _faker.Random.Hash(), Jwt = new Jwt() { Secret = _faker.Random.Hash() } };
        _sut = new UserService(_settings, _userRepository);
        _savedUserPassword = _faker.Random.Word();
        _savedUser = new User()
        {
            Id = _faker.Random.Int(), Username = _faker.Person.UserName, IsActive = true,
            PasswordHash = _savedUserPassword.Hash256(_settings.SecretKey)
        };
        _userRepository.GetByName(_savedUser.Username).Returns(Task.FromResult(_savedUser));
    }

    [Fact]
    public void Create_ShouldThrow_WhenUsernameOrPasswordIsNull()
    {
        var result = () => _sut.Create(new CreateUserDto());
        result.Should().ThrowAsync<DomainException>("Username Required");

        result = () => _sut.Create(new CreateUserDto() { Username = _savedUser.Username });
        result.Should().ThrowAsync<DomainException>("Password Required");
    }

    [Fact]
    public void Create_ShouldThrow_WhenUsernameAlreadyExists()
    {
        var otherUser = new User()
        {
            Id = _faker.Random.Int(1, 100), Username = _faker.Random.Word(), IsActive = true
        };
        _userRepository.GetByName(otherUser.Username).Returns(Task.FromResult(otherUser));
        var result = () => _sut.Create(
            new CreateUserDto{ Username = otherUser.Username, Password = _faker.Random.Word() });
        result.Should().ThrowAsync<DomainException>("Such username is already taken. Use different one");
    }

    [Fact]
    public async Task Create_ShouldCreateSuccessfully_WhenUsernameAndPasswordExists()
    {
        var faker = new Faker();
        var cmd = new CreateUserDto() { Username = faker.Person.UserName, Password = faker.Random.Word() };
        var savedUser = new User
        {
            Id = faker.Random.Int(), Username = cmd.Username, PasswordHash = cmd.Password.Hash256(_settings.SecretKey),
            IsActive = true
        };
        _userRepository.Create(Arg.Is<User>(x => x.Username == cmd.Username)).Returns(Task.FromResult(savedUser));
        var result = await _sut.Create(cmd);
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(new UserDto
            { Username = cmd.Username, IsActive = true, Id = savedUser.Id });
    }

    [Fact]
    public void LogIn_ShouldThrow_WhenUsernameOrPasswordIsNull()
    {
        var cmd = new LogInDto();
        var result = () => _sut.LogIn(cmd);
        result.Should().ThrowAsync<DomainException>("Username Required");

        cmd = cmd with { Username = _savedUser.Username };
        result = () => _sut.LogIn(cmd);
        result.Should().ThrowAsync<DomainException>("Password Required");
    }

    [Fact]
    public void LogIn_ShouldThrow_WhenUserDoesnotExist()
    {
        var _faker = new Faker();
        var cmd = new LogInDto() { Username = _faker.Person.UserName, Password = _savedUserPassword };
        var result = () => _sut.LogIn(cmd);
        result.Should().ThrowAsync<DomainException>("Such User doesn't exist");
    }

    [Fact]
    public void LogIn_ShouldThrow_WhenUserIsNotActive()
    {
        var faker = new Faker();
        var inactiveUser = new User { Id = faker.Random.Int(), Username = faker.Person.UserName, IsActive = false };
        _userRepository.GetByName(inactiveUser.Username).Returns(Task.FromResult(inactiveUser));
        var result = () => _sut.LogIn(new LogInDto()
            { Username = inactiveUser.Username, Password = faker.Random.Word() });
        result.Should().ThrowAsync<DomainException>("User not active");
    }

    [Fact]
    public void LogIn_ShouldThrow_WhenPasswordNotCorrect()
    {
        var logInDto = new LogInDto()
            { Username = _savedUser.Username, Password = _faker.Random.Word() };
        var result = () => _sut.LogIn(logInDto);
        result.Should().ThrowAsync<DomainException>("Password not correct");
    }

    [Fact]
    public async Task LogIn_ShouldSucceed_WhenUsernameAndPasswordIsCorrect()
    {
        var logInDto = new LogInDto { Username = _savedUser.Username, Password = _savedUserPassword };
        var result = await _sut.LogIn(logInDto);
        result.Should().NotBeNull();
        result.Username.Should().Be(_savedUser.Username);
    }

    [Fact]
    public async Task GetUserByName_ShouldReturnNull_WhenUserDoesnotExist()
    {
        var faker = new Faker();
        var result = await _sut.GetUserByName(faker.Person.UserName);
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUserByName_ShouldReturnSuccessfully_WhenUsernameExists()
    {
        var result = await _sut.GetUserByName(_savedUser.Username);
        result.Should().NotBeNull();
        result.Username.Should().Be(_savedUser.Username);
        result.Id.Should().Be(_savedUser.Id);
        result.IsActive.Should().Be(_savedUser.IsActive);
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnOneSuccessfully_WhenOnlyOneInRepository()
    {
        _userRepository.GetAll().Returns(Task.FromResult(
            new List<User>{ _savedUser}.AsEnumerable()
        ));
        var result = await _sut.GetAllUsers();
        result.Should().NotBeNull();
        result.Count().Should().Be(1);
        result.First().Username.Should().Be(_savedUser.Username);
    }
}