using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Essentials;

namespace AutoMapper
{
    public static class AutoMapperExtensions
    {

        public static MapExpression<TSource> Map<TSource>(this IMapper mapper, TSource source)
        {
            return new MapExpression<TSource>(mapper, source);
        }

        public class MapExpression<TSource>
        {
            private readonly IMapper _mapper;
            private readonly TSource _source;
            private readonly IDictionary<string, object> _items;

            public MapExpression(IMapper mapper, TSource source)
            {
                _mapper = mapper.IsRequired();
                _source = source;
                _items = new Dictionary<string, object>();
            }

            public MapExpression<TSource> With(params object[] arguments)
            {
                foreach (object argument in arguments.EmptyIfNull())
                {
                    if (argument != null)
                    {
                        if (argument is KeyValuePair<string, object>)
                        {
                            _items.Add((KeyValuePair<string, object>)argument);
                        }
                        else
                        {
                            _items.Set(argument);
                        }
                    }
                }

                return this;
            }

            public MapExpression<TSource> With(string key, object value)
            {
                _items.Add(key, value);
                return this;
            }

            public TDest To<TDest>(Action<TSource, TDest?>? afterMap = null)
            {
                return _mapper.Map<TSource, TDest>(_source,
                    options =>
                    {
                        foreach (var kvp in _items)
                            options.Items.Add(kvp);

                        if (afterMap != null)
                            options.AfterMap(afterMap);
                    });
            }

            public TDest To<TDest>(TDest destination)
            {
                return _mapper.Map<TSource, TDest>(_source, destination,
                    options =>
                    {
                        foreach (var kvp in _items)
                            options.Items.Add(kvp);
                    });
            }
        }


        public static CreateMapExpression<TSource, TDestination, TMember> Map<TSource, TDestination, TMember>(this IMappingExpression<TSource, TDestination> expression,
            Expression<Func<TSource, TMember>> sourceMember)
        {
            return new CreateMapExpression<TSource, TDestination, TMember>(expression, sourceMember);
        }

        public class CreateMapExpression<TSource, TDestination, TMember>
        {
            private readonly IMappingExpression<TSource, TDestination> _expression;
            private readonly Expression<Func<TSource, TMember>> _sourceMember;

            public CreateMapExpression(IMappingExpression<TSource, TDestination> expression,
                Expression<Func<TSource, TMember>> sourceMember)
            {
                _expression = expression.IsRequired();
                _sourceMember = sourceMember.IsRequired();
            }

            public IMappingExpression<TSource, TDestination> To(Expression<Func<TDestination, object?>> destinationMember)
            {
                return _expression.ForMember(destinationMember, options => options.MapFrom(_sourceMember));
            }
        }


        public static CreateMapWithExpression<TSource, TDestination> MapWith<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression,
            Func<ResolutionContext, Expression<Func<TSource, object>>> sourceMember)
        {
            return new CreateMapWithExpression<TSource, TDestination>(expression, sourceMember);
        }

        public static CreateMapWithExpression<TSource, TDestination> MapWith<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression,
            Func<ResolutionContext, object> resolver)
        {
            return new CreateMapWithExpression<TSource, TDestination>(expression, context => __ => resolver(context));
        }

        public class CreateMapWithExpression<TSource, TDestination>
        {
            private readonly IMappingExpression<TSource, TDestination> _expression;
            private readonly Func<ResolutionContext, Expression<Func<TSource, object>>> _mapping;

            public CreateMapWithExpression(IMappingExpression<TSource, TDestination> expression,
                Func<ResolutionContext, Expression<Func<TSource, object>>> mapping)
            {
                _expression = expression.IsRequired();
                _mapping = mapping.IsRequired();
            }

            public IMappingExpression<TSource, TDestination> To(Expression<Func<TDestination, object?>> destinationMember)
            {
                return _expression.ForMember(destinationMember,
                    options =>
                    {
                        options.ResolveUsing((source, dest, member, context) =>
                        {
                            Expression<Func<TSource, object>> sourceExpression = _mapping.Invoke(context);
                            return sourceExpression.Compile().Invoke(source);
                        });
                    });
            }

        }

        public static CreateMapWithFunc<TSource, TDestination> MapWithFunc<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression,
            Func<IDictionary<string, object>, Func<TSource, object>> sourceMember)
        {
            return new CreateMapWithFunc<TSource, TDestination>(expression, sourceMember);
        }

        public class CreateMapWithFunc<TSource, TDestination>
        {
            private readonly IMappingExpression<TSource, TDestination> _expression;
            private readonly Func<IDictionary<string, object>, Func<TSource, object>> _sourceMember;

            public CreateMapWithFunc(IMappingExpression<TSource, TDestination> expression,
                Func<IDictionary<string, object>, Func<TSource, object>> sourceMember)
            {
                _expression = expression.IsRequired();
                _sourceMember = sourceMember.IsRequired();
            }

            public IMappingExpression<TSource, TDestination> To(Expression<Func<TDestination, object?>> destinationMember)
            {
                return _expression.ForMember(destinationMember,
                    options =>
                    {
                        options.ResolveUsing((source, dest, member, context) =>
                        {
                            Func<TSource, object> sourceExpression = _sourceMember.Invoke(context.Items);
                            return sourceExpression.Invoke(source);
                        });
                    });
            }

        }

        public static ResolveExpression<TSource, TDestination, TMember> MapFrom<TSource, TDestination, TMember>(this IMemberConfigurationExpression<TSource, TDestination, TMember> expr)
            => new ResolveExpression<TSource, TDestination, TMember>(expr);

        public class ResolveExpression<TSource, TDestination, TMember>
        {
            private readonly IMemberConfigurationExpression<TSource, TDestination, TMember> _memberConfig;

            public ResolveExpression(IMemberConfigurationExpression<TSource, TDestination, TMember> memberConfig)
            {
                _memberConfig = memberConfig.IsRequired();
            }

            public void WithServices<TService1, TResult>(Func<TSource, TService1, TResult> resolver)
                where TService1 : class
            {
                _memberConfig.ResolveUsing((source, dest, member, context) =>
                    {
                        context.IsRequired()
                            .Options.IsRequired();

                        TService1 service1 = context.Options.CreateInstance<TService1>(); //.ServiceCtor.Invoke(typeof(TService1)) as TService1;

                        return resolver(source, service1);
                    });
            }

            public void WithServices<TService1, TService2, TResult>(Func<TSource, TService1, TService2, TResult> resolver)
                where TService1 : class
                where TService2 : class
            {
                _memberConfig.ResolveUsing((source, dest, member, context) =>
                    {
                        context.IsRequired()
                            .Options.IsRequired();

                        TService1 service1 = context.Options.CreateInstance<TService1>();
                        TService2 service2 = context.Options.CreateInstance<TService2>();

                        return resolver(source, service1, service2);
                    });
            }

        }

        public static IMappingExpression<TSource, TDestination> Ignore<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression,
            Expression<Func<TDestination, object?>> member)
        {
            return expression.ForMember(member, options => options.Ignore());
        }

        public static Expression<Func<TInput, object>> ToUntypedPropertyExpression<TInput, TOutput>(this Expression<Func<TInput, TOutput>> expression)
        {
            var memberName = ((MemberExpression)expression.Body).Member.Name;

            var param = Expression.Parameter(typeof(TInput));
            var field = Expression.Property(param, memberName);
            return Expression.Lambda<Func<TInput, object>>(field, param);
        }


        /// <summary>
        /// AutoMapper does not handle mapping child collections to existing objects very well. It will 
        /// create a new collection with the source data and map the child objects into the new collection. 
        /// This confuses Entity Framework because it is not tracking the new items. Therefore, this
        /// extension method uses the existing items on the destination (EF) collection and maps each object within
        /// the collection. It will correctly remove items that are removed from the source collection and add new
        /// objects that are added.
        /// </summary>
        /// <remarks>
        /// This method assumes collections are implemented by IList&lt;T&gt;. If you need others, create overloads.
        /// </remarks>
        /// <typeparam name="TSourceParent">Type of the source object being mapped in the CreateMap&lt;TSource, TDestination&gt; call</typeparam>
        /// <typeparam name="TSourceChild">Type of the object in the source collection</typeparam>
        /// <typeparam name="TDestinationParent">Type of the source object being mapped in the CreateMap&lt;TSource, TDestination&gt; call</typeparam>
        /// <typeparam name="TDestinationChild">Type of the object in the destination collection</typeparam>
        /// <param name="expression">AutoMapper mapping expression to extend</param>
        /// <param name="sourceCollection">selector expression for the source object to get the child collection</param>
        /// <param name="sourceKeySelector">selector for the source child object to get an equality key</param>
        /// <param name="destinationCollection">selector expression for the destination object to get the child collection</param>
        /// <param name="destinationKeySelector">selector for the destination child object to get an equality key</param>
        /// <param name="destinationFactory">Func to create a new destination child object if one has been added to the source collection</param>
        /// <returns>the mapping expression</returns>
        public static IMappingExpression<TSourceParent, TDestinationParent> MapCollection<TSourceParent, TSourceChild, TDestinationParent, TDestinationChild>(
            this IMappingExpression<TSourceParent, TDestinationParent> expression,
            Expression<Func<TSourceParent, IList<TSourceChild>>> sourceCollection,
            Func<TSourceChild, object> sourceKeySelector,
            Expression<Func<TDestinationParent, IList<TDestinationChild>>> destinationCollection,
            Func<TDestinationChild, object> destinationKeySelector,
            Func<TSourceChild, TDestinationChild>? destinationFactory = null)
        {
            var resolver = new CollectionResolver<TSourceParent, TDestinationParent, TSourceChild, TDestinationChild>(sourceKeySelector, destinationKeySelector, destinationFactory);

            return expression.ForMember(destinationCollection.ToUntypedPropertyExpression(),
                config => config.ResolveUsing(resolver, sourceCollection.ToUntypedPropertyExpression()));
        }

        private class CollectionResolver<TSourceParent, TDestinationParent, TSourceChild, TDestinationChild>
            : IMemberValueResolver<TSourceParent, TDestinationParent, object, object>
        {
            private readonly Func<TSourceChild, object> _sourceKeySelector;
            private readonly Func<TDestinationChild, object> _destinationKeySelector;
            private readonly Func<TSourceChild, TDestinationChild>? _destinationFactory;

            public CollectionResolver(
                Func<TSourceChild, object> sourceKeySelector,
                Func<TDestinationChild, object> destinationKeySelector,
                Func<TSourceChild, TDestinationChild>? destinationFactory)
            {
                _sourceKeySelector = sourceKeySelector.IsRequired();
                _destinationKeySelector = destinationKeySelector.IsRequired();
                _destinationFactory = destinationFactory;
            }

            public object Resolve(TSourceParent source, TDestinationParent destination, object sourceMember, object destMember, ResolutionContext context)
            {
                var mapper = context.Mapper;

                var sourceValues = sourceMember as IEnumerable<TSourceChild>;
                var destinationValues = destMember as IEnumerable<TDestinationChild>;

                if (destinationValues.IsNullOrEmpty())
                    destinationValues = new List<TDestinationChild>();

                var defaultConstructor = typeof(TDestinationChild).GetConstructor(Type.EmptyTypes);
                if (defaultConstructor == null)
                    throw new InvalidOperationException($"if type '{typeof(TDestinationChild)}' does not have a default constructor, you must provide the 'destinationFactory' argument");

                var output = new List<TDestinationChild>();
                foreach (TSourceChild sourceValue in sourceValues.EmptyIfNull())
                {
                    object key = _sourceKeySelector(sourceValue);
                    TDestinationChild destinationValue = destinationValues.FirstOrDefault(value => _destinationKeySelector(value).Equals(key));
                    if (destinationValue == null)
                    {
                        if (_destinationFactory != null)
                            destinationValue = _destinationFactory(sourceValue);
                        else
                        {
                            destinationValue = (TDestinationChild)defaultConstructor.Invoke(null);
                        }
                    }

                    mapper.Map(source: sourceValue, destination: destinationValue);

                    output.Add(destinationValue);
                }

                return output;
            }

        }

    }
}