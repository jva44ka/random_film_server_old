using Core.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("UserSettings")]
    public class UserSetting : IDataModel
    {
		[Key, Required]
		public Guid Id { get; set; }
		[Required]
		public string Name { get; set; }
		public string Type { get; set; }
		public string Value { get; set; }
		public Account User { get; set; }
	}
}
