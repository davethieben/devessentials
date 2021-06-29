using AutoMapper;
using Essentials.Data;

namespace Essentials.Helpers
{
    public class RepositoryResolver<TModel> : IMemberValueResolver<object, object, int, TModel>
    {
        private readonly IResolverRepository<TModel> _repository;

        public RepositoryResolver(IResolverRepository<TModel> repository)
        {
            _repository = repository;
        }

        public TModel Resolve(object source, object destination, int sourceId, TModel destMember, ResolutionContext context)
        {
            return _repository.GetById(sourceId);
        }
    }
}
