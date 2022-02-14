namespace MikyM.Common.DataAccessLayer.Specifications.Builders;

public interface IOrderedSpecificationBuilder<T> : ISpecificationBuilder<T> where T : class
{
    bool IsChainDiscarded { get; set; }
}