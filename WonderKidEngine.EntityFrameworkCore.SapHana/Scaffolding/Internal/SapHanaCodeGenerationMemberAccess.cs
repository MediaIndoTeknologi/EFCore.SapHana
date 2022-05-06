using System.Reflection;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Scaffolding.Internal
{
    internal class SapHanaCodeGenerationMemberAccess
    {
        public MemberInfo MemberInfo { get; }

        public SapHanaCodeGenerationMemberAccess(MemberInfo memberInfo)
        {
            MemberInfo = memberInfo;
        }
    }
}
