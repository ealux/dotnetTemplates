using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using SolutionTemplate.Interfaces.Base.Entities;
using SolutionTemplate.Interfaces.Base.Repositories;

namespace SolutionTemplate.Interfaces.Base.Extensions;

public static class RepositoryExtensions
{
    /// <summary>Enumerate all entities in repository</summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <typeparam name="TKey">Entity primary key type</typeparam>
    /// <param name="repository">Repository onwhere enumeration executed</param>
    /// <param name="PageSize">Page capacity</param>
    /// <param name="cancel">Cancel operation</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="PageSize"/> less or equals zero</exception>
    public static async IAsyncEnumerable<IPage<T>> EnumPages<T, TKey>(
        this IRepository<T, TKey> repository,
        int PageSize, 
        [EnumeratorCancellation] CancellationToken cancel = default) 
        where T : IEntity<TKey>
    {
        if (repository is null) throw new ArgumentNullException(nameof(repository));
        if (PageSize <= 0) throw new ArgumentOutOfRangeException(nameof(PageSize), PageSize, "PageSize argument should be greater then zero");

        IPage<T> page;
        var index = 0;
        do
        {
            page = await repository.GetPage(index++, PageSize, cancel).ConfigureAwait(false);
            yield return page;
        }
        while (page.HasNextPage);
    }
}