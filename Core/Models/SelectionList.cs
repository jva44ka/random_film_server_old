using Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("SelectionList")]
    public class SelectionList
    {
        [Key, Required]
        public Guid Id { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public AlgorithmType AlgorithmType { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual Account User { get; set; }
        public virtual ICollection<FilmSelectionList> FilmSelectionLists { get; set; }
    }
}
