using System;

namespace Essentials.Models
{
    public interface IAudited
    {
        public DateTimeOffset CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }

    }
}
