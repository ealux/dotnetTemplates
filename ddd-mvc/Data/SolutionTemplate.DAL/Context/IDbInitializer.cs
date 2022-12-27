using System.Threading;
using System.Threading.Tasks;

namespace SolutionTemplate.DAL.Context;

/// <summary>Initialize DataBase</summary>
public interface IDbInitializer
{
    /// <summary>Delete DB before creation?</summary>
    bool Recreate { get; set; }

    /// <summary>Delete DB</summary>
    /// <returns><see langword="true"/> if DB has been deleted</returns>
    bool Delete();

    /// <summary>Delete DB asynchronously</summary>
    /// <param name="cancel">Cancel operation</param>
    Task<bool> DeleteAsync(CancellationToken cancel = default);

    /// <summary>Initialize DB</summary>
    void Initialize();

    /// <summary>Initialize DB asynchronously</summary>
    /// <param name="cancel">Cancel operation</param>
    Task InitializeAsync(CancellationToken cancel = default);
}