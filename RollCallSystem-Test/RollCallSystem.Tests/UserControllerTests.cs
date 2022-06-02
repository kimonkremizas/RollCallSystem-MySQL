using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RollCallSystem.APIModels;
using RollCallSystem.Controllers;
using RollCallSystem.Database;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RollCallSystem_Test.RollCallSystem.Tests;

[TestClass]
public class UserControllerTests
{
    [TestMethod]
    public async Task TestGetUsers()
    {
        //Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "RollCallDatabase")
            .Options;

        using (var context = new ApplicationDbContext(options))
        {
            context.Users.Add(new User { Id = 1, Email = "one@email.com", Password = "pwd1", Salt = "salt", FirstName = "One", LastName = "One", RoleId = 0 });
            context.Users.Add(new User { Id = 2, Email = "two@email.com", Password = "pwd2", Salt = "salt", FirstName = "Two", LastName = "Two", RoleId = 0 });
            context.Users.Add(new User { Id = 3, Email = "three@email.com", Password = "pwd3", Salt = "salt", FirstName = "Three", LastName = "Three", RoleId = 0 });
            context.Users.Add(new User { Id = 4, Email = "four@email.com", Password = "pwd4", Salt = "salt", FirstName = "Four", LastName = "Four", RoleId = 1 });
            context.Users.Add(new User { Id = 5, Email = "five@email.com", Password = "pwd5", Salt = "salt", FirstName = "Five", LastName = "Five", RoleId = 1 });
            context.Users.Add(new User { Id = 6, Email = "six@email.com", Password = "pwd6", Salt = "salt", FirstName = "Six", LastName = "Six", RoleId = 2 });
            context.SaveChanges();
        }

        //Clean context
        using (var context = new ApplicationDbContext(options))
        {
            //Act
            UsersController usersController = new UsersController(context);
            var result = await usersController.GetUsers();
            List<TrimmedUser> users = (List<TrimmedUser>)result.Value!;

            context.Database.EnsureDeleted();
            //Assert
            Assert.AreEqual(6, users.Count);
        }
    }

    [TestMethod]
    public async Task TestGetTeachers()
    {
        //Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "RollCallDatabase")
            .Options;

        using (var context = new ApplicationDbContext(options))
        {
            context.Users.Add(new User { Id = 1, Email = "one@email.com", Password = "pwd1", Salt = "salt", FirstName = "One", LastName = "One", RoleId = 0 });
            context.Users.Add(new User { Id = 2, Email = "two@email.com", Password = "pwd2", Salt = "salt", FirstName = "Two", LastName = "Two", RoleId = 0 });
            context.Users.Add(new User { Id = 3, Email = "three@email.com", Password = "pwd3", Salt = "salt", FirstName = "Three", LastName = "Three", RoleId = 0 });
            context.Users.Add(new User { Id = 4, Email = "four@email.com", Password = "pwd4", Salt = "salt", FirstName = "Four", LastName = "Four", RoleId = 1 });
            context.Users.Add(new User { Id = 5, Email = "five@email.com", Password = "pwd5", Salt = "salt", FirstName = "Five", LastName = "Five", RoleId = 1 });
            context.Users.Add(new User { Id = 6, Email = "six@email.com", Password = "pwd6", Salt = "salt", FirstName = "Six", LastName = "Six", RoleId = 2 });
            context.SaveChanges();
        }

        //Clean context
        using (var context = new ApplicationDbContext(options))
        {
            //Act
            UsersController usersController = new UsersController(context);
            var result = await usersController.GetTeachers();
            List<TrimmedUser> users = (List<TrimmedUser>)result.Value!;

            context.Database.EnsureDeleted();
            //Assert
            Assert.AreEqual(2, users.Count);
            Assert.IsTrue(users.Any(x => x.Id == 4) && users.Any(x => x.Id == 5));
        }
    }

    [TestMethod]
    public async Task TestGetStudents()
    {
        //Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "RollCallDatabase")
            .Options;

        using (var context = new ApplicationDbContext(options))
        {
            context.Users.Add(new User { Id = 1, Email = "one@email.com", Password = "pwd1", Salt = "salt", FirstName = "One", LastName = "One", RoleId = 0 });
            context.Users.Add(new User { Id = 2, Email = "two@email.com", Password = "pwd2", Salt = "salt", FirstName = "Two", LastName = "Two", RoleId = 0 });
            context.Users.Add(new User { Id = 3, Email = "three@email.com", Password = "pwd3", Salt = "salt", FirstName = "Three", LastName = "Three", RoleId = 0 });
            context.Users.Add(new User { Id = 4, Email = "four@email.com", Password = "pwd4", Salt = "salt", FirstName = "Four", LastName = "Four", RoleId = 1 });
            context.Users.Add(new User { Id = 5, Email = "five@email.com", Password = "pwd5", Salt = "salt", FirstName = "Five", LastName = "Five", RoleId = 1 });
            context.Users.Add(new User { Id = 6, Email = "six@email.com", Password = "pwd6", Salt = "salt", FirstName = "Six", LastName = "Six", RoleId = 2 });
            context.SaveChanges();
        }

        //Clean context
        using (var context = new ApplicationDbContext(options))
        {
            //Act
            UsersController usersController = new UsersController(context);
            var result = await usersController.GetStudents();
            List<TrimmedUser> users = (List<TrimmedUser>)result.Value!;

            context.Database.EnsureDeleted();
            //Assert
            Assert.AreEqual(3, users.Count);
            Assert.IsTrue(users.Any(x => x.Id == 1) && users.Any(x => x.Id == 2) && users.Any(x => x.Id == 3));
        }
    }

    [TestMethod]
    public async Task TestGetUserById()
    {
        //Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "RollCallDatabase")
            .Options;

        using (var context = new ApplicationDbContext(options))
        {
            context.Users.Add(new User { Id = 1, Email = "one@email.com", Password = "pwd1", Salt = "salt", FirstName = "One", LastName = "One", RoleId = 0 });
            context.Users.Add(new User { Id = 2, Email = "two@email.com", Password = "pwd2", Salt = "salt", FirstName = "Two", LastName = "Two", RoleId = 0 });
            context.Users.Add(new User { Id = 3, Email = "three@email.com", Password = "pwd3", Salt = "salt", FirstName = "Three", LastName = "Three", RoleId = 0 });
            context.Users.Add(new User { Id = 4, Email = "four@email.com", Password = "pwd4", Salt = "salt", FirstName = "Four", LastName = "Four", RoleId = 1 });
            context.Users.Add(new User { Id = 5, Email = "five@email.com", Password = "pwd5", Salt = "salt", FirstName = "Five", LastName = "Five", RoleId = 1 });
            context.Users.Add(new User { Id = 6, Email = "six@email.com", Password = "pwd6", Salt = "salt", FirstName = "Six", LastName = "Six", RoleId = 2 });
            context.SaveChanges();
        }

        //Clean context
        using (var context = new ApplicationDbContext(options))
        {
            //Act
            UsersController usersController = new UsersController(context);
            var result = await usersController.GetUser(1);
            TrimmedUser user = result.Value!;

            context.Database.EnsureDeleted();
            //Assert
            Assert.AreEqual(1, user.Id);
        }
    }

    [TestMethod]
    public async Task TestGetUserByIdNotFound()
    {
        //Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "RollCallDatabase")
            .Options;

        using (var context = new ApplicationDbContext(options))
        {
            context.Users.Add(new User { Id = 1, Email = "one@email.com", Password = "pwd1", Salt = "salt", FirstName = "One", LastName = "One", RoleId = 0 });
            context.Users.Add(new User { Id = 2, Email = "two@email.com", Password = "pwd2", Salt = "salt", FirstName = "Two", LastName = "Two", RoleId = 0 });
            context.Users.Add(new User { Id = 3, Email = "three@email.com", Password = "pwd3", Salt = "salt", FirstName = "Three", LastName = "Three", RoleId = 0 });
            context.Users.Add(new User { Id = 4, Email = "four@email.com", Password = "pwd4", Salt = "salt", FirstName = "Four", LastName = "Four", RoleId = 1 });
            context.Users.Add(new User { Id = 5, Email = "five@email.com", Password = "pwd5", Salt = "salt", FirstName = "Five", LastName = "Five", RoleId = 1 });
            context.Users.Add(new User { Id = 6, Email = "six@email.com", Password = "pwd6", Salt = "salt", FirstName = "Six", LastName = "Six", RoleId = 2 });
            context.SaveChanges();
        }

        //Clean context
        using (var context = new ApplicationDbContext(options))
        {
            //Act
            UsersController usersController = new UsersController(context);
            var result = await usersController.GetUser(8);

            context.Database.EnsureDeleted();
            //Assert
            Assert.IsTrue(result.Result is NotFoundResult);
        }
    }

    [TestMethod]
    public async Task TestGetSelf()
    {
        //Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "RollCallDatabase")
            .Options;

        using (var context = new ApplicationDbContext(options))
        {
            context.Users.Add(new User { Id = 1, Email = "one@email.com", Password = "pwd1", Salt = "salt", FirstName = "One", LastName = "One", RoleId = 0 });
            context.Users.Add(new User { Id = 2, Email = "two@email.com", Password = "pwd2", Salt = "salt", FirstName = "Two", LastName = "Two", RoleId = 0 });
            context.Users.Add(new User { Id = 3, Email = "three@email.com", Password = "pwd3", Salt = "salt", FirstName = "Three", LastName = "Three", RoleId = 0 });
            context.Users.Add(new User { Id = 4, Email = "four@email.com", Password = "pwd4", Salt = "salt", FirstName = "Four", LastName = "Four", RoleId = 1 });
            context.Users.Add(new User { Id = 5, Email = "five@email.com", Password = "pwd5", Salt = "salt", FirstName = "Five", LastName = "Five", RoleId = 1 });
            context.Users.Add(new User { Id = 6, Email = "six@email.com", Password = "pwd6", Salt = "salt", FirstName = "Six", LastName = "Six", RoleId = 2 });
            context.SaveChanges();
        }

        TrimmedUser mockUser = new TrimmedUser(1, "one@email.com", "One", "One", 0);

        //Clean context
        using (var context = new ApplicationDbContext(options))
        {
            //Act
            UsersController usersController = new UsersController(context, mockUser);
            var result = await usersController.GetSelf();
            TrimmedUser user = result.Value!;

            context.Database.EnsureDeleted();
            //Assert
            Assert.AreEqual(1, user.Id);
        }
    }

    [TestMethod]
    public async Task TestPut()
    {
        //Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "RollCallDatabase")
            .Options;

        using (var context = new ApplicationDbContext(options))
        {
            context.Users.Add(new User { Id = 1, Email = "one@email.com", Password = "pwd1", Salt = "salt", FirstName = "One", LastName = "One", RoleId = 0 });
            context.Users.Add(new User { Id = 2, Email = "two@email.com", Password = "pwd2", Salt = "salt", FirstName = "Two", LastName = "Two", RoleId = 0 });
            context.Users.Add(new User { Id = 3, Email = "three@email.com", Password = "pwd3", Salt = "salt", FirstName = "Three", LastName = "Three", RoleId = 0 });
            context.Users.Add(new User { Id = 4, Email = "four@email.com", Password = "pwd4", Salt = "salt", FirstName = "Four", LastName = "Four", RoleId = 1 });
            context.Users.Add(new User { Id = 5, Email = "five@email.com", Password = "pwd5", Salt = "salt", FirstName = "Five", LastName = "Five", RoleId = 1 });
            context.Users.Add(new User { Id = 6, Email = "six@email.com", Password = "pwd6", Salt = "salt", FirstName = "Six", LastName = "Six", RoleId = 2 });
            context.SaveChanges();
        }

        User newUser = new User { Id = 1, Email = "newemail@email.com", Password = "pwd1", Salt = "salt", FirstName = "One", LastName = "One", RoleId = 0 };

        //Clean context
        using (var context = new ApplicationDbContext(options))
        {
            //Act
            UsersController usersController = new UsersController(context);
            await usersController.PutUser(1, newUser);
            var result = await usersController.GetUser(1);
            TrimmedUser user = result.Value!;

            context.Database.EnsureDeleted();
            //Assert
            Assert.IsTrue(user.Email == newUser.Email);
        }
    }

    [TestMethod]
    public async Task TestPutNotFound()
    {
        //Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "RollCallDatabase")
            .Options;

        using (var context = new ApplicationDbContext(options))
        {
            context.Users.Add(new User { Id = 1, Email = "one@email.com", Password = "pwd1", Salt = "salt", FirstName = "One", LastName = "One", RoleId = 0 });
            context.Users.Add(new User { Id = 2, Email = "two@email.com", Password = "pwd2", Salt = "salt", FirstName = "Two", LastName = "Two", RoleId = 0 });
            context.Users.Add(new User { Id = 3, Email = "three@email.com", Password = "pwd3", Salt = "salt", FirstName = "Three", LastName = "Three", RoleId = 0 });
            context.Users.Add(new User { Id = 4, Email = "four@email.com", Password = "pwd4", Salt = "salt", FirstName = "Four", LastName = "Four", RoleId = 1 });
            context.Users.Add(new User { Id = 5, Email = "five@email.com", Password = "pwd5", Salt = "salt", FirstName = "Five", LastName = "Five", RoleId = 1 });
            context.Users.Add(new User { Id = 6, Email = "six@email.com", Password = "pwd6", Salt = "salt", FirstName = "Six", LastName = "Six", RoleId = 2 });
            context.SaveChanges();
        }

        User newUser = new User { Id = 8, Email = "newemail@email.com", Password = "pwd1", Salt = "salt", FirstName = "One", LastName = "One", RoleId = 0 };

        //Clean context
        using (var context = new ApplicationDbContext(options))
        {
            //Act
            UsersController usersController = new UsersController(context);
            var result = await usersController.PutUser(8, newUser);

            context.Database.EnsureDeleted();
            //Assert
            Assert.IsTrue(result is NotFoundResult);
        }
    }

    [TestMethod]
    public async Task TestPutBadRequest()
    {
        //Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "RollCallDatabase")
            .Options;

        using (var context = new ApplicationDbContext(options))
        {
            context.Users.Add(new User { Id = 1, Email = "one@email.com", Password = "pwd1", Salt = "salt", FirstName = "One", LastName = "One", RoleId = 0 });
            context.Users.Add(new User { Id = 2, Email = "two@email.com", Password = "pwd2", Salt = "salt", FirstName = "Two", LastName = "Two", RoleId = 0 });
            context.Users.Add(new User { Id = 3, Email = "three@email.com", Password = "pwd3", Salt = "salt", FirstName = "Three", LastName = "Three", RoleId = 0 });
            context.Users.Add(new User { Id = 4, Email = "four@email.com", Password = "pwd4", Salt = "salt", FirstName = "Four", LastName = "Four", RoleId = 1 });
            context.Users.Add(new User { Id = 5, Email = "five@email.com", Password = "pwd5", Salt = "salt", FirstName = "Five", LastName = "Five", RoleId = 1 });
            context.Users.Add(new User { Id = 6, Email = "six@email.com", Password = "pwd6", Salt = "salt", FirstName = "Six", LastName = "Six", RoleId = 2 });
            context.SaveChanges();
        }

        User newUser = new User { Id = 1, Email = "newemail@email.com", Password = "pwd1", Salt = "salt", FirstName = "One", LastName = "One", RoleId = 0 };

        //Clean context
        using (var context = new ApplicationDbContext(options))
        {
            //Act
            UsersController usersController = new UsersController(context);
            var result = await usersController.PutUser(8, newUser);

            context.Database.EnsureDeleted();
            //Assert
            Assert.IsTrue(result is BadRequestResult);
        }
    }

    [TestMethod]
    public async Task TestPost()
    {
        //Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "RollCallDatabase")
            .Options;

        using (var context = new ApplicationDbContext(options))
        {
            context.Users.Add(new User { Id = 1, Email = "one@email.com", Password = "pwd1", Salt = "salt", FirstName = "One", LastName = "One", RoleId = 0 });
            context.Users.Add(new User { Id = 2, Email = "two@email.com", Password = "pwd2", Salt = "salt", FirstName = "Two", LastName = "Two", RoleId = 0 });
            context.Users.Add(new User { Id = 3, Email = "three@email.com", Password = "pwd3", Salt = "salt", FirstName = "Three", LastName = "Three", RoleId = 0 });
            context.Users.Add(new User { Id = 4, Email = "four@email.com", Password = "pwd4", Salt = "salt", FirstName = "Four", LastName = "Four", RoleId = 1 });
            context.Users.Add(new User { Id = 5, Email = "five@email.com", Password = "pwd5", Salt = "salt", FirstName = "Five", LastName = "Five", RoleId = 1 });
            context.Users.Add(new User { Id = 6, Email = "six@email.com", Password = "pwd6", Salt = "salt", FirstName = "Six", LastName = "Six", RoleId = 2 });
            context.SaveChanges();
        }

        User newUser = new User { Id = 7, Email = "newemail@email.com", Password = "pwd7", Salt = "salt", FirstName = "Seven", LastName = "Seven", RoleId = 2 };

        //Clean context
        using (var context = new ApplicationDbContext(options))
        {
            //Act
            UsersController usersController = new UsersController(context);
            await usersController.PostUser(newUser);
            var result = await usersController.GetUsers();
            List<TrimmedUser> users = (List<TrimmedUser>)result.Value!;

            context.Database.EnsureDeleted();
            //Assert
            Assert.IsTrue(users.Any(x => x.Id == 7));
        }
    }

    [TestMethod]
    public async Task TestDelete()
    {
        //Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "RollCallDatabase")
            .Options;

        using (var context = new ApplicationDbContext(options))
        {
            context.Users.Add(new User { Id = 1, Email = "one@email.com", Password = "pwd1", Salt = "salt", FirstName = "One", LastName = "One", RoleId = 0 });
            context.Users.Add(new User { Id = 2, Email = "two@email.com", Password = "pwd2", Salt = "salt", FirstName = "Two", LastName = "Two", RoleId = 0 });
            context.Users.Add(new User { Id = 3, Email = "three@email.com", Password = "pwd3", Salt = "salt", FirstName = "Three", LastName = "Three", RoleId = 0 });
            context.Users.Add(new User { Id = 4, Email = "four@email.com", Password = "pwd4", Salt = "salt", FirstName = "Four", LastName = "Four", RoleId = 1 });
            context.Users.Add(new User { Id = 5, Email = "five@email.com", Password = "pwd5", Salt = "salt", FirstName = "Five", LastName = "Five", RoleId = 1 });
            context.Users.Add(new User { Id = 6, Email = "six@email.com", Password = "pwd6", Salt = "salt", FirstName = "Six", LastName = "Six", RoleId = 2 });
            context.SaveChanges();
        }

        //Clean context
        using (var context = new ApplicationDbContext(options))
        {
            //Act
            UsersController usersController = new UsersController(context);
            await usersController.DeleteUser(1);
            var result = await usersController.GetUsers();
            List<TrimmedUser> users = (List<TrimmedUser>)result.Value!;

            context.Database.EnsureDeleted();
            //Assert
            Assert.IsFalse(users.Any(x => x.Id == 1));
        }
    }

    [TestMethod]
    public async Task TestDeleteNotFound()
    {
        //Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "RollCallDatabase")
            .Options;

        using (var context = new ApplicationDbContext(options))
        {
            context.Users.Add(new User { Id = 1, Email = "one@email.com", Password = "pwd1", Salt = "salt", FirstName = "One", LastName = "One", RoleId = 0 });
            context.Users.Add(new User { Id = 2, Email = "two@email.com", Password = "pwd2", Salt = "salt", FirstName = "Two", LastName = "Two", RoleId = 0 });
            context.Users.Add(new User { Id = 3, Email = "three@email.com", Password = "pwd3", Salt = "salt", FirstName = "Three", LastName = "Three", RoleId = 0 });
            context.Users.Add(new User { Id = 4, Email = "four@email.com", Password = "pwd4", Salt = "salt", FirstName = "Four", LastName = "Four", RoleId = 1 });
            context.Users.Add(new User { Id = 5, Email = "five@email.com", Password = "pwd5", Salt = "salt", FirstName = "Five", LastName = "Five", RoleId = 1 });
            context.Users.Add(new User { Id = 6, Email = "six@email.com", Password = "pwd6", Salt = "salt", FirstName = "Six", LastName = "Six", RoleId = 2 });
            context.SaveChanges();
        }

        //Clean context
        using (var context = new ApplicationDbContext(options))
        {
            //Act
            UsersController usersController = new UsersController(context);
            var result = await usersController.DeleteUser(8);

            context.Database.EnsureDeleted();
            //Assert
            Assert.IsTrue(result is NotFoundResult);
        }
    }
}
