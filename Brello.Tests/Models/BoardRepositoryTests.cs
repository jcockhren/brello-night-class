using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Brello.Models;
using Moq;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;

namespace Brello.Tests.Models
{
    [TestClass]
    public class BoardRepositoryTests
    {

        private Mock<BoardContext> mock_context;

        [TestInitialize]
        public void Initialize()
        {
            mock_context = new Mock<BoardContext>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            mock_context = null;
        }

        [TestMethod]
        public void BoardRepositoryEnsureICanCreateInstance()
        {
            BoardRepository board = new BoardRepository(mock_context.Object);
            Assert.IsNotNull(board);
        }
        
        [TestMethod]
        public void BoardRepositoryEnsureICanAddAList()
        {
            BoardRepository board_repo = new BoardRepository(mock_context.Object);
            BrelloList list = new BrelloList();
            Board board = new Board();

            bool actual = board_repo.AddList(board, list);

            Assert.AreEqual(1, board_repo.GetAllLists().Count);
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void BoardRepositoryEnsureThereAreZeroLists()
        {
            BoardRepository board_repo = new BoardRepository(mock_context.Object);
            
            int expected = 0;
            int actual = board_repo.GetAllLists().Count;
            Assert.AreEqual(expected, actual);
            
        }

        // These tests are telling us to start looking at
        // How to define CRUD operations for Boards
        // Why? Because a List cannot exists without a Board
        [TestMethod]
        public void BoardRepositoryEnsureABoardHasZeroLists()
        {
            /* Begin Arrange */
            var mock_boards = new Mock<DbSet<Board>>();
            ApplicationUser user1 = new ApplicationUser();
            var my_list = new List<Board> {
                new Board { Title = "Tim's Board", Owner = user1, BoardId = 1}
            }; // So I can use later
            var data = my_list.AsQueryable();

            mock_boards.As<IQueryable<Board>>().Setup(m => m.Provider).Returns(data.Provider);
            mock_boards.As<IQueryable<Board>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            mock_boards.As<IQueryable<Board>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mock_boards.As<IQueryable<Board>>().Setup(m => m.Expression).Returns(data.Expression);

            mock_context.Setup(m => m.Boards).Returns(mock_boards.Object);
            /* Begin Act */
            BoardRepository board_repo = new BoardRepository(mock_context.Object);
            /* Begin Assert */
            int expected = 0;
            Assert.AreEqual(expected, board_repo.GetAllLists(1).Count());
        }

        [TestMethod]
        public void BoardRepositoryCanGetABoard()
        {
            /* Begin Arrange */
            var mock_boards = new Mock<DbSet<Board>>();
            ApplicationUser user1 = new ApplicationUser();
            ApplicationUser user2 = new ApplicationUser();
            var my_list = new List<Board> {
                new Board { Title = "Tim's Board", Owner = user1},
                new Board {Title = "Sally's Board", Owner = user2 }
            }; // So I can use later
            var data = my_list.AsQueryable();

            mock_boards.As<IQueryable<Board>>().Setup(m => m.Provider).Returns(data.Provider);
            mock_boards.As<IQueryable<Board>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            mock_boards.As<IQueryable<Board>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mock_boards.As<IQueryable<Board>>().Setup(m => m.Expression).Returns(data.Expression);

            mock_context.Setup(m => m.Boards).Returns(mock_boards.Object);

            /* Leveraging the CreateBoard Method:
                 mock_boards.Setup(m => m.Add(It.IsAny<Board>())).Callback((Board b) => my_list.Add(b));
                 Board added_board = board_repo.CreateBoard(title, owner);
            */

            BoardRepository board_repository = new BoardRepository(mock_context.Object);
            /* Begin Act */
            List<Board> user_boards = board_repository.GetBoards(user1);
            /* Begin Assert */
            Assert.AreEqual(1, user_boards.Count());
            Assert.AreEqual("Tim's Board",user_boards.First().Title);
        }

        [TestMethod]
        public void BoardRepositoryCanGetBoardCount()
        {
            /* Begin Arrange */
            var mock_boards = new Mock<DbSet<Board>>();

            var my_list = new List<Board>(); // So I can use later

            var data = my_list.AsQueryable();

            //mock_boards.Object.Add(new Board { Title = "My Awesome Board", Owner = new ApplicationUser() });
           
            //var data = mock_boards.Object.AsQueryable();
            mock_boards.As<IQueryable<Board>>().Setup(m => m.Provider).Returns(data.Provider);
            mock_boards.As<IQueryable<Board>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            mock_boards.As<IQueryable<Board>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mock_boards.As<IQueryable<Board>>().Setup(m => m.Expression).Returns(data.Expression);

            mock_context.Setup(m => m.Boards).Returns(mock_boards.Object);
            //mock_context.Object.SaveChanges(); // This saves something to the Database
            BoardRepository board_repository = new BoardRepository(mock_context.Object);
            /* End Arrange */

            /* Begin Act */
            int actual = board_repository.GetBoardCount();
            /* End Act */

            /* Begin Assert */
            Assert.AreEqual(0, actual);
            /* End Assert */

            /* Begin Act */
            my_list.Add(new Board { Title = "My Awesome Board" });
            mock_boards.As<IQueryable<Board>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            /* End Act */

            /* Begin Assert */
            Assert.AreEqual(1, board_repository.GetBoardCount());
            /* End Assert */
        }

        [TestMethod]
        public void BoardRepositoryCanCreateBoard()
        {
            /* Begin Arrange */
            var mock_boards = new Mock<DbSet<Board>>();
            var my_list = new List<Board>();
            var data = my_list.AsQueryable();

            mock_boards.As<IQueryable<Board>>().Setup(m => m.Provider).Returns(data.Provider);
            mock_boards.As<IQueryable<Board>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            mock_boards.As<IQueryable<Board>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mock_boards.As<IQueryable<Board>>().Setup(m => m.Expression).Returns(data.Expression);

            // This allows BoardRepository to call Boards.Add and have it update the my_list instance and Enumerator
            // Connect DbSet.Add to List.Add so they work together
            mock_boards.Setup(m => m.Add(It.IsAny<Board>())).Callback((Board b) => my_list.Add(b));

            mock_context.Setup(m => m.Boards).Returns(mock_boards.Object);

            BoardRepository board_repo = new BoardRepository(mock_context.Object);
            string title = "My Awesome Board";
            ApplicationUser owner = new ApplicationUser();
            /* End Arrange */

            /* Begin Act */
            Board added_board = board_repo.CreateBoard(title, owner);
            /* End Act */

            /* Begin Assert */
            Assert.IsNotNull(added_board);
            mock_boards.Verify(m => m.Add(It.IsAny<Board>()));
            mock_context.Verify(x => x.SaveChanges(), Times.Once());
            Assert.AreEqual(1, board_repo.GetBoardCount());
            /* End Assert */
        }

        [TestMethod]
        public void BoardRepositoryEnsureICanGetAllBoards()
        {
            /* Begin Arrange */
            var mock_boards = new Mock<DbSet<Board>>();

            ApplicationUser owner = new ApplicationUser();

            // 1. Your data must be Queryable
            // 2. Mocks can only cast to an Interface (e.g. IQueryable, IDbSet, etc).
            // 3. You must ensure Provider, GetEnumerator(), ElementType, and Expression are defined
            //    with your collection class (the container class that holds your data).
            
            var data = new List<Board> {
                new Board { Title = "My Awesome Board", Owner = owner },
                new Board { Title = "My Other Awesome Board", Owner = owner }
            }.AsQueryable();
            
            mock_boards.As<IQueryable<Board>>().Setup(m => m.Provider).Returns(data.Provider);
            mock_boards.As<IQueryable<Board>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            mock_boards.As<IQueryable<Board>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mock_boards.As<IQueryable<Board>>().Setup(m => m.Expression).Returns(data.Expression);
            
            mock_context.Setup(m => m.Boards).Returns(mock_boards.Object);

            BoardRepository board_repo = new BoardRepository(mock_context.Object);
            /* End Arrange */

            /* Begin Act */
            List<Board> boards = board_repo.GetAllBoards();
            /* End Act */

            /* Begin Assert */
            Assert.AreEqual(2, boards.Count);
            /* End Assert */
        }
        
    }
}
