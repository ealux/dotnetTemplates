using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SolutionTemplate.DAL.Context;
using SolutionTemplate.Interfaces.Base.Entities;
using SolutionTemplate.Interfaces.Base.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SolutionTemplate.DAL.Repositories;

public class DbContextFactoryRepository<T> : IRepository<T> where T : class, IEntity, new()
{
    protected IDbContextFactory<SolutionTemplateDB> ContextFactory { get; }
    protected readonly ILogger<DbContextFactoryRepository<T>> _Logger;

    public DbContextFactoryRepository(IDbContextFactory<SolutionTemplateDB> ContextFactory, ILogger<DbContextFactoryRepository<T>> Logger)
    {
        this.ContextFactory = ContextFactory;
        _Logger = Logger;
    }

    /// <summary>Query to DB</summary>
    /// <param name="db">DB context</param>
    protected virtual IQueryable<T> GetDbQuery(SolutionTemplateDB db) => db.Set<T>();

    public async Task<bool> IsEmpty(CancellationToken cnacel = default)
    {
        await using var db = ContextFactory.CreateDbContext();
        return await db.Set<T>().AnyAsync(cnacel).ConfigureAwait(false);
    }

    #region [Exists]

    public async Task<bool> ExistsId(int Id, CancellationToken cnacel = default)
    {
        await using var db = ContextFactory.CreateDbContext();
        return await GetDbQuery(db).AnyAsync(item => item.Id == Id, cnacel).ConfigureAwait(false);
    }

    public async Task<bool> Exists(T item, CancellationToken cnacel = default)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        await using var db = ContextFactory.CreateDbContext();
        return await GetDbQuery(db).AnyAsync(i => i.Id == item.Id, cnacel).ConfigureAwait(false);
    }

    #endregion [Exists]

    #region [Get]

    public async Task<int> GetCount(CancellationToken cancel = default)
    {
        await using var db = ContextFactory.CreateDbContext();
        return await GetDbQuery(db).CountAsync(cancel).ConfigureAwait(false);
    }

    public async Task<IEnumerable<T>> GetAll(CancellationToken cancel = default)
    {
        await using var db = ContextFactory.CreateDbContext();
        return await GetDbQuery(db).ToArrayAsync(cancel).ConfigureAwait(false);
    }

    public async Task<IEnumerable<T>> Get(int Skip, int Count, CancellationToken cancel = default)
    {
        if (Count <= 0) return Enumerable.Empty<T>();

        await using var db = ContextFactory.CreateDbContext();
        var query = Skip <= 0 ? GetDbQuery(db) : GetDbQuery(db).Skip(Skip);
        return await query.Take(Count).ToArrayAsync(cancel);
    }

    public async Task<IPage<T>> GetPage(int PageNumber, int PageSize, CancellationToken cancel = default)
    {
        if (PageSize <= 0) return new Page<T>(Enumerable.Empty<T>(), PageSize, PageNumber, PageSize);

        await using var db = ContextFactory.CreateDbContext();

        IQueryable<T> query = GetDbQuery(db);
        var total_count = await query.CountAsync(cancel).ConfigureAwait(false);
        if (total_count == 0) return new Page<T>(Enumerable.Empty<T>(), PageSize, PageNumber, PageSize);

        if (PageNumber > 0) query = query.Skip(PageNumber * PageSize);
        query = query.Take(PageSize);
        var items = await query.ToArrayAsync(cancel).ConfigureAwait(false);

        return new Page<T>(items, total_count, PageNumber, PageSize);
    }

    public async Task<T> GetById(int Id, CancellationToken cancel = default)
    {
        await using var db = ContextFactory.CreateDbContext();
        return GetDbQuery(db) switch
        {
            DbSet<T> set => await set.FindAsync(new object[] { Id }, cancel).ConfigureAwait(false),
            { } query => query.FirstOrDefault(item => item.Id == Id),
            _ => throw new InvalidOperationException()
        };
    }

    #endregion [Get]

    #region [Add]

    public async Task<T> Add(T item, CancellationToken cancel = default)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        await using var db = ContextFactory.CreateDbContext();
        _Logger.LogInformation("Add {0} to repository...", item);

        db.Entry(item).State = EntityState.Added;
        await db.SaveChangesAsync(cancel).ConfigureAwait(false);

        _Logger.LogInformation("Add {0} to repository with id: {1} is completed", item, item.Id);

        return item;
    }

    public async Task AddRange(IEnumerable<T> items, CancellationToken cancel = default)
    {
        await using var db = ContextFactory.CreateDbContext();
        await db.AddRangeAsync(items, cancel).ConfigureAwait(false);
        await db.SaveChangesAsync(cancel).ConfigureAwait(false);
    }

    #endregion [Add]

    #region [Update]

    public async Task<T> Update(T item, CancellationToken cancel = default)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        await using var db = ContextFactory.CreateDbContext();

        db.Entry(item).State = EntityState.Modified;
        await db.SaveChangesAsync(cancel).ConfigureAwait(false);

        _Logger.LogInformation("Update with id: {0} - {1} is completed", item.Id, item);
        return item;
    }

    public async Task<T> UpdateAsync(int id, Action<T> ItemUpdated, CancellationToken cancel = default)
    {
        await using var db = ContextFactory.CreateDbContext();

        if (await GetById(id, cancel).ConfigureAwait(false) is not { } item)
            return default;
        ItemUpdated(item);
        await db.SaveChangesAsync(cancel).ConfigureAwait(false);
        return item;
    }

    public async Task UpdateRange(IEnumerable<T> items, CancellationToken cancel = default)
    {
        await using var db = ContextFactory.CreateDbContext();
        db.UpdateRange(items);
        await db.SaveChangesAsync(cancel).ConfigureAwait(false);
    }

    #endregion [Update]

    #region [Delete]

    public async Task<T> Delete(T item, CancellationToken cancel = default)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        await using var db = ContextFactory.CreateDbContext();

        db.Entry(item).State = EntityState.Deleted;
        await db.SaveChangesAsync(cancel).ConfigureAwait(false);

        _Logger.LogInformation("Delete id: {0} - {1} is completed", item.Id, item);
        return item;
    }

    public async Task DeleteRange(IEnumerable<T> items, CancellationToken cancel = default)
    {
        await using var db = ContextFactory.CreateDbContext();
        db.RemoveRange(items);
        await db.SaveChangesAsync(cancel).ConfigureAwait(false);
    }

    public async Task<T> DeleteById(int id, CancellationToken cancel = default)
    {
        await using var db = ContextFactory.CreateDbContext();
        var item = await db.Set<T>().FindAsync(new object[] { id }, cancel).ConfigureAwait(false);
        if (item is not null) return await Delete(item, cancel).ConfigureAwait(false);

        _Logger.LogInformation("On Delete with id: {0} - is not found", id);
        return null;
    }

    #endregion [Delete]
}