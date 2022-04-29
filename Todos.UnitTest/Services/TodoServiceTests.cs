using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using NSubstitute;
using Todos.Core.Dtos.Todos;
using Todos.Core.Exceptions;
using Todos.Core.Models;
using Todos.Core.Repositories;
using Todos.Core.Services;
using Todos.Core.Services.Abstract;
using Xunit;

namespace Todos.UnitTest.Services;

public class TodoServiceTests
{
    private readonly TodoService _sut;
    private readonly ITodoItemsRepository _todoItemsRepository;
    private readonly IAuthService _authService;
    private readonly Faker _faker;
    private TodoItem _savedTodoItem;
    private User _user;

    public TodoServiceTests()
    {
        _todoItemsRepository = Substitute.For<ITodoItemsRepository>();
        _authService = Substitute.For<IAuthService>();
        _sut = new TodoService(_todoItemsRepository, _authService);
        _faker = new Faker();
        _user = new User()
        {
            Id = _faker.Random.Int(1),
            Username = _faker.Person.UserName,
            IsActive = true
        };
        _savedTodoItem = new TodoItem()
        {
            Id = _faker.Random.Guid(),
            Description = _faker.Lorem.Sentence(),
            CreatedAt = _faker.Date.Past(),
            EndTime = null,
            IsDone = false,
            ParentId = null,
            StartTime = _faker.Date.Future(),
            UserId = _user.Id
        };
        _authService.GetUserId().Returns(_user.Id);
    }

    [Fact]
    public async Task GetMyTodoItems_ShouldReturnEmpty_WhenUserHasNoItems()
    {
        _todoItemsRepository.GetByUserId(_user.Id).Returns(Task.FromResult(Enumerable.Empty<TodoItem>()));
        var result = await _sut.GetMyTodoItems();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetMyTodoItems_ShouldReturnOne_WhenUserHasOneTodoItem()
    {
        _todoItemsRepository.GetByUserId(_user.Id).Returns(new List<TodoItem>() { _savedTodoItem }.AsEnumerable());
        var result = await _sut.GetMyTodoItems();
        result.Should().NotBeEmpty();
        result.Count().Should().Be(1);
        result.First().Id.Should().Be(_savedTodoItem.Id);
    }

    [Fact]
    public async Task Create_ShouldThrow_WhenDescriptionIsNull()
    {
        var dto = new CreateTodoItemDto() { };
        var result = () => _sut.Create(dto);
        result.Should().ThrowAsync<DomainException>("Description required");
    }

    [Fact]
    public async Task Create_ShouldThrow_WhenParentIdDoesnotExistInRepository()
    {
        var dto = new CreateTodoItemDto() { Description = _faker.Lorem.Sentence(), ParentId = _faker.Random.Guid() };
        _todoItemsRepository.GetById(dto.ParentId.Value).Returns(Task.FromResult<TodoItem>(null));
        var result = () => _sut.Create(dto);
        result.Should().ThrowAsync<DomainException>($"No TodoItem with id {dto.ParentId.Value}");
    }

    [Fact]
    public async Task Create_ShouldCompleteSuccessfully_WhenDescriptionIsNotNullAndParentIdIsNull()
    {
        var dto = new CreateTodoItemDto() { Description = _faker.Lorem.Sentence() };
        _todoItemsRepository.Create(Arg.Any<TodoItem>()).Returns(args => (TodoItem)args[0]);
        var result = await _sut.Create(dto);
        result.Should().NotBeNull();
        result.Description.Should().Be(dto.Description);
    }
}