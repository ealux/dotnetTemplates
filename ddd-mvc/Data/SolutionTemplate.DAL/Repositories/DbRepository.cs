using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SolutionTemplate.DAL.Context;
using SolutionTemplate.Interfaces.Base.Entities;
using SolutionTemplate.Interfaces.Base.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SolutionTemplate.DAL.Repositories;

/// <summary>Entity repository working with DB context</summary>
/// <typeparam name="T">Entity type</typeparam>
public class DbRepository<T> : IRepository<T> where T : class, IEntity, new()
{
    private readonly SolutionTemplateDB _db;
    protected readonly ILogger<DbRepository<T>> _Logger;

    /// <summary>Entities source from DB</summary>
    protected DbSet<T> Set { get; }

    /// <summary>Query entities from DB</summary>
    protected virtual IQueryable<T> Items => Set;

    /// <summary>Save changes automatic</summary>
    public bool AutoSaveChanges { get; set; } = true;

    public DbRepository(SolutionTemplateDB db, ILogger<DbRepository<T>> Logger)
    {
        _db = db;
        Set = db.Set<T>();
        _Logger = Logger;
    }

    public Task<bool> IsEmpty(CancellationToken cancel = default) => Set.AnyAsync(cancel);

    #region [Exists]

    public async Task<bool> ExistsId(int Id, CancellationToken cancel = default) =>
        await Set.AnyAsync(item => item.Id == Id, cancel).ConfigureAwait(false);

    public async Task<bool> Exists(T item, CancellationToken cancel = default)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        return await Set.AnyAsync(i => i.Id == item.Id, cancel).ConfigureAwait(false);
    }

    #endregion [Exists]

    #region [Get]

    public async Task<int> GetCount(CancellationToken cancel = default) =>
        await Items.CountAsync(cancel).ConfigureAwait(false);

    public async Task<IEnumerable<T>> GetAll(CancellationToken cancel = default) =>
        await Items.ToArrayAsync(cancel).ConfigureAwait(false);

    public async Task<IEnumerable<T>> Get(int Skip, int Count, CancellationToken cancel = default)
    {
        if (Count <= 0) return Enumerable.Empty<T>();

        var query = Items;
        if (Skip > 0) query = query.Skip(Skip);

        return await query.Take(Count).ToArrayAsync(cancel);
    }

    public async Task<IPage<T>> GetPage(int PageNumber, int PageSize, CancellationToken cancel = default)
    {
        if (PageSize <= 0) return new Page<T>(Enumerable.Empty<T>(), PageSize, PageNumber, PageSize);

        var query = Items;
        var total_count = await query.CountAsync(cancel).ConfigureAwait(false);
        if (total_count == 0) return new Page<T>(Enumerable.Empty<T>(), PageSize, PageNumber, PageSize);

        if (PageNumber > 0) query = query.Skip(PageNumber * PageSize);
        query = query.Take(PageSize);
        var items = await query.ToArrayAsync(cancel).ConfigureAwait(false);

        return new Page<T>(items, total_count, PageNumber, PageSize);
    }

    public async Task<T> GetById(int Id, CancellationToken cancel = default) => Items switch
    {
        DbSet<T> set => await set.FindAsync(new object[] { Id }, cancel).ConfigureAwait(false),
        { } items => await items.FirstOrDefaultAsync(item => item.Id == Id, cancel).ConfigureAwait(false),
    };

    #endregion [Get]

    #region [Add]

    public async Task<T> Add(T item, CancellationToken cancel = default)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        _Logger.LogInformation("Add {0} to repository...", item);

        _db.Entry(item).State = EntityState.Added;
        if (AutoSaveChanges) await SaveChangesAsync(cancel).ConfigureAwait(false);

        _Logger.LogInformation("Add {0} to repository with id: {1} completed", item, item.Id);

        return item;
    }

    public async Task AddRange(IEnumerable<T> items, CancellationToken cancel = default)
    {
        if (items is null) throw new ArgumentNullException(nameof(items));
        _Logger.LogInformation("Add enumerable entities...");
        await _db.AddRangeAsync(items, cancel).ConfigureAwait(false);
        if (AutoSaveChanges) await SaveChangesAsync(cancel).ConfigureAwait(false);
        _Logger.LogInformation("Add enumerable entities is completed");
    }

    #endregion [Add]

    #region [Update]

    public async Task<T> Update(T item, CancellationToken cancel = default)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        _Logger.LogInformation("Update id: {0} - {1}...", item.Id, item);

        _db.Entry(item).State = EntityState.Modified;
        if (AutoSaveChanges) await SaveChangesAsync(cancel).ConfigureAwait(false);

        _Logger.LogInformation("Update id: {0} - {1} completed", item.Id, item);
        return item;
    }

    public async Task<T> UpdateAsync(int id, Action<T> ItemUpdated, CancellationToken cancel = default)
    {
        if (await GetById(id, cancel).ConfigureAwait(false) is not { } item)
            return default;
        ItemUpdated(item);
        if (AutoSaveChanges) await SaveChangesAsync(cancel).ConfigureAwait(false);
        return item;
    }

    public async Task UpdateRange(IEnumerable<T> items, CancellationToken cancel = default)
    {
        _db.UpdateRange(items);
        if (AutoSaveChanges) await SaveChangesAsync(cancel).ConfigureAwait(false);
    }

    #endregion [Update]

    #region [Delete]

    public async Task<T> Delete(T item, CancellationToken cancel = default)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        _Logger.LogInformation("Delete id: {0} - {1}...", item.Id, item);

        _db.Entry(item).State = EntityState.Deleted;
        if (AutoSaveChanges) await SaveChangesAsync(cancel).ConfigureAwait(false);

        _Logger.LogInformation("Delete id: {0} - {1} completed", item.Id, item);
        return item;
    }

    public async Task DeleteRange(IEnumerable<T> items, CancellationToken cancel = default)
    {
        _db.RemoveRange(items);
        if (AutoSaveChanges) await SaveChangesAsync(cancel).ConfigureAwait(false);
    }

    public async Task<T> DeleteById(int id, CancellationToken cancel = default)
    {
        var item = await Set.FindAsync(new object[] { id }, cancel).ConfigureAwait(false);
        if (item is not null) return await Delete(item, cancel).ConfigureAwait(false);

        _Logger.LogInformation("On delete id: {0} - entity is not found", id);
        return null;
    }

    #endregion [Delete]

    public virtual async Task<int> SaveChangesAsync(CancellationToken cancel = default)
    {
        _Logger.LogInformation("Saving DB");
        var timer = Stopwatch.StartNew();

        var changes_count = await _db.SaveChangesAsync(cancel).ConfigureAwait(false);

        timer.Stop();
        _Logger.LogInformation($"DB saving is completed with {timer.Elapsed.TotalSeconds} s. Changes {changes_count}");
        return changes_count;
    }
}