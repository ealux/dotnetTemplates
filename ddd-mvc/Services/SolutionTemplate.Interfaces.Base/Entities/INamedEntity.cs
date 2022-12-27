using System.ComponentModel.DataAnnotations;

namespace SolutionTemplate.Interfaces.Base.Entities
{
    /// <summary> Generic typed Named Entity </summary>
    /// <typeparam name="TKey">Primary key type</typeparam>
    public interface INamedEntity<out TKey> : IEntity<TKey>
    {
        /// <summary> Entity Name </summary>
        [Required]
        string Name { get; }
    }

    /// <summary> Named Entity </summary>
    public interface INamedEntity : IEntity, INamedEntity<int>
    {
    }
}