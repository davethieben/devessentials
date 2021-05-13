using System;
using System.Collections.Generic;
using System.Linq;

namespace Essentials.Models
{
    public interface IValidator<T>
        where T : class
    {
        IEnumerable<ValidationResult> Validate(T input);
    }

    public class ValidationResult
    {
        public bool IsError { get; set; }
        public string? Property { get; set; }
        public string? Message { get; set; }

    }

    public static class ValidationExtensions
    {
        public static IEnumerable<string>? ToStrings(this IEnumerable<ValidationResult> results) => 
            results?.Where(r => !string.IsNullOrEmpty(r.Message)).Select(r => r.Message.IsRequired());

    }
}
