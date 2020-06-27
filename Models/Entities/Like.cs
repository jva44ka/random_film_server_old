using System;
using System.Collections.Generic;

namespace randomfilm_backend.Models.Entities
{
    public partial class Like
    {
        public int Id { get; set; }
        public int FilmId { get; set; }
        public int AccountId { get; set; }
        public bool LikeOrDislike { get; set; }

        public virtual Account Account { get; set; }
        public virtual Film Film { get; set; }
    }
}
