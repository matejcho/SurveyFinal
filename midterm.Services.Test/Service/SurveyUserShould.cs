using AutoMapper;
using midterm.Services.Test.Internal;
using midTerm.Models.Profiles;
using midTerm.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using midTerm.Models.Models.SurveyUser;

namespace midterm.Services.Test.Service
{
    public class SurveyUserShould : SqlLiteContext
    {
        private readonly IMapper _mapper;
        private readonly SurveyUserService _service;
        public SurveyUserShould() : base(true)
        {
            if (_mapper == null)
            {
                var mapper = new MapperConfiguration(cfg =>
                {
                    cfg.AddMaps(typeof(SurveyUserProfile));
                }).CreateMapper();
                _mapper = mapper;
            }
            _service = new SurveyUserService(DbContext, _mapper);
        }
        [Fact]
        public async Task GetUserById()
        {
            // Arrange
            var expected = 1;

            // Act
            var result = await _service.GetById(expected);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<midTerm.Models.Models.SurveyUser.SurveyUserExtended>();
            result.Id.Should().Be(expected);
        }
        [Fact]
        public async Task GetUsers()
        {
            // Arrange
            var expected = 3;

            // Act
            var result = await _service.Get();

            // Assert
            result.Should().NotBeEmpty().And.HaveCount(expected);
            result.Should().BeAssignableTo<IEnumerable<midTerm.Models.Models.SurveyUser.SurveyUserBaseModel>>();
        }
        [Fact]
        public async Task InsertNewUser()
        {
            // Arrange
            var user = new SurveyUserCreate
            {
                FirstName = "Matej",
                LastName = "Gjozinski",
                DoB = DateTime.Today.AddYears(-21),
                Gender = midTerm.Data.Enums.Gender.Male,
                Country = "Macedonia"
            };

            // Act
            var result = await _service.Insert(user);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<SurveyUserBaseModel>();
            result.Id.Should().NotBe(0);
        }
        [Fact]
        public async Task UpdateUser()
        {
            // Arrange
            var user = new SurveyUserUpdate
            {
                Id = 1,
                FirstName = "Matej",
                LastName = "Gjozinski",
                DoB = DateTime.Today.AddYears(-21),
                Gender = midTerm.Data.Enums.Gender.Male,
                Country = "Makedonija"
            };

            // Act
            var result = await _service.Update(user);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<SurveyUserBaseModel>();
            result.Id.Should().Be(user.Id);
            result.FirstName.Should().Be(user.FirstName);
            result.LastName.Should().Be(user.LastName);
            result.DoB.Should().Be(user.DoB);
            result.Gender.Should().Be(user.Gender);
            result.Country.Should().Be(user.Country);

        }
        [Fact]
        public async Task ThrowExceptionOnUpdateUser()
        {
            // Arrange
            var user = new SurveyUserUpdate
            {
                Id = 10,
                FirstName = "Matej",
                LastName = "Gjozinski",
                DoB = DateTime.Today.AddYears(-21),
                Gender = midTerm.Data.Enums.Gender.Male,
                Country = "Makedonija"
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.Update(user));
            Assert.Equal("User not found", ex.Message);

        }
        [Fact]
        public async Task DeleteUser()
        {
            // Arrange
            var expected = 1;

            // Act
            var result = await _service.Delete(expected);
            var user = await _service.GetById(expected);

            // Assert
            result.Should().Be(true);
            user.Should().BeNull();
        }
    }
}
