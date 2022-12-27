using System;
using System.Collections.Generic;

namespace SolutionTemplate.Interfaces.Base.Repositories;

/// <summary>Page of elements</summary>
/// <typeparam name="T">Element type</typeparam>
public interface IPage<out T>
{
    /// <summary>Page elements</summary>
    IEnumerable<T> Items { get; }

    /// <summary>Total elements count on all pages</summary>
    int TotalCount { get; }

    /// <summary>Current page index</summary>
    int PageNumber { get; }

    /// <summary>Amount of elements in one page</summary>
    int PageSize { get; }

    /// <summary>Amount of pages in list</summary>
    int TotalPagesCount => (int) Math.Ceiling((double) TotalCount / PageSize);

    /// <summary>Does previous page exists</summary>
    bool HasPrevPage => PageNumber >= 0;

    /// <summary>Does next page exists</summary>
    bool HasNextPage => PageNumber < TotalPagesCount;
}