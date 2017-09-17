using System;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Reflection;

namespace DefaultSort
{
    public sealed class DefaultSortConvention : Convention
    {
        public DefaultSortConvention()
        {
            this.Properties()
                 .Having(x => x.GetCustomAttribute<DefaultSortAtrributeBase>(true))
                 .Configure((config, att) => ConfigureDefaultSort(config, att));

        }

        private void ConfigureDefaultSort(ConventionPrimitivePropertyConfiguration config, DefaultSortAtrributeBase att)
        {
            var attType = att.GetType();
            if (attType == typeof(DefaultSortPropertyAttribute))
            {
                config.HasColumnAnnotation(OrderConstants.OrderProperty, OrderConstants.Ascending);
            }
            else if (attType == typeof(DefaultDescendingSortPropertyAttribute))
            {
                config.HasColumnAnnotation(OrderConstants.OrderProperty, OrderConstants.Descending);
            }
            else
            {
                throw new InvalidOperationException($"Unknown order attribute type {attType.FullName}");
            }
        }
    }
}
