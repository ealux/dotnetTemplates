using System.Threading;
using System.Threading.Tasks;
using SolutionTemplate.Interfaces.Base.Entities;

namespace SolutionTemplate.Interfaces.Base.Repositories;

/// <summary>Named entities repository</summary>
/// <typeparam name="T">Entity type</typeparam>
/// <typeparam name="TKey">Primary key type</typeparam>
public interface INamedRepository<T, in TKey> : IRepository<T, TKey> where T : INamedEntity<TKey>
{
    /// <summary>Check if Named Entity has allready existed</summary>
    /// <param name="Name">Entity name</param>
    /// <param name="cancel">Cancel operation</param>
    Task<bool> ExistName(string Name, CancellationToken cancel = default);

    /// <summary>Get Entity by Name</summary>
    /// <param name="Name">Entity name to be taken</param>
    /// <param name="cancel">Cancel operation</param>
    Task<T> GetByName(string Name, CancellationToken cancel = default);

    /// <summary>Delete Named Entity from repository</summary>
    /// <param name="Name">Entity Name</param>
    /// <param name="cancel">Cancel operation</param>
    Task<T> DeleteByName(string Name, CancellationToken cancel = default);
}

/// <summary>Named entities repository</summary>
/// <typeparam name="T">Entity type</typeparam>
public interface INamedRepository<T> : INamedRepository<T, int> where T : INamedEntity<int> { }