namespace SolutionTemplate.Interfaces.Base.Entities;

/// <summary>Generic typed Id Descripted Entity</summary>
/// <typeparam name="TKey">Primary key type</typeparam>
public interface IDescriptedEntity<out TKey> : IEntity<TKey>
{
    /// <summary>Description</summary>
    string Description { get; }
}

/// <summary>Descripted Entity</summary>
public interface IDescriptedEntity : IDescriptedEntity<int>, IEntity { }