using WonderKidEngine.EntityFrameworkCore.SapHana.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.Extensions.DependencyInjection;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Design.Internal
{
    public class SapHanaDesignTimeServices : IDesignTimeServices
    {
        public virtual void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
        {
            System.Diagnostics.Debugger.Launch();
            serviceCollection.AddEntityFrameworkSapHana();
            new EntityFrameworkRelationalDesignServicesBuilder(serviceCollection)
                .TryAdd<IAnnotationCodeGenerator, SapHanaAnnotationCodeGenerator>()
                .TryAdd<IDatabaseModelFactory, SapHanaDatabaseModelFactory>()
                .TryAdd<IProviderConfigurationCodeGenerator, SapHanaCodeGenerator>()
                .TryAddCoreServices();
        }
    }
}

