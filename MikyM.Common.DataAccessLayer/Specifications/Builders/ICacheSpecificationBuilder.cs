namespace MikyM.Common.DataAccessLayer.Specifications.Builders;

public interface ICacheSpecificationBuilder<T> : ISpecificationBuilder<T> where T : class
{
    bool IsChainDiscarded { get; set; }
}