using System.Xml.Linq;

// Technical/domain-building infrastructure. Stuff required to construct our domain model
namespace Curators.Domain.SeedWork;

// non-instantiable (abstract)
// Only ensures the correct Id assignation for our entities
// Helps managing multiple Id Types (int and Guid, mostly)
// Also helps to track IDs on this layer

public abstract class Entity<TId> where TId : struct, IEquatable<TId>
{
    public TId Id { get; }

    //protected Etity. Only called by inhereted classes using "base(id)"
    protected Entity() { }
    protected Entity(TId id)
    {
        this.Id = id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other) return false;
        if (ReferenceEquals(this, other)) return true;
        if (GetType() != other.GetType()) return false;
        if (Id.Equals(default(TId)) || other.Id.Equals(default(TId))) return false;

        return Id.Equals(other.Id);
    }

    public override int GetHashCode() => HashCode.Combine(this.GetType(), this.Id);
}
