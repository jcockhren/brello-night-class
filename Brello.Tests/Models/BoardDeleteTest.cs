using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using Brello.Models;
using System.Collections.Generic;
using System.Data.Entity;

namespace Brello.Tests.Models
{
    [TestClass]
    public class BoardDeleteTest { 

    private Mock<BoardContext> mock_context;
    private Mock<DbSet<Board>> mock_boards; 
    private List<Board> my_list;
    private ApplicationUser owner, user1, user2;

        private void ConnectMocksToDataSource()
        {
            // This setups the Mocks and connects to the Data Source (my_list in this case)
            var data = my_list.AsQueryable();

            mock_boards.As<IQueryable<Board>>().Setup(m => m.Provider).Returns(data.Provider);
            mock_boards.As<IQueryable<Board>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            mock_boards.As<IQueryable<Board>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mock_boards.As<IQueryable<Board>>().Setup(m => m.Expression).Returns(data.Expression);

            mock_context.Setup(m => m.Boards).Returns(mock_boards.Object);
        }

        [TestInitialize]
        public void Initialize()
        {
            mock_context = new Mock<BoardContext>();
            mock_boards = new Mock<DbSet<Board>>();
            my_list = new List<Board>();
            owner = new ApplicationUser();
            user1 = new ApplicationUser();
            user2 = new ApplicationUser();

        }

        [TestCleanup]
        public void Cleanup()
        {
            mock_context = null;
            mock_boards = null;
            my_list = null;
        }

        [TestMethod]
        public void EnsureICanDeleteABoard()
        {
            /* Begin Arrange */
            var data = my_list.AsQueryable();

            mock_boards.As<IQueryable<Board>>().Setup(m => m.Provider).Returns(data.Provider);
            mock_boards.As<IQueryable<Board>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            mock_boards.As<IQueryable<Board>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mock_boards.As<IQueryable<Board>>().Setup(m => m.Expression).Returns(data.Expression);

            
            mock_boards.Setup(m => m.Add (It.IsAny<Board>())).Callback((Board b) => my_list.Add(b));
            mock_boards.Setup(m => m.Remove(It.IsAny<Board>())).Callback((Board b) => my_list.Remove(b));
            mock_context.Setup(m => m.Boards).Returns(mock_boards.Object);

            BoardRepository board_repo = new BoardRepository(mock_context.Object);
            string title = "My Awesome Board";
            /* End Arrange */

            /* Begin Act */
            Board removed_board = board_repo.CreateBoard(title, owner);
            /* End Act */

            /* Begin Assert */
            Assert.IsNotNull(removed_board);
            mock_boards.Verify(m => m.Add(It.IsAny<Board>()));
            mock_context.Verify(x => x.SaveChanges(), Times.Once());
            Assert.AreEqual(1, board_repo.GetBoardCount());
            board_repo.DeleteBoard(removed_board);
            mock_boards.Verify(x => x.Remove(It.IsAny<Board>()));
            mock_context.Verify(x => x.SaveChanges(), Times.Exactly(2));
            Assert.AreEqual(0, board_repo.GetBoardCount());

            /* End Assert */
        }
    }
}
