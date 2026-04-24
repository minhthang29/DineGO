using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;

public class PostWithUserEmotion
{
    public Post post { get; set; }
    public int? user_emotion_type { get; set; } // null nếu chưa like
}
