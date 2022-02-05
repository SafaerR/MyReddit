using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace MyReddit.Models
{
    public partial class Post
    {
        public Post()
        {
            Comments = new HashSet<Comment>();
        }

        public int Id { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy HH:mm:ss}")]

        public DateTime? PublicationDate { get; set; }
        public int? Upvote { get; set; }
        public int? DownVote { get; set; }
        public int? UserId { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
