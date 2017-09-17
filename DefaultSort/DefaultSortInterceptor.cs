using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Interception;

namespace DefaultSort
{
    public class DefaultSortInterceptor : IDbCommandTreeInterceptor
    {
        public void TreeCreated(DbCommandTreeInterceptionContext interceptionContext)
        {
            if (interceptionContext.OriginalResult.DataSpace != DataSpace.SSpace)
            {
                return;
            }

            var queryCommand = interceptionContext.Result as DbQueryCommandTree;
            if (queryCommand != null)
            {
                interceptionContext.Result = HandleQueryCommand(queryCommand);
            }
        }

        private static DbCommandTree HandleQueryCommand(DbQueryCommandTree queryCommand)
        {
            var newQuery = queryCommand.Query.Accept(new DefaultSortVisitor());
            return new DbQueryCommandTree(
                queryCommand.MetadataWorkspace,
                queryCommand.DataSpace,
                newQuery);
        }

    }
}
