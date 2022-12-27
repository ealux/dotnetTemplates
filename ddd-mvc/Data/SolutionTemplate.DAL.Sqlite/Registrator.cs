using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SolutionTemplate.DAL.Context;
using SolutionTemplate.DAL.Repositories;

namespace SolutionTemplate.DAL.Sqlite;

/// <summary>Registrator for Sqlite DB</summary>
public static class Registrator
{
    /// <summary>Add data context to service container for Sqlite connection</summary>
    /// <param name="services">Service collection</param>
    /// <param name="ConnectionString">Server connection string</param>
    public static IServiceCollection AddSolutionTemplateDbContextSqlite(this IServiceCollection services, string ConnectionString) =>
        services
           .AddDbContext<SolutionTemplateDB>(
                opt => opt.UseSqlite(
                    ConnectionString,
                    o => o.MigrationsAssembly(typeof(Registrator).Assembly.FullName)))
           .AddScoped<IDbInitializer, SolutionTemplateDBInitializer>()
           .AddSolutionTemplateRepositories();

    /// <summary>Add data context factory to service container for Sqlite connection</summary>
    /// <param name="services">Service collection</param>
    /// <param name="ConnectionString">Server connection string</param>
    public static IServiceCollection AddSolutionTemplateDbContextFactorySqlite(this IServiceCollection services, string ConnectionString) =>
        services
           .AddDbContextFactory<SolutionTemplateDB>(
                opt => opt.UseSqlite(
                    ConnectionString, 
                    o => o.MigrationsAssembly(typeof(Registrator).Assembly.FullName)))
           .AddScoped<IDbInitializer, SolutionTemplateDBInitializer>()
           .AddSolutionTemplateRepositoryFactories();
}