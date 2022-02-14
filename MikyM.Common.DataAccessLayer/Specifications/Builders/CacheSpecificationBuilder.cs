namespace MikyM.Common.DataAccessLayer.Specifications.Builders;

public class CacheSpecificationBuilder<T> : ICacheSpecificationBuilder<T> where T : class
{
    public Specification<T> Specification { get; }
    public bool IsChainDiscarded { get; set; }

    public CacheSpecificationBuilder(Specification<T> specification, bool isChainDiscarded = false)
    {
        this.Specification = specification;
        this.IsChainDiscarded = isChainDiscarded;
    }
}