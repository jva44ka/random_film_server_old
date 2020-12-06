using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("FilmSelectionList")]
    public class FilmSelectionList
    {
        [Key, Required]
        public Guid Id { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        [ForeignKey(nameof(SelectionList))]
        public Guid SelectionListId { get; set; }
        public int Order { get; set; }

        public virtual Account User { get; set; }
        public virtual SelectionList SelectionList { get; set; }
    }
}
