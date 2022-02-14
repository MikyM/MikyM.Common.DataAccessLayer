
namespace MikyM.Common.DataAccessLayer.Specifications.Validators;

public interface IValidator
{
    bool IsValid<T>(T entity, ISpecification<T> specification) where T : class;
}