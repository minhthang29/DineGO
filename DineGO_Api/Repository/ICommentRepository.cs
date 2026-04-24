using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Core.Models;

namespace DineGO_Api.Repository
{
    public interface ICommentRepository
    {
        List<Comment> GetComments();

        Comment FindCommentById(int ID);

        void SaveComment(Comment comment);

        void UpdateComment(Comment comment);

        void DeleteComment(int comment);
    }
}