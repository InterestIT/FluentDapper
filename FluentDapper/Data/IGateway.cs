using System.Collections.Generic;
using System.Threading.Tasks;

namespace FluentDapper.Data
{
    /// <summary>
    /// Gateway interface to provide CRUD operations.
    /// </summary>
    /// <typeparam name="TDataModel">The data model type</typeparam>
    /// <typeparam name="TDataFilter">The data filter model type</typeparam>
    public interface IGateway<TDataModel, in TDataFilter>
    {
        /// <summary>
        /// Gets an <see cref="IEnumerable{TDataModel}">enumerable of <typeparamref name="TDataModel">data models</typeparamref></see> using the provided <typeparamref name="TDataFilter">data filter</typeparamref>.
        /// </summary>
        /// <param name="filter">The provided <typeparamref name="TDataFilter">data filter</typeparamref>.</param>
        /// <returns>An <see cref="IEnumerable{TDataModel}">enumerable of <typeparamref name="TDataModel">data models</typeparamref></see>, or an empty <see cref="IEnumerable{TDataModel}">enumerable</see> if no results.</returns>
        Task<IEnumerable<TDataModel>> Get(TDataFilter filter);

        /// <summary>
        /// Gets a single <typeparamref name="TDataModel">data model</typeparamref> by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A <typeparamref name="TDataModel">data model</typeparamref> if any; otherwise, default(<typeparamref name="TDataModel"/>).</returns>
        Task<TDataModel> GetSingle(int id);

        /// <summary>
        /// Adds a <typeparamref name="TDataModel">data model</typeparamref>.
        /// </summary>
        /// <param name="model">The <typeparamref name="TDataModel">data model</typeparamref> to add.</param>
        /// <returns>The newly added object's ID.</returns>
        Task<int> Add(TDataModel model);

        /// <summary>
        /// Updates a <typeparamref name="TDataModel">data model</typeparamref>.
        /// </summary>
        /// <param name="model">The <typeparamref name="TDataModel">data model</typeparamref> to update.</param>
        /// <returns>If successful, <c>true</c>; otherwise, <c>false</c>.</returns>
        Task<bool> Update(TDataModel model);

        /// <summary>
        /// Deletes a <typeparamref name="TDataModel">data model</typeparamref>.
        /// </summary>
        /// <param name="model">The <typeparamref name="TDataModel">data model</typeparamref> to delete.</param>
        /// <returns>If successful, <c>true</c>; otherwise, <c>false</c>.</returns>
        Task<bool> Delete(TDataModel model);
    }
}