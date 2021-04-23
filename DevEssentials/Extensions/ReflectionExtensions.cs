using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Essentials.Reflection
{
    public static class ReflectionExtensions
    {
        public static bool HasAttribute<T>(this ICustomAttributeProvider type)
        {
            return !type.GetCustomAttributes(typeof(T), true).IsNullOrEmpty();
        }

        public static T? GetAttribute<T>(this ICustomAttributeProvider type)
            where T : Attribute
        {
            return type.GetCustomAttributes(typeof(T), true)?.FirstOrDefault() as T;
        }

        public static bool Is<T>(this Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }

        public static TResult GetProperty<T, TResult>(this T target, Expression<Func<T, TResult>> expr)
        {
            var memberInfo = (PropertyInfo)((MemberExpression)expr.Body).Member;
            return (TResult)memberInfo.GetMethod.Invoke(target, null);
        }

        public static TResult GetPropertyValue<TResult>(this object target, string propertyName)
        {
            target.IsRequired();

            PropertyInfo property = target.GetType().GetProperty(propertyName);
            if (property != null)
            {
                if (!typeof(TResult).IsAssignableFrom(property.PropertyType))
                    throw new ArgumentException($"Invalid type argument. Property is Type: {property.PropertyType}");

                object value = property.GetValue(target);
                if (value != null)
                    return (TResult)value;
            }

            return default!;
        }

        public static void SetProperty<T, TResult>(this T target, Expression<Func<T, TResult>> expr, TResult value)
        {
            var memberInfo = (PropertyInfo)((MemberExpression)expr.Body).Member;
            memberInfo.SetMethod.Invoke(target, new object?[] { value });
        }

        public static bool IsNullableType(this Type type)
        {
            return type.GetTypeInfo().IsGenericType
                && type.GetGenericTypeDefinition().IsAssignableFrom(typeof(Nullable<>));
        }

        public static bool IsNumber(this Type type)
        {
            return type.Is<short>()
                || type.Is<int>()
                || type.Is<long>()
                || type.Is<float>()
                || type.Is<double>()
                || type.Is<decimal>();
        }

        public static bool IsCollection(this Type type)
        {
            Type enumerableType = ExtractGenericInterface(type, typeof(IEnumerable<>));
            return enumerableType != null;
        }

        public static bool IsDictionary(this Type type)
        {
            Type enumerableType = ExtractGenericInterface(type, typeof(IDictionary<,>));
            return enumerableType != null;
        }

        private static Type ExtractGenericInterface(Type queryType, Type interfaceType)
        {
            bool matchesInterface(Type t) => t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == interfaceType;

            if (matchesInterface(queryType))
                return queryType;
            else
                return queryType.GetInterfaces().FirstOrDefault(matchesInterface);
        }

        /// <summary>
        /// merges two regular System.objects into a single dynamic object
        /// </summary>
        public static dynamic Merge(this object item1, params object[] items)
        {
            var expando = new ExpandoObject();

            if (item1 != null)
                item1.CopyToDictionary(expando);

            foreach (var itemX in items.EmptyIfNull())
                if (itemX != null)
                    itemX.CopyToDictionary(expando);

            return expando;
        }

        /// <summary>
        /// creates a new Dictionary with the values from "target" and copies values from "source" over top.
        /// </summary>
        /// <param name="target">first dictonary of values</param>
        /// <param name="source">second dictionary of values</param>
        /// <returns>new dictionary containing the merged result values</returns>
        public static IDictionary<TKey, TValue> Merge<TKey, TValue>(this IDictionary<TKey, TValue> target, IDictionary<TKey, TValue> source)
            where TKey : notnull
        {
            IEqualityComparer<TKey> comparer = EqualityComparer<TKey>.Default;

            if (target == null)
                target = new Dictionary<TKey, TValue>();

            if (target is Dictionary<TKey, TValue> dictionary)
                comparer = dictionary.Comparer;

            var output = new Dictionary<TKey, TValue>(target, comparer);

            foreach (KeyValuePair<TKey, TValue> kvp in source.EmptyIfNull())
            {
                output[kvp.Key] = kvp.Value;
            }

            return output;
        }

        public static dynamic ToObject(this IDictionary<string, string> dictionary)
        {
            var expando = new ExpandoObject();

            if (dictionary != null)
                dictionary.CopyToDictionary(expando);

            return expando;
        }

        /// <summary>
        /// copies all elements of the target into the destination dictionary. if the target is a true object,
        /// it will copy all the public properties. if the target is a dictionary, does a dictionary copy.
        /// </summary>
        public static void CopyToDictionary(this object target, IDictionary<string, object> destination)
        {
            if (target == null)
                return;

            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            if (target.GetType().IsDictionary() ||
                typeof(IEnumerable<KeyValuePair<string, object>>).IsAssignableFrom(target.GetType()))
            {
                var dictionary = target as IEnumerable<KeyValuePair<string, object>>;
                foreach (KeyValuePair<string, object> kvp in dictionary.EmptyIfNull())
                {
                    if (kvp.Key.HasValue())
                    {
                        destination[kvp.Key] = kvp.Value;
                    }
                }
            }
            else
            {
                foreach (PropertyInfo property in target.GetType().GetProperties())
                {
                    destination[property.Name] = property.GetValue(target, null);
                }
            }
        }

        public static IDictionary<string, object> ToDictionary(this object target)
        {
            IDictionary<string, object> destination = new Dictionary<string, object>();
            target.CopyToDictionary(destination);
            return destination;
        }

        /// <summary>
        /// copies all public properties from one object to another of the same type. similar to Clone 
        /// but with using an existing object.
        /// </summary>
        public static void CopyTo<T>(this T source, T destination)
            where T : class
        {
            if (source == null || destination == null)
                return;

            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanRead && p.CanWrite);
            foreach (var property in properties)
            {
                object value = property.GetValue(source);
                property.SetValue(destination, value);
            }
        }

        public static IEnumerable<Assembly> DistinctAssemblies(this IEnumerable<Assembly> assemblies) => assemblies.Distinct(AssemblyNameComparer.IgnoreCaseIgnoreVersions);

        public static IEnumerable<Assembly> Load(this IEnumerable<AssemblyName> assemblyNames) => assemblyNames.Select(name => Assembly.Load(name));

        public static string GetDisplayName(this Assembly assembly, bool includeTimestamp = false)
        {
            string timestamp = includeTimestamp ? " - " + assembly.GetTimestamp()?.ToString() : "";

            return $"{assembly.GetName().Name} v{assembly.GetVersion()}{timestamp}";
        }

        public static DateTime? GetTimestamp(this Assembly assembly)
        {
            assembly.IsRequired();

            var attr = assembly.GetAttribute<AssemblyTimestampAttribute>();
            if (attr != null && attr.Timestamp != DateTime.MinValue)
                return attr.Timestamp;

            var assemblyFile = new FileInfo(assembly.Location);
            if (assemblyFile.Exists)
                return assemblyFile.LastWriteTime;

            return null;
        }

        public static string GetVersion(this Assembly assembly, bool includeRevision = true)
        {
            assembly.IsRequired();

            Version version;
            var versionAttribute = assembly.GetAttribute<AssemblyFileVersionAttribute>();
            if (versionAttribute != null)
                version = new Version(versionAttribute.Version);
            else
                version = assembly.GetName().Version;

            int fieldCount = includeRevision ? 4 : 3;
            return version.ToString(fieldCount);
        }

        public static Version RevisionInsensitive(this Version version)
        {
            return new Version(version.Major, version.Minor, version.Build);
        }

        /// <summary>
        /// build the Type name ourselves because we do not want version information to be stored 
        /// when the implementing assembly's version may change after an app update
        /// </summary>
        public static string VersionInsensitiveAssemblyQualifiedName(this Type type)
            => $"{type.FullName}, {type.GetTypeInfo().Assembly.GetName().Name}";

        public static string GetFullName(this MethodBase method)
        {
            var parameters = method.GetParameters().Select(x => x.ParameterType.Name + " " + x.Name);
            return $"{method.DeclaringType?.FullName}.{method.Name}({string.Join(", ", parameters)})";
        }

        public static string GetDisplayName(this Type type, bool includeNamespaces = true)
        {
            if (type == null)
                return string.Empty;

            string typeName = includeNamespaces ? type.FullName.SubstringBefore("`") : type.Name.SubstringBefore("`");

            if (type.GenericTypeArguments.Any())
            {
                typeName += "<" + string.Join(",", type.GenericTypeArguments.Select(argType => argType.GetDisplayName(includeNamespaces))) + ">";
            }

            return typeName;
        }

        public static void Load<T>(this Lazy<T> lazy)
        {
            if (!lazy.IsValueCreated)
                Task.Run(() => lazy.Value?.ToString())
                    .AndForget();
        }

        public static string GetCallStackSummary(int skipFrames = 1)
        {
            bool Include((int index, MethodBase method) item)
            {
                //if (item.index > 10)
                //    return false;

                string? @namespace = item.method?.DeclaringType?.Namespace;
                return @namespace != null
                    && !_ignoreNamespaces.Any(ns => @namespace.StartsWith(ns));
            }

            var methodCalls = new StackTrace(skipFrames)
                .GetFrames()
                .Select((f, index) => (index: index, method: f.GetMethod()))
                .Where(m => Include(m))
                .Select(m => $"{m.method.DeclaringType?.FullName}.{m.method.Name}");

            return string.Join(", ", methodCalls);
        }

        private static readonly string[] _ignoreNamespaces = new[] {
                    "System.Linq",
                    "System.Runtime",
                    "System.Threading",
                    "System.IO.Pipelines",
                    "Microsoft.AspNetCore.Server.Kestrel",
                    "Microsoft.AspNetCore.Hosting.Internal",
                    "Microsoft.AspNetCore.Diagnostics",
                    "Microsoft.AspNetCore.Builder",
                    "Essentials"
                };

    }

    public class AssemblyNameComparer : IEqualityComparer<Assembly>, IEqualityComparer<AssemblyName>, IComparer<AssemblyName>
    {
        public static AssemblyNameComparer IgnoreCaseIgnoreVersions = new AssemblyNameComparer();
        public static AssemblyNameComparer IgnoreCaseCompareVersions = new AssemblyNameComparer(compareVersions: true);

        private readonly bool _compareVersions;
        private readonly bool _revisionInsensitive;

        private AssemblyNameComparer(bool compareVersions = false, bool revisionInsensitive = true)
        {
            _compareVersions = compareVersions;
            _revisionInsensitive = revisionInsensitive;
        }

        public bool Equals(Assembly x, Assembly y)
        {
            return Equals(x.GetName(), y.GetName());
        }

        public int GetHashCode(Assembly obj)
        {
            return GetHashCode(obj.GetName());
        }

        public bool Equals(AssemblyName x, AssemblyName y)
        {
            if (string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(x.CultureName ?? "", y.CultureName ?? "", StringComparison.OrdinalIgnoreCase))
            {
                if (_compareVersions)
                {
                    if (_revisionInsensitive)
                    {
                        return x.Version.RevisionInsensitive().Equals(y.Version.RevisionInsensitive());
                    }
                    else
                    {
                        return x.Version.Equals(y.Version);
                    }
                }
                else
                    return true;
            }

            return false;
        }

        public int GetHashCode(AssemblyName obj)
        {
            var hashCode = 0;
            if (obj.Name != null)
            {
                hashCode ^= obj.Name.ToUpperInvariant().GetHashCode();
            }

            if (obj.Version != null && _compareVersions)
            {
                hashCode ^= obj.Version.GetHashCode();
            }

            hashCode ^= (obj.CultureName?.ToUpperInvariant() ?? "").GetHashCode();
            return hashCode;
        }

        public int Compare(AssemblyName x, AssemblyName y)
        {
            if (x == null && y != null)
                return 1;
            else if (x != null && y == null)
                return -1;
            else if (x == null && y == null)
                return 0;

            x = x.IsRequired();
            y = y.IsRequired();

            int result = x.Name.CompareTo(y.Name);
            if (result != 0)
                return result;

            if (_compareVersions)
            {
                if (_revisionInsensitive)
                {
                    return x.Version.RevisionInsensitive().CompareTo(y.Version.RevisionInsensitive());
                }
                else
                {
                    return x.Version.CompareTo(y.Version);
                }
            }

            return 0;
        }

    }
}
