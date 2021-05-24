using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Essentials.Reflection;

namespace Essentials
{
    public static class EnumExtensions
    {
        public static TAttr[] GetValues<TAttr>() => Enum.GetValues(typeof(TAttr)).Cast<TAttr>().ToArray();

        /// <summary>
        /// retrieves a Custom Attribute if its applied to a VALUE of an Enum
        /// </summary>
        /// <example>
        /// enum Example {
        ///     [Display("Hello")]
        ///     World
        /// }
        /// </example>
        /// <typeparam name="TAttribute">Type of the Attribute to try to get</typeparam>
        /// <param name="value">value to look for Attributes on</param>
        /// <returns>the Attribute if found, otherwise null</returns>
        public static TAttribute? GetEnumValueAttribute<TAttribute>(object value) where TAttribute : Attribute
        {
            Contract.IsRequired(value, nameof(value));

            Type type = value.GetType();
            if (!type.IsEnum)
                throw new ArgumentException($"Type {type} is not an enum");

            // Get the enum field.
            FieldInfo field = type.GetField(value.ToString());
            return field?.GetCustomAttribute<TAttribute>();
        }

        public static string GetDescription<TEnum>(this TEnum value) where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            var fi = value.GetType().GetField(value.ToString(CultureInfo.InvariantCulture));
            var attribute = fi.GetAttribute<DescriptionAttribute>();
            if (attribute != null && attribute.Description is string description && description != null && !string.IsNullOrEmpty(description))
                return description;

            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static string GetDisplayName<TEnum>(this TEnum value) where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            var fi = value.GetType().GetField(value.ToString(CultureInfo.InvariantCulture));
            var attribute = fi.GetAttribute<DisplayAttribute>();
            if (attribute != null && attribute.GetName() is string name && name != null && !string.IsNullOrEmpty(name))
                return name;

            return value.ToString(CultureInfo.InvariantCulture);
        }

    }
}
