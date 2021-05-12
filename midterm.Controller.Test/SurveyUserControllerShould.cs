using Bogus;
using midTerm.Controllers;
using midTerm.Models.Models.Answers;
using midTerm.Models.Models.SurveyUser;
using midTerm.Services.Abstractions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace midterm.Controller.Test
{
    public class SurveyUserControllerShould
    {
        private readonly Mock<ISurveyUserService> _mockService;
        private readonly SurveyUserController _controller;
        public SurveyUserControllerShould()
        {
            _mockService = new Mock<ISurveyUserService>();
            _controller = new SurveyUserController(_mockService.Object);
        }
        [Fact]
        public async Task ReturnExtendedUserByIdWhenHasData()
        {
            // Arrange
            int expectedId = 1;
            var user = new Faker<SurveyUserExtended>()
                .RuleFor(s => s.Id, v => ++v.IndexVariable)
                .RuleFor(s => s.Country, v => v.Address.FullAddress())
                .RuleFor(s => s.FirstName, v => v.Name.FirstName())
                .RuleFor(s => s.LastName, v => v.Name.LastName())
                .RuleFor(s => s.DoB, v => v.Date.Past(30))
                .Generate(3);

            _mockService.Setup(x => x.GetById(It.IsAny<int>()))
                .ReturnsAsync(user.Find(x => x.Id == expectedId))
                .Verifiable();

            // Act
            var result = await _controller.GetById(expectedId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var model = result as OkObjectResult;
            model?.Value.Should().BeOfType<SurveyUserExtended>().Subject.Id.Should().Be(expectedId);
        }
        [Fact]
        public async Task ReturnNoContentWhenHasNoData()
        {
            int expectedId = 1;
            var user = new Faker<SurveyUserExtended>()
                .RuleFor(s => s.Id, v => v.IndexVariable)
                .RuleFor(s => s.Country, v => v.Address.FullAddress())
                .RuleFor(s => s.FirstName, v => v.Name.FirstName())
                .RuleFor(s => s.LastName, v => v.Name.LastName())
                .RuleFor(s => s.DoB, v => v.Date.Past(30))
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
            var users = new Faker<SurveyUserBaseModel>()
                .RuleFor(s => s.Id, v => ++v.IndexVariable)
                .RuleFor(s => s.Country, v => v.Address.FullAddress())
                .RuleFor(s => s.FirstName, v => v.Name.FirstName())
                .RuleFor(s => s.LastName, v => v.Name.LastName())
                .RuleFor(s => s.DoB, v => v.Date.Past(30))
                .Generate(expectedCount);

            _mockService.Setup(x => x.Get())
                .ReturnsAsync(users)
                .Verifiable();

            var result = await _controller.Get();
            result.Should().BeOfType<OkObjectResult>();

            var model = result as OkObjectResult;
            model?.Value.Should().BeOfType<List<SurveyUserBaseModel>>().Subject.Count().Should().Be(expectedCount);
        }
        [Fact]
        public async Task ReturnEmptyListWhenHasNoData()
        {
            // Arrange
            var users = new Faker<SurveyUserBaseModel>()
                .RuleFor(s => s.Id, v => ++v.IndexVariable)
                .RuleFor(s => s.Country, v => v.Address.FullAddress())
                .RuleFor(s => s.FirstName, v => v.Name.FirstName())
                .RuleFor(s => s.LastName, v => v.Name.LastName())
                .RuleFor(s => s.DoB, v => v.Date.Past(30))
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
            var user = new Faker<SurveyUserCreate>()
                .RuleFor(s => s.DoB, v => v.Date.Between(DateTime.Today.AddYears(-18), DateTime.Today.AddYears(-23)))
                .RuleFor(s => s.FirstName, v => v.Name.FirstName())
                .RuleFor(s => s.LastName, v => v.Name.LastName())
                .RuleFor(s => s.Country, v => v.Address.FullAddress())
                .Generate();
            var expected = new Faker<SurveyUserBaseModel>()
                .RuleFor(s => s.Id, 1)
                .RuleFor(s => s.DoB, v => v.Date.Between(DateTime.Today.AddYears(-18), DateTime.Today.AddYears(-23)))
                .RuleFor(s => s.FirstName, v => v.Name.FirstName())
                .RuleFor(s => s.LastName, v => v.Name.LastName())
                .RuleFor(s => s.Country, v => v.Address.FullAddress())
                .Generate();

            _mockService.Setup(x => x.Insert(It.IsAny<SurveyUserCreate>()))
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
            var player = new Faker<SurveyUserCreate>()
                .RuleFor(s => s.DoB, v => v.Date.Between(DateTime.Today.AddYears(-18), DateTime.Today.AddYears(-23)))
                .RuleFor(s => s.FirstName, v => v.Name.FirstName())
                .RuleFor(s => s.LastName, v => v.Name.LastName())
                .RuleFor(s => s.Country, v => v.Address.FullAddress())
                .Generate();

            _mockService.Setup(x => x.Insert(It.IsAny<SurveyUserCreate>()))
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
            var user = new Faker<SurveyUserCreate>()
                .RuleFor(s => s.DoB, v => v.Date.Between(DateTime.Today.AddYears(-18), DateTime.Today.AddYears(-23)))
                .RuleFor(s => s.FirstName, v => v.Name.FirstName())
                .RuleFor(s => s.LastName, v => v.Name.LastName())
                .RuleFor(s => s.Country, v => v.Address.FullAddress())
                .Generate();
            var expected = new Faker<SurveyUserBaseModel>()
                .RuleFor(s => s.Id, 1)
                .RuleFor(s => s.DoB, v => v.Date.Between(DateTime.Today.AddYears(-18), DateTime.Today.AddYears(-23)))
                .RuleFor(s => s.FirstName, v => v.Name.FirstName())
                .RuleFor(s => s.LastName, v => v.Name.LastName())
                .RuleFor(s => s.Country, v => v.Address.FullAddress())
                .Generate();

            _mockService.Setup(x => x.Insert(It.IsAny<SurveyUserCreate>()))
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
            var user = new Faker<SurveyUserUpdate>()
                .RuleFor(s => s.Id, 1)
                .RuleFor(s => s.DoB, v => v.Date.Between(DateTime.Today.AddYears(-18), DateTime.Today.AddYears(-23)))
                .RuleFor(s => s.FirstName, v => v.Name.FirstName())
                .RuleFor(s => s.LastName, v => v.Name.LastName())
                .RuleFor(s => s.Country, v => v.Address.FullAddress())
                .Generate();
            var expected = new Faker<SurveyUserBaseModel>()
                .RuleFor(s => s.Id, 1)
                .RuleFor(s => s.DoB, v => v.Date.Between(DateTime.Today.AddYears(-18), DateTime.Today.AddYears(-23)))
                .RuleFor(s => s.FirstName, v => v.Name.FirstName())
                .RuleFor(s => s.LastName, v => v.Name.LastName())
                .RuleFor(s => s.Country, v => v.Address.FullAddress())
                .Generate();

            _mockService.Setup(x => x.Update(It.IsAny<SurveyUserUpdate>()))
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
            var user = new Faker<SurveyUserUpdate>()
                .RuleFor(s => s.Id, 1)
                .RuleFor(s => s.DoB, v => v.Date.Between(DateTime.Today.AddYears(-18), DateTime.Today.AddYears(-23)))
                .RuleFor(s => s.FirstName, v => v.Name.FirstName())
                .RuleFor(s => s.LastName, v => v.Name.LastName())
                .RuleFor(s => s.Country, v => v.Address.FullAddress())
                .Generate();
            var expected = new Faker<SurveyUserBaseModel>()
                .RuleFor(s => s.Id, 1)
                .RuleFor(s => s.DoB, v => v.Date.Between(DateTime.Today.AddYears(-18), DateTime.Today.AddYears(-23)))
                .RuleFor(s => s.FirstName, v => v.Name.FirstName())
                .RuleFor(s => s.LastName, v => v.Name.LastName())
                .RuleFor(s => s.Country, v => v.Address.FullAddress())
                .Generate();

            _mockService.Setup(x => x.Update(It.IsAny<SurveyUserUpdate>()))
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
            var player = new Faker<SurveyUserUpdate>()
                .RuleFor(s => s.Id, 1)
                .RuleFor(s => s.DoB, v => v.Date.Between(DateTime.Today.AddYears(-18), DateTime.Today.AddYears(-23)))
                .RuleFor(s => s.FirstName, v => v.Name.FirstName())
                .RuleFor(s => s.LastName, v => v.Name.LastName())
                .RuleFor(s => s.Country, v => v.Address.FullAddress())
                .Generate();
            var expected = new Faker<SurveyUserBaseModel>()
                .RuleFor(s => s.Id, 1)
                .RuleFor(s => s.DoB, v => v.Date.Between(DateTime.Today.AddYears(-18), DateTime.Today.AddYears(-23)))
                .RuleFor(s => s.FirstName, v => v.Name.FirstName())
                .RuleFor(s => s.LastName, v => v.Name.LastName())
                .RuleFor(s => s.Country, v => v.Address.FullAddress())
                .Generate();

            _mockService.Setup(x => x.Update(It.IsAny<SurveyUserUpdate>()))
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
