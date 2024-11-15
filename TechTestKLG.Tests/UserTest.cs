using Moq;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using TechTestKLG.Data;
using TechTestKLG.Models;
using TechTestKLG.Services;
using TechTestKLG.DTO;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TechTestKLG.Tests
{
    [TestFixture]
    public class UserTest
    {
        private Mock<ILogger<UserService>> _mockLogger;
        private DbContextOptions<DataContext> _dbContextOptions;
        private DataContext _context;
        private UserService _service;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<UserService>>();

            _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new DataContext(_dbContextOptions);
            _context.Database.EnsureCreated();

            _service = new UserService(_context, _mockLogger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Insert_ShouldInsertUserSuccessfully()
        {
            var user = new Users
            {
                Username = "testuser",
                Password = "TestPassword123",
                Name = "Test User"
            };

            var result = await _service.Insert(user);

            var insertedUser = await _context.Users.FindAsync(user.Username);
            Assert.That(result, Is.True);
            Assert.That(insertedUser, Is.Not.Null);
            Assert.That(insertedUser.Username, Is.EqualTo(user.Username));
        }

        [Test]
        public async Task Insert_ShouldFailWhenUsernameExists()
        {
            var user1 = new Users
            {
                Username = "testuser",
                Password = "TestPassword123",
                Name = "Test User"
            };

            var user2 = new Users
            {
                Username = "testuser",
                Password = "NewPassword123",
                Name = "New User"
            };

            await _service.Insert(user1);

            var result = await _service.Insert(user2);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task Update_ShouldUpdateUserSuccessfully()
        {
            var user = new Users
            {
                Username = "testuser",
                Password = "TestPassword123",
                Name = "Test User"
            };

            await _service.Insert(user);

            user.Name = "Updated User";
            var result = await _service.Update(user);

            var updatedUser = await _context.Users.FindAsync(user.Username);
            Assert.That(result, Is.True);
            Assert.That(updatedUser.Name, Is.EqualTo("Updated User"));
        }

        [Test]
        public async Task Update_ShouldFailWhenUserNotFound()
        {
            var user = new Users
            {
                Username = "nonexistentuser",
                Password = "TestPassword123",
                Name = "Test User"
            };

            var result = await _service.Update(user);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task Delete_ShouldDeleteUserSuccessfully()
        {
            var user = new Users
            {
                Username = "testuser",
                Password = "TestPassword123",
                Name = "Test User"
            };

            await _service.Insert(user);

            var result = await _service.Delete(user.Username);

            var deletedUser = await _context.Users.FindAsync(user.Username);
            Assert.That(result, Is.True);
            Assert.That(deletedUser, Is.Null);
        }

        [Test]
        public async Task Delete_ShouldFailWhenUserNotFound()
        {
            var result = await _service.Delete("nonexistentuser");

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task GetUser_ShouldReturnUserWhenExists()
        {
            var user = new Users
            {
                Username = "testuser",
                Password = "TestPassword123",
                Name = "Test User"
            };

            await _service.Insert(user);

            var result = await _service.GetUser(user.Username);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Username, Is.EqualTo(user.Username));
        }

        [Test]
        public async Task GetUser_ShouldReturnNullWhenUserNotFound()
        {
            var result = await _service.GetUser("nonexistentuser");

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetUsers_ShouldReturnPagedUsers()
        {
            var users = new List<Users>
            {
                new Users { Username = "user1", Password = "Password1", Name = "User One" },
                new Users { Username = "user2", Password = "Password2", Name = "User Two" }
            };

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            var request = new GridviewRequestDTO { Start = 1, Length = 2, Search = "" };
            var result = await _service.GetUsers(request);

            Assert.That(result.RecordsTotal, Is.EqualTo(2));
            Assert.That(result.RecordsFiltered, Is.EqualTo(1));
            Assert.That(result.Data.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetUsers_ShouldReturnFilteredUsers_WhenSearchIsProvided()
        {
            var users = new List<Users>
            {
                new Users { Username = "user1", Password = "Password1", Name = "User One" },
                new Users { Username = "user2", Password = "Password2", Name = "User Two" }
            };

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            var request = new GridviewRequestDTO { Start = 1, Length = 2, Search = "One" };
            var result = await _service.GetUsers(request);

            Assert.That(result.RecordsTotal, Is.EqualTo(2));
            Assert.That(result.RecordsFiltered, Is.EqualTo(1));
            Assert.That(result.Data.Count, Is.EqualTo(1));
            Assert.That(result.Data[0].Name, Is.EqualTo("User One"));
        }
    }
}
