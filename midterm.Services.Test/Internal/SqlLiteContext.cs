using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using midTerm.Data;
using midTerm.Data.Entities;


namespace midterm.Services.Test.Internal
{
    public abstract class SqlLiteContext : IDisposable
    {
        private const string InMemoryConnectionString = "DataSource=:memory:";
        private readonly SqliteConnection _connection;
        protected readonly MidTermDbContext DbContext;
        protected DbContextOptions<MidTermDbContext> CreateOptions()
        {
            return new DbContextOptionsBuilder<MidTermDbContext>()
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .UseSqlite(_connection)
                .Options;
        }
        protected SqlLiteContext(bool withData = false)
        {
            _connection = new SqliteConnection(InMemoryConnectionString);
            DbContext = new MidTermDbContext(CreateOptions());
            _connection.Open();
            DbContext.Database.EnsureCreated();
            if (withData)
                SeedData(DbContext);
        }
        private void SeedData(MidTermDbContext context)
        {
            var users = new List<SurveyUser>
                {
                    new SurveyUser
                    {
                        Id = 1,
                        FirstName = "Matej",
                        LastName = "Gjozinski",
                        DoB = DateTime.Today.AddYears(-21),
                        Gender = midTerm.Data.Enums.Gender.Male,
                        Country = "Macedonia",
                    },
                    new SurveyUser
                    {
                        Id = 2,
                        FirstName = "Mateja",
                        LastName = "Gjozinska",
                        DoB = DateTime.Today.AddYears(-22),
                        Gender = midTerm.Data.Enums.Gender.Female,
                        Country = "Macedonia",
                    },
                    new SurveyUser
                    {
                        Id = 3,
                        FirstName = "Matejcho",
                        LastName = "Gjozinski",
                        DoB = DateTime.Today.AddYears(-20),
                        Gender = midTerm.Data.Enums.Gender.Male,
                        Country = "Macedonia",
                    }
                };
            var questions = new List<Question>
                {
                    new Question
                    {
                        Id = 1,
                        Text = "Question 1",
                        Description = "Description 1"
                    },
                    new Question
                    {
                        Id = 2,
                        Text = "Question 2",
                        Description = "Description 2"
                    },
                    new Question
                    {
                        Id = 3,
                        Text = "Question 3",
                        Description = "Description 3"
                    },
                    new Question
                    {
                        Id = 4,
                        Text = "Question 4",
                        Description = "Description 4"
                    },
                };
            var options = new List<Option>
                {
                    new Option
                    {
                        Id = 1,
                        Text = "Option 1",
                        Order = 1,
                        QuestionId = 2
                    },
                    new Option
                    {
                        Id = 2,
                        Text = "Option 2",
                        Order = 1,
                        QuestionId = 2
                    },
                   new Option
                    {
                        Id = 3,
                        Text = "Option 3",
                        Order = 2,
                        QuestionId = 2
                    },
                    new Option
                    {
                        Id = 4,
                        Text = "Option 4",
                        Order = 2,
                        QuestionId = 2
                    },
                    new Option
                    {
                        Id = 5,
                        Text = "Option 1",
                        Order = 1,
                        QuestionId = 1
                    },
                    new Option
                    {
                        Id = 6,
                        Text = "Option 2",
                        Order = 1,
                        QuestionId = 1
                    },
                    new Option
                    {
                        Id = 7,
                        Text = "Option 3",
                        Order = 1,
                        QuestionId = 1
                    },
                    new Option
                    {
                        Id = 8,
                        Text = "Option 4",
                        Order = 1,
                        QuestionId = 1
                    },
                    new Option
                    {
                        Id = 9,
                        Text = "Option 1",
                        Order = 1,
                        QuestionId = 3
                    },
                    new Option
                    {
                        Id = 10,
                        Text = "Option 2",
                        Order = 1,
                        QuestionId = 3
                    },
                    new Option
                    {
                        Id = 11,
                        Text = "Option 3",
                        Order = 1,
                        QuestionId = 3
                    },
                    new Option
                    {
                        Id = 12,
                        Text = "Option 4",
                        Order = 1,
                        QuestionId = 3
                    },
                };
            var answers = new List<Answers>
            {
                new Answers
                {
                    Id = 1,
                    UserId = 1,
                    OptionId = 1
                },
                new Answers
                {
                    Id = 2,
                    UserId = 1,
                    OptionId = 2
                },
                new Answers
                {
                    Id = 3,
                    UserId = 1,
                    OptionId = 5
                },
                new Answers
                {
                    Id = 4,
                    UserId = 1,
                    OptionId = 6
                },
                new Answers
                {
                    Id = 5,
                    UserId = 1,
                    OptionId = 4
                },
                new Answers
                {
                    Id = 6,
                    UserId = 2,
                    OptionId = 1,
                },
                new Answers
                {
                    Id = 7,
                    UserId = 2,
                    OptionId = 8,
                },
                new Answers
                {
                    Id = 8,
                    UserId = 2,
                    OptionId = 9,
                },
            };
            context.AddRange(users);
            context.AddRange(options);
            context.AddRange(questions);
            context.AddRange(answers);
            context.SaveChanges();
        }

        public void Dispose()
        {
            _connection.Close();
        }
    }
}
