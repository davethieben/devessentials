using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
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
        public static Attribute[] GetCustomAttributes<TEnum>(this TEnum value)
            where TEnum : struct, IConvertible
        {
            Contract.IsRequired(value, nameof(value));

            Type type = value.GetType();
            if (!type.IsEnum)
                throw new ArgumentException($"Type '{type}' must be an enum");

            // Get the enum field.
            FieldInfo? field = type.GetField(value.ToString(CultureInfo.InvariantCulture));
            if (field != null)
            {
                return Attribute.GetCustomAttributes(field);
            }
            return Array.Empty<Attribute>();
        }

        public static TAttr? GetCustomAttribute<TEnum, TAttr>(this TEnum value)
            where TEnum : struct, IConvertible
            where TAttr : Attribute
        {
            return value.GetCustomAttributes().OfType<TAttr>().FirstOrDefault();
        }

        public static object? GetCustomAttribute<TEnum>(this TEnum value, Type attrType)
            where TEnum : struct, IConvertible
        {
            return value.GetCustomAttributes().FirstOrDefault(a => attrType.IsAssignableFrom(a.GetType()));
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
