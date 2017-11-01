using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DapperExtensions;
using FluentDapper.Filtering;

namespace FluentDapper.Data
{
    /// <inheritdoc />
    public class Gateway<TData, TDataFilter> : IGateway<TData, TDataFilter> where TData: class where TDataFilter: DataFilter<TDataFilter, TData>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IPredicateFactory _predicateFactory;

        /// <summary>
        /// Creates a new <see cref="Gateway{TData, TDataFilter}"/> instance.
        /// </summary>
        /// <param name="connectionFactory">The <see cref="IDbConnectionFactory">connection factory</see> to use.</param>
        /// <param name="predicateFactory">The <see cref="IPredicateFactory">predicate factory</see> to use.</param>
        public Gateway(IDbConnectionFactory connectionFactory, IPredicateFactory predicateFactory)
        {
            _connectionFactory = connectionFactory;
            _predicateFactory = predicateFactory;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TData>> Get(TDataFilter filter)
        {
            var filterPredicate = _predicateFactory.GetPredicate<TDataFilter, TData>(filter);
            var dataItems = await _connectionFactory.GetConnection().GetListAsync<TData>(filterPredicate);
            return dataItems?.ToList();
        }

        /// <inheritdoc />
        public async Task<TData> GetSingle(int id)
        {
            return await _connectionFactory.GetConnection().GetAsync<TData>(id);
        }

        /// <inheritdoc />
        public Task<int> Add(TData model)
        {
            int id = _connectionFactory.GetConnection().Insert(model);
            return Task.FromResult(id);
        }

        /// <inheritdoc />
        public Task<bool> Update(TData model)
        {
            var success = _connectionFactory.GetConnection().Update(model);
            return Task.FromResult(success);
        }

        /// <inheritdoc />
        public Task<bool> Delete(TData speaker)
        {
            var success = _connectionFactory.GetConnection().Delete(speaker);
            return Task.FromResult(success);
        }
    }
}