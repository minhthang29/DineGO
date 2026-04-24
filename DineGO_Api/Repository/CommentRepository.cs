using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DineGO_Api.Data;
using Core.Models;

namespace DineGO_Api.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly CommentDAO _commentDAO;
        public CommentRepository(CommentDAO commentDAO)
        =>
            _commentDAO = commentDAO;

        public void DeleteComment(int comment)
        =>
            _commentDAO.DeleteComment(comment);

        public Comment FindCommentById(int ID)
        =>
             _commentDAO.FindCommentById(ID);

        public List<Comment> GetComments()

            => _commentDAO.GetComments();

        public void SaveComment(Comment comment)
        =>
            _commentDAO.SaveComment(comment);

        public void UpdateComment(Comment comment)
        =>
            _commentDAO.UpdateComment(comment);
    }
}