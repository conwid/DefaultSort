using System;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

namespace DefaultSort
{
    public class DefaultSortVisitor : DefaultExpressionVisitor
    {
        public override DbExpression Visit(DbScanExpression expression)
        {
            var table = (EntityType)expression.Target.ElementType;
            var binding = expression.Bind();
            var orderProps = table.Properties.Where(p => p.MetadataProperties.Any(mp => mp.Name == "http://schemas.microsoft.com/ado/2013/11/edm/customannotation:"+OrderConstants.OrderProperty)).ToList();
            if (orderProps.Count == 0)
            {
                return expression;
            }
            if (orderProps.Count > 1)
            {
                var propNames = string.Join(",", orderProps.Select(p => p.Name));
                throw new InvalidOperationException($"Multiple default sort properties specified: {propNames}");
            }

            var orderProp = orderProps.Single();
            var sortingColumn = orderProp.Name;
            var sortKey = binding.VariableType.Variable(binding.VariableName).Property(sortingColumn);
            var sortProps = orderProp.MetadataProperties.Where(mp => mp.Name == "http://schemas.microsoft.com/ado/2013/11/edm/customannotation:"+OrderConstants.OrderProperty).ToList();
            if (sortProps.Count > 1)
            {
                throw new InvalidOperationException($"Property {orderProp.Name} is specified more than once as a default sorting property");
            }
            var sortDir = (string)sortProps.Single().Value;
            Func<DbExpression, DbSortClause> sortClauseCreator = sortDir == OrderConstants.Ascending ? new Func<DbExpression, DbSortClause>(DbExpressionBuilder.ToSortClause) : sortDir == OrderConstants.Descending ? new Func<DbExpression, DbSortClause>(DbExpressionBuilder.ToSortClauseDescending) : throw new InvalidOperationException("Unknown sort order");
            var sortClause = sortClauseCreator(sortKey);
            return DbExpressionBuilder.Sort(binding, new[] { sortClause });
        }
    }
}
