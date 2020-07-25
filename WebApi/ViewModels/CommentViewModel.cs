using System;

namespace WebApi.ViewModels
{
    public class CommentViewModel
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public DateTime? CreatedOn { get; set; }
        public AccountViewModel CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public AccountViewModel ModifiedBy { get; set; }

        public AccountViewModel Owner { get; set; }
        public virtual FilmViewModel Film { get; set; }
    }
}
