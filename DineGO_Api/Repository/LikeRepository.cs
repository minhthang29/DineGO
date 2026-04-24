using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using DineGO_Api.Data;
using System.Threading.Tasks;
using Core.Models.Client.Custom;

namespace DineGO_Api.Repository
{
    public class LikeRepository : ILikeRepository
    {
        private readonly LikeDAO _likeDao;
        public LikeRepository(LikeDAO likeDao)
        {
            _likeDao = likeDao;
        }

        public void AddOrUpdateReaction(int postId, int cusId, int emotionType)
        {
            _likeDao.AddOrUpdateReaction(postId, cusId, emotionType);
        }

        public void RemoveReaction(int postId, int cusId)
        {
            _likeDao.RemoveReaction(postId, cusId);
        }

        public int CountReactionsByType(int postId, int emotionType)
        {
            return _likeDao.CountReactionsByType(postId, emotionType);
        }
        public List<Like> GetAll()
        {
            return _likeDao.GetAll();
        }
        public int CountLikes(int postId)
        {
            return _likeDao.CountLikes(postId);
        }
        public List<ReactionViewModel> GetReactionsByPost(int postId)
        {
            return _likeDao.GetReactionsByPost(postId);
        }
    }
}
