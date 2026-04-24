using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DineGO_Api.Data
{
    public class CommentDAO
    {
        private readonly ApplicationDbContext _context;

        public CommentDAO(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<Comment> GetComments()
        {
            try
            {
                return _context.Comments.ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Error fetching Comments: {e.Message}");
            }
        }

        public Comment FindCommentById(int id)
        {
            try
            {
                return _context.Comments.SingleOrDefault(x => x.comment_id == id);
            }
            catch (Exception e)
            {
                throw new Exception($"Error finding Comment: {e.Message}");
            }
        }

        public void SaveComment(Comment Comment)
        {
            try
            {
                _context.Comments.Add(Comment);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"Error saving Comment: {e.Message}");
            }
        }

        public void UpdateComment(Comment updated)
        {
            var existing = _context.Comments.FirstOrDefault(c => c.comment_id == updated.comment_id && c.cus_id == updated.cus_id);
            if (existing == null) return;
            existing.comment_content = updated.comment_content;
            existing.comment_updated_date = DateTime.Now;
            _context.SaveChanges();
        }


        public void DeleteComment(int id)
        {
            try
            {
                var Comment = _context.Comments.SingleOrDefault(x => x.comment_id == id);
                if (Comment != null)
                {
                    _context.Comments.Remove(Comment);
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error deleting Comment: {e.Message}");
            }
        }
    }
}