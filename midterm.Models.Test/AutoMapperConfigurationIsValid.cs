using midterm.Models.Test.Internal;
using System;
using Xunit;
using midTerm.Models.Profiles;

namespace midterm.Models.Test
{
    public class AutoMapperConfigurationIsValid
    {
        [Fact]
        public void AutoMapper_Configuration_IsValid()
        {
            // Arrange
            var configuration = AutoMapperModule.CreateMapperConfiguration<SurveyUserProfile>();

            // Act/Assert
            configuration.AssertConfigurationIsValid();
        }
    }
}
