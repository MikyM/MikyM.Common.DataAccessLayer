using Autofac;
using MikyM.Common.DataAccessLayer.UnitOfWork;

namespace MikyM.Common.DataAccessLayer.Extensions
{
    /// <summary>
    /// Extension methods for setting up unit of work related services in a <see cref="ContainerBuilder"/>.
    /// </summary>
    public static class AutofacExtensions
    {
        /// <summary>
        /// Registers the unit of work given context as a service in the <see cref="ContainerBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="ContainerBuilder"/> to add services to.</param>
        /// <returns>The same service collection so that multiple calls can be chained.</returns>
        /// <remarks>
        /// This method only support one db context, if been called more than once, will throw exception.
        /// </remarks>
        public static ContainerBuilder AddUnitOfWork<TContext>(this ContainerBuilder builder)
        {
            builder.RegisterType<TContext>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(UnitOfWork<>)).As(typeof(IUnitOfWork<>)).InstancePerLifetimeScope();
            return builder;
        }
        /// <summary>
        /// Registers the unit of work given context as a service in the <see cref="ContainerBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="ContainerBuilder"/> to add services to.</param>
        /// <returns>The same service collection so that multiple calls can be chained.</returns>
        /// <remarks>
        /// </remarks>
        public static ContainerBuilder AddUnitOfWork(this ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(UnitOfWork<>)).As(typeof(IUnitOfWork<>)).InstancePerLifetimeScope();
            return builder;
        }
    }
}
