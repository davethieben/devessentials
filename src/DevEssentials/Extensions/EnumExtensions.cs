using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;

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
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : value.ToString(CultureInfo.InvariantCulture);
        }

        public static string GetDisplayName<TEnum>(this TEnum value) where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            var fi = value.GetType().GetField(value.ToString(CultureInfo.InvariantCulture));
            var attributes = (DisplayAttribute[])fi.GetCustomAttributes(typeof(DisplayAttribute), false);
            return (attributes.Length > 0) ? attributes[0].GetName() : value.ToString(CultureInfo.InvariantCulture);
        }

    }
}
