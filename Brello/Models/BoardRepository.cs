using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Brello.Models
{
    public class BoardRepository
    {
        private BoardContext context;
        public BoardRepository(BoardContext _context) {
            context = _context;
        }

        // void or bool or BrelloList
        public bool AddList(Board _board, BrelloList _list)
        {
            return false;
        }

        public List<BrelloList> GetAllLists()
        {
            return null;
        }

        // This is an example of overloading a method
        public List<BrelloList> GetAllLists(Board _board)
        {
            return null;
        }

        public Board CreateBoard(string title, ApplicationUser owner)
        {
            Board my_board = new Board { Title = title, Owner = owner};
            context.Boards.Add(my_board);
            context.SaveChanges(); // This saves something to the Database

            return my_board;
        }

        public List<Board> GetAllBoards()
        {
            return context.Boards.ToList();
        }
    }
}