using Moq;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using TechTestKLG.Data;
using TechTestKLG.Models;
using TechTestKLG.Services;
using TechTestKLG.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechTestKLG.DTO;

namespace TechTestKLG.Tests
{
    [TestFixture]
    public class ActivityServiceTests
    {
        private Mock<ILogger<ActivityService>> _mockLogger;
        private DbContextOptions<DataContext> _dbContextOptions;
        private DataContext _context;
        private ActivityService _service;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<ActivityService>>();

            _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new DataContext(_dbContextOptions);
            _context.Database.EnsureCreated();

            _service = new ActivityService(_context, _mockLogger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Insert_ShouldInsertActivitySuccessfully()
        {
            var activity = new Activities
            {
                Id = "1",
                Subject = "Test Activity",
                Description = "Test Description",
                Status = (int)ActivityStatus.UNMARKED
            };

            await _service.Insert(activity);

            var insertedActivity = await _context.Activities.FindAsync(activity.Id);
            Assert.That(insertedActivity, Is.Not.Null);
            Assert.That(insertedActivity.Subject, Is.EqualTo("Test Activity"));
        }

        [Test]
        public async Task Update_ShouldUpdateActivitySuccessfully()
        {
            var activity = new Activities
            {
                Id = "1",
                Subject = "Test Activity",
                Description = "Test Description",
                Status = (int)ActivityStatus.UNMARKED
            };

            await _context.Activities.AddAsync(activity);
            await _context.SaveChangesAsync();

            activity.Subject = "Updated Activity";

            await _service.Update(activity);

            var updatedActivity = await _context.Activities.FindAsync(activity.Id);
            Assert.That(updatedActivity.Subject, Is.EqualTo("Updated Activity"));
        }

        [Test]
        public async Task Delete_ShouldDeleteActivitySuccessfully()
        {
            var activity = new Activities
            {
                Id = "1",
                Subject = "Test Activity",
                Description = "Test Description",
                Status = (int)ActivityStatus.UNMARKED
            };

            await _context.Activities.AddAsync(activity);
            await _context.SaveChangesAsync();

            await _service.Delete(activity.Id);

            var deletedActivity = await _context.Activities.FindAsync(activity.Id);
            Assert.That(deletedActivity, Is.Null);
        }

        [Test]
        public async Task GetActivity_ShouldReturnActivity_WhenActivityExists()
        {
            var activity = new Activities
            {
                Id = "1",
                Subject = "Test Activity",
                Description = "Test Description",
                Status = (int)ActivityStatus.UNMARKED
            };

            await _context.Activities.AddAsync(activity);
            await _context.SaveChangesAsync();

            var result = await _service.GetActivity(activity.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Subject, Is.EqualTo("Test Activity"));
        }

        [Test]
        public async Task GetActivity_ShouldReturnPagedActivities_WhenRequestIsValid()
        {
            var activities = new List<Activities>
            {
                new Activities { Id = "1", Subject = "Test Activity 1", Description = "Description 1", Status = (int)ActivityStatus.UNMARKED },
                new Activities { Id = "2", Subject = "Test Activity 2", Description = "Description 2", Status = (int)ActivityStatus.UNMARKED }
            };

            await _context.Activities.AddRangeAsync(activities);
            await _context.SaveChangesAsync();

            var request = new GridviewRequestDTO { Start = 1, Length = 10, Search = "" };

            var result = await _service.GetActivity(request);

            Assert.That(result.RecordsTotal, Is.EqualTo(2));
            Assert.That(result.RecordsFiltered, Is.EqualTo(1));
        }
    }
}
