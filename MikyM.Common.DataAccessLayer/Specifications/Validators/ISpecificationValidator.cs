namespace MikyM.Common.DataAccessLayer.Specifications.Validators;

public interface ISpecificationValidator
{
    bool IsValid<T>(T entity, ISpecification<T> specification) where T : class;
}