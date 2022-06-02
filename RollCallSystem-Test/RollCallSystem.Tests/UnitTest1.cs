using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RollCallSystem.APIModels;
using RollCallSystem.Controllers;
using RollCallSystem.Database;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RollCallSystem_Test;

[TestClass]
public class UnitTest1
{
    //Template
    [TestMethod]
    public async Task TestMethod1()
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
}