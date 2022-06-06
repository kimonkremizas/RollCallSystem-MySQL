using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RollCallSystem;
using RollCallSystem.APIModels;
using RollCallSystem.Controllers;
using RollCallSystem.Database;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace RollCallSystem_Test.RollCallSystem.Tests;

[TestClass]
public class LessonControllerTests
{
    [DataTestMethod]
    [DataRow(1, 3)]
    [DataRow(2, 6)]
    [DataRow(3, 0)]
    [DataRow(4, 3)]
    [DataRow(5, 3)]
    [DataRow(6, 0)]
    public async Task TestGetLessonsByMonth(int userId, int expectedLessons)
    {
        //Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "RollCallDatabase")
            .Options;

        User user1 = new User { Id = 1, Email = "one@email.com", Password = "pwd1", Salt = "salt", FirstName = "One", LastName = "One", RoleId = 0 };
        User user2 = new User { Id = 2, Email = "two@email.com", Password = "pwd2", Salt = "salt", FirstName = "Two", LastName = "Two", RoleId = 0 };
        User user3 = new User { Id = 3, Email = "three@email.com", Password = "pwd3", Salt = "salt", FirstName = "Three", LastName = "Three", RoleId = 0 };
        User user4 = new User { Id = 4, Email = "four@email.com", Password = "pwd4", Salt = "salt", FirstName = "Four", LastName = "Four", RoleId = 1 };
        User user5 = new User { Id = 5, Email = "five@email.com", Password = "pwd5", Salt = "salt", FirstName = "Five", LastName = "Five", RoleId = 1 };
        User user6 = new User { Id = 6, Email = "six@email.com", Password = "pwd6", Salt = "salt", FirstName = "Six", LastName = "Six", RoleId = 2 };

        User[] users = new User[] { user1, user2, user3, user4, user5, user6 };

        using (var context = new ApplicationDbContext(options))
        {
            context.Lessons.Add(new Lesson { Id = 1, SubjectId = 1, CampusId = 1, StartTime = new DateTime(2022, 6, 14) });
            context.Lessons.Add(new Lesson { Id = 2, SubjectId = 1, CampusId = 1, StartTime = new DateTime(2022, 6, 15) });
            context.Lessons.Add(new Lesson { Id = 3, SubjectId = 1, CampusId = 1, StartTime = new DateTime(2022, 6, 16) });
            context.Lessons.Add(new Lesson { Id = 4, SubjectId = 1, CampusId = 1, StartTime = new DateTime(2022, 7, 14) });
            context.Lessons.Add(new Lesson { Id = 5, SubjectId = 1, CampusId = 1, StartTime = new DateTime(2022, 8, 14) });
            context.Lessons.Add(new Lesson { Id = 6, SubjectId = 2, CampusId = 1, StartTime = new DateTime(2022, 6, 14) });
            context.Lessons.Add(new Lesson { Id = 7, SubjectId = 2, CampusId = 1, StartTime = new DateTime(2022, 6, 15) });
            context.Lessons.Add(new Lesson { Id = 8, SubjectId = 2, CampusId = 1, StartTime = new DateTime(2022, 6, 16) });

            context.Subjects.Add(new Subject { Id = 1, Name = "Subject 1", TeacherId = 4, Students = new List<User> { user1, user2 } });
            context.Subjects.Add(new Subject { Id = 2, Name = "Subject 2", TeacherId = 5, Students = new List<User> { user2 } });

            context.Users.Add(user1);
            context.Users.Add(user2);
            context.Users.Add(user3);
            context.Users.Add(user4);
            context.Users.Add(user5);
            context.Users.Add(user6);

            context.SaveChanges();
        }

        int monthTested = 6;
        TrimmedUser mockUser = users[userId - 1];

        //Clean context
        using (var context = new ApplicationDbContext(options))
        {
            //Act
            LessonsController lessonsController = new LessonsController(context, mockUser);
            var result = await lessonsController.GetByMonth(monthTested);
            List<TrimmedLesson> lessons = (List<TrimmedLesson>)result.Value!;

            context.Database.EnsureDeleted();
            //Assert
            Assert.AreEqual(expectedLessons, lessons.Count);
        }
    }


    [DataTestMethod]
    [DataRow(1, true)]
    [DataRow(2, false)]
    public async Task TestDidStudentCheckIn(int userId, bool expectedResult)
    {
        //Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "RollCallDatabase")
            .Options;

        User user1 = new User { Id = 1, Email = "one@email.com", Password = "pwd1", Salt = "salt", FirstName = "One", LastName = "One", RoleId = 0 };
        User user2 = new User { Id = 2, Email = "two@email.com", Password = "pwd2", Salt = "salt", FirstName = "Two", LastName = "Two", RoleId = 0 };

        int lessonToCheck = 1;

        User[] users = new User[] { user1, user2 };

        TrimmedUser mockUser = users[userId - 1];

        using (var context = new ApplicationDbContext(options))
        {
            context.Lessons.Add(new Lesson { Id = 1, SubjectId = 1, CampusId = 1, StartTime = new DateTime(2022, 6, 14), Students = new List<User> { user1 } });

            context.Subjects.Add(new Subject { Id = 1, Name = "Subject 1", TeacherId = 4, Students = new List<User> { user1, user2 } });

            context.Users.Add(user1);
            context.Users.Add(user2);

            context.SaveChanges();
        }

        //Clean context
        using (var context = new ApplicationDbContext(options))
        {
            //Act
            LessonsController lessonsController = new LessonsController(context, mockUser);
            var result = await lessonsController.DidStudentCheckIn(lessonToCheck);
            bool checkedIn = result.Value!;

            context.Database.EnsureDeleted();
            //Assert
            Assert.AreEqual(expectedResult, checkedIn);
        }
    }

    [TestMethod]
    public async Task GetAllCheckedInStudentsTest()
    {
        //Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "RollCallDatabase")
            .Options;

        User user1 = new User { Id = 1, Email = "one@email.com", Password = "pwd1", Salt = "salt", FirstName = "One", LastName = "One", RoleId = 0 };
        User user2 = new User { Id = 2, Email = "two@email.com", Password = "pwd2", Salt = "salt", FirstName = "Two", LastName = "Two", RoleId = 0 };
        User user3 = new User { Id = 3, Email = "three@email.com", Password = "pwd3", Salt = "salt", FirstName = "Three", LastName = "Three", RoleId = 0 };
        User user4 = new User { Id = 4, Email = "four@email.com", Password = "pwd4", Salt = "salt", FirstName = "Four", LastName = "Four", RoleId = 0 };

        int lessonToCheck = 1;

        using (var context = new ApplicationDbContext(options))
        {
            context.Lessons.Add(new Lesson { Id = 1, SubjectId = 1, CampusId = 1, StartTime = new DateTime(2022, 6, 14), Students = new List<User> { user1, user2, user3 } });

            context.Subjects.Add(new Subject { Id = 1, Name = "Subject 1", TeacherId = 4, Students = new List<User> { user1, user2, user3, user4 } });

            context.Users.Add(user1);
            context.Users.Add(user2);

            context.SaveChanges();
        }

        //Clean context
        using (var context = new ApplicationDbContext(options))
        {
            //Act
            LessonsController lessonsController = new LessonsController(context);
            var result = await lessonsController.GetAllCheckedIn(lessonToCheck);
            List<TrimmedUser> checkedInStudents = (List<TrimmedUser>)result.Value!;

            context.Database.EnsureDeleted();
            //Assert
            Assert.AreEqual(3, checkedInStudents.Count);
        }
    }

    [DataTestMethod]
    [DataRow(0)]
    [DataRow(1)]
    public async Task TestGetCurrentLesson(int userNo)
    {
        //Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "RollCallDatabase")
            .Options;

        User user1 = new User { Id = 1, Email = "one@email.com", Password = "pwd1", Salt = "salt", FirstName = "One", LastName = "One", RoleId = 0 };
        User user4 = new User { Id = 4, Email = "four@email.com", Password = "pwd4", Salt = "salt", FirstName = "Four", LastName = "Four", RoleId = 1 };
        User[] users = new[] { user1, user4 };

        using (var context = new ApplicationDbContext(options))
        {
            context.Lessons.Add(new Lesson { Id = 1, SubjectId = 1, CampusId = 1, StartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour - 1, DateTime.Now.Minute, DateTime.Now.Second) });
            context.Lessons.Add(new Lesson { Id = 2, SubjectId = 1, CampusId = 1, StartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second) });
            context.Lessons.Add(new Lesson { Id = 3, SubjectId = 1, CampusId = 1, StartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour + 1, DateTime.Now.Minute, DateTime.Now.Second) });
            context.Lessons.Add(new Lesson { Id = 4, SubjectId = 1, CampusId = 1, StartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour + 2, DateTime.Now.Minute, DateTime.Now.Second) });

            context.Subjects.Add(new Subject { Id = 1, Name = "Subject 1", TeacherId = 4, Students = new List<User> { user1 } });

            context.Users.Add(user1);
            context.Users.Add(user4);

            context.Campuses.Add(new Campus { Id = 1, Name = "Campus 1", Location = "Location 1", Ssid = "SSID 1" });

            context.SaveChanges();
        }

        TrimmedUser mockUser = users[userNo];

        //Clean context
        using (var context = new ApplicationDbContext(options))
        {
            //Act
            LessonsController lessonsController = new LessonsController(context, mockUser);
            var result = await lessonsController.GetCurrentLesson();
            TrimmedLesson lesson = result.Value!;

            context.Database.EnsureDeleted();
            //Assert
            Assert.AreEqual(2, lesson.Id);
        }
    }

    //Student only gets their own, Admin and Teacher can get all
    [DataTestMethod]
    [DataRow(1, 5)]
    [DataRow(2, 8)]
    [DataRow(3, 0)]
    [DataRow(4, 8)]
    [DataRow(5, 8)]
    [DataRow(6, 8)]
    public async Task TestGetLessons(int userId, int expectedLessons)
    {
        //Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "RollCallDatabase")
            .Options;

        User user1 = new User { Id = 1, Email = "one@email.com", Password = "pwd1", Salt = "salt", FirstName = "One", LastName = "One", RoleId = 0 };
        User user2 = new User { Id = 2, Email = "two@email.com", Password = "pwd2", Salt = "salt", FirstName = "Two", LastName = "Two", RoleId = 0 };
        User user3 = new User { Id = 3, Email = "three@email.com", Password = "pwd3", Salt = "salt", FirstName = "Three", LastName = "Three", RoleId = 0 };
        User user4 = new User { Id = 4, Email = "four@email.com", Password = "pwd4", Salt = "salt", FirstName = "Four", LastName = "Four", RoleId = 1 };
        User user5 = new User { Id = 5, Email = "five@email.com", Password = "pwd5", Salt = "salt", FirstName = "Five", LastName = "Five", RoleId = 1 };
        User user6 = new User { Id = 6, Email = "six@email.com", Password = "pwd6", Salt = "salt", FirstName = "Six", LastName = "Six", RoleId = 2 };

        User[] users = new User[] { user1, user2, user3, user4, user5, user6 };

        using (var context = new ApplicationDbContext(options))
        {
            context.Lessons.Add(new Lesson { Id = 1, SubjectId = 1, CampusId = 1, StartTime = new DateTime(2022, 6, 14) });
            context.Lessons.Add(new Lesson { Id = 2, SubjectId = 1, CampusId = 1, StartTime = new DateTime(2022, 6, 15) });
            context.Lessons.Add(new Lesson { Id = 3, SubjectId = 1, CampusId = 1, StartTime = new DateTime(2022, 6, 16) });
            context.Lessons.Add(new Lesson { Id = 4, SubjectId = 1, CampusId = 1, StartTime = new DateTime(2022, 7, 14) });
            context.Lessons.Add(new Lesson { Id = 5, SubjectId = 1, CampusId = 1, StartTime = new DateTime(2022, 8, 14) });
            context.Lessons.Add(new Lesson { Id = 6, SubjectId = 2, CampusId = 1, StartTime = new DateTime(2022, 6, 14) });
            context.Lessons.Add(new Lesson { Id = 7, SubjectId = 2, CampusId = 1, StartTime = new DateTime(2022, 6, 15) });
            context.Lessons.Add(new Lesson { Id = 8, SubjectId = 2, CampusId = 1, StartTime = new DateTime(2022, 6, 16) });

            context.Subjects.Add(new Subject { Id = 1, Name = "Subject 1", TeacherId = 4, Students = new List<User> { user1, user2 } });
            context.Subjects.Add(new Subject { Id = 2, Name = "Subject 2", TeacherId = 5, Students = new List<User> { user2 } });

            context.Users.Add(user1);
            context.Users.Add(user2);
            context.Users.Add(user3);
            context.Users.Add(user4);
            context.Users.Add(user5);
            context.Users.Add(user6);

            context.Campuses.Add(new Campus { Id = 1, Name = "Campus 1", Location = "Location 1", Ssid = "SSID 1" });

            context.SaveChanges();
        }

        TrimmedUser mockUser = users[userId - 1];

        //Clean context
        using (var context = new ApplicationDbContext(options))
        {
            //Act
            LessonsController lessonsController = new LessonsController(context, mockUser);
            var result = await lessonsController.GetLessons();
            List<TrimmedLesson> lessons = (List<TrimmedLesson>)result.Value!;

            context.Database.EnsureDeleted();
            //Assert
            Assert.AreEqual(expectedLessons, lessons.Count);
        }
    }
}
