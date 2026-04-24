using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DineGO_Api.Data
{
    public class PostDAO
    {
        private readonly ApplicationDbContext _context;

        public PostDAO(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<Post> GetPosts()
        {
            try
            {
                var posts = _context.Posts.ToList();
                // Lấy lượt like
                var likeCounts = _context.Likes
                    .Where(l => l.post_id != null)
                    .GroupBy(l => l.post_id)
                    .Select(g => new { PostId = g.Key, Count = g.Count() })
                    .ToList();
                // Lấy lượt comment
                var commentCounts = _context.Comments
                    .Where(c => c.post_id != null)
                    .GroupBy(c => c.post_id)
                    .Select(g => new { PostId = g.Key, Count = g.Count() })
                    .ToList();
                var likeDict = likeCounts.ToDictionary(x => x.PostId!, x => x.Count);
                var commentDict = commentCounts.ToDictionary(x => x.PostId!, x => x.Count);
                foreach (var post in posts)
                {
                    post.post_like_count = likeDict.TryGetValue(post.post_id, out var lcount) ? lcount : 0;
                    post.post_comment_count = commentDict.TryGetValue(post.post_id, out var ccount) ? ccount : 0;
                }
                return posts;
            }
            catch (Exception e)
            {
                throw new Exception($"Error fetching Posts: {e.Message}");
            }
        }

        public Post FindPostById(int id)
        {
            try
            {
                return _context.Posts.SingleOrDefault(x => x.post_id == id);
            }
            catch (Exception e)
            {
                throw new Exception($"Error finding Post: {e.Message}");
            }
        }

        public void SavePost(Post Post)
        {
            try
            {
                _context.Posts.Add(Post);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"Error saving Post: {e.Message}");
            }
        }

        public void UpdatePost(Post Post)
        {
            try
            {
                Post.post_is_approve = false; // Cập nhật lại trạng thái duyệt
                _context.Entry(Post).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"Error updating Post: {e.Message}");
            }
        }

        public void DeletePost(int id)
        {
            try
            {
                var post = _context.Posts
                    .Include(p => p.comments)
                    .Include(p => p.likes)
                    .FirstOrDefault(p => p.post_id == id);

                if (post != null)
                {
                    // Xoá comment nếu có
                    if (post.comments != null)
                        _context.Comments.RemoveRange(post.comments);

                    // Xoá like nếu có
                    if (post.likes != null)
                        _context.Likes.RemoveRange(post.likes);

                    // Xoá bài viết
                    _context.Posts.Remove(post);

                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error deleting Post: {e.Message}");
            }
        }
    }
}