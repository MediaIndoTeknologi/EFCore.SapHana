using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Scaffolding.Internal
{
    internal class SapHanaCodeGenerationMemberAccessTypeMapping : RelationalTypeMapping
    {
        private const string DummyStoreType = "clrOnly";

        public SapHanaCodeGenerationMemberAccessTypeMapping()
            : base(new RelationalTypeMappingParameters(new CoreTypeMappingParameters(typeof(SapHanaCodeGenerationMemberAccess)), DummyStoreType))
        {
        }

        protected SapHanaCodeGenerationMemberAccessTypeMapping(RelationalTypeMappingParameters parameters)
            : base(parameters)
        {
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new SapHanaCodeGenerationMemberAccessTypeMapping(parameters);

        public override string GenerateSqlLiteral(object value)
            => throw new InvalidOperationException("This type mapping exists for code generation only.");

        public override Expression GenerateCodeLiteral(object value)
            => value is SapHanaCodeGenerationMemberAccess memberAccess
                ? Expression.MakeMemberAccess(null, memberAccess.MemberInfo)
                : null;
    }
}
