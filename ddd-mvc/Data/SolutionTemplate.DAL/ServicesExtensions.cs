using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SolutionTemplate.DAL.Context;
using System;

namespace SolutionTemplate.DAL;

public static class ServicesExtensions
{
    /// <summary>Take DB context</summary>
    /// <param name="services">Service provider</param>
    /// <returns>DB context</returns>
    public static SolutionTemplateDB GetSolutionTemplateDB(this IServiceProvider services) =>
        services.GetRequiredService<SolutionTemplateDB>();

    /// <summary>Take DB context factory</summary>
    /// <param name="services">Service provider</param>
    /// <returns>DB context factory</returns>
    public static IDbContextFactory<SolutionTemplateDB> GetSolutionTemplateDBFactory(this IServiceProvider services) =>
        services.GetRequiredService<IDbContextFactory<SolutionTemplateDB>>();

    #region [Scope]

    /// <summary>Take DB context</summary>
    /// <param name="scope">Service provider scope</param>
    /// <returns>DB context</returns>
    public static SolutionTemplateDB GetSolutionTemplateDB(this IServiceScope scope) =>
        scope.ServiceProvider.GetSolutionTemplateDB();

    /// <summary>Take DB context factory</summary>
    /// <param name="scope">Service provider scope</param>
    /// <returns>DB context factory</returns>
    public static IDbContextFactory<SolutionTemplateDB> GetSolutionTemplateDBFactory(this IServiceScope scope) =>
        scope.ServiceProvider.GetSolutionTemplateDBFactory();

    #endregion [Scope]
}