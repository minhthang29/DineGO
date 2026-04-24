using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Core.Models.Client.Custom;

namespace DineGO_Api.Data
{
    public class LikeDAO
    {
        private readonly ApplicationDbContext _context;

        public LikeDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Like> GetAll()
        {
            return _context.Likes.ToList();
        }


        public void AddOrUpdateReaction(int postId, int cusId, int emotionType)
        {
            var existing = _context.Likes.FirstOrDefault(x => x.post_id == postId && x.cus_id == cusId);
            if (existing != null)
            {
                existing.like_emotion_type = emotionType;
                _context.Likes.Update(existing);
            }
            else
            {
                _context.Likes.Add(new Like
                {
                    post_id = postId,
                    cus_id = cusId,
                    like_emotion_type = emotionType
                });
            }
            _context.SaveChanges();
        }

        public void RemoveReaction(int postId, int cusId)
        {
            var existing = _context.Likes.FirstOrDefault(x => x.post_id == postId && x.cus_id == cusId);
            if (existing != null)
            {
                _context.Likes.Remove(existing);
                _context.SaveChanges();
            }
        }

        public int CountReactionsByType(int postId, int emotionType)
        {
            return _context.Likes.Count(x => x.post_id == postId && x.like_emotion_type == emotionType);
        }

        public int CountLikes(int postId)
        {
            return _context.Likes.Count(l => l.post_id == postId);
        }

        public List<ReactionViewModel> GetReactionsByPost(int postId)
        {
            return _context.Likes
                .Where(l => l.post_id == postId && l.like_emotion_type > 0)
                .Join(_context.Customers,
                    like => like.cus_id,
                    cus => cus.cus_id,
                    (like, cus) => new ReactionViewModel
                    {
                        CustomerName = cus.cus_name,
                        CustomerImage = cus.cus_image,
                        EmotionType = like.like_emotion_type ?? 0
                    }).ToList();
        }
    }
}