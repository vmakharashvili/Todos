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
    private readonly TodoItem _savedTodoItem;
    private readonly User _user;

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
    public void Create_ShouldThrow_WhenDescriptionIsNull()
    {
        var dto = new CreateTodoItemDto() { };
        var result = () => _sut.Create(dto);
        result.Should().ThrowAsync<DomainException>("Description required");
    }

    [Fact]
    public void Create_ShouldThrow_WhenParentIdDoesnotExistInRepository()
    {
        var dto = new CreateTodoItemDto() { Description = _faker.Lorem.Sentence(), ParentId = _faker.Random.Guid() };
        _todoItemsRepository.GetById(dto.ParentId.Value).Returns(Task.FromResult<TodoItem?>(null));
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

    [Fact]
    public void Update_ShouldThrow_WhenTodoItemNotInRepository()
    {
        _todoItemsRepository.GetById(_savedTodoItem.Id).Returns(Task.FromResult<TodoItem?>(null));
        var dto = new UpdateTodoItemDto() { Id = _savedTodoItem.Id, Description = _faker.Lorem.Sentence() };
        var result = () => _sut.Update(dto);
        result.Should().ThrowAsync<DomainException>($"No such TodoItem with id {_savedTodoItem.Id}");
    }

    [Fact]
    public async Task Update_ShouldCompleteSuccessfully_WhenItemIsInRepositoryAndValid()
    {
        var faker2 = new Faker();
        var previousDescription = _savedTodoItem.Description;
        _todoItemsRepository.GetById(_savedTodoItem.Id).Returns(_savedTodoItem);
        var dto = new UpdateTodoItemDto() { Id = _savedTodoItem.Id, Description = faker2.Lorem.Sentence() };
        await _sut.Update(dto);
        await _todoItemsRepository.Received(1).Update(Arg.Is<TodoItem>(x => x.Description != previousDescription && x.Description == dto.Description));
    }

    [Fact]
    public void Create_ShouldThrow_WhenStartTimeIsInPast()
    {
        var dto = new CreateTodoItemDto() { Description = _faker.Lorem.Sentence(), StartTime = _faker.Date.Past() };
        var result = () => _sut.Create(dto);
        result.Should().ThrowAsync<DomainException>("Start time can't be in the past");
    }

    [Fact]
    public void Create_ShouldThrow_WhenEndtimeIsBeforeStartTime()
    {
        var dto = new CreateTodoItemDto { 
            Description = _faker.Lorem.Sentence(), 
            EndTime = _faker.Date.Between(DateTime.Now, 
            _faker.Date.Future()), 
            StartTime=_faker.Date.Future() 
        };
        var result = () => _sut.Create(dto);
        result.Should().ThrowAsync<DomainException>("End time can't be prior to start time");
    }

    [Fact]
    public void Create_ShouldThrow_WhenStarttimeIsNullAndEndtimeIsInPast()
    {
        var dto = new CreateTodoItemDto
        {
            Description = _faker.Lorem.Sentence(),
            EndTime = _faker.Date.Past()
        };
        var result = () => _sut.Create(dto);
        result.Should().ThrowAsync<DomainException>("End time can't be in the past");
    }

    [Fact]
    public void MakeItDone_ShouldThrow_WhenTodoItemDoesnotExist()
    {
        var randomGuid = _faker.Random.Guid();
        var result = () => _sut.MakeItDone(randomGuid, true);
        result.Should().ThrowAsync<DomainException>($"No such TodoItem with id {randomGuid}");
    }

    [Fact]
    public async void MakeItDonw_ShouldSucceed_WhenTodoItemExists()
    {
        var approval = _faker.Random.Bool();
        _todoItemsRepository.GetById(_savedTodoItem.Id).Returns(_savedTodoItem);
        await _sut.MakeItDone(_savedTodoItem.Id, approval);
        await _todoItemsRepository.Received(1).Update(Arg.Is<TodoItem>(x => x.Id == _savedTodoItem.Id && x.IsDone == approval));
    }
}