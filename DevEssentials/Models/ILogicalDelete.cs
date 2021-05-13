using System;

namespace Essentials.Models
{
    public interface ILogicalDelete
    {
        public DateTimeOffset? DeletedOn { get; set; }
        public string? DeletedBy { get; set; }

    }
}
