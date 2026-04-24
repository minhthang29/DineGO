using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Core.Models;

namespace DineGO_Api.Repository
{
    // Interface xử lý các thao tác với Blog
    public interface IBlogRepositoy
    {
        List<Blog> GetBlogs();

        Blog FindBlogById(int ID);

        void SaveBlog(Blog blog);

        void UpdateBlog(Blog blog);

        void DeleteBlog(int blog);
    }
}