using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DineGO_Api.Data;
using Core.Models;

namespace DineGO_Api.Repository
{
    public class BlogRepository : IBlogRepositoy
    {
        private readonly BlogDAO _blogDAO;
        public BlogRepository(BlogDAO blogDAO)
        =>
            _blogDAO = blogDAO;

        public void DeleteBlog(int blog)
        =>
            _blogDAO.DeleteBlog(blog);

        public Blog FindBlogById(int ID)
        =>
             _blogDAO.FindBlogById(ID);

        public List<Blog> GetBlogs()

            => _blogDAO.GetBlogs();

        public void SaveBlog(Blog blog)
        =>
            _blogDAO.SaveBlog(blog);

        public void UpdateBlog(Blog blog)
        =>
            _blogDAO.UpdateBlog(blog);
    }
}