using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DineGO_Api.Data;
using Core.Models;
using Core.Models.Client.Custom;
namespace DineGO_Api.Repository
{
    public interface ILikeRepository
    {
        void AddOrUpdateReaction(int postId, int cusId, int emotionType);
        void RemoveReaction(int postId, int cusId);
        int CountReactionsByType(int postId, int emotionType);
        List<Like> GetAll();
        int CountLikes(int postId);
        List<ReactionViewModel> GetReactionsByPost(int postId);
    }
}