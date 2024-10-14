using Microsoft.EntityFrameworkCore.Diagnostics;
using Serilog;
using System.Data.Common;

namespace Bdb.Curso.EntityFrameworkCore
{
    public class CustomDbCommandInterceptor : DbCommandInterceptor
    {

        public override InterceptionResult<DbDataReader> ReaderExecuting(
       DbCommand command,
       CommandEventData eventData,
       InterceptionResult<DbDataReader> result)
        {

            ManipulateCommand(command);

            return result;
        }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default)
        {

            Log.Information("Executing command: {CommandText}", command.CommandText);

            ManipulateCommand(command);

            return new ValueTask<InterceptionResult<DbDataReader>>(result);
        }

        private static void ManipulateCommand(DbCommand command)
        {
            if (command.CommandText.StartsWith("-- Use hint: robust plan", StringComparison.Ordinal))
            {
                command.CommandText += " OPTION (ROBUST PLAN)";
            }
        }

    }

}
