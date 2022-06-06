using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RollCallSystem.Controllers;
using RollCallSystem.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RollCallSystem_Test.RollCallSystem.Tests
{
    [TestClass]
    public class TrophyControllerTests
    {
        [TestMethod]
        public async Task TestGetTrophy()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "RollCallDatabase")
            .Options;

            using (var context = new ApplicationDbContext(options))
            {
                context.Trophies.Add(new Trophy { Id = 1, Name = "Trophyone", Automatic = true});
                context.Trophies.Add(new Trophy { Id = 2, Name = "Trophytwo", Automatic = false });
                context.Trophies.Add(new Trophy { Id = 3, Name = "Trophythree", Automatic = false });
                context.SaveChanges();
            }
            //Clean context
            using (var context = new ApplicationDbContext(options))
            {
                //Act
                TrophiesController trophyController = new TrophiesController(context);
                var result = await trophyController.GetTrophies();
                List<Trophy> trophies = (List<Trophy>)result.Value!;

                context.Database.EnsureDeleted();
                //Assert
                Assert.AreEqual(3, trophies.Count);
            }
        }
        [TestMethod]
        public async Task TestGetTrophyById()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "RollCallDatabase")
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                context.Trophies.Add(new Trophy { Id = 1, Name = "Trophyone", Automatic = true });
                context.Trophies.Add(new Trophy { Id = 2, Name = "Trophytwo", Automatic = false });
                context.Trophies.Add(new Trophy { Id = 3, Name = "Trophythree", Automatic = false });
                context.SaveChanges();
            }

            //Clean context
            using (var context = new ApplicationDbContext(options))
            {
                //Act
                TrophiesController trophyController = new TrophiesController(context);
                var result = await trophyController.GetTrophy(1);
                Trophy trophy = result.Value!;

                context.Database.EnsureDeleted();
                //Assert
                Assert.AreEqual(1, trophy.Id);
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
                context.Trophies.Add(new Trophy { Id = 1, Name = "Trophyone", Automatic = true });
                context.Trophies.Add(new Trophy { Id = 2, Name = "Trophytwo", Automatic = false });
                context.Trophies.Add(new Trophy { Id = 3, Name = "Trophythree", Automatic = false });
                context.SaveChanges();
            }

            Trophy newTrophy = new Trophy { Id = 3, Name = "Trophynew", Automatic= false };

            //Clean context
            using (var context = new ApplicationDbContext(options))
            {
                //Act
                TrophiesController trophiesController = new TrophiesController(context);
                await trophiesController.PutTrophy(3, newTrophy);
                var result = await trophiesController.GetTrophy(3);
                Trophy Trophy = result.Value!;

                context.Database.EnsureDeleted();

                //Assert
                Assert.IsTrue(Trophy.Name == newTrophy.Name);
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
                context.Trophies.Add(new Trophy { Id = 1, Name = "Trophyone", Automatic = true });
                context.Trophies.Add(new Trophy { Id = 2, Name = "Trophytwo", Automatic = false });
                context.Trophies.Add(new Trophy { Id = 3, Name = "Trophythree", Automatic = false });
                context.SaveChanges();
            }

            Trophy newTrophy = new Trophy { Id = 4, Name = "Trophynew", Automatic = false };

            //Clean context
            using (var context = new ApplicationDbContext(options))
            {
                //Act
                TrophiesController trophiesController = new TrophiesController(context);
                await trophiesController.PostTrophy(newTrophy);
                var result = await trophiesController.GetTrophies();
                List<Trophy> Trophies = (List<Trophy>)result.Value!;

                context.Database.EnsureDeleted();
                //Assert
                Assert.IsTrue(Trophies.Any(x => x.Id == 4));
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
                context.Trophies.Add(new Trophy { Id = 1, Name = "Trophyone", Automatic = true });
                context.Trophies.Add(new Trophy { Id = 2, Name = "Trophytwo", Automatic = false });
                context.Trophies.Add(new Trophy { Id = 3, Name = "Trophythree", Automatic = false });
                context.SaveChanges();
            }

            //Clean context
            using (var context = new ApplicationDbContext(options))
            {
                //Act
                TrophiesController trophiesController = new TrophiesController(context);
                await trophiesController.DeleteTrophy(1);
                var result = await trophiesController.GetTrophies();
                List<Trophy> Trophies = (List<Trophy>)result.Value!;
                context.Database.EnsureDeleted();
                //Assert
                Assert.IsFalse(Trophies.Any(x => x.Id == 1));
            }
        }

    }
}

