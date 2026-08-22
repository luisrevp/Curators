// Technical / domain - building infrastructure.
namespace Curators.Domain.SeedWork;

/*
    HOW TO USE:
    To enforce DDD principles, we will use this interface as a "Marker"
    this marker will make the registration of our aggregate entities to our infrastructure layer more feasible
    by enforcing the generic repositories to ONLY accept valid aggregate root entities

    so we normally just inherit it from our root entities
    then, we use it as a generic constraint for our repositories
    example: public class JobRepository<T> where T : IAggregateRoot
 */
public interface IAggregateRoot
{
}

