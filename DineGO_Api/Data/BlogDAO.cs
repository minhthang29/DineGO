using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DineGO_Api.Data
{
    public class BlogDAO
    {
        private readonly ApplicationDbContext _context;

        public BlogDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all Blogs
        public List<Blog> GetBlogs()
        {
            try
            {
                return _context.Blogs.Where(x => !x.blog_is_deleted).ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Error fetching Blogs: {e.Message}");
            }
        }

        // Get Blog by ID
        public Blog FindBlogById(int id)
        {
            try
            {
                return _context.Blogs.SingleOrDefault(x => x.blog_id == id && !x.blog_is_deleted);
            }
            catch (Exception e)
            {
                throw new Exception($"Error finding Blog: {e.Message}");
            }
        }

        // Save a new Blog
        public void SaveBlog(Blog Blog)
        {
            try
            {
                _context.Blogs.Add(Blog);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"Error saving Blog: {e.Message}");
            }
        }

        // Update Blog details
        public void UpdateBlog(Blog Blog)
        {
            try
            {
                _context.Entry(Blog).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"Error updating Blog: {e.Message}");
            }
        }

        // Delete Blog by ID
        public void DeleteBlog(int id)
        {
            try
            {
                var Blog = _context.Blogs.SingleOrDefault(x => x.blog_id == id);
                if (Blog != null)
                {
                    Blog.blog_is_deleted = true;
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error deleting Blog: {e.Message}");
            }
        }
        
    }
}