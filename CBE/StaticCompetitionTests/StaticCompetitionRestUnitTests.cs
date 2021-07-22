using System;
using Xunit;
using StaticCompetitionDL;
using StaticCompetitionBL;
using StaticCompetitionRest;
using StaticCompetitionModels;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Moq;
using System.Collections.Generic;
using StaticCompetitionRest.Controllers;
using Microsoft.Extensions.Options;
using StaticCompetitionRest.DTO;
using Microsoft.AspNetCore.Mvc;
using GACDRest.Controllers;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Security.Claims;

namespace CBETests
{
    public class CBEUnitTests
    {
        private readonly DbContextOptions<StaticCompetitionDbContext> options;
        public CBEUnitTests()
        {
            options = new DbContextOptionsBuilder<StaticCompetitionDbContext>().UseSqlite("Filename=Test.db;").Options;
            Seed();
        }
        /// <summary>
        /// Method to make sure AddUser adds a user to the db
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddUserShouldAddUserAsync()
        {
            using (var context = new StaticCompetitionDbContext(options))
            {
                IUserBL userBL = new UserBL(context);
                User user = new User();
                user.Auth0Id = "test";
                await userBL.AddUser(user);
                int userCount = (await userBL.GetUsers()).Count;
                int expected = 1;
                Assert.Equal(expected, userCount);
            }
        }
        /// <summary>
        /// Makes sure that Categories can be added
        /// </summary>
        /// <returns>True if successful/False on fail</returns>
        [Fact]
        public async Task AddCatShouldAddCatAsync()
        {
            using (var context = new StaticCompetitionDbContext(options))
            {
                ICategoryBL categoryBL = new CategoryBL(context);
                Category category = new Category();
                category.Name = 1;
                await categoryBL.AddCategory(category);
                Category category1 = new Category();
                category1.Name = 2;
                await categoryBL.AddCategory(category1);
                Category category2 = new Category();
                category2.Name = 3;
                await categoryBL.AddCategory(category2);
                int catCount = (await categoryBL.GetAllCategories()).Count;
                int expected = 3;
                Assert.Equal(expected, catCount);
            }
        }
        /// <summary>
        /// Asserts that competiton is created and the id is not -1 (error)
        /// </summary>
        /// <returns>True if comp Id is valid, false otherwise</returns>
        [Fact]
        public async Task CompetitionShouldBeCreated()
        {
            using (var context = new StaticCompetitionDbContext(options))
            {
                Competition c = new Competition();
                User user = new User();
                user.Auth0Id = "test";
                IUserBL userBL = new UserBL(context);
                ICategoryBL categoryBL = new CategoryBL(context);
                //IUserStatBL userStatBL = new UserStatBL(context);
                ICompBL compBL = new CompBL(context);
                Category category = new Category();
                category.Name = 1;
                await categoryBL.AddCategory(category);
                await userBL.AddUser(user);
                Category category1 = await categoryBL.GetCategory(1);
                string testForComp = "Console.WriteLine('Hello World');";
                int actual = await compBL.AddCompetition(DateTime.Now, DateTime.Now, category1.Id, "name", 1, testForComp);
                int notExpected = -1;
                Assert.NotEqual(notExpected, actual);
            }
        }
        /// <summary>
        /// Makes sure a competition can be created and the string can be accessed
        /// </summary>
        /// <returns>True if hello world found, false otherwise</returns>
        [Fact]
        public async Task CompetitionStringShouldBeAccessed()
        {
            using (var context = new StaticCompetitionDbContext(options))
            {
                Competition c = new Competition();
                User user = new User();
                user.Auth0Id = "test";
                IUserBL userBL = new UserBL(context);
                ICategoryBL categoryBL = new CategoryBL(context);
                //IUserStatBL userStatBL = new UserStatBL(context);
                ICompBL compBL = new CompBL(context);
                Category category = new Category();
                category.Name = 1;
                await categoryBL.AddCategory(category);
                await userBL.AddUser(user);
                Category category1 = await categoryBL.GetCategory(1);
                string testForComp = "Console.WriteLine('Hello World');";
                int compId = await compBL.AddCompetition(DateTime.Now, DateTime.Now, category1.Id, "name", 1, testForComp);
                string actual = await compBL.GetCompString(compId);
                Assert.Equal(testForComp, actual);
            }
        }
        [Fact]
        public async Task BadCompetitionStringShouldBeAccessed()
        {
            using (var context = new StaticCompetitionDbContext(options))
            {
                Competition c = new Competition();
                User user = new User();
                user.Auth0Id = "test";
                IUserBL userBL = new UserBL(context);
                ICategoryBL categoryBL = new CategoryBL(context);
                //IUserStatBL userStatBL = new UserStatBL(context);
                ICompBL compBL = new CompBL(context);
                Category category = new Category();
                category.Name = 1;
                await categoryBL.AddCategory(category);
                await userBL.AddUser(user);
                Category category1 = await categoryBL.GetCategory(1);
                string testForComp = "Console.WriteLine('Hello World');";
                string actual = await compBL.GetCompString(-1);
                Assert.Null(actual);
            }
        }
        /// <summary>
        /// Making sure competition adds a single entry without error
        /// </summary>
        /// <returns>True on success, false on fail</returns>
        [Fact]
        public async Task CompetitionShouldAddEntry()
        {
            using (var context = new StaticCompetitionDbContext(options))
            {
                Competition c = new Competition();
                User user = new User();
                user.Auth0Id = "test";
                IUserBL userBL = new UserBL(context);
                ICategoryBL categoryBL = new CategoryBL(context);
                // IUserStatBL userStatBL = new UserStatBL(context);
                ICompBL compBL = new CompBL(context);
                Category category = new Category();
                category.Name = 1;
                await categoryBL.AddCategory(category);
                await userBL.AddUser(user);
                Category category1 = await categoryBL.GetCategory(1);
                string testForComp = "Console.WriteLine('Hello World');";
                int compId = await compBL.AddCompetition(DateTime.Now, DateTime.Now, category1.Id, "name", 1, testForComp, "testauthor");
                CompetitionStat competitionStat = new CompetitionStat();
                competitionStat.WPM = 50;
                competitionStat.UserId = 1;
                competitionStat.CompetitionId = compId;
                int actual = await compBL.InsertCompStatUpdate(competitionStat, 100, 6);
                int expected = 1;
                Assert.Equal(expected, actual);
            }
        }

        /// <summary>
        /// Makes sure competition updates rank (last person should be second)
        /// </summary>
        /// <returns>True on success/False on fail</returns>
        [Fact]
        public async Task CompetitionShouldUpdateRank()
        {
            using (var context = new StaticCompetitionDbContext(options))
            {
                Competition c = new Competition();
                User user = new User();
                user.Auth0Id = "test";
                IUserBL userBL = new UserBL(context);
                ICategoryBL categoryBL = new CategoryBL(context);
                //IUserStatBL userStatBL = new UserStatBL(context);
                ICompBL compBL = new CompBL(context);
                Category category = new Category();
                category.Name = 1;
                await categoryBL.AddCategory(category);
                await userBL.AddUser(user);
                user = new User();
                user.Auth0Id = "test1";
                await userBL.AddUser(user);
                user = new User();
                user.Auth0Id = "test2";
                await userBL.AddUser(user);
                Category category1 = await categoryBL.GetCategory(1);
                string testForComp = "Console.WriteLine('Hello World');";
                int compId = await compBL.AddCompetition(DateTime.Now, DateTime.Now, category1.Id, "name", 1, testForComp);
                CompetitionStat competitionStat = new CompetitionStat();
                competitionStat.WPM = 50;
                competitionStat.UserId = 1;
                competitionStat.CompetitionId = compId;
                await compBL.InsertCompStatUpdate(competitionStat, 100, 6);
                competitionStat = new CompetitionStat();
                competitionStat.WPM = 30;
                competitionStat.UserId = 2;
                competitionStat.CompetitionId = compId;
                await compBL.InsertCompStatUpdate(competitionStat, 100, 6);
                competitionStat = new CompetitionStat();
                competitionStat.WPM = 40;
                competitionStat.UserId = 3;
                competitionStat.CompetitionId = compId;
                int actual = await compBL.InsertCompStatUpdate(competitionStat, 100, 6);
                int expected = 2;
                Assert.Equal(expected, actual);
            }
        }
        /// <summary>
        /// Checks the GetCompetitionStats method to make sure it correctly returns 3 people
        /// </summary>
        /// <returns>True on success/ False on fail</returns>
        [Fact]
        public async Task CompetitionStatsShouldGetCompStats()
        {
            using (var context = new StaticCompetitionDbContext(options))
            {
                Competition c = new Competition();
                User user = new User();
                user.Auth0Id = "test";
                IUserBL userBL = new UserBL(context);
                ICategoryBL categoryBL = new CategoryBL(context);
                // IUserStatBL userStatBL = new UserStatBL(context);
                ICompBL compBL = new CompBL(context);
                Category category = new Category();
                category.Name = 1;
                await categoryBL.AddCategory(category);
                await userBL.AddUser(user);
                user = new User();
                user.Auth0Id = "test1";
                await userBL.AddUser(user);
                user = new User();
                user.Auth0Id = "test2";
                await userBL.AddUser(user);
                Category category1 = await categoryBL.GetCategory(1);
                string testForComp = "Console.WriteLine('Hello World');";
                int compId = await compBL.AddCompetition(DateTime.Now, DateTime.Now, category1.Id, "name", 1, testForComp);
                CompetitionStat competitionStat = new CompetitionStat();
                competitionStat.WPM = 50;
                competitionStat.UserId = 1;
                competitionStat.CompetitionId = compId;
                await compBL.InsertCompStatUpdate(competitionStat, 100, 6);
                competitionStat = new CompetitionStat();
                competitionStat.WPM = 30;
                competitionStat.UserId = 2;
                competitionStat.CompetitionId = compId;
                await compBL.InsertCompStatUpdate(competitionStat, 100, 6);
                competitionStat = new CompetitionStat();
                competitionStat.WPM = 40;
                competitionStat.UserId = 3;
                competitionStat.CompetitionId = compId;
                await compBL.InsertCompStatUpdate(competitionStat, 100, 6);
                int actual = (await compBL.GetCompetitionStats(compId)).Count;
                int expected = 3;
                Assert.Equal(expected, actual);
            }
        }
        [Fact]
        public async Task BadCompStatsShouldBeNull() { 
            using (var context = new StaticCompetitionDbContext(options))
            {
                Competition c = new Competition();
                User user = new User();
                user.Auth0Id = "test";
                IUserBL userBL = new UserBL(context);
                ICategoryBL categoryBL = new CategoryBL(context);
                // IUserStatBL userStatBL = new UserStatBL(context);
                ICompBL compBL = new CompBL(context);
                Category category = new Category();
                category.Name = 1;
                await categoryBL.AddCategory(category);
                await userBL.AddUser(user);
                user = new User();
                user.Auth0Id = "test1";
                await userBL.AddUser(user);
                user = new User();
                user.Auth0Id = "test2";
                await userBL.AddUser(user);
                Category category1 = await categoryBL.GetCategory(1);
                string testForComp = "Console.WriteLine('Hello World');";
                int compId = await compBL.AddCompetition(DateTime.Now, DateTime.Now, category1.Id, "name", 1, testForComp);
                CompetitionStat competitionStat = new CompetitionStat();
                competitionStat.WPM = 50;
                competitionStat.UserId = 1;
                competitionStat.CompetitionId = compId;
                await compBL.InsertCompStatUpdate(competitionStat, 100, 6);
                competitionStat = new CompetitionStat();
                competitionStat.WPM = 30;
                competitionStat.UserId = 2;
                competitionStat.CompetitionId = compId;
                await compBL.InsertCompStatUpdate(competitionStat, 100, 6);
                competitionStat = new CompetitionStat();
                competitionStat.WPM = 40;
                competitionStat.UserId = 3;
                competitionStat.CompetitionId = compId;
                await compBL.InsertCompStatUpdate(competitionStat, 100, 6);
                int actual = (await compBL.GetCompetitionStats(-1)).Count;
                int expected = 0;
                Assert.Equal(expected, actual);
            }
        }
        [Fact]
        public async Task SnippetShouldGetRandomQuote()
        {
            ISnippets snippetBL = new Snippets();
            var quote = await snippetBL.GetRandomQuote();
            Assert.NotNull(quote);
            Assert.IsType<TestMaterial>(quote);
        }

        [Fact]
        public async Task SnippetShouldGetCodeSnippet()
        {
            var settings = Options.Create(new ApiSettings());
            ISnippets snippetBL = new Snippets(settings);
            var code = snippetBL.GetCodeSnippet(1);
            Assert.NotNull(code);
            await Assert.IsType<Task<TestMaterial>>(code);
        }

        /// <summary>
        /// Makes sure adding two of the same category returns null
        /// </summary>
        /// <returns>True on success</returns>
        [Fact]
        public async Task AddingCategoryTwiceShouldBeNull()
        {
            using (var context = new StaticCompetitionDbContext(options))
            {
                ICategoryBL categoryBL = new CategoryBL(context);
                Category category = new Category();
                category.Name = 1;
                await categoryBL.AddCategory(category);
                Assert.Null(await categoryBL.AddCategory(category));
            }
        }
        /// <summary>
        /// Makes sure adding two of the same user returns null
        /// </summary>
        /// <returns>True on success</returns>
        [Fact]
        public async Task AddingUserTwiceShouldBeNull()
        {
            using (var context = new StaticCompetitionDbContext(options))
            {
                IUserBL userBL = new UserBL(context);
                User user = new User();
                user.Auth0Id = "test";
                await userBL.AddUser(user);
                Assert.Null(await userBL.AddUser(user));
            }
        }
        /// <summary>
        /// Makes sure we are able to get a user by their id
        /// </summary>
        /// <returns>True on success</returns>
        [Fact]
        public async Task GetUserByIdShouldWork()
        {
            using (var context = new StaticCompetitionDbContext(options))
            {
                IUserBL userBL = new UserBL(context);
                User user = new User();
                user.Auth0Id = "test";
                await userBL.AddUser(user);
                string expected = "test";
                string actual = (await userBL.GetUser(1)).Auth0Id;
                Assert.Equal(expected, actual);
            }
        }
        /// <summary>
        /// Makes sure that a user not in the database returns null
        /// </summary>
        /// <returns>True on success</returns>
        [Fact]
        public async Task GetBadUserIdShouldBeNull()
        {
            using (var context = new StaticCompetitionDbContext(options))
            {
                IUserBL userBL = new UserBL(context);
                Assert.Null(await userBL.GetUser(1));
            }
        }
        /// <summary>
        /// Just makes sure that a bogus comp id will return no competitionstats
        /// </summary>
        /// <returns>True on success</returns>
        [Fact]
        public async Task EmptyCompetitionShouldHaveEmptyStats()
        {
            using (var context = new StaticCompetitionDbContext(options))
            {
                int expected = 0;
                ICompBL compBL = new CompBL(context);
                int actual = (await compBL.GetCompetitionStats(1)).Count;
                Assert.Equal(expected, actual);
            }
        }
        /// <summary>
        /// Makes sure that we can retrieve the competition stuff from the database
        /// </summary>
        /// <returns>True on success</returns>
        [Fact]
        public async Task CompStuffShouldBeRetrieved()
        {
            using (var context = new StaticCompetitionDbContext(options))
            {
                Competition c = new Competition();
                User user = new User();
                user.Auth0Id = "test";
                IUserBL userBL = new UserBL(context);
                ICategoryBL categoryBL = new CategoryBL(context);
                // IUserStatBL userStatBL = new UserStatBL(context);
                ICompBL compBL = new CompBL(context);
                Category category = new Category();
                category.Name = 1;
                await categoryBL.AddCategory(category);
                await userBL.AddUser(user);
                Category category1 = await categoryBL.GetCategory(1);
                string testForComp = "Console.WriteLine('Hello World');";
                int compId = await compBL.AddCompetition(DateTime.Now, DateTime.Now, category1.Id, "name", 1, testForComp, "Ada Lovelace");
                Tuple<string, string, int> tuple = await compBL.GetCompStuff(compId);
                Assert.Equal(testForComp, tuple.Item2);
            }
        }
        /// <summary>
        /// Makes sure that we are able to get category by id
        /// </summary>
        /// <returns>True on success</returns>
        [Fact]
        public async Task GetCategoryByIdShouldWork()
        {
            using (var context = new StaticCompetitionDbContext(options))
            {
                ICategoryBL categoryBL = new CategoryBL(context);
                Category category = new Category();
                category.Name = 3;
                await categoryBL.AddCategory(category);
                Category category1 = await categoryBL.GetCategoryById(1);
                int expected = 3;
                int actual = category1.Name;
                Assert.Equal(expected, actual);
            }
        }
        [Fact]
        public async Task GettingABadCategoryShouldReturnNull()
        {
            using (var context = new StaticCompetitionDbContext(options))
            {
                ICategoryBL categoryBL = new CategoryBL(context);
                Category category1 = await categoryBL.GetCategoryById(-1);
                Assert.Null(category1);
            }
        }
        /// <summary>
        /// Makes sure competition will show that count is one when we add a competition
        /// </summary>
        /// <returns>True on success</returns>
        [Fact]
        public async Task GetCompetitionsShouldGetAComp()
        {
            using (var context = new StaticCompetitionDbContext(options))
            {
                Competition c = new Competition();
                User user = new User();
                user.Auth0Id = "test";
                IUserBL userBL = new UserBL(context);
                ICategoryBL categoryBL = new CategoryBL(context);
                ICompBL compBL = new CompBL(context);
                Category category = new Category();
                category.Name = 1;
                await categoryBL.AddCategory(category);
                await userBL.AddUser(user);
                Category category1 = await categoryBL.GetCategory(1);
                string testForComp = "Console.WriteLine('Hello World');";
                await compBL.AddCompetition(DateTime.Now, DateTime.Now, category1.Id, "name", 1, testForComp, "testauthor");
                int expected = 1;
                int actual = (await compBL.GetAllCompetitions()).Count;
                Assert.Equal(expected, actual);
            }
        }
        /// <summary>
        /// Makes sure that the get competitions is empty without adding a competition
        /// </summary>
        /// <returns>True on success</returns>
        [Fact]
        public async Task GetCompetitionsOnEmptyIsNewList()
        {
            using (var context = new StaticCompetitionDbContext(options))
            {
                ICompBL compBL = new CompBL(context);
                int expected = 0;
                int actual = (await compBL.GetAllCompetitions()).Count;
                Assert.Equal(expected, actual);
            }
        }

        /// <summary>
        /// Making sure getting a bad competition is null
        /// </summary>
        /// <returns>True on success</returns>
        [Fact]
        public async Task GettingBadCompetitionByIdShouldBeNull()
        {
            using (var context = new StaticCompetitionDbContext(options))
            {
                ICompBL compBL = new CompBL(context);
                Assert.Null(await compBL.GetCompetition(-1));
            }
        }

        [Fact]
        public async Task CompetitionControllerShouldReturnListOfCompetitionObject()
        {
            var mockCompBL = new Mock<ICompBL>();
            mockCompBL.Setup(x => x.GetAllCompetitions()).ReturnsAsync(
                new List<Competition>
                {
                    new Competition(){
                        UserCreatedId = 1,
                        StartDate = new DateTime(),
                        EndDate = new DateTime(),
                        CategoryId = 1,
                        CompetitionName = "Competition",
                        TestString = "Test",
                        TestAuthor = "Author"
                    },
                    new Competition()
                    {
                        UserCreatedId = 2,
                        StartDate = new DateTime(),
                        EndDate = new DateTime(),
                        CategoryId = 1,
                        CompetitionName = "Competition",
                        TestString = "String",
                        TestAuthor = "Author"
                    }
                }
                );
            var mockCatBL = new Mock<ICategoryBL>();
            mockCatBL.Setup(x => x.GetCategoryById(1)).ReturnsAsync(new Category());
            var mockUserBL = new Mock<IUserBL>();
            var settings = Options.Create(new ApiSettings());

            var controller = new CompetitionController(mockCompBL.Object, mockCatBL.Object, mockUserBL.Object, settings);
            var result = await controller.GetCompAsync();
            Assert.NotNull(result);
            Assert.IsType<ActionResult<IEnumerable<CompetitionObject>>>(result);
        }

        [Fact]
        public async Task CompetitionControllerShouldReturnListOfUsers()
        {
            var mockCompBL = new Mock<ICompBL>();
            var mockCatBL = new Mock<ICategoryBL>();
            var mockUserBL = new Mock<IUserBL>();
            mockUserBL.Setup(x => x.GetUsers()).ReturnsAsync(
                new List<User>
                {
                    new User(){
                        Auth0Id = "AM",
                        Revapoints = 500
                    },
                    new User()
                    {
                        Auth0Id = "AM",
                        Revapoints = 1
                    }
                }
                );
            var settings = Options.Create(new ApiSettings());

            var controller = new CompetitionController(mockCompBL.Object, mockCatBL.Object, mockUserBL.Object, settings);
            var result = await controller.GetAllUsers();
            Assert.NotNull(result);
            Assert.IsType<ActionResult<IEnumerable<UserNameModel>>>(result);
        }

        [Fact]
        public async Task CompetitionControllerShouldReturnListOfCompStatOutput()
        {
            var mockCompBL = new Mock<ICompBL>();
            mockCompBL.Setup(x => x.GetCompetitionStats(1)).ReturnsAsync(
                new List<CompetitionStat>
                {
                    new CompetitionStat() {
                        CompetitionId = 1,
                        UserId = 1,
                        rank = 2,
                        WPM = 30,
                        Accuracy = 6
                    },
                    new CompetitionStat()
                    {
                        CompetitionId = 2,
                        UserId = 1,
                        rank = 1,
                        WPM = 60,
                        Accuracy = 5
                    }
                }
                );
            mockCompBL.Setup(x => x.GetCompetition(1)).ReturnsAsync(new Competition() { Id = 1, UserCreatedId = 1, StartDate = new DateTime(), EndDate = new DateTime(), CategoryId = 1, CompetitionName = "Name", TestString = "String", TestAuthor = "Author" });
            var mockCatBL = new Mock<ICategoryBL>();
            var mockUserBL = new Mock<IUserBL>();
            mockUserBL.Setup(userBL => userBL.GetUser(1)).ReturnsAsync(new User() { Id = 1, Auth0Id = "BZ", Revapoints = 5000 });
            var settings = Options.Create(new ApiSettings());

            var controller = new CompetitionController(mockCompBL.Object, mockCatBL.Object, mockUserBL.Object, settings);
            var result = await controller.GetAsync(1);
            Assert.NotNull(result);
            Assert.IsType<ActionResult<IEnumerable<CompStatOutput>>>(result);
        }

        [Fact]
        public async Task CompetitionControllerShouldPost()
        {
            var mockCompBL = new Mock<ICompBL>();
            mockCompBL.Setup(compBL => compBL.AddCompetition(new DateTime(), new DateTime(), 1, "Name", 1, "String", "Author")).ReturnsAsync(1);
            var mockCatBL = new Mock<ICategoryBL>();
            mockCatBL.Setup(catBL => catBL.GetCategory(1)).ReturnsAsync(new Category() { Id = 1, Name = 1 });
            var mockUserBL = new Mock<IUserBL>();
            mockUserBL.Setup(userBL => userBL.GetUser("BZ")).ReturnsAsync(new User() { Id = 1, Auth0Id = "BZ", Revapoints = 5000 });
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "BZ")
            }));
            var settings = Options.Create(new ApiSettings());
            var controller = new CompetitionController(mockCompBL.Object, mockCatBL.Object, mockUserBL.Object, settings);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
            var result = await controller.Post(new CompetitionObject() { Name = "Name", Start = new DateTime(), End = new DateTime(), Category = 1, snippet = "String", author = "Author", compId = 1 });
            var createResult = result as CreatedAtRouteResult;
            Assert.NotNull(createResult);
            Assert.True(createResult is CreatedAtRouteResult);
            Assert.Equal(StatusCodes.Status201Created, createResult.StatusCode);
        }

        [Fact]
        public async Task CompetitionTestControllerShouldReturnCompetitionContent()
        {
            var mockCompBL = new Mock<ICompBL>();
            mockCompBL.Setup(x => x.GetCompStuff(1)).ReturnsAsync(Tuple.Create("author", "string", 1));
            var mockCatBL = new Mock<ICategoryBL>();
            mockCatBL.Setup(x => x.GetCategoryById(1)).ReturnsAsync(new Category());
            var mockUserBL = new Mock<IUserBL>();
            var mockSnippets = new Mock<ISnippets>();
            var controller = new CompetitonTestsController(mockUserBL.Object, mockCatBL.Object, mockCompBL.Object, mockSnippets.Object);
            var result = await controller.Get(1);
            Assert.NotNull(result);
            Assert.IsType<ActionResult<CompetitionContent>>(result);
        }
        [Fact]
        public async Task CompetitionTestControllerShouldReturn404()
        {
            var mockCompBL = new Mock<ICompBL>();
            mockCompBL.Setup(x => x.GetCompStuff(1)).ReturnsAsync(Tuple.Create("author", "string", 1));
            var mockCatBL = new Mock<ICategoryBL>();
            mockCatBL.Setup(x => x.GetCategoryById(1)).ReturnsAsync(new Category());
            var mockUserBL = new Mock<IUserBL>();
            var mockSnippets = new Mock<ISnippets>();
            var controller = new CompetitonTestsController(mockUserBL.Object, mockCatBL.Object, mockCompBL.Object, mockSnippets.Object);
            var result = await controller.Get(-1);
            var returnedStatus = result.Result as NotFoundResult;
            Assert.Equal(returnedStatus.StatusCode, StatusCodes.Status404NotFound);

        }
        [Fact]
        public void CheckScopeAuthShouldThrowAnException()
        {
            Assert.Throws<ArgumentNullException>(() => new CheckScopeAuth(null, null));
        }


        [Fact]
        public async Task CompetitionTestsShouldPost()
        {
            var mockCompBL = new Mock<ICompBL>();
            DateTime date = DateTime.Now;
            mockCompBL.Setup(compBL => compBL.GetCompetition(1)).ReturnsAsync(new Competition() { Id = 1, UserCreatedId = 1, StartDate = new DateTime(), EndDate = date.AddHours(1), CategoryId = 1, CompetitionName = "Name", TestString = "String", TestAuthor = "Author" });
            mockCompBL.Setup(compBL => compBL.InsertCompStatUpdate(new CompetitionStat() { CompetitionId = 1, UserId = 1, rank = 5, WPM = 25, Accuracy = 6 }, 500, 2)).ReturnsAsync(1);
            var mockUserBL = new Mock<IUserBL>();
            mockUserBL.Setup(userBL => userBL.GetUser("BZ")).ReturnsAsync(new User() { Id = 1, Auth0Id = "BZ", Revapoints = 5000 });
            var mockCatBL = new Mock<ICategoryBL>();
            mockCatBL.Setup(catBL => catBL.GetCategory(1)).ReturnsAsync(new Category() { Id = 1, Name = 1 });
            ISnippets snippetService = new Snippets();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "BZ")
            }));
            var controller = new CompetitonTestsController(mockUserBL.Object, mockCatBL.Object, mockCompBL.Object, snippetService);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
            var result = await controller.Post(new CompInput() { compId = 1 });
            Assert.NotNull(result);
            Assert.Equal(0, result.Value);
        }
       
        // [Fact]
        // public async Task EnsureLiveCompStatsReturnsEmpty()
        // {
        //     Mock mcontext = new Mock<StaticCompetitionDbContext>();
        //     mcontext.Setup(x => x.LiveCompStats).returns(null);
        //     using (var context = new StaticCompetitionDbContext(options))
        //     {
        //         //ICompBL compBL = new CompBL(context);
        //         Repo _repo = new Repo(context);

        //         var actual = (await _repo.GetLiveCompStats(3));
        //         Assert.Empty(actual);
        //     }
        // }

        private void Seed()
        {
            using (var context = new StaticCompetitionDbContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
        }
    } 
}