//@BaseCode
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Exceptions;

namespace SETemplate.Logic.DataContext
{
    partial class SetBase<TElement>
    {
        #region properties
        protected virtual int MaxCount { get; } = 500;
        protected virtual ParsingConfig ParsingConfig
        {
            get
            {
                return new ParsingConfig
                {
                    ResolveTypesBySimpleName = true,
                    AllowNewToEvaluateAnyType = false,
                    EvaluateGroupByAtDatabase = true,
                };
            }
        }
        #endregion properties

        #region overridables
        /// <summary>
        /// Copies properties from the source element to the target element.
        /// </summary>
        /// <param name="target">The target element.</param>
        /// <param name="source">The source element.</param>
        protected abstract void CopyProperties(TElement target, TElement source);
        #endregion overridables

        #region methods
        /// <summary>
        /// Returns the count of elements in the set asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the count of elements.</returns>
        internal virtual Task<int> ExecuteCountAsync()
        {
            return DbSet.CountAsync();
        }

        /// <summary>
        /// Gets the queryable set of elements.
        /// </summary>
        /// <returns>An <see cref="IQueryable{TElement}"/> that can be used to query the set of elements.</returns>
        internal virtual IQueryable<TElement> ExecuteAsQuerySet() => DbSet.AsQueryable();

        /// <summary>
        /// Gets the no-tracking queryable set of elements.
        /// </summary>
        /// <returns>An <see cref="IQueryable{TElement}"/> that can be used to query the set of elements without tracking changes.</returns>
        internal virtual IQueryable<TElement> ExecuteAsNoTrackingSet() => ExecuteAsQuerySet().AsNoTracking();

        /// <summary>
        /// Retrieves all elements from the set without tracking changes.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a collection of elements limited to <see cref="MaxCount"/>.
        /// </returns>
        internal virtual Task<IEnumerable<TElement>> ExecuteGetAllAsync()
        {
            return ExecuteAsNoTrackingSet().Take(MaxCount).ToArrayAsync().ContinueWith(t => (IEnumerable<TElement>)t.Result);
        }

        /// <summary>
        /// Queries elements from the set based on the provided query parameters.
        /// </summary>
        /// <param name="queryParams">The query parameters containing filter, values, and includes.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of elements matching the query criteria.</returns>
        /// <exception cref="Modules.Exceptions.LogicException">Thrown when the filter expression is empty or invalid.</exception>
        internal virtual async Task<IEnumerable<TElement>> ExecuteQueryAsync(Models.QueryParams queryParams)
        {
            try
            {
                var set = ExecuteAsNoTrackingSet();
                var query = default(TElement[]);

                foreach (var include in queryParams.Includes ?? [])
                {
                    if (!string.IsNullOrWhiteSpace(include))
                    {
                        BeforeIncludeExecuting(include);
                        set = set.Include(include);
                    }
                }

                if (string.IsNullOrWhiteSpace(queryParams.SortBy) == false)
                {
                    set = set.OrderBy(queryParams.SortBy);
                }

                if (string.IsNullOrWhiteSpace(queryParams.Filter) == false
                    && queryParams.Values != null
                    && queryParams.Values.Length > 0)
                {
                    query = await set.Where(ParsingConfig, queryParams.Filter, queryParams.Values)
                                     .Take(MaxCount)
                                     .ToArrayAsync()
                                     .ConfigureAwait(false);
                }
                else
                {
                    query = await set.Take(MaxCount)
                                     .ToArrayAsync()
                                     .ConfigureAwait(false);
                }
                return query;
            }
            catch (ParseException ex)
            {
                throw new Modules.Exceptions.LogicException($"Invalid filter expression: {ex.Message}");
            }
        }

        /// <summary>
        /// Called before including a navigation property in a query. Override to add access checks.
        /// </summary>
        /// <param name="include">The include path being applied.</param>
        protected virtual void BeforeIncludeExecuting(string include)
        {
        }
        #endregion methods
    }
}
