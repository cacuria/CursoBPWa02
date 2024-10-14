using Microsoft.EntityFrameworkCore.Diagnostics;
using Serilog;
using System.Data.Common;

namespace Bdb.Curso.EntityFrameworkCore
{

    public class PerformanceInterceptor : DbCommandInterceptor
    {                  

        public PerformanceInterceptor()
        {

        }

        public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
        {
            if (eventData.Duration.TotalMilliseconds > 1)
            {
                LogLongQuery(command, eventData);
            }
            return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
        }

        public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
        {
            if (eventData.Duration.TotalMilliseconds > 2000)
            {
                LogLongQuery(command, eventData);
            }
            return base.ReaderExecuted(command, eventData, result);
        }

        private void LogLongQuery(DbCommand command, CommandExecutedEventData eventData)
        {
            Log.Information($"Long query: {command.CommandText}. Duration: {eventData.Duration.TotalMilliseconds} ms");


        }
    }


}
