using WonderKidEngine.EntityFrameworkCore.SapHana.Infrastructure.Internal;
using WonderKidEngine.EntityFrameworkCore.SapHana.Internal;
using WonderKidEngine.EntityFrameworkCore.SapHana.Migrations.Internal;
using WonderKidEngine.EntityFrameworkCore.SapHana.Storage.Internal;
using WonderKidEngine.EntityFrameworkCore.SapHana.Update.Internal;
using WonderKidEngine.EntityFrameworkCore.SapHana.ValueGeneration.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using WonderKidEngine.EntityFrameworkCore.SapHana.Diagnostics.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using WonderKidEngine.EntityFrameworkCore.SapHana.Metadata.Internal;
using WonderKidEngine.EntityFrameworkCore.SapHana.Migrations;
using WonderKidEngine.EntityFrameworkCore.SapHana.Query.ExpressionVisitors.Internal;
using WonderKidEngine.EntityFrameworkCore.SapHana.Query.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class SapHanaServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityFrameworkSapHana([NotNull] this IServiceCollection serviceCollection)
        {
            Check.NotNull(serviceCollection, nameof(serviceCollection));

            var builder = new EntityFrameworkRelationalServicesBuilder(serviceCollection)
                .TryAdd<LoggingDefinitions, SapHanaLoggingDefinitions>()
                .TryAdd<IDatabaseProvider, DatabaseProvider<SapHanaOptionsExtension>>()
                //.TryAdd<IValueGeneratorCache>(p => p.GetService<ISapHanaValueGeneratorCache>())
                .TryAdd<IRelationalTypeMappingSource, SapHanaTypeMappingSource>()
                .TryAdd<ISqlGenerationHelper, SapHanaSqlGenerationHelper>()
                .TryAdd<IRelationalAnnotationProvider, SapHanaAnnotationProvider>()
                .TryAdd<IModelValidator, SapHanaModelValidator>()
                .TryAdd<IProviderConventionSetBuilder, SapHanaConventionSetBuilder>()
                //.TryAdd<IRelationalValueBufferFactoryFactory, TypedRelationalValueBufferFactoryFactory>() // What is that?
                .TryAdd<IUpdateSqlGenerator, SapHanaUpdateSqlGenerator>()
                .TryAdd<IModificationCommandBatchFactory, SapHanaModificationCommandBatchFactory>()
                .TryAdd<IValueGeneratorSelector, SapHanaValueGeneratorSelector>()
                .TryAdd<IRelationalConnection>(p => p.GetService<ISapHanaRelationalConnection>())
                .TryAdd<IMigrationsSqlGenerator, SapHanaMigrationsSqlGenerator>()
                .TryAdd<IRelationalDatabaseCreator, SapHanaDatabaseCreator>()
                .TryAdd<IHistoryRepository, SapHanaHistoryRepository>()
                .TryAdd<ICompiledQueryCacheKeyGenerator, SapHanaCompiledQueryCacheKeyGenerator>()
                .TryAdd<IExecutionStrategyFactory, SapHanaExecutionStrategyFactory>()
                .TryAdd<IRelationalQueryStringFactory, SapHanaQueryStringFactory>()
                .TryAdd<IMethodCallTranslatorProvider, SapHanaMethodCallTranslatorProvider>()
                .TryAdd<IMemberTranslatorProvider, SapHanaMemberTranslatorProvider>()
                .TryAdd<IEvaluatableExpressionFilter, SapHanaEvaluatableExpressionFilter>()
                .TryAdd<IQuerySqlGeneratorFactory, SapHanaQuerySqlGeneratorFactory>()
                .TryAdd<IRelationalSqlTranslatingExpressionVisitorFactory, SapHanaSqlTranslatingExpressionVisitorFactory>()
                .TryAdd<IRelationalParameterBasedSqlProcessorFactory, SapHanaParameterBasedSqlProcessorFactory>()
                .TryAdd<ISqlExpressionFactory, SapHanaSqlExpressionFactory>()
                .TryAdd<ISingletonOptions, ISapHanaOptions>(p => p.GetService<ISapHanaOptions>())
                //.TryAdd<IValueConverterSelector, SapHanaValueConverterSelector>()
                .TryAdd<IQueryCompilationContextFactory, SapHanaQueryCompilationContextFactory>()
                .TryAdd<IQueryTranslationPostprocessorFactory, SapHanaQueryTranslationPostprocessorFactory>()
                .TryAdd<IMigrationsModelDiffer, SapHanaMigrationsModelDiffer>()
                .TryAdd<IMigrator, SapHanaMigrator>()
                .TryAddProviderSpecificServices(m => m
                    //.TryAddSingleton<ISapHanaValueGeneratorCache, SapHanaValueGeneratorCache>()
                    .TryAddSingleton<ISapHanaOptions, SapHanaOptions>()
                    //.TryAddScoped<ISapHanaSequenceValueGeneratorFactory, SapHanaSequenceValueGeneratorFactory>()
                    .TryAddScoped<ISapHanaUpdateSqlGenerator, SapHanaUpdateSqlGenerator>()
                    .TryAddScoped<ISapHanaRelationalConnection, SapHanaRelationalConnection>());

            builder.TryAddCoreServices();

            return serviceCollection;
        }
    }
}
