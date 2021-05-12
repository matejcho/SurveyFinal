using AutoMapper;
using midterm.Services.Test.Internal;
using midTerm.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using midTerm.Models.Profiles;
using Xunit;
using FluentAssertions;
using midTerm.Models.Models.Answers;

namespace midterm.Services.Test.Service
{
    public class AnswersShould : SqlLiteContext
    {
        private readonly IMapper _mapper;
        private readonly AnswerService _service;
        public AnswersShould() : base(true)
        {
            if (_mapper == null)
            {
                var mapper = new MapperConfiguration(cfg =>
                {
                    cfg.AddMaps(typeof(AnswersProfile));
                }).CreateMapper();
                _mapper = mapper;
            }
            _service = new AnswerService(DbContext, _mapper);
        }
        [Fact]
        public async Task GetAnswerById()
        {
            // Arrange
            var expected = 1;

            // Act
            var result = await _service.GetById(expected);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<midTerm.Models.Models.Answers.AnswersExtended>();
            result.Id.Should().Be(expected);
        }
        [Fact]
        public async Task GetAnswers()
        {
            // Arrange
            var expected = 8;

            // Act
            var result = await _service.Get();

            // Assert
            result.Should().NotBeEmpty().And.HaveCount(expected);
            result.Should().BeAssignableTo<IEnumerable<midTerm.Models.Models.Answers.AnswersBaseModel>>();
        }
        [Fact]
        public async Task InsertNewAnswer()
        {
            // Arrange
            var answers = new AnswerCreateModel
            {
                UserId = 1,
                OptionId = 1
            };

            // Act
            var result = await _service.Insert(answers);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<AnswersBaseModel>();
            result.Id.Should().NotBe(0);
        }
        [Fact]
        public async Task UpdateAnswer()
        {
            // Arrange
            var answers = new AnswersUpdateModel
            {
                Id = 1,
                UserId = 1,
                OptionId = 3
            };

            // Act
            var result = await _service.Update(answers);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<AnswersBaseModel>();
            result.Id.Should().Be(answers.Id);
            result.UserId.Should().Be(answers.UserId);
            result.OptionId.Should().Be(answers.OptionId);
        }
        [Fact]
        public async Task ThrowExceptionOnUpdateAnswer()
        {
            // Arrange
            var answers = new AnswersUpdateModel
            {
                Id = 10,
                UserId = 2,
                OptionId = 2
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.Update(answers));
            Assert.Equal("Answer not found", ex.Message);

        }
        [Fact]
        public async Task DeleteAnswer()
        {
            // Arrange
            var expected = 1;

            // Act
            var result = await _service.Delete(expected);
            var answers = await _service.GetById(expected);

            // Assert
            result.Should().Be(true);
            answers.Should().BeNull();
        }
    }
}
