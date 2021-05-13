using System;

namespace Essentials
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    public abstract class BindInterfaceAttribute : Attribute
    {
        public BindInterfaceAttribute(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            ImplementingType = type;
        }

        public Type ImplementingType { get; }

    }

    /// <summary>
    /// declares that the service INTERFACE this attribute is applied to is implemented by
    /// the given type in a SINGLETON lifetime
    /// </summary>
    public class SingletonAttribute : BindInterfaceAttribute
    {
        public SingletonAttribute(Type type) : base(type) { }
    }

    /// <summary>
    /// declares that the service INTERFACE this attribute is applied to is implemented by
    /// the given type in a REQUEST lifetime
    /// </summary>
    public class RequestScopedAttribute : BindInterfaceAttribute
    {
        public RequestScopedAttribute(Type type) : base(type) { }
    }

    /// <summary>
    /// declares that the service INTERFACE this attribute is applied to is implemented by
    /// the given type in a TRANSIENT lifetime
    /// </summary>
    public class ServiceAttribute : BindInterfaceAttribute
    {
        public ServiceAttribute(Type type) : base(type) { }
    }

    /// <summary>
    /// declares that the implementation CLASS this attribute is applied to should be
    /// bound to ALL the interfaces it implements automatically in a SINGLETON lifetime
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class SingletonImplementationAttribute : Attribute { }

    /// <summary>
    /// declares that the implementation CLASS this attribute is applied to should be
    /// bound to ALL the interfaces it implements automatically in a REQUEST lifetime
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class RequestScopedImplementationAttribute : Attribute { }

    /// <summary>
    /// declares that the implementation CLASS this attribute is applied to should be
    /// bound to ALL the interfaces it implements automatically in a TRANSIENT lifetime
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ServiceImplementationAttribute : Attribute { }

}
