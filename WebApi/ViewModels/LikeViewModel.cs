using System;

namespace WebApi.ViewModels
{
    public class LikeViewModel
    {
        public Guid Id { get; set; }
        public bool IsLike { get; set; }
        public DateTime? CreatedOn { get; set; }

        public virtual AccountViewModel Owner { get; set; }
        public virtual FilmViewModel Film { get; set; }
    }
}
