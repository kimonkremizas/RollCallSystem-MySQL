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
    public class CampusControllerTests
    {
        [TestMethod]
        public async Task TestGetCampus()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "RollCallDatabase")
            .Options;

            using (var context = new ApplicationDbContext(options))
            {
                context.Campuses.Add(new Campus { Id = 1, Name="Campusone", Location ="location", Ssid="ssid" });
                context.Campuses.Add(new Campus { Id = 2, Name = "Campustwo", Location = "location", Ssid = "ssid" });
                context.SaveChanges();
            }
            //Clean context
            using (var context = new ApplicationDbContext(options))
            {
                //Act
                CampusController campusController = new CampusController(context);
                var result = await campusController.GetCampuses();
                List<Campus> campuses = (List<Campus>)result.Value!;

                context.Database.EnsureDeleted();
                //Assert
                Assert.AreEqual(2, campuses.Count);
            }
           
        }
        [TestMethod]
        public async Task TestGetCampusById()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "RollCallDatabase")
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                context.Campuses.Add(new Campus { Id = 1, Name = "Campusone", Location = "location", Ssid = "ssid" });
                context.Campuses.Add(new Campus { Id = 2, Name = "Campustwo", Location = "location", Ssid = "ssid" });
                context.SaveChanges();
            }

            //Clean context
            using (var context = new ApplicationDbContext(options))
            {
                //Act
                CampusController campusesController = new CampusController(context);
                var result = await campusesController.GetCampus(1);
                Campus campus = result.Value!;

                context.Database.EnsureDeleted();
                //Assert
                Assert.AreEqual(1, campus.Id);
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
                context.Campuses.Add(new Campus { Id = 1, Name = "Campusone", Location = "location1", Ssid = "ssid1" });
                context.Campuses.Add(new Campus { Id = 2, Name = "Campustwo", Location = "location2", Ssid = "ssid2" });
                context.SaveChanges();
            }

            Campus newCampus = new Campus { Id = 1, Name = "Campusnew", Location = "location1", Ssid="ssid1" };

            //Clean context
            using (var context = new ApplicationDbContext(options))
            {
                //Act
                CampusController campusesController = new CampusController(context);
                await campusesController.PutCampus(1, newCampus);
                var result = await campusesController.GetCampus(1);
                Campus campus = result.Value!;

                context.Database.EnsureDeleted();

                //Assert
                Assert.IsTrue(campus.Name == newCampus.Name);
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
                context.Campuses.Add(new Campus { Id = 1, Name = "Campusone", Location = "location1", Ssid = "ssid1" });
                context.Campuses.Add(new Campus { Id = 2, Name = "Campustwo", Location = "location2", Ssid = "ssid2" });

                context.SaveChanges();
            }

            Campus newCampus = new Campus { Id =4, Name = "Campusnew", Location = "location1", Ssid = "ssid1" };

            //Clean context
            using (var context = new ApplicationDbContext(options))
            {
                //Act
                CampusController campusesController = new CampusController(context);
                await campusesController.PostCampus(newCampus);
                var result = await campusesController.GetCampuses();
                List<Campus> campuses = (List<Campus>)result.Value!;

                context.Database.EnsureDeleted();
                //Assert
                Assert.IsTrue(campuses.Any(x => x.Id == 4));
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
                context.Campuses.Add(new Campus { Id = 1, Name = "Campusone", Location = "location1", Ssid = "ssid1" });
                context.Campuses.Add(new Campus { Id = 2, Name = "Campustwo", Location = "location2", Ssid = "ssid2" });
                context.SaveChanges();
            }

            //Clean context
            using (var context = new ApplicationDbContext(options))
            {
                //Act
                CampusController campusesController = new CampusController(context);
                await campusesController.DeleteCampus(1);
                var result = await campusesController.GetCampuses();
                List<Campus> campuses = (List<Campus>)result.Value!;
                context.Database.EnsureDeleted();
                //Assert
                Assert.IsFalse(campuses.Any(x => x.Id == 1));
            }
        }

    }
}
