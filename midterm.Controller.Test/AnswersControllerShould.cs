using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using midTerm.Controllers;
using midTerm.Models.Models.Answers;
using midTerm.Services.Abstractions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace midterm.Controller.Test
{
    public class AnswersControllerShould
    {
        private readonly Mock<IAnswerService> _mockService;
        private readonly AnswersController _controller;
        public AnswersControllerShould()
        {
            _mockService = new Mock<IAnswerService>();
            _controller = new AnswersController(_mockService.Object);
        }
        [Fact]
        public async Task ReturnExtendedAnswerByIdWhenHasData()
        {
            // Arrange
            int expectedId = 1;
            var user = new Faker<AnswersExtended>()
                .RuleFor(s => s.Id, v => ++v.IndexVariable)
                .RuleFor(s => s.OptionId, v => v.Random.Int())
                .RuleFor(s => s.UserId, v => v.Random.Int())
                .Generate(3);

            _mockService.Setup(x => x.GetById(It.IsAny<int>()))
                .ReturnsAsync(user.Find(x => x.Id == expectedId))
                .Verifiable();

            // Act
            var result = await _controller.GetById(expectedId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var model = result as OkObjectResult;
            model?.Value.Should().BeOfType<AnswersExtended>().Subject.Id.Should().Be(expectedId);
        }
        [Fact]
        public async Task ReturnNoContentWhenHasNoData()
        {
            int expectedId = 1;
            var user = new Faker<AnswersExtended>()
                .RuleFor(s => s.Id, v => v.IndexVariable)
                .RuleFor(s => s.OptionId, v => v.Random.Int())
                .RuleFor(s => s.UserId, v => v.Random.Int())
                .Generate(3);

            _mockService.Setup(x => x.GetById(It.IsAny<int>()))
                .ReturnsAsync(user.Find(x => x.Id == expectedId))
                .Verifiable();

            // Act
            var result = await _controller.GetById(expectedId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }
        [Fact]
        public async Task ReturnUsersWhenHasData()
        {
            int expectedCount = 10;
            var users = new Faker<AnswersBaseModel>()
                .RuleFor(s => s.Id, v => ++v.IndexVariable)
               .RuleFor(s => s.OptionId, v => v.Random.Int())
                .RuleFor(s => s.UserId, v => v.Random.Int())
                .Generate(expectedCount);

            _mockService.Setup(x => x.Get())
                .ReturnsAsync(users)
                .Verifiable();

            var result = await _controller.Get();
            result.Should().BeOfType<OkObjectResult>();

            var model = result as OkObjectResult;
            model?.Value.Should().BeOfType<List<AnswersBaseModel>>().Subject.Count().Should().Be(expectedCount);
        }
        [Fact]
        public async Task ReturnEmptyListWhenHasNoData()
        {
            // Arrange
            var users = new Faker<AnswersBaseModel>()
                .RuleFor(s => s.Id, v => v.IndexVariable)
                .RuleFor(s => s.OptionId, v => v.Random.Int())
                .RuleFor(s => s.UserId, v => v.Random.Int())
                .Generate(0);

            _mockService.Setup(x => x.Get())
                .ReturnsAsync(users)
                .Verifiable();

            // Act
            var result = await _controller.Get();

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }
        [Fact]
        public async Task ReturnCreatedUserOnCreateWhenCorrectModel()
        {
            // Arrange
            var user = new Faker<AnswerCreateModel>()
                .RuleFor(s => s.OptionId, v => v.Random.Int())
                .RuleFor(s => s.UserId, v => v.Random.Int())
                .Generate();
            var expected = new Faker<AnswersBaseModel>()
                .RuleFor(s => s.Id, 1)
                .RuleFor(s => s.OptionId, v => v.Random.Int())
                .RuleFor(s => s.UserId, v => v.Random.Int())
                .Generate();

            _mockService.Setup(x => x.Insert(It.IsAny<AnswerCreateModel>()))
                .ReturnsAsync(expected)
                .Verifiable();

            // Act
            var result = await _controller.Post(user);

            // Assert
            result.Should().BeOfType<CreatedAtRouteResult>();

            var model = result as CreatedResult;
            model?.Value.Should().Be(1);
            model?.Location.Should().Be("/api/SurveyUser/1");
        }
        [Fact]
        public async Task ReturnConflictOnCreateWhenRepositoryError()
        {
            // Arrange
            var player = new Faker<AnswerCreateModel>()
                 .RuleFor(s => s.OptionId, v => v.Random.Int())
                .RuleFor(s => s.UserId, v => v.Random.Int())
                .Generate();

            _mockService.Setup(x => x.Insert(It.IsAny<AnswerCreateModel>()))
                .ReturnsAsync(() => null)
                .Verifiable();

            // Act
            var result = await _controller.Post(player);

            // Assert
            result.Should().BeOfType<ConflictResult>();
        }
        [Fact]
        public async Task ReturnBadRequestOnCreateWhenModelNotValid()
        {
            // Arrange
            _controller.ModelState.AddModelError("FakeError", "fakeError message");
            var user = new Faker<AnswerCreateModel>()
                .RuleFor(s => s.OptionId, v => v.Random.Int())
                .RuleFor(s => s.UserId, v => v.Random.Int())
                .Generate();
            var expected = new Faker<AnswersBaseModel>()
                .RuleFor(s => s.Id, 1)
                 .RuleFor(s => s.OptionId, v => v.Random.Int())
                .RuleFor(s => s.UserId, v => v.Random.Int())
                .Generate();

            _mockService.Setup(x => x.Insert(It.IsAny<AnswerCreateModel>()))
                .ReturnsAsync(expected)
                .Verifiable();

            // Act
            var result = await _controller.Post(user);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }
        [Fact]
        public async Task ReturnBadRequestOnUpdateWhenModelNotValid()
        {
            // Arrange
            _controller.ModelState.AddModelError("FakeError", "fakeError message");
            var user = new Faker<AnswersUpdateModel>()
                .RuleFor(s => s.Id, 1)
                .RuleFor(s => s.OptionId, v => v.Random.Int())
                .RuleFor(s => s.UserId, v => v.Random.Int())
                .Generate();
            var expected = new Faker<AnswersBaseModel>()
                .RuleFor(s => s.Id, 1)
                .RuleFor(s => s.OptionId, v => v.Random.Int())
                .RuleFor(s => s.UserId, v => v.Random.Int())
                .Generate();

            _mockService.Setup(x => x.Update(It.IsAny<AnswersUpdateModel>()))
                .ReturnsAsync(expected)
                .Verifiable();

            // Act
            var result = await _controller.Put(user.Id, user);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }
        [Fact]
        public async Task ReturnUserOnUpdateWhenCorrectModel()
        {
            // Arrange
            var user = new Faker<AnswersUpdateModel>()
                .RuleFor(s => s.Id, 1)
                .RuleFor(s => s.OptionId, v => v.Random.Int())
                .RuleFor(s => s.UserId, v => v.Random.Int())
                .Generate();
            var expected = new Faker<AnswersBaseModel>()
                .RuleFor(s => s.Id, 1)
                .RuleFor(s => s.OptionId, v => v.Random.Int())
                .RuleFor(s => s.UserId, v => v.Random.Int())
                .Generate();

            _mockService.Setup(x => x.Update(It.IsAny<AnswersUpdateModel>()))
                .ReturnsAsync(expected)
                .Verifiable();

            // Act
            var result = await _controller.Put(user.Id, user);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var model = result as OkObjectResult;
            model?.Value.Should().Be(expected);
        }
        [Fact]
        public async Task ReturnNoContentOnUpdateWhenRepositoryError()
        {
            // Arrange
            var player = new Faker<AnswersUpdateModel>()
                .RuleFor(s => s.Id, 1)
                .RuleFor(s => s.OptionId, v => v.Random.Int())
                .RuleFor(s => s.UserId, v => v.Random.Int())
                .Generate();
            var expected = new Faker<AnswersBaseModel>()
                .RuleFor(s => s.Id, 1)
                .RuleFor(s => s.OptionId, v => v.Random.Int())
                .RuleFor(s => s.UserId, v => v.Random.Int())
                .Generate();

            _mockService.Setup(x => x.Update(It.IsAny<AnswersUpdateModel>()))
                .ReturnsAsync(() => null)
                .Verifiable();

            // Act
            var result = await _controller.Put(player.Id, player);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }
        [Fact]
        public async Task ReturnOkWhenDeletedData()
        {
            // Arrange
            int id = 1;
            bool expected = true;

            _mockService.Setup(x => x.Delete(It.IsAny<int>()))
                .ReturnsAsync(expected)
                .Verifiable();

            // Act
            var result = await _controller.Delete(id);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var model = result as OkObjectResult;
            model?.Value.Should().Be(expected);
        }
        [Fact]
        public async Task ReturnOkFalseWhenNoData()
        {
            // Arrange
            int id = 1;
            bool expected = false;

            _mockService.Setup(x => x.Delete(It.IsAny<int>()))
                .ReturnsAsync(expected)
                .Verifiable();

            // Act
            var result = await _controller.Delete(id);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var model = result as OkObjectResult;
            model?.Value.Should().Be(expected);
        }
        [Fact]
        public async Task ReturnBadResultWhenModelNotValid()
        {
            // Arrange
            _controller.ModelState.AddModelError("FakeError", "fakeError message");

            // Act
            var result = await _controller.Delete(1);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }
    }
}
