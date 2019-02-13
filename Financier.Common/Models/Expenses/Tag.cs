using System;
using System.ComponentModel.DataAnnotations;

namespace Financier.Common.Models.Expenses
{
    public class Tag
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        private string _name;
        [Required]
        public string Name 
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value.Trim().ToLower();
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as Tag;
            if (other == null)
            {
                return false;
            }

            if ((Name ?? string.Empty) != (other.Name ?? string.Empty))
            {
                return false;
            }

            return true;
        }

        public static bool operator ==(Tag x, Tag y)
        {
            if (object.ReferenceEquals(x, null))
            {
                return (object.ReferenceEquals(y, null));
            }

            return x.Equals(y);
        }

        public static bool operator !=(Tag x, Tag y)
        {
            return !(x == y);
        }
    }
}
