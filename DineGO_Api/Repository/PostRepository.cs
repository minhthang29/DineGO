using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DineGO_Api.Data;
using Core.Models;

namespace DineGO_Api.Repository
{
    public class PostRepository : IPostRepository
    {
        private readonly PostDAO _postDAO;
        public PostRepository(PostDAO postDAO)
        =>
            _postDAO = postDAO;

        public void DeletePost(int post)
        =>
            _postDAO.DeletePost(post);

        public Post FindPostById(int ID)
        =>
             _postDAO.FindPostById(ID);

        public List<Post> GetPosts()

            => _postDAO.GetPosts();

        public void SavePost(Post post)
        =>
            _postDAO.SavePost(post);

        public void UpdatePost(Post post)
        =>
            _postDAO.UpdatePost(post);
    }
}