using System;
using System.Collections.Generic;

namespace randomfilm_backend.Models.Entities
{
    public partial class Comment
    {
        public int FilmId { get; set; }
        public int AccountId { get; set; }
        public string Text { get; set; }
        public int Id { get; set; }

        public virtual Account Account { get; set; }
        public virtual Film Film { get; set; }
    }
}
