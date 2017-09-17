using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;

namespace DefaultSort
{
    public static class DefaultSortExtensions
    {
        public static PrimitivePropertyConfiguration IsDefaultSort(this PrimitivePropertyConfiguration property)
        {
            property.HasColumnAnnotation(OrderConstants.OrderProperty, OrderConstants.Ascending);
            return property;
        }

        public static PrimitivePropertyConfiguration IsDefaultSortDescending(this PrimitivePropertyConfiguration property)
        {
            property.HasColumnAnnotation(OrderConstants.OrderProperty, OrderConstants.Descending);
            return property;
        }

        public static void UseDefaultSortProperties(this DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Add<DefaultSortConvention>();
        }
    }
}
