namespace MikyM.Common.DataAccessLayer.Specifications.Builders;

public class IncludableSpecificationBuilder<T, TProperty> : IIncludableSpecificationBuilder<T, TProperty> where T : class
{
    public Specification<T> Specification { get; }
    public bool IsChainDiscarded { get; set; }

    public IncludableSpecificationBuilder(Specification<T> specification, bool isChainDiscarded = false)
    {
        this.Specification = specification;
        this.IsChainDiscarded = isChainDiscarded;
    }
}