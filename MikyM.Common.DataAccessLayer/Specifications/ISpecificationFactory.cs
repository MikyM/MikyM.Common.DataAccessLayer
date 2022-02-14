namespace MikyM.Common.DataAccessLayer.Specifications;

public interface ISpecificationFactory
{
    TSpecification GetSpecification<TSpecification>() where TSpecification : ISpecification;
}