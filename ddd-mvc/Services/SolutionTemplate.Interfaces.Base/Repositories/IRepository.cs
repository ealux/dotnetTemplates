using SolutionTemplate.Interfaces.Base.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SolutionTemplate.Interfaces.Base.Repositories;

/// <summary>Entity repository</summary>
/// <typeparam name="T">Repository entity type</typeparam>
/// <typeparam name="TKey">Primary key type</typeparam>
public interface IRepository<T, in TKey> where T : IEntity<TKey>
{
    /// <summary>Check if repository is empty</summary>
    /// <param name="cancel">Cancel opeartion token</param>
    Task<bool> IsEmpty(CancellationToken cancel = default);

    #region [Exists]

    /// <summary>Check if entity of specified Id exsists</summary>
    /// <param name="Id">Entity Id</param>
    /// <param name="cancel">Cancel opeartion token</param>
    Task<bool> ExistsId(TKey Id, CancellationToken cancel = default);

    /// <summary>Check if scepified entity exsists</summary>
    /// <param name="item">Entity</param>
    /// <param name="cancel">Cancel opeartion token</param>
    Task<bool> Exists(T item, CancellationToken cancel = default);

    #endregion [Exists]

    #region [Get]

    /// <summary>Get entities count</summary>
    /// <param name="cancel">Cancel opeartion token</param>
    Task<int> GetCount(CancellationToken cancel = default);

    /// <summary>Get all entities</summary>
    /// <param name="cancel">Cancel opeartion token</param>
    Task<IEnumerable<T>> GetAll(CancellationToken cancel = default);

    /// <summary>Get a number of entities skiping someone</summary>
    /// <param name="Skip">Entities count to skip</param>
    /// <param name="Count">Entities count to take</param>
    /// <param name="cancel">Cancel opeartion token</param>
    Task<IEnumerable<T>> Get(int Skip, int Count, CancellationToken cancel = default);

    /// <summary>Get <see cref="IPage"/> with entities</summary>
    /// <param name="PageNumber">Page number (from 0)</param>
    /// <param name="PageSize">Page capacity</param>
    /// <param name="cancel">Cancel opeartion token</param>
    Task<IPage<T>> GetPage(int PageNumber, int PageSize, CancellationToken cancel = default);

    /// <summary>Get entity by Id</summary>
    /// <param name="Id">Entity Id</param>
    /// <param name="cancel">Cancel opeartion token</param>
    Task<T> GetById(int Id, CancellationToken cancel = default);

    #endregion [Get]

    #region [Add]

    /// <summary>Add entity to repository</summary>
    /// <param name="item">Entity to add</param>
    /// <param name="cancel">Cancel opeartion token</param>
    /// <returns>Return Entity added to repository or <see langword="null"/></returns>
    Task<T> Add(T item, CancellationToken cancel = default);

    /// <summary>Add enumerable entities to repository</summary>
    /// <param name="cancel">Cancel opeartion token</param>
    Task AddRange(IEnumerable<T> items, CancellationToken cancel = default);

    /// <summary>Add entity to repository with fabric method</summary>
    /// <param name="ItemFactory">Method to add entity</param>
    /// <param name="cancel">Cancel opeartion token</param>
    /// <returns>Return Entity added to repository or <see langword="null"/></returns>
    Task<T> AddAsync(Func<T> ItemFactory, CancellationToken cancel = default) => Add(ItemFactory(), cancel);

    #endregion [Add]

    #region [Update]

    /// <summary>Update entity in repository</summary>
    /// <param name="item">Entity to be updated</param>
    /// <param name="cancel">Cancel opeartion token</param>
    Task<T> Update(T item, CancellationToken cancel = default);

    /// <summary>Update entity in repository by Id</summary>
    /// <param name="id">Entity Id</param>
    /// <param name="ItemUpdated">Update method</param>
    /// <param name="cancel">Cancel opeartion token</param>
    Task<T> UpdateAsync(TKey id, Action<T> ItemUpdated, CancellationToken cancel = default);

    /// <summary>Update enumeration of entities</summary>
    /// <param name="cancel">Cancel opeartion token</param>
    Task UpdateRange(IEnumerable<T> items, CancellationToken cancel = default);

    #endregion [Update]

    #region [Delete]

    /// <summary>Delete entity from repository</summary>
    /// <param name="item">Entity to be deleted</param>
    /// <param name="cancel">Cancel opeartion token</param>
    Task<T> Delete(T item, CancellationToken cancel = default);

    /// <summary>Delete entity from repository</summary>
    /// <param name="cancel">Cancel opeartion token</param>
    Task DeleteRange(IEnumerable<T> items, CancellationToken cancel = default);

    /// <summary>Delete entity from repository by Id</summary>
    /// <param name="id">Entity Id</param>
    /// <param name="cancel">Cancel opeartion token</param>
    Task<T> DeleteById(TKey id, CancellationToken cancel = default);

    #endregion [Delete]
}

/// <summary>Entity repository</summary>
public interface IRepository<T> : IRepository<T, int> where T : IEntity<int>
{ }