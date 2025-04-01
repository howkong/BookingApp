using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingApp.Data.Entities
{
    public class FeatureEntity : BaseEntity
    {
        public string Title { get; set; }

        //  Relational Property
        public ICollection<HotelFeatureEntity> HotelFeatures { get; set; }
    }

    public class FeatureConfiguration : BaseConfiguration<FeatureEntity>
    {
        public override void Configure(EntityTypeBuilder<FeatureEntity> builder)
        {
            base.Configure(builder);
        }
    }
}
