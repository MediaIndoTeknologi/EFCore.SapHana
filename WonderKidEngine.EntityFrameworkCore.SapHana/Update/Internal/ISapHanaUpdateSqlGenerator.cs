using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Update;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Update.Internal
{
    public interface ISapHanaUpdateSqlGenerator : IUpdateSqlGenerator
    {
        ResultSetMapping AppendBulkInsertOperation(
            [NotNull] StringBuilder commandStringBuilder,
            [NotNull] IReadOnlyList<IReadOnlyModificationCommand> modificationCommands,
            int commandPosition);
    }
}
