using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Core.Models;

namespace DineGO_Api.Repository
{
    public interface IPostRepository
    {
        List<Post> GetPosts();

        Post FindPostById(int ID);

        void SavePost(Post post);

        void UpdatePost(Post post);

        void DeletePost(int post);
    }
}